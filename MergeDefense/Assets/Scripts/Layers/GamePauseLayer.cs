using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseLayer : MonoBehaviour
{
    private void Awake()
    {
        LayerManager.LayerShowAni(transform.Find("bg"),()=>
        {
            Time.timeScale = 0;
            AudioScript.s_instance.pauseMusic();
        });
    }

    public void onClickQuit()
    {
        Time.timeScale = 1;
        AudioScript.s_instance.playSound_btn();
        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            Destroy(gameObject);
            GameUILayer.s_instance.gameOver();
        });
    }

    public void onClickContinue()
    {
        Time.timeScale = GameFightData.s_instance.gameTimeScale;
        AudioScript.s_instance.playSound_btn();
        AudioScript.s_instance.resumeMusic();

        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            Destroy(gameObject);
        });
    }
}
