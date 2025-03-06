using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverLayer : MonoBehaviour
{
    public Text text_wave;
    public Text text_damage;

    void Start()
    {
        AudioScript.s_instance.playSound("gameOver");
        text_wave.text = GameUILayer.s_instance.curBoCi.ToString();
        LayerManager.LayerShowAni(transform.Find("bg"));
    }

    bool isClosed = false;
    public void onClickClose()
    {
        if (isClosed)
        {
            return;
        }
        isClosed = true;

        AudioScript.s_instance.playSound_btn();

        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            Destroy(GameLayer.s_instance.gameObject);
            Destroy(gameObject);

            LayerManager.ShowLayer(Consts.Layer.MainLayer);
        });
    }
}
