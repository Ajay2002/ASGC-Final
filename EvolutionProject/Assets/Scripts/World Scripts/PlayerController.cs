using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;

public class PlayerController : MonoBehaviour
{
	public float dragFollowSpeed = 1f;

	public Vector3 entityHeight;

	private List<Transform>       selectedEntityTransforms = new List<Transform>();
	private List<GeneticEntity_T> selectedEntities         = new List<GeneticEntity_T>();

	private bool checkingForDrag;
	private bool dragging;

	private Vector3 boxSelectStartPosition;
	private Vector3 boxSelectEndPosition;

	private List<FinishingDrag> finishingDrags = new List<FinishingDrag>();

	private struct FinishingDrag
	{
		public Transform       transform;
		public GeneticEntity_T entity;
		public Vector3         position;
	}

	void Update ()
	{
		//if (MouseInputUIBlocker.BlockedByUI) return;

		/*
		 if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetMouseButtonDown(0))
		{
			BeginBoxSelect();
			return;
		}
		*/

		if (Input.GetMouseButtonDown(0))
		{
			SelectCurrent();
			return;
		}

		if (Input.GetMouseButtonDown(1))
		{
			checkingForDrag = true;
			return;
		}

		if (checkingForDrag && Input.GetMouseButton(1))
		{
			BeginDrag();
			return;
		}

		if (dragging && Input.GetMouseButton(1))
		{
			UpdateDrag();
			return;
		}

		if (dragging && Input.GetMouseButtonUp(1))
		{
			EndDrag();
			return;
		}

		FinishDrags();
	}

	void SelectCurrent ()
	{
		if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.RightShift) == false)
		{
			selectedEntityTransforms.Clear();
			selectedEntities.Clear();
		}

		if (Camera.main == null)
		{
			throw new Exception();
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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

	void BeginBoxSelect ()
	{
		if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.RightShift) == false)
		{
			selectedEntityTransforms.Clear();
			selectedEntities.Clear();
		}

		if (Camera.main == null)
		{
			throw new Exception();
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit)) return;

		boxSelectStartPosition = hit.point;

		//TODO: Update Visuals of box select
	}

	void UpdateBoxSelect ()
	{
		//TODO:
	}

	void EndBoxSelect ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			boxSelectEndPosition = hit.point + Vector3.up * 5;

			//TODO: Update Visuals of box select
		}
	}

	void BeginDrag ()
	{
		UnityEngine.Debug.Log("beginning drag");
		checkingForDrag = false;
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
		UnityEngine.Debug.Log("dragging");

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit)) return;

		Vector3 currentMouseWorldPosition = hit.point;

		UnityEngine.Debug.DrawLine(Camera.main.transform.position, currentMouseWorldPosition);


		for (int i = 0; i < selectedEntityTransforms.Count; i++)
		{
			selectedEntityTransforms[i].position = Vector3.Lerp(selectedEntityTransforms[i].position,
																currentMouseWorldPosition + Vector3.up +
																DragDisplacementFunction(i),
																Time.deltaTime * dragFollowSpeed);
		}
	}


	private void EndDrag ()
	{
		dragging = false;
		
		UnityEngine.Debug.Log("ending drag");

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit)) return;

		Vector3 currentMouseWorldPosition = hit.point;


		for (int i = 0; i < selectedEntityTransforms.Count; i++)
		{
			FinishingDrag fd = new FinishingDrag
			{
				transform = selectedEntityTransforms[i],
				entity    = selectedEntities[i],
				position  = currentMouseWorldPosition + DragDisplacementFunction(i)
			};
			
			finishingDrags.Add(fd);
		}
		
		UnityEngine.Debug.Log(finishingDrags[0]);
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
		return Vector3.zero + entityHeight / 2;
	}
}