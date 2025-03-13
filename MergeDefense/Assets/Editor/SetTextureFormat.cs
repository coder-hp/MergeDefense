using System.IO;
using UnityEditor;
using UnityEngine;

public class SetTextureFormat : MonoBehaviour
{
    [MenuItem("Assets/转换图片格式")]
    public static void SetFormat()
    {
        // 获取选中的对象(文件夹或图片文件)
        // 可以同时选中多个文件夹，也可以点击文件夹内空白处获取当前所在文件夹
        Object[] selectedObjects = Selection.objects;

        foreach (Object selectedObject in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(selectedObject);

            // 如果选中的是文件夹
            if (Directory.Exists(path))
            {
                // 获取文件夹下所有资源的 GUID
                string[] guids = AssetDatabase.FindAssets("", new[] { path });

                // 遍历 GUID 并获取资源路径
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    setFormat(assetPath);
                }
            }
            // 如果选中的是文件
            else
            {
                setFormat(path);
            }
        }

        Debug.Log("图片格式转换完成！");
    }

    static void setFormat(string assetPath)
    {
        // 获取 TextureImporter
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (textureImporter != null && textureImporter.textureType != TextureImporterType.Sprite)
        {
            Debug.Log("setFormat:" + assetPath);

            // 设置图片格式
            textureImporter.textureType = TextureImporterType.Sprite;
            // 设置压缩格式
            // textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

            // 设置平台特定的格式
            //TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            //settings.format = TextureImporterFormat.RGBA32;                         // 设置格式为 RGBA32
            //settings.textureCompression = TextureImporterCompression.Compressed;    // 设置压缩
            //settings.compressionQuality = 50;                                       // 设置压缩质量
            //settings.maxTextureSize = 1024;                                         // 设置最大纹理尺寸

            //textureImporter.SetPlatformTextureSettings(settings);

            // 应用修改
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
    }
}
