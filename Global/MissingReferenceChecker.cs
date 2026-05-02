//using UnityEditor;
//using UnityEngine;

//public class MissingReferenceChecker
//{
//    [MenuItem("Tools/Check Missing References")]
//    static void CheckMissingReferences()
//    {
//        var all = AssetDatabase.GetAllAssetPaths();
//        foreach (var path in all)
//        {
//            var obj = AssetDatabase.LoadMainAssetAtPath(path);
//            if (obj == null) continue;

//            var dependencies = EditorUtility.CollectDependencies(new Object[] { obj });
//            foreach (var dep in dependencies)
//            {
//                if (dep == null)
//                {
//                    Debug.LogWarning($"Missing reference in: {path}");
//                }
//            }
//        }
//    }
//}
