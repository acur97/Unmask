using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public const string _StartAnimation = "StartAnimation";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void GameStartLogic()
    {
        if (PlayerPrefs.GetInt(_StartAnimation, 0) == 0)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    [ContextMenu("Delete PlayerPrefs")]
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteKey(_StartAnimation);
    }
}