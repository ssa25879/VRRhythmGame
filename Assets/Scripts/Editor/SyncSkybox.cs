using UnityEditor;
using UnityEngine;

public class SyncSkybox : Editor
{
    [MenuItem("VRBeatSaber/Sync Skybox Settings")]
    public static void Execute()
    {
        string matPath = "Assets/360 Music/Material360.mat";
        string rtPath = "Assets/360 Music/RenderTexture360.renderTexture";

        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(rtPath);

        if (mat == null || rt == null)
        {
            Debug.LogError("[SyncSkybox] Material 또는 RenderTexture를 찾을 수 없습니다.");
            return;
        }

        // 1. 셰이더 변경 및 속성 설정
        mat.shader = Shader.Find("Skybox/Panoramic");
        mat.SetTexture("_Tex", rt);
        mat.SetFloat("_Mapping", 1); // Latitude Longitude Layout
        mat.SetFloat("_ImageType", 0); // 360 degrees
        
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();

        // 2. 현재 씬의 스카이박스 설정 적용
        RenderSettings.skybox = mat;
        
        Debug.Log("[SyncSkybox] 배경 렌더링 방식 동기화 완료 (Skybox/Panoramic 적용)");
    }
}
