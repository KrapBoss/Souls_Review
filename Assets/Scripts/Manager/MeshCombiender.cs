using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;


//[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class MeshCombineder : MonoBehaviour
{

    [SerializeField, Range(0f, 1f), Tooltip("The desired quality of the simplified mesh.")]
    private float quality = 0.5f;

    [SerializeField, Tooltip("True if you want reduce vertex in Mesh")]
    private bool VertexDecrease = false;

    [SerializeField, Range(0f, 1.0f)]
    private float deleteRate = 0.0f;

    
    private void Start()
    {
        //모바일 기기에서만 적용
        if(CombinededObjects.Count > 0 && VertexDecrease && !GameConfig.IsPc())
        {
            MeshSimplifier meshSimplifier = new MeshSimplifier();
            
            foreach(GameObject go in CombinededObjects)
            {
                if (go == null) continue;
                MeshFilter mesh = go.GetComponent<MeshFilter>();
                meshSimplifier.Simplify3(mesh, quality);
            }
        }
    }
    

    public void AdvancedMerge()
    {
        Clear();
        ShowChildObjects();

        // All our children (and us)
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        Vector4 _CombineVector4 = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1);

        // 1.  머티리얼 분류
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(); //하위 자식의 메쉬 렌더러를 가져온다.
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform)//현재의 객체라면 건너뛴다.
                continue;
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                    materials.Add(localMat);
        }



        // 구분한 머티리얼에 해당하는 오브젝트를 구분하여 병합을 준비한다.
        List<Mesh> submeshes = new List<Mesh>();
        foreach (Material material in materials)
        {
            // Make a combiner for each (sub)mesh that is mapped to the right material.
            List<CombineInstance> combiners = new List<CombineInstance>();//구분된 메쉬를 합쳐주기 위한 것들을 담아둔다.
            foreach (MeshFilter filter in filters)//하위오브젝트의 메쉬필터를 머티리얼에 따라 분류한다.
            {
                if (filter.transform == transform) continue;
                // The filter doesn't know what materials are involved, get the renderer.
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();  // <-- (Easy optimization is possible here, give it a try!)
                if (renderer == null)
                {
                    Debug.LogError(filter.name + " has no MeshRenderer");
                    continue;
                }

                // Let's see if their materials are the one we want right now.
                //해당 메쉬가 가진 머티리얼을 구분하여 그려줄 순서를 지정해준다.
                Material[] localMaterials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)//현재 머티리얼이 아닌 경우 다시 찾는다.
                        continue;

                    //메쉬 병합을 위한 것들을 
                    CombineInstance ci = new CombineInstance();
                    //filter의 Mesh 정보 중 몇번째 하위 인덱스 메쉬정보를 가지고 병합을 할 것인지를 나타냅니다.
                    ci.mesh = filter.sharedMesh;
                    ci.subMeshIndex = materialIndex; // 현재 메쉬데이터의 하위메쉬 인덱스 번호를 대입한다. // 기본적으로 0`N까지

                    //해당 메쉬 좌표를 월드상의 보이는 좌표로 지정한다.
                    Matrix4x4 _matrix = filter.transform.localToWorldMatrix;
                    Vector4 _worldPosition = _matrix.GetColumn(3);
                    _matrix.SetColumn(3, _worldPosition - _CombineVector4);

                    ci.transform = _matrix; // 현재 위치에 해당하는 좌표를 월드 좌표로 지정하여 메쉬를 넘긴다.
                    combiners.Add(ci);
                }
            }

            // Flatten into a single mesh.
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiners.ToArray(), true); // 위에서 분류된 메쉬를 합쳐준다.[subMehs =1]
            Debug.Log($"{mesh.subMeshCount}");
            submeshes.Add(mesh);//머티리얼 종류별로 메쉬를 그려서 저장한다.
        }

        // The final mesh: combine all the material-specific meshes as independent submeshes.
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            finalCombiners.Add(ci);
        }

        Mesh finalMesh = new Mesh();
        finalMesh.name = transform.name + " CombinededMesh";        //set Mesh Name
        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);//각 메쉬가 서로 다른 머티리얼을 사용하므로 단일 메쉬가 아닌 하위 메쉬로서 같이 그려지도록해야 된다.
        //Debug.Log($"{finalMesh.subMeshCount}");
        GetComponent<MeshFilter>().sharedMesh = finalMesh;
        GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();//최종적으로 그린 오브젝트에 머티리얼을 지정한다.
        Debug.Log("현재 오브젝트가 해당 머티리얼의 개수를 가집니다." + submeshes.Count + " materials.");

        HideChildObjects();
    }

    [Header("Dont Touch this Value")]
    public string MeshColliderName="Default";//기본적으로 부모이름 + 현재 이름


    //합쳐진 오브젝트를 저장한다.
    public List<GameObject> CombinededObjects = new List<GameObject>();

    //키워드에 따른 메쉬를 분류하여 메쉬를 종합합니다.
    public void MergeMeshAccordingToShaderKeyword()
    {
        Clear();
        ShowChildObjects();

        Debug.LogWarning($"병합을 시도합니다. [현재 오브젝트 이름 : {transform.name}]");

        // All our children (and us)
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        Vector4 _CombineVector4 = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1);

        // 1.  머티리얼 분류
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(); //하위 자식의 메쉬 렌더러를 가져온다.
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform)//현재의 객체라면 건너뛴다.
                continue;
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                {
                    materials.Add(localMat);
                    Debug.Log($"머티리얼을 추가합니다. {materials[materials.Count - 1]}  [{materials.Count - 1}]");
                }
                    
        }
        //foreach (Material localMat in materials) Debug.Log($"Materials {localMat}");


        //2, 키워드에 따른 머티리얼 분류
        List<string> _Keywords = new List<string>();
        int[] _KeyWordIndex = new int[materials.Count];//****구분된 머티리얼에 대응하는 키워드 인덱스
        for (int i = 0; i < materials.Count; i++)
        {
            string _keyword = "";
            if (materials[i] == null) Debug.Log($"머티리얼 값이 널입니다. {i}");
            Debug.Log($"{materials[i].name} [ {i} ]");
            if (materials[i].shaderKeywords.Length > 0)
            {
                foreach (string word in materials[i].shaderKeywords)
                {
                    _keyword += $"{word}, ";
                }
            }
            else
            {
                _keyword = "NonKeyWord";
            }

            if (_Keywords.Contains(_keyword))//만약 이미 키워드를 포함하고 있다면? 끝내기
            {
                int _index = _Keywords.IndexOf(_keyword);
                //Debug.LogWarning($"키워드가 존재합니다. {_keyword} / INDEX {_index}");
                _KeyWordIndex[i] = _index;
                continue;
            }

            _Keywords.Add(_keyword);
            _KeyWordIndex[i] = _Keywords.Count - 1;
            //Debug.LogWarning($"키워드가 존재하지 않습니다. {_keyword} / INDEX {_Keywords.Count - 1}");
        }


        //3. 키워드 개수에 따른 하위 오브젝트를 생성합니다.
        for (int i = 0; i < _Keywords.Count; i++)
        {
            GameObject _go = new GameObject();
            _go.name = $"{transform.name} NUM[{i}] Object";
            _go.transform.parent = transform;
            _go.transform.localPosition = Vector3.zero;
            _go.AddComponent<MeshRenderer>();
            _go.AddComponent<MeshFilter>();
            CombinededObjects.Add(_go);
        }


        //MeshSimplifier meshSimplifier = new MeshSimplifier();

        //4. 키워드에 따른 메쉬 데이터 생성
        for (int i = 0; i < _Keywords.Count; i++)// 키워드 갯수 만큼 반복
        {
            // 구분한 머티리얼에 해당하는 메쉬의 병합을 준비한다.
            List<Mesh> submeshes = new List<Mesh>();
            List<Material> materialAccordingToKeyword = new List<Material>();

            for (int _index = 0; _index < materials.Count; _index++)
            {
                if (_KeyWordIndex[_index] != i) { continue; }//만약 머티리얼에 대응하는 인덱스가 아닌 경우에는 건너뛴다.

                if (!materialAccordingToKeyword.Contains(materials[_index]))
                {
                    materialAccordingToKeyword.Add(materials[_index]); // 재질 저장
                    Debug.LogWarning($"재질을 저장합니다. {materials[_index]}");
                }
                else
                {
                    Debug.LogWarning($"재질을 저장하지 않음. {materials[_index]}");
                }

                //메쉬 병합 1단계 => 머티리얼에 따른 메쉬 구분
                List<CombineInstance> combiners = new List<CombineInstance>();//메쉬를 병합하기 위한 데이터 모음
                foreach (MeshFilter filter in filters)//하위 오브젝트의 메쉬들을 구별하여 병합하는 작업을 할것임.
                {
                    if (filter.transform == transform) continue; //본인 오브젝트일 경우에 건너뛴다.

                    //해당 필터의 렌더러를 가져온다.
                    MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
                    if (renderer == null)
                    {
                        Debug.LogError(filter.name + " has no MeshRenderer");
                        continue;
                    }

                    Material[] localMaterials = renderer.sharedMaterials;//해당 메쉬의 머티리얼을 모두 가져온다.
                    for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++) //비교
                    {
                        if (localMaterials[materialIndex] != materials[_index])//현재 찾고 있는 머티리얼이 아닌 경우 건너뛰기
                            continue;

                        //맞다면?
                        CombineInstance ci = new CombineInstance();
                        //filter의 Mesh 정보 중 몇번째 하위 인덱스 메쉬정보를 가지고 병합을 할 것인지를 나타냅니다.
                        /*if (VertexDecrease)
                        {//버텍스 퀄리티는 조절할 것인가?
                            ci.mesh = meshSimplifier.Simplify2(filter, quality); //퀄리티가 변경된 메쉬를 가져온다.
                        }
                        else
                        {//그대로 사용할 것인가?
                            ci.mesh = filter.sharedMesh;     //해당 머티리얼을 그리는 메쉬
                        }*/
                        ci.mesh = filter.sharedMesh;     //해당 머티리얼을 그리는 메쉬
                        ci.subMeshIndex = materialIndex; // 몇번째 머티리얼에 해당하는 그룹을 만드는 것

                        //월드상 좌표 선정
                        Matrix4x4 _matrix = filter.transform.localToWorldMatrix;
                        Vector4 _worldPosition = _matrix.GetColumn(3);
                        _matrix.SetColumn(3, _worldPosition - _CombineVector4);

                        ci.transform = _matrix; // 현재 위치에 해당하는 좌표를 월드 좌표로 지정하여 메쉬를 넘긴다.
                        combiners.Add(ci);
                    }
                    //Resources.UnloadUnusedAssets();
                }

                // 해당하는 머티리얼의 메쉬를 모두 합친다.
                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.CombineMeshes(combiners.ToArray(), true); // 위에서 분류된 메쉬를 합쳐준다.[subMehs =1]
                //Debug.Log($"{mesh.subMeshCount}");
                submeshes.Add(mesh);//머티리얼 종류별로 메쉬를 그려서 저장한다.
            }

            //Resources.UnloadUnusedAssets();

            // 마지막으로 머티리얼에 따라 구분된 메시들을 모두 합친다.
            List<CombineInstance> finalCombiners = new List<CombineInstance>();
            foreach (Mesh mesh in submeshes)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.subMeshIndex = 0;
                ci.transform = Matrix4x4.identity;
                finalCombiners.Add(ci);
            }

            Mesh finalMesh = new Mesh();
            finalMesh.name = $"{transform.name}_NUM[{i}]_{MeshColliderName}_CombinededMesh";      //set Mesh Name
            finalMesh.CombineMeshes(finalCombiners.ToArray(), false);//각 메쉬가 서로 다른 머티리얼을 사용하므로 단일 메쉬가 아닌 하위 메쉬로서 나누어 병합한다.

            //Debug.Log($"{finalMesh.subMeshCount}");//결과를 보면 각각 키워드의 대응하는 머티리얼의 개수가 나온다.

            //CombinededObjects[i].GetComponent<MeshFilter>().sharedMesh = finalMesh;
            MeshSave(finalMesh);
            CombinededObjects[i].GetComponent<MeshRenderer>().sharedMaterials = materialAccordingToKeyword.ToArray();//최종적으로 그린 오브젝트에 머티리얼을 지정한다.
            CombinededObjects[i].tag = gameObject.tag;
            CombinededObjects[i].layer = gameObject.layer;
            Debug.Log($" KEYWORD [{i}] Final mesh has {submeshes.Count} materials.");

        }

        materials = null;
        renderers = null;
        filters = null;
        _KeyWordIndex = null;
        _Keywords = null;

        Resources.UnloadUnusedAssets();

        HideChildObjects();
    }

    void MeshSave(Mesh _mesh)// 지정된 메쉬를 저장한다.
    {
#if UNITY_EDITOR
        { // Mesh 저장
            string path = $"Assets/Meshes/{_mesh.name}.asset";
            AssetDatabase.CreateAsset(_mesh, AssetDatabase.GenerateUniqueAssetPath(path));
            AssetDatabase.SaveAssets();
            Debug.Log($"메쉬가 저장되었습니다. // {path}");
        }
#endif
    }

    public void ShowChildObjects() // 하위오브젝트들을 보여준다.
    {
        Debug.Log($"자식들을 활성화 처리합니다. [현재 오브젝트 이름 : {transform.name}]");

        Transform[] _Gameobjets = GetComponentsInChildren<Transform>(true);
        foreach (Transform _go in _Gameobjets)
        {
            if (_go.transform == this.transform) continue;
            _go.gameObject.SetActive(true);
        }

        _Gameobjets = null;
    }
    public void HideChildObjects() //하위 오브젝트를 숨긴다.
    {
        Transform[] _Gameobjets = GetComponentsInChildren<Transform>(true);
        foreach (Transform _go in _Gameobjets)
        {
            if (_go.transform == this.transform) continue;
            _go.gameObject.SetActive(false);
        }
        Debug.Log($"자식들을 비활성화 처리합니다. [현재 오브젝트 이름 : {transform.name}]");

        //만약 키워드 구분 오브젝트가 있다면?
        if (CombinededObjects.Count > 0)
        {
            for (int i = 0; CombinededObjects.Count > i; i++)
            {
                CombinededObjects[i].SetActive(true);
            }
        }
        _Gameobjets = null;
    }

    public void Clear()
    {
        Debug.Log($"메쉬데이터를 제거합니다. [현재 오브젝트 이름 : {transform.name}]");
        if (CombinededObjects.Count > 0)
        {
            for (int i = 0; CombinededObjects.Count > i; i++)
            {
                DestroyImmediate(CombinededObjects[i]);
            }
            CombinededObjects.Clear();
        }
    }

    public void ShowOnlyChildCollider() // 컬라이더만 남기고 렌더러를 없애준다.
    {
        HideChildRenderer();
    }

    void HideChildRenderer()//자식들의 렌더러를 끈다.
    {
        ShowChildObjects();

        Debug.Log($"자식들의 렌더러만 비활성화합니다. [현재 오브젝트 이름 : {transform.name}]");

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].transform == this.transform) continue;
            renderers[i].enabled = false;
        }
        renderers = null;

        if (CombinededObjects.Count > 0)
        {
            for (int i = 0; CombinededObjects.Count > i; i++)
            {
                CombinededObjects[i].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public void ShowChildRenderer()//자식들의 렌더러를 보인다.
    {
        Debug.Log($"자식들의 렌더러를 활성화합니다. [현재 오브젝트 이름 : {transform.name}]");
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].transform == this.transform) continue;
            renderers[i].enabled = true;
        }
        renderers = null;
    }

    public void AddMeshCollider()
    {
        Debug.Log($"메쉬 필터 객체에 Mesh 컬라이더를 생성합니다. [현재 오브젝트 이름 : {transform.name}]");
        if(CombinededObjects.Count > 0)
        {
            for(int i = 0;CombinededObjects.Count > i; i++)
            {
                CombinededObjects[i].AddComponent<MeshCollider>().convex = true;
            }
        }
    }
    public void AddBoxCollider()
    {
    //    MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(); 
    //    for (int i = 0; i < renderers.Length; i++)
    //    {

    //        renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    //    }
        
        Debug.Log($"메쉬 필터 객체에 컬라이더를 생성합니다. [현재 오브젝트 이름 : {transform.name}]");
        if (CombinededObjects.Count > 0)
        {
            for (int i = 0; CombinededObjects.Count > i; i++)
            {
                CombinededObjects[i].AddComponent<BoxCollider>();
            }
        }
    }

    //랜덤으로 하위 오브젝트를 삭제한다.
    public void DeleteChildRandom()
    {
        List<MeshFilter> gos =  GetComponentsInChildren<MeshFilter>().ToList();

        int deleteNum = (int)(gos.Count * deleteRate);

        if(deleteNum > 0 && gos.Count>1)
        {
            while (deleteNum>0)
            {
                deleteNum--;
                int index = Random.Range(0, gos.Count);
                DestroyImmediate(gos[index].gameObject);
                gos.RemoveAt(index);
            }
        }
    }
}