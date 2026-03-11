using UnityEngine;

public class Sword : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;
    public Material highlightMaterial;
    public AudioClip collectibleSound;
    public float lookRange = 5f;

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
            if (hit.collider.GetComponentInParent<Sword>() == this)
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
                //UIManager.Instance.pressFText.text = "Press F to collect the Sword";
                UIManager.Instance.pressFText.gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = originalMaterials[i];
                //UIManager.Instance.pressFText.text = "";
                UIManager.Instance.pressFText.gameObject.SetActive(false);
            }
        }
    }

    public void OnPickUpSword()
    {
        if (!isLookedAt) return;
        AudioManager.Instance.PlayAudioSFX(collectibleSound, 0.8f);
        GameManager.Instance.swordsNumber--;
        Destroy(gameObject);
    }
}

