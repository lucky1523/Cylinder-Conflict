using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;
    public Material highlightMaterial;
    public GameObject weaponPrefab;
    public float lookRange = 3f;

    private PlayerShooting player;
    private Camera playerCam;
    private bool isLookedAt;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }

        player = FindAnyObjectByType<PlayerShooting>();
        playerCam = player.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookRange))
        {
            if (hit.collider.GetComponentInParent<PickUp>() == this)
            {
                if (!isLookedAt)
                {
                    SetLookedAt(true);
                }
                return;
            }
        }

        if (isLookedAt)
        {
            SetLookedAt(false);
        }
    }

    void SetLookedAt(bool lookedAt)
    {
        isLookedAt = lookedAt;

        if (lookedAt)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.material = highlightMaterial;
            }
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = originalMaterials[i];
            }
        }
    }

    public void OnPickUp()
    {
        if (!isLookedAt) return;

        if (player.gun != null)
        {
            player.OnDrop();
        }

        GameObject newWeapon = Instantiate(weaponPrefab, player.gunHolder);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        player.gun = newWeapon.GetComponent<Gun>();

        Destroy(gameObject);
    }
}
