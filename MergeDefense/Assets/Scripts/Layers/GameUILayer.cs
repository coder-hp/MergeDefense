using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUILayer : MonoBehaviour
{
    public static GameUILayer s_instance = null;

    public HeroInfoPanel heroInfoPanel;
    public GameObject prefab_bloodBar;
    public GameObject prefab_heroStar;
    public GameObject prefab_heroEmoji;
    public GameObject item_weapon;
    public Image img_enemyCountProgress;
    public Transform bloodPointTrans;
    public Transform heroStarPointTrans;
    public Transform weaponGridTrans;
    public Transform curGoldIconTrans;
    public Text text_enemyCount;
    public Text text_time;
    public Text text_boci;
    public Text text_gold;
    public Text text_diamond;
    public Text btn_summon_gold;
    public Text btn_forge_gold;

    int curBoCi = 0;
    int curBoCiRestTime = 20;

    [HideInInspector]
    public int curGold = Consts.startHaveGold;
    [HideInInspector]
    public int curSummonGold = Consts.startSummonGold;
    [HideInInspector]
    public int curForgeGold = Consts.startForgeGold;

    List<EnemyWaveData> waitAddEnemy = new List<EnemyWaveData>();

    [HideInInspector]
    public bool isCanDragWeapon = true;

    private void Awake()
    {
        s_instance = this;

        text_gold.text = curGold.ToString();
        btn_summon_gold.text = curSummonGold.ToString();
        btn_forge_gold.text = curForgeGold.ToString();

        Invoke("startBoCi",0.5f);
    }

    void startBoCi()
    {
        ++curBoCi;

        // ToastScript.show("WAVE：" + curBoCi);

        EnemyWaveData enemyWaveData = EnemyWaveEntity.getInstance().getData(curBoCi);

        curBoCiRestTime = enemyWaveData.time;
        text_boci.text = "WAVE：" + curBoCi + "/80";

        if(curBoCiRestTime == 60)
        {
            text_time.text = "01:00";
        }
        else
        {
            text_time.text = "00:" + curBoCiRestTime;
        }

        waitAddEnemy.Clear();
        for(int i = 0; i < enemyWaveData.count; i++)
        {
            //enemyWaveData.prefab = "enemy29";
            waitAddEnemy.Add(enemyWaveData);
        }

        InvokeRepeating("onInvokeBoCiSecond",1,1);
        InvokeRepeating("onInvokeAddEnemy", 0.6f, 0.6f);
    }

    void onInvokeBoCiSecond()
    {
        --curBoCiRestTime;

        if(curBoCiRestTime >= 10)
        {
            text_time.text = "00:" + curBoCiRestTime;
        }
        else
        {
            text_time.text = "00:0" + curBoCiRestTime;
        }

        if (curBoCiRestTime <= 0)
        {
            CancelInvoke("onInvokeBoCiSecond");
            Invoke("startBoCi", 2);
        }
    }

    void onInvokeAddEnemy()
    {
        if(waitAddEnemy.Count > 0)
        {
            GameLayer.s_instance.addEnemy(waitAddEnemy[0]);
            waitAddEnemy.RemoveAt(0);

            if (waitAddEnemy.Count == 0)
            {
                CancelInvoke("onInvokeAddEnemy");
            }
        }
    }

    public void forceToBoCi(int boci)
    {
        curBoCiRestTime = 1;
        curBoCi = boci - 1;
    }

    public void refreshEnemyCount()
    {
        text_enemyCount.text = EnemyManager.s_instance.getEnemyCount() + "/" + GameLayer.s_instance.maxEnemyCount;
        img_enemyCountProgress.fillAmount = EnemyManager.s_instance.getEnemyCount() / 100f;
    }

    Sequence tween_changeGold = null;
    public void changeGold(int value)
    {
        curGold += value;
        text_gold.text = curGold.ToString();

        KillGoldManager.s_instance.showKillGold(value);

        if (tween_changeGold == null)
        {
            tween_changeGold = DOTween.Sequence();
            tween_changeGold.Append(curGoldIconTrans.DOScale(1.3f, 0.2f))
                       .Append(curGoldIconTrans.DOScale(1, 0.2f));
            tween_changeGold.SetAutoKill(false);
        }
        else
        {
            tween_changeGold.Restart();
        }

        // 检查召唤金额是否足够
        if(curGold >= curSummonGold)
        {
            btn_summon_gold.color = Color.white;
        }
        else
        {
            btn_summon_gold.color = new Color(0.97f, 0.26f, 0.26f, 1);
        }

        // 检查锻造金额是否足够
        if (curGold >= curForgeGold)
        {
            btn_forge_gold.color = Color.white;
        }
        else
        {
            btn_forge_gold.color = new Color(0.97f, 0.26f, 0.26f, 1);
        }
    }

    public void onClickPause()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void onClickSort()
    {
        ToastScript.show("暂未开放");
    }

    public void onClickAutoMake()
    {
        ToastScript.show("暂未开放");
    }

    public void onClickMyth()
    {
        ToastScript.show("暂未开放");
    }

    // 召唤
    public void onClickHero()
    {
        if (curGold < curSummonGold)
        {
            ToastScript.show("Coins Not Enough!");
            return;
        }

        if (GameLayer.s_instance.addHero())
        {
            int costGold = curSummonGold;

            // 增加下次召唤金额
            curSummonGold += Consts.summonAddGold;
            btn_summon_gold.text = curSummonGold.ToString();

            changeGold(-costGold);
        }
    }

    // 锻造
    public void onClickForge()
    {
        if (curGold < curForgeGold)
        {
            ToastScript.show("Coins Not Enough!");
            return;
        }

        bool isForgeSuccess = false;
        int forgeCount = RandomUtil.getRandom(1, 3);
        for (int c = 0; c < forgeCount; c++)
        {
            for (int i = 0; i < weaponGridTrans.childCount; i++)
            {
                if (weaponGridTrans.GetChild(i).childCount == 0)
                {
                    isForgeSuccess = true;

                    UIItemWeapon uIItemWeapon = Instantiate(item_weapon, weaponGridTrans.GetChild(i)).GetComponent<UIItemWeapon>();
                    uIItemWeapon.init(RandomUtil.getRandom(1, 5), 1);
                    break;
                }
            }
        }

        if (isForgeSuccess)
        {
            int costGold = curForgeGold;

            // 增加下次锻造金额
            curForgeGold += Consts.forgeAddGold;
            btn_forge_gold.text = curForgeGold.ToString();

            changeGold(-costGold);
        }
    }

    public void onClickShop()
    {
        ToastScript.show("暂未开放");
    }
}
