using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnSellHero : MonoBehaviour
{
    public static BtnSellHero s_instance = null;

    public bool isClicked = false;

    private void Awake()
    {
        s_instance = this;
    }

    private void OnMouseDown()
    {
        isClicked = true;
    }

    private void OnMouseExit()
    {
        isClicked = false;
    }

    private void OnMouseUp()
    {
        isClicked = false;
    }
}
