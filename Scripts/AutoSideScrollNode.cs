using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SideScrollNodeInfo {

	[Tooltip("Camera FOV when the camera reaches this point along the path.")]
	public float cameraFov = 30f;

	[Tooltip("Camera movement speed at this point along the path.")]
	public float pathSpeed = 1.0f;

	// Create node values interpolated between p1 and p2.
	//
	// p1: The previous node.
	// p2: The next node.
	// pct: The percentage [0.0,1.0] along the path between the two.
	//
	public static SideScrollNodeInfo GetInterpolatedInfo(SideScrollNodeInfo p1, SideScrollNodeInfo p2, float pct)
	{
		pct = Mathf.Clamp01 (pct);
		SideScrollNodeInfo info = new SideScrollNodeInfo ();
		info.cameraFov = Mathf.Lerp (p1.cameraFov, p2.cameraFov, pct);
		info.pathSpeed = Mathf.Lerp (p1.pathSpeed, p2.pathSpeed, pct);

		return info;
	}
}

// Component that can be added to a point along a path.
//
public class AutoSideScrollNode : MonoBehaviour {
	public SideScrollNodeInfo info;
}
