using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowCamera : MonoBehaviour
{
    public Renderer debugRenderer1;
    public Renderer debugRenderer2;
    
    public Material blurMaterial;
    
    private RenderTexture prePass;
    private RenderTexture blurred;
    
    private static readonly int GLOW_BLURRED_TEX = Shader.PropertyToID("_GlowBlurredTex");

    private void Start()
    {
        prePass = new RenderTexture(Screen.width, Screen.height, 24);
        blurred = new RenderTexture(Screen.width, Screen.height, 24);
        Camera camera = GetComponent<Camera>();
        Shader glowShader = Shader.Find("Hidden/GlowReplace");
        camera.targetTexture = prePass;
        camera.SetReplacementShader(glowShader, "Glowable");
        Shader.SetGlobalTexture("_GlowPrePassTex", prePass);

        debugRenderer1.material.mainTexture = prePass;
    }

    private void OnRenderImage (RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest);
        
        Graphics.SetRenderTarget(blurred);
        
        GL.Clear(false, true, Color.clear);
        
        Graphics.Blit(src, blurred);

        for (int i = 0; i < 1; i++)
        {
            RenderTexture temp = RenderTexture.GetTemporary(blurred.width, blurred.height);
            Graphics.Blit(blurred, temp, blurMaterial);
            Graphics.Blit(temp, blurred, blurMaterial);
            RenderTexture.ReleaseTemporary(temp);
        }

        debugRenderer2.material.mainTexture = blurred;
        
        Shader.SetGlobalTexture(GLOW_BLURRED_TEX, blurred);
    }
}
