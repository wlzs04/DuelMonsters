using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGroupScript : MonoBehaviour {
    GameManager gameManager;
    public Transform allCardScrollViewTransform;
    public GameObject cardPre;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.GetSingleInstance();

        UserData userData = gameManager.GetUserData();
        foreach (var item in userData.userCardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                Instantiate(cardPre, allCardScrollViewTransform);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
