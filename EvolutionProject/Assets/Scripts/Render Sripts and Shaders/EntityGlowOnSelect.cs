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
		
		currentColor = Color.Lerp(currentColor, targetColour, Time.deltaTime * colourLerpSpeed);

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