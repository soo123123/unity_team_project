using UnityEngine;

public class PlayerChargeJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float minJumpPower = 7f;        // 최소 점프력 (기존보다 상승)
    public float maxJumpPower = 18f;       // 최대 점프력 (조금 더 강하게)
    public float chargeSpeed = 3f;         // 점프 충전 속도 (조정 가능)

    [Header("Gravity Settings")]
    public float fallMultiplier = 2.5f;    // 하강 중 중력 강화 배율

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isCharging = false;
    private float currentJumpPower = 0f;
    private bool queuedJump = false;
    private float queuedPower = 0f;
    private bool isGrounded = false;

    public bool IsCharging => isCharging;
    public bool IsGrounded => isGrounded;
    public float CurrentPower => currentJumpPower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 origin = groundCheck ? (Vector2)groundCheck.position : (Vector2)transform.position;
        isGrounded = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayer);

        // --- 충전 시작 ---
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            currentJumpPower = minJumpPower;
        }

        // --- 충전 중 ---
        if (isGrounded && Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentJumpPower += chargeSpeed * Time.deltaTime;
            currentJumpPower = Mathf.Min(currentJumpPower, maxJumpPower);
        }

        // --- 충전 종료 ---
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            queuedJump = true;
            queuedPower = currentJumpPower;
            isCharging = false;
        }
    }

    private void FixedUpdate()
    {
        // --- 실제 점프 수행 ---
        if (queuedJump && isGrounded)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = queuedPower;
            rb.linearVelocity = velocity;
            queuedJump = false;
        }

        // --- 하강 시 중력 강화 ---
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = groundCheck ? groundCheck.position : transform.position;
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
    }
}
