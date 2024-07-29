using CustomUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//현재 씬에서 플레이어와 사운드 이벤트와 관련된 정보를 받는다.
//캐릭터와 몬스터의 이벤트를 확인한다.
public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public Transform GlobalSoundTarget { get; private set; }   // 현재 물건의 소리 이벤트에 반응하기 위함.
    public Transform LocalSoundTarget { get; private set; }


    //사운드 오브젝트 정보를 담아두는 변수
    List<ObjectInfo> objInfo = new List<ObjectInfo>();

    //악령의 위치좌표
    Ghost cs_Ghost;

    //영혼들
    [SerializeField]GameObject GhostWrap;
    [SerializeField]GameObject SoulWrap;
    //검은 악귀
    [SerializeField]GameObject go_GhostBlack;

    //카메라 장착에 대한 동작을 수행한다. //카메라 컨트롤러에서 호출된다.
    public Action<bool> Action_CameraEquip { get; set; }

    //몬스터가 플레이어 주변에 배회할 경우 실행되는 액션
    //몬스터와 가깝다 1 , 멀다 0을 반환
    public Action<float> Action_AroundMonster { get; set; }

    CameraShaking cameraShaking;                    //카메라 흔드는 효과

    public Action Action_CameraFlash { get; set; }  // 카메라 촬영 이벤트


    //승천 가능한 영혼을 저장
    List<Soul> list_Soul = new List<Soul>();

    MusicboxSpawn musicboxSpawn;

    private void Awake()
    {
        if (instance == null) instance = this;

        cameraShaking = FindObjectOfType<CameraShaking>();

        cs_Ghost = FindObjectOfType<Ghost>();

        musicboxSpawn= FindObjectOfType<MusicboxSpawn>();

    }
    private void OnDestroy()
    {
        Destroy(instance);
        instance = null;
    }
    //글로벌 사운드 리스트에 오브젝트가 존재하는지 판단
    bool FindObject(string Name)
    {
        foreach (var n in objInfo)
        {
            if (n.Name.Equals(Name)) { return true; }
        }
        return false;
    }

    public void DeActiveGhost()
    {
        go_GhostBlack.SetActive(false);
        GhostWrap.SetActive(false);
        SoulWrap.SetActive(false);
    }

    //전역사운드를 지정을 위해 지정한다.
    public void ActiveTheHightestPrioritySound(ObjectInfo obj)
    {
        // 기존 오브젝트에 존재하면 비활성화
        if (FindObject(obj.Name)) return;

        CancleLocalSoundDelay();//로컬 사운드 딜레이가 있을 경우 제거

        //이미 있을 경우 단순히 추가
        if (objInfo.Count > 0)
        {
            objInfo.Add(obj);
        }
        //아무것도 없을 경우 타겟설정
        else
        {
            GlobalSoundTarget = obj.Trans;
            objInfo.Add(obj);
        }
    }

    //글로벌 사운드를 꺼낸다.
    public void DeActiveTheHightestPrioritySound(ObjectInfo obj)
    {
        //Debug.Log($"사운드 오브젝트 비활성화 시작 {objInfo.Count}");

        //만약 오브젝트가 존재한다면, 같은 이름의 오브젝트를 제거한다.
        for (int i = 0; i < objInfo.Count; i++)
        {
            if (objInfo[i].Name.Equals(obj.Name))
            {
                objInfo.RemoveAt(i);
                //Debug.Log($"사운드 오브젝트 비활성화 완료 {objInfo.Count}");
                break;
            }
        }

        //오브젝트가 존재하면 다름 오브젝트를 타겟으로 지정한다.
        if (objInfo.Count > 0)
        {
            GlobalSoundTarget = objInfo[0].Trans;
            //Debug.Log("새로운 전역 사운드로 대체한다!!");
        }
        //저장된 전역 사운드가 없다면 지정을 해제한다.
        else
        {
            GlobalSoundTarget = null;
            //Debug.Log("새로운 전역 사운드로 없다!!!");
        }
    }

    #region------LocalSound--------
    IEnumerator EventSoundTimeout; // 타임아웃 코루틴 저장
    //소리에 따른 이벤트 발생지와 거리를 전달 받는다
    //전달받은 위치와 감지 거리를 기반으로 감지 거리가 몬스터가 사운드 발생범위 안에 있다면 몬스터에게 새로운 위치를 갱신해준다.
    //최종적으로 아이템과 몬스터의 거리가 _distace 매개변수보다 작다면, 새로운 Transform을 갱신받아 타겟을 지정하게 된다
    public bool EventSound(Transform _target, float _distance)
    {
        //글로벌 사운드가 존재할 경우 실행하지 않음.
        if (GlobalSoundTarget != null) return false;

        //기존 로컬 사운드 존재 시 실행하지 않음.
        //if (LocalSoundTarget) return false;

        //소리가 울려퍼지는 범위 _distance
        //소리가 발생한 지점은 _target
        if ((GetMonsterPosition() - _target.position).magnitude < (_distance* DataSet.Instance.GameDifficulty.SoundSensitivity))
        {
            //Debug.Log(string.Format($"EventSound :: 소리가 몬스터 감지범위에 탐지됩니다. {transform.name} :: {_distance}"));

            //사운드 타겟 지정
            LocalSoundTarget = _target;

            //이전 딜레이 제거 후 사운드 실행
            CancleLocalSoundDelay();
            EventSoundTimeout = EventSoundCroutine();
            StartCoroutine(EventSoundTimeout);

            //Debug.Log("EventManager :: 일시적인 사운드 이벤트 발생 입력 성공");

            return true;    // 성공
        }

        return false;// 실패
    }

    //로컬 사운드가 지정되었을 때, 딜레이 후 사운드를 제거합니다.
    IEnumerator EventSoundCroutine() // 이벤트 사운드 타겟 저장 시간 = 1초
    {
        yield return new WaitForSeconds(1);
        LocalSoundTarget = null;
        EventSoundTimeout = null;

        //Debug.Log("EventMAnager :: 사운드 근원지 TimeOut 초기화");
    }

    // 로컬 사운드의 좌표값을 얻어오며 참조를 제거한다.
    public Vector3 GetLocalSoundTarget()
    {
        //지정된 위치가 없다면, Zero값 반환
        if (LocalSoundTarget == null) return Vector3.zero;

        Vector3 target = LocalSoundTarget.position;
        LocalSoundTarget = null;

        CancleLocalSoundDelay();

        Debug.Log("EventMAnager :: 사운드 근원지 반환 및 초기화");

        return target;
    }

    //로컬사운드 딜레이 코루틴을 제거한다.
    void CancleLocalSoundDelay()
    {
        if (EventSoundTimeout != null) StopCoroutine(EventSoundTimeout);
        EventSoundTimeout = null;
    }
    #endregion


    #region ------------영혼 승천을 위한 저장--------------
    public void SaveAscentionSoul(in Soul _soul)
    {
        for (int i = 0; i < list_Soul.Count; i++)
        {
            if (list_Soul[i] == _soul)
            {
                Debug.Log("승천 가능한 영혼이 이미 존재합니다.");
                return;
            }
        }

        list_Soul.Add(_soul);
    }
    public void DeleteAscentionSoul(in Soul _soul)
    {
        for (int i = 0; i < list_Soul.Count; i++)
        {
            if (list_Soul[i] == _soul)
            {
                Debug.Log("승천가능 영혼 목록에서 제거합니다.");
                list_Soul.RemoveAt(i);
                return;
            }
        }
    }

    //들어온 입력에서 가장 거리값이 가까운 변수 반환합니다.
    public Soul GetSoul(Vector3 currentPosition)
    {
        if (list_Soul.Count < 1) { return null; }

        float minDistance = Mathf.Infinity;
        Soul _soul = null;
        for (int i = 0; i < list_Soul.Count; i++)
        {
            //카메라 내부에 존재한다.
            if (Calculator.ObjectInViewport(list_Soul[i].GetPosition() + new Vector3(0, 0.8f, 0)))
            {
                Debug.Log($"카메라 내부에 영혼이 위치합니다.");

                //가장 거리가 가까운 영혼을 가져온다.
                float dis = Vector3.Distance(currentPosition, list_Soul[i].GetPosition());
                if (dis < minDistance)
                {
                    minDistance = dis;
                    _soul = list_Soul[i];
                }
            }
        }
        return _soul;
    }
    #endregion


    public Vector3 GetMonsterPosition()
    {
        if (cs_Ghost && GhostWrap.activeSelf) return cs_Ghost.transform.position;
        else return new Vector3(0, Mathf.Infinity, 0);
    }

    //악귀와 영혼을 활성화
    public void ActiveSouls(Vector3 ghostPosition)
    {
        Debug.LogWarning("악귀와 영혼들이 활성화됩니다.");
        GhostWrap.SetActive(true);
        cs_Ghost.Activation(ghostPosition);
        SoulWrap.SetActive(true);
    } 

    public void DeactiveSouls()
    {
        Debug.LogWarning("악귀와 영혼들이 비활성화됩니다.");
        GhostWrap.SetActive(false);
        SoulWrap.SetActive(false);
        //검은 악귀를 비활성화합니다.
        go_GhostBlack.SetActive(false);
    }

    #region 카메라
    //카메라 흔들기
    public void CameraShake(float t, float force) { cameraShaking.Shake(t, force); }
    
    #endregion


    //오르골이 터짐을 알림
    public void MusicBoxDestroy()
    {
        musicboxSpawn.MusicBoxDestroy();
    }

    //현재 남아있는 영혼의 수 반환
    public int GetActivationSouls()
    {
        Debug.LogError($"EventManager : 현재 영혼 수를 반환합니다. {SoulWrap.transform.childCount}");
        return SoulWrap.transform.childCount;
    }

    //오르골 스포너 제거
    public void DestroyMusicboxSpawner()
    {
        Destroy(musicboxSpawn.gameObject); 
    }

    public void ActiveGhostBlack()
    {
        //고스트가 활성화가 안되고, 난이도가 쉬움이 아닌 경우에만 악귀 활성화
        if(!go_GhostBlack.activeSelf && DataSet.Instance.SettingValue.CurrentDiff != 0)
        {
            Debug.LogWarning("검은 악귀를 활성화합니다.");

            go_GhostBlack.SetActive(true);

            AudioManager.instance.PlayEffectiveSound("GhostBlack", 1.0f);

            //대사
            //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "ActiveBlack"), 4.0f);
            //UI.staticUI.ShowLine();

            AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.0f);

            if(DataSet.Instance.SettingValue.CurrentDiff > 0)
            {
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "ActiveBlack2"), false);
            }
        }
    }

    //영혼이 승천하면 속도가 증가합니다.
    public void AscendedSoul()
    {
        cs_Ghost.AscensededSoul();
    }
}
