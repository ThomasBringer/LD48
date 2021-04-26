using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] float delay = 2;

    void OnEnable() { Destroy(gameObject, delay); }
}