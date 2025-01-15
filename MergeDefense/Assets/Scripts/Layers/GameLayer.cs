using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    public List<Transform> enemyMoveFourPoint = new List<Transform>();
    public Transform enemyPoint;

    Vector3 enemyBornPos;

    void Start()
    {
        enemyBornPos = enemyMoveFourPoint[0].position;

        LayerManager.ShowLayer(Consts.Layer.GameUILayer);

        InvokeRepeating("onInvokeAddEnemy",1,1);
    }

    void onInvokeAddEnemy()
    {
        Transform enemyTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Enemys/enemy1"), enemyPoint).transform;
        enemyTrans.position = enemyBornPos;
    }
}
