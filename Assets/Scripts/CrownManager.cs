using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownManager : MonoBehaviour
{
    [SerializeField] List<MeshRenderer> crowns;

    [SerializeField] List<int> crownDists;


    [SerializeField] Transform goal;

    private int minIdx = 0;

    private void Awake()
    {
        for (int i = 0; i < crowns.Count; i++)
        {
            crowns[i].enabled = false;
        }
        
        //Global.StartGameAction += Check;
    }

    //private void OnDestroy()
    //{
    //    Global.StartGameAction -= Check;
    //}

    private void Update()
    {
        Check();
    }

    private void Check()
    {
        if (Global.Instance.stopCheckingFirstPosition)
        {
            return;
        }

        for (int i = 0; i < crowns.Count; i++)
        {
            crowns[i].enabled = false;
            crownDists[i] = (int)crowns[i].transform.position.z;
        }

        minIdx = crownDists.IndexOf(crownDists.Max());
        
        crowns[minIdx].enabled = true;
    }
}
