using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroUpgradeLayer : MonoBehaviour
{
    public Text text_name;
    public Text text_level;
    public Text text_skillDesc;
    public Text text_atk;
    public Text text_atk_speed;
    public Text text_crit_rate;
    public Text text_crit_damage;
    public Image img_icon;
    public Transform weaponBuffsTrans;
    public Transform skillTrans;

    HeroSkillData[] skillsArray;
    HeroLogicBase heroLogicBase;

    public void init(HeroLogicBase _heroLogicBase)
    {
        heroLogicBase = _heroLogicBase;

        text_name.text = heroLogicBase.name;
        text_level.text = "Lv."+GameData.getHeroLevel(heroLogicBase.id);
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroLogicBase.id);

        // 技能
        {
            string[] array = heroLogicBase.heroData.skills.Split("_");
            skillsArray = new HeroSkillData[array.Length];
            for (int i = 0; i < skillTrans.childCount; i++)
            {
                if ((i + 1) <= array.Length)
                {
                    skillsArray[i] = HeroSkillEntity.getInstance().getData(int.Parse(array[i]));
                    skillTrans.Find(i.ToString()).gameObject.SetActive(true);
                    skillTrans.Find(i + "/mask/icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("skill_" + array[i]);
                }
                else
                {
                    skillTrans.Find(i.ToString()).gameObject.SetActive(false);
                }
            }
        }
    }

    public void onClickSkillIcon(int index)
    {
        AudioScript.s_instance.playSound_btn();

        for (int i = 0; i < skillTrans.childCount; i++)
        {
            if (i == index)
            {
                skillTrans.Find(i + "/choiced").localScale = Vector3.one;
                text_skillDesc.text = skillsArray[index].desc;
            }
            else
            {
                skillTrans.Find(i + "/choiced").localScale = Vector3.zero;
            }
        }
    }

    public void onClickUpgrade()
    {
        AudioScript.s_instance.playSound_btn();
    }

    public void onClickClose()
    {
        AudioScript.s_instance.playSound_btn();

        Destroy(gameObject);
    }
}
