using Assets.Script;
using Assets.Script.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardScript : MonoBehaviour,IPointerClickHandler, IBeginDragHandler, IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler{
    float selectScale = 1.2f;//当鼠标移动到卡牌上时卡牌方法的倍数。
    CardBase card;
    CardGroupScript cardGroupScript;

    public Transform allCardTransform;
    public Transform cardGroupTransform;

    //public GameObject allCardViewport;
    //public GameObject cardGroupViewport;

    GameObject dragFromObject = null;
    GameObject dragToObject = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetCard(CardBase card)
    {
        this.card = card;
    }

    public void SetRootScript(CardGroupScript cardGroupScript)
    {
        this.cardGroupScript = cardGroupScript;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale = new Vector3(selectScale, selectScale, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            cardGroupScript.SetInfoContent(card);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(gameObject.transform.parent==cardGroupTransform)
            {
                cardGroupScript.RemoveCardFromCardGroup(gameObject,card);
            }
            if (gameObject.transform.parent == allCardTransform)
            {
                cardGroupScript.AddCardToCardGroup(card);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragFromObject = transform.parent.gameObject;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragToObject = eventData.pointerEnter.transform.GetChild(0).gameObject;
        if(dragFromObject== allCardTransform.gameObject && dragToObject == cardGroupTransform.gameObject)
        {
            cardGroupScript.AddCardToCardGroup(card);
        }
        else if(dragFromObject == cardGroupTransform.gameObject && dragToObject == allCardTransform.gameObject)
        {
            cardGroupScript.RemoveCardFromCardGroup(gameObject,card);
        }
        dragFromObject = null;
        dragToObject = null;
    }
}
