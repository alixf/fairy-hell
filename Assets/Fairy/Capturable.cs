using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx.Async;

public class Capturable : MonoBehaviour
{
	async void OnTriggerEnter2D(Collider2D other)
	{
		transform.SetParent(other.transform, true);
		StartCoroutine(Coroutines.LerpOverTime(transform.localPosition, Vector2.zero, 0.2f, f => {
			if(transform.gameObject) {
				transform.localPosition = f;
			}
		}));
		GetComponent<AudioSource>().Play();
		await Coroutines.Delay(1f);
		GameManager.instance.GameOver();
	}
}
