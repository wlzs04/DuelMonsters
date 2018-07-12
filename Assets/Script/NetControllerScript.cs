using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetControllerScript : NetworkBehaviour
{
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isLocalPlayer)
        {

        }
	}

    public override void OnStartLocalPlayer()
    {
        GameManager.ShowMessage("OnStartLocalPlayer");
    }
}
