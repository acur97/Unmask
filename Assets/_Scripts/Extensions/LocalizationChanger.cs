using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationChanger : MonoBehaviour
{
    public static LocalizationChanger instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            string current = LocalizationSettings.SelectedLocale.Identifier.Code;

            string next = current == "es" ? "en" : "es";

            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.GetLocale(next);
        }
    }
}