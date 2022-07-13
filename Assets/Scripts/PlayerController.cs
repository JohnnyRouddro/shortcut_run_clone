using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : CharacterBehavior
{
    [SerializeField] GameObject playerMesh;

    [Space]

    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private CinemachineVirtualCamera vCam2;

    private InputHandler inputHandler;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();

        playerMesh.transform.SetParent(null, false);
    }

    private void OnDestroy()
    {
        Global.LevelCompleteAction -= EndLevelCamera;
    }

    private void StartGame()
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


        if (characterState == CharacterState.LevelRunning || characterState == CharacterState.PostFinishLine)
        {
            transform.position += transform.forward * currentMovementSpeed * Time.deltaTime;

            targetRotation = Vector3.up * inputHandler.TouchRelative.x * touchSensitivity;

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles + targetRotation, angularSpeed * Time.deltaTime);

            targetRotation = transform.eulerAngles;

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
}
