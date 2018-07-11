using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardScript : MonoBehaviour,IPointerClickHandler ,IPointerEnterHandler,IPointerExitHandler{
    float selectScale = 1.2f;//当鼠标移动到卡牌上时卡牌方法的倍数。
    public int cardNo = 0;
    public Sprite image;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
        GameObject.Find("CardImage").GetComponent<Image>().sprite = image;
    }
}
