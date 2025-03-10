using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBar : MonoBehaviour
{
    public Text text_level;

    [HideInInspector]
    public WeaponData weaponData;

    public void setData(WeaponData _weaponData)
    {
        weaponData = _weaponData;
        text_level.text = weaponData.level.ToString();

        transform.Find("bg").localScale = Vector3.one;
    }
}
