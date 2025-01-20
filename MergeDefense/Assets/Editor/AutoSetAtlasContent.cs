using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class AutoSetAtlasContent: EditorWindow
{
    [MenuItem("HP/合图集")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AutoSetAtlasContent));
    }

    List<string> list = new List<string>() {};

    private void OnEnable()
    {
        this.titleContent = new GUIContent("合图集");

        // 自动检索Assets/images下的文件夹
        {
            list.Clear();
            string path = Application.dataPath + "/images";
            string[] files = Directory.GetDirectories(path);
            for (int i = 0; i < files.Length; i++)
            {
#if UNITY_EDITOR_WIN
                string[] temp = files[i].Split('\\');
                list.Add(temp[temp.Length - 1]);
#elif UNITY_EDITOR_OSX
                string[] temp = files[i].Split('/');
                list.Add(temp[temp.Length - 1]);
#endif
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        for (int i = 0; i < list.Count; i++)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            // 图集名
            {
                GUI.skin.label.fontSize = 20;
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(list[i], GUILayout.Width(120));
            }

            // 图集对象
            {
                SpriteAtlas spriteAtlas = Resources.Load("Atlas/" + list[i], typeof(SpriteAtlas)) as SpriteAtlas;
                spriteAtlas = (SpriteAtlas)EditorGUILayout.ObjectField("", spriteAtlas, typeof(SpriteAtlas), false, GUILayout.Width(100));

                // 图集尺寸
                //{
                //    GUI.skin.label.fontSize = 20;
                //    GUI.skin.label.alignment = TextAnchor.MiddleLeft;

                //    // 需要Unity2021版本
                //    GUILayout.Label(spriteAtlas.GetPreferredSize(), GUILayout.Width(120));
                //}
            }

            // 合图集按钮
            {
                GUILayout.Space(50);
                if (GUILayout.Button("合成", GUILayout.Width(60)))
                {
                    SetAtlas(list[i]);
                    Debug.Log("图集" + list[i] + "生成完毕");
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        GUILayout.Space(50);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(50);

        if (GUILayout.Button("一键合图", GUILayout.Width(100)))
        {
            for (int i = 0; i < list.Count; i++)
            {
                SetAtlas(list[i]);
                Debug.Log("图集" + list[i] + "生成完毕");
            }
        }
        GUILayout.Space(50);
        if (GUILayout.Button("清除图集", GUILayout.Width(100)))
        {
            string _atlasPath2 = "Assets/Resources/Atlas";
            string[] files = Directory.GetFiles(_atlasPath2);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }

            // 必须调用一下刷新，否则资源面板里还是会显示图集文件
            AssetDatabase.Refresh();

            Debug.Log("图集删除完毕");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    void SetAtlas(string atlasName)
    {
        string _atlasPath = "Assets/Resources/Atlas/" + atlasName + ".spriteatlas";
        string _texturePath = "Assets/images/" + atlasName + "/";

        SpriteAtlas atlas = new SpriteAtlas();

        // 设置参数 可根据项目具体情况进行设置
        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
        {
            blockOffset = 2,
            enableRotation = true,
            enableTightPacking = false,
            padding = 8,
        };
        atlas.SetPackingSettings(packSetting);

        SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear,
        };
        atlas.SetTextureSettings(textureSetting);

        TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings()
        {
            maxTextureSize = 2048,
            //format = TextureImporterFormat.Automatic,
            //format = TextureImporterFormat.RGBA32,
            format = TextureImporterFormat.ASTC_4x4,
            crunchedCompression = true,
            textureCompression = TextureImporterCompression.Uncompressed,
            //textureCompression = TextureImporterCompression.Compressed,
            //compressionQuality = 100,
        };
        atlas.SetPlatformSettings(platformSetting);

        AssetDatabase.CreateAsset(atlas, _atlasPath);

        // 1、添加文件
        DirectoryInfo dir = new DirectoryInfo(_texturePath);
        // 这里我使用的是png图片，已经生成Sprite精灵了
        FileInfo[] files = dir.GetFiles("*.png");
        foreach (FileInfo file in files)
        {
            atlas.Add(new[] { AssetDatabase.LoadAssetAtPath<Sprite>($"{_texturePath}/{file.Name}") });
        }

        // 2、添加文件夹
        //Object obj = AssetDatabase.LoadAssetAtPath(_texturePath, typeof(Object));
        //atlas.Add(new[] { obj });

        AssetDatabase.SaveAssets();
    }
}
