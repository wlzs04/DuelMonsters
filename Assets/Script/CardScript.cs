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
    
    GameObject dragObject = null;

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    void OnDestroy()
    {
        if(dragObject)
        {
            DestroyImmediate(dragObject);
        }
    }

    public void SetCard(CardBase card)
    {
        this.card = card;
        GetComponent<Image>().sprite = card.GetImage();
    }

    public CardBase GetCard()
    {
        return card;
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
        else
        {
            cardGroupEditScript.RemoveCardFromCardGroup(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragObject = Instantiate(cardGroupEditScript.cardPre, GameObject.Find("Canvas").transform);
        dragObject.GetComponent<CardScript>().SetCard(card);
        dragObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        CanvasGroup canvasGroup = dragObject.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        dragObject.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragObject.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyImmediate(dragObject);
    }
}
