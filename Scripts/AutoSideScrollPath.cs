// Used to define a path that an object can travel along at specified world speed.
// Abstract class.  Child class must override AdvancePath and GetPosition according
// to the type of path.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AutoSideScrollPath : MonoBehaviour {

	[Tooltip("Speed multiplier for the skybox speed relative to camera speed.")]
	public float skyboxSpeed = 0.0f;

	[Tooltip("True to automatically add 2D collision at screen edges.")]
	public bool playerCollideEdges = false;

	[Tooltip("Values to use for nodes not specifying other values.")]
	public SideScrollNodeInfo defaultNodeInfo;

	[Tooltip("Top speed at which the FOV can change.")]
	public float fovChangeSpeed = 3.0f;

	private Skybox skybox = null;
	private float skyboxRotation = 0f;
	private BoxCollider2D[] edgeCollision; // Starting from top, clockwise, ending on left.

	protected abstract void AdvancePath (float speed, float deltaTime);
	protected abstract Vector3 GetPosition();
	protected abstract SideScrollNodeInfo GetCurrentScrollInfo ();

	protected virtual void Start() {

		skybox = Camera.main.gameObject.GetComponent<Skybox> ();

		if (skybox != null) {
			skyboxRotation = skybox.material.GetFloat ("_Rotation");
		}

		if (playerCollideEdges) {
			edgeCollision = new BoxCollider2D[4];
			for (int i = 0; i < 4; i++) {
				GameObject o = new GameObject ();
				edgeCollision[i] = o.AddComponent<BoxCollider2D> ();
				o.transform.SetParent (transform);
				o.name = "Camera Collision " + i;
			}
			UpdateEdgeCollision ();
		}
	}

	// Reposition and resize the collision so it is exactly at the edges of the screen.
	//
	void UpdateEdgeCollision()
	{
		if (!playerCollideEdges) {
			return;
		}

		const float EDGE_FATNESS = 0.1f;

		// TODO: Calculate this based on a focal object, such as the player.
		float zDist = Mathf.Abs(transform.position.z);

		Vector3 topLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0f, 1f, zDist));
		Vector3 topRight = Camera.main.ViewportToWorldPoint (new Vector3 (1f, 1f, zDist));
		Vector3 bottomLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0f, 0f, zDist));

		for (int i = 0; i < 4; i++) {
			edgeCollision [i].gameObject.transform.position = transform.position;
		}

		float screenWidth = Vector3.Distance (topLeft, topRight);
		float screenHeight = Vector3.Distance (topLeft, bottomLeft);

		Vector2 horizSize = new Vector2 (screenWidth, EDGE_FATNESS);
		Vector2 vertSize = new Vector2 (EDGE_FATNESS, screenHeight);
		edgeCollision [0].size = horizSize;
		edgeCollision [1].size = vertSize;
		edgeCollision [2].size = horizSize;
		edgeCollision [3].size = vertSize;

		edgeCollision [0].offset = new Vector2 (0f, screenHeight * 0.5f);
		edgeCollision [1].offset = new Vector2 (screenWidth * 0.5f, 0f);
		edgeCollision [2].offset = new Vector2 (0f, screenHeight * -0.5f);
		edgeCollision [3].offset = new Vector2 (screenWidth * -0.5f, 0f);
	}

	// Move the skybox relative to the amount that the camera has moved.
	//
	// xChange: Distance the camera has moved along the x axis this frame.
	//
	void MoveSkybox (float xChange) {
		if (skybox == null || skyboxSpeed == 0f) {
			return;
		}

		skyboxRotation += xChange * skyboxSpeed;

		// Keep it in the range of 0 to 360.
		if (skyboxRotation >= 360f) {
			skyboxRotation -= 360f;
		} else if (skyboxRotation < 0f) {
			skyboxRotation += 360f;
		}

		skybox.material.SetFloat ("_Rotation", skyboxRotation);
	}

	// Advance the field of view toward the specified goal.
	//
	// goalFov: the desired final FOV.
	//
	void UpdateFOV(float goalFov)
	{
		Camera.main.fieldOfView = SloneUtil.AdvanceValue (Camera.main.fieldOfView, goalFov, fovChangeSpeed);
	}

	protected virtual void Update() {
		float oldX = transform.position.x;
		SideScrollNodeInfo info = GetCurrentScrollInfo ();

		AdvancePath (info.pathSpeed, Time.deltaTime);
		MoveSkybox( transform.position.x - oldX );

		// Values will have changed after advancing the path, so update.
		info = GetCurrentScrollInfo ();
		UpdateFOV (info.cameraFov);
		UpdateEdgeCollision ();
	}
}
