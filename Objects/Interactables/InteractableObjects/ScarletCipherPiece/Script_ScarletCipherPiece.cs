using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Script_ScarletCipherPiece : Script_InteractableObject
{
    [SerializeField] private int _scarletCipherId;
    
    public int ScarletCipherId
    {
        get => _scarletCipherId;
        private set => _scarletCipherId = value;
    }

    public bool DidPickUp()
    {
        return Script_ScarletCipherManager.Control.ScarletCipherVisibility[ScarletCipherId];
    }

    protected override void Start()
    {
        base.Start();
        // Singletons hooked up in Awake() 
        if (DidPickUp())
        {
            Hide();
        }
    }

    public override void ActionDefault()
    {
        Script_ScarletCipherManager.Control.RevealScarletCipherSlot(ScarletCipherId);
        Hide();

        // Use Singleton for debugging purposes.
        Script_Game.Game.GetPlayer().ScarletCipherPickUpSFX();

        Script_ScarletCipherEventsManager.ScarletCipherPiecePickUp(ScarletCipherId);
    }

    public void UpdateActiveState()
    {
        this.gameObject.SetActive(!DidPickUp());
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Script_ScarletCipherPiece))]
public class Script_ScarletCipherPieceTester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_ScarletCipherPiece t = (Script_ScarletCipherPiece)target;
        if (GUILayout.Button("Pick Up"))
        {
            t.ActionDefault();
        }
    }
}
#endif