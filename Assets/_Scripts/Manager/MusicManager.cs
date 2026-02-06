using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource desktop;
    [SerializeField] private AudioSource photoshop;

    [Space]
    [SerializeField] private float fadeTime = 2;

    private float mix = 0;
    private int tweenId = 0;

    private void Awake()
    {
        GameManager.OnStartLevel += OpenPhotoshop;
        GameManager.OnCloseLevel += ClosePhotoshop;
    }

    private void OnDestroy()
    {
        GameManager.OnStartLevel -= OpenPhotoshop;
        GameManager.OnCloseLevel -= ClosePhotoshop;
    }

    private void OpenPhotoshop()
    {
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.value(mix, 1, fadeTime).setOnUpdate(UpdateValues).id;
    }

    private void ClosePhotoshop()
    {
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.value(mix, 0, fadeTime).setOnUpdate(UpdateValues).id;
    }

    private void UpdateValues(float value)
    {
        mix = value;

        desktop.volume = 1 - mix;
        photoshop.volume = mix;
    }
}