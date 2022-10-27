using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrWeaponNodeHelper : MonoBehaviour {

    [Header("Display & Debug Settings")]
    public Mesh WeaponReference;
    public float meshScale = 1.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawMesh(WeaponReference, transform.position, transform.rotation, Vector3.one * meshScale);
        
    }
}
