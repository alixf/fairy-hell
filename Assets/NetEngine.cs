using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NetEngine : MonoBehaviour
{
    public GameObject[] allTheNets;
    Dictionary<string, GameObject> nets;

    void Start()
    {
        nets = new Dictionary<string, GameObject>();
        foreach (var net in allTheNets) {
            if(nets.ContainsKey(net.gameObject.name)) {
                Debug.LogWarning("wtf : "+net.gameObject.name);
            } else {
                nets.Add(net.gameObject.name, net.gameObject);
            }
        }
        Debug.Log(nets.Count);
    }

    void OnEnable() {
        foreach (var net in allTheNets) {
            net.SetActive(false);
        }
        run = StartCoroutine(Run());
    }

    Coroutine run;

    private float startTime = 0f;

    IEnumerator Run() {
        yield return new WaitForSeconds(3f);
        startTime = Time.time;
        while(true) {
            var cooldown = cooldownOverTime.Evaluate(Time.time - startTime);
            var netCount = Mathf.FloorToInt(netCountOverTime.Evaluate(Time.time - startTime));
            Debug.Log(cooldown + " -> " + netCount);
            var activeNets = GetNElementsRandom(netCount);
            foreach (var net in activeNets) {
                StartCoroutine(Spawn(nets[net]));
            }
            yield return new WaitForSecondsRealtime(cooldown);
        }
    }

    IEnumerator Spawn(GameObject net) {
        if(!net.activeSelf) {
            net.SetActive(true);
            yield return new WaitForSecondsRealtime(2f);
            net.SetActive(false);
        }
    }

    IEnumerable<string> GetNElementsRandom(int n) {
        return nets.Keys.ToArray().OrderBy(x => Random.value).Take(n);
    }

    void OnDisable() {
        if(run != null) {
            StopCoroutine(run);
        }
    }

    public AnimationCurve netCountOverTime;
    public AnimationCurve cooldownOverTime;
}
