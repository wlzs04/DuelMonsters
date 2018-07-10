using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScript : MonoBehaviour {

    GameManager gameManager;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.GetSingleInstance();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)||Input.GetMouseButtonUp(1))
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
}
