using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;

public class LinearNet : MonoBehaviour
{
	public Image area;
	public Vector2 move = Vector2.zero;
	public new AudioClip[] audio;
	Vector3 origin;
	float alpha;

	void Awake() {
		origin = transform.localPosition;
		alpha = area.color.a;
	}

	async void OnEnable() {
		transform.localPosition = origin;
		area.color = new Color(area.color.r, area.color.g, area.color.b, 0f);
		await Coroutines.Frame();
		await Coroutines.LerpOverTime(0f, alpha, 1f, f => {
			if(area.gameObject) {
				area.color = new Color(area.color.r, area.color.g, area.color.b, f);
			}
		});
		GetComponent<AudioSource>().clip = audio[Random.Range(0, audio.Length)];
		GetComponent<AudioSource>().Play();
		CameraShake.Shake(0.3f, 10f);
		await Coroutines.LerpOverTime(transform.localPosition, transform.localPosition + transform.rotation * new Vector3(move.x, move.y, 0f) * Mathf.Sign(transform.localScale.x), 1f, f => {
			if(transform.gameObject) {
				transform.localPosition = f;
			}
		});
	}

	void OnDisable() {
		StopAllCoroutines();
	}
}
