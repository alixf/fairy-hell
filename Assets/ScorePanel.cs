using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;


public class ScorePanel : MonoBehaviour
{
    public GameObject loading;
    public GameObject scorePrefab;
    public Transform scoreList;

    async void OnEnable()
    {
        foreach (Transform child in scoreList) {
            GameObject.Destroy(child.gameObject);
        }
        loading.SetActive(true);
        var scores = await GameManager.instance.GetScores();
        loading.SetActive(false);
        foreach(var score in scores) {
            var scoreObj = Instantiate(scorePrefab);
            scoreObj.GetComponent<Text>().text = score.name + " : " + FormatScore(score.score);
            scoreObj.transform.SetParent(scoreList);
            scoreObj.transform.localScale = Vector3.one;
            scoreObj.transform.localPosition = Vector3.zero;
        }
    }

    string FormatScore(int score) {
        return Mathf.Floor(score / 60).ToString().PadLeft(2, '0') + "'" + (score % 60).ToString().PadLeft(2, '0') + "\"";
    }
}
