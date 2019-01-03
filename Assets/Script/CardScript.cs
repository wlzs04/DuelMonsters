using Assets.Script;
using Assets.Script.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardScript : MonoBehaviour,IPointerClickHandler, IBeginDragHandler, IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    float selectScale = 1.2f;//当鼠标移动到卡牌上时卡牌放大的倍数。
    CardBase card;
    CardGroupEditScript cardGroupEditScript;

    public Transform allCardTransform;
    public Transform cardGroupTransform;

    GameObject dragFromObject = null;
    GameObject dragToObject = null;

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void SetCard(CardBase card)
    {
        this.card = card;
    }

    public void SetRootScript(CardGroupEditScript cardGroupEditScript)
    {
        this.cardGroupEditScript = cardGroupEditScript;
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
            cardGroupEditScript.ShowCardDetailInfo(card);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(gameObject.transform.parent==cardGroupTransform)
            {
                cardGroupEditScript.RemoveCardFromCardGroup(gameObject,card);
            }
            if (gameObject.transform.parent == allCardTransform)
            {
                cardGroupEditScript.AddCardToCardGroup(card);
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
        if (eventData.pointerEnter.transform.childCount==0)
        {
            dragToObject= eventData.pointerEnter.transform.parent.gameObject;
        }
        else
        {
            dragToObject = eventData.pointerEnter.transform.GetChild(0).gameObject;
        }
        
        if(dragFromObject== allCardTransform.gameObject && dragToObject == cardGroupTransform.gameObject)
        {
            cardGroupEditScript.AddCardToCardGroup(card);
        }
        else if(dragFromObject == cardGroupTransform.gameObject && dragToObject == allCardTransform.gameObject)
        {
            cardGroupEditScript.RemoveCardFromCardGroup(gameObject,card);
        }
        dragFromObject = null;
        dragToObject = null;
    }
}
