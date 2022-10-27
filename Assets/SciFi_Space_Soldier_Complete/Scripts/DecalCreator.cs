using UnityEngine;
using System.Collections;

public class DecalCreator : MonoBehaviour {

    public GameObject DecalMesh;
    public Vector3 Direction = Vector3.down;
    public Vector3 Offset = Vector3.up * 0.05f;
    public float RandomRotation = 180f;
    public float RandomScale = 1.0f;

	private void Start () {
        if (DecalMesh)
        {
            RaycastHit hit;
            float RandomRot = Random.Range(-RandomRotation, RandomRotation);
            if (Physics.Raycast(transform.position, Direction, out hit))
            {
              GameObject Decal = Instantiate(DecalMesh, hit.point + Offset, Quaternion.Euler(transform.eulerAngles + new Vector3(0f, RandomRot, 0.0f))) as GameObject;

              Decal.transform.localScale += Vector3.one * Random.Range(0f, RandomScale);
              Decal.transform.parent = hit.transform;
            }
        }
            
	}
}
