using System;
using System.Collections;
using UnityEngine;

static class Coroutines {
	public static IEnumerator LerpOverTime(float from, float to, float duration, Action<float> effect) {
		effect(from);
		for(var t = 0f; t < duration; t += Time.deltaTime) {
			effect(Mathf.Lerp(from, to, t / duration));
			yield return null;
		}
		effect(to);
	}

	public static IEnumerator LerpOverTime(Vector3 from, Vector3 to, float duration, Action<Vector3> effect) {
		effect(from);
		for(var t = 0f; t < duration; t += Time.deltaTime) {
			effect(Vector3.Lerp(from, to, t / duration));
			yield return null;
		}
		effect(to);
	}

	public static IEnumerator Delay(float delay) {
		for(var t = 0f; t < delay; t += Time.deltaTime) {
			yield return null;
		}
	}
	
	public static IEnumerator Frame() {
		yield return new WaitForEndOfFrame();
	}
}