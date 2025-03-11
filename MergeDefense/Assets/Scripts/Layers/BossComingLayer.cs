using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossComingLayer : MonoBehaviour
{
    public Image img_icon;
    public Text text_wave;

    void Start()
    {
        AudioScript.s_instance.playSound("bossComing");

        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite(EnemyWaveEntity.getInstance().getData(GameFightData.s_instance.curBoCi + 1).prefab);
        text_wave.text = "WAVE " + (GameFightData.s_instance.curBoCi + 1);

        Destroy(gameObject,2);
    }
}
