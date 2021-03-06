﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ItemObject : Script_Interactable
{
    public int myLevelBehavior;
    public Script_ItemPickUpTheatricsPlayer pickUpTheatricsPlayer;
    [SerializeField] private Script_Item item;
    public bool initialDescription;
    public bool showTyping;
    [SerializeField] private SpriteRenderer graphics;

    public Script_Item Item
    {
        get => item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns>False if inventory was full, failed to pick up</returns>
    public void HandleAction(string action)
    {
        if (action == Const_KeyCodes.Action1)
        {
            PickUp();
        }
    }

    /// <summary>
    /// player will trigger this on space OR
    /// HandleAutoPickUp()
    /// </summary>
    public void PickUp()
    {
        gameObject.SetActive(false);
        
        /// Must destroy, so we don't reactivate the item again via the PersistentDrops cycle
        Destroy(gameObject);
    }

    public void SetAlpha(float alpha)
    {
        Color newColor = graphics.color;
        newColor.a = alpha;
        graphics.color = newColor;
    }
}
