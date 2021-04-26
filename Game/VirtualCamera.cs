using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCamera : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    CinemachineTransposer transposer;

    Level level;

    [SerializeField] float radiusMultiplier = 2;
    [SerializeField] float minRadius = 5;

    void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        level = FindObjectOfType<Level>();
    }

    void Start()
    {
        AdaptOffset();
    }

    public void AdaptOffset()
    {
        transposer.m_FollowOffset = Vector3.back * (Mathf.Clamp(level.radius * radiusMultiplier, minRadius, float.MaxValue));
    }
}