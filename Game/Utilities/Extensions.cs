using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ExtensionMethods
{
    public static T[] MultiplyEach<T>(this T[] items, int[] counts)
    {
        var multipliedItems = new List<T>();
        for (int i = 0; i < items.Length; i++)
        {
            for (int j = 0; j < counts[i]; j++)
            {
                multipliedItems.Add(items[i]);
            }
        }
        return multipliedItems.ToArray();
    }

    public static void Scalate(this GameObject go, float scale) { go.transform.localScale = Vector3.one * scale; }

    public static int Sum(this int[] values)
    {
        int s = 0;
        foreach (int i in values) s += i;
        return s;
    }

    public static GameObject[] GetAllChildren(this GameObject go)
    {
        var children = new List<GameObject>();
        Transform[] transforms = go.GetComponentsInChildren<Transform>();
        foreach (var trans in transforms) children.Add(trans.gameObject);
        return children.ToArray();
    }

    public static T RandomItem<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static bool Approximate(this float a, float b)
    {
        return Mathf.Abs(a - b) <= Utilities.ApproximateComparisonNumber;
    }

    public static void SetLayer(this GameObject go, string layer)
    {
        go.layer = LayerMask.NameToLayer(layer);
    }

    public static void ApplyDrag(this Rigidbody2D rb, Vector2 axis, float drag = 1)
    {
        float amount = 1 - Time.fixedDeltaTime * drag;
        rb.velocity = Vector2.Scale(rb.velocity, new Vector2(axis.x == 0 ? 1 : amount, axis.y == 0 ? 1 : amount));
    }

    public static void ApplyHorizontalDrag(this Rigidbody2D rb, float drag = 1)
    {
        rb.ApplyDrag(Vector2.right, drag);
    }

    public static bool Probability(this float prob)
    {
        return Random.Range(0f, 1f) <= prob;
    }
}

public static class Utilities
{
    public static int NegPos { get { return UnityEngine.Random.Range(0, 2) * 2 - 1; } }

    public static GameObject[] GetAllGameObjects() { return GameObject.FindObjectsOfType<GameObject>(); }

    public static void ReloadScene() { UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); }

    public static void LoadNextScene() { UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1); }

    public static void LoadPreviousScene() { UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1); }

    public static float ApproximateComparisonNumber = .001f;
}