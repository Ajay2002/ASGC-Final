using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGlowOnSelect : MonoBehaviour
{
	public Color targetColour;
	public float colourLerpSpeed;

	private Material[] glowMaterials;

	private Color currentColor;

	private static readonly int GLOW_COLOUR = Shader.PropertyToID("_GlowColour");

	private void OnEnable ()
	{
		if (glowMaterials == null) glowMaterials = transform.GetChild(0).GetComponent<Renderer>().materials;
	}

	private void Update ()
	{
		if (currentColor.r >= targetColour.r - 0.01f && currentColor.r <= targetColour.r + 0.01f &&
			currentColor.g >= targetColour.g - 0.01f && currentColor.g <= targetColour.g + 0.01f &&
			currentColor.b >= targetColour.b - 0.01f && currentColor.b <= targetColour.b + 0.01f &&
			currentColor.a >= targetColour.a - 0.01f && currentColor.a <= targetColour.a + 0.01f)
			return;

		currentColor = Color.Lerp(currentColor, targetColour, Time.unscaledDeltaTime * colourLerpSpeed);

		foreach (Material glowMaterial in glowMaterials)
		{
			glowMaterial.SetColor(GLOW_COLOUR, currentColor);
		}
	}

	public void SetSelected (bool selected)
	{
		targetColour = selected 
						   ? new Color(targetColour.r, targetColour.g, targetColour.b, 1)
						   : new Color(targetColour.r, targetColour.g, targetColour.b, 0);
	}
}