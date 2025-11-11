using UnityEngine;

public class ImageFlip : MonoBehaviour
{
    // 이동 방향에 따라 스프라이트 이미지를 반전시켜주는 스크립트

    private SpriteRenderer spriteRenderer;  // 스프라이트 렌더러
    private Rigidbody2D rb;                  // 물리엔진(속도 방향 판별용)

    void Awake()
    {
        // SpriteRenderer와 Rigidbody2D 컴포넌트를 가져옴
        rb = GetComponentInParent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // --- 좌우 이동 방향에 따라 이미지 반전 ---
        if (rb.linearVelocity.x > 0.05f)
        {
            spriteRenderer.flipX = false; // 오른쪽 이동 → 원래 이미지
        }
        else if (rb.linearVelocity.x < -0.05f)
        {
            spriteRenderer.flipX = true;  // 왼쪽 이동 → 반전
        }
    }
}
