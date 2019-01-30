using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    AudioSource bgmPlayer;
    Dictionary<string, AudioClip> audioMap=new Dictionary<string, AudioClip>();

    string audioPath = "Audio/";
    string currentPlayAudioName = "";

    GameManager gameManager;
    
    void Start ()
    {
        DontDestroyOnLoad(gameObject);
        bgmPlayer = gameObject.GetComponent<AudioSource>();

        gameManager = GameManager.GetSingleInstance();
    }
	
	void Update ()
    {
        gameManager.Update();
	}

    /// <summary>
    /// 通过名称获得音乐
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public AudioClip GetAudioByName(string name)
    {
        if (!audioMap.ContainsKey(name))
        {
            AudioClip audioClip = Resources.Load<AudioClip>(audioPath + name);
            if (audioClip != null)
            {
                audioMap[name] = audioClip;
                return audioClip;
            }
            else
            {
                return null;
            }
        }
        return audioMap[name];
    }

    /// <summary>
    /// 获得当前播放音乐的名称
    /// </summary>
    /// <returns></returns>
    public string GetCurrentPlayAudioName()
    {
        return currentPlayAudioName;
    }

    /// <summary>
    /// 设置播放音乐
    /// </summary>
    /// <param name="name"></param>
    public void SetAudioByName(string name)
    {
        bool canPlay = false;
        if(!audioMap.ContainsKey(name))
        {
            AudioClip audioClip = Resources.Load<AudioClip>(audioPath + name);
            if (audioClip != null)
            {
                audioMap[name] = audioClip;
                canPlay = true;
            }
        }
        else
        {
            canPlay = true;
        }
        if(canPlay)
        {
            currentPlayAudioName = name;
            bgmPlayer.clip = audioMap[name];
            bgmPlayer.Play();
        }
        else
        {
            Debug.LogError("音频文件：" + audioPath + name + "不存在。");
        }
    }

    /// <summary>
    /// 设置音量大小
    /// </summary>
    /// <param name="value"></param>
    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        bgmPlayer.volume = value;
    }
}
