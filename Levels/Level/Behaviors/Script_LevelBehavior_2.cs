﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// This uses the old way without timeline to move the NPC
/// </summary>
public class Script_LevelBehavior_2 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isInitialized = false;
    public bool isDone = false;
    public bool isActivated = false;
    public int activeTriggerIndex = 0;
    
    public string EroFaceDirection;
    public bool[] switchesStates;
    /* ======================================================================= */
    
    public Script_DialogueManager dm;
    public Script_MovingNPC Ero;
    public Script_DialogueNode[] TriggerNodes;
    public Script_Exits exitsHandler;
    public Script_BgThemePlayer EroBgThemePlayerPrefab;
    public Transform lightSwitchesParent;
    public Transform painting1Parent;
    public Transform painting2Parent;
    public Transform painting3Parent;

    private Script_LBSwitchHandler switchHandler;
    [SerializeField] private Model_MoveSet[] truncatedMoveSet = new Model_MoveSet[0];
    [SerializeField] private Script_MovingNPCMoveSets moveSetsData;
    [SerializeField] private PlayableDirector nameplateDirector;
    [SerializeField] private Script_DialogueNode afterNameplateNode;
    [SerializeField] private Script_PRCSPlayer namePlatePRCSPlayer;
    [SerializeField] private bool isPRCSActive; /// Used to not trigger move when we want to call PRCS
    // private Queue<string> cachedCurrentMoves = new Queue<string>();
    // private Queue<string[]> cachedAllMoves = new Queue<string[]>();
    
    protected override void OnEnable()
    {
        nameplateDirector.stopped += OnNameplateDone;
    }

    protected override void OnDisable()
    {
        nameplateDirector.stopped -= OnNameplateDone;
    }
    
    public override bool ActivateTrigger(string Id)
    {
        if (
            (
                (Id == "hallway_1" && activeTriggerIndex == 0)
                || (Id == "hallway_2" && activeTriggerIndex == 1)
                || (Id == "hallway_3" && activeTriggerIndex == 2)
            )
            && !isDone
        )
        {
            OnTrigger();
            return true;
        }

        return false;
    }

    void OnTrigger()
    {
        game.PauseBgMusic();
        if (game.GetNPCBgThemeActive())     game.UnPauseNPCBgTheme();
        else                                game.PlayNPCBgTheme(EroBgThemePlayerPrefab);
        
        CacheMovingNPCMoves(0);
        game.ChangeStateCutSceneNPCMoving();
        activeTriggerIndex++;
        if (activeTriggerIndex > 2) isDone = true;
        
        game.GetMovingNPC(0).ApproachTarget(
            game.GetPlayerLocation(),
            new Vector3(1f, 0, 0),
            Directions.Left,
            NPCEndCommands.None
        );
    }
    
    protected override void HandleOnEntrance() {
        if (!exitsHandler.isFadeIn && !isActivated)
        {
            isActivated = true;
            game.ChangeStateCutSceneNPCMoving();
            game.TriggerMovingNPCMove(0);            
        }
    }
    public override void HandleMovingNPCOnApproachedTarget(int i)
    {
        dm.StartDialogueNode(TriggerNodes[activeTriggerIndex - 1]);
        RehydrateMovingNPCMoves(0);
        game.ChangeStateCutScene();
    }

    /*
        when player is done with current convo this turns state to
        npc-cut-scene-moving which triggers Script_MovingNPC Update()
    */
    protected override void HandleAction()
    {
        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
            && !isPRCSActive
        )
        {
            game.ChangeStateCutSceneNPCMoving();
            // need this bc once leave room, no longer inProgress
            game.TriggerMovingNPCMove(0);
        }

        base.HandleDialogueAction();
    }

    public override void SetSwitchState(int Id, bool isOn)
    {
        switchHandler.SetSwitchState(switchesStates, Id, isOn);
    }

    // need this to save NPC moves since ForceMove will erase its moveSets
    void CacheMovingNPCMoves(int Id)
    {
        Script_MovingNPC npc = game.GetMovingNPC(Id);
        
        // Model_NPC NPCData = game.Levels.levelsData[game.level].NPCsData[Id];
        // Model_MoveSet[] allMoveSets = NPCData.moveSets;
        // use GameObject, don't use gameData NPCData
        Model_MoveSet[] allMoveSets = moveSetsData.moveSets;
        truncatedMoveSet = new Model_MoveSet[
            Mathf.Max(allMoveSets.Length - activeTriggerIndex - 1, 0)
        ];
        
        for (int j = 0, k = activeTriggerIndex + 1; j < truncatedMoveSet.Length; j++, k++)
        {
            truncatedMoveSet[j] = allMoveSets[k];
        }
    }

    void RehydrateMovingNPCMoves(int Id)
    {
        Script_MovingNPC npc = game.GetMovingNPC(Id);
        
        npc.moveSets = truncatedMoveSet;

        npc.QueueMoves();
    }

    public void NameplateTimeline()
    {
        Debug.Log("Calling from node");
        isPRCSActive = true;
        game.ChangeStateCutScene();
        namePlatePRCSPlayer.Play();
    }

    private void OnNameplateDone(PlayableDirector aDirector)
    {
        namePlatePRCSPlayer.Stop();
        dm.StartDialogueNode(afterNameplateNode, SFXOn: false);
        isPRCSActive = false;
    }
    
    public override void Setup()
    {
        switchHandler = GetComponent<Script_LBSwitchHandler>();
        switchHandler.Setup(game);
        switchesStates = switchHandler.SetupSwitchesState(
            lightSwitchesParent,
            switchesStates,
            isInitialize: !isInitialized
        );
        game.SetupInteractableObjectsText(painting1Parent, !isInitialized);
        game.SetupInteractableObjectsText(painting2Parent, !isInitialized);
        game.SetupInteractableObjectsText(painting3Parent, !isInitialized);
        game.SetupMovingNPC(Ero, isInitialize: !isInitialized);
        
        isInitialized = true;
    }
}
