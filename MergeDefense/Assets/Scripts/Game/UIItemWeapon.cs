using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemWeapon : MonoBehaviour
{
    public Image img_level_bg;
    public Text text_level;

    [HideInInspector]
    public WeaponData weaponData;

    public void init(int type,int level)
    {
        weaponData = WeaponEntity.getInstance().getData(type, level);

        text_level.text = level.ToString();
        transform.GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + type);

        switch (weaponData.type)
        {
            // 剑
            case 1:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#FFF04C");
                    break;
                }

            // 弓
            case 2:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#CBF736");
                    break;
                }

            // 斧
            case 3:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#FFB14C");
                    break;
                }

            // 手里剑
            case 4:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#5CE4FF");
                    break;
                }

            // 魔杖
            case 5:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#E3A9FF");
                    break;
                }

            // 拳套
            case 6:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#FF8A8C");
                    break;
                }
        }
    }
}
