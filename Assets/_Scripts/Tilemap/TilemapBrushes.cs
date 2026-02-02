using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapBrushes : MonoBehaviour
{
    [SerializeField] private GameData data;

    [Space]
    [SerializeField] private bool debugLogs;
    [SerializeField] private bool limits = true;

    public static bool CanDraw = false;
    public static int BrushSize = 1;
    public static Vector3 WorldTilePosition = new();

    [Space]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Grid grid;
    [SerializeField] private TilemapCollider2D collider2d;
    [SerializeField] private CompositeCollider2D composite;

    [Space]
    [SerializeField] private TextMeshProUGUI timer;
    private float timeLeft;
    [SerializeField] private Graphic whiteWin;

    [Space]
    [SerializeField] private Vector2 screenOffset;
    [SerializeField] private Vector2 screenOffsetSize;

    [Space]
    private Vector2 centeredMousePos = Vector2.zero;
    private Vector3Int mouseToTile;

    private int px, py;
    private readonly List<Vector3Int> ranges = new();
    private TileBase[] tiles;

    private bool updateComposite = false;

    [Header("Limiters")]
    private float timeToCpu;
    private bool limitCpu;
    private float speedToHdd;
    private bool limitHdd;
    private float distanceToRam;
    private bool limitRam;

    private int minutes;
    private int secs;

    private enum LimiterStatus
    {
        None,
        Cpu,
        Hdd,
        Ram
    }
    private LimiterStatus currentStatus;
    private bool statusChange = false;

    private void Awake()
    {
        GameManager.OnWinLevel += ClearAllTiles;
        GameManager.OnPrepareLevel += StartLimits;

        whiteWin.gameObject.SetActive(false);
        whiteWin.CrossFadeAlpha(1, 0, true);
    }

    private void StartLimits(int level)
    {
        timeLeft = data.levels[level].timer;
    }

    private void Start()
    {
        composite.GenerateGeometry();
    }

    private string FormatTime(float seconds)
    {
        minutes = Mathf.FloorToInt(seconds / 60f);
        secs = Mathf.FloorToInt(seconds % 60f);

        return $"Time left: {minutes}:{secs:00}";
    }

    private void Update()
    {
        if (!GameManager.IsPlaying)
            return;

        timeLeft -= Time.deltaTime;
        timer.text = FormatTime(timeLeft);

        if (timeLeft < 0)
        {
            //timer.text = FormatTime(0);
            timer.text = "ERROR 404";

            GameManager.instance.LoseLevel();
            return;
        }

        if (updateComposite)
        {
            composite.GenerateGeometry();
            updateComposite = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            collider2d.enabled = true;
            updateComposite = true;

            currentStatus = LimiterStatus.None;
            statusChange = true;

            CheckStatus();

            return;
        }

        limitCpu = false;
        limitHdd = false;
        limitRam = false;

        if (Input.GetMouseButton(0))
        {
            if (timeToCpu < data.limiter_timeToCpu)
            {
                timeToCpu += Time.deltaTime;
            }
            else
            {
                limitCpu = true;
            }
        }
        else
        {
            if (timeToCpu > 0)
            {
                timeToCpu -= Time.deltaTime * 2;
            }
        }

        if (!CanDraw)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            collider2d.enabled = false;
            SetTileToBlank();

            distanceToRam = 0;
        }

        if (Input.GetMouseButton(0) && Input.mousePositionDelta != Vector3.zero)
        {
            SetTileToBlank();

            speedToHdd = Input.mousePositionDelta.magnitude;
            if (speedToHdd >= 50)
                speedToHdd = 0;

            if (speedToHdd > data.limiter_speedToHdd)
            {
                limitHdd = true;
            }

            distanceToRam += speedToHdd;
            if (distanceToRam > data.limiter_distanceToRam)
            {
                limitRam = true;
            }
        }

        if (!Input.GetMouseButton(0) && timeToCpu > 0)
        {
            timeToCpu -= Time.deltaTime;
        }

        if (limitCpu && currentStatus != LimiterStatus.Cpu)
        {
            currentStatus = LimiterStatus.Cpu;
            statusChange = true;
        }
        else if (limitHdd && currentStatus != LimiterStatus.Hdd)
        {
            currentStatus = LimiterStatus.Hdd;
            statusChange = true;
        }
        else if (limitRam && currentStatus != LimiterStatus.Ram)
        {
            currentStatus = LimiterStatus.Ram;
            statusChange = true;
        }
        else if (!limitCpu && !limitHdd && !limitRam && currentStatus != LimiterStatus.None)
        {
            currentStatus = LimiterStatus.None;
            statusChange = true;
        }

        CheckStatus();
    }

    private void CheckStatus()
    {
        if (statusChange)
        {
            statusChange = false;

            if (!limits)
                return;

            if (currentStatus != LimiterStatus.None)
            {
                CanDraw = false;

                if (debugLogs)
                    Debug.LogWarning("Cant Draw");
            }

            switch (currentStatus)
            {
                case LimiterStatus.None:
                    break;

                case LimiterStatus.Cpu:
                    if (debugLogs)
                        Debug.LogWarning("Limite de CPU!");
                    Notifications.instance.ShowPopup(data.limiter_textCpu);
                    break;

                case LimiterStatus.Hdd:
                    if (debugLogs)
                        Debug.LogWarning("Limite de HDD!");
                    Notifications.instance.ShowPopup(data.limiter_textHdd);
                    break;

                case LimiterStatus.Ram:
                    if (debugLogs)
                        Debug.LogWarning("Limite de Ram!");
                    Notifications.instance.ShowPopup(data.limiter_textRam);
                    break;
            }
        }
    }

    private void SetTileToBlank()
    {
        centeredMousePos.x = (((RaycastTransform.ScreenPosition.x * screenOffsetSize.x) + screenOffset.x) / 959) * 178;
        centeredMousePos.y = (((RaycastTransform.ScreenPosition.y * screenOffsetSize.y) + screenOffset.y) / 539) * 100;

        mouseToTile = new Vector3Int(Mathf.RoundToInt(centeredMousePos.x), Mathf.RoundToInt(centeredMousePos.y), 0);
        BlankTiles(BrushSize);

        WorldTilePosition = new Vector2(mouseToTile.x * grid.cellSize.x, mouseToTile.y * grid.cellSize.y);
        WorldTilePosition += grid.transform.position;
    }

    private void BlankTiles(int size)
    {
        ranges.Clear();

        for (int y = -size; y <= size; y++)
        {
            for (int x = -size; x <= size; x++)
            {
                if (x * x + y * y <= size * size)
                {
                    px = mouseToTile.x + x;
                    py = mouseToTile.y + y;

                    ranges.Add(new Vector3Int(px, py, 0));
                }
            }
        }

        tiles = new TileBase[ranges.Count];
        tilemap.SetTiles(ranges.ToArray(), tiles);
    }

    private void ClearAllTiles()
    {
        whiteWin.gameObject.SetActive(true);
        whiteWin.CrossFadeAlpha(0, 1, false);

        tilemap.ClearAllTiles();
    }
}