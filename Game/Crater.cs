using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Crater : MonoBehaviour
{
    [SerializeField] float minDelay = 2;
    [SerializeField] float maxDelay = 8;

    float delay;

    VisualEffect fire;
    Collider cldr;

    void OnEnable()
    {
        delay = Random.Range(minDelay, maxDelay);
        fire = GetComponentInChildren<VisualEffect>();
        cldr = GetComponent<Collider>();

        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        bool activate = false;
        while (true)
        {
            yield return StartCoroutine(Activate(activate));
            activate = !activate;
        }
    }

    IEnumerator Activate(bool activate = true)
    {
        cldr.enabled = activate;
        if (activate) fire.Play();
        else fire.Stop();
        yield return new WaitForSeconds(delay);
    }
}