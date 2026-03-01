using UnityEngine;

public class IconLevel : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public bool isAvalible;
    private int _level;

    public void Set(int level, bool avalible)
    {
        _level = level;
        isAvalible = avalible;

        if (avalible)
        {
            anim.gameObject.SetActive(false);
        }
        else
        {
            anim.gameObject.SetActive(true);
            anim.speed = Random.Range(0.5f, 2.5f);
            anim.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        }
    }

    public void StartLevel()
    {
        if (GameManager.IsPlaying)
            return;

        if (isAvalible)
        {
            GameManager.instance.StartLevel(_level);
        }
        else
        {
            CharacterDialogue.instance.Conversation_CorruptedLevel();
        }
    }

    public void ClearIcon()
    {
        isAvalible = true;
        anim.gameObject.SetActive(false);
    }
}