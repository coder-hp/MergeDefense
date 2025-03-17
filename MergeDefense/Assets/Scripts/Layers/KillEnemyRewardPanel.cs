using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillEnemyRewardPanel : MonoBehaviour
{
    public static KillEnemyRewardPanel s_instance = null;

    public Transform reward_money;
    public Transform reward_hero;
    public Transform reward_weapon;
    public Transform buff_hero;
    public Transform buff_weapon;
    public Text text_title;
    public Text text_diamond;

    EnemyWaveData enemyWaveData;
    KillRewardData killRewardData;

    int heroId;
    int heroStar;
    int weaponType;
    int weaponLevel;

    private void Awake()
    {
        s_instance = this;
    }

    public void show(EnemyWaveData _enemyWaveData)
    {
        AudioScript.s_instance.playSound("killEnemyRewardPanel");

        GameFightData.s_instance.isCanOnInvokeBoCiSecond = false;
        LayerManager.LayerShowAni(transform.Find("bg"));

        enemyWaveData = _enemyWaveData;
        killRewardData = KillRewardEntity.getInstance().getData(enemyWaveData.wave);

        GameFightData.s_instance.changeHeroWeight(killRewardData.list_heroWeight);
        GameFightData.s_instance.changeWeaponWeight(killRewardData.list_weaponWeight);

        // 精英敌人
        if (enemyWaveData.enemyType == 2)
        {
            text_title.text = "ELITE ENEMY SLAIN";

            buff_weapon.localScale = Vector3.one;
            buff_hero.localScale = Vector3.zero;
        }
        // Boss敌人
        else if (enemyWaveData.enemyType == 3)
        {
            text_title.text = "BOSS SLAIN";

            buff_weapon.localScale = Vector3.zero;
            buff_hero.localScale = Vector3.one;
        }

        if(enemyWaveData.wave == 10)
        {
            GameFightData.s_instance.list_canSummonHero.Add(106);
            GameFightData.s_instance.list_canSummonHero.Add(107);
            GameFightData.s_instance.list_canSummonHero.Add(108);
        }
        else if (enemyWaveData.wave == 20)
        {
            GameFightData.s_instance.list_canSummonHero.Add(109);
            GameFightData.s_instance.list_canSummonHero.Add(110);
        }

        text_diamond.text = "+" + killRewardData.diamond;

        if (killRewardData.heroStar != 0)
        {
            if(killRewardData.heroQuality == -1)
            {
                heroId = GameFightData.s_instance.randomSummonHero();
            }
            else
            {
                List<int> list_hero = new List<int>();
                for (int i = 0; i < HeroEntity.getInstance().list.Count; i++)
                {
                    if (HeroEntity.getInstance().list[i].quality == killRewardData.heroQuality)
                    {
                        list_hero.Add(HeroEntity.getInstance().list[i].id);
                    }
                }
                if (list_hero.Count > 0)
                {
                    heroId = list_hero[RandomUtil.getRandom(0, list_hero.Count - 1)];
                }
                else
                {
                    heroId = GameFightData.s_instance.randomSummonHero();
                }
            }

            heroStar = killRewardData.heroStar;

            HeroData heroData = HeroEntity.getInstance().getData(heroId);
            reward_hero.Find("head_bg/name_bg/name").GetComponent<Text>().text = heroData.name;
            reward_hero.Find("head_bg/head_mask/head").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroId);
            reward_hero.Find("head_bg/kuang").GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_1");
            reward_hero.Find("head_bg").GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_2");
            reward_hero.Find("head_bg/shadow").GetComponent<Image>().color = Consts.list_heroQualityColor[heroData.quality];
        }
        else
        {
            reward_hero.gameObject.SetActive(false);
        }

        if (killRewardData.weaponLevel != 0)
        {
            weaponType = RandomUtil.getRandom(1,5);
            weaponLevel = killRewardData.weaponLevel;

            WeaponData weaponData = WeaponEntity.getInstance().getData(weaponType, weaponLevel);

            Transform weaponTrans = reward_weapon.Find("item_weapon");
            weaponTrans.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + weaponData.type);
            weaponTrans.Find("level").GetComponent<Text>().text = weaponData.level.ToString();
            reward_weapon.Find("name").GetComponent<Text>().text = weaponData.name;
        }
        else
        {
            reward_weapon.gameObject.SetActive(false);
        }

        Invoke("onClickClose",5);
    }

    bool isClosed = false;
    public void onClickClose()
    {
        if(isClosed)
        {
            return;
        }
        isClosed = true;

        AudioScript.s_instance.playSound_btn();

        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            GameUILayer.s_instance.changeDiamond(killRewardData.diamond);
            if (killRewardData.heroStar != 0)
            {
                GameLayer.s_instance.addHeroByIdStar(heroId, heroStar);
            }
            else if (killRewardData.weaponLevel != 0)
            {
                GameUILayer.s_instance.addWeapon(WeaponEntity.getInstance().getData(weaponType, weaponLevel));
            }

            if (enemyWaveData.enemyType == 3)
            {
                LayerManager.ShowLayer(Consts.Layer.BossRewardPanel).GetComponent<BossRewardPanel>().init(enemyWaveData, killRewardData);
            }
            else
            {
                GameFightData.s_instance.isCanOnInvokeBoCiSecond = true;
            }

            Destroy(gameObject);
        });
    }
}
