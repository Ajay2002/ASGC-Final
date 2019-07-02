using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGlowOnSelect : MonoBehaviour
{
	public Color targetColour;
	public float colourLerpSpeed;

	private Material glowMaterial;

	private Color currentColor;

	private void OnEnable ()
	{
		if (glowMaterial == null) glowMaterial = transform.GetChild(0).GetComponent<Renderer>().material;
	}

	private void Update ()
	{
		currentColor = Color.Lerp(currentColor, targetColour, Time.deltaTime * colourLerpSpeed);

		glowMaterial.SetColor("_GlowColour", currentColor);
	}

	public void SetSelected (bool selected)
	{
		targetColour = selected
						   ? new Color(targetColour.r, targetColour.g, targetColour.b, 1)
						   : new Color(targetColour.r, targetColour.g, targetColour.b, 0);
	}
}