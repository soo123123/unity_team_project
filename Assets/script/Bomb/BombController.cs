using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BombController : MonoBehaviour
{
    [Header("Fuse (sec)")]
    public float fuseTime = 10f;  // 점화 후 수명

    [Header("Throw Tuning")]
    public float throwForce = 8f; // 던질 때 기본 힘
    public float throwArc = 3f;   // 위로 들어올리는 양 (상향)

    private Rigidbody2D rb;
    private Collider2D col;

    // 내부 상태
    private bool isIgnited = false; // 점화 중인지
    private bool isPicked = false;  // 플레이어가 들고 있는지
    private float elapsed = 0f;     // 경과 시간
    private Coroutine fuseCo;

    // 삭제/소멸 시 매니저에게 알리기 위한 이벤트
    public event Action<BombController> OnDestroyed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // 점화 시작 (타이머 가동)
    public void Ignite()
    {
        if (isIgnited) return;
        isIgnited = true;

        // 물리 활성
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.isTrigger = false;

        // 타이머 시작
        fuseCo = StartCoroutine(FuseCountdown());
    }

    private IEnumerator FuseCountdown()
    {
        elapsed = 0f;
        while (elapsed < fuseTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 타이머 종료 시 분기:
        if (isPicked)
        {
            // 플레이어가 들고 있다면 폭발 대신 '심지 꺼짐' (그냥 사라짐)
            Defuse();
        }
        else
        {
            // 들고 있지 않다면 폭발 (이번 단계는 소멸만)
            Explode();
        }
    }

    // 플레이어가 들었을 때
    public void PickUp(Transform hand)
    {
        if (!isIgnited) return; // 항상 점화 상태로만 다룬다 (기획)
        isPicked = true;

        // 물리 끄고 손 위치로 고정
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.isTrigger = true;
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // 손에서 던지기
    public void Throw(Vector2 dir)
    {
        if (!isIgnited) return;

        isPicked = false;
        transform.SetParent(null);

        rb.bodyType = RigidbodyType2D.Dynamic;
        col.isTrigger = false;

        // 던지는 힘 적용 (포물선용 위쪽 성분 추가)
        var force = new Vector2(dir.x * throwForce, dir.y * throwForce + throwArc);
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void Defuse()
    {
        // '심지 꺼짐' 연출 지점 (지금은 그냥 삭제)
        CleanupAndDestroy();
    }

    private void Explode()
    {
        // 폭발 연출 지점 (지금은 그냥 삭제)
        CleanupAndDestroy();
    }

    private void CleanupAndDestroy()
    {
        if (fuseCo != null) StopCoroutine(fuseCo);
        OnDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this); // 안전망(중복 호출 무해)
    }

    // 편의: 플레이어 근처 판정용 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}
