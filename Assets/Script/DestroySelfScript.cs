using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 只为MessageTip提供，定时自动回调
/// </summary>
public class DestroySelfScript : MonoBehaviour {

    public float life = 5;
    float startTime = 0;
    
	void Start ()
    {
        startTime = Time.time;
    }
	
	void Update ()
    {
		if(Time.time-startTime> life)
        {
            GameManager.RemoveMessage(gameObject);
        }
	}
}
