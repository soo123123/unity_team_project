using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;          // 따라갈 플레이어
    public Vector3 offset = new Vector3(0, 1f, -10f); // 카메라 위치 오프셋

    [Header("Smooth Settings")]
    public float smoothSpeed = 0.125f; // 값이 낮을수록 더 부드럽게
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // player가 비어있으면 오류 방지
        if (player == null) return;

        // 목표 위치 계산
        Vector3 desiredPosition = player.position + offset;

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothSpeed
        );
    }
}
