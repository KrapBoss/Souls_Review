using UnityEditor;
using UnityEngine;

public class GenerateButton : Editor
{
    [CustomEditor(typeof(MeshCombineder))]
    public class MeshCombineButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MeshCombineder generator = (MeshCombineder)target;
            //if (GUILayout.Button("Merge Mesh"))
            //{
            //    generator.AdvancedMerge();
            //}

            if (GUILayout.Button("MergeSameKeywordMaterial"))
            {
                generator.MergeMeshAccordingToShaderKeyword();
            }
            if (GUILayout.Button("ShowOnlyChildCollider"))
            {
                generator.ShowOnlyChildCollider();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("ShowChilds"))
            {
                generator.ShowChildObjects();
                generator.ShowChildRenderer();
            }

            if (GUILayout.Button("HideChileds"))
            {
                generator.HideChildObjects();
            }

            if (GUILayout.Button("ClaerResultOfCombine"))
            {
                generator.Clear();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("AddMeshCollider"))
            {
                generator.AddMeshCollider();
            }
            if (GUILayout.Button("AddBoxCollider"))
            {
                generator.AddBoxCollider();
            }
            if (GUILayout.Button("RandomDelete"))
            {
                generator.DeleteChildRandom();
            }
        }
    }
}