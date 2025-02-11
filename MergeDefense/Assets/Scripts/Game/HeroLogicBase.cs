using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogicBase : MonoBehaviour
{
    public int id;

    [HideInInspector]
    public HeroData heroData;
    [HideInInspector]
    public SkeletonAnimation spineAni;

    List<WeaponData> list_weapon = new List<WeaponData>();

    float atkRange = 0f;
    bool isAttacking = false;

    public void Awake()
    {
        heroData = HeroEntity.getInstance().getData(id);

        // 近战
        if (heroData.isJinZhan == 1)
        {
            atkRange = 1.9f;
        }
        // 远程
        else
        {
            atkRange = 10;
        }
    }

    private void Start()
    {
        spineAni = transform.Find("body").GetComponent<SkeletonAnimation>();
        spineAni.state.Complete += OnSpineAniComplete;
    }

    private void Update()
    {
        if (!isAttacking)
        {
            EnemyLogic enemyLogic = EnemyManager.s_instance.getMinDisTarget(transform);
            if (enemyLogic && Vector3.Distance(transform.position, enemyLogic.transform.position) <= atkRange)
            {
                isAttacking = true;
                playAni("attack",false);

                // 单体攻击
                if (heroData.isAtkSingle == 1)
                {
                    enemyLogic.damage(getAtk());
                }
                // 群体攻击
                else
                {

                }
            }
        }
    }

    int getAtk()
    {
        return heroData.atk;
    }

    public void addWeapon(WeaponData weaponData)
    {
        ToastScript.show("增加武器:" + weaponData.name + " level" + weaponData.level);
        Debug.Log(transform.name + "增加武器:" + weaponData.name + " level" + weaponData.level);
        list_weapon.Add(weaponData);
    }

    public void playAni(string aniName,bool isLoop)
    {
        spineAni.state.SetAnimation(0, aniName, isLoop);
    }

    void OnSpineAniComplete(TrackEntry entry)
    {
        switch (entry.Animation.Name)
        {
            case "attack":
                {
                    isAttacking = false;
                    playAni("idle", true);
                    break;
                }
        }
    }
}
