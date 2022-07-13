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
    [SerializeField] GameObject levelEndUI;
    
    [SerializeField] TMP_Text rewardText;

    [SerializeField] TMP_Text debugText;

    public static Action LevelFailedAction;
    public static Action LevelCompleteAction;
    public static Action CheckGroundUnderAction;
    public static Action StartGameAction;
    public static Action FinishLineTouchedAction;

    public int currentLevelFinishMultiplier = 1;

    private void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        LevelFailedAction += LevelFailed;
        LevelCompleteAction += LevelComplete;

        Application.targetFrameRate = 120;
    }

    private void OnDestroy()
    {
        LevelFailedAction -= LevelFailed;
        LevelCompleteAction -= LevelComplete;
    }


    private void LevelFailed()
    {
        levelFailedUI.SetActive(true);
    }

    private void LevelComplete()
    {
        levelEndUI.SetActive(true);
        rewardText.text = "Get X" + currentLevelFinishMultiplier.ToString();
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
