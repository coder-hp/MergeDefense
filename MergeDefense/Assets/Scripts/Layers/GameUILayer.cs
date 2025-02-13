using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUILayer : MonoBehaviour
{
    public static GameUILayer s_instance = null;

    public GameObject prefab_bloodBar;
    public GameObject item_weapon;
    public Image img_enemyCountProgress;
    public Transform bloodPointTrans;
    public Transform weaponGridTrans;
    public Text text_enemyCount;
    public Text text_time;
    public Text text_boci;

    int curBoCi = 0;
    int curBoCiRestTime = 20;

    List<int> waitAddEnemy = new List<int>();

    private void Awake()
    {
        s_instance = this;

        Invoke("startBoCi",0.5f);
    }

    void startBoCi()
    {
        ++curBoCi;

        ToastScript.show("WAVE：" + curBoCi);

        curBoCiRestTime = 20;

        text_boci.text = "WAVE：" + curBoCi + "/80";
        text_time.text = "00:" + curBoCiRestTime;

        waitAddEnemy.Clear();
        for(int i = 0; i < 15; i++)
        {
            waitAddEnemy.Add(1);
        }

        InvokeRepeating("onInvokeBoCiSecond",1,1);
        InvokeRepeating("onInvokeAddEnemy", 0.5f, 0.5f);
    }

    void onInvokeBoCiSecond()
    {
        --curBoCiRestTime;

        if(curBoCiRestTime >= 10)
        {
            text_time.text = "00:" + curBoCiRestTime;
        }
        else
        {
            text_time.text = "00:0" + curBoCiRestTime;
        }

        if (curBoCiRestTime <= 0)
        {
            CancelInvoke("onInvokeBoCiSecond");
            Invoke("startBoCi", 2);
        }
    }

    void onInvokeAddEnemy()
    {
        if(waitAddEnemy.Count > 0)
        {
            GameLayer.s_instance.addEnemy(waitAddEnemy[0]);
            waitAddEnemy.RemoveAt(0);

            if (waitAddEnemy.Count == 0)
            {
                CancelInvoke("onInvokeAddEnemy");
            }
        }
    }

    public void refreshEnemyCount()
    {
        text_enemyCount.text = EnemyManager.s_instance.getEnemyCount() + "/" + GameLayer.s_instance.maxEnemyCount;
        img_enemyCountProgress.fillAmount = EnemyManager.s_instance.getEnemyCount() / 100f;
    }

    public void onClickPause()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void onClickSort()
    {
        ToastScript.show("暂未开放");
    }

    public void onClickAutoMake()
    {
        ToastScript.show("暂未开放");
    }

    public void onClickMyth()
    {
        ToastScript.show("暂未开放");
    }

    // 召唤
    public void onClickHero()
    {
        GameLayer.s_instance.addHero();
    }

    // 锻造
    public void onClickForge()
    {
        int forgeCount = RandomUtil.getRandom(1,3);
        for(int c = 0; c < forgeCount; c++)
        {
            for (int i = 0; i < weaponGridTrans.childCount; i++)
            {
                if (weaponGridTrans.GetChild(i).childCount == 0)
                {
                    UIItemWeapon uIItemWeapon = Instantiate(item_weapon, weaponGridTrans.GetChild(i)).GetComponent<UIItemWeapon>();
                    uIItemWeapon.init(RandomUtil.getRandom(1, 5), 1);
                    break;
                }
            }
        }
    }

    public void onClickShop()
    {
        ToastScript.show("暂未开放");
    }
}
