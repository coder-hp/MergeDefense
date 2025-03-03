using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnWeaponSellEvent : MonoBehaviour
{
    public static BtnWeaponSellEvent s_instance = null;

    [HideInInspector]
    public UIItemWeapon itemWeapon;

    private void Awake()
    {
        s_instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UIItemWeapon"))
        {
            itemWeapon = collision.GetComponent<UIItemWeapon>();
            transform.DOScale(1.2f, 0.2f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("UIItemWeapon"))
        {
            itemWeapon = null;
            transform.DOScale(1f, 0.2f);
        }
    }
}
