using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRoadGuide : MonoBehaviour
{
    void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

}
