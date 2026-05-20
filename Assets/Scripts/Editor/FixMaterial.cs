using UnityEngine;
using UnityEditor;

public class FixMaterial
{
    public static void Execute()
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/360 Music/Material360.mat");
        if (mat == null) { Debug.LogError("[FixMat] Material360.mat 없음"); return; }

        // 기존 mainTexture 보존
        var tex = mat.mainTexture;

        // Unlit/Texture 셰이더로 변경 (조명 영향 없이 텍스처만 표시)
        var shader = Shader.Find("Unlit/Texture");
        if (shader == null) { Debug.LogError("[FixMat] Unlit/Texture 셰이더 없음"); return; }

        mat.shader = shader;
        mat.mainTexture = tex; // RenderTexture360 재할당

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();

        Debug.Log($"[FixMat] 셰이더 변경 완료: Skybox/Panoramic → Unlit/Texture");
        Debug.Log($"[FixMat] mainTexture: {(mat.mainTexture != null ? mat.mainTexture.name : "NULL")}");
    }
}
