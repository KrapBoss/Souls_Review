using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get Player Location by Local Collider Bounding
/// If player are this object collider range, Floor Name assign to MapManager
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class PlayerLocationBoundary : MonoBehaviour
{
    public string locationName;
    public BoxCollider boxCollider;
    float x, y, z;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        x = boxCollider.bounds.extents.x;
        y = boxCollider.bounds.extents.y;
        z = boxCollider.bounds.extents.z;
    }


}
