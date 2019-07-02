using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
	public  Transform      selectionBoxSpriteTransform;
	private SpriteRenderer selectionBoxSprite;

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

	private void Start ()
	{
		selectionBoxSprite = selectionBoxSpriteTransform.GetComponent<SpriteRenderer>();
		selectionBoxSpriteTransform.gameObject.SetActive(false);
	}

	private void Update ()
	{
		//if (MouseInputUIBlocker.BlockedByUI) return;

		if (Input.GetMouseButtonDown(1)) BeginDrag();
		if (dragging && Input.GetMouseButton(1)) UpdateDrag();
		if (dragging && Input.GetMouseButtonUp(1)) EndDrag();

		FinishDrags();

		if (!dragging)
		{
			if (Input.GetMouseButtonDown(0)) BeginBoxSelect();
			if (selecting && Input.GetMouseButton(0)) UpdateBoxSelect();
			if (selecting && Input.GetMouseButtonUp(0)) EndBoxSelect();
		}
	}


	#region Selection

	private void SelectCurrent (Ray ray)
	{
		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit) || !hit.transform.CompareTag("Player")) return;

		if (selectedEntityTransforms.Contains(hit.transform.parent))
		{
			hit.transform.GetComponent<EntityGlowOnSelect>().SetSelected(false); //Remove Highlighting from Entities

			selectedEntityTransforms.Remove(hit.transform.parent);
			selectedEntities.Remove(hit.transform.parent.GetComponent<GeneticEntity_T>());
		}
		else
		{
			hit.transform.parent.GetComponent<EntityGlowOnSelect>().SetSelected(true); //Add Highlighting to Entities

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

		selecting              = true;
		boxSelectStartPosition = MapManager.Instance.NearestPointOnMap(hit.point);
		boxSelectEndPosition   = boxSelectStartPosition;


		//Update the graphics for the selection box
		selectionBoxSpriteTransform.gameObject.SetActive(true);

		Vector3 centre = new Vector3((boxSelectStartPosition.x + boxSelectEndPosition.x) / 2,
									 0.1f,
									 (boxSelectStartPosition.z + boxSelectEndPosition.z) / 2
									);

		Vector3 size = new Vector2((boxSelectEndPosition.x - boxSelectStartPosition.x) / selectionBoxSpriteTransform.lossyScale.x,
								   (boxSelectEndPosition.z - boxSelectStartPosition.z) / selectionBoxSpriteTransform.lossyScale.y
								  );

		selectionBoxSpriteTransform.position = centre;
		selectionBoxSprite.size              = size;
	}

	private void UpdateBoxSelect ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		LayerMask mask = LayerMask.GetMask("Ground");

		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		boxSelectEndPosition = MapManager.Instance.NearestPointOnMap(hit.point);

		//Update the graphics for the selection box
		Vector3 centre = new Vector3((boxSelectStartPosition.x + boxSelectEndPosition.x) / 2,
									 0.1f,
									 (boxSelectStartPosition.z + boxSelectEndPosition.z) / 2
									);

		Vector3 size = new Vector2((boxSelectEndPosition.x - boxSelectStartPosition.x) / selectionBoxSpriteTransform.lossyScale.x,
								   (boxSelectEndPosition.z - boxSelectStartPosition.z) / selectionBoxSpriteTransform.lossyScale.y
								  );

		selectionBoxSpriteTransform.position = centre;
		selectionBoxSprite.size              = size;
	}

	private void EndBoxSelect ()
	{
		selecting = false;

		if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.RightShift) == false)
		{
			foreach (GeneticEntity_T entity in selectedEntities)
			{
				if (entity != null) entity.GetComponent<EntityGlowOnSelect>().SetSelected(false); // Remove Highlighting from Entities
			}

			selectedEntityTransforms.Clear();
			selectedEntities.Clear();
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		SelectCurrent(ray);

		LayerMask mask = LayerMask.GetMask("Ground");

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) boxSelectEndPosition = MapManager.Instance.NearestPointOnMap(hit.point);

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
				eo.GetComponent<EntityGlowOnSelect>().SetSelected(true); //Add Highlighting to Entities

				selectedEntityTransforms.Add(eo.transform);
				selectedEntities.Add(eo.GetComponent<GeneticEntity_T>());
			}
		}
		
		//Turn off visuals for the selection
		selectionBoxSpriteTransform.gameObject.SetActive(false);
	}

	#endregion


	#region Dragging

	private void BeginDrag ()
	{
		dragging = true;

		for (int i = finishingDrags.Count - 1; i >= 0; i--)
		{
			Debug.Log(i);
			if (selectedEntityTransforms.Contains(finishingDrags[i].transform)) finishingDrags.RemoveAt(i);
		}

		for (int i = selectedEntities.Count - 1; i >= 0; i--)
		{
			GeneticEntity_T entity = selectedEntities[i];

			if (entity == null)
			{
				selectedEntities.RemoveAt(i);
				selectedEntityTransforms.RemoveAt(i);
				continue;
			}

			entity.GetComponent<NavMeshAgent>().enabled      = false;
			entity.GetComponent<GeneticController>().enabled = false;
			entity.enabled                                   = false;
			entity.transform.GetChild(0).gameObject.layer    = 2;
		}
	}

	private void UpdateDrag ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		LayerMask mask = ~(1 << 2); //Collides with all layers except layer 2

		//mask = LayerMask.GetMask("Ground");

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		Vector3 currentMouseWorldPosition = hit.point;

		Debug.DrawLine(Camera.main.transform.position, currentMouseWorldPosition);


		for (int i = 0; i < selectedEntityTransforms.Count; i++)
		{
			if (selectedEntityTransforms[i] == null) continue;

			selectedEntityTransforms[i].position = Vector3.Lerp(selectedEntityTransforms[i].position,
																currentMouseWorldPosition + Vector3.up +
																DragDisplacementFunction(i),
																Time.deltaTime * dragFollowSpeed / Time.timeScale);
		}
	}

	private void EndDrag ()
	{
		dragging = false;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		LayerMask mask = ~(1 << 2); //Collides with all layers except layer 2

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		Vector3 currentMouseWorldPosition = hit.point;


		for (int i = 0; i < selectedEntityTransforms.Count; i++)
		{
			if (selectedEntityTransforms[i] == null) continue;

			FinishingDrag fd = new FinishingDrag
			{
				transform = selectedEntityTransforms[i],
				entity    = selectedEntities[i],
				position  = MapManager.Instance.NearestPointOnMap(currentMouseWorldPosition + DragDisplacementFunction(i))
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
				drag.transform.GetChild(0).gameObject.layer           = 0;

				finishingDrags.RemoveAt(i);

				continue;
			}

			drag.transform.position = Vector3.Lerp(drag.transform.position, drag.position,
												   Time.deltaTime * dragFollowSpeed / Time.timeScale);
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

	#endregion

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