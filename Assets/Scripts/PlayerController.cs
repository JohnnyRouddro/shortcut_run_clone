using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCam;
    
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

    private int groundColCount = 0;

    private List<BlockBehavior> carryBlocks = new List<BlockBehavior>();

    private bool playerIsDead = false;
    private bool gameStarted = false;


    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();

        Global.CheckGroundUnderAction += CheckGroundUnder;
        Global.StartGameAction += StartGame;

        currentMovementSpeed = groundMovementSpeed;
    }

    private void OnDestroy()
    {
        Global.CheckGroundUnderAction -= CheckGroundUnder;
        Global.StartGameAction -= StartGame;
    }

    private void StartGame()
    {
        gameStarted = true;

        playerAnim.SetBool("running", true);
    }

    private void Update()
    {
        if (playerIsDead)
        {
            return;
        }

        if (!gameStarted)
        {
            return;
        }

        transform.position += transform.forward * currentMovementSpeed * Time.deltaTime;

        targetRotation = Vector3.up * inputHandler.TouchRelative.x * touchSensitivity;

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles + targetRotation, angularSpeed * Time.deltaTime);

        targetRotation = transform.eulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerIsDead)
        {
            return;
        }

        if (!gameStarted)
        {
            return;
        }

        if (other.CompareTag("Ground"))
        {
            playerAnim.SetBool("jumping", false);
            playerAnim.SetBool("running", true);

            groundColCount++;
        }

        if (other.CompareTag("PickupBlock"))
        {
            BlockBehavior block = other.transform.parent.GetComponent<BlockBehavior>();

            block.transform.SetParent(carryBlockRoot);

            block.transform.localPosition = Vector3.zero + Vector3.up * carryBlocks.Count * 0.3f;
            block.transform.localEulerAngles = Vector3.zero;

            block.SetMode(BlockMode.carry);

            carryBlocks.Add(block);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerIsDead)
        {
            return;
        }

        if (!gameStarted)
        {
            return;
        }

        if (other.CompareTag("Ground"))
        {
            currentMovementSpeed = groundMovementSpeed;

            groundColCount--;

            groundColCount = Mathf.Max(groundColCount, 0);

            if (carryBlocks.Count == 0 && groundColCount == 0)
            {
                playerAnim.SetBool("running", false);
                playerAnim.SetBool("jumping", true);
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


    public void CheckGroundUnder()
    {
        if (groundColCount == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        playerAnim.Play("Player Die", -1, 0);
        playerAnim.SetBool("jumping", false);
        playerAnim.SetBool("running", false);

        playerIsDead = true;

        Global.LevelFailedAction?.Invoke();

        inputHandler.enabled = false;
    }
}
