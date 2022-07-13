using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }

    [SerializeField] GameObject levelStartUI;
    [SerializeField] GameObject levelFailedUI;

    [SerializeField] TMP_Text debugText;

    public static Action LevelFailedAction;
    public static Action CheckGroundUnderAction;
    public static Action StartGameAction;


    private void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        LevelFailedAction += LevelFailed;

        Application.targetFrameRate = 120;
    }

    private void OnDestroy()
    {
        LevelFailedAction -= LevelFailed;
    }


    private void LevelFailed()
    {
        levelFailedUI.SetActive(true);
    }

    public void LevelRestart()
    {
        levelFailedUI.SetActive(false);
    }

    public void StartGame()
    {
        StartGameAction?.Invoke();
        levelStartUI.SetActive(false);
    }


    public void DebugText(string text)
    {
        debugText.text = text;
    }
}
