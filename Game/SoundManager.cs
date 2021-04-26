using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource beepSource;
    public AudioSource hitSource;
    public AudioSource fallSource;

    public static SoundManager sm;

    void Awake()
    {
        sm = this;
    }
}