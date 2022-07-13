using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelFinishX : MonoBehaviour
{
    public int multiplierValue;

    [SerializeField] GameObject worldCanvas;
    [SerializeField] GameObject cylinder;
    public Color bgColor;
    [SerializeField] Image bgImage;
    [SerializeField] TMP_Text text;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        worldCanvas.SetActive(false);
        cylinder.SetActive(false);
    }

    private void Init()
    {
        worldCanvas.SetActive(true);
        cylinder.SetActive(true);

        text.text = "X" + multiplierValue.ToString();

        bgImage.color = bgColor;

        animator.Play("pop");
    }

    public void Activate(float delay)
    {
        Invoke("Init", delay);
    }
}
