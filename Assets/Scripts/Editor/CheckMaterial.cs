using UnityEngine;
using UnityEditor;

public class CheckMaterial
{
    public static void Execute()
    {
        // Material360 확인
        var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/360 Music/Material360.mat");
        if (mat == null) { Debug.LogError("[CheckMat] Material360.mat 없음"); return; }

        Debug.Log($"[CheckMat] Shader: {mat.shader.name}");
        Debug.Log($"[CheckMat] mainTexture: {(mat.mainTexture != null ? mat.mainTexture.name : "NULL")}");

        // RenderTexture 확인
        var rt = AssetDatabase.LoadAssetAtPath<RenderTexture>("Assets/360 Music/RenderTexture360.renderTexture");
        Debug.Log($"[CheckMat] RenderTexture360: {(rt != null ? $"{rt.width}x{rt.height}" : "NULL")}");

        // SkyDome MeshRenderer 확인
        var sky = GameObject.Find("SkyDome360");
        if (sky != null)
        {
            var mr = sky.GetComponent<MeshRenderer>();
            Debug.Log($"[CheckMat] SkyDome mat: {(mr?.sharedMaterial != null ? mr.sharedMaterial.name : "NULL")}");
            Debug.Log($"[CheckMat] SkyDome scale: {sky.transform.localScale}");
        }

        // VideoPlayer 확인
        var vp = GameObject.Find("Video")?.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (vp != null)
        {
            Debug.Log($"[CheckMat] VideoPlayer clip: {(vp.clip != null ? vp.clip.name : "NULL")}");
            Debug.Log($"[CheckMat] VideoPlayer renderMode: {vp.renderMode}");
            Debug.Log($"[CheckMat] VideoPlayer targetTexture: {(vp.targetTexture != null ? vp.targetTexture.name : "NULL")}");
        }
    }
}
