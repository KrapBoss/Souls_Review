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
        //����� ��⿡���� ����
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

        // 1.  ��Ƽ���� �з�
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(); //���� �ڽ��� �޽� �������� �����´�.
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform)//������ ��ü��� �ǳʶڴ�.
                continue;
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                    materials.Add(localMat);
        }



        // ������ ��Ƽ���� �ش��ϴ� ������Ʈ�� �����Ͽ� ������ �غ��Ѵ�.
        List<Mesh> submeshes = new List<Mesh>();
        foreach (Material material in materials)
        {
            // Make a combiner for each (sub)mesh that is mapped to the right material.
            List<CombineInstance> combiners = new List<CombineInstance>();//���е� �޽��� �����ֱ� ���� �͵��� ��Ƶд�.
            foreach (MeshFilter filter in filters)//����������Ʈ�� �޽����͸� ��Ƽ���� ���� �з��Ѵ�.
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
                //�ش� �޽��� ���� ��Ƽ������ �����Ͽ� �׷��� ������ �������ش�.
                Material[] localMaterials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)//���� ��Ƽ������ �ƴ� ��� �ٽ� ã�´�.
                        continue;

                    //�޽� ������ ���� �͵��� 
                    CombineInstance ci = new CombineInstance();
                    //filter�� Mesh ���� �� ���° ���� �ε��� �޽������� ������ ������ �� �������� ��Ÿ���ϴ�.
                    ci.mesh = filter.sharedMesh;
                    ci.subMeshIndex = materialIndex; // ���� �޽��������� �����޽� �ε��� ��ȣ�� �����Ѵ�. // �⺻������ 0`N����

                    //�ش� �޽� ��ǥ�� ������� ���̴� ��ǥ�� �����Ѵ�.
                    Matrix4x4 _matrix = filter.transform.localToWorldMatrix;
                    Vector4 _worldPosition = _matrix.GetColumn(3);
                    _matrix.SetColumn(3, _worldPosition - _CombineVector4);

                    ci.transform = _matrix; // ���� ��ġ�� �ش��ϴ� ��ǥ�� ���� ��ǥ�� �����Ͽ� �޽��� �ѱ��.
                    combiners.Add(ci);
                }
            }

            // Flatten into a single mesh.
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiners.ToArray(), true); // ������ �з��� �޽��� �����ش�.[subMehs =1]
            Debug.Log($"{mesh.subMeshCount}");
            submeshes.Add(mesh);//��Ƽ���� �������� �޽��� �׷��� �����Ѵ�.
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
        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);//�� �޽��� ���� �ٸ� ��Ƽ������ ����ϹǷ� ���� �޽��� �ƴ� ���� �޽��μ� ���� �׷��������ؾ� �ȴ�.
        //Debug.Log($"{finalMesh.subMeshCount}");
        GetComponent<MeshFilter>().sharedMesh = finalMesh;
        GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();//���������� �׸� ������Ʈ�� ��Ƽ������ �����Ѵ�.
        Debug.Log("���� ������Ʈ�� �ش� ��Ƽ������ ������ �����ϴ�." + submeshes.Count + " materials.");

        HideChildObjects();
    }

    [Header("Dont Touch this Value")]
    public string MeshColliderName="Default";//�⺻������ �θ��̸� + ���� �̸�


    //������ ������Ʈ�� �����Ѵ�.
    public List<GameObject> CombinededObjects = new List<GameObject>();

    //Ű���忡 ���� �޽��� �з��Ͽ� �޽��� �����մϴ�.
    public void MergeMeshAccordingToShaderKeyword()
    {
        Clear();
        ShowChildObjects();

        Debug.LogWarning($"������ �õ��մϴ�. [���� ������Ʈ �̸� : {transform.name}]");

        // All our children (and us)
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        Vector4 _CombineVector4 = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1);

        // 1.  ��Ƽ���� �з�
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(); //���� �ڽ��� �޽� �������� �����´�.
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform)//������ ��ü��� �ǳʶڴ�.
                continue;
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                {
                    materials.Add(localMat);
                    Debug.Log($"��Ƽ������ �߰��մϴ�. {materials[materials.Count - 1]}  [{materials.Count - 1}]");
                }
                    
        }
        //foreach (Material localMat in materials) Debug.Log($"Materials {localMat}");


        //2, Ű���忡 ���� ��Ƽ���� �з�
        List<string> _Keywords = new List<string>();
        int[] _KeyWordIndex = new int[materials.Count];//****���е� ��Ƽ���� �����ϴ� Ű���� �ε���
        for (int i = 0; i < materials.Count; i++)
        {
            string _keyword = "";
            if (materials[i] == null) Debug.Log($"��Ƽ���� ���� ���Դϴ�. {i}");
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

            if (_Keywords.Contains(_keyword))//���� �̹� Ű���带 �����ϰ� �ִٸ�? ������
            {
                int _index = _Keywords.IndexOf(_keyword);
                //Debug.LogWarning($"Ű���尡 �����մϴ�. {_keyword} / INDEX {_index}");
                _KeyWordIndex[i] = _index;
                continue;
            }

            _Keywords.Add(_keyword);
            _KeyWordIndex[i] = _Keywords.Count - 1;
            //Debug.LogWarning($"Ű���尡 �������� �ʽ��ϴ�. {_keyword} / INDEX {_Keywords.Count - 1}");
        }


        //3. Ű���� ������ ���� ���� ������Ʈ�� �����մϴ�.
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

        //4. Ű���忡 ���� �޽� ������ ����
        for (int i = 0; i < _Keywords.Count; i++)// Ű���� ���� ��ŭ �ݺ�
        {
            // ������ ��Ƽ���� �ش��ϴ� �޽��� ������ �غ��Ѵ�.
            List<Mesh> submeshes = new List<Mesh>();
            List<Material> materialAccordingToKeyword = new List<Material>();

            for (int _index = 0; _index < materials.Count; _index++)
            {
                if (_KeyWordIndex[_index] != i) { continue; }//���� ��Ƽ���� �����ϴ� �ε����� �ƴ� ��쿡�� �ǳʶڴ�.

                if (!materialAccordingToKeyword.Contains(materials[_index]))
                {
                    materialAccordingToKeyword.Add(materials[_index]); // ���� ����
                    Debug.LogWarning($"������ �����մϴ�. {materials[_index]}");
                }
                else
                {
                    Debug.LogWarning($"������ �������� ����. {materials[_index]}");
                }

                //�޽� ���� 1�ܰ� => ��Ƽ���� ���� �޽� ����
                List<CombineInstance> combiners = new List<CombineInstance>();//�޽��� �����ϱ� ���� ������ ����
                foreach (MeshFilter filter in filters)//���� ������Ʈ�� �޽����� �����Ͽ� �����ϴ� �۾��� �Ұ���.
                {
                    if (filter.transform == transform) continue; //���� ������Ʈ�� ��쿡 �ǳʶڴ�.

                    //�ش� ������ �������� �����´�.
                    MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
                    if (renderer == null)
                    {
                        Debug.LogError(filter.name + " has no MeshRenderer");
                        continue;
                    }

                    Material[] localMaterials = renderer.sharedMaterials;//�ش� �޽��� ��Ƽ������ ��� �����´�.
                    for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++) //��
                    {
                        if (localMaterials[materialIndex] != materials[_index])//���� ã�� �ִ� ��Ƽ������ �ƴ� ��� �ǳʶٱ�
                            continue;

                        //�´ٸ�?
                        CombineInstance ci = new CombineInstance();
                        //filter�� Mesh ���� �� ���° ���� �ε��� �޽������� ������ ������ �� �������� ��Ÿ���ϴ�.
                        /*if (VertexDecrease)
                        {//���ؽ� ����Ƽ�� ������ ���ΰ�?
                            ci.mesh = meshSimplifier.Simplify2(filter, quality); //����Ƽ�� ����� �޽��� �����´�.
                        }
                        else
                        {//�״�� ����� ���ΰ�?
                            ci.mesh = filter.sharedMesh;     //�ش� ��Ƽ������ �׸��� �޽�
                        }*/
                        ci.mesh = filter.sharedMesh;     //�ش� ��Ƽ������ �׸��� �޽�
                        ci.subMeshIndex = materialIndex; // ���° ��Ƽ���� �ش��ϴ� �׷��� ����� ��

                        //����� ��ǥ ����
                        Matrix4x4 _matrix = filter.transform.localToWorldMatrix;
                        Vector4 _worldPosition = _matrix.GetColumn(3);
                        _matrix.SetColumn(3, _worldPosition - _CombineVector4);

                        ci.transform = _matrix; // ���� ��ġ�� �ش��ϴ� ��ǥ�� ���� ��ǥ�� �����Ͽ� �޽��� �ѱ��.
                        combiners.Add(ci);
                    }
                    //Resources.UnloadUnusedAssets();
                }

                // �ش��ϴ� ��Ƽ������ �޽��� ��� ��ģ��.
                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.CombineMeshes(combiners.ToArray(), true); // ������ �з��� �޽��� �����ش�.[subMehs =1]
                //Debug.Log($"{mesh.subMeshCount}");
                submeshes.Add(mesh);//��Ƽ���� �������� �޽��� �׷��� �����Ѵ�.
            }

            //Resources.UnloadUnusedAssets();

            // ���������� ��Ƽ���� ���� ���е� �޽õ��� ��� ��ģ��.
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
            finalMesh.CombineMeshes(finalCombiners.ToArray(), false);//�� �޽��� ���� �ٸ� ��Ƽ������ ����ϹǷ� ���� �޽��� �ƴ� ���� �޽��μ� ������ �����Ѵ�.

            //Debug.Log($"{finalMesh.subMeshCount}");//����� ���� ���� Ű������ �����ϴ� ��Ƽ������ ������ ���´�.

            //CombinededObjects[i].GetComponent<MeshFilter>().sharedMesh = finalMesh;
            MeshSave(finalMesh);
            CombinededObjects[i].GetComponent<MeshRenderer>().sharedMaterials = materialAccordingToKeyword.ToArray();//���������� �׸� ������Ʈ�� ��Ƽ������ �����Ѵ�.
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

    void MeshSave(Mesh _mesh)// ������ �޽��� �����Ѵ�.
    {
#if UNITY_EDITOR
        { // Mesh ����
            string path = $"Assets/Meshes/{_mesh.name}.asset";
            AssetDatabase.CreateAsset(_mesh, AssetDatabase.GenerateUniqueAssetPath(path));
            AssetDatabase.SaveAssets();
            Debug.Log($"�޽��� ����Ǿ����ϴ�. // {path}");
        }
#endif
    }

    public void ShowChildObjects() // ����������Ʈ���� �����ش�.
    {
        Debug.Log($"�ڽĵ��� Ȱ��ȭ ó���մϴ�. [���� ������Ʈ �̸� : {transform.name}]");

        Transform[] _Gameobjets = GetComponentsInChildren<Transform>(true);
        foreach (Transform _go in _Gameobjets)
        {
            if (_go.transform == this.transform) continue;
            _go.gameObject.SetActive(true);
        }

        _Gameobjets = null;
    }
    public void HideChildObjects() //���� ������Ʈ�� �����.
    {
        Transform[] _Gameobjets = GetComponentsInChildren<Transform>(true);
        foreach (Transform _go in _Gameobjets)
        {
            if (_go.transform == this.transform) continue;
            _go.gameObject.SetActive(false);
        }
        Debug.Log($"�ڽĵ��� ��Ȱ��ȭ ó���մϴ�. [���� ������Ʈ �̸� : {transform.name}]");

        //���� Ű���� ���� ������Ʈ�� �ִٸ�?
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
        Debug.Log($"�޽������͸� �����մϴ�. [���� ������Ʈ �̸� : {transform.name}]");
        if (CombinededObjects.Count > 0)
        {
            for (int i = 0; CombinededObjects.Count > i; i++)
            {
                DestroyImmediate(CombinededObjects[i]);
            }
            CombinededObjects.Clear();
        }
    }

    public void ShowOnlyChildCollider() // �ö��̴��� ����� �������� �����ش�.
    {
        HideChildRenderer();
    }

    void HideChildRenderer()//�ڽĵ��� �������� ����.
    {
        ShowChildObjects();

        Debug.Log($"�ڽĵ��� �������� ��Ȱ��ȭ�մϴ�. [���� ������Ʈ �̸� : {transform.name}]");

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

    public void ShowChildRenderer()//�ڽĵ��� �������� ���δ�.
    {
        Debug.Log($"�ڽĵ��� �������� Ȱ��ȭ�մϴ�. [���� ������Ʈ �̸� : {transform.name}]");
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
        Debug.Log($"�޽� ���� ��ü�� Mesh �ö��̴��� �����մϴ�. [���� ������Ʈ �̸� : {transform.name}]");
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
        
        Debug.Log($"�޽� ���� ��ü�� �ö��̴��� �����մϴ�. [���� ������Ʈ �̸� : {transform.name}]");
        if (CombinededObjects.Count > 0)
        {
            for (int i = 0; CombinededObjects.Count > i; i++)
            {
                CombinededObjects[i].AddComponent<BoxCollider>();
            }
        }
    }

    //�������� ���� ������Ʈ�� �����Ѵ�.
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