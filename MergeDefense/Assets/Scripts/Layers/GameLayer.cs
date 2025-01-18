using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    public static GameLayer s_instance = null;

    public List<Transform> enemyMoveFourPoint = new List<Transform>();
    public Transform enemyPoint;
    public Transform heroPoint;

    [HideInInspector]
    public List<Vector3> list_enemyMoveFourPos = new List<Vector3>();
    [HideInInspector]
    public int maxEnemyCount = 100;

    private void Awake()
    {
        s_instance = this;
    }

    void Start()
    {
        for(int i = 0; i < enemyMoveFourPoint.Count; i++)
        {
            list_enemyMoveFourPos.Add(enemyMoveFourPoint[i].position);
        }

        LayerManager.ShowLayer(Consts.Layer.GameUILayer);

        InvokeRepeating("onInvokeAddEnemy",1,1);
    }

    void onInvokeAddEnemy()
    {
        if (EnemyManager.s_instance.getEnemyCount() < maxEnemyCount)
        {
            Instantiate(ObjectPool.getPrefab("Prefabs/Enemys/enemy1"), enemyPoint);
        }
    }

    public void addHero()
    {
        for(int i = 0; i < heroPoint.childCount; i++)
        {
            if(heroPoint.GetChild(i).childCount == 0)
            {
                Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero1"), heroPoint.GetChild(i)).transform;
                return;
            }
        }
    }
}
