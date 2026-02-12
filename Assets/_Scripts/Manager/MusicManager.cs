using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource desktop;
    [SerializeField, Range(0, 1)] private float desktopVolume = 1;
    [SerializeField] private AudioSource photoshop;
    [SerializeField, Range(0, 1)] private float photoshopVolume = 1;

    [Space]
    [SerializeField] private float fadeTime = 2;
    [SerializeField] private float bpm = 150;
    [SerializeField] private LeanTweenType ease;

    float secondsPerBeat;
    int lastBeat;
    int currentBeat;

    private bool needsChange = false;

    private float mix = 0;
    private int tweenId = 0;

    private void Awake()
    {
        GameManager.OnPrepareLevel += OpenPhotoshop;
        GameManager.OnCloseLevel += ClosePhotoshop;

        secondsPerBeat = 60f / bpm;
        lastBeat = -1;

        UpdateValues(0);
    }

    private void OnDestroy()
    {
        GameManager.OnPrepareLevel -= OpenPhotoshop;
        GameManager.OnCloseLevel -= ClosePhotoshop;
    }

    private void OpenPhotoshop(int _, bool __)
    {
        needsChange = !needsChange;
    }

    private void ClosePhotoshop()
    {
        needsChange = !needsChange;
    }

    private void MixAudios(int to)
    {
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.value(mix, to, fadeTime).setEase(ease).setOnUpdate(UpdateValues).id;
    }

    private void UpdateValues(float value)
    {
        mix = value;

        desktop.volume = (1 - mix) * desktopVolume;
        photoshop.volume = mix * photoshopVolume;
    }

    private void Update()
    {
        currentBeat = Mathf.FloorToInt(desktop.time / secondsPerBeat);

        if (currentBeat != lastBeat)
        {
            lastBeat = currentBeat;
            //Debug.Log("Beat");

            if (needsChange)
            {
                needsChange = false;
                MixAudios(mix > 0 ? 0 : 1);
            }
        }
    }
}