using UnityEngine;
using TMPro;

public class ChargeTextUI : MonoBehaviour
{
    // 점프 충전 수치를 실시간으로 UI에 표시하는 스크립트

    public PlayerChargeJump chargeJump; // PlayerChargeJump 스크립트 참조
    public TextMeshPro textMesh;        // 머리 위에 있는 TextMeshPro

    void Update()
    {
        // 참조가 없으면 아무 것도 하지 않음
        if (chargeJump == null || textMesh == null) return;

        // 충전 중일 때만 숫자 표시
        if (chargeJump.IsCharging)
        {
            textMesh.text = $"{chargeJump.CurrentPower:F0}"; // 소수점 1자리까지 표시
        }
        else
        {
            textMesh.text = ""; // 충전 안 하면 숨김
        }
    }
}
