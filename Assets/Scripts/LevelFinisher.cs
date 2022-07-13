using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    [SerializeField] List<LevelFinishX> multipliers;

    [SerializeField] List<Color> multiplierBgColor;

    private void Awake()
    {
        int bgIdx = 0;

        for (int i = 0; i < multipliers.Count; i++)
        {
            multipliers[i].bgColor = multiplierBgColor[bgIdx];

            bgIdx++;

            if (bgIdx >= multiplierBgColor.Count)
            {
                bgIdx = 0;
            }
        }


        Global.FinishLineTouchedAction += ShowMultipliers;
    }

    private void OnDestroy()
    {
        Global.FinishLineTouchedAction -= ShowMultipliers;
    }

    public void ShowMultipliers()
    {
        for (int i = 0; i < multipliers.Count; i++)
        {
            multipliers[i].Activate(i * 0.2f);
        }
    }
}
