using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowImageEffect : MonoBehaviour
{
	public Material glowImageEffectMaterial;
	public float intensity = 1;
	
	private void OnRenderImage (RenderTexture src, RenderTexture dest)
	{
		glowImageEffectMaterial.SetFloat("_Intensity", intensity);
		Graphics.Blit(src, dest, glowImageEffectMaterial, 0);
	}
}
