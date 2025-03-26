using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SummonSpecialHeroEye : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        SummonSpecialHero.s_instance.setIsShow(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SummonSpecialHero.s_instance.setIsShow(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SummonSpecialHero.s_instance.setIsShow(true);
    }
}
