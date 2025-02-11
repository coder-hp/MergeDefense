using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    [HideInInspector]
    public Transform centerPoint;

    int curTargetPosIndex = 1;
    float moveSpeed = 2;

    float curHP;
    float fullHP;

    Transform bloodPoint;
    Transform bloodBarTrans;
    Transform bloodProgressTrans;

    private void Awake()
    {
        centerPoint = transform.Find("centerPoint");

        EnemyManager.s_instance.addEnemy(this);
        transform.position = GameLayer.s_instance.list_enemyMoveFourPos[0];

        curHP = 150;
        fullHP = 150;

        bloodPoint = transform.Find("bloodPoint");

        // 创建血条
        {
            bloodBarTrans = Instantiate(GameUILayer.s_instance.prefab_bloodBar, GameUILayer.s_instance.bloodPointTrans).transform;
            bloodBarTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, bloodPoint.position);
            bloodProgressTrans = bloodBarTrans.GetChild(0);
        }

        transform.localScale = Vector3.zero;
        transform.DOScale(1,0.5f);
    }

    public void move()
    {
        if (curHP > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameLayer.s_instance.list_enemyMoveFourPos[curTargetPosIndex], moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, GameLayer.s_instance.list_enemyMoveFourPos[curTargetPosIndex]) <= 0.1f)
            {
                if (++curTargetPosIndex > 3)
                {
                    curTargetPosIndex = 0;
                }

                if (curTargetPosIndex == 3)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (curTargetPosIndex == 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }

            if (bloodBarTrans)
            {
                bloodBarTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, bloodPoint.position);
            }
        }
    }

    public void damage(int atk)
    {
        if (curHP > 0)
        {
            curHP -= atk;
            if (curHP <= 0)
            {
                curHP = 0;
                die();
            }

            bloodProgressTrans.localScale = new Vector3(curHP / fullHP, 1,1);
        }
    }

    void die()
    {
        EnemyManager.s_instance.removeEnemy(this);

        Destroy(bloodBarTrans.gameObject);
        Destroy(gameObject);
    }
}
