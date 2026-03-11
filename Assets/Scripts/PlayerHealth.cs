using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public AudioClip gameOverSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            DecreaseHealth(10);
        }
    }

    private void DecreaseHealth(int decreaseAmount)
    {
        health -= decreaseAmount;
        PlayerLook.Instance.AddShake(0.1f,0.25f);
        PlayerLook.Instance.showHitUi();
        Debug.Log("health----->"+health);
        UIManager.Instance.SetHealthValue(health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Time.timeScale = 0f;
        UIManager.Instance.EnableDeathUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioManager.Instance.PlayAudioSFX(gameOverSound,0.8f);

    }
}