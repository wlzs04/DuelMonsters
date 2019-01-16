using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfScript : MonoBehaviour {

    public float life = 5;
    float startTime = 0;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
		if(Time.time-startTime> life)
        {
            Destroy(gameObject);
        }
	}
}
