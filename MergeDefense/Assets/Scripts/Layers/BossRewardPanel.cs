using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Consts;

public class BossRewardPanel : MonoBehaviour
{
    public Transform rewardItemsTrans;

    EnemyWaveData enemyWaveData;
    KillRewardData killRewardData;

    public void init(EnemyWaveData _enemyWaveData, KillRewardData _killRewardData)
    {
        enemyWaveData = _enemyWaveData;
        killRewardData = _killRewardData;

        string[] rewardTypr = killRewardData.reward3_1.Split('_');
        for(int i = 0; i < rewardTypr.Length; i++)
        {
            rewardItemsTrans.GetChild(int.Parse(rewardTypr[i]) - 1).gameObject.SetActive(true);
        }
    }

    // 1武器币     // 2增加角色高星概率     // 3增加武器高等级概率     // 4从召唤池中删除角色
    public void onClickReward(int rewardType)
    {
        Destroy(gameObject);

        GameUILayer.s_instance.isCanOnInvokeBoCiSecond = true;
        GameUILayer.s_instance.changeDiamond(killRewardData.diamond);

        // 如果场上没有敌人，直接开始下一波
        //if(EnemyManager.s_instance.list_enemy.Count == 0)
        {
            GameUILayer.s_instance.forceToBoCi(enemyWaveData.wave + 1);
        }
    }
}
