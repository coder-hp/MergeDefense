using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Consts;

public class BossRewardPanel : MonoBehaviour
{
    public Transform rewardItemsTrans;
    public Transform btn_ok;

    EnemyWaveData enemyWaveData;
    KillRewardData killRewardData;

    int choicedRewardType = -1;

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
                    break;
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
}
