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
    private float randomZ;
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
        GameManager.OnWinLevel += WinLevel;
    }

    private void InstancePatterns(int level)
    {
        Instantiate(data.levels[level].patterns[Random.Range(0, data.levels[level].patterns.Length)], root);

        randomZ = Random.Range(0f, 360f);
        root.localEulerAngles = new Vector3(0, 0, randomZ);

        bugs = root.GetChild(0).childCount;
        bugsPosition = new Vector2[bugs];
        distancePerBug = new float[bugs];

        for (int i = 0; i < bugs; i++)
        {
            bug = root.GetChild(0).GetChild(i);
            bugsPosition[i] = bug.position;
            bug.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        }

        started = true;
    }

    private void WinLevel()
    {
        root.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!started)
            return;

        closeEvent = false;
        contactEvent = false;

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