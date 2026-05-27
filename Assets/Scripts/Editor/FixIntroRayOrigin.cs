using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Intro 씬의 XR Ray Origin 문제 수정 스크립트
/// - Old Near-Far Interactor (컨트롤러 하위) 비활성화
/// - Hand Near-Far Interactor 비활성화
/// - Teleport Stabilized Origin 비활성화
/// - Gaze / Teleport Interactor 비활성화 상태 검증
/// </summary>
public class FixIntroRayOrigin
{
    [MenuItem("VRBeatSaber/Fix Intro Ray Origin")]
    public static void Execute()
    {
        // Intro 씬 경로 확인
        Scene introScene = SceneManager.GetSceneByName("Intro");
        if (!introScene.IsValid())
        {
            Debug.LogError("[FixIntroRayOrigin] Intro 씬을 찾을 수 없습니다. Intro 씬을 먼저 열어주세요.");
            return;
        }

        if (!introScene.isLoaded)
        {
            Debug.LogError("[FixIntroRayOrigin] Intro 씬이 로드되어 있지 않습니다.");
            return;
        }

        Debug.Log("=== [FixIntroRayOrigin] 시작 ===");

        // XR Origin Hands 루트 오브젝트 찾기
        GameObject xrOrigin = null;
        foreach (var root in introScene.GetRootGameObjects())
        {
            if (root.name == "XR Origin Hands (XR Rig)")
            {
                xrOrigin = root;
                break;
            }
        }

        if (xrOrigin == null)
        {
            Debug.LogError("[FixIntroRayOrigin] 'XR Origin Hands (XR Rig)' 를 찾을 수 없습니다.");
            return;
        }

        // ─────────────────────────────────────────────
        // 1. 비활성화 대상 오브젝트 목록 (path는 XR Origin 하위 기준)
        // ─────────────────────────────────────────────
        string[] toDeactivate = new string[]
        {
            // 구형 Near-Far Interactor (NearFarInteractor 컴포넌트 disabled, 하지만 GO 활성)
            "Camera Offset/Left Controller/Near-Far Interactor",
            "Camera Offset/Right Controller/Near-Far Interactor",

            // 핸드 기반 Near-Far Interactor (컨트롤러 없을 때 head 위치에서 Ray 발생 가능)
            "Camera Offset/Left Hand/Near-Far Interactor",
            "Camera Offset/Right Hand/Near-Far Interactor",

            // Teleport 보조 Origin (Teleport Interactor가 inactive여도 GO는 살아 있을 수 있음)
            "Camera Offset/Left Controller Teleport Stabilized Origin",
            "Camera Offset/Right Controller Teleport Stabilized Origin",

            // Gaze 관련 (이미 inactive 일 수 있으나 명시적으로 확인 후 비활성화)
            "Camera Offset/Gaze Interactor",
            "Camera Offset/Gaze Stabilized",

            // Teleport Interactor (이미 inactive 일 수 있으나 명시적으로 확인)
            "Camera Offset/Left Controller/Teleport Interactor",
            "Camera Offset/Right Controller/Teleport Interactor",
        };

        // ─────────────────────────────────────────────
        // 2. 활성 유지 대상 (검증용)
        // ─────────────────────────────────────────────
        string[] toKeepActive = new string[]
        {
            "Camera Offset/Left Controller/Left_NearFarInteractor",
            "Camera Offset/Right Controller/Right_NearFarInteractor",
            "Camera Offset/Left Controller/VisibleUIPointer",
            "Camera Offset/Right Controller/VisibleUIPointer",
        };

        int deactivatedCount = 0;
        int alreadyInactiveCount = 0;

        foreach (string path in toDeactivate)
        {
            Transform t = xrOrigin.transform.Find(path);
            if (t == null)
            {
                Debug.LogWarning($"[FixIntroRayOrigin] 오브젝트를 찾을 수 없음: {path}");
                continue;
            }

            if (t.gameObject.activeSelf)
            {
                Undo.RecordObject(t.gameObject, "Deactivate Ray Origin Object");
                t.gameObject.SetActive(false);
                deactivatedCount++;
                Debug.Log($"[FixIntroRayOrigin] ✓ 비활성화 완료: {path}");
            }
            else
            {
                alreadyInactiveCount++;
                Debug.Log($"[FixIntroRayOrigin] - 이미 비활성화 상태: {path}");
            }
        }

        // ─────────────────────────────────────────────
        // 3. 활성 유지 대상 검증
        // ─────────────────────────────────────────────
        Debug.Log("=== [FixIntroRayOrigin] 활성 유지 오브젝트 검증 ===");
        foreach (string path in toKeepActive)
        {
            Transform t = xrOrigin.transform.Find(path);
            if (t == null)
            {
                Debug.LogError($"[FixIntroRayOrigin] 활성 유지 대상을 찾을 수 없음: {path}");
                continue;
            }

            bool isActive = t.gameObject.activeInHierarchy;
            if (isActive)
            {
                Debug.Log($"[FixIntroRayOrigin] ✓ 활성 상태 정상: {path}");
            }
            else
            {
                Debug.LogWarning($"[FixIntroRayOrigin] ⚠ 활성 유지 대상이 비활성 상태: {path}");
            }
        }

        // ─────────────────────────────────────────────
        // 4. VisibleUIPointer rayOrigin 검증
        // ─────────────────────────────────────────────
        Debug.Log("=== [FixIntroRayOrigin] VisibleUIPointer rayOrigin 검증 ===");
        string[] visiblePointerPaths = new string[]
        {
            "Camera Offset/Left Controller/VisibleUIPointer",
            "Camera Offset/Right Controller/VisibleUIPointer",
        };
        string[] expectedRayOrigins = new string[]
        {
            "Camera Offset/Left Controller",
            "Camera Offset/Right Controller",
        };

        for (int i = 0; i < visiblePointerPaths.Length; i++)
        {
            Transform t = xrOrigin.transform.Find(visiblePointerPaths[i]);
            if (t == null) continue;

            var visualizer = t.GetComponent<ControllerPointerVisualizer>();
            if (visualizer == null)
            {
                Debug.LogWarning($"[FixIntroRayOrigin] ControllerPointerVisualizer 없음: {visiblePointerPaths[i]}");
                continue;
            }

            // SerializedObject로 rayOrigin 필드 읽기
            SerializedObject so = new SerializedObject(visualizer);
            SerializedProperty rayOriginProp = so.FindProperty("rayOrigin");
            if (rayOriginProp != null && rayOriginProp.objectReferenceValue != null)
            {
                Transform rayOriginTf = rayOriginProp.objectReferenceValue as Transform;
                string expected = expectedRayOrigins[i];
                Transform expectedTf = xrOrigin.transform.Find(expected);

                if (rayOriginTf == expectedTf)
                {
                    Debug.Log($"[FixIntroRayOrigin] ✓ rayOrigin 정상: {visiblePointerPaths[i]} → {rayOriginTf.name}");
                }
                else
                {
                    Debug.LogWarning($"[FixIntroRayOrigin] ⚠ rayOrigin 불일치: {visiblePointerPaths[i]} → {rayOriginTf?.name ?? "null"} (기대: {expected})");
                }
            }
            else
            {
                Debug.LogWarning($"[FixIntroRayOrigin] rayOrigin 참조 없음: {visiblePointerPaths[i]}");
            }
        }

        // ─────────────────────────────────────────────
        // 5. NearFarInteractor castOrigin 검증
        // ─────────────────────────────────────────────
        Debug.Log("=== [FixIntroRayOrigin] NearFarInteractor castOrigin 검증 ===");
        string[] nearFarInteractors = new string[]
        {
            "Camera Offset/Left Controller/Left_NearFarInteractor",
            "Camera Offset/Right Controller/Right_NearFarInteractor",
        };

        foreach (string path in nearFarInteractors)
        {
            Transform t = xrOrigin.transform.Find(path);
            if (t == null)
            {
                Debug.LogWarning($"[FixIntroRayOrigin] NearFarInteractor 찾을 수 없음: {path}");
                continue;
            }

            // CurveInteractionCaster 컴포넌트 확인
            var casters = t.GetComponents<MonoBehaviour>();
            foreach (var c in casters)
            {
                if (c == null) continue;
                string typeName = c.GetType().Name;
                if (typeName == "CurveInteractionCaster")
                {
                    SerializedObject so = new SerializedObject(c);
                    SerializedProperty castOriginProp = so.FindProperty("m_CastOrigin");
                    if (castOriginProp == null) castOriginProp = so.FindProperty("castOrigin");
                    if (castOriginProp != null && castOriginProp.objectReferenceValue != null)
                    {
                        Transform castOriginTf = castOriginProp.objectReferenceValue as Transform;
                        // Camera나 Camera Offset이 아닌지 확인
                        bool isCamera = castOriginTf.name.Contains("Camera") && !castOriginTf.name.Contains("Controller");
                        if (isCamera)
                        {
                            Debug.LogError($"[FixIntroRayOrigin] ⚠ castOrigin이 카메라 관련 오브젝트: {path} → {castOriginTf.name}");
                        }
                        else
                        {
                            Debug.Log($"[FixIntroRayOrigin] ✓ castOrigin 정상: {path} → {castOriginTf.name}");
                        }
                    }
                }
            }
        }

        // ─────────────────────────────────────────────
        // 6. 씬 저장
        // ─────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(introScene);
        bool saved = EditorSceneManager.SaveScene(introScene);

        Debug.Log($"=== [FixIntroRayOrigin] 완료 ===");
        Debug.Log($"  비활성화: {deactivatedCount}개, 이미 비활성: {alreadyInactiveCount}개");
        Debug.Log($"  씬 저장: {(saved ? "성공" : "실패")}");

        if (saved)
        {
            Debug.Log("[FixIntroRayOrigin] ✅ Intro.unity 저장 완료");
        }
        else
        {
            Debug.LogError("[FixIntroRayOrigin] ❌ 씬 저장 실패");
        }
    }
}
