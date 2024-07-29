using UnityEngine;
using CustomUI;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using Random = UnityEngine.Random;

using System.Collections;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 3.0f;
        [Tooltip("HoldOnBreathSpeed speed of the character in m/s")]
        public float HoldOnBreathSpeed = 0.5f;
        [Tooltip("Sit speed of the character in m/s")]
        public float SitSpeed = 1.5f;
        public float RunMultiply = 1.5f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 5.0f; // 기본적인 기준 민감도
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 80.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -80.0f;


        [Space]
        [Header("CameraMovementAnimtion")]
        public Animator AnimatorCameramove;
        public PlayerEventSound CS_FootStep;
        int[] hash_state = new int[]
        {
            Animator.StringToHash("Idle"),
            Animator.StringToHash("Walking"),
            Animator.StringToHash("Run"),
            Animator.StringToHash("Dead"),
            Animator.StringToHash("Speed"),
            Animator.StringToHash("Spawn"),
            Animator.StringToHash("Paint")
        };
        enum MovementState
        {
            Idle = 0,
            Walking,
            Run,
            Dead,
            Speed,
            Spawn,
            Paint
        }
        MovementState previous_state;   // 이전 상태를 저장함.
        bool jumped = false;// 점프를 했었음.


        [Space]
        [Header("Controller Height Value")]
        public float CharacterHeight = 2.0f;        //일어선 키
        public float CharacterSittingHeight = 0.8f; //앉은 높이

        [Header("Camera Position Value")]
        public Transform CameraWrap;                //카메라 조정 오브젝트
        public float camera_StandView = 1.6f;               //일어선 카메라 위치
        public float camera_SitView = 1.2f;                //앉은 카메라 위치

        [Header("PlayerCollsion about Ceiling")]
        public PlayerCollsion playerCollsion;       //천장 충돌 판정을 위함.


        [Space]
        [Header("Grab Value")]
        public Transform CameraRoot;
        public Transform GrabPoint;         //아이템을 잡아서 놓을 위치
        public float GrabDistance = 5.0f;  //아이템을 얻기 위한 판단
        public float GrabGetDistance = 1.0f;    //아이템이 얻어 졌을 경우
        public float GrabThrowForce = 20.0f;
        public float GrabTimeout = 0.3f;
        public LayerMask GrabLayer = 31;        //오브젝트 레이어로 설정

        [Space]
        [Header("Respawn Position")]
        public Transform RespawnPosition;


        ItemPoint itemPoint;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _itemTimeout;

        //sitting value
        private bool _sit = false;   //앉음을 판단 + 머리 위에 오브젝트가 있으면 못 일어선다.
        ///private bool _isPreviousSit = false; //이전 상태가 앉은 상태였는가?


        //Grab value
        float Grab_timeout = 0; //그랩을 위한 쿨타임


        //jump
        float _jump_y;  // 천장에 머리를 부딪힐 경우 점프가 공중에 계속 떠있는 문제점을 발견
        float _jump_y_offset = 0.001f;      // 천장에 부딪힐 경우의 업데이트 값에 대한 차이값 허용범위


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.001f;

        /*
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }
        */

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            AnimatorCameramove.SetBool(hash_state[0], true);
            previous_state = MovementState.Idle;
            AnimatorCameramove.SetFloat(hash_state[(int)MovementState.Speed], 1);

            itemPoint = GetComponentInChildren<ItemPoint>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;


            //Character Height Setting
            _controller.height = CharacterHeight;
            _controller.center = new Vector3(0, Mathf.Round(CharacterHeight / 2), 0);
            GroundedOffset = (_controller.height - CharacterHeight) * 0.5f;


            //CameraPostionSetting
            CameraWrap.localPosition = new Vector3(0, camera_StandView, 0);

            //Action Set
            //PlayerEvent.instance.Action_PlayerDead += Dead;

            PlayerEvent.instance.Action_Init += ()=>GrabOff(0.0f);
        }

        private void Update()
        {
            //Debug.LogError(_speed);
            //Debug.DrawRay(CameraRoot.transform.position, CameraRoot.transform.forward * 5, Color.red);

            //셋팅 창을 열겠다.
            Setting();

            if (PlayerEvent.instance.isDead || GameManager.Instance.DontMove || GameManager.Instance.FrozenGame)
            {
                //모든 입력을 초기화한다.
                _input.InitAll();
                return;
            }

            ////만약 인트로인 경우 게임의 입력값을 판단시킨다.
            //if (GameManager.Instance.IsIntro) { GameManager.Instance.SetIntroInputValue(); }

            GroundedCheck();
            JumpAndGravity();
            JudgeSit();         //앉음을 먼저 체크해야 된다. 앉음의 판단 변수를 따로 지정되기 때문
            Move();

            //카메라의 움직임을 설정하기 위함
            CameraMovement();
            // 머리 위에 충돌체가 있는 경우 바로 추락시킴.
            CheckJumpCollsion();

            //물건을 잡기 위함
            Grab();
            //물건과의 상호작용
            GrabFuntion();
            //휠 입력값을 판단하기 위함.
            WheelInput();

            //아이템 관련 동작
            ItemEquip();
        }

        private void LateUpdate()
        {
            if (PlayerEvent.instance.isDead || GameManager.Instance.DontMove) return;
            CameraRotation();
        }


        //아이템 장착을 판단합니다.
        void ItemEquip()
        {
            //손전등
            if (_input.flash)
            {
                PlayerEvent.instance.FlashLightEquip(true);
                _input.flash = false;
            }

            //카메라 사용
            if (_input.cameraView)
            {
                PlayerEvent.instance.CameraEquip();

                _input.cameraView = false;
            }

            //TIP 표시
            if (_input.tap)
            {
                short number = (short)(itemPoint.EquipItemNumber() - 1);

                //오르골을 가지고 있을 경우
                if (_interactionGrabObject)
                {
                    //오르골인 경우
                    if (_interactionGrabObject.name.Contains("MusicBox"))
                    {
                        number = 2;
                    }
                }

                //장착하고 있는 도구가 있다.
                if(number > -1)
                {
                    //아이템이 있는 경우 떨어트립니다.
                    GrabOff(0.0f);

                    PlayerEvent.instance.ShowVideo((VideoType)number);
                    //Debug.Log($"카메라로 비디오 시청 : {number}");
                }
                else
                {
                    //비디오를 보고 있지 않은 경우 목표를 다시 보여준다.
                    if (!PlayerEvent.instance.showVideo)
                    {
                        UI.topUI.ReShowNotice();
                    }
                    //비디오를 보고 있으면 끈다.
                    else
                    {
                        PlayerEvent.instance.ShowVideo(VideoType.None);
                    }

                    //Debug.Log("비디오 시청을 종료합니다");
                }

                _input.tap = false;
            }

            _itemTimeout -= Time.deltaTime;
            if (_itemTimeout < 0.00001f)
            {
                //미터기
                if (_input.numOne)
                {
                    if (!GameManager.Instance.DontUseGhostMeter)
                    {
                        //Debug.Log("아이템을 사용합니다. GhostMeter");

                        itemPoint.Equip(1);

                        _itemTimeout = 0.1f;
                    }

                    _input.numOne = false;
                }
                
                //보주
                if (_input.numTwo)
                {
                    if (!GameManager.Instance.DontUseGhostBall)
                    {
                        //Debug.Log("아이템을 사용합니다. GhostChrystal");

                        itemPoint.Equip(2);

                        _itemTimeout = 0.1f;
                    }
                    _input.numTwo = false;
                }
            }
        }

        void Setting()
        {
            //씬을 로딩하고 있을 경우에는 실행하지 않습니다.
            if (SceneLoader.sceneInformation.SceneLoading) return;

            //게임이 일시정지일 경우 정지 요소를 제거합니다.
            if (GameManager.Instance.FrozenGame)
            {
                if (_input.funtion || _input.esc)
                {
                    GameManager.Instance.FrozenOffGame();
                }
                _input.funtion = false;
                _input.esc = false;
            }
            else
            {
                // 셋팅창을 띄웁니다.
                if (_input.esc)
                {
                    if (!PlayerEvent.instance.isDead)
                    {
                        UI.topUI.ShowSetting();
                    }
                    _input.esc = false;
                }
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold && !GameManager.Instance.FrozenGame) // 마우스 입력 값이 최소값을 넘는경우
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = Time.deltaTime;// = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                float _rotationSpeed = RotationSpeed * DataSet.Instance.SettingValue.MouseSensitivity * 300;

                _cinemachineTargetPitch += _input.look.y * _rotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * _rotationSpeed * deltaTimeMultiplier;

                if(GameConfig.IsPc()) _input.look *=0.5f;
                else _input.look *= 0.3f;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }


        private void Move()
        {
            // 캐릭터 속도를 해당하는 키를 눌렀을 경우로 반영해 준다.
            float targetSpeed = (_sit ? (PlayerEvent.instance.CanHoldOnBreath ? HoldOnBreathSpeed :SitSpeed)
                : _input.run? (MoveSpeed * RunMultiply) : MoveSpeed);

            //Debug.LogWarning($"Speed : {targetSpeed}");


            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero && !_input.holdOnBreath) targetSpeed = 0.0f;

            JudgePlayerEvent(targetSpeed);

            // 현재 컨트롤러의 속도를 참조한다.
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            //오차범위 스피드를 지정
            float speedOffset = 0.1f;
            //입력된 키보드의 입력 크기를 넘겨받는다.
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // 상황에 따라 속도 증가와 감속을 이어간다.
            if (currentHorizontalSpeed < targetSpeed - speedOffset  // 현재 속도가 타겟속도보다 작다.
                || currentHorizontalSpeed > targetSpeed + speedOffset)  //현재 속도가 타겟속도보다 크다.
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }


            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            Vector3 v = inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

            // 실질적인 이동
            _controller.Move(v);
        }

        //플레이어의 동작에 따라 소리를 판단해서 보내준다.
        void JudgePlayerEvent(float targetSpeed)
        {
            if (!Grounded) return;  // 바닥이 아닌 경우 실행되지 않는다.

            //Debug.LogWarning($"Speed2 : {targetSpeed}"); 

            //이벤트에 따른 값을 전달한다.
            if (targetSpeed.Equals(0.0f)) { PlayerEvent.instance.PlayerEventSound(EnumPlayerEvent.Idle); }
            else if (targetSpeed.Equals(HoldOnBreathSpeed)) { PlayerEvent.instance.PlayerEventSound(EnumPlayerEvent.HoldOnBreath); }
            else if (targetSpeed.Equals(SitSpeed)) { PlayerEvent.instance.PlayerEventSound(EnumPlayerEvent.Sit); }
            else if (targetSpeed.Equals(MoveSpeed)) { PlayerEvent.instance.PlayerEventSound(EnumPlayerEvent.Walk); }
            else if (targetSpeed.Equals(MoveSpeed * RunMultiply)) { PlayerEvent.instance.PlayerEventSound(EnumPlayerEvent.Run); }
            else { PlayerEvent.instance.PlayerEventSound(EnumPlayerEvent.Jump); }
        }

        float delta_time = 10.0f;
        float animSpeed = 1.0f;
        //플레이어가 앉아 있는 경우를 판단한다.
        void JudgeSit()
        {
            //앉는키를 눌렀다.
            _sit = _input.holdOnBreath;


            Debug.Log($"JudgeSit0 {_sit}");

            // 현재 천장이 존재할 경우 강제로 앉기
            if (playerCollsion.CeilingCollsion)     
            {
                _sit = true;
            }

            Debug.Log($"JudgeSit {_sit}");

            //그냥 싯, 홀드온 씻, 스탠드
            if (_sit)
            {
                //숨참기가 가능한 경우
                if (PlayerEvent.instance.CanHoldOnBreath)
                {
                    if (animSpeed != 0.3f)  // 앉은 상태일 경우 발소리와 애님 속도를 줄인다.
                    {
                        animSpeed = 0.3f;
                        AudioManager.instance.SetFootVolume(0.0f);
                        AnimatorCameramove.SetFloat(hash_state[(int)MovementState.Speed], animSpeed);
                    }
                }
                //일반적인 앉기의 경우
                else
                {
                    if (animSpeed != 0.7f)  // 앉은 상태일 경우 발소리와 애님 속도를 줄인다.
                    {
                        animSpeed = 0.7f;
                        AudioManager.instance.SetFootVolume(0.5f);
                        AnimatorCameramove.SetFloat(hash_state[(int)MovementState.Speed], animSpeed);
                    }
                }

                // 캐릭터의 height를 변경한다.
                if (Mathf.Abs(_controller.height - CharacterSittingHeight) < 0.01f) return;

                _controller.height = Mathf.Lerp(_controller.height, CharacterSittingHeight, Time.deltaTime * delta_time);
                GroundedOffset = (_controller.height - CharacterHeight) * 0.5f;

                float _view = Mathf.Lerp(CameraWrap.localPosition.y, camera_SitView, Time.deltaTime * delta_time);
                CameraWrap.localPosition = new Vector3(0, _view, 0);
            }
            else
            {
                animSpeed = _speed / MoveSpeed;
                AnimatorCameramove.SetFloat(hash_state[(int)MovementState.Speed], animSpeed);

                //기존 속도와 맞지 않으면 더욱 빠른 속도감을 느끼도록 한다.
                if (_input.run)
                    AudioManager.instance.SetFootVolume(1.2f);
                else
                    AudioManager.instance.SetFootVolume(1.0f);


                if (Mathf.Abs(_controller.height - CharacterHeight) < 0.01f) return;

                float lastHeight = _controller.height;
                _controller.height = Mathf.Lerp(_controller.height, CharacterHeight, Time.deltaTime * 8f);
                Vector3 tmpPosition = transform.position;
                tmpPosition.y += (_controller.height - lastHeight);
                transform.position = tmpPosition;

                GroundedOffset = (_controller.height - CharacterHeight) * 0.5f;

                float _view = Mathf.Lerp(CameraWrap.localPosition.y, camera_StandView, Time.deltaTime * delta_time);
                CameraWrap.localPosition = new Vector3(0, _view, 0);
            }
        }



        InteractionObjects _interactionGrabObject = null;   // 손에 쥐고 있는 오브젝트
        InteractionObjects _interactionRay = null;          //레이 발사해서 현재 앞에 오브젝트의 존재를 나타냄.
        RaycastHit hit;
        float rayTimeOut;   //레이캐스트 연산 타임아웃 시간
        bool isExpand = false;
        void Grab()		//물건을 잡는다.
        {
            if (rayTimeOut >= 0) { rayTimeOut -= Time.deltaTime; }
            else
            {
                //Debug.DrawRay(CameraRoot.position, CameraRoot.forward * GrabDistance, Color.yellow);

                _interactionRay = null;// 감지된 오브젝트가 없음을 나타냄.

                if (Physics.Raycast(CameraRoot.position, CameraRoot.forward, out hit, GrabDistance, GrabLayer))
                {
                    //기존에 감지된 물건이 없는 상태라면, 감지시킨다.
                    if (DataSet.Instance.Layers.IsObject(hit.transform.gameObject.layer))
                    {
                        _interactionRay = hit.transform.GetComponent<InteractionObjects>();
                        //Debug.Log($"{hit.transform.name} 레이 물체 감지");

                        if (_interactionRay)
                        {
                            if (_interactionGrabObject != null && _interactionRay == _interactionGrabObject) { _interactionRay = null; }
                            //확장형 오브젝트인 경우 상호작용이 가능한지 판단한다.
                            else if(_interactionRay.GetInteractionType().Equals(InteractionType.Expand)) isExpand = ((ExpandObject)_interactionRay).IsInteract(_interactionGrabObject?.NAME);
                        }
                    }

                }
                rayTimeOut = 0.05f;
            }

            //Ui Interative 표시
            if (_interactionRay)
            {
                UI.activityUI.SetInteractionUI(_interactionRay.GetPivotPosition(), isExpand, (short)_interactionRay.GetInteractionType());
                if (UI.mobileControllerUI != null)
                {
                    UI.mobileControllerUI.ShowFunc(true);
                }
            }
            else
            {
                UI.activityUI.SetInteractionUI(Vector3.zero); 
                if (UI.mobileControllerUI != null)
                {
                    UI.mobileControllerUI.ShowFunc(false);
                }
            }

            //아이템을 집는다. [마우스 좌측]
            if (_input.Grab && (Grab_timeout <= 0))
            {
                //카메라를 장착하고 있다면, 카메라의 플래쉬를 터트린다.
                if (PlayerEvent.instance.cameraEquipState) 
                {
                    //눈 앞에 오브젝트가 존재하는 경우 
                    if (_interactionRay)
                    {
                        //아이템을 잡으려면 카메라를 꺼야됩니다.
                        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "WithoutCamera"), false);
                    }
                    //눈 앞에 오브젝트가 존재하지 않는 경우 사진을 찍습니다.
                    else
                    {
                        PlayerEvent.instance.CameraTakePicture();
                    }
                }
                //그랩된 오브젝트를 손에서 놓습니다.
                else
                //현재 가지고 있는 물건이 있는 경우 물건을 놓는다.
                if (_interactionGrabObject)
                {
                    GrabOff(0.0f);
                }
                //가지고 있는 물건이 없는 경우
                else
                if (_interactionRay && !PlayerEvent.instance.cameraEquipState) //현재 앞에 물건이 있는 경우
                {
                    //비디오를 시청하고 있을 경우 비디오를 끕니다.
                    if (PlayerEvent.instance.showVideo)
                    {
                        PlayerEvent.instance.ShowVideo(VideoType.None);
                    }

                    if (_interactionRay.GrabOn(GrabPoint))//잡을 수 없는 것이라면, false를 반환한다.
                    {
                        //그랩 시 드롭으로 변경
                        if(UI.mobileControllerUI != null)
                        {
                            UI.mobileControllerUI.ClickStateSet(MobileControllerUI.MobileClickState.Drop);
                        }

                        _interactionGrabObject = _interactionRay;
                    }
                }

                Grab_timeout = GrabTimeout;

                _input.GrabInput(false);
            }
            //아이템을 던진다. [마우스 우측]
            else
            if (_input.throwing && (Grab_timeout <= 0))//마우스 우클릭을 한다면, 오브젝트를 던진다.
            {
                if (_interactionGrabObject)//현재 가지고 있는 오브젝트가 있는 경우
                {
                    GrabOff(GrabThrowForce);
                }


                _input.ThrowInput(false);
            }

            if (Grab_timeout > 0)
            {
                if (GameManager.Instance.FrozenGame) Grab_timeout = 0;
                Grab_timeout -= Time.deltaTime;
            }
        }

        //그랩을 해제합니다.
        public void GrabOff(float power)
        {
            Grab_timeout = GrabTimeout;                    //그랩시간 딜레이를 준다.

            //잡고 있는게 없다면 리턴
            if (_interactionGrabObject == null) return;

            //그랩을 해제가 가능한 물건이라면 그랩을 해제한다.
            if (_interactionGrabObject.GrabOff(new Vector3(transform.position.x, CameraWrap.position.y - 0.2f, transform.position.z) + CameraWrap.forward * 0.3f, CameraRoot.forward * power))
            {
                _interactionGrabObject = null;
            }

            //모바일 상태 변경
            if(UI.mobileControllerUI != null)
            {
                UI.mobileControllerUI.ClickStateSet(MobileControllerUI.MobileClickState.Grab);
            }
        }


        float timeOutFunc; //Func를 빠르게 누르면 오류가 발생하기 때문에 적용해줌.
        void GrabFuntion()	// Func 키를 눌렀을 때 아이템을 활성화 하는 것.
        {
            if (timeOutFunc > 0.0f)
            {
                timeOutFunc -= Time.unscaledDeltaTime;
                _input.funtion = false;
                return;
            }

            if (_input.funtion)//F키를 눌러 상호작용을 했다.
            {
                timeOutFunc = 0.1f;

                if (_interactionGrabObject)				// 손에 오브젝트가 있는 경우
                {
                    if (_interactionRay)    //현재 앞에 레이로 감지되는 오브젝트가 있음
                    {
                        if (_interactionRay.GetInteractionType().Equals(InteractionType.Expand))    //해당 오브젝트가 상호작용형임
                        {
                            //상호작용합니다.
                            if (_interactionRay.Func(_interactionGrabObject.NAME))	
                            {
                                //Debug.Log("플레이어 컨트롤러 상호작용에 성공");
                            }
                            else
                            {
                                //Debug.Log("플레이어 컨트롤러 상호작용에 실패");
                                WrongFuncFeedback();
                            }
                        }
                        else					//상호작용형 오브젝트가 아님. => 레이에 감지된 오브젝트 실행.
                        {
                            if (!_interactionRay.Func())//상호작용에 실패한다면?
                            {
                                WrongFuncFeedback();
                            }
                        }
                    }
                    else					//레이에 감지된 오브젝트가 없음. // 손에 들고 있는 아이템 실행.
                    {
                        if (_interactionGrabObject.Func()) GrabOff(0);
                        else
                        {
                            WrongFuncFeedback();
                        }
                    }
                }
                else		// 손에 오브젝트가 없는 경우 => 레이에 감지된 오브젝트를 사용한다.
                {
                    if (_interactionRay && !_interactionRay.GetInteractionType().Equals(InteractionType.None)) //비활성화가된 오브젝트가 아닐 경우에만 실행시킨다.
                    {
                        if (_interactionRay.GetInteractionType().Equals(InteractionType.Expand))// && !_interactionRay.GetInteractionType().Equals(InteractionType.Expand))	// 상호작용형 오브젝트가 아닐때 활성
                        {
                            if (!_interactionRay.Func("")) WrongFuncFeedback();
                        }
                        else
                        {
                            if (!_interactionRay.Func()) WrongFuncFeedback();
                        }
                    }
                }

                _input.funtion = false;
            }
        }

        void WrongFuncFeedback()
        {
            AudioManager.instance.PlayUISound("Beep", 1.0f);
            EventManager.instance.CameraShake(0.2f, 0.01f);
        }

        void WheelInput()
        {
            if (_input.wheelValue > 0.1f || _input.wheelValue < -0.1f)
            {
                //Debug.Log("휠 입력값이 들어옴");
                if (_interactionRay && _interactionRay.GetInteractionType().Equals(InteractionType.Continues))
                {
                    //Debug.Log("문을 조작합니다");
                    _interactionRay.Func(_input.wheelValue / 70.0f);
                }
            }
        }

        void CameraMovement()
        {
            if (!Grounded)  // 바닥이 아닐 경우
            {
                jumped = true;
            }
            else            //바닥에 있는 경우
            {
                if (jumped)
                {
                    CS_FootStep.FootJump();
                    JudgePlayerEvent(-1f);
                    jumped = false;
                }
            }

            if (_input.move != Vector2.zero)    // 플레이어가 움직이고 있는 경우
            {
                if (!Grounded)
                {
                    CameraMovementState(MovementState.Idle);
                }
                else if (_input.run)
                {
                    CameraMovementState(MovementState.Run);
                }
                else 
                {
                    CameraMovementState(MovementState.Walking);
                }
            }
            else
            {
                CameraMovementState(MovementState.Idle);
            }
        }

        void CameraMovementState(MovementState state)
        {
            if (previous_state == state) return;

            //파라미터가 Bool일 경우에만 실행합니다.

            AnimatorCameramove.SetBool(hash_state[(int)previous_state], false);
            AnimatorCameramove.SetBool(hash_state[(int)state], true);
            previous_state = state;
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // 바닥에 있을 때 y 축 값이 지속적으로 하락하는 것을 방지한다.
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -3f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && !playerCollsion.CeilingCollsion && (_verticalVelocity <= 0.01f))
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    // reset the jump timeout timer
                    _jumpTimeoutDelta = JumpTimeout;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            //}
            //else
            //{
            //    // 바닥이 아닐 경우 점프가 이루어지지 않는다.
            //    if(!WireBox.Active) _input.jump = false;
            //}

            //최고 Y 속도 아래에 점프값이 존재하는 경우 중력을 가한다.
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            //이전 위치 높이 값을 저장한다.
            _jump_y = transform.position.y;
        }

        //Jump와 Move에 대한 판단이 이루어진 상태에서 이루어지며, 천장과 부딪혀 Y축 위치값이 변화하지 못하고 있을 경우 이루어진다.
        void CheckJumpCollsion()
        {
            if (_verticalVelocity > 0.0f)
            {
                //다음 위치값을 추상적으로 비교하여 y에 대한 힘을 조정한다.
                float delta_t = (_jump_y + _verticalVelocity * Time.deltaTime) - transform.position.y;
                if (delta_t > _jump_y_offset)	// 충돌이 발생하였다.
                {
                    _verticalVelocity = 0.0f;

                    //Debug.Log("천장과 충돌이 발생하였습니다.");
                }
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        #region DeadScene/////////////////////////////////////
        //외부 참조 함수
        /*public void Dead()  // 죽었을 경우 호출되는 함수
        {
            Debug.LogWarning("---------------Player Dead-------------------");

            //기본 값 설정
            SetDefaultTransform();

            //플레이어가 죽는 애니메이션
            AnimatorCameramove.SetBool(hash_state[(int)previous_state], false);
            AnimatorCameramove.SetTrigger(hash_state[(int)MovementState.Dead]);

            Spawn(3.5f, null);
            Fade.FadeSetting(true,3.5f * 1.1f, Color.black);
        }*/

        public void Faint(Action action = null)// 졸도 효과
        {
            Debug.LogWarning("---------------Player Paint-------------------");

            //플레이어를 기본적인 위치로 초기화
            SetDefaultTransform();

            //기절 애니메이션 실행
            AnimatorCameramove.SetBool(hash_state[(int)previous_state], false);
            AnimatorCameramove.SetTrigger(hash_state[(int)MovementState.Paint]);

            Spawn(1.1f, action);
            Fade.FadeSetting(true,1.0f, Color.black);
        }


        //일어나는 모션
        public void Spawn(float delayTime, Action action)
        {
            StartCoroutine(SpawnCroutine(delayTime, action));
        }

        //플레이어가 되살아나도록 하는 코루틴
        public IEnumerator SpawnCroutine(float deadTime, Action action)
        {
            Debug.LogWarning("---------------Player Spawn-------------------");

            yield return new WaitForSeconds(deadTime + 2.0f);

            EventManager.instance.Action_AroundMonster(0);

            //리스폰 위치 선정과 카메라 각도, 애니메이션 초기화
            transform.position = RespawnPosition.position;
            CameraWrap.rotation = Quaternion.Euler(Vector3.zero);
            AnimatorCameramove.SetTrigger(hash_state[(int)MovementState.Spawn]);


            //화면가린것을 제거합니다.
            Fade.FadeSetting(false, 3.0f, Color.black);

            //애니메이션 시간만큼 대기한다.
            yield return new WaitForSeconds(3.0f);

            //애니메이션 상태를 변경한다.
            previous_state = MovementState.Walking;   
            CameraMovementState(MovementState.Idle);

            if (action != null) { action(); }

            //기본 움직임값 초기화
            _input.look = Vector2.zero;
            _input.move = Vector2.zero;

            //**다시 조작이 가능하도록하는 필수 요소
            PlayerEvent.instance.isDead = false;

            //애니메이션 시간만큼 대기한다.
            yield return new WaitForSeconds(1.0f);

            //손전등을 킵니다.
            PlayerEvent.instance.FlashLightEquip(true);

            //사운드 Groan
            //AudioManager.instance.PlayEffectiveSound($"Groan{Random.Range(0, 3)}", 1.0f);

            if (!GameConfig.IsPc())
            {
                UI.mobileControllerUI.SetActive(true);
            }
        }

        //애니메이션 수행을 위한 카메라 상태로 변경합니다.
        public void SetDefaultTransform()
        {
            //일어선 상태로 변경한다.
            CameraWrap.localPosition = new Vector3(0, camera_StandView, 0);
            _controller.height = CharacterHeight;

            //애니메이션 상태 변경
            CameraMovementState(MovementState.Idle);

            //카메라 장착 해제
            PlayerEvent.instance.SetCameraUnEquip();

            //손에 쥔 물건이 있는 경우 떨어트린다.
            if (_interactionGrabObject)
            {
                GrabOff(0.0f); // 그랩 해제
            }
        }
        #endregion


        public Vector2 GetRotationCameraView()
        {
            return new Vector2(CinemachineCameraTarget.transform.localEulerAngles.x, transform.eulerAngles.y);
        }

        //애니메이션 Idle로 변경
        public void AnimationIdle()
        {
            CameraMovementState(MovementState.Idle);
        }

        //카메라가 바라보는 회전값 설정
        public void SetCameraView(Vector2 rotation)
        {
            _input.look = Vector2.zero;
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        }

        //현재 잡고 있는 아이템을 제거합니다.
        public void DestroyGrabedItem()
        {
            //오브젝트 삭제하고 그랩을 해제합니다.
            Destroy(_interactionGrabObject.gameObject);
            GrabOff(0.0f);
        }

        public void SetPosition(Vector3 position)
        {
            _controller.enabled = false;
            transform.position = position;
            _controller.enabled = true;
        }
    }
}