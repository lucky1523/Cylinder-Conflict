using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int swordsNumber;
    public static GameManager Instance;
    public TextMeshProUGUI swordsUI;

    private void Awake()
    {
        Instance = this;
        swordsNumber = transform.childCount;
        swordsUI.text = swordsNumber.ToString();
    }
    private void Update()
    {
        swordsUI.text = swordsNumber.ToString();

        if (transform.childCount <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
