using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class AdsManager : MonoBehaviour
{
    [SerializeField]
    int CDinMin;

    string gameId = "3085625";
    bool testMode = true;

    public const string AD_3MIN= "3min";

    void Start()
    {
        Monetization.Initialize(gameId, testMode);
        StartCoroutine(DisplayAdsEvery(CDinMin));
    }

    public static void DisplayAds(string ad_name)
    {
        //float t =0;
        //while (!Monetization.IsReady(ad_name)) { t += Time.deltaTime; if (t > 5) break; }

        if(Monetization.IsReady(ad_name))
        {
            ShowAdPlacementContent ad = null;
            ad = Monetization.GetPlacementContent(ad_name) as ShowAdPlacementContent;

            if (ad != null)
            {
                Debug.Log("showing ad");
                ad.Show(OnAdFinished);
                Time.timeScale = 0;
            }
        }
    }

    IEnumerator DisplayAdsEvery(int _CdInMin)
    {
        while (true)
        {
            yield return new WaitForSeconds(_CdInMin * 60);
            DisplayAds(AD_3MIN);
        }
    }

    public static void OnAdFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            Time.timeScale = 1;
        }
        else if (result == ShowResult.Skipped)
        {
            Time.timeScale = 1;
            Debug.LogWarning("The player skipped the video - DO NOT REWARD!");

        }
        else if (result == ShowResult.Failed)
        {
            Time.timeScale = 1;
            Debug.LogError("Video failed to show");
        }
    }

    
}
