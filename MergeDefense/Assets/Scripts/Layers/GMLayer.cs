using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GMLayer : MonoBehaviour
{
    public static GMLayer s_instance = null;

    public bool isDebug = false;
    public GameObject bg;
    public Text text_fps;
    public Transform addHeroPanel;
    public GameObject prefab_addHero;
    public InputField inputField_level;

    bool isShowFPS = false;

    private void Awake()
    {
        s_instance = this;

        if(isDebug)
        {
            text_fps.gameObject.SetActive(true);
        }

        for(int i = 0; i < HeroEntity.getInstance().list.Count; i++)
        {
            int id = HeroEntity.getInstance().list[i].id;
            Transform trans = Instantiate(prefab_addHero, addHeroPanel).transform;
            trans.GetChild(0).GetComponent<Text>().text = HeroEntity.getInstance().list[i].name;
            trans.GetComponent<Button>().onClick.AddListener(()=>
            {
                GameLayer.s_instance.addHeroById(id);
            });
        }
    }

    int fpsIndex = 0;
    Vector2 touchBegin;
    void Update()
    {
        if (isDebug)
        {
            transform.SetAsLastSibling();

            if (Input.GetMouseButtonDown(0))
            {
                touchBegin = CommonUtil.getCurMousePosToUI();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 touchEnd = CommonUtil.getCurMousePosToUI();

                if ((touchBegin.y >= (1440 / 2 - 200)) && (touchEnd.y >= (1440 / 2 - 200)))
                {
                    if ((touchEnd.x - touchBegin.x) > 100)
                    {
                        bg.SetActive(true);
                    }
                }
            }

            if (isShowFPS && ++fpsIndex == 15)
            {
                fpsIndex = 0;
                text_fps.text = "FPS:" + (int)(1f / Time.deltaTime);
            }
        }

        //if(Input.GetKeyDown(KeyCode.W))
        //{
        //    LayerManager.ShowLayer(Consts.Layer.KillEnemyRewardPanel).GetComponent<KillEnemyRewardPanel>().show(EnemyWaveEntity.getInstance().getData(10));
        //}
    }

    public void onClickClearData()
    {
        PlayerPrefs.DeleteAll();

        // 删除缓存文件
        //{
        //    DirectoryInfo TheFolder = new DirectoryInfo(DownScript.s_instance.savePath);
        //    foreach (FileInfo file in TheFolder.GetFiles())
        //    {
        //        File.Delete(file.FullName);
        //    }
        //}

        ToastScript.show("数据清除完毕，重启pp");
        onClickClose();
    }

    public void onClickAddGold()
    {
        if(GameUILayer.s_instance)
        {
            GameUILayer.s_instance.changeGold(1000);
        }
        ToastScript.show("+1000");
    }

    public void onClickAddDiamond()
    {
        if (GameUILayer.s_instance)
        {
            GameUILayer.s_instance.changeDiamond(1000);
        }
        ToastScript.show("+1000");
    }

    public void onClickUnlockAll()
    {
        
        ToastScript.show("解锁成功");
        onClickClose();
    }

    public void onClickRefreshWeaponShop()
    {
        WeaponShopPanel.s_instance.refreshWeapon();
    }

    public void onClickFPS()
    {
        isShowFPS = true;
        onClickClose();
    }

    public void onClickDeleteRes()
    {
        Resources.UnloadUnusedAssets();
    }

    public void onClickChangeLevel()
    {
        if(inputField_level.text != "")
        {
            GameUILayer.s_instance.forceToBoCi(int.Parse(inputField_level.text));
            onClickClose();
        }
    }

    public void onClickGameOver()
    {
        if (GameUILayer.s_instance)
        {
            GameUILayer.s_instance.gameOver();
        }
    }

    public void onClickAddHeroRate()
    {
        if (GameFightData.s_instance)
        {
            GameFightData.s_instance.changeHeroHighStarRate(20);
        }
    }

    public void onClickAddWeaponRate()
    {
        if (GameFightData.s_instance)
        {
            GameFightData.s_instance.changeWeaponHighLevelRate(10);
        }
    }

    public void onClickClose()
    {
        bg.SetActive(false);
    }
}
