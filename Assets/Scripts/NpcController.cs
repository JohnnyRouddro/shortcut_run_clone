using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : CharacterBehavior
{
    private void Start()
    {
        TriggerEnterAction += TriggerEnter;
        TriggerExitAction += TriggerExit;
    }

    private void Update()
    {
        if (characterState == CharacterState.LevelRunning)
        {
            transform.position += transform.forward * currentMovementSpeed * Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        TriggerEnterAction -= TriggerEnter;
        TriggerExitAction -= TriggerExit;
    }

    private void TriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC Road Guide"))
        {
            transform.eulerAngles = other.transform.eulerAngles;
        }
    }

    private void TriggerExit(Collider other)
    {

    }
}
