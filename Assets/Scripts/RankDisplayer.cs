using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankDisplayer : MonoBehaviour
{
    private GameObject[] rankPrefabs;

    public GameObject rankBronzeNormal;
    public GameObject rankBronzeTop;
    public GameObject rankBronzeBest;
    public GameObject rankSilverNormal;
    public GameObject rankSilverTop;
    public GameObject rankSilverBest;
    public GameObject rankGoldNormal;
    public GameObject rankGoldTop;
    public GameObject rankGoldBest;
    public GameObject rankPlatinumNormal;
    public GameObject rankPlatinumTop;
    public GameObject rankPlatinumBest;
    public GameObject rankDiamondNormal;
    public GameObject rankDiamondTop;
    public GameObject rankDiamondBest;
    public GameObject rankMasterNormal;
    public GameObject rankMasterTop;
    public GameObject rankMasterBest;

    private int[] rankRanges;

    private int rankBronzeNormalRange = 0;
    private int rankBronzeTopRange = 100;
    private int rankBronzeBestRange = 150;
    private int rankSilverNormalRange = 200;
    private int rankSilverTopRange = 250;
    private int rankSilverBestRange = 300;
    private int rankGoldNormalRange = 350;
    private int rankGoldTopRange = 400;
    private int rankGoldBestRange = 450;
    private int rankPlatinumNormalRange = 500;
    private int rankPlatinumTopRange = 550;
    private int rankPlatinumBestRange = 600;
    private int rankDiamondNormalRange = 650;
    private int rankDiamondTopRange = 700;
    private int rankDiamondBestRange = 750;
    private int rankMasterNormalRange = 800;
    private int rankMasterTopRange = 850;
    private int rankMasterBestRange = 900;

    public static RankDisplayer instance;

    private void Awake()
    {
        instance = this;

        rankPrefabs = new GameObject[] {
            rankBronzeNormal, rankBronzeTop, rankBronzeBest,
            rankSilverNormal, rankSilverTop, rankSilverBest,
            rankGoldNormal, rankGoldTop, rankGoldBest,
            rankPlatinumNormal, rankPlatinumTop, rankPlatinumBest,
            rankDiamondNormal, rankDiamondTop, rankDiamondBest,
            rankMasterNormal, rankMasterTop, rankMasterBest
        };

        rankRanges = new int[] {
            rankBronzeNormalRange, rankBronzeTopRange, rankBronzeBestRange,
            rankSilverNormalRange, rankSilverTopRange, rankSilverBestRange,
            rankGoldNormalRange, rankGoldTopRange, rankGoldBestRange,
            rankPlatinumNormalRange, rankPlatinumTopRange, rankPlatinumBestRange,
            rankDiamondNormalRange, rankDiamondTopRange, rankDiamondBestRange,
            rankMasterNormalRange, rankMasterTopRange, rankMasterBestRange
        };
    }

    public void UpdateRankDisplay()
    {
        // get player rank
        int rank = CloudManager.Instance.GetRank();

        // check all ranges
        for (int i = 0; i < rankRanges.Length - 1; i++)
        {
            // have i reached this rank?
            if (rankRanges[i] <= rank)
            {
                // set this ranks as display in the menu
                rankPrefabs[i].SetActive(true);
            }
        }
    }
}
