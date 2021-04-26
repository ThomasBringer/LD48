using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class Tools : MonoBehaviour
{
    static int screenshotCount = 0;

    [MenuItem("Image/Screenshot/Take Screenshot &x")]
    static void TakeScreenshot()
    {
        string fileName;
        do
        {
            screenshotCount++;
            fileName = "Picture" + screenshotCount + ".png";

        } while (File.Exists(fileName));

        ScreenCapture.CaptureScreenshot("Assets/Screenshots/Pictures/" + fileName);
        print("Screenshot: " + fileName);
    }
}