using Newtonsoft.Json;
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
    public InputField inputField_hero;
    public InputField inputField_weapon;
    public InputField inputField_rank;
    public InputField inputField_HeroExp;

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
            trans.GetChild(0).GetComponent<Text>().text = HeroEntity.getInstance().list[i].id + "-" + HeroEntity.getInstance().list[i].name;
            trans.GetComponent<Button>().onClick.AddListener(()=>
            {
                if(HeroEntity.getInstance().getData(id).quality == 4)
                {
                    GameLayer.s_instance.addHeroByIdStar(id,10);
                }
                else
                {
                    GameLayer.s_instance.addHeroById(id);
                }
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
            else if (Input.GetKeyDown(KeyCode.M))
            {
                LayerManager.ShowLayer(Consts.Layer.GetMythicHeroLayer).GetComponent<GetMythicHeroLayer>().init(112);
            }

            if (isShowFPS && ++fpsIndex == 15)
            {
                fpsIndex = 0;
                text_fps.text = "FPS:" + (int)(1f / Time.deltaTime);
            }
        }
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

    public void onClickAddMyGold()
    {
        GameData.changeMyGold(10000,"");
        ToastScript.show("+10000");
    }

    public void onClickAddMyDiamond()
    {
        GameData.changeMyDiamond(1000);
        ToastScript.show("+1000");
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
            //GameFightData.s_instance.changeHeroHighStarRate(20);
            GameFightData.s_instance.changeHeroHighStarRate(140);
        }
    }

    public void onClickAddWeaponRate()
    {
        if (GameFightData.s_instance)
        {
            GameFightData.s_instance.changeWeaponHighLevelRate(10);
        }
    }

    public void onClickUnlockAllHero()
    {
        for(int i = 0; i < HeroEntity.getInstance().list.Count; i++)
        {
            if (HeroEntity.getInstance().list[i].quality == 4)
            {
                GameData.unlockHero(HeroEntity.getInstance().list[i].id);
            }
        }
        ToastScript.show("解锁成功");
    }

    public void onClickAddHero()
    {
        if(inputField_hero.text != "")
        {
            int id = int.Parse(inputField_hero.text.Split('_')[0]);
            int star = int.Parse(inputField_hero.text.Split('_')[1]);
            GameLayer.s_instance.addHeroByIdStar(id, star);
        }
    }

    public void onClickAddWeapon()
    {
        if (inputField_weapon.text != "")
        {
            int type = int.Parse(inputField_weapon.text.Split('_')[0]);
            int level = int.Parse(inputField_weapon.text.Split('_')[1]);
            GameUILayer.s_instance.addWeapon(WeaponEntity.getInstance().getData(type, level));
        }
    }

    public void onClickRank()
    {
        if (inputField_rank.text != "")
        {
            int wave = int.Parse(inputField_rank.text.Split('_')[0]);
            int damage = int.Parse(inputField_rank.text.Split('_')[1]);

            ReqDataSubmitRankData reqData = new ReqDataSubmitRankData();
            reqData.rankType = RankType.GlobalRank.ToString();
            reqData.uid = GameData.getUID();
            reqData.name = GameData.getName();
            reqData.score = wave;
            reqData.score2 = damage;
            string reqDataStr = JsonConvert.SerializeObject(reqData);
            HttpUtil.s_instance.reqPost(Consts.getServerUrl() + ServerInterface.submitRankData.ToString(), reqDataStr, (result, data) =>
            {
                if (result)
                {
                    BackDataSubmitRankData backData = JsonConvert.DeserializeObject<BackDataSubmitRankData>(data);
                    if (backData.serverCode == ServerCode.OK)
                    {
                        Debug.Log("排行数据提交成功");
                    }
                    else
                    {
                        Debug.Log("排行数据提交失败1：" + backData.desc);
                    }
                }
                else
                {
                    Debug.Log("排行数据提交失败2：" + data);
                }
            });
        }
    }

    public void onClickAddSkillRate()
    {
        if (GameFightData.s_instance)
        {
            GameFightData.s_instance.addGlobalHeroBuff(new Consts.BuffData(Consts.BuffType.SkillRate, 100, 999, "GM", true, false));
            ToastScript.show("技能概率+100");
        }
    }

    public void onClickAddHeroExp()
    {
        if (inputField_HeroExp.text != "")
        {
            int id = int.Parse(inputField_HeroExp.text.Split('_')[0]);
            int exp = int.Parse(inputField_HeroExp.text.Split('_')[1]);
            GameData.changeHeroExp(id, exp);
        }
    }

    public void onClickClose()
    {
        bg.SetActive(false);
    }
}
