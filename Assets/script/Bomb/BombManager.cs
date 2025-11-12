using UnityEngine;

public class BombManager : MonoBehaviour
{
    // 씬에 하나만 존재 (간단 싱글턴)
    public static BombManager Instance { get; private set; }

    [Header("Prefabs")]
    public BombController bombPrefab;   // Bomb 프리팹

    // 현재 필드에 존재하는 점화된 폭탄 (1개 제한)
    public BombController ActiveBomb { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // 필요시 DontDestroyOnLoad(gameObject);
    }

    public bool HasActiveBomb => ActiveBomb != null;

    // 폭탄 생성 (점화 상태로 스폰)
    public BombController SpawnBomb(Vector2 pos, Quaternion rot)
    {
        if (HasActiveBomb) return null;

        var bomb = Instantiate(bombPrefab, pos, rot);
        ActiveBomb = bomb;
        bomb.OnDestroyed += HandleBombDestroyed; // 폭탄 제거 콜백
        bomb.Ignite(); // 점화 & 타이머 시작
        return bomb;
    }

    // 폭탄이 삭제/폭발/소멸될 때 호출
    private void HandleBombDestroyed(BombController bomb)
    {
        if (ActiveBomb == bomb)
            ActiveBomb = null;
    }
}
