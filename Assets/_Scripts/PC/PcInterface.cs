using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Audio")]
    [SerializeField] private Slider audio_slider;
    [SerializeField] private GameObject audio_panel;
    [SerializeField] private Image audio_icon;
    [SerializeField] private Sprite audio_Sound;
    [SerializeField] private Sprite audio_Mute;

    private void Awake()
    {
        GameManager.OnPrepareLevel += OpenPhotoshop;

        OffMenu(false);
        startMenu.gameObject.SetActive(false);
        AudioOptiones(false);
        audio_slider.value = 1;
    }

    private void OnDestroy()
    {
        GameManager.OnPrepareLevel -= OpenPhotoshop;
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

    private void OpenPhotoshop(int _)
    {
        PhotoshopApp(true);
    }

    public void PhotoshopApp(bool on)
    {
        fotoshopIcon.SetActive(on);

        if (on)
        {
            fotoshopAnim.SetTrigger(Hash._Open);
        }
        else
        {
            GameManager.instance.CloseLevel();

            fotoshopAnim.SetTrigger(Hash._Close);
        }
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

    public void AudioOptiones()
    {
        AudioOptiones(!audio_panel.activeSelf);
    }

    public void AudioOptiones(bool on)
    {
        audio_panel.SetActive(on);
    }

    public void SetAudioVolume(float value)
    {
        AudioListener.volume = value;

        if (value == 0)
        {
            audio_icon.sprite = audio_Mute;
        }
        else
        {
            audio_icon.sprite = audio_Sound;
        }
    }
}