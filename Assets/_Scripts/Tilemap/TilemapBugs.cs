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
    }

    private void OnDestroy()
    {
        GameManager.OnPrepareLevel -= InstancePatterns;
        GameManager.OnWinLevel -= CloseLevel;
        GameManager.OnCloseLevel -= CloseLevel;
    }

    private void InstancePatterns(int level)
    {
        root.gameObject.SetActive(false);

        Instantiate(data.levels[level].patterns[Random.Range(0, data.levels[level].patterns.Length)], root);

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

        DelayInitBugs().Forget();
    }

    private async UniTaskVoid DelayInitBugs()
    {
        await UniTask.WaitForSeconds(0.25f);

        root.gameObject.SetActive(true);

        started = true;
    }

    private void CloseLevel()
    {
        root.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!started)
            return;

        closeEvent = false;
        contactEvent = false;

        if (!TilemapBrushes.CanDraw)
            return;

        for (int i = 0; i < bugs; i++)
        {
            distancePerBug[i] = Vector2.Distance(TilemapBrushes.WorldTilePosition, bugsPosition[i]);

            if (distancePerBug[i] < data.bugs_contactDistance)
            {
                contactEvent = true;
                //currentStatus_BugIndex = i;
            }
            else if (distancePerBug[i] < data.bugs_closeDistance)
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