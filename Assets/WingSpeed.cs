using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingSpeed : MonoBehaviour
{
    Vector3 lastPosition;
    float angle = 0f;
    float vertical = 0f;

    void Update()
    {
        var offset = lastPosition - transform.position;
        angle = Mathf.Abs(offset.x) > 5f ? Mathf.Sign(offset.x) * 30f : 0f;
        transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(transform.localEulerAngles.z, angle, Time.deltaTime * 120f));
        lastPosition = transform.position;
        var verticalTarget = offset.y > 5f ? -1f : offset.y < -5f ? 1f : 0f;
        vertical = Mathf.MoveTowards(vertical, verticalTarget, Time.deltaTime * 5f);
        GetComponent<Animator>().SetFloat("velocity", vertical);
    }
}
