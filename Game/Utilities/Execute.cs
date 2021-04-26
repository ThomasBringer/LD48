using UnityEngine.Events;
using UnityEngine;
public class Execute : MonoBehaviour { [SerializeField] UnityEvent methods; public void Invoke() { methods.Invoke(); } }