using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReplacementShaderEffect : MonoBehaviour
{
	public Shader replacementShader;
	public Color overDrawColour;

	private Camera camera;

	private void OnEnable ()
	{
		if (camera == null) camera = GetComponent<Camera>();
		
		if (replacementShader != null) camera.SetReplacementShader(replacementShader, "");
	}

	private void OnDisable ()
	{
		camera.ResetReplacementShader();
	}

	private void OnValidate ()
	{
		Shader.SetGlobalColor("_OverDrawColour", overDrawColour);
	}
}
