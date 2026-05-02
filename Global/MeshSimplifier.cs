using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSimplifier
{
    /*
    [SerializeField, Range(0f, 1f), Tooltip("The desired quality of the simplified mesh.")]
    private float quality = 0.5f;

    private void Start()
    {
        Simplify2();
    }
    */
    /*
    [SerializeField] bool active = false;

    private void Start()
    {
        if(active) Simplify3();
    }
    */

    private void Simplify(MeshFilter filter, float quality)
    {
        if (filter == null) // verify that there is a mesh filter
            return;

        Mesh sourceMesh = filter.sharedMesh;
        if (sourceMesh == null) // verify that the mesh filter actually has a mesh
            return;

        // Create our mesh simplifier and setup our vertices and indices from all sub meshes in it
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Vertices = sourceMesh.vertices;

        for (int i = 0; i < sourceMesh.subMeshCount; i++)
        {
            meshSimplifier.AddSubMeshTriangles(sourceMesh.GetTriangles(i));
        }

        // This is where the magic happens, lets simplify!
        meshSimplifier.SimplifyMesh(quality);

        // Create our new mesh and transfer vertices and indices from all sub meshes
        var newMesh = new Mesh();
        newMesh.subMeshCount = meshSimplifier.SubMeshCount;
        newMesh.vertices = meshSimplifier.Vertices;

        for (int i = 0; i < meshSimplifier.SubMeshCount; i++)
        {
            newMesh.SetTriangles(meshSimplifier.GetSubMeshTriangles(i), 0);
        }

        filter.sharedMesh = newMesh;
    }


    //하위 자식들의 메쉬필터를 변경
    public Mesh Simplify2(MeshFilter filters , float quality)
    {
        return SimplifyMeshFilter(filters, quality);
    }

    private Mesh SimplifyMeshFilter(MeshFilter meshFilter, float quality)
    {
        Mesh sourceMesh = meshFilter.sharedMesh;
        if (sourceMesh == null) // verify that the mesh filter actually has a mesh
            return null;

        // Create our mesh simplifier and setup our entire mesh in it
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);

        // This is where the magic happens, lets simplify!
        meshSimplifier.SimplifyMesh(quality);

        Mesh mesh = meshSimplifier.ToMesh();
        mesh.name = "[RV]"+sourceMesh.name;

        // Create our final mesh and apply it back to our mesh filter
        //meshFilter.sharedMesh = mesh;

        //unload any meshes in memory
        Resources.UnloadUnusedAssets();

        return mesh;
    }


    //하위 자식들의 메쉬필터를 변경
    public void Simplify3(MeshFilter meshFilter, float quality)
    {
        
          SimplifyMeshFilter2(meshFilter, quality);
    }

    private void SimplifyMeshFilter2(MeshFilter meshFilter, float quality)
    {
        Mesh sourceMesh = meshFilter.sharedMesh;
        if (sourceMesh == null) // verify that the mesh filter actually has a mesh
            return;

        // Create our mesh simplifier and setup our entire mesh in it
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);

        // This is where the magic happens, lets simplify!
        meshSimplifier.SimplifyMesh(quality);

        Mesh mesh = meshSimplifier.ToMesh();
        mesh.name = "[RV]" + sourceMesh.name;

        // Create our final mesh and apply it back to our mesh filter
        meshFilter.sharedMesh = mesh;

        //unload any meshes in memory
        Resources.UnloadUnusedAssets();
    }
}
