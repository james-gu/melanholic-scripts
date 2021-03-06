﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PushablesCreator : MonoBehaviour
{
    public void SetupPushables(
        Transform pushablesParent,
        List<Script_InteractableObject> interactableObjects,
        List<Script_Pushable> pushables,
        bool isInit
    )
    {
        Script_Pushable[] pushableChildren = pushablesParent.GetComponentsInChildren<Script_Pushable>(true);
        
        for (int i = 0; i < pushableChildren.Length; i++)
        {
            Script_Pushable p = pushableChildren[i];
            if (p == null)  continue;
            
            interactableObjects.Add(p);
            pushables.Add(p);

            if (isInit)
            {
                p.Id = interactableObjects.Count - 1;
                p.pushableId = i;
            }
        }
    }
}
