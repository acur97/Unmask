using System;
using UnityEngine;

public static class Hash
{
    public const string _Horizontal = "Horizontal";
    public const string _Vertical = "Vertical";

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

    [Space]
    public Level[] levels;
}