using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float jumpForce;

    public float moveSpeed = 5f;
    public float groundDistance = 0.4f;

    public AudioClip footSteps;
    private AudioSource footstepSource;

    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private PlayerInput playerInput;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();

        //3D audio settigns
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.clip = footSteps;

        footstepSource.loop = true;
        footstepSource.playOnAwake = false;

        footstepSource.spatialBlend = 1f;
        footstepSource.volume = 0.3f;
        footstepSource.pitch = 1f;

        footstepSource.minDistance = 1f;
        footstepSource.maxDistance = 15f;

    }

    void Update()
    {
        checkPlayerIsGrounded();

        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        if (isGrounded && isMoving)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void OnJump()
    {
        if (isGrounded) {
            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    private void checkPlayerIsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;

        //Debug.Log("direction Normalized: " + direction);
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
    }

}
