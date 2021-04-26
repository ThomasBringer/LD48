using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPillar : MonoBehaviour
{
    Animator anim;
    Transform animTransform;
    Transform animChild;

    bool fallen = false;

    void OnEnable()
    {
        anim = GetComponentInChildren<Animator>();
        animTransform = anim.transform;
        animChild = animTransform.GetChild(0);
    }

    void OnTriggerEnter(Collider cldr)
    {
        if (!fallen)
        {
            if (!FindObjectOfType<Player>().controllable)
            {
                fallen = true;
                return;
            }

            fallen = true;
            Quaternion savedRot = animChild.rotation;

            //transform.Rotate(Vector3.up * (Vector3.Angle(transform.right, cldr.transform.position - transform.position)), Space.Self);

            transform.rotation = Quaternion.LookRotation(cldr.transform.position - transform.position, transform.up);

            //transform.forward = cldr.transform.position - transform.position;

            //transform.rotation = Quaternion.Euler(Vector3.Scale(Vector3.up, transform.localRotation.eulerAngles));
            //transform.up = transform.position;
            animChild.rotation = savedRot;

            anim.SetTrigger("Fall");
            SoundManager.sm.fallSource.Play();
        }
    }

    public void FallEnd()
    {
        animChild.gameObject.layer = 7;
        animChild.GetComponent<Collider>().isTrigger = false;
        transform.GetChild(1).gameObject.SetActive(false);
        SoundManager.sm.hitSource.Play();
    }
}