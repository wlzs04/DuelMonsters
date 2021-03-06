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

    /// <summary>
    /// 是否已拥有此卡片
    /// </summary>
    bool ownedCard = true;

    void OnDestroy()
    {
        if(dragObject)
        {
            DestroyImmediate(dragObject);
        }
    }

    public void SetCard(CardBase card,bool ownedCard = true)
    {
        this.card = card;
        GetComponent<Image>().sprite = card.GetImage();
        SetCardOwned(ownedCard);
    }

    /// <summary>
    /// 设置当前卡牌是否已获得
    /// </summary>
    /// <param name="ownedCard"></param>
    public void SetCardOwned(bool ownedCard)
    {
        this.ownedCard = ownedCard;
        if (!ownedCard)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        }
    }

    public bool IsOwnedCard()
    {
        return ownedCard;
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
        cardGroupEditScript.ShowCardDetailInfo(card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            cardGroupEditScript.RemoveCardFromCardGroup(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(ownedCard)
        {
            dragObject = Instantiate(cardGroupEditScript.cardPre, GameObject.Find("Canvas").transform);
            dragObject.GetComponent<CardScript>().SetCard(card);
            dragObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            CanvasGroup canvasGroup = dragObject.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            dragObject.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ownedCard)
        {
            dragObject.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ownedCard)
        {
            DestroyImmediate(dragObject);
        }
    }
}
