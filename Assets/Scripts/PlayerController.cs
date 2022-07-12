using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float angularSpeed;

    [Space]

    [SerializeField] Animator playerAnim;

    [Space]

    [SerializeField] Transform carryBlockRoot;

    private Rigidbody rb;
    private InputHandler inputHandler;

    private Vector3 targetRotation = Vector3.zero;


    private List<BlockBehavior> carryBlocks = new List<BlockBehavior>();

    private void Awake()
    {
        rb = FindObjectOfType<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Update()
    {
        transform.position += transform.forward * movementSpeed * Time.deltaTime;

        targetRotation = Vector3.up * inputHandler.TouchRelative.x;

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles + targetRotation, angularSpeed * Time.deltaTime);

        targetRotation = transform.eulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            playerAnim.SetBool("jumping", false);
            playerAnim.SetBool("running", true);
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
        if (other.CompareTag("Ground"))
        {
            if (carryBlocks.Count == 0)
            {
                playerAnim.SetBool("running", false);
                playerAnim.SetBool("jumping", true);
            }
            else
            {
                BlockBehavior block = carryBlocks[carryBlocks.Count - 1];

                block.transform.SetParent(null);

                Vector3 newPos = block.transform.position;
                newPos.y = 0;
                block.transform.position = newPos;

                block.SetMode(BlockMode.ground);

                carryBlocks.RemoveAt(carryBlocks.Count - 1);
            }

        }
    }
}
