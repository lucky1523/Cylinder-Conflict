using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject hitUI;
    public GameObject deathUI;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI maxAmmoText;
    public TextMeshProUGUI pressFText;

    public Image healthBar;
    public Gradient gradientBar;

    public AudioClip gameOverSound;

    private void Awake()
    {
        Instance = this;
    }

    public void InstantiateHitUI()
    {
        Instantiate(hitUI, transform);
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EnableDeathUI()
    {
        deathUI.SetActive(true);
        AudioManager.Instance.PlayAudioSFX(gameOverSound, 0.8f);
    }

    public void SetHealthValue(int health)
    {
        float floatHealth = (float)health / 100;
        healthBar.color = gradientBar.Evaluate(floatHealth); 
        healthBar.fillAmount = floatHealth;
    }
}