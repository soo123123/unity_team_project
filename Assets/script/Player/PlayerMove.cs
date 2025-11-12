using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 5f;         // 이동 속도
    public float airMoveMultiplier = 0f; // 공중 이동 제어 비율 (0 = 완전 금지, 0.3 = 살짝 허용)

    private Rigidbody2D rb;
    private PlayerChargeJump jumpScript;
    private float inputX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpScript = GetComponent<PlayerChargeJump>();
    }

    void Update()
    {
        // 항상 방향 입력은 받는다 (애니메이션 등 용도)
        inputX = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        // 충전 중에는 완전 금지
        if (jumpScript != null && jumpScript.IsCharging)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // --- 바닥 상태 ---
        if (jumpScript != null && jumpScript.IsGrounded)
        {
            rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
        }
        // --- 공중 상태 ---
        else
        {
            // airMoveMultiplier == 0 → 공중 이동 완전 금지
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x + inputX * moveSpeed * airMoveMultiplier * Time.fixedDeltaTime,
                rb.linearVelocity.y
            );
        }
    }
}
