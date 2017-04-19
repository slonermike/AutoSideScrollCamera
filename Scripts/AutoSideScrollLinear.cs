// Linear version of the autoscroll path.  This will follow a path defined by the
// transforms stored in the path list.
//
// DEPENDENCIES
//
// SloneUtil: https://github.com/slonermike/SloneUtil
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSideScrollLinear : AutoSideScrollPath {

	[Tooltip("List of points along which to move.")]
	public List<Transform> path;

	// t-value (index + percentage toward next index)
	private float t = 0f;

	// Advance the current position along the path according to the speed
	// and time that has passed.
	//
	protected override void AdvancePath (float speed, float deltaTime) {
		Vector3 pos = transform.position;
		int index = Mathf.FloorToInt (t);

		float moveDist = speed * deltaTime;
		float moveDistSq = moveDist * moveDist;

		Transform p1, p2;

		p1 = path [index];

		if (index >= path.Count - 1) {
			t = (float)(path.Count - 1);
		} else {
			p2 = path [index + 1];

			while (pos.DistanceSquared(p2.position) < moveDistSq) {
				t = Mathf.Floor(t + 1.0f);

				if (t >= path.Count - 1) {
					t = (float)path.Count - 1;
					break;
				}

				moveDist -= Vector3.Distance (pos, p2.position);
				moveDistSq = moveDist * moveDist;
				index = Mathf.FloorToInt (t);
				p1 = path [index];
				p2 = path [index + 1];
			}

			if (moveDist > 0) {
				t += (moveDist / (Vector3.Distance (p1.position, p2.position)));
			}
		}

		transform.position = GetPosition (t);
	}

	// Get the scroll node info at the specified point index.
	//
	// pointIndex: Index of the point from which you're getting the scroll node info.
	//
	SideScrollNodeInfo GetInfoAtPoint(int pointIndex) {
		if (pointIndex < 0 || pointIndex >= path.Count) {
			return defaultNodeInfo;
		}
			
		AutoSideScrollNode node = path [pointIndex].GetComponent<AutoSideScrollNode> ();
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

		if (pointIndex >= path.Count - 1) {
			return p1;
		}

		SideScrollNodeInfo p2 = GetInfoAtPoint(pointIndex + 1);
		float pct = t - ((float)pointIndex);
		return SideScrollNodeInfo.GetInterpolatedInfo (p1, p2, pct);
	}

	// Get a position on the path at the specified tValue.
	//
	// tVal: the t-value from which to retrieve the point along the path.
	//
	Vector3 GetPosition(float tVal) {
		int index = Mathf.FloorToInt (tVal);
		if (index >= path.Count - 1) {
			return path [path.Count - 1].position;
		}

		return Vector3.Lerp (path [index].position, path [index + 1].position, tVal - Mathf.Floor (tVal));
	}

	// Get the current position along the curve.
	//
	protected override Vector3 GetPosition() {
		return GetPosition (t);
	}
}
