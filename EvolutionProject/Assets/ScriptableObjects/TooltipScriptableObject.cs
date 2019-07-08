using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tooltip", menuName = "Tooltips/Tooltip")]
public class TooltipScriptableObject : ScriptableObject
{
	public string ID;
	public string heading;
	public string text;
}
