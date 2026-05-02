
using UnityEngine;
using UnityEditor;

public static class UIElementSaver
{
    //[MenuItem("Tools/💾 Save All UI Positions")]
    public static void SaveAllUIPositions()
    {
        SavingElementAnchor[] all = Object.FindObjectsOfType<SavingElementAnchor>(true);
        int count = 0;

        foreach (var item in all)
        {
            item.SavePosition();
            //EditorUtility.SetDirty(item); // 저장
            count++;
        }

        Debug.Log($"[UIPositionSaver] {count}개의 UI 위치를 저장했습니다.");
    }
}