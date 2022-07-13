using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockMode
{
    pickup,
    ground,
    carry,
}

public class BlockBehavior : MonoBehaviour
{
    [SerializeField] GameObject pickupBlock;
    [SerializeField] GameObject groundBlock;
    public GameObject carryBlock;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMode(BlockMode blockMode)
    {
        pickupBlock.SetActive(false);
        groundBlock.SetActive(false);
        carryBlock.SetActive(false);

        switch (blockMode)
        {
            case BlockMode.pickup:
                pickupBlock.SetActive(true);
                break;
            case BlockMode.ground:
                groundBlock.SetActive(true);
                break;
            case BlockMode.carry:
                carryBlock.SetActive(true);
                break;
            default:
                break;
        }

        animator.Play("pop", -1, 0);
    }
}
