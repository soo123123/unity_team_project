using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBombController : MonoBehaviour
{
    [Header("Refs")]
    public Transform hand; // 플레이어 손 위치(자식 오브젝트)

    [Header("Pickup")]
    public float pickupRadius = 0.7f;  // 폭탄 줍기 허용 거리
    public LayerMask bombLayer;        // Bomb 레이어

    [Header("Throw Direction")]
    public float deadZone = 0.2f;      // 방향 입력 데드존
    private Vector2 lastAim = Vector2.right; // 마지막 바라보는 방향(기본 오른쪽)

    private BombController heldBomb;   // 들고 있는 폭탄 (있으면 재투척 가능)

    void Update()
    {
        // 1) 방향 입력 업데이트 (4방향 우선)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 aim = Vector2.zero;
        if (Mathf.Abs(h) > Mathf.Abs(v))
        {
            if (Mathf.Abs(h) > deadZone) aim = new Vector2(Mathf.Sign(h), 0f);
        }
        else
        {
            if (Mathf.Abs(v) > deadZone) aim = new Vector2(0f, Mathf.Sign(v));
        }
        if (aim != Vector2.zero) lastAim = aim;

        // 2) 줍기 (E) : 근처 점화 폭탄을 손에 든다
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldBomb == null)
            {
                var bomb = FindNearestIgnitedBomb();
                if (bomb != null)
                {
                    heldBomb = bomb;
                    heldBomb.PickUp(hand);
                }
            }
        }

        // 3) 던지기 / 생성 (F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 손에 들고 있으면 → 던지기
            if (heldBomb != null)
            {
                heldBomb.Throw(GetThrowDir());
                heldBomb = null;
            }
            else
            {
                // 손에 들고 있지 않고, 필드에 폭탄이 없다면 → 생성 + 자동 점화
                if (!BombManager.Instance.HasActiveBomb)
                {
                    var bomb = BombManager.Instance.SpawnBomb(hand.position, Quaternion.identity);
                    if (bomb != null)
                    {
                        // 생성 즉시 손에 들고 있다고 가정하지 않고, 바로 던지는 연출 원하면:
                        // bomb.PickUp(hand);
                        // bomb.Throw(GetThrowDir());

                        // 이번 가이드는 "생성만" 하고 플레이어가 다시 F를 눌러 던지거나,
                        // E로 먼저 집는 흐름을 택하도록 둔다.
                    }
                }
            }
        }
    }

    private Vector2 GetThrowDir()
    {
        // 방향이 0이면 마지막 방향 사용 (기본 오른쪽)
        return lastAim == Vector2.zero ? Vector2.right : lastAim.normalized;
    }

    private BombController FindNearestIgnitedBomb()
    {
        // 주변 원형 탐색
        var hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius, bombLayer);
        BombController nearest = null;
        float minDist = float.MaxValue;

        foreach (var h in hits)
        {
            var bomb = h.GetComponent<BombController>();
            if (bomb == null) continue;

            // 점화 상태만 줍는다 (기획)
            // bomb가 항상 점화 상태로만 운용된다면 이 체크는 생략 가능
            var dist = Vector2.Distance(transform.position, bomb.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = bomb;
            }
        }
        return nearest;
    }

    // 줍기 반경 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
