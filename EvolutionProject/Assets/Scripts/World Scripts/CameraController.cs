using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Vector3 minPosLimits;
	public Vector3 maxPosLimits;

	public float moveSpeed;
	public float zoomSpeed;

	private Vector3 translatePosition;
	private Vector3 zoomPosition;

	private void Start ()
	{
		var position = transform.position;
		translatePosition = new Vector3(position.x, 0, position.z);
		zoomPosition      = new Vector3(0, position.y, 0);
	}

	private void Update ()
	{
		Vector3 shift = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * moveSpeed * Time.deltaTime *
						((translatePosition.y - minPosLimits.y) / (maxPosLimits.y - minPosLimits.y) * 0.5f + 0.5f); //Slowing down movement when zoomed in

		float newX = Mathf.Clamp(translatePosition.x + shift.x, minPosLimits.x, maxPosLimits.x);
		float newZ = Mathf.Clamp(translatePosition.z + shift.z, minPosLimits.z, maxPosLimits.z);

		translatePosition = new Vector3(newX, 0, newZ);

		float zoomMagnitude                                                                                                                = Input.mouseScrollDelta.y * zoomSpeed;
		float rot = transform.rotation.eulerAngles.x / 180 * Mathf.PI;
		if (-zoomMagnitude * Mathf.Sin(rot) + zoomPosition.y > maxPosLimits.y) zoomMagnitude = -(maxPosLimits.y - zoomPosition.y) / Mathf.Sin(rot);
		if (-zoomMagnitude * Mathf.Sin(rot) + zoomPosition.y < minPosLimits.y) zoomMagnitude = -(minPosLimits.y - zoomPosition.y) / Mathf.Sin(rot);
		zoomPosition += transform.forward * zoomMagnitude;

		transform.position = translatePosition + zoomPosition;
	}
}