using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    // Public Settings from Image
    public float reloadTime = 1f;
    public float fireRate = 0.15f;
    public int magSize = 20;

    // Object References
    public GameObject bullet;
    public GameObject droppedWeapon;
    public Transform bulletSpawnPoint;

    public AudioClip shootingSound;

    public GameObject weaponLight;
    private float recoilDistance = 0.1f;
    private float recoilSpeed = 15f;

    // Private State Variables
    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);

    private Vector3 weaponPosition = new Vector3(0.43f,-0.3f,0.55f);
    private Quaternion weaponRotation = Quaternion.Euler(0f, 90f, 0f);


    void Start()
    {
        currentAmmo = magSize;
        initialRotation = weaponRotation;
        initialPosition = weaponPosition;

        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;

        UIManager.Instance.ammoText.text = currentAmmo.ToString();
        UIManager.Instance.maxAmmoText.text = "/20";
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
        

        Quaternion scaledBulletRotation = bulletSpawnPoint.rotation * Quaternion.Euler(0f,-3f,-1f);

        Instantiate(bullet, bulletSpawnPoint.position, scaledBulletRotation);
        Instantiate(weaponLight, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));

        AudioManager.Instance.PlayAudioSFX(shootingSound, 0.4f);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + reloadRotationOffset);
        float halfReload = reloadTime / 2f;
        float t = 0f;

        // Rotate to reload position
        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / halfReload);
            //Debug.Log("tiempo ---->" + t);
            //Debug.Log("tiempo dividido entre halfReload ---->" + t / halfReload);
            //Debug.Log("Rotation ---->" + transform.localRotation);
            yield return null;
        }

        t = 0f;

        // Rotate back to initial position
        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, t / halfReload);
            yield return null;
        }

        currentAmmo = magSize;
        isReloading = false;
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
    }

    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo == magSize) return;

        StartCoroutine(Reload());
    }

    private IEnumerator Recoil()
    {
        Vector3 targetPosition = initialPosition + new Vector3(0,0,-recoilDistance);
        float t = 0f;

        // move to recoil position
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        t = 0f;

        // move back to initial position
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, t);
            yield return null;
        }

        transform.localPosition = initialPosition;
    } 

    public void Drop()
    {
        Instantiate(droppedWeapon, transform.position, transform.rotation);
        Destroy(gameObject);
        UIManager.Instance.ammoText.text = "";
        UIManager.Instance.maxAmmoText.text = "";
    }
}