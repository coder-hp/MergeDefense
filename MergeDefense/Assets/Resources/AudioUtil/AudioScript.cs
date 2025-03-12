using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System;

public class AudioScript : MonoBehaviour
{
    public static AudioScript s_instance = null;

    // 背景音乐
    [HideInInspector]
    public AudioSource m_musicAudioSource;
    
    // 音效
    [HideInInspector]
    List<AudioSource> m_soundAudioSource = new List<AudioSource>();

    // 音效音源
    Dictionary<string, AudioClip> dic_soundAudioClip = new Dictionary<string, AudioClip>();

    public void Awake()
    {
        s_instance = this;

        m_musicAudioSource = transform.Find("Music").GetComponent<AudioSource>();

        for(int i = 0; i < transform.Find("Sound").childCount; i++)
        {
            m_soundAudioSource.Add(transform.Find("Sound").GetChild(i).GetComponent<AudioSource>());
        }
    }

    public void playMusic(string name, bool isLoop)
    {
        if (m_musicAudioSource.isPlaying)
        {
            m_musicAudioSource.Stop();
        }
        m_musicAudioSource.clip = Resources.Load("Audios/Music/" + name, typeof(AudioClip)) as AudioClip;
        m_musicAudioSource.time = 0;
        m_musicAudioSource.volume = GameData.getIsOpenMusic();
        m_musicAudioSource.Play();
        m_musicAudioSource.loop = isLoop;
    }

    AudioClip getSoundAudioClip(string name)
    {
        if(dic_soundAudioClip.ContainsKey(name))
        {
            return dic_soundAudioClip[name];
        }

        AudioClip audioClip = Resources.Load("Audios/Sound/" + name, typeof(AudioClip)) as AudioClip;
        if(audioClip == null)
        {
            Debug.Log("音频缺失:" + name);
        }
        dic_soundAudioClip[name] = audioClip;
        return audioClip;
    }

    Dictionary<string, long> dic_soundPlayTime = new Dictionary<string, long>();
    public void playSound(string audioName,float volume = 1,bool isLoop = false)
    {
        if (GameData.getIsOpenSound() == 0)
        {
            return;
        }

        if(dic_soundPlayTime.ContainsKey(audioName))
        {
            if ((CommonUtil.getTimeStamp_Millisecond() - dic_soundPlayTime[audioName]) < 100)
            {
                return;
            }
        }

        dic_soundPlayTime[audioName] = CommonUtil.getTimeStamp_Millisecond();
        for (int i = 0; i < m_soundAudioSource.Count; i++)
        {
            if (!m_soundAudioSource[i].isPlaying)
            {
                m_soundAudioSource[i].volume = volume;
                m_soundAudioSource[i].loop = isLoop;
                m_soundAudioSource[i].clip = getSoundAudioClip(audioName);
                m_soundAudioSource[i].Play();

                return;
            }
        }

        // 如果执行到这里，说明暂时没有空余的音效组件使用，则取第一个组件使用
        m_soundAudioSource[0].volume = volume;
        m_soundAudioSource[0].loop = isLoop;
        m_soundAudioSource[0].clip = getSoundAudioClip(audioName);
        m_soundAudioSource[0].Play();
        return;
    }
    
    public void pauseMusic()
    {
        if (m_musicAudioSource.isPlaying)
        {
            m_musicAudioSource.Pause();
        }
    }

    public void resumeMusic()
    {
        if (m_musicAudioSource)
        {
            m_musicAudioSource.volume = GameData.getIsOpenMusic();
            m_musicAudioSource.Play();
        }
    }

    public void stopMusic()
    {
        m_musicAudioSource.Stop();
        if (m_musicAudioSource.clip)
        {
            m_musicAudioSource.clip.UnloadAudioData();
        }
    }

    public void playSound_btn()
    {
        playSound("btn_click");
    }
}
