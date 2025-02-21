using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleDemonstrationScript : MonoBehaviour
{
    public Text text_heroName;
    public Transform heroTrans;
    public Button btn_SwitchHero;
    public Button btn_Play_Idle;
    public Button btn_Play_Attack;
    public Button btn_Play_Run;
    int herpId = 0;
    

    private void Awake()
    {
        btn_SwitchHero.onClick.AddListener(SwitchHero);
        btn_Play_Idle.onClick.AddListener(() => PlayAnimation("idle"));
        btn_Play_Attack.onClick.AddListener(() => PlayAnimation("attack"));
        btn_Play_Run.onClick.AddListener(() => PlayAnimation("run"));
    }
    void Start()
    {
        HideAllHero();
        heroTrans.GetChild(herpId).gameObject.SetActive(true);
        RefreshHeroName();
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
        RefreshHeroName();
    }
    void PlayAnimation(string aniStr)
    {
        heroTrans.GetChild(herpId).GetComponent<Animator>().Play(aniStr, 0,0);
    }
    string GetHeroName(int _heroId)
    {
        string _heroName = "";
        switch (_heroId)
        {
            case 0:
                _heroName = "剑士";
                break;
            case 1:
                _heroName = "弓箭";
                break;
            case 2:
                _heroName = "法师";
                break;
            case 3:
                _heroName = "斧手";
                break;
        }
        return _heroName;
    }
    void RefreshHeroName()
    {
        text_heroName.text = GetHeroName(herpId);
    }

}
