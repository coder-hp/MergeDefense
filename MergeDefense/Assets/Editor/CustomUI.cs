using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomUI : MonoBehaviour
{
    [MenuItem("GameObject/UI/HPButton")]
    public static void onClickHPButton()
    {
        addUI("HPButton");
    }

    [MenuItem("GameObject/UI/HPText")]
    public static void onClickHPText()
    {
        addUI("HPText");
    }

    static void addUI(string name)
    {
        if (Selection.gameObjects.Length > 0)
        {
            // 指定预设的路径（Assets文件夹中的相对路径）
            string prefabPath = "Assets/CustomUI/" + name + ".prefab";

            // 加载预设
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"预设未找到: {prefabPath}");
                return;
            }

            // 在场景中实例化预设
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance != null)
            {
                instance.transform.SetParent(Selection.gameObjects[0].transform);
                instance.transform.localScale = Vector3.one;
                instance.transform.localPosition = Vector3.zero;

                // 选中实例（可选）
                Selection.activeObject = instance;
            }
            else
            {
                Debug.LogError("实例化预设失败");
            }
        }
    }
}
