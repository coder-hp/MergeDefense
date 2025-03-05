using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossComingLayer : MonoBehaviour
{
    public Text text_wave;

    void Start()
    {
        AudioScript.s_instance.playSound("bossComing");
        text_wave.text = "WAVE " + (GameUILayer.s_instance.curBoCi + 1);

        Destroy(gameObject,2);
    }
}
