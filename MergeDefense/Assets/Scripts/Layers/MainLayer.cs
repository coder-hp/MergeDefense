using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLayer : MonoBehaviour
{
    public void onClickStartGame()
    {
        Destroy(gameObject);

        AudioScript.s_instance.playSound_btn();
        LayerManager.ShowLayer(Consts.Layer.GameLayer);
    }

    public void onClickClaw()
    {
        AudioScript.s_instance.playSound_btn();
        LayerManager.ShowLayer(Consts.Layer.ClawLayer);
    }
}
