using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUILayer : MonoBehaviour
{
    public static GameUILayer s_instance = null;

    public GameObject prefab_bloodBar;
    public GameObject item_weapon;
    public Text text_enemyCount;
    public Image img_enemyCountProgress;
    public Transform bloodPointTrans;
    public Transform weaponGridTrans;

    private void Awake()
    {
        s_instance = this;
    }

    public void refreshEnemyCount()
    {
        text_enemyCount.text = EnemyManager.s_instance.getEnemyCount() + "/" + GameLayer.s_instance.maxEnemyCount;
        img_enemyCountProgress.transform.localScale = new Vector3(EnemyManager.s_instance.getEnemyCount() / 100f, 1,1);
    }

    public void onClickPause()
    {

    }

    public void onClickSort()
    {

    }

    public void onClickAutoMake()
    {

    }

    public void onClickMyth()
    {

    }

    // 召唤
    public void onClickHero()
    {
        GameLayer.s_instance.addHero();
    }

    // 锻造
    public void onClickForge()
    {
        int forgeCount = RandomUtil.getRandom(1,5);
        for(int c = 0; c < forgeCount; c++)
        {
            for (int i = 0; i < weaponGridTrans.childCount; i++)
            {
                if (weaponGridTrans.GetChild(i).childCount == 0)
                {
                    UIItemWeapon uIItemWeapon = Instantiate(item_weapon, weaponGridTrans.GetChild(i)).GetComponent<UIItemWeapon>();
                    uIItemWeapon.init(RandomUtil.getRandom(1, 6), RandomUtil.getRandom(1, 10));
                    break;
                }
            }
        }
    }

    public void onClickShop()
    {

    }
}
