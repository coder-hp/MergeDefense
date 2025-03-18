using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Consts;

public class EnemyLogic : MonoBehaviour
{
    [HideInInspector]
    public int curTargetPosIndex = 1;

    float moveSpeed = 2;
    [HideInInspector]
    public int curHP;
    [HideInInspector]
    public int fullHP;

    float defaultSpineSpeed = 1;
    bool isCanMove = true;

    Transform bodyTrans;
    Transform bloodPoint;
    Transform bloodBarTrans;
    Image bloodProgressImg;
    Text bloodText;

    [HideInInspector]
    public EnemyWaveData enemyWaveData;

    MeshRenderer meshRenderer;
    MaterialPropertyBlock propRedColor;
    SkeletonAnimation skeletonAnimation;

    List<Consts.BuffData> list_buffDatas = new List<Consts.BuffData>();

    Vector3 bodyScaleRight;
    Vector3 bodyScaleLeft;

    private void Awake()
    {
        EnemyManager.s_instance.addEnemy(this);
        transform.position = GameLayer.s_instance.list_enemyMoveFourPos[0];

        bodyTrans = transform.Find("body");
        bloodPoint = transform.Find("bloodPoint");

        bodyScaleRight = bodyTrans.localScale;
        bodyScaleLeft = new Vector3(-bodyScaleRight.x, bodyScaleRight.y, bodyScaleRight.z);

        transform.localScale = Vector3.zero;
        transform.DOScale(1,0.5f);

        propRedColor = new MaterialPropertyBlock();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        defaultSpineSpeed = skeletonAnimation.timeScale;

        transform.GetChild(0).localPosition -= new Vector3(0,0, GameFightData.s_instance.addedEnemyCount * 0.000001f);
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
        // buff倒计时
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if(!list_buffDatas[i].isForever && list_buffDatas[i].time > 0)
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

            // 全局敌人减速buff
            for (int i = 0; i < GameFightData.s_instance.list_globalEnemyBuff.Count; i++)
            {
                if (GameFightData.s_instance.list_globalEnemyBuff[i].buffType == Consts.BuffType.MoveSpeed)
                {
                    moveSpeedXiShu += GameFightData.s_instance.list_globalEnemyBuff[i].value;
                }
            }

            if (moveSpeedXiShu < 0)
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
                    bodyTrans.localScale = bodyScaleLeft;
                }
                else if (curTargetPosIndex == 1)
                {
                    bodyTrans.localScale = bodyScaleRight;
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
        AudioScript.s_instance.playSound("enemyDamage");

        if (curHP > 0)
        {
            // 检查是否有增加伤害buff
            {
                float damageXiShu = 1;
                for (int i = 0; i < list_buffDatas.Count; i++)
                {
                    if (list_buffDatas[i].time > 0)
                    {
                        // 易伤：受到的伤害+20%
                        if (list_buffDatas[i].buffType == Consts.BuffType.YiShang)
                        {
                            damageXiShu += 0.2f;
                        }
                    }
                }
                atk = Mathf.RoundToInt(atk * damageXiShu);
            }

            DamageNumManager.s_instance.showDamageNum(atk,bloodPoint.position, isCrit);

            GameFightData.s_instance.addAllDamage(atk);

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
        if (buffData.isCanRepeatFrom)
        {
            list_buffDatas.Add(buffData);
        }
        else
        {
            for (int i = 0; i < list_buffDatas.Count; i++)
            {
                // 如果已存在该buff,则重置时间
                if (list_buffDatas[i].buffType == buffData.buffType && list_buffDatas[i].from == buffData.from)
                {
                    if (list_buffDatas[i].time < buffData.time)
                    {
                        list_buffDatas[i].time = buffData.time;
                    }
                    return;
                }
            }
            list_buffDatas.Add(buffData);
        }

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

    public void removeBuff(Consts.BuffType buffType,string from)
    {
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if (list_buffDatas[i].buffType == buffType && list_buffDatas[i].from == from)
            {
                list_buffDatas.RemoveAt(i);
                --i;
            }
        }
    }

    void die()
    {
        EffectManager.s_instance.enemyDie(transform.position);
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
            // 最后一关没有奖励
            int maxWave = EnemyWaveEntity.getInstance().list[EnemyWaveEntity.getInstance().list.Count - 1].wave;
            if (GameFightData.s_instance.curBoCi < maxWave)
            {
                LayerManager.ShowLayer(Consts.Layer.KillEnemyRewardPanel).GetComponent<KillEnemyRewardPanel>().show(enemyWaveData);
            }
        }
    }

    private void OnDestroy()
    {
        if(tween_hitRedColor != null)
        {
            tween_hitRedColor.Kill();
        }

        if(GameFightData.s_instance.isAddEnemyEnd && EnemyManager.s_instance.list_enemy.Count == 0)
        {
            GameUILayer.s_instance.btn_spawn.localScale = Vector3.one;
        }
    }
}
