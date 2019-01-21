using Assets.Script;
using Assets.Script.Card;
using Assets.Script.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardOperationButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    CardOperation cardOperation;
    Text buttonText = null;
    float selectScale = 1.2f;//当鼠标移动到按钮上时按钮放大的倍数。

    DuelCardScript duelCardScript = null;

    void Awake()
    {
        buttonText = transform.GetChild(0).GetComponent<Text>();
        GetComponent<Button>().onClick.AddListener(OperationButtonClickedEvent);
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="duelCardScript"></param>
    /// <param name="cardOperation"></param>
    public void SetInfo(DuelCardScript duelCardScript,CardOperation cardOperation)
    {
        this.duelCardScript = duelCardScript;
        this.cardOperation = cardOperation;
        CardOperationConfig cardOperationConfig = ConfigManager.GetConfigByName("CardOperation") as CardOperationConfig;
        buttonText.text = cardOperationConfig.GetRecordById((int)cardOperation).value;
    }

    /// <summary>
    /// 操作按钮点击事件
    /// </summary>
    void OperationButtonClickedEvent()
    {
        if(duelCardScript!=null)
        {
            duelCardScript.Operation(cardOperation);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale = new Vector3(selectScale, selectScale, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale = Vector3.one;
    }
}
