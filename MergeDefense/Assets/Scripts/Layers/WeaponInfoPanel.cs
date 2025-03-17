using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoPanel : MonoBehaviour
{
    public static WeaponInfoPanel s_instance = null;

    public Image img_icon;
    public Text txt_name;
    public Text txt_level;
    public Transform weaponBuffsTrans;
    public Transform heroListTrans;

    void Awake()
    {
        s_instance = this;
        gameObject.SetActive(false);
    }

    public void show(WeaponData weaponData)
    {
        gameObject.SetActive(true);

        txt_name.text = weaponData.name;
        txt_level.text = weaponData.level.ToString();
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + weaponData.type);

        // 武器buff
        {
            // 攻击力
            weaponBuffsTrans.Find("buff1/value").GetComponent<Text>().text = weaponData.buff1.ToString();

            // 攻击力百分比加成
            weaponBuffsTrans.Find("buff2/value").GetComponent<Text>().text = Mathf.RoundToInt(weaponData.buff2 * 100f) + "%";

            // 第三个任意属性Buff
            {
                weaponBuffsTrans.Find("buff3/icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("buff_" + (int)weaponData.buff3Type);
                float buff3Value = weaponData.buff3Value;
                switch (weaponData.buff3Type)
                {
                    case Consts.BuffType.CritRate:
                        {
                            weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Crit Rate";

                            int value = Mathf.RoundToInt(buff3Value);
                            weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                            break;
                        }

                    case Consts.BuffType.CritDamage:
                        {
                            weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Crit Damage";

                            int value = Mathf.RoundToInt(buff3Value * 100);
                            weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                            break;
                        }

                    case Consts.BuffType.AtkSpeed:
                        {
                            weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Attack Speed";
                            weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = buff3Value.ToString();
                            break;
                        }

                    case Consts.BuffType.SkillRate:
                        {
                            weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Proc Chance";

                            int value = Mathf.RoundToInt(buff3Value);
                            weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                            break;
                        }
                }
            }
        }

        // 可使用的角色
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int j = 0; j < HeroManager.s_instance.list_hero.Count; j++)
            {
                HeroLogicBase heroLogicBase = HeroManager.s_instance.list_hero[j];
                if (heroLogicBase.heroData.goodWeapon == weaponData.type)
                {
                    if (dic.ContainsKey(heroLogicBase.id))
                    {
                        ++dic[heroLogicBase.id];
                    }
                    else
                    {
                        dic[heroLogicBase.id] = 1;
                    }
                }
            }

            for(int i = 0; i < heroListTrans.childCount; i++)
            {
                heroListTrans.GetChild(i).gameObject.SetActive(false);
            }

            int index = -1;
            foreach (KeyValuePair<int, int> kvp in dic)
            {
                ++index;
                Transform item = heroListTrans.GetChild(index);
                item.gameObject.SetActive(true);
                item.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + kvp.Key);
                item.Find("count").GetComponent<Text>().text = "x" + kvp.Value;
            }
        }
    }

    public void close()
    {
        gameObject.SetActive(false);
    }

    public void onClickClose()
    {
        close();
    }
}
