// Bezier version of the autoscroll path.  This will follow a BezierCurve defined by the
// script available at https://www.assetstore.unity3d.com/en/#!/content/11278 on the Asset
// Store from "Arkham Interactive."
//
// Note: I directly call the static function BezierCurve.GetPoint, as it is more efficient than
// calling the member function BezierCurve.GetPointAt, which calls ApproximateLength several times
// with each call.
//
//
// DEPENDENCIES
//
// SloneUtil: https://github.com/slonermike/SloneUtil
// BezierCurve: https://www.assetstore.unity3d.com/en/#!/content/11278
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSideScrollBezier : AutoSideScrollPath {

	[Tooltip("Bezier curve path along which the camera will move.")]
	public BezierCurve curve;

	private float[] curveLengths;
	private float t = 0f;

	protected override void Start()
	{
		base.Start ();

		BezierPoint[] points = curve.GetAnchorPoints ();
		curveLengths = new float[curve.pointCount-1];
		for (int i = 0; i < curve.pointCount-1; i++) {
			BezierPoint p1 = points [i];
			BezierPoint p2 = points [i + 1];

			// As you increase resolution, the length approaches but is always less than the true
			// length of the curve.
			curveLengths [i] = BezierCurve.ApproximateLength (p1, p2, resolution: 20);
		}
	}

	// Advance the current position along the path according to the speed
	// and time that has passed.
	//
	protected override void AdvancePath (float speed, float deltaTime) {
		Vector3 pos = transform.position;

		float desiredDistance = speed * deltaTime;
		desiredDistance *= desiredDistance;

		while (pos.DistanceSquared (transform.position) < desiredDistance) {
			int pointIndex = Mathf.FloorToInt (t);
			if (pointIndex >= curve.pointCount-1) {
				break;
			}

			float stepSize = desiredDistance / curveLengths [pointIndex];
			t += stepSize;

			pos = GetPosition (t);
		}

		t = Mathf.Clamp (t, 0.0f, (float)(curve.pointCount-1));
		transform.position = SloneUtil.AdvanceValue (transform.position, pos, speed);
	}

	// Get the position at the specified t value.
	//
	// tVal: Value of the progression along the spline [0.0,nPoints]
	//
	Vector3 GetPosition(float tVal) {
		int pointIndex = Mathf.FloorToInt (tVal);
		float pct = tVal - ((float)pointIndex);

		BezierPoint[] points = curve.GetAnchorPoints ();

		if (points.Length == 1 || pointIndex >= points.Length-1) {
			return points [points.Length - 1].position;
		} else {
			return BezierCurve.GetPoint (points [pointIndex], points [pointIndex + 1], pct);
		}
	}

	// Get the z-axis angle of the path at the current position.
	//
	protected override float GetPathAngle ()
	{
		int pointIndex = Mathf.FloorToInt (t);
		float pct = t - ((float)pointIndex);
		BezierPoint[] points = curve.GetAnchorPoints ();

		if (pct == 0f) {
			return points [pointIndex].transform.eulerAngles.z;
		} else {
			Vector3 p1 = GetPosition (t);
			Vector3 p2 = GetPosition (Mathf.Clamp(t+0.01f, 0.0f, (float)(curve.pointCount-1)));
			float angle = Mathf.Atan2 (p2.y - p1.y, p2.x - p1.x);
			return Mathf.Rad2Deg * angle;
		}
	}

	// Get the scroll node info at the specified point index.
	//
	// pointIndex: Index of the point from which you're getting the scroll node info.
	//
	SideScrollNodeInfo GetInfoAtPoint(int pointIndex) {
		if (pointIndex < 0 || pointIndex >= curve.pointCount) {
			return defaultNodeInfo;
		}

		BezierPoint[] points = curve.GetAnchorPoints ();
		AutoSideScrollNode node = points [pointIndex].GetComponent<AutoSideScrollNode> ();
		if (node == null) {
			return defaultNodeInfo;
		} else {
			return node.info;
		}
	}

	// Get the scrolling info at the current point along the curve.
	//
	protected override SideScrollNodeInfo GetCurrentScrollInfo() {
		int pointIndex = Mathf.FloorToInt (t);

		SideScrollNodeInfo p1 = GetInfoAtPoint (pointIndex);

		if (pointIndex >= curve.pointCount - 1) {
			return p1;
		}

		SideScrollNodeInfo p2 = GetInfoAtPoint(pointIndex + 1);
		float pct = t - ((float)pointIndex);
		return SideScrollNodeInfo.GetInterpolatedInfo (p1, p2, pct);
	}

	// Get the current position along the curve.
	//
	protected override Vector3 GetPosition() {
		return GetPosition (t);
	}
}
