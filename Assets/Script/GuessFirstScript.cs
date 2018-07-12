using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.GetSingleInstance().SetMyGuess(selectGuessEnum))
        {
            transform.localScale = new Vector3(selectScale, selectScale, 1);
        }
    }
}
