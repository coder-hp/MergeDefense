using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class BuildTool : EditorWindow
{
    [MenuItem("HP/打包工具")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BuildTool));
    }

    string bundleVersion = "";
    string bundleVersionCode = "";
    string productName = "";
    string applicationIdentifier = "";
    string dataPath = "";

    string tip = "";
    long startTime;

    private void OnEnable()
    {
        this.titleContent = new GUIContent("打包工具");

        bundleVersion = PlayerSettings.bundleVersion;
        bundleVersionCode = PlayerSettings.Android.bundleVersionCode.ToString();
        productName = PlayerSettings.productName.Replace(" ","");
        applicationIdentifier = PlayerSettings.applicationIdentifier;
        dataPath = Application.dataPath;
    }
    
    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(20);
        bundleVersion = EditorGUILayout.TextField("bundleVersion", bundleVersion, GUILayout.Width(260));

        GUILayout.Space(10);
        bundleVersionCode = EditorGUILayout.TextField("bundleVersionCode", bundleVersionCode, GUILayout.Width(260));

        GUILayout.Space(20);
        tip = GUILayout.TextArea(tip);

        GUILayout.Space(20);

        GUILayout.EndVertical();

        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("打测试包", GUILayout.Width(100)))
            {
                startBuild(true);
            }

            GUILayout.Space(50);

            if (GUILayout.Button("安装测试包", GUILayout.Width(100)))
            {
                Task task = new Task(() =>
                {
                    installApk(true);
                });
                task.Start();
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("打正式包", GUILayout.Width(100)))
            {
                startBuild(false);
            }

            GUILayout.Space(50);

            if (GUILayout.Button("安装正式包", GUILayout.Width(100)))
            {
                Task task = new Task(() =>
                {
                    installApk(false);
                });
                task.Start();
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("卸载", GUILayout.Width(100)))
            {
                Task task = new Task(() =>
                {
                    uninstallApk();
                });
                task.Start();
            }

            GUILayout.Space(50);

            if (GUILayout.Button("打开目录", GUILayout.Width(100)))
            {
                OpenFolder();
            }

            GUILayout.EndHorizontal();
        }
    }

    void setIsDebug(bool isDebug)
    {
        GameObject.Find("Canvas_Top/GMLayer").GetComponent<GMLayer>().isDebug = isDebug;
    }

    void startBuild(bool isDebug)
    {
        try
        {
            setIsDebug(isDebug);

            PlayerSettings.bundleVersion = bundleVersion;
            PlayerSettings.Android.bundleVersionCode = int.Parse(bundleVersionCode);

            tip = "正在打包...";
            string path = dataPath;
            path = path.Replace("Assets", "output/" + productName + "-" + bundleVersion + (isDebug ? "-Debug" : "-Release") +".apk");
            startTime = CommonUtil.getTimeStamp_Second();
            BuildPlayerOptions opt = new BuildPlayerOptions();
            opt.scenes = new string[] { "Assets/Scenes/MainScene.unity" };
            opt.locationPathName = path;
            opt.target = BuildTarget.Android;
            opt.options = BuildOptions.CompressWithLz4;
            BuildPipeline.BuildPlayer(opt);

            Debug.Log("打包成功");
            Debug.Log("文件路径：" + path);

            long endTime = CommonUtil.getTimeStamp_Second();
            string time = ((float)(endTime - startTime) / 60f).ToString("f1");
            tip = "打包完成，耗时" + time + "分";
        }
        catch (Exception ex)
        {
            tip = "打包失败:" + ex.Message;
        }
    }

    void installApk(bool isDebug)
    {
        try
        {
            tip = "正在安装...";

            System.Collections.Generic.List<string> commands = new System.Collections.Generic.List<string>();

            // 退出App
            commands.Add("adb shell am force-stop " + applicationIdentifier);

            // 安装,命令不要有中文，否则会乱码
            string apkPath = dataPath;
            apkPath = apkPath.Replace("Assets", "output/" + productName + "-" + bundleVersion + (isDebug ? "-Debug" : "-Release") + ".apk");
            if (!File.Exists(apkPath))
            {
                tip = "apk不存在:" + apkPath;
                return;
            }
            commands.Add("adb install -r " + apkPath);

            // 启动
            commands.Add("adb shell am start " + applicationIdentifier + "/com.unity3d.player.UnityPlayerActivity");

            doBat(commands);

            tip = "安装完毕";
        }
        catch (Exception ex)
        {
            tip = "bat执行失败:" + ex.Message;
        }
    }

    void uninstallApk()
    {
        tip = "正在卸载...";

        System.Collections.Generic.List<string> commands = new System.Collections.Generic.List<string>();

        // 卸载
        commands.Add("adb uninstall " + applicationIdentifier);

        doBat(commands);

        tip = "卸载完毕";
    }

    void doBat(System.Collections.Generic.List<string> commands)
    {
        // 桌面路径
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        // bat路径
        string path = desktopPath + "/BuildTool.bat";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        string cmd = "";
        for(int i = 0; i < commands.Count; i++)
        {
            cmd += commands[i];
            cmd += "\n";
        }
        WriteFileByLine(path , cmd);

        System.Diagnostics.Process pro = new System.Diagnostics.Process();
        FileInfo file = new FileInfo(path);
        pro.StartInfo.WorkingDirectory = file.Directory.FullName;
        pro.StartInfo.FileName = path;
        pro.StartInfo.CreateNoWindow = false;
        pro.Start();
        pro.WaitForExit();
    }

    void WriteFileByLine(string file_path, string str_info)
    {
        using (StreamWriter sw = new StreamWriter(file_path, false))
        {
            sw.WriteLine(str_info); sw.Close();
            sw.Dispose();
        }
    }

    void OpenFolder()
    {
        string folderPath = dataPath;
        folderPath = folderPath.Replace("Assets", "output/");

        // windows不支持"/",所以必须全换成"\"
        folderPath = folderPath.Replace("/", "\\");

        if (System.IO.Directory.Exists(folderPath))
        {
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }
        else
        {
            Debug.LogError("文件夹不存在:" + folderPath);
        }
    }
}
