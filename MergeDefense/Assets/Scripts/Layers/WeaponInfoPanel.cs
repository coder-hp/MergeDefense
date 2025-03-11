using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Consts;

public class WeaponInfoPanel : MonoBehaviour
{
    public static WeaponInfoPanel s_instance = null;

    public Image img_icon;
    public Text txt_name;
    public Text txt_level;
    public Transform weaponBuffsTrans;

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
