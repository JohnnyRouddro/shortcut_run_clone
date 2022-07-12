using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEventFire : MonoBehaviour
{
    public void CheckGroundUnder()
    {
        Global.CheckGroundUnderAction?.Invoke();
    }
}
