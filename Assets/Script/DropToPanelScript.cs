using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DropToPanelScript : MonoBehaviour, IDropHandler
{
    UnityAction<PointerEventData> action;

    void Start ()
    {
		
	}

    /// <summary>
    /// 添加拖放后进行处理的方法
    /// </summary>
    public void AddDropHandler(UnityAction<PointerEventData> action)
    {
        this.action = action;
    }

    public void OnDrop(PointerEventData eventData)
    {
        action(eventData);
    }
}
