using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const float SPEED = 5f;
    private const float BASE_SPRINT_MULTIPLIER = 2f;

    private Vector2 moveInput;
    private bool isSprinting;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private Rigidbody2D rb;
    // Added Rigidbody logic
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        // Get existing actions from your Player action map
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];

        // Bind callbacks
        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

        sprintAction.performed += ctx => {
            isSprinting = true;
            Debug.Log("Sprint started");
        };
        sprintAction.canceled += ctx => {
            isSprinting = false;
            Debug.Log("Sprint canceled");
        };
    }

    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
    }

    void FixedUpdate()
    {
        float sprintMultiplier = isSprinting ? BASE_SPRINT_MULTIPLIER : 1f;
        Vector2 move = moveInput.normalized;
        rb.linearVelocity = move * (SPEED * sprintMultiplier);

        // Swift Steps passive 
        float finalSpeed = PassiveStats.ModifyMoveSpeed(SPEED);

        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.Translate(move * (finalSpeed * sprintMultiplier) * Time.deltaTime, Space.World);
    }
}