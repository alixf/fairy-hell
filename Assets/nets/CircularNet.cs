using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;

public class CircularNet : MonoBehaviour
{
	public Image area;
	public float targetAngle = 360f;
	public new AudioClip[] audio;
	Vector3 origin;
	float alpha;

	void Awake() {
		origin = transform.localEulerAngles;
		alpha = area.color.a;
	}

	async void OnEnable() {
		transform.localEulerAngles = origin;
		area.fillAmount = 0.5f;
		var ta = targetAngle * Mathf.Sign(transform.localScale.x) + transform.localEulerAngles.z;
		area.color = new Color(area.color.r, area.color.g, area.color.b, 0f);
		await Coroutines.Frame();
		await Coroutines.LerpOverTime(0f, alpha, 1f, f => {
			if(area.gameObject) {
				area.color = new Color(area.color.r, area.color.g, area.color.b, f);
			}
		});
		StartCoroutine(Coroutines.LerpOverTime(0.5f, 0f, 1f / 2f, f => {
			if(area.gameObject) {
				area.fillAmount = f;
			}
		}));
		GetComponent<AudioSource>().clip = audio[Random.Range(0, audio.Length)];
		GetComponent<AudioSource>().Play();
		CameraShake.Shake(0.3f, 10f);
		await Coroutines.LerpOverTime(transform.localEulerAngles.z, ta, 1f, f => {
			if(transform.gameObject) {
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, f);
			}
		});
	}

	void OnDisable() {
		StopAllCoroutines();
	}
}
