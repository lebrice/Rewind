using UnityEngine;

namespace utils
{
    [CreateAssetMenu(menuName = "SharedVariables/float")]
    public class FloatVariable : SharedVariable<float> { }

    //public static class ScriptableObjectUtility
    //{
    //    /// <summary>
    //    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    //    /// </summary>
    //    public static void CreateAsset<T>() where T : ScriptableObject
    //    {
    //        T asset = ScriptableObject.CreateInstance<T>();

    //        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //        if (path == "")
    //        {
    //            path = "Assets";
    //        }
    //        else if (Path.GetExtension(path) != "")
    //        {
    //            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    //        }
    //        var bob = path + "/New " + typeof(T).ToString() + ".asset";
    //        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(bob);

    //        AssetDatabase.CreateAsset(asset, assetPathAndName);

    //        AssetDatabase.SaveAssets();
    //        AssetDatabase.Refresh();
    //        EditorUtility.FocusProjectWindow();
    //        Selection.activeObject = asset;
    //    }
    //}
}

