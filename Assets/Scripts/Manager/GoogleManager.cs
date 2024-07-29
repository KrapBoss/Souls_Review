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
/// ���� ������ �����մϴ�.
/// ���� ���� �� ���ʷ� �����Ǹ�, ���� ��尡 ������� ��쿡�� ����˴ϴ�.
/// ��Ʈ��ũ�� ����Ǿ� �ִ� ���¸� �Ǵ��� ���� �����ŵ�ϴ�.
/// </summary>
/// 
public class GoogleManager : MonoBehaviour
{

#if UNITY_ANDROID

    public static GoogleManager instance;

    //���� �ε��ϱ� ���� ������ �ε�
    public GameObject go_Delay;

    public TMP_Text txt;

    private GoogleAds Admob;


    //��Ʈ��ũ�� ���� ������� ����
    public bool Connected
    {
        get { return CheckNetworking(); }
    }


    //���� �α��� ���¸� �����մϴ�.
    private bool logind = false;
    private bool checkedUpdate = false;


    private void Start()
    {
        //�ǽ��� ��� �����մϴ�.
        if (GameConfig.IsPc())
        {
            Debug.LogWarning("GoogleManager ���� �Ŵ����� ����");
            Destroy(this.gameObject);
            Fade.FadeSetting(false, 0.2f, Color.black);
        }
        else
        {
            //���� �����ϴ� ������Ʈ�� ����
            if (instance != null)
            {
                Debug.LogWarning("GoogleManager ���� �Ŵ����� ����");
                Destroy(this.gameObject);
                return;
            }

            Debug.LogWarning("GoogleManager ���� �Ŵ����� ����");

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
        Debug.LogWarning("GoogleManager �÷��̾� �α��� �õ�");

        Social.localUser.Authenticate((success) =>
        {
            if (success)
            {
                logind = true;
                Debug.LogWarning("GoogleManager �÷��̾� �α��� ���� ������Ʈ Ȯ��");
            }
            else
            {
                Debug.LogWarning("GoogleManager �÷��̾� �α��� ����");
            }
        });
    }

    //��Ʈ��ũ ������¸� �Ǵ��մϴ�.
    bool CheckNetworking()
    {
        ////�α����� �ȵǾ� �ִ� ��� 
        //if (!logind)
        //{
        //    Login();
        //    return false;
        //}

        //��Ʈ��ũ�� ����Ǿ����� �ʴ� ��� false
        return !Application.internetReachability.Equals(NetworkReachability.NotReachable);
    }
    
    //���� �����ݴϴ�.
    public bool ShowAd(Action action)
    {
        Debug.LogWarning("GoogleManager ���� �����ݴϴ�.");


        //ĵ������ ��Ʈ��ũ�� ������� �ʾ����� ��Ÿ����.
        if (!Connected)
        {
            Debug.LogWarning("GoogleAds ��Ʈ��ũ�� �����ϴ�.");
            ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "Connecting"));

            return false;
        }

        //������Ʈ�� üũ���� �ʾҽ��ϴ�.
        if (!checkedUpdate)
        {
            AppUpdateCheck();
            ShowNotice("Checking Update Please...");

            return false;
        }

        go_Delay.SetActive(true);
        
        //���� �ִ��� �Ǵ��մϴ�.
        if (Admob.ShowRewardedAd())
        {
            Debug.LogWarning("GoogleManager ���� �����մϴ�.");

            if (action != null)
            {
                StartCoroutine(RewardDelay(action));
            }
        }
        else
        {
            Debug.LogWarning("GoogleManager ���� �����ϴ�.");

            StartCoroutine(CallRewardDelay(action));
        }

        return true;
    }
    
    //���� �����ϴ� ��� ����Ǵ� ����
    IEnumerator RewardDelay(Action action)
    {
        Debug.LogWarning("GoogleManager ���� ���� ��� ����-------------");

        //���� ���� ������ ���
        yield return new WaitUntil(() => RewardEnd());

        Debug.LogWarning("GoogleManager ���� ���� ��� ����-------------");

        if (RewardFailed())
        {
            Debug.LogWarning("GoogleManager ���� ����.--------------");

            StartCoroutine(CallRewardDelay(action));
        }
        else
        {
            Debug.LogWarning("GoogleManager ���� ���� ������ �̺�Ʈ�� �����մϴ�.--------------");
            action();
            Admob.RequestAndLoadRewardedAd();
            go_Delay.SetActive(false);
        }

    }

    //���� ���� ��� �ҷ����� ���� �����մϴ�.
    IEnumerator CallRewardDelay(Action action)
    {
        //���� �ҷ�����
        Admob.RequestAndLoadRewardedAd();

        //���� �ε带 ����մϴ�.
        float time = 0;
        do
        {
            time += Time.unscaledDeltaTime;
            if (Admob.GetIsADLoaded())
            {
                //����ε� �Ϸ�
                Debug.LogWarning("GoogleManager ���� �ε� ����.--------------");
                break;
            }

            yield return null;

        } while (time < 10.0f);


        //���� ���� ������ ���� ����
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
    //���� ����Ǿ����� �Ǵ�
    public bool RewardEnd()
    {
        return Admob.isRewardEnd;
    }
    //���� ����Ǿ����� �Ǵ�
    public bool RewardFailed()
    {
        return Admob.isFailed;
    }


    IEnumerator showNoticeCroutine;
    //�ʿ��� ������ �ȳ����� �����ݴϴ�.
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
    /// ���� ������Ʈ�� Ȯ���մϴ�.
    /// </summary>
    /// 
    AppUpdateManager appUpdateManager;
    public void AppUpdateCheck()
    {
        //�ȵ���̵� �� ��쿡�� ������Ʈ�� ȣ���մϴ�.
        if(Application.platform == RuntimePlatform.Android)
        {
            appUpdateManager = new AppUpdateManager();
            Debug.Log("GoogleManager �� ������Ʈ�� Ȯ���� �����մϴ�.");
            StartCoroutine(CheckForUpdate());
        }
        else
        {
            checkedUpdate = true;
        }
    }

    //�ۿ� ���� ������ ������Ʈ�� �ִ��� Ȯ���մϴ�.
    IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        go_Delay.SetActive(true);

        // ������ Ȯ�� �۾�
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            //������Ʈ ���� ����
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                //��� ������Ʈ
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

                var startUpdateRequest = appUpdateManager.StartUpdate( appUpdateInfoResult, appUpdateOptions);

                while (!startUpdateRequest.IsDone)
                {
                    if(startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        Debug.Log("GoogleManager �� �ٿ� ��");
                    }
                    else if(startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        Debug.Log("GoogleManager �� �ٿ� �Ϸ�");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Canceled)
                    {
                        Debug.Log("GoogleManager �� ������Ʈ ���");
                        Application.Quit();
                    }

                    yield return null;
                }

                checkedUpdate = true;

                var result = appUpdateManager.CompleteUpdate();

                //�Ϸ� Ȯ��
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            else if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("GoogleManager ������Ʈ ���� , �α��� �õ�");
                checkedUpdate = true;
                Login();
                yield return (int)UpdateAvailability.UpdateNotAvailable;
            }
            else
            {
                Debug.Log("GoogleManager ������Ʈ ���� ���θ� �� �� ����");
                checkedUpdate = false;
                yield return (int)UpdateAvailability.Unknown;
            }
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
            Debug.Log("GoogleManager ������Ʈ ���� " + appUpdateInfoOperation.Error);
            checkedUpdate = false;
            ShowNotice("GoogleManager UpdateError " + appUpdateInfoOperation.Error, 3.0f);
        }

        go_Delay.SetActive(false);
    }
#endif
    /*
    #region Save / Load
    ISavedGameMetadata myGame;//���� ���� ������ ������ ��Ÿ ������

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

    void SaveData(ISavedGameMetadata game, string data)             //��Ÿ �����͸� �����Ͽ� 
    {
        myData_Binary = Encoding.UTF8.GetBytes(data);               //String�� Byte Ÿ������ ���ڵ�

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build(); //������ ���� ��ü ����
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, myData_Binary, SaveCallBack);//������ ����
    }

    private void SaveCallBack(SavedGameRequestStatus status, ISavedGameMetadata game)   //Save �ݹ� �Լ�
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
        //���ϸ��� �Ѱ��ָ鼭 ����Ǿ� �ִ� ���� ��Ÿ�����͸� �����´�.
        ((PlayGamesPlatform)Social.Active).SavedGame.
                OpenWithAutomaticConflictResolution("save", DataSource.ReadCacheOrNetwork,
                                                    ConflictResolutionStrategy.UseLastKnownGood, LoadGame);
    }

    void LoadGame(SavedGameRequestStatus status, ISavedGameMetadata game)       //Load �ݹ��Լ�
    {
        if (status == SavedGameRequestStatus.Success)                           //��Ÿ �����Ͱ� ���������ٸ�, ���� ��ȯ
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

    void LoadData(ISavedGameMetadata game)      //***Load�� �ҷ��� ��Ÿ �����͸� �ҷ��� ��ȯ�Ѵ�.
    {
        Debug.LogWarning("DataLoad 3");
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, LoadDataCallBack);        //��ȯ
    }


    void LoadDataCallBack(SavedGameRequestStatus status, byte[] LoadedData)
    {
        if (status == SavedGameRequestStatus.Success)                       //������ ��ȯ�� �����ߴٸ�,
        {
            Debug.LogWarning("DataLoad 4");
            try
            {
                myData_String = Encoding.UTF8.GetString(LoadedData);        //Byte To String
                DATA d = JsonUtility.FromJson<DATA>(myData_String);         //Json���� ���� ������ �������� �����Ų��.

                if (d == null)
                {
                    Debug.LogError("GOOGLE �����Ͱ� �����ϴ�.");
                    d = new SaveData();
                }
                else
                {
                    Debug.LogError("GOOGLE ������ �ε� ����");
                }

                TempData = d;
            }
            catch (Exception e)                                             //������ �ε�� ���� ��� �����͸� ���� ����
            {
                Debug.LogError($"Error : {e.Message}  from GoogleManager.cs");

                DATA d = new SaveData();                                        //���ο� �����͸� �����, �ʱ�ȭ.
                Debug.LogError("GOOGLE ������ �ε� ��ȸ �߻� : NEW ������ ���� ");
            }

            isLoaded = true;                                                //�ε尡 �������� �˷��ش�.
        }
        else
        {
            Debug.LogError("GOOGLE ReadBineary_Failed");
        }
    }

    #endregion
    */
}