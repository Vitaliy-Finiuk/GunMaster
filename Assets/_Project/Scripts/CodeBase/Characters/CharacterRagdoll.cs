using _Project.Scripts.CodeBase.WeaponsSystem;
using _Project.Scripts.CodeBase.WeaponsSystem.Weapons.Projectile;
using UnityEngine;

namespace _Project.Scripts.CodeBase.Characters
{
    public class CharacterRagdoll : MonoBehaviour {

        public GameObject[] ragdollBones;
        public bool DeactivateAtStart = true;
        private Transform weaponObject;

        private ParticleSystem[] VFXParticles;
        private bool active = false;

        private float ragdollTimer = 5.0f;
        private float activeRagdollTimer = 0.0f;

        private void Start () {
            InitializeRagdoll();
            if (GetComponentInChildren<ParticleSystem>())
                VFXParticles = GetComponentsInChildren<ParticleSystem>();
        }

        // Update is called once per frame
        private void Update () {
            if (active)
            {
                if (activeRagdollTimer < ragdollTimer)
                    activeRagdollTimer += Time.deltaTime;
                else
                    DeactivateRagdoll();
            }
        }

        public void InitializeRagdoll()
        {
            if (transform.GetComponentInChildren<Weapon>())
            {
                weaponObject = transform.GetComponentInChildren<Weapon>().transform;
                weaponObject.gameObject.layer = LayerMask.NameToLayer("PlayerCharacter");
            }
       
            Rigidbody[] temp = transform.GetComponentsInChildren<Rigidbody>();
            ragdollBones = new GameObject[temp.Length];
            int t = 0;
            foreach (Rigidbody r in temp)
            {
                if (r.gameObject.name != gameObject.name)
                {
                    r.gameObject.layer = LayerMask.NameToLayer("DeadCharacter");
                    ragdollBones.SetValue(r.gameObject, t);
                    t += 1;
                }
            }

            if (DeactivateAtStart)
            {
                foreach (GameObject GO in ragdollBones)
                {
                    if (GO != null)
                    {
                        GO.GetComponent<Collider>().enabled = false;
                        GO.GetComponent<Rigidbody>().isKinematic = true;
                        if (GO.GetComponent<CharacterJoint>()) 
                            GO.GetComponent<CharacterJoint>().enableProjection = true;

                    }
                }

                if (weaponObject)
                {
                    weaponObject.gameObject.GetComponent<Collider>().enabled = false;
                    weaponObject.gameObject.layer = LayerMask.NameToLayer("Weapon");
                }
            }
        }

        public void DeactivateRagdoll()
        {
            foreach (var GO in ragdollBones)
            {
                if (GO != null)
                {
                    GO.GetComponent<Collider>().enabled = false;
                    GO.GetComponent<Rigidbody>().isKinematic = true;

                }
            }
            active = false;
            activeRagdollTimer = 0.0f;
        }

        public void ActivateRagdoll()
        {
            active = true;
            if (GetComponent<Animator>())
                GetComponent<Animator>().enabled = false;
            foreach (GameObject GO in ragdollBones)
            {
                if (GO != null)
                {
                    GO.GetComponent<Collider>().enabled = true;
                    GO.GetComponent<Rigidbody>().isKinematic = false;
                
                }
            }
            if (weaponObject)
            {
                weaponObject.SetParent(null);
                weaponObject.gameObject.AddComponent<Rigidbody>();
                weaponObject.gameObject.GetComponent<Collider>().enabled = true;
            }
          
        }
        public void SetForceToRagdoll(Vector3 position, Vector3 force, Transform target)
        {
            if (!active)
                ActivateRagdoll();

            GameObject targetBone = ragdollBones[0];
            if (target != null)
            {
                targetBone = target.gameObject;
            }
            else
            {
                float dist = 200f;
                foreach (GameObject go in ragdollBones)
                {
                    if (go != null)
                    {
                        float tempDist = Vector3.Distance(go.transform.position, position);
                        if (dist > tempDist)
                        {
                            dist = tempDist;
                            targetBone = go;
                        }
                    }
                }
            }
       

            targetBone.GetComponent<Rigidbody>().AddForceAtPosition(force, position,ForceMode.Impulse);

            if (weaponObject)
            {    //weaponObject.GetComponent<Rigidbody>().AddExplosionForce(1.0f, position, 1.0f);
                weaponObject.GetComponent<Rigidbody>().AddForceAtPosition(force * 0.1f, position, ForceMode.Impulse);
                weaponObject.gameObject.AddComponent<DestroyTimer>();
            }
        }

        public void SetExplosiveForce(Vector3 position)
        {
            InitializeRagdoll();
    
            foreach (GameObject go in ragdollBones)
            {
                if (go != null)
                    go.GetComponent<Rigidbody>().AddExplosionForce(20.0f, position, 2.0f, 0.25f, ForceMode.Impulse);
            }

            if (VFXParticles != null && VFXParticles.Length > 0)
            {
                foreach (ParticleSystem p in VFXParticles) 
                    p.Play();
            }
        }
    }
}
