using Cysharp.Threading.Tasks;
using UnityEngine;

public class TilemapBugs : MonoBehaviour
{
    [SerializeField] private GameData data;

    [Space]
    [SerializeField] private bool debugLogs;
    [SerializeField] private Transform root;

    [Space]
    private bool closeEvent = false;
    private bool contactEvent = false;

    private bool started = false;
    private int bugs;
    private Vector2[] bugsPosition;
    private float[] distancePerBug;
    private Transform bug;

    private enum BugDistanceStatus
    {
        Far,
        Close,
        Contact
    }
    private BugDistanceStatus currentStatus;
    //private int currentStatus_BugIndex;
    private bool statusChange = false;

    private void Awake()
    {
        GameManager.OnPrepareLevel += InstancePatterns;
        GameManager.OnWinLevel += CloseLevel;
        GameManager.OnCloseLevel += CloseLevel;

        CloseLevel();
    }

    private void OnDestroy()
    {
        GameManager.OnPrepareLevel -= InstancePatterns;
        GameManager.OnWinLevel -= CloseLevel;
        GameManager.OnCloseLevel -= CloseLevel;
    }

    private void InstancePatterns(int level, bool avalible)
    {
        if (avalible)
            return;

        InitBugs(level).Forget();
    }

    private async UniTaskVoid InitBugs(int level)
    {
        root.gameObject.SetActive(false);

        await Extensions.AsyncInstantiate(data.levels[level].patterns[Random.Range(0, data.levels[level].patterns.Length)], root);

        root.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));

        bugs = root.GetChild(0).childCount;
        bugsPosition = new Vector2[bugs];

        distancePerBug = new float[bugs];
        for (int i = 0; i < distancePerBug.Length; i++)
        {
            distancePerBug[i] = 1000f;
        }

        for (int i = 0; i < bugs; i++)
        {
            bug = root.GetChild(0).GetChild(i);
            bugsPosition[i] = bug.position;
            bug.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        }

        await UniTask.WaitForSeconds(0.25f);

        root.gameObject.SetActive(true);
        started = true;
    }

    private void CloseLevel()
    {
        root.gameObject.SetActive(false);

        if (root.childCount > 0)
            Destroy(root.GetChild(0).gameObject);

        started = false;
    }

    private void Update()
    {
        if (!GameManager.IsPlaying || !started)
            return;

        closeEvent = false;
        contactEvent = false;

        if (!TilemapBrushes.CanDraw)
        {
            currentStatus = BugDistanceStatus.Far;
            return;
        }

        for (int i = 0; i < bugs; i++)
        {
            distancePerBug[i] = Vector2.Distance(TilemapBrushes.WorldTilePosition, bugsPosition[i]);

            if (distancePerBug[i] < data.bugs_contactDistance + (TilemapBrushes.BrushSize * 0.01f))
            {
                contactEvent = true;
                //currentStatus_BugIndex = i;
            }
            else if (distancePerBug[i] < data.bugs_closeDistance + (TilemapBrushes.BrushSize * 0.01f))
            {
                closeEvent = true;
                //currentStatus_BugIndex = i;
            }
        }

        if (contactEvent && currentStatus != BugDistanceStatus.Contact)
        {
            currentStatus = BugDistanceStatus.Contact;
            statusChange = true;
        }
        else if (closeEvent && currentStatus != BugDistanceStatus.Close)
        {
            currentStatus = BugDistanceStatus.Close;
            statusChange = true;
        }
        else if (!closeEvent && !contactEvent && currentStatus != BugDistanceStatus.Far)
        {
            currentStatus = BugDistanceStatus.Far;
            statusChange = true;
        }

        if (statusChange)
        {
            statusChange = false;

            switch (currentStatus)
            {
                case BugDistanceStatus.Far:
                    if (debugLogs)
                        Debug.Log("far");
                    break;

                case BugDistanceStatus.Close:
                    if (debugLogs)
                        Debug.LogWarning("Close!");

                    PlayerController.instance.Set_Scared().Forget();
                    CharacterDialogue.instance.Conversation_Scared();
                    break;

                case BugDistanceStatus.Contact:
                    if (debugLogs)
                        Debug.LogError("CONTACT!");

                    GameManager.instance.LoseLevel();
                    TilemapBrushes.CanDraw = false;
                    break;
            }
        }
    }
}