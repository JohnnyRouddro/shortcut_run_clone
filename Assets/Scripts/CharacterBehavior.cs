using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    PreGame,
    LevelRunning,
    PostFinishLine,
    LevelEnded
}

public class CharacterBehavior : MonoBehaviour
{
    [SerializeField] protected bool isPlayer = false;
    [Space]

    [SerializeField] protected float groundMovementSpeed;
    [SerializeField] protected float bridgeMovementSpeed;
    protected float currentMovementSpeed;
    [SerializeField] protected float angularSpeed;
    [SerializeField] protected float touchSensitivity;

    [Space]

    [SerializeField] protected Animator characterAnim;

    [Space]

    [SerializeField] protected Transform carryBlockRoot;

    protected Vector3 targetRotation = Vector3.zero;

    protected int groundColCount = 1;
    protected int speedBoostColCount = 0;

    protected List<BlockBehavior> carryBlocks = new List<BlockBehavior>();

    protected bool characterIsDead = false;

    protected Transform currentMultiplierTransform;

    protected CharacterState characterState = CharacterState.PreGame;

    protected Action<Collider> TriggerEnterAction;
    protected Action<Collider> TriggerExitAction;
    protected Action OnDestroyAction;
    protected Action GotOutOfBridgeAction;

    private void Awake()
    {
        Global.CheckGroundUnderAction += CheckGroundUnder;
        Global.StartGameAction += StartGame;

        currentMovementSpeed = groundMovementSpeed;

        currentMultiplierTransform = null;
    }

    private void OnDestroy()
    {
        Global.CheckGroundUnderAction -= CheckGroundUnder;
        Global.StartGameAction -= StartGame;

        OnDestroyAction?.Invoke();
    }

    private void StartGame()
    {
        characterState = CharacterState.LevelRunning;

        characterAnim.SetBool("running", true);
    }


    protected void OnTriggerEnter(Collider other)
    {
        TriggerEnterAction?.Invoke(other);

        if (characterIsDead)
        {
            return;
        }

        if (characterState == CharacterState.LevelEnded)
        {
            return;
        }

        if (characterState == CharacterState.LevelRunning || characterState == CharacterState.PostFinishLine)
        {
            if (other.CompareTag("Ground"))
            {
                characterAnim.SetBool("jumping", false);
                characterAnim.SetBool("running", true);

                if (carryBlocks.Count == 0)
                {
                    characterAnim.SetBool("carrying", false);
                }

                groundColCount++;
            }
        }

        if (characterState == CharacterState.LevelRunning)
        {
            if (other.CompareTag("SpeedBoost"))
            {
                currentMovementSpeed = bridgeMovementSpeed;

                speedBoostColCount++;
            }

            if (other.CompareTag("PickupBlock"))
            {
                characterAnim.SetBool("carrying", true);

                BlockBehavior block = other.transform.parent.GetComponent<BlockBehavior>();

                block.transform.SetParent(carryBlockRoot);

                block.transform.localPosition = Vector3.zero + Vector3.up * carryBlocks.Count * 0.3f;
                block.transform.localEulerAngles = Vector3.zero;

                block.SetMode(BlockMode.carry);

                carryBlocks.Add(block);
            }

            if (other.CompareTag("Finish"))
            {
                characterState = CharacterState.PostFinishLine;

                if (isPlayer)
                {
                    currentMovementSpeed = bridgeMovementSpeed;
                    Global.FinishLineTouchedAction?.Invoke();
                }
                else
                {
                    transform.position = other.GetComponent<LevelFinisher>().GetDancePoint().position;
                    transform.eulerAngles = Vector3.up * 180;
                    carryBlockRoot.gameObject.SetActive(false);
                    EndLevel();
                }
            }
        }
        else if (characterState == CharacterState.PostFinishLine)
        {
            if (other.CompareTag("Multiplier"))
            {
                currentMultiplierTransform = other.transform;

                if (isPlayer)
                {
                    Global.Instance.currentLevelFinishMultiplier = other.GetComponent<LevelFinishX>().multiplierValue;
                }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExitAction?.Invoke(other);

        if (characterIsDead)
        {
            return;
        }

        if (characterState == CharacterState.LevelEnded)
        {
            return;
        }

        if (characterState == CharacterState.LevelRunning)
        {
            if (other.CompareTag("SpeedBoost"))
            {
                speedBoostColCount--;

                speedBoostColCount = Mathf.Max(speedBoostColCount, 0);

                if (speedBoostColCount == 0)
                {
                    currentMovementSpeed = groundMovementSpeed;

                    GotOutOfBridgeAction?.Invoke();
                }
            }
        }

        if (other.CompareTag("Ground"))
        {
            //if (Global.Instance.gameState == GameState.LevelRunning)
            //{
            //    currentMovementSpeed = groundMovementSpeed;
            //}

            groundColCount--;

            groundColCount = Mathf.Max(groundColCount, 0);

            if (carryBlocks.Count == 0 && groundColCount == 0)
            {
                if (characterState == CharacterState.LevelRunning)
                {
                    characterAnim.SetBool("running", false);
                    characterAnim.SetBool("carrying", false);
                    characterAnim.SetBool("jumping", true);
                }
                else if (characterState == CharacterState.PostFinishLine)
                {
                    characterAnim.SetBool("running", false);
                    characterAnim.SetBool("carrying", false);
                    characterAnim.SetBool("jumping", true);
                }
                else if (characterState == CharacterState.LevelEnded)
                {
                    characterAnim.SetBool("carrying", false);
                    characterAnim.SetBool("running", true);
                }

            }
            else if (carryBlocks.Count > 0 && groundColCount == 0)
            {
                DropBlock();
            }
        }

    }

    protected void DropBlock()
    {
        BlockBehavior block = carryBlocks[carryBlocks.Count - 1];

        block.transform.SetParent(null);

        Vector3 newPos = block.transform.position;
        newPos.y = 0;
        block.transform.position = newPos;

        block.SetMode(BlockMode.ground);

        carryBlocks.RemoveAt(carryBlocks.Count - 1);

        currentMovementSpeed = bridgeMovementSpeed;
    }

    protected void EndLevel()
    {
        characterState = CharacterState.LevelEnded;

        characterAnim.SetBool("dead", false);
        characterAnim.SetBool("jumping", false);
        characterAnim.SetBool("carrying", false);
        characterAnim.SetBool("running", false);

        characterAnim.SetBool("dancing", true);

        if (isPlayer)
        {
            Global.LevelCompleteAction?.Invoke();
        }
    }


    public void CheckGroundUnder()
    {
        if (transform.position.y > 2)
        {
            return;
        }

        if (groundColCount == 0)
        {
            if (!currentMultiplierTransform)
            {
                Die();
                return;
            }

            if (characterState == CharacterState.PostFinishLine)
            {
                EndLevel();
            }
            else
            {
                Die();
            }

        }
    }

    protected void Die()
    {
        //playerAnim.Play("Player Die", -1, 0);
        characterAnim.SetBool("dead", true);

        characterAnim.SetBool("jumping", false);
        characterAnim.SetBool("carrying", false);
        characterAnim.SetBool("running", false);

        characterIsDead = true;

        if (isPlayer)
        {
            Global.LevelFailedAction?.Invoke();
        }
    }
}
