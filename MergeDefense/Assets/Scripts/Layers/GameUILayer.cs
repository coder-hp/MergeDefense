using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUILayer : MonoBehaviour
{
    public static GameUILayer s_instance = null;

    public GameObject prefab_bloodBar;
    public Text text_enemyCount;
    public Image img_enemyCountProgress;
    public Transform bloodPointTrans;

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

    public void onClickHero()
    {
        GameLayer.s_instance.addHero();
    }

    public void onClickWeapon()
    {

    }

    public void onClickShop()
    {

    }
}
