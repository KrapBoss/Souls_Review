using UnityEngine;

/// <summary>
/// 영혼을 스폰합니다.
/// 난이도에 영향을 받아 생성됩니다.
/// </summary>
public class SoulSpawn : MonoBehaviour
{
    public int DefaultSoulCount = 6;

    [Header("Clone을 넣습니다. 기본 속성은 In,Out 입니다.")]
    public GameObject go_Souls;

    private void Awake()
    {
        int count = DataSet.Instance.GameDifficulty.SoulCount - DefaultSoulCount;

        if(count > 0 )
        {
            for( int i = 0; i < count; i++ )
            {
                GameObject go = Instantiate(go_Souls,transform);
            }
        }
    }
}
