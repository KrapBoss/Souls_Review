using UnityEngine;

//�� �ʿ� ���� ��ġ���� �Ѱܹ޾� �����ޱ� ���� ������
[System.Serializable]
public class Location
{
    public string Name;
    public Transform[] LocalLocation;

    //�÷��̾��� ��ġ���� Ȯ���ϱ� ���� ��谪
    public BoxCollider[] Bounding;
    Vector3[,] BoundaryLength;


    public void Init()
    {
        //�� �ö��̴��� �ش��ϴ� �ö��̴��� ������� ũ�Ⱚ�� �����մϴ�.
        BoundaryLength = new Vector3[Bounding.Length,2];
        for (int i = 0; i < Bounding.Length; i++)
        {
            Vector3 _center = Bounding[i].gameObject.transform.position + Bounding[i].center;
            Vector3 _extents_half = Bounding[i].size * 0.5f;
            BoundaryLength[i,0] = _center - _extents_half;
            BoundaryLength[i,1] = _center + _extents_half;

            Bounding[i].enabled = false;

            //Debug.Log($"��ġ�� �ٿ�带 �����߽��ϴ�. {BoundaryLength[i, 0]} ~ {BoundaryLength[i, 1]}");
        }
    }

    //���� ������ ������ ������ ��ġ���� ��ȯ�մϴ�.
    public Vector3 GetRandomLocation()
    {
        return LocalLocation[Random.Range(0, LocalLocation.Length)].position;
    }

    //�÷��̾ ���� ������ �ö��̴� �ȿ� �����Ѵٸ�? true�� ��ȯ�Ѵ�.
    public bool PlayerInBoundary(Vector3 playerPosition)
    {
        for (int i = 0; i < Bounding.Length; i++)
        {
            if (((BoundaryLength[i, 0].x <= playerPosition.x) && (playerPosition.x <= BoundaryLength[i, 1].x))
                && ((BoundaryLength[i, 0].y <= playerPosition.y) && (playerPosition.y <= BoundaryLength[i, 1].y))
                && ((BoundaryLength[i, 0].z <= playerPosition.z) && (playerPosition.z <= BoundaryLength[i, 1].z)))
            {
                //Debug.Log($"Player�� ���� {Name} �ȿ� �����մϴ�.");
                return true;
            }
        }
        return false;
    }
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
  
    [SerializeField] Location[] locations = new Location[0];
    string playerLocationName= "default";

    //�÷��̾� ��ġ �˻� �ֱ�
    public float searchTime = 0.07f;
    float searchTimeOut = 0.1f;

    public Light sunLight;

    public Transform DefaultObjectParent;

    private void Awake()
    {
        if(instance == null) { instance = this; }
        else
        {
            Destroy(instance);
            instance = this;
        }

        foreach (Location location in locations) { location.Init(); }
    }
    private void OnDestroy()
    {
        Destroy(instance);
        instance = null;
    }

    private void Update()
    {
        SearchPlayerLocation();
    }

    //�÷��̾� ������ ������Ʈ�մϴ�.
    public void SearchPlayerLocation()
    {
        if (searchTimeOut > 0.0f) { 
            searchTimeOut -= Time.deltaTime;
            return;
        }

        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].PlayerInBoundary(PlayerEvent.instance.transform.position))
            {
                playerLocationName = locations[i].Name;
                searchTimeOut = searchTime;
                return;
            }
        }

        searchTimeOut = searchTime;
        //Debug.LogWarning("���� �÷��̾��� ��ġ�� ã�� �� �����ϴ�.");
    }

    //�÷��̾ ��ġ�� ������ ������ ��ġ�� ��ȯ�մϴ�.
    public Vector3 GetLocationAroundPlayer()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].Name.Equals(playerLocationName))
            {
                return locations[i].GetRandomLocation();
            }
        }

        return Vector3.zero;
    }

    //�̸��� ���� ������ ������ġ���� �޾ƿɴϴ�.
    public Vector3 GetLocationByName(string _Name)
    {
        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].Name.Equals(_Name))
            {
                return locations[i].GetRandomLocation();
            }
        }

        Debug.LogError($"MapManager :: ������ �̸��� ��ġ ������ �����ϴ�. {_Name}");
        return Vector3.zero;
    }

    //�¾� ����
    public void SetDirectional(Vector3 rotate, bool active)
    {
        if (sunLight.gameObject.activeSelf != active) { sunLight.gameObject.SetActive(active); }

        if (active)
        {
            sunLight.transform.rotation = Quaternion.Euler(rotate);
        }
    }
}
