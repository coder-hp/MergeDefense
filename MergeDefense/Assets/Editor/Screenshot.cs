using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Screenshot
{
    [MenuItem("HP/截图")]
    public static void TakeScreenshot()
    {
        // 获取电脑桌面路径
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        string screenshotFilename = desktopPath + $"\\Screenshot_{CommonUtil.getTimeStamp_Second()}.png";
        ScreenCapture.CaptureScreenshot(screenshotFilename);
        Debug.Log($"截图成功: {screenshotFilename}");
    }
}
