using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroll : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0, -Time.time * speed));
    }
}
