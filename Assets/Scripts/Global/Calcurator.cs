using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;


//�ش� ������Ʈ���� �ʿ��� ������ ��Ƶ� ��.
public static class Calculator
{
    //�� ������Ʈ ���� ���� �����ϸ� false�� ��ȯ
    public static bool GetBetweenHeight(Vector3 front, Vector3 back)
    {
        return Mathf.Abs(front.y - back.y) < 0.9f;
    }

    // ������Ʈ�� ȭ�� ���ο� �����ϴ��� �Ǵ��Ѵ�.
    public static bool ObjectInViewport(Vector3 pos) 
    {
        Plane[] plane = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        for (int i = 0; i < plane.Length; i++)
        {
            if (plane[i].GetDistanceToPoint(pos) < 0.0f) return false;
        }
        return true;
    }

    // �� ������Ʈ ���� ���Ⱚ ���
    public static Vector3 GetBetweenVecter(Vector3 lookAt, Vector3 current)
    {
        return (lookAt - current).normalized;
    }

    static NavMeshPath path = new NavMeshPath();
    //�ʹ� ���� ȣ���ϸ� ���� �ʽ��ϴ�.
    //���� �׺�޽��� ������� �Ÿ����� ����ؼ� �Ѱ��ݴϴ�.
    public static float GetPathDistance(Vector3 from, Vector3 to)
    {
        if (to.y == Mathf.Infinity) return Mathf.Infinity;
        if (!NavMesh.CalculatePath(from, to, 1, path))
        {
            Debug.LogWarning("�׺� �޽��� �������� �ʽ��ϴ�.");
            return Mathf.Infinity;
        }

        float distance = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            Debug.DrawLine(path.corners[i], path.corners[i + 1]);
        }

        //Debug.LogWarning($"���� ��� �Ÿ��� ���� : {distance}");

        return distance;
    }
}