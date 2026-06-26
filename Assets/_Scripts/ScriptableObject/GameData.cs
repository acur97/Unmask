using System;
using UnityEngine;

public static class Hash
{
    public const string _Horizontal = "Horizontal";
    public const string _Vertical = "Vertical";

    public const string _LevelIndex = "LevelIndex";

    public static readonly int _Open = Animator.StringToHash("Open");
    public static readonly int _Close = Animator.StringToHash("Close");

    public static readonly int _Next = Animator.StringToHash("Next");

    public static readonly int _Front = Animator.StringToHash("Front");
    public static readonly int _Back = Animator.StringToHash("Back");
    public static readonly int _Side = Animator.StringToHash("Side");

    public static readonly int _Idle = Animator.StringToHash("Idle");
    public static readonly int _IsTalking = Animator.StringToHash("IsTalking");
    public static readonly int _Happy = Animator.StringToHash("Happy");
    public static readonly int _Scared = Animator.StringToHash("Scared");
    public static readonly int _Dead = Animator.StringToHash("Dead");

    public static readonly int _Distance = Shader.PropertyToID("_Distance");
}

[Serializable]
public struct Level
{
    public string name;
    public Sprite image;
    public GameObject[] patterns;
    public float timer;
}

[CreateAssetMenu(fileName = "Game Data", menuName = "Scriptable Objects/Game Data", order = 0)]
public class GameData : ScriptableObject
{
    [Header("Limiters")]
    public float limiter_timeToCpu;
    [TextArea] public string limiter_textCpu;
    public float limiter_speedToHdd;
    [TextArea] public string limiter_textHdd;
    public float limiter_distanceToRam;
    [TextArea] public string limiter_textRam;
    public float limiter_time;

    [Header("Bugs")]
    public float bugs_closeDistance;
    public float bugs_contactDistance;

    [Header("Character")]
    public float character_speed;

    [Header("Conversations")]
    public DialogueScriptable dialogue_scared;
    public DialogueScriptable dialogue_tutorial;
    public DialogueScriptable dialogue_endTutorial;
    public DialogueScriptable dialogue_corruptedLevel;
    public DialogueScriptable dialogue_endGame;
    public DialogueScriptable dialogue_openCleared;
    public DialogueScriptable dialogue_openCorrupted;

    [Space]
    public Level[] levels;

    #region Tutorial
    private int animateSliderId;

    public void Tutorial_PlayerPosition_Start()
    {
        PlayerController.instance.SetStartPosition();
    }
    public void Tutorial_PlayerPosition_1()
    {
        PlayerController.instance.transform.position = new Vector2(43.22f, 5.37f);
    }
    public void Tutorial_PlayerPosition_2()
    {
        PlayerController.instance.transform.position = new Vector2(60.85f, 2.98f);
    }
    public void Tutorial_PlayerPosition_3()
    {
        PlayerController.instance.transform.position = new Vector2(42.2f, 1.1f);
    }
    public void Tutorial_PlayerPosition_4()
    {
        PlayerController.instance.transform.position = new Vector2(43.52f, 6.06f);
    }

    public void Tutorial_OpenTutorialLevel()
    {
        GameManager.instance.StartLevel(0);
    }

    public void Tutorial_SetHappy()
    {
        PlayerController.instance.SetHappy();
    }

    public void Tutorial_StartSliderAnimation()
    {
        animateSliderId = LeanTween.value(1, 10, 2).setLoopPingPong().setOnUpdate(Tutorial_SliderAnimation).id;
    }
    private void Tutorial_SliderAnimation(float value)
    {
        PhotoshopInterface.instance.sizeSlider.value = value;
    }
    public void Tutorial_EndSliderAnimation()
    {
        LeanTween.cancel(animateSliderId);
        PhotoshopInterface.instance.sizeSlider.value = 1;
    }
    #endregion
}