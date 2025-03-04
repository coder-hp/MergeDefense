using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillEnemyRewardPanel : MonoBehaviour
{
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

    public void show(EnemyWaveData _enemyWaveData)
    {
        AudioScript.s_instance.playSound("killEnemyRewardPanel");

        GameUILayer.s_instance.isCanOnInvokeBoCiSecond = false;
        LayerManager.LayerShowAni(transform.Find("bg"));

        enemyWaveData = _enemyWaveData;
        killRewardData = KillRewardEntity.getInstance().getData(enemyWaveData.wave);

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

        text_diamond.text = "+"+killRewardData.diamond;

        if (killRewardData.heroStar != 0)
        {
            heroId = RandomUtil.getRandom(101, GameLayer.s_instance.getSummonHeroMaxId());
            heroStar = killRewardData.heroStar;

            HeroData heroData = HeroEntity.getInstance().getData(heroId);
            reward_hero.Find("head_bg/name_bg/name").GetComponent<Text>().text = heroData.name;
            reward_hero.Find("head_bg/head").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroId);
            reward_hero.Find("head_bg/kuang").GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_1");
            reward_hero.Find("head_bg").GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_2");

            if (heroData.quality == 2)
            {
                reward_hero.Find("head_bg/shadow").GetComponent<Image>().color = CommonUtil.stringToColor("#457dd8");
            }
            else if (heroData.quality == 3)
            {
                reward_hero.Find("head_bg/shadow").GetComponent<Image>().color = CommonUtil.stringToColor("#9146da");
            }
            else if (heroData.quality == 4)
            {
                reward_hero.Find("head_bg/shadow").GetComponent<Image>().color = CommonUtil.stringToColor("#eb9b10");
            }
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
            weaponTrans.Find("frame").GetComponent<Image>().color = Consts.list_weaponColor[weaponData.type];
            weaponTrans.Find("level_bg").GetComponent<Image>().color = Consts.list_weaponColor[weaponData.type];
            weaponTrans.Find("level_bg/level").GetComponent<Text>().text = weaponData.level.ToString();
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
            GameUILayer.s_instance.isCanOnInvokeBoCiSecond = true;
            GameUILayer.s_instance.changeDiamond(killRewardData.diamond);
            if(killRewardData.heroStar != 0)
            {
                GameLayer.s_instance.addHeroByIdStar(heroId, heroStar);
            }
            else if (killRewardData.weaponLevel != 0)
            {
                GameUILayer.s_instance.addWeapon(WeaponEntity.getInstance().getData(weaponType,weaponLevel));
            }

            Destroy(gameObject);

            // 如果场上没有敌人，直接开始下一波
            if(EnemyManager.s_instance.list_enemy.Count == 0)
            {
                GameUILayer.s_instance.forceToBoCi(enemyWaveData.wave + 1);
            }
        });
    }
}
