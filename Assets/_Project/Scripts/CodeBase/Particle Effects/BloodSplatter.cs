using UnityEngine;

namespace _Project.Scripts.CodeBase.Particle_Effects
{
    public class BloodSplatter : MonoBehaviour
    {
        public Transform rayDir;
        public Transform rayTarget;
        public float directionRandom = 5.0f;
        private Vector3 _finalOrientation;

        public int splatNumber = 5;
        private ParticleSystem _particles;

        private void Start()
        {
            _particles = GetComponentInChildren<ParticleSystem>();

            _particles.Emit(1);
            _particles.Clear();

            rayDir = new GameObject("rayDir").transform;
            var position = rayDir.position;
            var transform1 = transform;

            position = transform1.position + (transform1.up * 1.5f);
            position -= transform1.forward * 0.5f;
            rayDir.position = position;
            rayDir.rotation = transform1.rotation;
            rayDir.parent = transform1;

            rayTarget = new GameObject("rayTarget").transform;

            var position1 = rayTarget.position;

            position1 = rayDir.position + rayDir.forward * -2;
            position1 -= transform1.up * 0.5f;
            rayTarget.position = position1;
            rayTarget.rotation = transform1.rotation;
            rayTarget.parent = rayDir;
        }


        private void RandomRotation()
        {
            float randomRot = Random.Range(directionRandom, -directionRandom);
            float randomRot2 = Random.Range(0, -directionRandom * 4);
            float randomRot3 = Random.Range(directionRandom, -directionRandom);

            _finalOrientation = rayTarget.position + new Vector3(randomRot, randomRot2, randomRot3);
            _finalOrientation = _finalOrientation - rayDir.position;
            _finalOrientation = _finalOrientation.normalized * 10;
        }

        public void Splat()
        {
            for (var i = 0; i < splatNumber; i++)
            {
                RandomRotation();

                RaycastHit hit;

                int layerMask = 1 << 0;
                int LayerMask2 = 1 << 8;
                layerMask = layerMask | LayerMask2;

                if (Physics.Raycast(rayDir.position, _finalOrientation, out hit, 10.0f, layerMask,
                        QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.tag != "Player" && hit.collider.tag != "Enemy")
                    {
                        //Debug.Log("Collider =" + hit.collider + " " + hit.normal);
                        _particles.transform.rotation = Quaternion.LookRotation(hit.normal);
                        Vector3 rot = _particles.transform.rotation.eulerAngles;

                        _particles.transform.position = hit.point + (_particles.transform.forward * 0.025f);

                        var emitParams = new ParticleSystem.EmitParams();
                        emitParams.rotation3D = rot;
                        emitParams.position = hit.point + (_particles.transform.forward * 0.025f) +
                                              (Vector3.up * Random.Range(0.03f, 0.01f));

                        _particles.Emit(emitParams, 1);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (rayDir && rayTarget)
            {
                Gizmos.DrawLine(rayDir.position, _finalOrientation);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(rayDir.position, _finalOrientation);
                Gizmos.DrawSphere(rayTarget.position, 0.25f);
            }
        }
    }
}