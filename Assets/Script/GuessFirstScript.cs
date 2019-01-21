using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 猜先的选择类型
/// </summary>
public enum GuessEnum
{
    Unknown,
    Jiandao,
    Shitou,
    Bu
}

public class GuessFirstScript : MonoBehaviour, IPointerClickHandler
{
    public GuessEnum selectGuessEnum;
    float selectScale = 1.2f;
    public bool isMyChoose = false;

    GuessFirstSceneScript guessFirstSceneScript = null;
    
    void Start()
    {
        guessFirstSceneScript = GameObject.Find("Main Camera").GetComponent<GuessFirstSceneScript>();
    }

    public void SetChooseState()
    {
        transform.localScale = new Vector3(selectScale, selectScale, 1);
    }

    public void ClearChooseState()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isMyChoose)
        {
            guessFirstSceneScript.SetMyGuess(selectGuessEnum);
        }
    }
}
