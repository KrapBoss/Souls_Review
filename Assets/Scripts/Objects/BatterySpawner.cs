using System.Linq;
using UnityEngine;

/// <summary>
/// 데미지 손상을 복구할 수 있는 배터리 아이템을 복구합니다.
/// </summary>
public class BatterySpawner : MonoBehaviour
{
    public GameObject[] Points;

    // Start is called before the first frame update
    void Start()
    {
        Points = FindObjectsOfType<BatteryRecovery>().Select(x => x.gameObject).ToArray();
        Debug.LogWarning($"배터리 활성화를 위한 위치를 찾았습니다 {Points.Length}");
        
        foreach ( GameObject p in Points ) p.SetActive( false );
        

        if (DataSet.Instance.GameDifficulty.CameraRecoveryBatteryCount > Points.Length)
        {
            Debug.LogError("배터리 생성에 문제가 생겼습니다. OverFlow");
            return;
        }

        for (int i = 0; i < DataSet.Instance.GameDifficulty.CameraRecoveryBatteryCount; i++)
        {
            Debug.LogWarning("카메라 배터리 포인트 자동 생성 => " + i);

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
