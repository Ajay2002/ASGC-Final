using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
	public float dragFollowSpeed = 1f;

	public Vector3 entityHeight;

	private readonly List<Transform>       selectedEntityTransforms = new List<Transform>();
	private readonly List<GeneticEntity_T> selectedEntities         = new List<GeneticEntity_T>();

	private float checkTime;

	private bool dragging;

	private bool selecting;

	private Vector3 boxSelectStartPosition;
	private Vector3 boxSelectEndPosition;

	private readonly List<FinishingDrag> finishingDrags = new List<FinishingDrag>();

	private struct FinishingDrag
	{
		public Transform       transform;
		public GeneticEntity_T entity;
		public Vector3         position;
	}

	private void Update ()
	{
		//if (MouseInputUIBlocker.BlockedByUI) return;

		if (Input.GetMouseButtonDown(0)) BeginBoxSelect();
		if (selecting    && Input.GetMouseButton(0)) UpdateBoxSelect();
		if (selecting    && Input.GetMouseButtonUp(0)) EndBoxSelect();

		if (Input.GetMouseButtonDown(1)) BeginDrag();
		if (dragging   && Input.GetMouseButton(1)) UpdateDrag();
		if (dragging   && Input.GetMouseButtonUp(1)) EndDrag();

		FinishDrags();
	}

	private void SelectCurrent (Ray ray)
	{
		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit) || !hit.transform.CompareTag("Player")) return;

		if (selectedEntityTransforms.Contains(hit.transform.parent))
		{
			selectedEntityTransforms.Remove(hit.transform.parent);
			selectedEntities.Remove(hit.transform.parent.GetComponent<GeneticEntity_T>());
		}
		else
		{
			selectedEntityTransforms.Add(hit.transform.parent);
			selectedEntities.Add(hit.transform.parent.GetComponent<GeneticEntity_T>());
		}
	}

	private void BeginBoxSelect ()
	{
		if (Camera.main == null)
		{
			throw new Exception();
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		LayerMask mask = LayerMask.GetMask("Ground");

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		selecting = true;
		boxSelectStartPosition = hit.point;

		//TODO: Update Visuals of box select
	}

	private void UpdateBoxSelect ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		LayerMask mask = LayerMask.GetMask("Ground");

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		boxSelectEndPosition = hit.point;

		//TODO:
	}

	private void EndBoxSelect ()
	{
		selecting = false;

		if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.RightShift) == false)
		{
			selectedEntityTransforms.Clear();
			selectedEntities.Clear();
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		SelectCurrent(ray);

		LayerMask mask = LayerMask.GetMask("Ground");

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) boxSelectEndPosition = hit.point;

		//TODO: Update Visuals of box select

		GameObject[] entityObjects = GameObject.FindGameObjectsWithTag("Player");

		foreach (GameObject eo in entityObjects)
		{
			if (eo.name == "Model") continue;

			Vector3 pos = eo.transform.position;

			float minX = Mathf.Min(boxSelectStartPosition.x, boxSelectEndPosition.x);
			float minZ = Mathf.Min(boxSelectStartPosition.z, boxSelectEndPosition.z);
			float maxX = Mathf.Max(boxSelectStartPosition.x, boxSelectEndPosition.x);
			float maxZ = Mathf.Max(boxSelectStartPosition.z, boxSelectEndPosition.z);

			if (minX < pos.x && pos.x < maxX &&
				minZ < pos.z && pos.z < maxZ)
			{
				selectedEntityTransforms.Add(eo.transform);
				selectedEntities.Add(eo.GetComponent<GeneticEntity_T>());
			}
		}
	}

	private void BeginDrag ()
	{
		dragging        = true;

		foreach (var entity in selectedEntities)
		{
			entity.GetComponent<NavMeshAgent>().enabled      = false;
			entity.GetComponent<GeneticController>().enabled = false;
			entity.enabled                                   = false;
		}
	}

	private void UpdateDrag ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit)) return;

		Vector3 currentMouseWorldPosition = hit.point;

		Debug.DrawLine(Camera.main.transform.position, currentMouseWorldPosition);


		for (int i = 0; i < selectedEntityTransforms.Count; i++)
		{
			if (selectedEntityTransforms[i] == null) continue;

			selectedEntityTransforms[i].position = Vector3.Lerp(selectedEntityTransforms[i].position,
																currentMouseWorldPosition + Vector3.up +
																DragDisplacementFunction(i),
																Time.deltaTime * dragFollowSpeed);
		}
	}

	private void EndDrag ()
	{
		dragging = false;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit)) return;

		Vector3 currentMouseWorldPosition = hit.point;


		for (int i = 0; i < selectedEntityTransforms.Count; i++)
		{
			if (selectedEntityTransforms[i] == null) continue;

			FinishingDrag fd = new FinishingDrag
			{
				transform = selectedEntityTransforms[i],
				entity    = selectedEntities[i],
				position  = currentMouseWorldPosition + DragDisplacementFunction(i)
			};

			finishingDrags.Add(fd);
		}
	}

	private void FinishDrags ()
	{
		for (int i = finishingDrags.Count - 1; i >= 0; i--)
		{
			FinishingDrag drag = finishingDrags[i];

			if (Vector3.SqrMagnitude(drag.position - drag.transform.position) < 0.005f)
			{
				//Drag Finished
				drag.transform.position                               = drag.position;
				drag.entity.enabled                                   = true;
				drag.entity.GetComponent<NavMeshAgent>().enabled      = true;
				drag.entity.GetComponent<GeneticController>().enabled = true;

				finishingDrags.RemoveAt(i);

				continue;
			}

			drag.transform.position = Vector3.Lerp(drag.transform.position, drag.position,
												   Time.deltaTime * dragFollowSpeed);
		}
	}

	private Vector3 DragDisplacementFunction (int i)
	{
		const float radianAngle = 137.5f / 360 * Mathf.PI * 2;
		const float c           = 0.5f;

		Vector3 displacement = new Vector3(c * Mathf.Sqrt(i) * Mathf.Cos(i * radianAngle), 0,
										   c * Mathf.Sqrt(i) * Mathf.Sin(i * radianAngle));

		return displacement + entityHeight / 2;
	}

	private void OnDrawGizmos ()
	{
		if (!selecting) return;

		Vector3 centre = new Vector3((boxSelectStartPosition.x + boxSelectEndPosition.x) / 2,
									 0,
									 (boxSelectStartPosition.z + boxSelectEndPosition.z) / 2
									);

		Vector3 size = new Vector3((boxSelectEndPosition.x - boxSelectStartPosition.x),
								   0.01f,
								   (boxSelectEndPosition.z - boxSelectStartPosition.z)
								  );

		Gizmos.DrawWireCube(centre, size);
	}
}