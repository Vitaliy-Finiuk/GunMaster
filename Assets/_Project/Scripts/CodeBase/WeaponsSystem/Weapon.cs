using System.Collections.Generic;
using CodeBase.Characters.AI;
using CodeBase.Characters.Player;
using UnityEngine;

namespace CodeBase.WeaponsSystem
{
    public class Weapon : MonoBehaviour {


        public string WeaponName = "Rifle";

        public enum WT
        {
            Rifle = 1,  Melee = 4
        }

        public WT Type = WT.Rifle;
        public bool useIK = true;

        [Header("Melee Weapon")]
        public float MeleeRadius = 1.0f;
        public int meleeDamage = 1;
        private List<GameObject> meleeFinalTarget;

        [Header("Stats")]

        public int BulletsPerShoot = 1;
        public int BulletDamage = 20;
        public float tempModFactor = 0.0f;
        public float BulletSize = 1.0f;


        public float BulletSpeed = 1.0f;
        public float BulletAccel = 0.0f;

        public int Bullets = 10;
        [HideInInspector]
        public int ActualBullets = 0;

        public int Clips = 3;
        [HideInInspector]
        public int ActualClips = 0;



        public float bulletTimeToLive = 3.0f;


        public float FireRate = 0.1f;
        public float AccDiv = 0.0f;

        public float radialAngleDirection = 0.0f;

        public float shootingNoise = 25f;



        [Header("References & VFX")]
        public float shootShakeFactor = 2.0f;
        public Transform ShootFXPos;
        public GameObject BulletPrefab;
        public GameObject ShootFXFLash;
        public Light ShootFXLight;
        private TopDownCamera playerCamera;

        [Header("Laser Weapon Settings")]
        public GameObject laserBeamPrefab;
        private GameObject[] actualBeams;

        private GameObject actualWarmingVFX;
        public GameObject laserHitVFX;
        private GameObject[] actualLaserHits;

        [HideInInspector]
        public Transform ShootTarget;
        [HideInInspector]
        public GameObject Player;

        [Header("Sound FX")]
        public AudioClip[] ShootSFX;
        public AudioClip ShootEmptySFX;
        [HideInInspector]
        public AudioSource Audio;

        [Header("Autoaim")]
        public float AutoAimAngle = 7.5f;
        public float AutoAimDistance = 10.0f;

        private Vector3 EnemyTargetAuto = Vector3.zero;
        private Vector3 FinalTarget = Vector3.zero;

        //Object Pooling Manager
        public bool usePooling = true;
        private GameObject[] GameBullets;
        private GameObject BulletsParent;
        private int ActualGameBullet = 0;
        private GameObject Muzzle;

        [HideInInspector]
        public bool AIWeapon = false;
        [HideInInspector]
        public Transform AIEnemyTarget;

        [HideInInspector]
        public int team = 0;

        private void Awake()
        {
            ActualBullets = Bullets;
            ActualClips = Clips;

        }

        // Use this for initialization
        void Start()
        {
            Audio = transform.parent.GetComponent<AudioSource>();

       

            if (Type == WT.Rifle )
            {
                if (usePooling)
                {
                    GameBullets = new GameObject[Bullets * BulletsPerShoot];
                    BulletsParent = new GameObject(WeaponName + "_Bullets");

                    for (int i = 0; i < (Bullets * BulletsPerShoot); i++)
                    {
                        GameBullets[i] = Instantiate(BulletPrefab, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
                        GameBullets[i].SetActive(false);
                        GameBullets[i].name = WeaponName + "_Bullet_" + i.ToString();
                        GameBullets[i].transform.parent = BulletsParent.transform;

                        GameBullets[i].GetComponent<Bullet>().team = team;
                        GameBullets[i].GetComponent<Bullet>().usePooling = true;
                        GameBullets[i].GetComponent<Bullet>().InitializePooling();
                    }
                }
            
            }



            if (ShootFXFLash)
            {
                Muzzle = Instantiate(ShootFXFLash, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
                Muzzle.transform.parent = ShootFXPos.transform;
                Muzzle.SetActive(false);
            }

            if (GameObject.Find("PlayerCamera") != null)
            {
                playerCamera = GameObject.Find("PlayerCamera").GetComponent<TopDownCamera>();

            }
        

        }

   

        private void OnDestroy()
        {
            if (BulletsParent)
                Destroy(BulletsParent);
        }






        void AutoAim()
        {
            //Autoaim////////////////////////

            GameObject[] Enemys = GameObject.FindGameObjectsWithTag("Enemy");
            if (Enemys != null)
            {
                float BestDistance = 100.0f;

                foreach (GameObject Enemy in Enemys)
                {
                    Vector3 EnemyPos = Enemy.transform.position;
                    Vector3 EnemyDirection = EnemyPos - Player.transform.position;
                    float EnemyDistance = EnemyDirection.magnitude;

                    if (Vector3.Angle(Player.transform.forward, EnemyDirection) <= AutoAimAngle && EnemyDistance < AutoAimDistance)
                    {
                        //
                        if (Enemy.GetComponent<EnemyAI>().actualState != EnemyAI.AIState.Dead)
                        {
                            if (EnemyDistance < BestDistance)
                            {
                                BestDistance = EnemyDistance;
                                EnemyTargetAuto = EnemyPos + new Vector3(0, 1, 0);
                            }
                        }
                   

                    }
                }
            }

            if (EnemyTargetAuto != Vector3.zero)
            {
                FinalTarget = EnemyTargetAuto;
                ShootFXPos.transform.LookAt(FinalTarget);
            }
            else
            {
                ShootFXPos.transform.LookAt(ShootTarget.position);
                FinalTarget = ShootTarget.position;
            }

            //End of AutoAim
            /////////////////////////////////

        }

        void AIAutoAim()
        {
            //Autoaim////////////////////////

            Vector3 PlayerPos = AIEnemyTarget.position + new Vector3(0, 1.5f, 0);
            FinalTarget = PlayerPos;
        
      
        }

        public void PlayShootAudio()
        {
            if (ShootSFX.Length > 0)
            {
                int FootStepAudio = 0;

                if (ShootSFX.Length > 1)
                {
                    FootStepAudio = Random.Range(0, ShootSFX.Length);
                }

                float RandomVolume = Random.Range(0.6f, 1.0f);

                Audio.PlayOneShot(ShootSFX[FootStepAudio], RandomVolume);

            }
        }

        public void Shoot()
        {
            if (AIWeapon)
            {
                AIAutoAim();
            }
            else
            {
                AutoAim();
            }

            if (ActualBullets > 0)
                PlayShootAudio();
            //else
            //    Audio.PlayOneShot(ShootEmptySFX);
            float angleStep = radialAngleDirection / BulletsPerShoot;
            float finalAngle = 0.0f; 

            for (int i = 0; i < BulletsPerShoot; i++)
            {
            
                float FinalAccuracyModX = Random.Range(AccDiv, -AccDiv) * Vector3.Distance(Player.transform.position, FinalTarget);
                FinalAccuracyModX /= 100;

                float FinalAccuracyModY = Random.Range(AccDiv, -AccDiv) * Vector3.Distance(Player.transform.position, FinalTarget);
                FinalAccuracyModY /= 100;

                float FinalAccuracyModZ = Random.Range(AccDiv, -AccDiv) * Vector3.Distance(Player.transform.position, FinalTarget);
                FinalAccuracyModZ /= 100;
          
                Vector3 FinalOrientation = FinalTarget + new Vector3(FinalAccuracyModX, FinalAccuracyModY, FinalAccuracyModZ);

                ShootFXPos.transform.LookAt(FinalOrientation);

                if (BulletsPerShoot > 1 && radialAngleDirection > 0.0f)
                {
                    Quaternion aimLocalRot = Quaternion.Euler(0, finalAngle - (radialAngleDirection / 2) + (angleStep * 0.5f), 0);
                    ShootFXPos.transform.rotation = ShootFXPos.transform.rotation * aimLocalRot;

                    finalAngle += angleStep;
                }

                if ( BulletPrefab && ShootFXPos )
                {
                    if (ActualBullets > 0)
                    {
                        GameObject Bullet;
                        if (usePooling)
                        {
                            //Object Pooling Method 
                            Bullet = GameBullets[ActualGameBullet];
                            Bullet.transform.position = ShootFXPos.position;
                            Bullet.transform.rotation = ShootFXPos.rotation;
                            Bullet.GetComponent<Rigidbody>().isKinematic = false;
                            Bullet.GetComponent<Collider>().enabled = true;
                            Bullet.GetComponent<Bullet>().timeToLive = bulletTimeToLive;
                            Bullet.GetComponent<Bullet>().ResetPooling();
                            Bullet.SetActive(true);
                            ActualGameBullet += 1;
                            if (ActualGameBullet >= GameBullets.Length)
                                ActualGameBullet = 0;
                        }
                        else
                        {
                            Bullet = Instantiate(BulletPrefab, ShootFXPos.position, ShootFXPos.rotation);
                            Bullet.GetComponent<Bullet>().usePooling = false;
                            Bullet.SetActive(true);
                            Bullet.GetComponent<Rigidbody>().isKinematic = false;
                            Bullet.GetComponent<Collider>().enabled = true;
                            Bullet.GetComponent<Bullet>().timeToLive = bulletTimeToLive;
                        }
                        

                        //Object Pooling VFX
                        Muzzle.transform.rotation = transform.rotation;
                        EmitParticles(Muzzle);

                        //Generic 
                        Bullet.GetComponent<Bullet>().Damage = BulletDamage;
                        Bullet.GetComponent<Bullet>().temperatureMod = tempModFactor;
                        Bullet.GetComponent<Bullet>().BulletSpeed = BulletSpeed;
                        Bullet.GetComponent<Bullet>().BulletAccel = BulletAccel;
                        if (usePooling)
                            Bullet.transform.localScale = Bullet.GetComponent<Bullet>().OriginalScale * BulletSize;

                        ActualBullets -= 1;

                        if (playerCamera)
                        {
                            if (!AIWeapon)
                                playerCamera.Shake(shootShakeFactor, 0.2f);
                            else
                                playerCamera.Shake(shootShakeFactor * 0.5f, 0.2f);
                        }


                    }

                }


                EnemyTargetAuto = Vector3.zero;

            
            }
        }

        static void EmitParticles(GameObject VFXEmiiter)
        {
            VFXEmiiter.SetActive(true);
            VFXEmiiter.GetComponent<ParticleSystem>().Play();
        }


        public void AIAttackMelee(Vector3 playerPos, GameObject targetGO)
        {
            PlayShootAudio();

            //Object Pooling VFX
            if (Muzzle)
            {
                EmitParticles(Muzzle);
            }

            if (Vector3.Distance(playerPos + Vector3.up, ShootFXPos.position) <= MeleeRadius)
            {
                //Debug.Log("Hit Player Sucessfully");
                targetGO.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                targetGO.SendMessage("BulletPos", ShootFXPos.position, SendMessageOptions.DontRequireReceiver);
                targetGO.SendMessage("ApplyDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
            }
        }


    }
}
