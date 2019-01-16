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

    Text nameText = null;
    Image editImage = null;
    Image deleteImage = null;

    CardGroupScript cardGroupScript = null;
    GuessFirstSceneScript guessFirstSceneScript = null;

    void Awake()
    {
        nameText = gameObject.transform.GetChild(0).GetComponent<Text>();

        editImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        editImage.GetComponent<Button>().onClick.AddListener(() => { EditCardGroup(); });

        deleteImage = gameObject.transform.GetChild(2).GetComponent<Image>();
        deleteImage.GetComponent<Button>().onClick.AddListener(() => { DeleteCardGroup(); });
    }

    void Update ()
    {
		
	}

    /// <summary>
    /// 设置卡组名称
    /// </summary>
    public void InitInfo(string cardCroupName,CardGroupScript cardGroupScript=null, GuessFirstSceneScript guessFirstSceneScript = null)
    {
        this.cardCroupName = cardCroupName;
        nameText.text = cardCroupName;
        this.cardGroupScript = cardGroupScript;
        this.guessFirstSceneScript = guessFirstSceneScript;
        //当是猜先场景使用时隐藏编辑和删除按钮
        if(guessFirstSceneScript!=null)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(cardGroupScript!=null)
        {
            if (!isSelected)
            {
                cardGroupScript.ClearAllSelectState();
                cardGroupScript.SelectCardGroupByName(cardCroupName);
                SetSelectState(true);
            }
        }
        if(guessFirstSceneScript!=null)
        {
            guessFirstSceneScript.SelectCardGroupByName(cardCroupName);
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
        nameText.text = cardCroupName;
    }

    public string GetCardGroupName()
    {
        return cardCroupName;
    }
    
    /// <summary>
    /// 编辑当前卡组
    /// </summary>
    public void EditCardGroup()
    {
        if(cardGroupScript!=null)
        {
            cardGroupScript.EditCardGroupByName(cardCroupName);
        }
    }

    /// <summary>
    /// 删除当前卡组
    /// </summary>
    public void DeleteCardGroup()
    {
        if (cardGroupScript != null)
        {
            cardGroupScript.DeleteCardGroupByName(cardCroupName);
        }
    }
}
