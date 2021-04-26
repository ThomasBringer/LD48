using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [SerializeField] AnimationCurve fallCurve;
    Transform meteorite;
    float timerOffset;
    [SerializeField] GameObject dirtPrefab;
    bool reachedGround = false;

    void OnEnable()
    {
        meteorite = transform.GetChild(0);
        timerOffset = Time.time;
        SoundManager.sm.fallSource.Play();
    }

    void Update()
    {
        meteorite.localPosition = Vector3.up * fallCurve.Evaluate(Time.time - timerOffset);

        if (!reachedGround && meteorite.localPosition.y == 0)
        {
            reachedGround = true;
            meteorite.gameObject.layer = 7;
            GetComponentInChildren<Collider>().isTrigger = false;
            GetComponentInChildren<Light>().enabled = false;
            Instantiate(dirtPrefab, meteorite);
            SoundManager.sm.hitSource.Play();
        }
    }
}