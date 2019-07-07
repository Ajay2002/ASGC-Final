using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance;

	[FormerlySerializedAs("maxSelectTime")]
	public float maxDragTime;

	public  Transform      selectionBoxSpriteTransform;
	private SpriteRenderer selectionBoxSprite;

	public float dragFollowSpeed = 5f;

	public Vector3 entityHeight;

	public readonly List<Transform> selectedEntityTransforms = new List<Transform>();

	private float checkTime;

	private bool dragging;

	private bool selecting;

	private Vector3 boxSelectStartPosition;
	private Vector3 boxSelectEndPosition;

	private float dragStartTime;

	private readonly List<FinishingDrag> finishingDrags = new List<FinishingDrag>();

	private static readonly int BEING_DRAGGED = Animator.StringToHash("beingDragged");

	private struct FinishingDrag
	{
		public Transform transform;
		public Vector3   position;
	}

	private void Start ()
	{
		if (Instance == null) Instance = this;

		selectionBoxSprite = selectionBoxSpriteTransform.GetComponent<SpriteRenderer>();

		selectionBoxSpriteTransform.gameObject.SetActive(false);
	}

	private void Update ()
	{
		//if (MouseInputUIBlocker.BlockedByUI) return;

		if (!MouseInputUIBlocker.BlockedByUI && Input.GetMouseButtonDown(1)) BeginDrag();
		if (dragging                         && (Input.GetMouseButton(0)                                || Input.GetMouseButton(1))) UpdateDrag();
		if (dragging                         && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Time.time - dragStartTime > maxDragTime)) EndDrag();

		FinishDrags();

		if (!dragging)
		{
			if (!MouseInputUIBlocker.BlockedByUI && Input.GetMouseButtonDown(0)) BeginBoxSelect();
			if (selecting                        && Input.GetMouseButton(0)) UpdateBoxSelect();
			if (selecting                        && Input.GetMouseButtonUp(0)) EndBoxSelect();
		}
	}


	#region Selection Methods

	public void AddTransformToSelect (Transform t)
	{
		selectedEntityTransforms.Add(t);
	}

	private void SelectCurrent (Ray ray)
	{
		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit) ||
			!hit.transform.parent.CompareTag("Player")) return;

		if (selectedEntityTransforms.Contains(hit.transform.parent))
		{
			hit.transform.GetComponent<EntityGlowOnSelect>().SetSelected(false); //Remove Highlighting from Entities

			selectedEntityTransforms.Remove(hit.transform.parent);
		}
		else
		{
			hit.transform.parent.GetComponent<EntityGlowOnSelect>().SetSelected(true); //Add Highlighting to Entities

			selectedEntityTransforms.Add(hit.transform.parent);
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

		boxSelectEndPosition = MapManager.Instance.NearestPointInMapArea(hit.point);

		//Update the graphics for the selection box
		Vector3 centre = new Vector3((boxSelectStartPosition.x + boxSelectEndPosition.x) / 2,
									 0.1f,
									 (boxSelectStartPosition.z + boxSelectEndPosition.z) / 2
									);

		selectionBoxSpriteTransform.position = centre;
		selectionBoxSprite.size              = GetSelectionSize();
	}

	private void EndBoxSelect ()
	{
		selecting     = false;

		if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.RightShift) == false) ClearSelect();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		SelectCurrent(ray);

		LayerMask mask = LayerMask.GetMask("Ground");

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) boxSelectEndPosition = MapManager.Instance.NearestPointOnMap(hit.point);

		GameObject[] entityObjects = GameObject.FindGameObjectsWithTag("Player");

		Vector3 centre = new Vector3((boxSelectStartPosition.x + boxSelectEndPosition.x) / 2,
									 0,
									 (boxSelectStartPosition.z + boxSelectEndPosition.z) / 2
									);

		Vector3 size = GetSelectionSize();

		float minX = Mathf.Min(centre.x - size.x / 2, centre.x + size.x / 2);
		float maxX = Mathf.Max(centre.x - size.x / 2, centre.x + size.x / 2);
		float minZ = Mathf.Min(centre.z - size.y / 2, centre.z + size.y / 2);
		float maxZ = Mathf.Max(centre.z - size.y / 2, centre.z + size.y / 2);

		foreach (GameObject eo in entityObjects)
		{
			if (eo.name == "Model") continue;

			Vector3 pos = eo.transform.position;

			pos = pos - centre;

			pos = Quaternion.Euler(0, -45, 0) * pos;

			pos = pos + centre;

			if (minX < pos.x && pos.x < maxX &&
				minZ < pos.z && pos.z < maxZ)
			{
				eo.GetComponent<EntityGlowOnSelect>().SetSelected(true); //Add Highlighting to Entities

				selectedEntityTransforms.Add(eo.transform);
			}
		}

		//Turn off visuals for the selection
		selectionBoxSpriteTransform.gameObject.SetActive(false);
	}

	public void ClearSelect ()
	{
		foreach (Transform entity in selectedEntityTransforms)
		{
			if (entity != null) entity.GetComponent<EntityGlowOnSelect>().SetSelected(false); // Remove Highlighting from Entities
		}

		selectedEntityTransforms.Clear();
	}

	private Vector3 GetSelectionSize ()
	{
		float a   = Camera.main.transform.rotation.eulerAngles.y;
		float sin = Mathf.Sin(a / 180 * Mathf.PI);
		float cos = Mathf.Cos(a / 180 * Mathf.PI);
		float tan = Mathf.Tan(a / 180 * Mathf.PI);

		float x1 = (tan * tan * boxSelectStartPosition.x - tan * boxSelectStartPosition.z + boxSelectEndPosition.x + tan * boxSelectEndPosition.z) / (tan * tan + 1);

		float height = (boxSelectStartPosition.x - x1) / cos;
		float width  = (boxSelectEndPosition.x   - x1) / cos;

		Vector3 lossyScale = selectionBoxSpriteTransform.lossyScale;

		return new Vector2(width  / lossyScale.x,
						   height / lossyScale.y
						  );
	}

	#endregion


	#region Dragging Methods

	public void BeginDrag ()
	{
		dragging = true;
		dragStartTime = Time.time;
		
		for (int i = finishingDrags.Count - 1; i >= 0; i--)
		{
			if (selectedEntityTransforms.Contains(finishingDrags[i].transform)) finishingDrags.RemoveAt(i);
		}

		for (int i = selectedEntityTransforms.Count - 1; i >= 0; i--)
		{
			Transform entity = selectedEntityTransforms[i];

			if (entity == null)
			{
				selectedEntityTransforms.RemoveAt(i);
				continue;
			}

			if (entity.GetComponent<EntityManager>() != null)
			{
				entity.GetComponent<EntityManager>().enabled   = false;
				entity.GetComponent<StateManager>().enabled    = false;
				entity.GetComponent<DecisionManager>().enabled = false;
				entity.GetComponent<ActionManager>().enabled   = false;
				entity.GetComponent<NavMeshAgent>().enabled    = false;

				//entity.GetChild(0).GetComponent<Animator>().SetBool(BEING_DRAGGED, true);
			}

			FoodSpawner fs = entity.GetComponent<FoodSpawner>();
			if (fs != null) fs.SetPauseSpawning(true);

			entity.gameObject.layer = 2;
		}
	}

	private void UpdateDrag ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		LayerMask mask = ~(1 << 2); //Collides with all layers except layer 2

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		Vector3 currentMouseWorldPosition = hit.point;

		for (int i = 0; i < selectedEntityTransforms.Count; i++)
		{
			if (selectedEntityTransforms[i] == null)
			{
				selectedEntityTransforms.RemoveAt(i);
				continue;
			}

			selectedEntityTransforms[i].position = Vector3.Lerp(selectedEntityTransforms[i].position,
																currentMouseWorldPosition   + Vector3.up +
																DragDisplacementFunction(i) + entityHeight,
																Time.deltaTime * dragFollowSpeed / Time.timeScale);
		}
	}

	private void EndDrag ()
	{
		dragging = false;
		dragStartTime = 0;

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
				position  = MapManager.Instance.NearestPointOnMap(currentMouseWorldPosition + DragDisplacementFunction(i)) + entityHeight
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
				drag.transform.position = drag.position;

				if (drag.transform.GetComponent<EntityManager>() != null)
				{
					drag.transform.GetComponent<EntityManager>().enabled   = true;
					drag.transform.GetComponent<StateManager>().enabled    = true;
					drag.transform.GetComponent<DecisionManager>().enabled = true;
					drag.transform.GetComponent<ActionManager>().enabled   = true;
					drag.transform.GetComponent<NavMeshAgent>().enabled    = true;

					//drag.transform.GetChild(0).GetComponent<Animator>().SetBool(BEING_DRAGGED, false);
				}

				FoodSpawner fs = drag.transform.GetComponent<FoodSpawner>();
				if (fs != null) fs.SetPauseSpawning(false);

				drag.transform.gameObject.layer = 0;

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
		const float c           = 1f;

		Vector3 displacement = new Vector3(c * Mathf.Sqrt(i) * Mathf.Cos(i * radianAngle), 0,
										   c * Mathf.Sqrt(i) * Mathf.Sin(i * radianAngle));

		return displacement + entityHeight / 2;
	}

	#endregion


	#region External Tie In Methods

	public void BeginDragWithEntity (Transform entityTransform)
	{
		dragging = true;

		ClearSelect();

		selectedEntityTransforms.Add(entityTransform);

		BeginDrag();
	}

	#endregion


	private void OnDrawGizmos ()
	{
		if (dragging)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;

			LayerMask mask = ~(1 << 2); //Collides with all layers except layer 2

			if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

			Vector3 currentMouseWorldPosition = hit.point;

			for (int i = 0; i < selectedEntityTransforms.Count; i++)
			{
				Gizmos.DrawSphere(currentMouseWorldPosition   + Vector3.up +
								  DragDisplacementFunction(i) + entityHeight, 0.1f);
				Gizmos.DrawLine(currentMouseWorldPosition   + Vector3.up +
								DragDisplacementFunction(i) + entityHeight,
								currentMouseWorldPosition   +
								DragDisplacementFunction(i) + entityHeight);
			}
		}

		foreach (FinishingDrag drag in finishingDrags)
		{
			Gizmos.DrawSphere(drag.position, 0.1f);
		}
	}
}