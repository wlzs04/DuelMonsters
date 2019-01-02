using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardCroupItemScript : MonoBehaviour, IPointerDownHandler
{
    float selectScale = 1.05f;//当鼠标移动到Item上时卡牌放大的倍数。
    string cardCroupName = "未命名";

    //选中状态
    bool isSelected = false;

    InputField inputField = null;

    CardGroupScript cardGroupScript = null;

    void Awake()
    {
        inputField = gameObject.transform.GetChild(0).GetComponent<InputField>();
        inputField.onEndEdit.AddListener((string value) => { SetCardGroupName(value); });
    }

    void Update ()
    {
		
	}

    /// <summary>
    /// 设置卡组名称
    /// </summary>
    public void InitInfo(string cardCroupName,CardGroupScript cardGroupScript)
    {
        this.cardCroupName = cardCroupName;
        inputField.text = cardCroupName;
        this.cardGroupScript = cardGroupScript;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSelected)
        {
            inputField.interactable = true;
            inputField.GetComponent<CanvasGroup>().interactable = true;
            inputField.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            cardGroupScript.ClearAllSelectState();
            cardGroupScript.SelectCardGroupByName(cardCroupName);
            SetSelectState(true);
        }
    }

    /// <summary>
    /// 设置选中状态
    /// </summary>
    public void SetSelectState(bool selectedState)
    {
        isSelected = selectedState;
        if(!selectedState)
        {
            inputField.DeactivateInputField();
            //inputField.interactable = false;
            gameObject.transform.localScale = Vector3.one;
        }
        else
        {
            gameObject.transform.localScale = new Vector3(selectScale, selectScale, 1);
        }
    }

    /// <summary>
    /// 设置当前卡组名称
    /// </summary>
    /// <param name="value"></param>
    public void SetCardGroupName(string value)
    {
        if(value == cardCroupName)
        {
        }
        else if (value == "")
        {
            GameManager.ShowMessage("名称不能为空！");
        }
        else if (GameManager.GetSingleInstance().GetUserData().GetCardGroupByName(value)==null)
        {
            GameManager.GetSingleInstance().GetUserData().GetCardGroupByName(cardCroupName).cardGroupName = value;
            cardCroupName = value;
        }
        else
        {
            GameManager.ShowMessage("当前名称已存在！");
        }
        inputField.DeactivateInputField();
        //inputField.interactable = false;
        inputField.GetComponent<CanvasGroup>().interactable = false;
        inputField.GetComponent<CanvasGroup>().blocksRaycasts = false;
        inputField.text = cardCroupName;
    }

    public string GetCardGroupName()
    {
        return cardCroupName;
    }
}
