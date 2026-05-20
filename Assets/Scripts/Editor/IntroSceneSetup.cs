using UnityEngine;
using UnityEngine.Video;
using UnityEditor;

public class IntroSceneSetup
{
    public static void Execute()
    {
        // 1. 기존 불필요한 오브젝트 정리
        foreach (var name in new[] { "Cube", "Cube (1)", "Spawner", "Canvas" })
        {
            var go = GameObject.Find(name);
            if (go != null) Object.DestroyImmediate(go);
        }

        // 2. SkyDome360 — 내부에서 보이도록 음수 X 스케일
        var existing = GameObject.Find("SkyDome360");
        if (existing != null) Object.DestroyImmediate(existing);

        var skyDome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        skyDome.name = "SkyDome360";
        skyDome.transform.localScale = new Vector3(-100f, 100f, 100f);
        Object.DestroyImmediate(skyDome.GetComponent<SphereCollider>());

        var skyRenderer = skyDome.GetComponent<MeshRenderer>();
        skyRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        skyRenderer.receiveShadows = false;
        skyRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        skyRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        var mat360 = AssetDatabase.LoadAssetAtPath<Material>("Assets/360 Music/Material360.mat");
        if (mat360 != null)
            skyRenderer.sharedMaterial = mat360;
        else
            Debug.LogWarning("[IntroSetup] Material360.mat 를 찾지 못했습니다.");

        // 3. VideoPlayer — motion.mp4 를 SkyDome 메인 텍스처에 출력
        var existingVP = GameObject.Find("VideoPlayer360");
        if (existingVP != null) Object.DestroyImmediate(existingVP);

        var vpGo = new GameObject("VideoPlayer360");
        var vp = vpGo.AddComponent<VideoPlayer>();
        vp.playOnAwake = true;
        vp.isLooping = true;
        vp.renderMode = VideoRenderMode.MaterialOverride;
        vp.targetMaterialRenderer = skyRenderer;
        vp.targetMaterialProperty = "_MainTex";
        var videoClip = AssetDatabase.LoadAssetAtPath<VideoClip>("Assets/360 Music/motion.mp4");
        if (videoClip != null)
            vp.clip = videoClip;
        else
            Debug.LogWarning("[IntroSetup] motion.mp4 를 찾지 못했습니다.");

        // 4. BGM AudioSource
        var existingBGM = GameObject.Find("BGM");
        if (existingBGM != null) Object.DestroyImmediate(existingBGM);

        var bgmGo = new GameObject("BGM");
        var bgmSrc = bgmGo.AddComponent<AudioSource>();
        bgmSrc.loop = true;
        bgmSrc.playOnAwake = true;
        bgmSrc.spatialBlend = 0f;
        bgmSrc.volume = 0.6f;
        var bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/360 Music/About That Oldie - Vibe Tracks.mp3");
        if (bgmClip != null)
            bgmSrc.clip = bgmClip;

        // 5. Directional Light 분위기 조정
        var dirLight = GameObject.Find("Directional Light");
        if (dirLight != null)
        {
            var light = dirLight.GetComponent<Light>();
            if (light != null)
            {
                light.color = new Color(0.4f, 0.45f, 0.9f);
                light.intensity = 0.4f;
            }
        }

        Debug.Log("[IntroSetup] SkyDome & Audio 설정 완료");
    }
}
