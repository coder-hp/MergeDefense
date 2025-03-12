using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBar : MonoBehaviour
{
    public Text text_level;
    public Image img_icon;

    [HideInInspector]
    public WeaponData weaponData;

    public void setData(WeaponData _weaponData)
    {
        if(weaponData != null && _weaponData.type == weaponData.type && _weaponData.level == weaponData.level && weaponData.level < 10)
        {
            weaponData = WeaponEntity.getInstance().getData(weaponData.type, weaponData.level + 1);
        }
        else
        {
            if (weaponData != null)
            {
                GameUILayer.s_instance.addWeapon(weaponData);
            }

            weaponData = _weaponData;
        }
        text_level.text = weaponData.level.ToString();
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + weaponData.type);

        transform.Find("bg").localScale = Vector3.one;

        AudioScript.s_instance.playSound("equipWeapon");
    }

    public void onClick()
    {
        if (weaponData != null)
        {
            AudioScript.s_instance.playSound_btn();
            WeaponInfoPanel.s_instance.show(weaponData);
        }
    }
}
