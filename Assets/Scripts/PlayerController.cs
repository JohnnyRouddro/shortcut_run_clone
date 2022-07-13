using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : CharacterBehavior
{
    [SerializeField] GameObject playerMesh;

    [Space]

    [SerializeField] private CinemachineVirtualCamera vCam0;
    [SerializeField] private CinemachineVirtualCamera vCam1;
    [SerializeField] private CinemachineVirtualCamera vCam2;

    private InputHandler inputHandler;

    private float targetPosY = 0;
    private float posY = 0;

    [Space]

    [SerializeField] float jumpAccel;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravity;

    private bool onAir = false;


    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();

        playerMesh.transform.SetParent(null, false);

        Global.CountDownStartedAction += CountDownStarted;
        Global.StartGameAction += PlayerStart;
        
        OnDestroyAction += OnBaseDestroy;

        TriggerEnterAction += TriggerEnter;
    }

    private void OnBaseDestroy()
    {
        Global.CountDownStartedAction -= CountDownStarted;
        Global.StartGameAction -= PlayerStart;
        Global.LevelCompleteAction -= EndLevelCamera;
    }

    private void CountDownStarted()
    {
        vCam0.Priority = 0;
    }

    private void PlayerStart()
    {
        characterAnim.SetBool("running", true);

        Global.LevelCompleteAction += EndLevelCamera;
    }


    private void EndLevelCamera()
    {
        vCam2.Priority = 2;
    }

    private void Update()
    {
        if (characterState == CharacterState.PreGame)
        {
            if (Input.GetMouseButton(0))
            {
                Global.Instance.StartCountDown();
            }
        }

        if (characterIsDead)
        {
            return;
        }

        if (characterState == CharacterState.PostFinishLine)
        {
            if (groundColCount > 0 && carryBlocks.Count == 0)
            {
                EndLevel();
            }
        }

        if (characterState >= CharacterState.PreGame && characterState <= CharacterState.PostFinishLine)
        {
            targetRotation = Vector3.up * inputHandler.TouchRelative.x * touchSensitivity;

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles + targetRotation, angularSpeed * Time.deltaTime);

            targetRotation = transform.eulerAngles;
        }

        if (characterState == CharacterState.LevelRunning || characterState == CharacterState.PostFinishLine)
        {
            transform.position += currentMovementSpeed * Time.deltaTime * transform.forward;

            posY = Mathf.Lerp(posY, targetPosY, jumpAccel * Time.deltaTime);

            Vector3 pos = transform.position;
            pos.y = posY;
            transform.position = pos;


            // =====================================================================
            if (onAir)
            {
                targetPosY -= Time.deltaTime * gravity;

                if (groundColCount >= 0)
                {
                    currentMovementSpeed = groundMovementSpeed;
                    targetPosY = Mathf.Max(targetPosY, 0);
                }
                else
                {
                    if (carryBlocks.Count > 0)
                    {
                        targetPosY = Mathf.Max(targetPosY, 0);

                        DropBlock();
                    }
                    else
                    {
                        if (targetPosY < 2)
                        {
                            Die();
                        }
                    }
                }

                if (targetPosY <= 0 && transform.position.y <= 1f)
                {
                    onAir = false;

                    pos.y = 0;
                    transform.position = pos;

                    if (carryBlocks.Count > 0)
                    {
                        DropBlock();
                    }
                }
            }
            // =====================================================================



            if (transform.position.y > 2)
            {
                currentMovementSpeed = bridgeMovementSpeed;
            }


            //Global.Instance.DebugText(groundColCount.ToString());
        }
        else if (characterState == CharacterState.LevelEnded)
        {
            transform.position = Vector3.Lerp(transform.position, currentMultiplierTransform.position, 5f * Time.deltaTime);

            Vector3 rot = transform.eulerAngles;

            rot.y = Mathf.LerpAngle(rot.y, 180, 5f * Time.deltaTime);

            transform.eulerAngles = rot;
        }

        playerMesh.transform.position = transform.position;

        playerMesh.transform.rotation = Quaternion.Slerp(playerMesh.transform.rotation, transform.rotation, angularSpeed * Time.deltaTime);
    }


    private void TriggerEnter(Collider other)
    {
        if (characterState == CharacterState.LevelRunning)
        {
            if (other.CompareTag("JumpingPod"))
            {
                targetPosY = jumpHeight;

                onAir = true;
            }
        }
    }
}
