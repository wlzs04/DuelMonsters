using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIScript : MonoBehaviour {

    GameManager gameManager;
    
	void Start ()
    {
        gameManager = GameManager.GetSingleInstance();
        if(gameManager.GetCurrentGameState()==GameState.SettingScene)
        {
            float audioValue = gameManager.GetUserData().audioValue;
            GameObject.Find("AudioSlider").GetComponent<Slider>().value = audioValue;
        }
    }
	
	void Update ()
    {
		if(Input.GetKeyUp(KeyCode.Escape))
        {
            gameManager.ReturnLastScene();
        }
    }
    
    public void SelectModeButtonClick()
    {
        gameManager.EnterSelectModeScene();
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

    public void SingleButton()
    {
        gameManager.SetSingleMode();
    }

    public void NetButton()
    {
        gameManager.SetNetMode();
    }
}
