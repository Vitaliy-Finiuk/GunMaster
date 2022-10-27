using UnityEngine;

public class WeaponNodeHelper : MonoBehaviour {

    [Header("Display & Debug Settings")]
    public Mesh WeaponReference;
    public float MeshScale = 1.0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawMesh(WeaponReference, transform.position, transform.rotation, Vector3.one * MeshScale);
    }
}
