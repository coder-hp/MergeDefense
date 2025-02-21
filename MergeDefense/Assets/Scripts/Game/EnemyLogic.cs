using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLogic : MonoBehaviour
{
    int curTargetPosIndex = 1;

    float moveSpeed = 2;
    float curHP;
    float fullHP;

    Transform bloodPoint;
    Transform bloodBarTrans;
    Image bloodProgressImg;

    EnemyWaveData enemyWaveData;

    private void Awake()
    {
        EnemyManager.s_instance.addEnemy(this);
        transform.position = GameLayer.s_instance.list_enemyMoveFourPos[0];

        bloodPoint = transform.Find("bloodPoint");

        // 创建血条
        {
            bloodBarTrans = Instantiate(GameUILayer.s_instance.prefab_bloodBar, GameUILayer.s_instance.bloodPointTrans).transform;
            bloodBarTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, bloodPoint.position);
            bloodProgressImg = bloodBarTrans.GetChild(0).GetComponent<Image>();
        }

        transform.localScale = Vector3.zero;
        transform.DOScale(1,0.5f);
    }

    public void init(EnemyWaveData _enemyWaveData)
    {
        enemyWaveData = _enemyWaveData;

        curHP = enemyWaveData.hp;
        fullHP = curHP;
        moveSpeed *= enemyWaveData.speed;
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

    // 返回值：本次攻击是否造成死亡
    public bool damage(int atk,bool isCrit)
    {
        if (curHP > 0)
        {
            DamageNumManager.s_instance.showDamageNum(atk,bloodPoint.position);

            curHP -= atk;
            if (curHP <= 0)
            {
                curHP = 0;
                die();
                return true;
            }

            bloodProgressImg.DOFillAmount(curHP / fullHP,0.2f);
        }

        return false;
    }

    void die()
    {
        GameUILayer.s_instance.changeGold(enemyWaveData.killGold);
        EnemyManager.s_instance.removeEnemy(this);

        Destroy(bloodBarTrans.gameObject);
        Destroy(gameObject);
    }
}
