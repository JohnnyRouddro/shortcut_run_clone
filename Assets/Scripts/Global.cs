using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JiRO;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }

    [SerializeField] List<Transform> NpcList;
    [SerializeField] Transform player;

    [SerializeField] GameObject levelStartUI;
    [SerializeField] GameObject levelFailedUI;
    [SerializeField] GameObject levelEndUI;
    
    [SerializeField] TMP_Text countDownText;
    [SerializeField] TMP_Text playerPositionText;

    [SerializeField] TMP_Text rewardText;

    [SerializeField] TMP_Text debugText;

    public static Action LevelFailedAction;
    public static Action LevelCompleteAction;
    public static Action CheckGroundUnderAction;
    public static Action CountDownStartedAction;
    public static Action StartGameAction;
    public static Action FinishLineTouchedAction;
    
    public bool stopCheckingFirstPosition;

    private int currentLevelFinishMultiplier = 1;
    
    public int CurrentLevelFinishMultiplier
    {
        get
        {
            return currentLevelFinishMultiplier;
        }

        set
        {
            currentLevelFinishMultiplier = value;

            //if (currentLevelFinishMultiplier == 15)
            //{
            //    LevelCompleteAction?.Invoke();
            //}
        }
    }

    private Timer countDownTimer = new Timer();

    private void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        LevelFailedAction += LevelFailed;
        LevelCompleteAction += LevelComplete;

        Application.targetFrameRate = 120;

        countDownText.gameObject.SetActive(false);
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

    public void StartCountDown()
    {
        if (!countDownTimer.finished)
        {
            return;
        }

        countDownTimer = Timer.CreateNew(gameObject, 3);
        countDownTimer.OnFinished += StartGame;
        countDownTimer.StartTimer();

        countDownText.gameObject.SetActive(true);

        levelStartUI.SetActive(false);
        CountDownStartedAction?.Invoke();
    }

    private void Update()
    {
        if (!stopCheckingFirstPosition)
        {
            int passCount = 0;

            for (int i = 0; i < NpcList.Count; i++)
            {
                if (player.transform.position.z > NpcList[i].position.z)
                {
                    passCount++;
                }
            }

            if (passCount == 3)
            {
                playerPositionText.text = "1st";
            }
            else if (passCount == 2)
            {
                playerPositionText.text = "2nd";
            }
            else if (passCount == 1)
            {
                playerPositionText.text = "3rd";
            }
            else
            {
                playerPositionText.text = "4th";
            }
        }



        if (countDownTimer.finished)
        {
            return;
        }

        countDownText.text = countDownTimer.timeLeft.ToString("F2");
    }

    public void StartGame()
    {
        countDownText.gameObject.SetActive(false);
        playerPositionText.gameObject.SetActive(true);

        StartGameAction?.Invoke();
    }


    public void DebugText(string text)
    {
        debugText.text = text;
    }
}
