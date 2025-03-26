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
        if (_weaponData != null)
        {
            if (weaponData != null && _weaponData.type == weaponData.type && _weaponData.level == weaponData.level && weaponData.level < 10)
            {
                weaponData = WeaponEntity.getInstance().getData(weaponData.type, weaponData.level + 1);
                GameUILayer.s_instance.checkMythicHeroProgress();
            }
            else
            {
                if (weaponData != null)
                {
                    GameUILayer.s_instance.addWeapon(weaponData);
                }

                weaponData = _weaponData;
            }

            if (weaponData != null)
            {
                text_level.text = weaponData.level.ToString();
                img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + weaponData.type);

                transform.Find("bg").localScale = Vector3.one;

                AudioScript.s_instance.playSound("equipWeapon");
            }
            else
            {
                transform.Find("bg").localScale = Vector3.zero;
            }

            // 小船任务
            if (BattleMission.s_instance.curMissionData != null && BattleMission.s_instance.isTakeMission)
            {
                switch (BattleMission.s_instance.curMissionData.id)
                {
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        {
                            BattleMission.s_instance.checkWeaponMission();
                            break;
                        }
                }
            }
        }
        else
        {
            weaponData = null;
            transform.Find("bg").localScale = Vector3.zero;
        }
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
