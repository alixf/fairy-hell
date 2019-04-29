using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using System.Threading.Tasks;
#if !UNITY_WEBGL
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
#endif
using System;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Canvas ui;
    public CanvasGroup logo;
    public CanvasGroup play;
    public CanvasGroup scores;
    public CanvasGroup quit;
    public CanvasGroup gameover;

    public GameObject[] levels;
    public GameObject fairy;
    public GameObject fairyPrefab;
    public GameObject scoreBoard;
    public Text scoreDisplay;
    public float currentScore;

    public GameObject textInput;

    public string playerName = "Anonymous";

    public static GameManager instance;

    Boolean playing = false;

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            #if !UNITY_WEBGL
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://fairy-hell.firebaseio.com/");
            #endif
        }
    }

    async void Start()
    {
        #if UNITY_WEBGL
        scores.gameObject.SetActive(false);
        #endif
        textInput.SetActive(false);
        logo.alpha = 0f;
        play.alpha = 0f;
        scores.alpha = 0f;
        quit.alpha = 0f;
        gameover.alpha = 0f;
        fairy.SetActive(false);
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.5f, f => logo.alpha = f));
        await Coroutines.Delay(1f);
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.33f, f => play.alpha = f));
        await Coroutines.Delay(0.2f);
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.33f, f => scores.alpha = f));
        await Coroutines.Delay(0.2f);
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.33f, f => quit.alpha = f));
        textInput.SetActive(true);
    }

    public void SetPlayerName(string text) {
        this.playerName = text;
    }

    Coroutine scoreCoroutine;
    public async void Play()
    {
        playing = true;
        textInput.SetActive(false);
        currentScore = 0;
        scoreCoroutine = StartCoroutine(ScoreUpdate());
        play.alpha = 0f;
        await Coroutines.Delay(0.05f);
        play.alpha = 1f;
        await Coroutines.Delay(0.05f);
        play.alpha = 0f;
        await Coroutines.Delay(0.05f);
        play.alpha = 1f;
        await Coroutines.Delay(0.2f);
        StartCoroutine(Coroutines.LerpOverTime(1f, 0f, 0.2f, f => logo.transform.localScale = Vector3.one * f));
        StartCoroutine(Coroutines.LerpOverTime(1f, 0f, 0.2f, f => gameover.transform.localScale = Vector3.one * f));
        await Coroutines.Delay(0.05f);
        StartCoroutine(Coroutines.LerpOverTime(1f, 0f, 0.2f, f => play.transform.localScale = Vector3.one * f));
        await Coroutines.Delay(0.05f);
        #if  !UNITY_WEBGL
        StartCoroutine(Coroutines.LerpOverTime(1f, 0f, 0.2f, f => scores.transform.localScale = Vector3.one * f));
        await Coroutines.Delay(0.05f);
        #endif
        StartCoroutine(Coroutines.LerpOverTime(1f, 0f, 0.2f, f => quit.transform.localScale = Vector3.one * f));
        await Coroutines.Delay(1f);

        // ui.gameObject.SetActive(false);
        levels[0].SetActive(true);
        Destroy(fairy);
        fairy = Instantiate(fairyPrefab);
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator ScoreUpdate() {
        var start = Time.time;
        while(true) {
            currentScore = Time.time - start;
            scoreDisplay.text = FormatScore(currentScore);
            yield return null;
        }
    }

    string FormatScore(float score) {
        return Mathf.FloorToInt(score / 60).ToString().PadLeft(2, '0') + "'" + Mathf.FloorToInt(score % 60).ToString().PadLeft(2, '0') + "\"";
    }

    public async void GameOver()
    {
        if(!playing) {
            return;
        }
        playing = false;
        StopCoroutine(scoreCoroutine);
        SaveScore(playerName, Mathf.FloorToInt(currentScore));
        currentScore = 0f;
        fairy.SetActive(false);
        GetComponent<AudioSource>().Play();
        levels[0].SetActive(false);
        // ui.gameObject.SetActive(true);
        logo.alpha = 0f;
        gameover.transform.localScale = Vector3.one;
        play.transform.localScale = Vector3.one;
        scores.transform.localScale = Vector3.one;
        quit.transform.localScale = Vector3.one;
        play.alpha = 0f;
        scores.alpha = 0f;
        quit.alpha = 0f;
        gameover.alpha = 0f;
        fairy.SetActive(false);
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.5f, f => gameover.alpha = f));
        await Coroutines.Delay(1f);
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.33f, f => play.alpha = f));
        await Coroutines.Delay(0.2f);
        #if !UNITY_WEBGL
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.33f, f => scores.alpha = f));
        await Coroutines.Delay(0.2f);
        #endif
        StartCoroutine(Coroutines.LerpOverTime(0f, 1f, 0.33f, f => quit.alpha = f));
        textInput.SetActive(true);
    }

    public void ShowScoreboard()
    {
        scoreBoard.SetActive(true);
    }

    public void CloseScoreboard()
    {
        scoreBoard.SetActive(false);
    }

    public Task SaveScore(string name, int score)
    {   
        #if !UNITY_WEBGL
        try {
            DatabaseReference database = FirebaseDatabase.DefaultInstance.RootReference;
            string key = database.Child("scores").Push().Key;
            Dictionary<string, string> scoreDict = new Dictionary<string, string>();
            scoreDict["name"] = name;
            scoreDict["score"] = score.ToString();
            Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
            childUpdates["/scores/" + key] = new Score(name, score).ToDictionary();
            return database.UpdateChildrenAsync(childUpdates);
        } catch(Exception error) {
            Debug.LogError(error);
            return Task.CompletedTask;
        }
        #else
        return Task.CompletedTask;
        #endif
    }

    public async Task<List<Score>> GetScores()
    {
        #if !UNITY_WEBGL
        try {
            DatabaseReference database = FirebaseDatabase.DefaultInstance.RootReference;
            var data = await database.Child("scores").OrderByChild("score").LimitToLast(10).GetValueAsync();
            var  t= data.Value as Dictionary<string, System.Object>;
            var result = new List<Score>();
            foreach(var t1 in t) {
                var t11 = (t1.Value as Dictionary<string, System.Object>);
                result.Add(new Score(
                    t11["name"] as string,
                    Convert.ToInt32(t11["score"])
                ));
            }
            return result.OrderBy(a => -a.score).Take(10).ToList();
        } catch(Exception error) {
            Debug.LogError(error);
            return new List<Score>();
        }
        #else
        return new List<Score>();
        #endif
    }
}

public class Score
{
    public string name;
    public int score;
    public Score(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

	public Dictionary<string, System.Object> ToDictionary() {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
        result["name"] = name;
        result["score"] = score;

        return result;
    }
}