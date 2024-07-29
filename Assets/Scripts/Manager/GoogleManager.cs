using UnityEngine;
using System;
using System.Collections;
using TMPro;

#if UNITY_ANDROID
using Google.Play.Common;
using Google.Play.AppUpdate;
#endif



#if UNITY_ANDROID
using GooglePlayGames;
#endif

/// <summary>
/// 구글 연결을 관리합니다.
/// 게임 시작 시 최초로 생성되며, 게임 모드가 모바일일 경우에만 실행됩니다.
/// 네트워크에 연결되어 있는 상태를 판단해 게임 진행시킵니다.
/// </summary>
/// 
public class GoogleManager : MonoBehaviour
{

#if UNITY_ANDROID

    public static GoogleManager instance;

    //광고를 로딩하기 위한 딜레이 로딩
    public GameObject go_Delay;

    public TMP_Text txt;

    private GoogleAds Admob;


    //네트워크와 구글 연결상태 관리
    public bool Connected
    {
        get { return CheckNetworking(); }
    }


    //구글 로그인 상태를 관리합니다.
    private bool logind = false;
    private bool checkedUpdate = false;


    private void Start()
    {
        //피시일 경우 제거합니다.
        if (GameConfig.IsPc())
        {
            Debug.LogWarning("GoogleManager 구글 매니저를 제거");
            Destroy(this.gameObject);
            Fade.FadeSetting(false, 0.2f, Color.black);
        }
        else
        {
            //현재 존재하는 오브젝트를 삭제
            if (instance != null)
            {
                Debug.LogWarning("GoogleManager 구글 매니저를 제거");
                Destroy(this.gameObject);
                return;
            }

            Debug.LogWarning("GoogleManager 구글 매니저를 생성");

            txt.enabled = false;
            Admob = new GoogleAds();
            instance = this;
            //Frame.SetActive(false);
            DontDestroyOnLoad(this.gameObject);

            AppUpdateCheck();
        }
    }

    void Login()
    {
        Debug.LogWarning("GoogleManager 플레이어 로그인 시도");

        Social.localUser.Authenticate((success) =>
        {
            if (success)
            {
                logind = true;
                Debug.LogWarning("GoogleManager 플레이어 로그인 성공 업데이트 확인");
            }
            else
            {
                Debug.LogWarning("GoogleManager 플레이어 로그인 실패");
            }
        });
    }

    //네트워크 연결상태를 판단합니다.
    bool CheckNetworking()
    {
        ////로그인이 안되어 있는 경우 
        //if (!logind)
        //{
        //    Login();
        //    return false;
        //}

        //네트워크에 연결되어있지 않는 경우 false
        return !Application.internetReachability.Equals(NetworkReachability.NotReachable);
    }
    
    //광고를 보여줍니다.
    public bool ShowAd(Action action)
    {
        Debug.LogWarning("GoogleManager 광고를 보여줍니다.");


        //캔버스에 네트워크가 연결되지 않았음을 나타낸다.
        if (!Connected)
        {
            Debug.LogWarning("GoogleAds 네트워크가 없습니다.");
            ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "Connecting"));

            return false;
        }

        //업데이트를 체크하지 않았습니다.
        if (!checkedUpdate)
        {
            AppUpdateCheck();
            ShowNotice("Checking Update Please...");

            return false;
        }

        go_Delay.SetActive(true);
        
        //광고가 있는지 판단합니다.
        if (Admob.ShowRewardedAd())
        {
            Debug.LogWarning("GoogleManager 광고가 존재합니다.");

            if (action != null)
            {
                StartCoroutine(RewardDelay(action));
            }
        }
        else
        {
            Debug.LogWarning("GoogleManager 광고가 없습니다.");

            StartCoroutine(CallRewardDelay(action));
        }

        return true;
    }
    
    //광고가 존재하는 경우 실행되는 동작
    IEnumerator RewardDelay(Action action)
    {
        Debug.LogWarning("GoogleManager 광고가 종료 대기 시작-------------");

        //광고가 끝날 때까지 대기
        yield return new WaitUntil(() => RewardEnd());

        Debug.LogWarning("GoogleManager 광고가 종료 대기 종료-------------");

        if (RewardFailed())
        {
            Debug.LogWarning("GoogleManager 광고 실패.--------------");

            StartCoroutine(CallRewardDelay(action));
        }
        else
        {
            Debug.LogWarning("GoogleManager 광고가 끝나 지정된 이벤트를 실행합니다.--------------");
            action();
            Admob.RequestAndLoadRewardedAd();
            go_Delay.SetActive(false);
        }

    }

    //광고가 없는 경우 불러오고 광고를 실행합니다.
    IEnumerator CallRewardDelay(Action action)
    {
        //광고 불러오기
        Admob.RequestAndLoadRewardedAd();

        //광고 로드를 대기합니다.
        float time = 0;
        do
        {
            time += Time.unscaledDeltaTime;
            if (Admob.GetIsADLoaded())
            {
                //광고로드 완료
                Debug.LogWarning("GoogleManager 광고 로드 성공.--------------");
                break;
            }

            yield return null;

        } while (time < 10.0f);


        //광고 존재 유무에 따라 실행
        if (Admob.GetIsADLoaded())
        {
            ShowAd(action);
        }
        else
        {
            if(action!=null)action(); 
            go_Delay.SetActive(false);
        }
    }

    /// <summary>
    /// ////////////////////////////////////////////////
    /// </summary>
    /// <returns></returns>
    //광고가 종료되었는지 판단
    public bool RewardEnd()
    {
        return Admob.isRewardEnd;
    }
    //광고가 종료되었는지 판단
    public bool RewardFailed()
    {
        return Admob.isFailed;
    }


    IEnumerator showNoticeCroutine;
    //필요한 내용의 안내문을 보여줍니다.
    public void ShowNotice(string t, float time = 1.0f)
    {
        txt.enabled = true;
        txt.text = t;

        if (showNoticeCroutine != null)
        {
            StopCoroutine(showNoticeCroutine);
        }
        showNoticeCroutine = FadeTextCroutine(time);
        StartCoroutine(showNoticeCroutine);
    }
    IEnumerator FadeTextCroutine(float time)
    {
        float t = time;
        Color c = txt.color;
        do
        {
            t -= Time.unscaledDeltaTime;
            c.a = t;
            txt.color = c;

            yield return null;

        } while (t > 0.0f);

        txt.enabled = false;

        showNoticeCroutine = null;
    }

    /// <summary>
    /// 앱의 업데이트를 확인합니다.
    /// </summary>
    /// 
    AppUpdateManager appUpdateManager;
    public void AppUpdateCheck()
    {
        //안드로이드 일 경우에만 업데이트를 호출합니다.
        if(Application.platform == RuntimePlatform.Android)
        {
            appUpdateManager = new AppUpdateManager();
            Debug.Log("GoogleManager 앱 업데이트를 확인을 시작합니다.");
            StartCoroutine(CheckForUpdate());
        }
        else
        {
            checkedUpdate = true;
        }
    }

    //앱에 적용 가능한 업데이트가 있는지 확인합니다.
    IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        go_Delay.SetActive(true);

        // 앱정보 확인 작업
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            //업데이트 가능 상태
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                //즉시 업데이트
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

                var startUpdateRequest = appUpdateManager.StartUpdate( appUpdateInfoResult, appUpdateOptions);

                while (!startUpdateRequest.IsDone)
                {
                    if(startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        Debug.Log("GoogleManager 앱 다운 중");
                    }
                    else if(startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        Debug.Log("GoogleManager 앱 다운 완료");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Canceled)
                    {
                        Debug.Log("GoogleManager 앱 업데이트 취소");
                        Application.Quit();
                    }

                    yield return null;
                }

                checkedUpdate = true;

                var result = appUpdateManager.CompleteUpdate();

                //완료 확인
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            else if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("GoogleManager 업데이트 없음 , 로그인 시도");
                checkedUpdate = true;
                Login();
                yield return (int)UpdateAvailability.UpdateNotAvailable;
            }
            else
            {
                Debug.Log("GoogleManager 업데이트 가능 여부를 알 수 없음");
                checkedUpdate = false;
                yield return (int)UpdateAvailability.Unknown;
            }
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
            Debug.Log("GoogleManager 업데이트 오류 " + appUpdateInfoOperation.Error);
            checkedUpdate = false;
            ShowNotice("GoogleManager UpdateError " + appUpdateInfoOperation.Error, 3.0f);
        }

        go_Delay.SetActive(false);
    }
#endif
    /*
    #region Save / Load
    ISavedGameMetadata myGame;//게임 저장 정보를 가져올 메타 데이터

    string myData_String;
    byte[] myData_Binary;


    //////////
    // SAVE //
    //////////
    public void Save(string d)
    {
        Debug.LogError($"GOOGLE Save{myGame.Description} / {myGame.Filename}");
        SaveData(myGame, d);
    }

    void SaveData(ISavedGameMetadata game, string data)             //메타 데이터를 지정하여 
    {
        myData_Binary = Encoding.UTF8.GetBytes(data);               //String을 Byte 타입으로 엔코딩

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build(); //저장을 위한 객체 빌드
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, myData_Binary, SaveCallBack);//데이터 저장
    }

    private void SaveCallBack(SavedGameRequestStatus status, ISavedGameMetadata game)   //Save 콜백 함수
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.LogWarning("GOOGLE Success Save");
            Load();
        }
        else
        {
            Debug.LogError($"GOOGLE Save Failed{game.Description} / {myGame.Filename}");
        }
    }


    //////////
    // LOAD //
    //////////

    public void Load()
    {
        Debug.LogWarning("DataLoad 1");
        isLoaded = false;
        //파일명을 넘겨주면서 저장되어 있는 게임 메타데이터를 가져온다.
        ((PlayGamesPlatform)Social.Active).SavedGame.
                OpenWithAutomaticConflictResolution("save", DataSource.ReadCacheOrNetwork,
                                                    ConflictResolutionStrategy.UseLastKnownGood, LoadGame);
    }

    void LoadGame(SavedGameRequestStatus status, ISavedGameMetadata game)       //Load 콜백함수
    {
        if (status == SavedGameRequestStatus.Success)                           //메타 데이터가 가져와졌다면, 형식 변환
        {
            Debug.LogWarning("DataLoad 2");
            myGame = game;
            LoadData(myGame);
        }
        else
        {
            Debug.LogWarning("DataLoad 2 : Faild LoadGame");
        }
    }

    void LoadData(ISavedGameMetadata game)      //***Load로 불러온 메타 데이터를 불러와 변환한다.
    {
        Debug.LogWarning("DataLoad 3");
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, LoadDataCallBack);        //변환
    }


    void LoadDataCallBack(SavedGameRequestStatus status, byte[] LoadedData)
    {
        if (status == SavedGameRequestStatus.Success)                       //데이터 변환이 성공했다면,
        {
            Debug.LogWarning("DataLoad 4");
            try
            {
                myData_String = Encoding.UTF8.GetString(LoadedData);        //Byte To String
                DATA d = JsonUtility.FromJson<DATA>(myData_String);         //Json으로 원래 데이터 포맷으로 변경시킨다.

                if (d == null)
                {
                    Debug.LogError("GOOGLE 데이터가 없습니다.");
                    d = new SaveData();
                }
                else
                {
                    Debug.LogError("GOOGLE 데이터 로드 성공");
                }

                TempData = d;
            }
            catch (Exception e)                                             //데이터 로드시 없을 경우 데이터를 새로 생성
            {
                Debug.LogError($"Error : {e.Message}  from GoogleManager.cs");

                DATA d = new SaveData();                                        //새로운 데이터를 만든다, 초기화.
                Debug.LogError("GOOGLE 데이터 로드 예회 발생 : NEW 데이터 생성 ");
            }

            isLoaded = true;                                                //로드가 끝났음을 알려준다.
        }
        else
        {
            Debug.LogError("GOOGLE ReadBineary_Failed");
        }
    }

    #endregion
    */
}