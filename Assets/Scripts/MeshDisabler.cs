using UnityEngine;

public class DisableAllMeshColliders : MonoBehaviour
{
    void Start()
    {
        MeshCollider[] colliders = FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);

        foreach (MeshCollider col in colliders)
        {
            col.enabled = false;
        }

        Debug.Log("All MeshColliders disabled.");
    }
}