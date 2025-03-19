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
    public GameObject item_mythicHero;
    public GameObject item_weapon;
    public Image img_enemyCountProgress;
    public Transform bloodPointTrans;
    public Transform heroUIPointTrans;
    public Transform weaponGridTrans;
    public Transform curGoldIconTrans;
    public Transform curDiamondIconTrans;
    public Transform btn_weaponShop;
    public Transform btn_weaponSell;
    public Transform bellTrans;
    public Transform bossRedKuangTrans;
    public Transform weaponBarPointTrans;
    public Transform effectPoint;
    public Transform list_content_mythicHero;
    public Transform btn_mythicRedPoint;
    public Transform btn_spawn;
    public Text text_mythicRedPointNum;
    public Text text_weaponSellPrice;
    public Text text_enemyCount;
    public Text text_time;
    public Text text_boci;
    public Text text_gold;
    public Text text_diamond;
    public Text btn_summon_gold;
    public Text btn_forge_gold;
    public Text text_timeScale;
    public List<WeaponBar> list_weaponBar = new List<WeaponBar>();

    List<EnemyWaveData> waitAddEnemy = new List<EnemyWaveData>();
    List<HeroData> list_mythicHero = new List<HeroData>();
    List<GameObject> list_mythicHeroItem = new List<GameObject>();

    private void Awake()
    {
        s_instance = this;

        GameFightData.s_instance.curGold = Consts.startHaveGold;
        GameFightData.s_instance.curDiamond = Consts.startHaveDiamond;

        text_gold.text = GameFightData.s_instance.curGold.ToString();
        text_diamond.text = GameFightData.s_instance.curDiamond.ToString();
        btn_summon_gold.text = GameFightData.s_instance.curSummonGold.ToString();
        btn_forge_gold.text = GameFightData.s_instance.curForgeGold.ToString();
    }

    private void Start()
    {
        LayerManager.ShowLayer(Consts.Layer.HeroInfoPanel);
        LayerManager.ShowLayer(Consts.Layer.WeaponInfoPanel);
        LayerManager.ShowLayer(Consts.Layer.WeaponShopPanel);

        Invoke("startBoCi", 0.5f);

        // 神话角色列表
        {
            for (int i = 0; i < HeroEntity.getInstance().list.Count; i++)
            {
                if (HeroEntity.getInstance().list[i].quality == 4 && GameData.isUnlockHero(HeroEntity.getInstance().list[i].id))
                {
                    HeroData heroData = HeroEntity.getInstance().list[i];
                    list_mythicHero.Add(heroData);

                    Transform item = Instantiate(item_mythicHero,list_content_mythicHero).transform;
                    item.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + heroData.id);
                    item.GetComponent<Button>().onClick.AddListener(()=>
                    {
                        AudioScript.s_instance.playSound_btn();
                        GameLayer.s_instance.summonMythicHero(heroData);
                    });

                    item.gameObject.SetActive(false);
                    list_mythicHeroItem.Add(item.gameObject);

                    Transform textTrans = item.Find("Text");
                    Sequence seq = DOTween.Sequence();
                    seq.Append(textTrans.DOScale(1.2f,0.4f))
                            .Append(textTrans.DOScale(1f, 0.4f)).SetLoops(-1);
                }
            }
        }
    }

    void startBoCi()
    {
        GameFightData.s_instance.isAddEnemyEnd = false;
        ++GameFightData.s_instance.curBoCi;
        btn_spawn.localScale = Vector3.zero;

        int maxWave = EnemyWaveEntity.getInstance().list[EnemyWaveEntity.getInstance().list.Count - 1].wave;
        if (GameFightData.s_instance.curBoCi > maxWave)
        {
            GameFightData.s_instance.curBoCi = maxWave;
            gameOver();
            return;
        }

        // ToastScript.show("WAVE：" + curBoCi);

        EnemyWaveData enemyWaveData = EnemyWaveEntity.getInstance().getData(GameFightData.s_instance.curBoCi);

        GameFightData.s_instance.curBoCiRestTime = enemyWaveData.time;

        for (int i = 0; i < HeroManager.s_instance.list_hero.Count; i++)
        {
            // 角色119技能2：每波WAVE时长增加3s
            if (HeroManager.s_instance.list_hero[i].id == 119)
            {
                GameFightData.s_instance.curBoCiRestTime += 3;

                // 角色119技能1：每波WAVE开始时5%的概率消灭一半敌人
                if(RandomUtil.getRandom(1,100) <= (5 + HeroManager.s_instance.list_hero[i].getAddSkillRate()))
                {
                    if (EnemyManager.s_instance.list_enemy.Count > 1)
                    {
                        for (int j = 0; j < EnemyManager.s_instance.list_enemy.Count; j++)
                        {
                            if (EnemyManager.s_instance.list_enemy[j].enemyWaveData.enemyType == 1)
                            {
                                if (EnemyManager.s_instance.list_enemy[j].damage(EnemyManager.s_instance.list_enemy[j].curHP, false))
                                {
                                    // 消灭一半,所以得间隔一个，不用--i了
                                    // --i;
                                }
                            }
                        }
                    }
                }

                break;
            }
        }

        text_boci.text = "WAVE：" + GameFightData.s_instance.curBoCi + "/80";

        if (GameFightData.s_instance.curBoCiRestTime == 60)
        {
            text_time.text = "01:00";
        }
        else
        {
            text_time.text = "00:" + GameFightData.s_instance.curBoCiRestTime;
        }

        waitAddEnemy.Clear();
        for (int i = 0; i < enemyWaveData.count; i++)
        {
            //enemyWaveData.prefab = "enemy29";
            waitAddEnemy.Add(enemyWaveData);
        }

        InvokeRepeating("onInvokeBoCiSecond", 1, 1);
        InvokeRepeating("onInvokeAddEnemy", 0.6f, 0.6f);

        // 刷新武器商店
        WeaponShopPanel.s_instance.refreshWeapon();

        // bgm
        {
            if (GameFightData.s_instance.curBoCi % 10 == 0)
            {
                AudioScript.s_instance.playMusic("bgm_battle_boss", true);
            }
            else if (GameFightData.s_instance.curBoCi % 10 == 1)
            {
                AudioScript.s_instance.playMusic("bgm_battle", true);
            }
        }

        if (GameFightData.s_instance.curBoCi == 11)
        {
            ToastScript.show("<color=\"#698AFF\">Rare</color> Heroes Now Available!");
        }
        else if (GameFightData.s_instance.curBoCi == 21)
        {
            ToastScript.show("<color=\"#BD2DE7\">Epic</color> Heroes Now Available!");
        }
    }

    void onInvokeBoCiSecond()
    {
        if (!GameFightData.s_instance.isCanOnInvokeBoCiSecond)
        {
            return;
        }

        --GameFightData.s_instance.curBoCiRestTime;

        if (GameFightData.s_instance.curBoCiRestTime < 0)
        {
            GameFightData.s_instance.curBoCiRestTime = 0;
        }

        if (GameFightData.s_instance.curBoCiRestTime >= 10)
        {
            text_time.text = "00:" + GameFightData.s_instance.curBoCiRestTime;
        }
        else
        {
            text_time.text = "00:0" + GameFightData.s_instance.curBoCiRestTime;
        }

        checkIsShowBossRedKuang();

        if (GameFightData.s_instance.curBoCiRestTime <= 0)
        {
            CancelInvoke("onInvokeBoCiSecond");

            // 如果是boss波次，并且没有击杀，则直接游戏结束
            if (GameFightData.s_instance.curBoCi % 10 == 0)
            {
                for(int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                {
                    if (EnemyManager.s_instance.list_enemy[i].enemyWaveData.enemyType == 3)
                    {
                        gameOver();
                        return;
                    }
                }
            }

            Invoke("startBoCi", 1);

            if ((GameFightData.s_instance.curBoCi + 1) % 10 == 0)
            {
                LayerManager.ShowLayer(Consts.Layer.BossComingLayer);
            }

            // 判断场上是否有角色113，技能3：每一波结束后30%概率获得5枚金币
            for(int i = 0; i < HeroManager.s_instance.list_hero.Count; i++)
            {
                if (HeroManager.s_instance.list_hero[i].id == 113)
                {
                    if (RandomUtil.getRandom(1,100) <= (30 + HeroManager.s_instance.list_hero[i].getAddSkillRate()))
                    {
                        changeDiamond(5);
                    }
                }
            }
        }

        // boss波次,小于20秒时显示警告弹窗
        if ((GameFightData.s_instance.curBoCiRestTime == 20) && (GameFightData.s_instance.curBoCi % 10 == 0))
        {
            bool haveBoss = false;
            for(int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (EnemyManager.s_instance.list_enemy[i].enemyWaveData.enemyType == 3)
                {
                    haveBoss = true;
                    break;
                }
            }

            if (haveBoss)
            {
                LayerManager.ShowLayer(Consts.Layer.BossNoticeLayer);
            }
        }
    }

    void onInvokeAddEnemy()
    {
        if (waitAddEnemy.Count > 0)
        {
            GameLayer.s_instance.addEnemy(waitAddEnemy[0]);
            waitAddEnemy.RemoveAt(0);

            if (waitAddEnemy.Count == 0)
            {
                CancelInvoke("onInvokeAddEnemy");
                GameFightData.s_instance.isAddEnemyEnd = true;
            }
        }
    }

    public void forceToBoCi(int boci)
    {
        GameFightData.s_instance.curBoCiRestTime = 0;
        GameFightData.s_instance.curBoCi = boci - 1;
    }

    public void checkMythicHeroProgress()
    {
        int canSummonCount = 0;
        for(int i = 0; i < list_mythicHero.Count; i++)
        {
            int[] progress = GameLayer.s_instance.getMythicHeroProgress(list_mythicHero[i]);
            int okCount = 0;
            for (int j = 0; j < progress.Length; j++)
            {
                if (progress[j] == 1)
                {
                    ++okCount;
                }
            }
            if (okCount >= list_mythicHero[i].list_summonWay.Count)
            {
                ++canSummonCount;
                list_mythicHeroItem[i].SetActive(true);
            }
            else
            {
                list_mythicHeroItem[i].SetActive(false);
            }
        }

        if(canSummonCount > 0)
        {
            btn_mythicRedPoint.localScale = Vector3.one;
            text_mythicRedPointNum.text = canSummonCount.ToString();
        }
        else
        {
            btn_mythicRedPoint.localScale = Vector3.zero;
        }
    }

    public void refreshEnemyCount()
    {
        text_enemyCount.text = EnemyManager.s_instance.getEnemyCount() + "/" + Consts.maxEnemyCount;
        img_enemyCountProgress.fillAmount = EnemyManager.s_instance.getEnemyCount() / (float)Consts.maxEnemyCount;

        if (EnemyManager.s_instance.getEnemyCount() >= Consts.maxEnemyCount)
        {
            gameOver();
        }
    }

    public void checkIsShowBossRedKuang()
    {
        if(GameFightData.s_instance.curBoCi % 10 == 0 && GameFightData.s_instance.curBoCiRestTime == 20)
        {
            bossRedKuangTrans.localScale = Vector3.one;
            return;
        }

        if (EnemyManager.s_instance.getEnemyCount() >= 40)
        {
            bossRedKuangTrans.localScale = Vector3.one;
            return;
        }

        bossRedKuangTrans.localScale = Vector3.zero;
    }

    public void setIsShowBtnWeaponSell(bool isShow, WeaponData weaponData = null)
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
        GameFightData.s_instance.curGold += value;
        text_gold.text = GameFightData.s_instance.curGold.ToString();

        MoneyChangeTextPoint.s_instance.show(value, 1);

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
        if (GameFightData.s_instance.curGold >= GameFightData.s_instance.curSummonGold)
        {
            btn_summon_gold.color = Color.white;
        }
        else
        {
            btn_summon_gold.color = new Color(0.97f, 0.26f, 0.26f, 1);
        }

        // 检查锻造金额是否足够
        if (GameFightData.s_instance.curGold >= GameFightData.s_instance.curForgeGold)
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
        GameFightData.s_instance.curDiamond += value;
        text_diamond.text = GameFightData.s_instance.curDiamond.ToString();

        MoneyChangeTextPoint.s_instance.show(value, 2);

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
            AudioScript.s_instance.resumeMusic();
        }
        else
        {
            Time.timeScale = 0;
            AudioScript.s_instance.pauseMusic();
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
        LayerManager.ShowLayer(Consts.Layer.MythicHeroLayer);
    }

    // 召唤
    public void onClickHero()
    {
        AudioScript.s_instance.playSound_btn();

        if (GameFightData.s_instance.curGold < GameFightData.s_instance.curSummonGold)
        {
            AudioScript.s_instance.playSound("summonNotHaveGold");
            ToastScript.show("Coins Not Enough!");
            return;
        }

        if (GameLayer.s_instance.addHero())
        {
            int costGold = GameFightData.s_instance.curSummonGold;

            // 增加下次召唤金额
            GameFightData.s_instance.curSummonGold += Consts.summonAddGold;
            btn_summon_gold.text = GameFightData.s_instance.curSummonGold.ToString();

            changeGold(-costGold);
        }
    }

    // 锻造
    public void onClickForge()
    {
        AudioScript.s_instance.playSound_btn();

        if (GameFightData.s_instance.curGold < GameFightData.s_instance.curForgeGold)
        {
            AudioScript.s_instance.playSound("summonNotHaveGold");
            ToastScript.show("Coins Not Enough!");
            return;
        }

        bool isForgeSuccess = false;
        // int forgeCount = RandomUtil.getRandom(1, 3);
        int forgeCount = 1;
        for (int c = 0; c < forgeCount; c++)
        {
            for (int i = 0; i < weaponGridTrans.childCount; i++)
            {
                if (weaponGridTrans.GetChild(i).childCount == 0)
                {
                    isForgeSuccess = true;

                    UIItemWeapon uIItemWeapon = Instantiate(item_weapon, weaponGridTrans.GetChild(i)).GetComponent<UIItemWeapon>();
                    int weaponLevel = RandomUtil.SelectProbability(GameFightData.s_instance.list_weaponWeight) + 1;
                    uIItemWeapon.init(RandomUtil.getRandom(1,5), weaponLevel);
                    break;
                }
            }
        }

        if (isForgeSuccess)
        {
            int costGold = GameFightData.s_instance.curForgeGold;

            // 增加下次锻造金额
            GameFightData.s_instance.curForgeGold += Consts.forgeAddGold;
            btn_forge_gold.text = GameFightData.s_instance.curForgeGold.ToString();

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

    Sequence seq_bell = null;
    public void setIsShowBell(bool isShow)
    {
        if(isShow)
        {
            bellTrans.localScale = Vector3.one;

            if(seq_bell == null)
            {
                seq_bell = DOTween.Sequence();
                seq_bell.Append(bellTrans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 15), 0.08f).SetEase(Ease.Linear))
                        .Append(bellTrans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -15), 0.16f).SetEase(Ease.Linear))
                        .Append(bellTrans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 15), 0.08f).SetEase(Ease.Linear))
                        .Append(bellTrans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -15), 0.16f).SetEase(Ease.Linear))
                        .Append(bellTrans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0.08f))
                        .Append(bellTrans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 1.5f).SetEase(Ease.Linear)).SetLoops(-1);
                seq_bell.SetAutoKill(false);
            }
            else
            {
                seq_bell.Restart();
            }

            //AudioScript.s_instance.playSound("shopNotice");
        }
        else
        {
            bellTrans.localScale = Vector3.zero;

            if (seq_bell != null)
            {
                seq_bell.Pause();
            }
        }
    }

    public void onClickShop()
    {
        AudioScript.s_instance.playSound_btn();

        setIsShowBell(false);
        WeaponShopPanel.s_instance.show();
    }

    public void onClickTimeScale()
    {
        AudioScript.s_instance.playSound_btn();

        if(GameFightData.s_instance.gameTimeScale == 1)
        {
            GameFightData.s_instance.gameTimeScale = 1.5f;
        }
        else
        {
            GameFightData.s_instance.gameTimeScale = 1;
        }
        text_timeScale.text = "x"+GameFightData.s_instance.gameTimeScale;
        Time.timeScale = GameFightData.s_instance.gameTimeScale;
    }

    public void onClickSpawn()
    {
        AudioScript.s_instance.playSound_btn();
        forceToBoCi(GameFightData.s_instance.curBoCi + 1);
        btn_spawn.localScale = Vector3.zero;
    }

    bool isCalledGameOver = false;
    public void gameOver()
    {
        if(isCalledGameOver)
        {
            return;
        }

        isCalledGameOver = true;
        GameFightData.s_instance.isGameOver = true;
        CancelInvoke("startBoCi");
        CancelInvoke("onInvokeBoCiSecond");
        CancelInvoke("onInvokeAddEnemy");
        LayerManager.ShowLayer(Consts.Layer.GameOverLayer);
        AudioScript.s_instance.stopMusic();
    }

    private void OnDestroy()
    {
        if (tween_changeDiamondIcon != null)
        {
            tween_changeDiamondIcon.Kill();
        }

        if (tween_changeDiamondText != null)
        {
            tween_changeDiamondText.Kill();
        }

        if (seq_bell != null)
        {
            seq_bell.Kill();
        }
    }
}
