using UnityEngine;

//각 맵에 대한 위치값을 넘겨받아 지정받기 위한 데이터
[System.Serializable]
public class Location
{
    public string Name;
    public Transform[] LocalLocation;

    //플레이어의 위치값을 확인하기 위한 경계값
    public BoxCollider[] Bounding;
    Vector3[,] BoundaryLength;


    public void Init()
    {
        //각 컬라이더에 해당하는 컬라이더의 월드상의 크기값을 저장합니다.
        BoundaryLength = new Vector3[Bounding.Length,2];
        for (int i = 0; i < Bounding.Length; i++)
        {
            Vector3 _center = Bounding[i].gameObject.transform.position + Bounding[i].center;
            Vector3 _extents_half = Bounding[i].size * 0.5f;
            BoundaryLength[i,0] = _center - _extents_half;
            BoundaryLength[i,1] = _center + _extents_half;

            Bounding[i].enabled = false;

            //Debug.Log($"위치값 바운드를 지정했습니다. {BoundaryLength[i, 0]} ~ {BoundaryLength[i, 1]}");
        }
    }

    //현재 지정된 지역의 랜던한 위치값을 반환합니다.
    public Vector3 GetRandomLocation()
    {
        return LocalLocation[Random.Range(0, LocalLocation.Length)].position;
    }

    //플레이어가 현재 지정된 컬라이더 안에 존재한다면? true를 반환한다.
    public bool PlayerInBoundary(Vector3 playerPosition)
    {
        for (int i = 0; i < Bounding.Length; i++)
        {
            if (((BoundaryLength[i, 0].x <= playerPosition.x) && (playerPosition.x <= BoundaryLength[i, 1].x))
                && ((BoundaryLength[i, 0].y <= playerPosition.y) && (playerPosition.y <= BoundaryLength[i, 1].y))
                && ((BoundaryLength[i, 0].z <= playerPosition.z) && (playerPosition.z <= BoundaryLength[i, 1].z)))
            {
                //Debug.Log($"Player가 현재 {Name} 안에 존재합니다.");
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

    //플레이어 위치 검색 주기
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

    //플레이어 지역을 업데이트합니다.
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
        //Debug.LogWarning("현재 플레이어의 위치를 찾을 수 없습니다.");
    }

    //플레이어가 위치한 지역의 랜덤한 위치를 반환합니다.
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

    //이름에 따른 랜덤한 지역위치값을 받아옵니다.
    public Vector3 GetLocationByName(string _Name)
    {
        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].Name.Equals(_Name))
            {
                return locations[i].GetRandomLocation();
            }
        }

        Debug.LogError($"MapManager :: 지정한 이름의 위치 정보가 없습니다. {_Name}");
        return Vector3.zero;
    }

    //태양 조절
    public void SetDirectional(Vector3 rotate, bool active)
    {
        if (sunLight.gameObject.activeSelf != active) { sunLight.gameObject.SetActive(active); }

        if (active)
        {
            sunLight.transform.rotation = Quaternion.Euler(rotate);
        }
    }
}
