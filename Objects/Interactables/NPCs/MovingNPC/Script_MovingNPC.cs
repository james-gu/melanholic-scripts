﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes game state to interact after moves are done
/// </summary>
public class Script_MovingNPC : Script_StaticNPC
{
    public static string LastMoveX  = "LastMoveX";
    public static string LastMoveZ  = "LastMoveZ";
    public static string MoveX      = "MoveX";
    public static string MoveZ      = "MoveZ";


    public int MovingNPCId;
    public Vector3 startLocation;
    public Vector3 location;
    public float speed;
    public float progress;
    public string localState;
    public bool shouldExit = true;
    public int moveSetIndex;
    [SerializeField] private bool defaultFacingDirectionDisabled;


    public Model_MoveSet[] moveSets = new Model_MoveSet[0];
    // use this to keep reference to model moveSets
    public Script_MovingNPCMoveSets movingNPCMoveSets;
    
    public AnimationCurve progressCurve;
    public Dictionary<Directions, Vector3> directionToVector;
    public Queue<Directions> currentMoves = new Queue<Directions>();
    public Queue<Directions[]> allMoves = new Queue<Directions[]>();


    private Animator animator;
    [SerializeField] private bool isApproachingTarget; // bool used to know if current moves are result of appraoching
    [SerializeField] private Directions lastFacingDirection;
    
    void OnEnable() {
        if (lastFacingDirection != Directions.None && animator != null)
            FaceLastDirection();
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {   
        if (game.state == "cut-scene_npc-moving" && localState == "move")
        {
            ActuallyMove();
        }
    }

    public override void TriggerDialogue()
    {
        Script_Player player = Script_Game.Game.GetPlayer();
        Directions dir = Script_Utils.GetDirectionToTarget(transform.position, player.transform.position);
        FaceDirection(dir);
        base.TriggerDialogue();
    }

    /// <summary>
    /// if is ending the conversation, NPC should return to original
    ///  specified facing direction if any
    /// </summary>
    public override void ContinueDialogue()
    {
        // check the current node before switching to the next node to check
        if (dialogueManager.currentNode.data.disableReturnToDefaultFaceDir)
        {
            dialogueManager.ContinueDialogue();
            return;
        }

        bool? didContinue = dialogueManager.ContinueDialogue();
        
        // meaning it was a valid continuation but there are no more nodes
        if (
            didContinue == false
            && defaultFacingDirection != Directions.None
            && !defaultFacingDirectionDisabled
        )
        {
            FaceDefaultDirection();
            Debug.Log($"MovingNPC returning to default direction: {defaultFacingDirection}");
        }
    }

    void QueueUpAllMoves()
    {
        allMoves.Clear();
        
        foreach(Model_MoveSet moveSet in moveSets)
        {
            allMoves.Enqueue(moveSet.moves);
        }
    }

    void QueueUpCurrentMoves()
    {
        Directions[] moves = allMoves.Dequeue();
        currentMoves.Clear();

        foreach (Directions move in moves)
        {
            currentMoves.Enqueue(move);
        }
    }

    public void QueueMoves()
    {
        QueueUpAllMoves();
        QueueUpCurrentMoves();
    }

    public override void Move()
    {
        Vector3 desiredDirection = directionToVector[currentMoves.Dequeue()];

        startLocation = location;
        location += desiredDirection;
        localState = "move";

        progress = 0f;

        AnimatorSetDirection(desiredDirection.x, desiredDirection.z);
        animator.SetBool("NPCMoving", true);
    }

    public void ActuallyMove()
    {
        progress += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(
            startLocation,
            location,
            progressCurve.Evaluate(progress)
        );

        if (progress >= 1f)
        {
            progress = 1f;
            transform.position = location;
            
            if (currentMoves.Count == 0) {
                localState = "interact";
                animator.SetBool("NPCMoving", false);
                
                NPCEndCommands endCommand = moveSets[moveSetIndex].NPCEndCommand;
                Directions endFaceDirection = moveSets[moveSetIndex].endFaceDirection;
                FaceDirection(endFaceDirection);
                
                game.ChangeStateInteract();
                game.CurrentMovesDoneAction();
                

                if (allMoves.Count == 0)
                {
                    game.ChangeStateInteract();
                    game.AllMovesDoneAction(MovingNPCId);

                    if (isApproachingTarget)
                    {
                        isApproachingTarget = false;
                        // allow LB to handle this event
                        game.OnApproachedTarget(MovingNPCId);
                    }

                    if (endCommand == NPCEndCommands.Exit)
                    {
                        Exit();
                    }
                    
                    return;
                }
                // TODO: this might need to happen before calling current moves done
                moveSetIndex++;
                QueueUpCurrentMoves();
                
                return;
            }
            Move();
        }
    }

    void Exit()
    {
        game.DestroyMovingNPC(MovingNPCId);
    }

    void AnimatorSetDirection(float x, float z)
    {
        animator.SetFloat(LastMoveX, x);
        animator.SetFloat(LastMoveZ, z);
        animator.SetFloat(MoveX, x);
        animator.SetFloat(MoveZ, z);
    }

    public void FaceDirection(Directions direction)
    {
        if (direction == Directions.Down)        AnimatorSetDirection(0  , -1f);
        else if (direction == Directions.Up)     AnimatorSetDirection(0  ,  1f);
        else if (direction == Directions.Left)   AnimatorSetDirection(-1f,  0f);
        else if (direction == Directions.Right)  AnimatorSetDirection(1f ,  0f );

        if (direction != Directions.None)
            lastFacingDirection = direction;
    }

    /// Face the last set direction by FaceDirection()
    public void FaceLastDirection()
    {
        FaceDirection(lastFacingDirection);
    }

    public void FaceDefaultDirection()
    {
        FaceDirection(defaultFacingDirection);
    }

    public void FacePlayer()
    {
        Directions directionToPlayer = transform.GetMyDirectionToTarget(game.GetPlayer().transform);
        Debug.Log($"{name} facing player in Direction: {directionToPlayer}");

        FaceDirection(directionToPlayer);
    }

    public void ChangeSpeed(float _speed)
    {
        speed = _speed;
    }

    public void ForceMove(Model_MoveSet _moveSet)
    {
        moveSets = new Model_MoveSet[]{ _moveSet };
        moveSetIndex = 0;
        
        QueueUpAllMoves();
        QueueUpCurrentMoves();

        if (_moveSet.moves.Length == 0)
        {
            // also face direction
            FaceDirection(_moveSet.endFaceDirection); 
            game.OnApproachedTarget(MovingNPCId);
        }
        else
        {
            isApproachingTarget = true;
            Move();
        }
    }

    public void ApproachTarget(
        Vector3 target,
        Vector3 adjustment,
        Directions endFaceDirection,
        NPCEndCommands endCommand
    )
    {
        Vector3 toMove = (target + adjustment) - location;
        Directions xMove;
        Directions zMove;
        Directions[] moves;

        int x = (int)Mathf.Round(toMove.x);
        int z = (int)Mathf.Round(toMove.z);
        int absX = Mathf.Abs(x);
        int absZ = Mathf.Abs(z);

        if (x < 0)  xMove = Directions.Left;
        else        xMove = Directions.Right;

        if (z < 0)  zMove = Directions.Down;
        else        zMove = Directions.Up;
        
        moves = new Directions[absX + absZ];

        for (int i = 0; i < moves.Length; i++)
        {
            if (i < absX)   moves[i] = xMove;
            else            moves[i] = zMove;   
        }

        ForceMove(new Model_MoveSet(
            moves,
            endFaceDirection,
            endCommand
        ));
    }

    /// <summary>
    /// used when NPC is controlled by Timeline
    /// </summary>
    public void UpdateLocation()
    {
        location = transform.position;
    }

    public virtual void SetMoveSpeedRun(){}
    public virtual void SetMoveSpeedWalk(){}

    public override void Setup()
    {
        directionToVector = Script_Utils.GetDirectionToVectorDict();
        
        base.Setup();

        animator = rendererChild.GetComponent<Animator>();
        animator.SetBool("NPCMoving", false);
        
        if (lastFacingDirection == Directions.None)
        {
            FaceDefaultDirection();
        }
        else
        {
            FaceLastDirection();
        }

        progress = 1f;
        location = transform.position;

        // first queue up moves
        if (movingNPCMoveSets != null)  moveSets = movingNPCMoveSets.moveSets;
        if (moveSets.Length != 0)
        {
            QueueUpAllMoves();
            QueueUpCurrentMoves();
        }
    }
}
