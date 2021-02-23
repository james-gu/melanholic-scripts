using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Script_LevelBehavior_42 : Script_LevelBehavior
{
    // ==================================================================
    // State Data
    
    // ==================================================================
    [SerializeField] private Script_WellsPuzzleController wellsPuzzleController;

    public override void Setup()
    {
        wellsPuzzleController.InitialState();
    }
}