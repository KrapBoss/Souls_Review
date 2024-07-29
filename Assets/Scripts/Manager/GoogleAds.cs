using UnityEngine;

#if UNITY_ANDROID
using GoogleMobileAds.Api;
#endif

using System;

/// <summary>
/// ���� ���� �ε��ϰ�, �����ݴϴ�.
/// </summary>
/// 
public class GoogleAds : MonoBehaviour
{

#if UNITY_ANDROID
    // ���� ���� �� ����� �̺�Ʈ�� ����ϴ�.
    public Action OnAdClosedEvent;

    string adTestId = "ca-app-pub-3940256099942544/5224354917";
    string adUnitId = "ca-app-pub-4507641890363321/1308410259";

    //������ ���� ����
    public bool isRewardEnd { get; private set; } = false;
    //����
    public bool isFailed { get; private set; } = false;

    //������ ��ü
    public RewardedAd rewardedAd;

    //���� ������ ��� �õ��ϴ� Ƚ��
    public int TryCount = 0;

    void PrintStatus(string s)
    {
        Debug.LogWarning(s);
    }

    public void RequestAndLoadRewardedAd()
    {
        PrintStatus("���� �ε� ����");

        rewardedAd = null;
        Resources.UnloadUnusedAssets();

        if (!GoogleManager.instance.Connected)
        {
            PrintStatus("GoogleAds ���� ���� :: ��Ʈ��ũ�� ����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                //������ ��û�� ������ ���
                if (loadError != null)
                {
                    PrintStatus("Google Rewarded ad failed to load with error: " +
                                loadError.GetMessage());

                    return;
                }
                else if (ad == null)
                {
                    PrintStatus("Google Rewarded ad failed to load."); 

                    return;
                }
                else
                {
                    PrintStatus("Google Rewarded loaded");

                    rewardedAd = ad;
                    TryCount = 0;

                    //�� ��Ȳ�� �̺�Ʈ �Լ�
                    //rewardedAd������ �� ��Ȳ�� ���� ���ప���� �־��ش�.
                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Google ���� ����");
                        isRewardEnd = false;
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Google ���� ����");

                        //���� �ε� �Ϸ�
                        isRewardEnd = true;
                        TryCount = 0;
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Google ���� ���� ���п� ���� : " +
                                   error.GetMessage()+
                                   " // �ٽ� ���� ȣ��");
                        rewardedAd = null;
                        isFailed = true;
                        isRewardEnd = true;
                    };
                }
            });
    }

    //������ ���� ������
    public bool ShowRewardedAd()
    {
        isFailed = false;

        // ������ ���� ���� ��쿡 ����
        if (rewardedAd != null) 
        {
            isRewardEnd = false;

            Debug.LogWarning("GoogleAds ���� ǥ��.");

            //���� �����ݴϴ�.
            rewardedAd.Show((Reward reward) =>
            {
                PrintStatus("GoogleAds ������ ���� ���� ���� : " + reward.Amount);
                rewardedAd = null;
            });

            return true;
        }
        //���� ���⿡ �ε��մϴ�.
        else
        {
            PrintStatus("Google Rewarded ad is not ready yet.");

            ////2���� ���� ���� �õ� �� �����Ѵ�.
            //if (TryCount < 2)
            //{
            //    TryCount++;

            //    GoogleManager.instance.ShowNotice($"[ {3-TryCount} ]");

            //    RequestAndLoadRewardedAd();

            //    return false;
            //}

            return false;
        }
    }

    //������ ���� ��� �ִ��� Ȯ��
    public bool GetIsADLoaded()
    {
        return (rewardedAd != null);
    }

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest();
    }
    #endregion
#endif

}
