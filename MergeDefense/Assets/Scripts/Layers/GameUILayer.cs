using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUILayer : MonoBehaviour
{
    public static GameUILayer s_instance = null;

    public GameObject prefab_bloodBar;
    public GameObject prefab_bloodBar_big;
    public GameObject prefab_heroUI;
    public GameObject item_weapon;
    public Image img_enemyCountProgress;
    public Transform bloodPointTrans;
    public Transform heroUIPointTrans;
    public Transform weaponGridTrans;
    public Transform curGoldIconTrans;
    public Transform curDiamondIconTrans;
    public Transform btn_weaponShop;
    public Transform btn_weaponSell;
    public Text text_weaponSellPrice;
    public Text text_enemyCount;
    public Text text_time;
    public Text text_boci;
    public Text text_gold;
    public Text text_diamond;
    public Text btn_summon_gold;
    public Text btn_forge_gold;

    [HideInInspector]
    public int curBoCi = 0;
    int curBoCiRestTime = 20;

    [HideInInspector]
    public int curGold = Consts.startHaveGold;
    [HideInInspector]
    public int curDiamond = Consts.startHaveDiamond;
    [HideInInspector]
    public int curSummonGold = Consts.startSummonGold;
    [HideInInspector]
    public int curForgeGold = Consts.startForgeGold;

    List<EnemyWaveData> waitAddEnemy = new List<EnemyWaveData>();

    [HideInInspector]
    public bool isCanDragWeapon = true;
    [HideInInspector]
    public bool isCanOnInvokeBoCiSecond = true;

    private void Awake()
    {
        s_instance = this;

        curGold = Consts.startHaveGold;
        curDiamond = Consts.startHaveDiamond;

        text_gold.text = curGold.ToString();
        text_diamond.text = curDiamond.ToString();
        btn_summon_gold.text = curSummonGold.ToString();
        btn_forge_gold.text = curForgeGold.ToString();
    }

    private void Start()
    {
        LayerManager.ShowLayer(Consts.Layer.HeroInfoPanel);
        LayerManager.ShowLayer(Consts.Layer.WeaponShopPanel);

        Invoke("startBoCi", 0.5f);
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

        // 刷新武器商店
        WeaponShopPanel.s_instance.refreshWeapon();

        // bgm
        {
            if(curBoCi % 10 == 0)
            {
                AudioScript.s_instance.playMusic("bgm_battle_boss", true);
            }
            else if (curBoCi % 10 == 1)
            {
                AudioScript.s_instance.playMusic("bgm_battle", true);
            }
        }

        if(curBoCi == 11)
        {
            ToastScript.show("Rare Heroes Now Available!");
        }
        else if (curBoCi == 21)
        {
            ToastScript.show("Epic Heroes Now Available!");
        }
    }

    void onInvokeBoCiSecond()
    {
        if(!isCanOnInvokeBoCiSecond)
        {
            return;
        }

        --curBoCiRestTime;

        if(curBoCiRestTime < 0)
        {
            curBoCiRestTime = 0;
        }

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
            Invoke("startBoCi", 1);
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
        curBoCiRestTime = 0;
        curBoCi = boci - 1;
    }

    public void refreshEnemyCount()
    {
        text_enemyCount.text = EnemyManager.s_instance.getEnemyCount() + "/" + Consts.maxEnemyCount;
        img_enemyCountProgress.fillAmount = EnemyManager.s_instance.getEnemyCount() / (float)Consts.maxEnemyCount;
    }

    public void setIsShowBtnWeaponSell(bool isShow,WeaponData weaponData = null)
    {
        if (isShow)
        {
            btn_weaponShop.localScale = Vector3.zero;
            btn_weaponSell.localScale = Vector3.one;

            text_weaponSellPrice.text = weaponData.level.ToString();
        }
        else
        {
            btn_weaponShop.localScale = Vector3.one;
            btn_weaponSell.localScale = Vector3.zero;
        }
    }

    Sequence tween_changeGoldIcon = null;
    Sequence tween_changeGoldText = null;
    public void changeGold(int value)
    {
        curGold += value;
        text_gold.text = curGold.ToString();

        MoneyChangeTextPoint.s_instance.show(value,1);

        if (tween_changeGoldIcon == null)
        {
            tween_changeGoldIcon = DOTween.Sequence();
            tween_changeGoldIcon.Append(curGoldIconTrans.DOScale(1.3f, 0.2f))
                                .Append(curGoldIconTrans.DOScale(1, 0.2f));
            tween_changeGoldIcon.SetAutoKill(false);
        }
        else
        {
            tween_changeGoldIcon.Restart();
        }

        if (tween_changeGoldText == null)
        {
            tween_changeGoldText = DOTween.Sequence();
            tween_changeGoldText.Append(text_gold.transform.DOScale(1.3f, 0.2f))
                                .Append(text_gold.transform.DOScale(1, 0.2f));
            tween_changeGoldText.SetAutoKill(false);
        }
        else
        {
            tween_changeGoldText.Restart();
        }

        // 检查召唤金额是否足够
        if (curGold >= curSummonGold)
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

    Sequence tween_changeDiamondIcon = null;
    Sequence tween_changeDiamondText = null;
    public void changeDiamond(int value)
    {
        curDiamond += value;
        text_diamond.text = curDiamond.ToString();

        MoneyChangeTextPoint.s_instance.show(value,2);

        if (tween_changeDiamondIcon == null)
        {
            tween_changeDiamondIcon = DOTween.Sequence();
            tween_changeDiamondIcon.Append(curDiamondIconTrans.DOScale(1.3f, 0.2f))
                                   .Append(curDiamondIconTrans.DOScale(1, 0.2f));
            tween_changeDiamondIcon.SetAutoKill(false);
        }
        else
        {
            tween_changeDiamondIcon.Restart();
        }

        if (tween_changeDiamondText == null)
        {
            tween_changeDiamondText = DOTween.Sequence();
            tween_changeDiamondText.Append(text_diamond.transform.DOScale(1.3f, 0.2f))
                                   .Append(text_diamond.transform.DOScale(1, 0.2f));
            tween_changeDiamondText.SetAutoKill(false);
        }
        else
        {
            tween_changeDiamondText.Restart();
        }

        WeaponShopPanel.s_instance.diamondChanged();
    }

    public void onClickPause()
    {
        AudioScript.s_instance.playSound_btn();

        if (Time.timeScale == 0)
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
        AudioScript.s_instance.playSound_btn();

        ToastScript.show("暂未开放");
    }

    public void onClickAutoMake()
    {
        AudioScript.s_instance.playSound_btn();

        ToastScript.show("暂未开放");
    }

    public void onClickMyth()
    {
        AudioScript.s_instance.playSound_btn();

        ToastScript.show("暂未开放");
    }

    // 召唤
    public void onClickHero()
    {
        AudioScript.s_instance.playSound_btn();

        if (curGold < curSummonGold)
        {
            AudioScript.s_instance.playSound("summonNotHaveGold");
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
        AudioScript.s_instance.playSound_btn();

        if (curGold < curForgeGold)
        {
            AudioScript.s_instance.playSound("summonNotHaveGold");
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

    public void addWeapon(WeaponData weaponData)
    {
        for (int i = 0; i < weaponGridTrans.childCount; i++)
        {
            if (weaponGridTrans.GetChild(i).childCount == 0)
            {
                UIItemWeapon uIItemWeapon = Instantiate(item_weapon, weaponGridTrans.GetChild(i)).GetComponent<UIItemWeapon>();
                uIItemWeapon.init(weaponData.type, weaponData.level);
                break;
            }
        }
    }

    public void onClickShop()
    {
        AudioScript.s_instance.playSound_btn();

        WeaponShopPanel.s_instance.show();
    }
}
