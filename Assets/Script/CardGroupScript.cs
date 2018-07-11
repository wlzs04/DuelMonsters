using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGroupScript : MonoBehaviour {
    GameManager gameManager;
    public Transform allCardScrollViewTransform;
    public Transform cardGroupScrollViewTransform;
    public GameObject cardPre;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.GetSingleInstance();

        UserData userData = gameManager.Userdata;
        foreach (var item in userData.userCardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject go = Instantiate(cardPre, allCardScrollViewTransform);
                Sprite image=gameManager.allCardInfoList[item.cardNo].GetImage();
                go.GetComponent<Image>().sprite = image;
                go.GetComponent<CardScript>().cardNo = item.cardNo;
                go.GetComponent<CardScript>().image = image;
            }
        }

        foreach (var item in userData.userCardGroupList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject go = Instantiate(cardPre, cardGroupScrollViewTransform);
                Sprite image = gameManager.allCardInfoList[item.cardNo].GetImage();
                go.GetComponent<Image>().sprite = image;
                go.GetComponent<CardScript>().cardNo = item.cardNo;
                go.GetComponent<CardScript>().image = image;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
