using UnityEngine;

/// <summary>
/// ��ȥ�� �����մϴ�.
/// ���̵��� ������ �޾� �����˴ϴ�.
/// </summary>
public class SoulSpawn : MonoBehaviour
{
    public int DefaultSoulCount = 6;

    [Header("Clone�� �ֽ��ϴ�. �⺻ �Ӽ��� In,Out �Դϴ�.")]
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
