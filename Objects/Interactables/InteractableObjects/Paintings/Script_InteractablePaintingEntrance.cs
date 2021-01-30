﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractablePaintingEntrance : Script_InteractableObjectText
{
    static readonly private string BoarNeedle = Const_Items.BoarNeedleId; 
    static readonly private float BoarNeedleWaitTime = 0.5f; 
    [SerializeField] private Script_ExitMetadataObject exit;
    public Script_DialogueNode[] paintingDialogueNodes;
    private int paintingDialogueIndex;
    
    public override void ActionDefault()
    {
        if (isDialogueCoolDown)     return;
        if (CheckDisabledDirections())  return;
        
        // If already talking to the Painting, then just continue dialogue.
        if (
            Script_ActiveStickerManager.Control.IsActiveSticker(BoarNeedle)
            && Script_Game.Game.GetPlayer().State == Const_States_Player.Dialogue
        )
        {
            ContinueDialogue();
        }
        else
        {
            base.ActionDefault();
        }
    }

    public void InitiatePaintingEntrance()
    {
        Script_Game.Game.GetPlayer().SetIsStandby();
        StartCoroutine(WaitToStartEntranceNode());

        IEnumerator WaitToStartEntranceNode()
        {
            yield return new WaitForSeconds(BoarNeedleWaitTime);
            
            // Script_Game.Game.GetPlayer().SetIsInteract();
            print("starting dialogue node in painting");
            /// Player state is set to dialogue by DM but just to be safe
            Script_Game.Game.GetPlayer().SetIsTalking();
            Script_DialogueManager.DialogueManager.StartDialogueNode(
                paintingDialogueNodes[paintingDialogueIndex],
                SFXOn: true,
                type: Const_DialogueTypes.Type.PaintingEntrance,
                this
            );
            HandlePaintingDialogueNodeIndex();
        }
    }

    public void HandleExit()
    {
        Script_Game.Game.Exit(
            exit.data.level,
            exit.data.playerSpawn,
            exit.data.facingDirection,
            true
        );   
    }

    void HandlePaintingDialogueNodeIndex()
    {
        if (paintingDialogueIndex == paintingDialogueNodes.Length - 1)
        {
            paintingDialogueIndex = 0;    
        }
        else
        {
            paintingDialogueIndex++;
        }
    }
}
