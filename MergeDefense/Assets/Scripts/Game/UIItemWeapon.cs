using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemWeapon : MonoBehaviour
{
    public Image img_icon;
    public Image img_level_bg;
    public Text text_level;

    [HideInInspector]
    public WeaponData weaponData;

    Transform parentTrans;
    Transform mergeTarget = null;

    RaycastHit raycastHit;

    public void init(int type,int level)
    {
        parentTrans = transform.parent;

        weaponData = WeaponEntity.getInstance().getData(type, level);

        text_level.text = level.ToString();

        // 品质框
        transform.GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("dikuang_1");

        // 武器icon
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + type);

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

            // 拳套
            case 4:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#FF8A8C");
                    break;
                }

            // 魔杖
            case 5:
                {
                    img_level_bg.color = CommonUtil.stringToColor("#E3A9FF");
                    break;
                }

            // 

        }
    }

    public void addLevel()
    {
        if(weaponData.level >= 10)
        {
            return;
        }

        weaponData = WeaponEntity.getInstance().getData(weaponData.type, weaponData.level + 1);
        text_level.text = weaponData.level.ToString();
    }

    private void OnMouseDown()
    {
        mergeTarget = null;
        transform.SetParent(GameUILayer.s_instance.transform);
    }

    private void OnMouseDrag()
    {
        transform.localPosition = CommonUtil.getCurMousePosToUI();

        Transform trans = getMinDisItemWeapon();
        if (trans)
        {
            if (Vector2.Distance(trans.position, transform.position) <= 0.5f)
            {
                // 先重置之前选中的格子
                if (mergeTarget && mergeTarget != trans)
                {
                    mergeTarget.GetComponent<Image>().color = Color.white;
                    mergeTarget.DOScale(1f, 0.2f);
                }

                mergeTarget = trans;
                mergeTarget.GetComponent<Image>().color = Color.green;
                mergeTarget.DOScale(1.2f, 0.2f);
            }
            else if (mergeTarget)
            {
                mergeTarget.GetComponent<Image>().color = Color.white;
                mergeTarget.DOScale(1f, 0.2f);
                mergeTarget = null;
            }
        }
    }

    private void OnMouseUp()
    {
        if (mergeTarget)
        {
            if (mergeTarget.childCount > 0)
            {
                UIItemWeapon uIItemWeapon = mergeTarget.GetChild(0).GetComponent<UIItemWeapon>();
                if (uIItemWeapon.weaponData.type == weaponData.type && uIItemWeapon.weaponData.level == weaponData.level && uIItemWeapon.weaponData.level <= 9)
                {
                    mergeTarget.GetComponent<Image>().color = Color.white;
                    mergeTarget.DOScale(1f, 0.2f);
                    uIItemWeapon.addLevel();
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                transform.SetParent(mergeTarget);
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;

                mergeTarget.GetComponent<Image>().color = Color.white;
                mergeTarget.DOScale(1f, 0.2f);
                return;
            }
        }

        if (mergeTarget)
        {
            mergeTarget.GetComponent<Image>().color = Color.white;
            mergeTarget.DOScale(1f, 0.2f);
        }

        // 检测是否拖到角色上
        {
            raycastHit = RayUtil.getEndPoint(CommonUtil.mousePosToWorld(GameLayer.s_instance.camera3D));
            if (raycastHit.collider && raycastHit.collider.CompareTag("Hero"))
            {
                raycastHit.collider.GetComponent<HeroLogicBase>().addWeapon(weaponData);
                Destroy(gameObject);
                return;
            }
            else
            {
                // 未拖到角色上
            }
        }

        transform.SetParent(parentTrans);
        transform.DOLocalMove(Vector3.zero, 0.2f);
    }

    Transform getMinDisItemWeapon()
    {
        float tempDis;
        float minDis = 999;
        Transform trans = null;
        for (int i = 0; i < GameUILayer.s_instance.weaponGridTrans.childCount; i++)
        {
            // 不限制是否为空格子
            // if (GameUILayer.s_instance.weaponGridTrans.GetChild(i).childCount == 1)
            {
                tempDis = Vector2.Distance(GameUILayer.s_instance.weaponGridTrans.GetChild(i).position, transform.position);
                if (tempDis < minDis)
                {
                    minDis = tempDis;
                    trans = GameUILayer.s_instance.weaponGridTrans.GetChild(i);
                }
            }
        }

        return trans;
    }
}
