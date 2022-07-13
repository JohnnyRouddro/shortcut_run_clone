using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private CinemachineVirtualCamera vCam2;
    
    [Space]

    [SerializeField] float groundMovementSpeed;
    [SerializeField] float bridgeMovementSpeed;
    private float currentMovementSpeed;
    [SerializeField] float angularSpeed;
    [SerializeField] float touchSensitivity;

    [Space]

    [SerializeField] Animator playerAnim;

    [Space]

    [SerializeField] Transform carryBlockRoot;

    private InputHandler inputHandler;

    private Vector3 targetRotation = Vector3.zero;

    private int groundColCount = 1;
    private int speedBoostColCount = 0;

    private List<BlockBehavior> carryBlocks = new List<BlockBehavior>();

    private bool playerIsDead = false;

    private Rigidbody rb;

    private Transform currentMultiplierTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();

        Global.CheckGroundUnderAction += CheckGroundUnder;
        Global.StartGameAction += StartGame;

        currentMovementSpeed = groundMovementSpeed;

        currentMultiplierTransform = null;
    }

    private void OnDestroy()
    {
        Global.CheckGroundUnderAction -= CheckGroundUnder;
        Global.StartGameAction -= StartGame;
    }

    private void StartGame()
    {
        playerAnim.SetBool("running", true);
    }

    private void Update()
    {
        if (playerIsDead)
        {
            return;
        }

        if (Global.Instance.gameState == GameState.PostFinishLine)
        {
            if (groundColCount > 0 && carryBlocks.Count == 0)
            {
                EndLevel();
            }
        }

        if (Global.Instance.gameState == GameState.LevelRunning || Global.Instance.gameState == GameState.PostFinishLine)
        {
            transform.position += transform.forward * currentMovementSpeed * Time.deltaTime;

            targetRotation = Vector3.up * inputHandler.TouchRelative.x * touchSensitivity;

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles + targetRotation, angularSpeed * Time.deltaTime);

            targetRotation = transform.eulerAngles;

            //Global.Instance.DebugText(groundColCount.ToString());
        }
        else if (Global.Instance.gameState == GameState.LevelEnded)
        {
            transform.position = Vector3.Lerp(transform.position, currentMultiplierTransform.position, 5f * Time.deltaTime);

            Vector3 rot = transform.eulerAngles;

            rot.y = Mathf.LerpAngle(rot.y, 180, 5f * Time.deltaTime);

            transform.eulerAngles = rot;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerIsDead)
        {
            return;
        }

        if (Global.Instance.gameState == GameState.LevelEnded)
        {
            return;
        }

        if (Global.Instance.gameState == GameState.LevelRunning)
        {
            if (other.CompareTag("SpeedBoost"))
            {
                currentMovementSpeed = bridgeMovementSpeed;

                speedBoostColCount++;
            }

            if (other.CompareTag("Ground"))
            {
                playerAnim.SetBool("jumping", false);
                playerAnim.SetBool("running", true);

                if (carryBlocks.Count == 0)
                {
                    playerAnim.SetBool("carrying", false);
                }

                groundColCount++;
            }

            if (other.CompareTag("PickupBlock"))
            {
                playerAnim.SetBool("carrying", true);

                BlockBehavior block = other.transform.parent.GetComponent<BlockBehavior>();

                block.transform.SetParent(carryBlockRoot);

                block.transform.localPosition = Vector3.zero + Vector3.up * carryBlocks.Count * 0.3f;
                block.transform.localEulerAngles = Vector3.zero;

                block.SetMode(BlockMode.carry);

                carryBlocks.Add(block);
            }

            if (other.CompareTag("Finish"))
            {
                Global.Instance.gameState = GameState.PostFinishLine;

                currentMovementSpeed = bridgeMovementSpeed;

                Global.FinishLineTouchedAction?.Invoke();
            }
        }
        else if (Global.Instance.gameState == GameState.PostFinishLine)
        {
            if (other.CompareTag("Multiplier"))
            {
                currentMultiplierTransform = other.transform;

                Global.Instance.currentLevelFinishMultiplier = other.GetComponent<LevelFinishX>().multiplierValue;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (playerIsDead)
        {
            return;
        }

        if (Global.Instance.gameState == GameState.LevelEnded)
        {
            return;
        }
        
        if (Global.Instance.gameState == GameState.LevelRunning)
        {
            if (other.CompareTag("SpeedBoost"))
            {
                speedBoostColCount--;

                speedBoostColCount = Mathf.Max(speedBoostColCount, 0);

                if (speedBoostColCount == 0)
                {
                    currentMovementSpeed = groundMovementSpeed;
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
                if (Global.Instance.gameState == GameState.LevelRunning)
                {
                    playerAnim.SetBool("running", false);
                    playerAnim.SetBool("carrying", false);
                    playerAnim.SetBool("jumping", true);
                }
                else if (Global.Instance.gameState == GameState.PostFinishLine)
                {
                    playerAnim.SetBool("running", false);
                    playerAnim.SetBool("carrying", false);
                    playerAnim.SetBool("jumping", true);
                }
                else if (Global.Instance.gameState == GameState.LevelEnded)
                {
                    playerAnim.SetBool("carrying", false);
                    playerAnim.SetBool("running", true);
                }

            }
            else if (carryBlocks.Count > 0 && groundColCount == 0)
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
        }

    }

    private void EndLevel()
    {
        Global.Instance.gameState = GameState.LevelEnded;

        playerAnim.SetBool("dead", false);
        playerAnim.SetBool("jumping", false);
        playerAnim.SetBool("carrying", false);
        playerAnim.SetBool("running", false);

        playerAnim.SetBool("dancing", true);

        vCam2.Priority = 2;

        Global.LevelCompleteAction?.Invoke();
    }


    public void CheckGroundUnder()
    {
        if (groundColCount == 0)
        {
            if (!currentMultiplierTransform)
            {
                Die();
                return;
            }

            if (Global.Instance.gameState == GameState.PostFinishLine)
            {
                EndLevel();
            }
            else
            {
                Die();
            }

        }
    }

    private void Die()
    {
        //playerAnim.Play("Player Die", -1, 0);
        playerAnim.SetBool("dead", true);

        playerAnim.SetBool("jumping", false);
        playerAnim.SetBool("carrying", false);
        playerAnim.SetBool("running", false);

        playerIsDead = true;

        Global.LevelFailedAction?.Invoke();

        inputHandler.enabled = false;
    }
}
