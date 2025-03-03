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
    int curHP;
    int fullHP;

    float defaultSpineSpeed = 1;
    bool isCanMove = true;

    Transform bloodPoint;
    Transform bloodBarTrans;
    Image bloodProgressImg;
    Text bloodText;

    EnemyWaveData enemyWaveData;

    MeshRenderer meshRenderer;
    MaterialPropertyBlock propRedColor;
    SkeletonAnimation skeletonAnimation;

    List<Consts.BuffData> list_buffDatas = new List<Consts.BuffData>();

    private void Awake()
    {
        EnemyManager.s_instance.addEnemy(this);
        transform.position = GameLayer.s_instance.list_enemyMoveFourPos[0];

        bloodPoint = transform.Find("bloodPoint");

        transform.localScale = Vector3.zero;
        transform.DOScale(1,0.5f);

        propRedColor = new MaterialPropertyBlock();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        defaultSpineSpeed = skeletonAnimation.timeScale;

        transform.GetChild(0).localPosition -= new Vector3(0,0,GameLayer.s_instance.addedEnemyCount * 0.000001f);
    }

    public void init(EnemyWaveData _enemyWaveData)
    {
        enemyWaveData = _enemyWaveData;

        curHP = enemyWaveData.hp;
        fullHP = curHP;
        moveSpeed *= enemyWaveData.speed;

        // 创建血条
        {
            if (enemyWaveData.enemyType == 1)
            {
                bloodBarTrans = Instantiate(GameUILayer.s_instance.prefab_bloodBar, GameUILayer.s_instance.bloodPointTrans).transform;
            }
            else
            {
                bloodBarTrans = Instantiate(GameUILayer.s_instance.prefab_bloodBar_big, GameUILayer.s_instance.bloodPointTrans).transform;
                bloodText = bloodBarTrans.Find("Text").GetComponent<Text>();
            }
            bloodBarTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, bloodPoint.position);
            bloodProgressImg = bloodBarTrans.GetChild(0).GetComponent<Image>();
        }

        if (enemyWaveData.enemyType > 1)
        {
            bloodText.text = CommonUtil.intToStrK(curHP).ToString();
        }
    }

    // 此方法每帧都会调用，所以不用再写一个Update方法
    public void move()
    {
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if(list_buffDatas[i].time > 0)
            {
                list_buffDatas[i].time -= Time.deltaTime;
                if(list_buffDatas[i].time <= 0)
                {
                    if(list_buffDatas[i].buffType == Consts.BuffType.Stun)
                    {
                        isCanMove = true;
                        skeletonAnimation.timeScale = defaultSpineSpeed;
                    }
                    list_buffDatas.RemoveAt(i);
                    --i;
                }
            }
        }

        if(!isCanMove)
        {
            return;
        }

        if (curHP > 0)
        {
            float moveSpeedXiShu = 1;
            for (int i = 0; i < list_buffDatas.Count; i++)
            {
                if (list_buffDatas[i].buffType == Consts.BuffType.MoveSpeed)
                {
                    moveSpeedXiShu += list_buffDatas[i].value;
                }
            }
            if(moveSpeedXiShu < 0)
            {
                moveSpeedXiShu = 0;
            }

            transform.position = Vector3.MoveTowards(transform.position, GameLayer.s_instance.list_enemyMoveFourPos[curTargetPosIndex], moveSpeed * moveSpeedXiShu * Time.deltaTime);
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
    Tween tween_hitRedColor = null;
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

            if (enemyWaveData.enemyType > 1)
            {
                bloodText.text = CommonUtil.intToStrK(curHP).ToString();
            }

            bloodProgressImg.DOFillAmount((float)curHP / (float)fullHP,0.2f);

            setHitColorProgress(1);

            // 从红色渐变为0
            if(tween_hitRedColor == null)
            {
                tween_hitRedColor = DOTween.To(() => 1f, setHitColorProgress, 0f, 0.3f).SetEase(Ease.Linear).SetAutoKill(false);
            }
            else
            {
                tween_hitRedColor.Restart();
            }
        }

        return false;
    }

    private void setHitColorProgress(float f)
    {
        propRedColor.SetFloat("_Hit", f);
        meshRenderer.SetPropertyBlock(propRedColor);
    }

    public void addBuff(Consts.BuffData buffData)
    {
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            // 如果已存在该buff,则重置时间
            if (list_buffDatas[i].buffType == buffData.buffType && list_buffDatas[i].from == buffData.from)
            {
                list_buffDatas[i].time = buffData.time;
                return;
            }
        }

        list_buffDatas.Add(buffData);

        switch(buffData.buffType)
        {
            case Consts.BuffType.Stun:
                {
                    isCanMove = false;
                    skeletonAnimation.timeScale = 0;
                    break;
                }
        }
    }

    void die()
    {
        EffectManager.enemyDie(transform.position);
        GameUILayer.s_instance.changeGold(enemyWaveData.killGold);
        EnemyManager.s_instance.removeEnemy(this);

        Destroy(bloodBarTrans.gameObject);
        Destroy(gameObject);

        if(tween_hitRedColor != null)
        {
            tween_hitRedColor.Kill();
        }

        if(enemyWaveData.enemyType >= 2)
        {
            LayerManager.ShowLayer(Consts.Layer.KillEnemyRewardPanel).GetComponent<KillEnemyRewardPanel>().show(enemyWaveData);
        }
    }
}
