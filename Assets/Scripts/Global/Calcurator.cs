using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;


//해당 프로젝트에서 필요한 계산식을 모아둔 곳.
public static class Calculator
{
    //두 오브젝트 사이 층이 존재하면 false를 반환
    public static bool GetBetweenHeight(Vector3 front, Vector3 back)
    {
        return Mathf.Abs(front.y - back.y) < 0.9f;
    }

    // 오브젝트가 화면 내부에 존재하는지 판단한다.
    public static bool ObjectInViewport(Vector3 pos) 
    {
        Plane[] plane = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        for (int i = 0; i < plane.Length; i++)
        {
            if (plane[i].GetDistanceToPoint(pos) < 0.0f) return false;
        }
        return true;
    }

    // 두 오브젝트 사이 방향값 계산
    public static Vector3 GetBetweenVecter(Vector3 lookAt, Vector3 current)
    {
        return (lookAt - current).normalized;
    }

    static NavMeshPath path = new NavMeshPath();
    //너무 자주 호출하면 좋지 않습니다.
    //현재 네비메쉬를 기반으로 거리값을 계산해서 넘겨줍니다.
    public static float GetPathDistance(Vector3 from, Vector3 to)
    {
        if (to.y == Mathf.Infinity) return Mathf.Infinity;
        if (!NavMesh.CalculatePath(from, to, 1, path))
        {
            Debug.LogWarning("네비 메쉬가 존재하지 않습니다.");
            return Mathf.Infinity;
        }

        float distance = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            Debug.DrawLine(path.corners[i], path.corners[i + 1]);
        }

        //Debug.LogWarning($"현재 경로 거리값 제공 : {distance}");

        return distance;
    }
}