using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
	public float speed;
	public Vector3? target = null;
	private new Camera camera;

	void Start() {
		camera = Camera.main;
	}

	void Update()
	{
		if(Input.GetMouseButton(0)) {
			target = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
		}
		if(Input.touchCount > 0) {
			var touch = Input.GetTouch(0);
			target = camera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -camera.transform.position.z));
		}
		if(target != null && target != transform.position) {
			transform.position = Vector3.MoveTowards(transform.position, target.Value, Time.deltaTime * speed);
		}
	}
}
