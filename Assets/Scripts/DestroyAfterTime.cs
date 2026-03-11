using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{

    private float time = 0.1f;
    void Start()
    {
        Destroy(gameObject, time);
    }
}
