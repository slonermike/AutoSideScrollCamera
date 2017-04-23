using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add this component to an object which you would like to move as though it were attached to the camera.
//
// Commonly needed in shmups.
//
public class AutoSideScrollFollower : MonoBehaviour {

	private static LinkedList<AutoSideScrollFollower> all = new LinkedList<AutoSideScrollFollower>();

	// Use this for initialization
	void Start () {
		all.AddFirst (this);
	}
	
	void OnDestroy() {
		all.Remove (this);
	}

	// Nudge position and rotation of each object following the camera
	//
	public static void MoveAll(Vector2 cameraMovement, float cameraRotation)
	{
		Vector3 moveAmt = new Vector3 (cameraMovement.x, cameraMovement.y, 0f);
		foreach (AutoSideScrollFollower follower in all) {
			follower.gameObject.transform.position += moveAmt;
			follower.gameObject.transform.Rotate (0f, 0f, cameraRotation);
		}
	}
}
