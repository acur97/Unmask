using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PcInterface : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI systemTime;
    private System.DateTime now;
    private bool systemTimeSpacer = false;

    [Header("Photoshop")]
    [SerializeField] private GameObject fotoshopIcon;
    [SerializeField] private Animator fotoshopAnim;

    [Space]
    [SerializeField] private GameObject espotifyIcon;

    [Space]
    [SerializeField] private Animator startMenu;

    [Space]
    [SerializeField] private GameObject offOptions;

    private void Awake()
    {
        OffMenu(false);
        startMenu.gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateSystemTime().Forget();
    }

    private async UniTaskVoid UpdateSystemTime()
    {
        while (Application.isPlaying)
        {
            now = System.DateTime.Now;
            systemTimeSpacer = !systemTimeSpacer;

            systemTime.text = $"{now:hh}{(systemTimeSpacer ? ":" : " ")}{now.Minute} {now:tt}\r\n{now.Day}/{now.Month:00}/{now.Year}";

            await UniTask.WaitForSeconds(1, cancellationToken: destroyCancellationToken);
        }
    }

    public void PhotoshopApp(bool on)
    {
        if (on)
        {
            fotoshopAnim.SetTrigger(Hash._Open);
        }
        else
        {
            //GameManager.instance.Closelevel();
            Debug.LogWarning("CERRAR NIVEL");

            fotoshopAnim.SetTrigger(Hash._Close);
        }
    }

    public void PhotoshopIcon(bool on)
    {
        fotoshopIcon.SetActive(on);
    }

    public void EspotifyIcon(bool on)
    {
        espotifyIcon.SetActive(on);
    }

    public void StartMenu()
    {
        StartMenu(!startMenu.gameObject.activeSelf);
    }

    public void StartMenu(bool on)
    {
        if (on)
        {
            startMenu.gameObject.SetActive(true);
        }
        else
        {
            startMenu.SetTrigger(Hash._Close);
            OffMenu(false);
        }
    }

    public void OffMenu()
    {
        OffMenu(!offOptions.activeSelf);
    }

    public void OffMenu(bool on)
    {
        offOptions.SetActive(on);
    }
}