using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGUIJoystick : MonoBehaviour {
	public float radius = 2.0f;
	public Vector3 scale = Vector3.one;
	private Plane mPlane;
	private Vector3 mLastPos;
	private UIPanel mPanel;
	private Vector3 center;
	[HideInInspector]
	public Vector2 position;
	
	public List<PlayerLook> lookScripts;
	
	private void Start () {
		center = transform.localPosition;
	}
	
	private void OnPress (bool pressed) {
		if (enabled && gameObject.activeInHierarchy) {
			if (pressed) {
				mLastPos = UICamera.lastHit.point;
				mPlane = new Plane (Vector3.back, mLastPos);
				
				foreach(PlayerLook lookScript in lookScripts) {
					lookScript.touchIndex = UICamera.currentTouchID;
				}
			} else {
				transform.localPosition = center;
				position=Vector2.zero;
				
				foreach(PlayerLook lookScript in lookScripts) {
					lookScript.touchIndex = -1;
				}
			}
		}
	}

	void OnDrag (Vector2 delta) {
		if (enabled && gameObject.activeInHierarchy) {
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			
			Ray ray = UICamera.currentCamera.ScreenPointToRay (UICamera.currentTouch.pos);
			float dist = 0f;
			
			if (mPlane.Raycast (ray, out dist)) {
				Vector3 currentPos = ray.GetPoint (dist);
				Vector3 offset = currentPos - mLastPos;
				mLastPos = currentPos;

				if (offset.x != 0f || offset.y != 0f) {
					offset = transform.InverseTransformDirection (offset);
					offset.Scale (scale);
					offset = transform.TransformDirection (offset);
				}
				
				offset.z = 0;
				transform.position += offset;
				
				float length = transform.localPosition.magnitude;
				 
				if (length > radius) {
					transform.localPosition = Vector3.ClampMagnitude (transform.localPosition, radius);
				}

				position = new Vector2((transform.localPosition.x-center.x)/radius,(transform.localPosition.y-center.y)/radius);
			}
		}
	}
}
