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

    private string ad_3min= "3min";

    void Start()
    {
        Monetization.Initialize(gameId, testMode);
        StartCoroutine(DisplayAdsEvery(CDinMin));
    }


    IEnumerator DisplayAdsEvery(int _CdInMin)
    {
        while (true)
        {

            yield return new WaitForSeconds(_CdInMin * 60);
            if(Monetization.IsReady(ad_3min))
            {
                ShowAdPlacementContent ad = null;
                ad = Monetization.GetPlacementContent(ad_3min) as ShowAdPlacementContent;

                if (ad != null)
                {
                    Debug.Log("showing ad");
                    ad.Show(AdFinished);
                    Time.timeScale = 0;
                }

            }

        }
    }

    void AdFinished(ShowResult result)
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
