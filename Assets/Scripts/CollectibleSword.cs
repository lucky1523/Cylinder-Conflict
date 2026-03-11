using UnityEngine;

public class CollectibleSword : MonoBehaviour
{
    public AudioClip collectibleSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") {
            AudioManager.Instance.PlayAudioSFX(collectibleSound, 0.8f);
            GameManager.Instance.swordsNumber--;
            Destroy(gameObject);
        }
    }
}
