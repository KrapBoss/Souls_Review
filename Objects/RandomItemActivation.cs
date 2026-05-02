using UnityEngine;

/// <summary>
/// 지정된 개수 만큼만 활성화 됩니다.
/// </summary>
public class RandomItemActivation : MonoBehaviour
{
    public GameObject[] go_Objects;

    public int SpawnCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        if ((go_Objects.Length < SpawnCount) || (go_Objects.Length <1) || (SpawnCount <1)) return;

        foreach(GameObject go in go_Objects)
        {
            go.SetActive(false);
        }

        for (int i = 0; i < SpawnCount; i++)
        {
            bool activation= false;

            while (!activation)
            {
                int index = Random.Range(0, go_Objects.Length);
                if (!go_Objects[index].activeSelf)
                {
                    activation = true;
                    go_Objects[index].SetActive(true);
                }
            }
        }
    }
}
