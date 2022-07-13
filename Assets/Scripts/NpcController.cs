using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : CharacterBehavior
{
    private bool takingShortcut = false;

    private void Start()
    {
        TriggerEnterAction += TriggerEnter;
        TriggerExitAction += TriggerExit;
        GotOutOfBridgeAction += GotOutOfBridge;
    }

    private void Update()
    {
        if (characterIsDead)
        {
            return;
        }

        if (characterState == CharacterState.LevelRunning)
        {
            transform.position += transform.forward * currentMovementSpeed * Time.deltaTime;
        }
    }

    private void GotOutOfBridge()
    {
        takingShortcut = false;
    }

    private void TriggerEnter(Collider other)
    {
        if (takingShortcut)
        {
            return;
        }

        if (other.CompareTag("NPC Road Guide"))
        {
            transform.eulerAngles = other.transform.eulerAngles;
        }

        if (other.CompareTag("NPC Shortcut Guide"))
        {
            Transform target = other.GetComponent<NPCShortcutGuide>().target;

            float distance = Vector3.Distance(target.position, other.transform.position);

            if (carryBlocks.Count * 2.75f > distance)
            {
                transform.LookAt(target);

                takingShortcut = true;
            }

        }
    }

    private void TriggerExit(Collider other)
    {

    }
}
