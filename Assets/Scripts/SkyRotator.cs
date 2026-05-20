using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
// SkyRotator — SkyDome360 구체를 천천히 Y축으로 회전시켜 배경에 움직임을 줍니다.
//
// ★ 사용법:
//   SkyDome360 오브젝트에 이 컴포넌트가 붙어 있습니다.
//   Inspector에서 Speed 값을 조절하세요.
//     - 0    : 회전 없음 (정지 배경)
//     - 1~3  : 느린 회전 (권장)
//     - 10+  : 빠른 회전 (어지러울 수 있음)
//
// ★ 회전을 끄고 싶을 때:
//   Inspector에서 이 컴포넌트의 체크박스를 해제하거나 Speed = 0 으로 설정
// ─────────────────────────────────────────────────────────────────────────────
public class SkyRotator : MonoBehaviour
{
    [Tooltip("초당 회전 각도 (도). 0 = 정지, 1~3 = 느린 회전 권장")]
    public float speed = 1.5f;

    void Update()
    {
        // World 기준 Y축으로 회전 — Space.World 를 써야 구체가 기울어져도 수평 유지
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
    }
}
