using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossRewardPanel : MonoBehaviour
{
    public static BossRewardPanel s_instance = null;

    public Transform choiceRewardPanel;
    public Transform deleteHeroPanel;
    public Transform rewardItemsTrans;
    public Transform herosTrans;
    public Transform btn_ok;
    public Transform btn_delete;
    public Text text_tip;
    public Text text_time;

    EnemyWaveData enemyWaveData;
    KillRewardData killRewardData;

    int choicedRewardType = -1;
    int choiceDeleteHeroIndex;
    List<int> canChoiceDeleteHero = new List<int>();

    int restTime = 15;

    private void Awake()
    {
        s_instance = this;

        InvokeRepeating("onInvokeSecond",1,1);
    }

    void onInvokeSecond()
    {
        --restTime;
        text_time.text = restTime + "s";


        // 倒计时结束后，直接默认选择角色
        if(restTime <= 0)
        {
            CancelInvoke("onInvokeSecond");
            onClickReward(2);
        }
    }

    public void init(EnemyWaveData _enemyWaveData, KillRewardData _killRewardData)
    {
        enemyWaveData = _enemyWaveData;
        killRewardData = _killRewardData;

        string[] rewardType = killRewardData.reward3_1.Split('_');
        for(int i = 0; i < rewardType.Length; i++)
        {
            Transform item = rewardItemsTrans.GetChild(int.Parse(rewardType[i]) - 1);
            item.gameObject.SetActive(true);

            item.DOScaleX(0, 0.3f).SetEase(Ease.InQuad).SetDelay(0.4f).OnComplete(()=>
            {
                item.Find("info").localScale = Vector3.one;
                item.Find("mask").localScale = Vector3.zero;
                item.DOScaleX(1, 0.3f).SetEase(Ease.OutQuad);
            });
        }
    }

    // 1武器币     // 2增加角色高星概率     // 3增加武器高等级概率     // 4从召唤池中删除角色
    public void onClickReward(int rewardType)
    {
        AudioScript.s_instance.playSound_btn();

        btn_ok.localScale = Vector3.one;
        choicedRewardType = rewardType;

        for (int i = 0; i < rewardItemsTrans.childCount; i++)
        {
            if((i + 1) == rewardType)
            {
                rewardItemsTrans.GetChild(i).Find("info/choiced").localScale = Vector3.one;
            }
            else
            {
                rewardItemsTrans.GetChild(i).Find("info/choiced").localScale = Vector3.zero;
            }
        }
    }

    public void onClickConfirm()
    {
        AudioScript.s_instance.playSound_btn();

        CancelInvoke("onInvokeSecond");
        text_time.transform.parent.localScale = Vector3.zero;

        switch (choicedRewardType)
        {
            // 1武器币
            case 1:
                {
                    GameUILayer.s_instance.changeDiamond(100);
                    break;
                }

            // 2增加角色高星概率 
            case 2:
                {
                    GameFightData.s_instance.changeHeroHighStarRate(20);
                    break;
                }

            // 3增加武器高等级概率 
            case 3:
                {
                    GameFightData.s_instance.changeWeaponHighLevelRate(10);
                    break;
                }

            // 4从召唤池中删除角色
            case 4:
                {
                    showDeleteHeroPanel();
                    return;
                }
        }

        Destroy(gameObject);

        GameFightData.s_instance.isCanOnInvokeBoCiSecond = true;
        GameUILayer.s_instance.changeDiamond(killRewardData.diamond);

        // 如果场上没有敌人，直接开始下一波
        //if(EnemyManager.s_instance.list_enemy.Count == 0)
        {
            GameUILayer.s_instance.forceToBoCi(enemyWaveData.wave + 1);
        }
    }

    void showDeleteHeroPanel()
    {
        text_tip.text = "Please select the hero you want to exclude";

        choiceRewardPanel.localScale = Vector3.zero;
        deleteHeroPanel.localScale = Vector3.one;

        List<int> list_canSummonHero = new List<int>();
        for (int i = 0; i < GameFightData.s_instance.list_canSummonHero.Count; i++)
        {
            list_canSummonHero.Add(GameFightData.s_instance.list_canSummonHero[i]);
        }
        while (canChoiceDeleteHero.Count < 3)
        {
            int heroId = list_canSummonHero[RandomUtil.getRandom(0, list_canSummonHero.Count - 1)];
            canChoiceDeleteHero.Add(heroId);
            list_canSummonHero.Remove(heroId);
        }

        for (int i = 0; i < herosTrans.childCount; i++)
        {
            HeroData heroData = HeroEntity.getInstance().getData(canChoiceDeleteHero[i]);
            herosTrans.GetChild(i).Find("name_bg/name").GetComponent<Text>().text = heroData.name;
            herosTrans.GetChild(i).Find("head").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + canChoiceDeleteHero[i]);
            herosTrans.GetChild(i).GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_2");
            herosTrans.GetChild(i).Find("kuang").GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_1");
        }
    }

    public void onClickHero(int index)
    {
        AudioScript.s_instance.playSound_btn();

        btn_delete.localScale = Vector3.one;
        choiceDeleteHeroIndex = index;

        for (int i = 0; i < herosTrans.childCount; i++)
        {
            if (i == index)
            {
                herosTrans.GetChild(i).Find("choiced").localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            else
            {
                herosTrans.GetChild(i).Find("choiced").localScale = Vector3.zero;
            }
        }
    }

    public void onClickDeleteHero()
    {
        AudioScript.s_instance.playSound_btn();

        GameFightData.s_instance.list_canSummonHero.Remove(canChoiceDeleteHero[choiceDeleteHeroIndex]);

        Destroy(gameObject);

        GameFightData.s_instance.isCanOnInvokeBoCiSecond = true;
        GameUILayer.s_instance.changeDiamond(killRewardData.diamond);

        // 如果场上没有敌人，直接开始下一波
        //if(EnemyManager.s_instance.list_enemy.Count == 0)
        {
            GameUILayer.s_instance.forceToBoCi(enemyWaveData.wave + 1);
        }
    }
}
