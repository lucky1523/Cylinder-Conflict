using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    private Rigidbody rb;


    public Material damagedMaterial;
    public Material originalMaterial;
    public Renderer rend;

    //Player Shooting
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject weaponFlash;
    public float bloom;
    public float fireRate;
    private float lastShotTime = 0f;

    // AI Settings
    public int currentPointIndex = 0;
    public Vector3 currentTarget;
    public float positionThreshold;
    public float idleTime = 5f;
    public float attackDistance = 5f;
    public float maxVisionDistance = 20f;
    public float minChasingHealth = 30f;

    public Transform[] patrolPoints;
    private float idleTimeCounter;
    private Transform playerTransform;
    private bool canSeePlayer;
    private Vector3 lastKnownPlayerPosition;

    public AudioClip shootingSound;
    public AudioClip enemyHit;

    private NavMeshAgent agent;

    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State state = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;

        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        //GameObject patrolPointParent = GameObject.FindWithTag("PatrolPoint");
        //patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>().Where(t => t != patrolPointParent.transform).ToArray();

        //Transform patrolParent = transform.Find("PatrolPoints");

        /*if (patrolParent != null)
        {
            patrolPoints = patrolParent
                .GetComponentsInChildren<Transform>()
                .Where(t => t != patrolParent)
                .ToArray();

            if (patrolPoints.Length > 0)
            {
                currentTarget = patrolPoints[0].position;
            }
        }
        else
        {
            Debug.LogWarning("No PatrolPoints found for " + gameObject.name);
        }*/

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[0].position;
            state = State.Patrolling;   // start directly patrolling
        }
        else
        {
            Debug.LogError(gameObject.name + " has no patrol points assigned.");
        }

        currentTarget = patrolPoints[0].position;
    }
    private void Update()
    {
        LookForPlayer();

        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrolling();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Chasing:
                Chasing();
                break;
        }

        //rb.linearVelocity = Vector3.zero;


        LookAtPlayer();
        SetLastKnownPlayerPosition();

        if (health <= 50)
        {
            agent.speed += 1;
        }
        //Debug.Log("can see player----->" + canSeePlayer);
        //Debug.Log("Agent remaining distance: " + agent.remainingDistance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            health -= 10;
            AudioManager.Instance.PlayAudioSFX(enemyHit, 0.7f);

            if (health <= 0)
            {
                Die();
            } else
            {
                StartCoroutine(Blink());
            }
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator Blink()
    {
        rend.material = damagedMaterial;
        yield return new WaitForSeconds(0.1F);
        rend.material = originalMaterial;
    }
    private void LookForPlayer()
    {
        canSeePlayer = false;
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == playerTransform;

           
        }
    }

    private void Idle()
    {
        agent.ResetPath();
        idleTimeCounter -= Time.deltaTime;

        if (idleTimeCounter < 0)
        {
            state = State.Patrolling;
            idleTimeCounter = idleTime;
        }
    }

    private void Patrolling()
    {
        if (canSeePlayer)
        {
            state = State.Chasing;
            return;
        }

        if (Vector3.Distance(currentTarget, transform.position) < positionThreshold)
        {
            currentPointIndex++;
            currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
        }
        //Debug.Log("Destination------>: " + currentTarget);
        agent.SetDestination(currentTarget);
    }

    private void Attacking()
    {
        idleTimeCounter = idleTime;
        agent.ResetPath();

        Shoot();

        if (Vector3.Distance(transform.position, playerTransform.position) > attackDistance || !canSeePlayer)
        {
            if (health < minChasingHealth)
            {
                state = State.Patrolling;
            }
            else
            {
                state = State.Chasing;
            }
        }
    }

    private void Chasing()
    {
        idleTimeCounter = idleTime;
        agent.SetDestination(lastKnownPlayerPosition);


        if (health < minChasingHealth)
        {
            state = State.Patrolling;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) <= attackDistance && canSeePlayer)
        {
            state = State.Attacking;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) > maxVisionDistance)
        {
            state = State.Patrolling;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) < positionThreshold && !canSeePlayer)
        {
            state = State.Patrolling;
        }

        if (!canSeePlayer)
        {
            agent.ResetPath();
            state = State.Patrolling;
            return;
        }
    }

    private void LookAtPlayer()
    {
        if (canSeePlayer)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }
    }

    private void SetLastKnownPlayerPosition()
    {
        if (canSeePlayer)
        {
            lastKnownPlayerPosition = playerTransform.position;
        }
    }

    private void Shoot()
    {
        if (Time.time > lastShotTime + fireRate)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.Normalize();

            Quaternion bulletRotation = Quaternion.LookRotation(directionToPlayer);

            float maxInaccuracy = 2.5f;
            float currentInaccuracy = bloom * maxInaccuracy;
            float randomJaw = Random.Range(-currentInaccuracy, currentInaccuracy);
            float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

            bulletRotation *= Quaternion.Euler(randomPitch, randomJaw + 90, 0f);

            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
            Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            AudioManager.Instance.PlayAudioSFX(shootingSound, 0.4f);
            lastShotTime = Time.time;
        }
    }
}
