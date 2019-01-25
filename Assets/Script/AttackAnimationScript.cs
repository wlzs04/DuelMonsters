using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationScript : MonoBehaviour {

    bool startPlay = false;
    float playTime = 1;
    float residueTime = 1;
    Vector3 fromPosition;
    Vector3 toPosition;

    public delegate void FinishEvent();

    public FinishEvent AnimationFinishEvent = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(startPlay)
        {
            residueTime -= Time.deltaTime;
            if(residueTime<=0)
            {
                StopPlay();
                return;
            }
            float xLength = toPosition.x - fromPosition.x;
            float yLength = toPosition.y - fromPosition.y;
            float rate = 1-residueTime / playTime;

            transform.localPosition = new Vector3(fromPosition.x+ xLength*rate, fromPosition.y + yLength * rate);
        }
	}

    /// <summary>
    /// 停止播放
    /// </summary>
    public void StopPlay()
    {
        if(AnimationFinishEvent!=null)
        {
            AnimationFinishEvent.Invoke();
            AnimationFinishEvent = null;
        }
        startPlay = false;
        gameObject.SetActive(false);
        residueTime = 0;
    }

    /// <summary>
    /// 开始播放
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    public void StartPlay(Vector3 fromPosition,Vector3 toPosition)
    {
        if (startPlay)
        {
            Debug.LogError("当前攻击动画正在播放！");
        }
        transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
        transform.localRotation = Quaternion.Euler(0,0, Mathf.Atan2(fromPosition.x - toPosition.x, toPosition.y - fromPosition.y) * 180 / Mathf.PI);
        startPlay = true;
        transform.localPosition = fromPosition;
        gameObject.SetActive(true);
        this.fromPosition = fromPosition;
        this.toPosition = toPosition;
        residueTime = playTime;
    }
}
