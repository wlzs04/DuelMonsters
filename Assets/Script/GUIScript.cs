using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIScript : MonoBehaviour {

    GameManager gameManager;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.GetSingleInstance();
        if(gameManager.CurrentGameState==GameState.SettingScene)
        {
            float audioValue = gameManager.Userdata.audioValue;
            GameObject.Find("AudioSlider").GetComponent<Slider>().value = audioValue;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape))
        {
            gameManager.ReturnLastScene();
        }
	}

    public void DuelButtonClick()
    {
        gameManager.EnterDuelScene();
    }

    public void CardGroupButtonClick()
    {
        gameManager.EnterCardGroupScene();
    }

    public void SettingButtonClick()
    {
        gameManager.EnterSettingScene();
    }

    public void QuitButtonClick()
    {
        gameManager.QuitGame();
    }

    public void ReturnButtonClick()
    {
        gameManager.ReturnLastScene();
    }

    public void AudioSliderChange()
    {
        gameManager.SetAudioVolume(GameObject.Find("AudioSlider").GetComponent<Slider>().value);
    }
}
