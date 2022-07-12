using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Vector2 touchRelative = Vector2.zero;

    public Vector2 TouchRelative
    {
        get => touchRelative;
    }

    private Vector3 lastMousePos = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }


        if (Input.GetMouseButton(0))
        {
            touchRelative = Input.mousePosition - lastMousePos;

            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            touchRelative = Vector2.zero;
        }

    }
}
