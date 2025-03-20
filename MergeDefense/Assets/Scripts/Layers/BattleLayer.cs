using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLayer : MonoBehaviour
{
    public void onClickStartGame()
    {
        Destroy(MainLayer.s_instance.gameObject);

        AudioScript.s_instance.playSound_btn();
        LayerManager.ShowLayer(Consts.Layer.GameLayer);
    }

    public void onClickRank()
    {
        AudioScript.s_instance.playSound_btn();
        LayerManager.ShowLayer(Consts.Layer.RankLayer);
    }
}
