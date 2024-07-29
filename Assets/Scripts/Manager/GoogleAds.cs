using UnityEngine;

#if UNITY_ANDROID
using GoogleMobileAds.Api;
#endif

using System;

/// <summary>
/// 구글 광고를 로드하고, 보여줍니다.
/// </summary>
/// 
public class GoogleAds : MonoBehaviour
{

#if UNITY_ANDROID
    // 광고가 끝난 후 실행될 이벤트를 담습니다.
    public Action OnAdClosedEvent;

    string adTestId = "ca-app-pub-3940256099942544/5224354917";
    string adUnitId = "ca-app-pub-4507641890363321/1308410259";

    //리워드 종료 구분
    public bool isRewardEnd { get; private set; } = false;
    //실패
    public bool isFailed { get; private set; } = false;

    //리워드 객체
    public RewardedAd rewardedAd;

    //광고가 없었을 경우 시도하는 횟수
    public int TryCount = 0;

    void PrintStatus(string s)
    {
        Debug.LogWarning(s);
    }

    public void RequestAndLoadRewardedAd()
    {
        PrintStatus("광고 로드 시작");

        rewardedAd = null;
        Resources.UnloadUnusedAssets();

        if (!GoogleManager.instance.Connected)
        {
            PrintStatus("GoogleAds 광고 생성 :: 네트워크가 연결되어 있지 않습니다.");
            return;
        }

        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                //리워드 요청이 실패할 경우
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

                    //각 상황별 이벤트 함수
                    //rewardedAd변수에 각 상황에 대한 실행값들을 넣어준다.
                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Google 광고 오픈");
                        isRewardEnd = false;
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Google 광고 닫음");

                        //광고 로드 완료
                        isRewardEnd = true;
                        TryCount = 0;
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Google 광고 노출 실패와 이유 : " +
                                   error.GetMessage()+
                                   " // 다시 광고 호출");
                        rewardedAd = null;
                        isFailed = true;
                        isRewardEnd = true;
                    };
                }
            });
    }

    //리워드 광고를 보여줌
    public bool ShowRewardedAd()
    {
        isFailed = false;

        // 리워드 광고가 있을 경우에 실헹
        if (rewardedAd != null) 
        {
            isRewardEnd = false;

            Debug.LogWarning("GoogleAds 광고 표시.");

            //광고를 보여줍니다.
            rewardedAd.Show((Reward reward) =>
            {
                PrintStatus("GoogleAds 리워드 광고 보상 제공 : " + reward.Amount);
                rewardedAd = null;
            });

            return true;
        }
        //광고가 없기에 로드합니다.
        else
        {
            PrintStatus("Google Rewarded ad is not ready yet.");

            ////2번의 광고 보기 시도 후 실행한다.
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

    //리워드 광고가 비어 있는지 확인
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
