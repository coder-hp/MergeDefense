using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleDemonstrationScript : MonoBehaviour
{
    public Transform heroTrans;
    public Button btn_SwitchHero;
    public Button btn_Play_Idle;
    public Button btn_Play_Attack;
    public Button btn_Play_Run;
    int herpId = 0;

    private void Awake()
    {
        btn_SwitchHero.onClick.AddListener(SwitchHero);
        btn_Play_Idle.onClick.AddListener(PlayIdle);
        btn_Play_Attack.onClick.AddListener(PlayAttack);
        btn_Play_Run.onClick.AddListener(PlayRun);
    }
    void Start()
    {
        HideAllHero();
        heroTrans.GetChild(herpId).gameObject.SetActive(true);
    }

    private void Update()
    {
        heroTrans.Rotate(new Vector3(0,-20f,0) * Time.deltaTime);
    }
    void HideAllHero()
    {
        for (int i = 0; i < heroTrans.childCount; i++)
        {
            heroTrans.GetChild(i).gameObject.SetActive(false);
        }
    }

    void SwitchHero()
    {
        heroTrans.GetChild(herpId).gameObject.SetActive(false);
        herpId += 1;
        if (herpId >= heroTrans.childCount)
        {
            herpId = 0;
        }
        heroTrans.GetChild(herpId).gameObject.SetActive(true);
    }
    void PlayIdle()
    {
        heroTrans.GetChild(herpId).GetComponent<Animator>().Play("idle",0,0);
    }
    void PlayAttack()
    {
        heroTrans.GetChild(herpId).GetComponent<Animator>().Play("attack", 0, 0);
    }
    void PlayRun()
    {
        heroTrans.GetChild(herpId).GetComponent<Animator>().Play("run", 0, 0);
    }
}
