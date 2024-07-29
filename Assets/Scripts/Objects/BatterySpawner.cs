using System.Linq;
using UnityEngine;

/// <summary>
/// ������ �ջ��� ������ �� �ִ� ���͸� �������� �����մϴ�.
/// </summary>
public class BatterySpawner : MonoBehaviour
{
    public GameObject[] Points;

    // Start is called before the first frame update
    void Start()
    {
        Points = FindObjectsOfType<BatteryRecovery>().Select(x => x.gameObject).ToArray();
        Debug.LogWarning($"���͸� Ȱ��ȭ�� ���� ��ġ�� ã�ҽ��ϴ� {Points.Length}");
        
        foreach ( GameObject p in Points ) p.SetActive( false );
        

        if (DataSet.Instance.GameDifficulty.CameraRecoveryBatteryCount > Points.Length)
        {
            Debug.LogError("���͸� ������ ������ ������ϴ�. OverFlow");
            return;
        }

        for (int i = 0; i < DataSet.Instance.GameDifficulty.CameraRecoveryBatteryCount; i++)
        {
            Debug.LogWarning("ī�޶� ���͸� ����Ʈ �ڵ� ���� => " + i);

            do
            {
                int index = Random.Range(0, Points.Length);

                if (!Points[index].activeSelf)
                {
                    Points[index].SetActive(true);
                    break;
                }
            } while (true);
        }
    }
}
