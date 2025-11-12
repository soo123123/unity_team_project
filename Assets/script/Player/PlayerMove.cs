using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 5f; // 이동 속도
    public float airMoveMultiplier = 0f; // 공중 이동 제어 비율 (0 = 완전 금지, 0.3 = 살짝 허용)

    private Rigidbody2D rb;
    private PlayerChargeJump jumpScript; // 클래스를 자료형으로 하는 참조변수를 선언.(지금은 null값.)
    private float inputX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpScript = GetComponent<PlayerChargeJump>(); // 이걸로 참조변수를 지정해 주는 것.
    }

    void Update()
    {
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
