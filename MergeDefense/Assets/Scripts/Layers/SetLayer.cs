using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLayer : MonoBehaviour
{
    public Transform panel_set;
    public Transform panel_changeHead;
    public Transform panel_changeName;
    public Transform list_content_head;
    public GameObject item_head;
    public Image img_head;
    public Image img_head2;
    public Text text_name;
    public InputField inputField_name;

    int curClickHeadItemId = 0;

    void Start()
    {
        text_name.text = GameData.getName();
        inputField_name.text = text_name.text;
        img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + GameData.getHead());
        img_head2.sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + GameData.getHead());

        LayerManager.LayerShowAni(panel_set);
    }

    public void onClickHead()
    {
        AudioScript.s_instance.playSound_btn();

        panel_set.localScale = Vector3.zero;
        panel_changeHead.gameObject.SetActive(true);
        panel_changeHead.localScale = Vector3.zero;
        LayerManager.LayerShowAni(panel_changeHead);

        if(list_content_head.childCount == 0)
        {
            for(int i = 0; i < HeroEntity.getInstance().list.Count; i++)
            {
                int id = HeroEntity.getInstance().list[i].id;
                Transform itemTrans = Instantiate(item_head,list_content_head).transform;
                itemTrans.name = id.ToString();
                itemTrans.Find("head").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + id);
                if(GameData.getHead() == id)
                {
                    itemTrans.Find("used").localScale = Vector3.one;
                }
                else
                {
                    itemTrans.Find("used").localScale = Vector3.zero;
                }

                if (GameData.isUnlockHero(id))
                {
                    itemTrans.Find("lock").localScale = Vector3.zero;
                }
                else
                {
                    itemTrans.Find("lock").localScale = Vector3.one;
                }

                itemTrans.GetComponent<Button>().onClick.AddListener(()=>
                {
                    AudioScript.s_instance.playSound_btn();
                    onClickHeadItem(id);
                });
            }
            onClickHeadItem(GameData.getHead());
        }
    }

    void onClickHeadItem(int id)
    {
        if(curClickHeadItemId == id)
        {
            return;
        }

        curClickHeadItemId = id;
        for (int i = 0; i < list_content_head.childCount; i++)
        {
            Transform itemTrans = list_content_head.GetChild(i);
            if (int.Parse(itemTrans.name) == id)
            {
                itemTrans.GetComponent<Image>().color = CommonUtil.stringToColor("#9ea8e3");
                itemTrans.Find("kuang").GetComponent<Image>().color = CommonUtil.stringToColor("#afb8ef");
            }
            else
            {
                itemTrans.GetComponent<Image>().color = CommonUtil.stringToColor("#6f79b3");
                itemTrans.Find("kuang").GetComponent<Image>().color = CommonUtil.stringToColor("#8892cf");
            }
        }
    }

    public void onClickChangeHead()
    {
        AudioScript.s_instance.playSound_btn();

        if (GameData.isUnlockHero(curClickHeadItemId))
        {
            GameData.setHead(curClickHeadItemId);
            img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + curClickHeadItemId);
            img_head2.sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + curClickHeadItemId);

            for (int i = 0; i < list_content_head.childCount; i++)
            {
                Transform itemTrans = list_content_head.GetChild(i);
                if (int.Parse(itemTrans.name) == curClickHeadItemId)
                {
                    itemTrans.Find("used").localScale = Vector3.one;
                }
                else
                {
                    itemTrans.Find("used").localScale = Vector3.zero;
                }
            }
        }
    }

    public void onClickChangeName()
    {
        if (inputField_name.text.Length >= 2)
        {
            GameData.setName(inputField_name.text);
            text_name.text = inputField_name.text;
            onClickClose();
        }
        else
        {
            ToastScript.show("最少两个字符");
        }
    }

    public void onClickName()
    {
        AudioScript.s_instance.playSound_btn();

        panel_set.localScale = Vector3.zero;
        panel_changeName.gameObject.SetActive(true);
        panel_changeName.localScale = Vector3.zero;
        LayerManager.LayerShowAni(panel_changeName);
    }

    bool isClosed = false;
    public void onClickClose()
    {
        if (isClosed)
        {
            return;
        }
        isClosed = true;

        AudioScript.s_instance.playSound_btn();

        if(panel_changeHead.gameObject.activeInHierarchy && panel_changeHead.localScale.x > 0)
        {
            LayerManager.LayerCloseAni(panel_changeHead, () =>
            {
                Destroy(gameObject);
            });
        }
        else if (panel_changeName.gameObject.activeInHierarchy && panel_changeName.localScale.x > 0)
        {
            LayerManager.LayerCloseAni(panel_changeName, () =>
            {
                Destroy(gameObject);
            });
        }
        else
        {
            LayerManager.LayerCloseAni(panel_set, () =>
            {
                Destroy(gameObject);
            });
        }
    }
}
