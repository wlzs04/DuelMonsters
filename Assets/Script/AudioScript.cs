using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    AudioSource bgmPlayer;
    Dictionary<string, AudioClip> bgmMap=new Dictionary<string, AudioClip>();

    string audioPath = "Audio/";
    string bgmName = "DuelBGM";
    string duelBgmName = "光宗信吉-神々の戦い";

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);

        
    }

    public void Init()
    {
        bgmPlayer = gameObject.GetComponent<AudioSource>();
        bgmMap[bgmName] = Resources.Load<AudioClip>(audioPath + bgmName);
        bgmMap[duelBgmName] = Resources.Load<AudioClip>(audioPath + duelBgmName);
    }
	
	// Update is called once per frame
	void Update () {
        GameManager.GetSingleInstance().Update();
	}

    public AudioClip GetAudioClipByName(string name)
    {
        return bgmMap[name];
    }

    public void SetAudioByName(string name)
    {
        bgmPlayer.clip = bgmMap[name];
        bgmPlayer.Play();
    }

    public void SetVolume(float value)
    {
        bgmPlayer.volume = value;
    }
}
