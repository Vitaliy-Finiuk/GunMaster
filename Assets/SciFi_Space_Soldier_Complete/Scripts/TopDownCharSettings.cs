using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopDownCharSettings : MonoBehaviour
{
    [Header("Visuals")] public string characterName = "Stickman";

    [Header("Stats")] public int Health = 100;
    [HideInInspector] public int ActualHealth = 100;
    public bool oneShotHealth = false;

    [HideInInspector] public bool isDead = false;

    private bool Damaged = false;
    private float DamagedTimer = 0.0f;


    [Header("Weapons")] public bool alwaysAim = false;
    public int playerWeaponLimit = 2;
    public Weapon[] InitialWeapons;

    [HideInInspector] public bool Armed = true;
    [HideInInspector] public GameObject[] Weapon;
    [HideInInspector] public int[] actualWeaponTypes;
    [HideInInspector] public int ActiveWeapon = 0;
    private bool CanShoot = true;
    public WeaponList WeaponListObject;
    private GameObject[] WeaponList;
    public Transform WeaponR;
    public Transform WeaponL;

    public bool aimingIK = false;
    public bool useArmIK = true;

    [HideInInspector] public bool Aiming = false;

    private float FireRateTimer = 0.0f;
    private float LastFireTimer = 0.0f;

    private Transform AimTarget;

    [Header("VFX")] public GameObject DamageFX;
    private Vector3 LastHitPos = Vector3.zero;

    [Space] public GameObject damageSplatVFX;
    private BloodSplatter actualSplatVFX;


    [Header("Sound FX")] public float FootStepsRate = 0.4f;
    public float GeneralFootStepsVolume = 1.0f;
    public AudioClip[] Footsteps;
    private float LastFootStepTime = 0.0f;
    private AudioSource Audio;

    public bool UsingObject = false;


    public GameObject HUDHealthBar;
    public GameObject HUDDamageFullScreen;


    [HideInInspector] public TopDownCharController charController;
    [HideInInspector] public Animator charAnimator;

    private GameObject[] Canvases;

    //ArmIK
    private Transform ArmIKTarget = null;

    private CharacterIK CharacterIKController;


    private void Start()
    {
        Weapon = new GameObject[playerWeaponLimit];
        actualWeaponTypes = new int[playerWeaponLimit];

        WeaponList = WeaponListObject.weapons;

        ActualHealth = Health;

        Audio = GetComponent<AudioSource>() as AudioSource;
        charAnimator = GetComponent<Animator>();


        charController = GetComponent<TopDownCharController>();
        AimTarget = charController.AimFinalPos;
        HUDHealthBar.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);


        if (InitialWeapons.Length > 0)
        {
            InstantiateWeapons();

            Armed = true;
        }
        else
            Armed = false;

        Canvases = GameObject.FindGameObjectsWithTag("Canvas");
        if (Canvases.Length > 0)
            foreach (GameObject C in Canvases)
                UnparentTransforms(C.transform);

        if (alwaysAim)
        {
            Aiming = true;
            charAnimator.SetBool("Aiming", true);
        }

        InitializeHUD();


        if (damageSplatVFX)
        {
            GameObject GOactualSplatVFX =
                Instantiate(damageSplatVFX, transform.position, transform.rotation) as GameObject;
            GOactualSplatVFX.transform.position = transform.position;
            GOactualSplatVFX.transform.parent = transform;
            actualSplatVFX = GOactualSplatVFX.GetComponent<BloodSplatter>();
        }

        AIUpdatePlayerCount();


        if (oneShotHealth)
            ApplyOneShotHealth();
    }


    private void ApplyOneShotHealth()
    {
        ActualHealth = 1;
        HUDHealthBar.transform.parent.gameObject.SetActive(false);
    }

    public void AIUpdatePlayerCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            foreach (GameObject e in enemies)
                e.SendMessage("FindPlayers", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void UnparentTransforms(Transform Target) =>
        Target.SetParent(null);

    private void InstantiateWeapons()
    {
        int weapType = 0;
        foreach (Weapon Weap in InitialWeapons)
        {
            int weapInt = 0;

            foreach (GameObject weap in WeaponList)
            {
                if (Weap.gameObject.name == weap.name)
                {
                    actualWeaponTypes[weapType] = weapInt;
                    PickupWeapon(weapInt);
                }

                else
                    weapInt += 1;
            }


            weapType += 1;
        }
    }

    public void FootStep()
    {
        if (Footsteps.Length > 0 && Time.time >= (LastFootStepTime + FootStepsRate))
        {
            int FootStepAudio = 0;

            if (Footsteps.Length > 1)
                FootStepAudio = Random.Range(0, Footsteps.Length);

            float FootStepVolume = charAnimator.GetFloat("Speed") * GeneralFootStepsVolume;
            if (Aiming)
                FootStepVolume *= 0.5f;

            Audio.PlayOneShot(Footsteps[FootStepAudio], FootStepVolume);

            LastFootStepTime = Time.time;
        }
    }


    private void EnableArmIK(bool active)
    {
        if (CharacterIKController && useArmIK)
            if (Weapon[ActiveWeapon].GetComponent<Weapon>().useIK)
                CharacterIKController.ikActive = active;
            else
                CharacterIKController.ikActive = false;
    }


    private void Update()
    {
        if (Damaged)
        {
            DamagedTimer = Mathf.Lerp(DamagedTimer, 0.0f, Time.deltaTime * 10);

            if (Mathf.Approximately(DamagedTimer, 0.0f))
            {
                DamagedTimer = 0.0f;
                Damaged = false;
            }


            if (HUDDamageFullScreen)
                HUDDamageFullScreen.GetComponent<UnityEngine.UI.Image>().color =
                    new Vector4(1, 1, 1, DamagedTimer * 0.5f);
        }

        if (!isDead)
        {
            // Equip Weapon
            if (Input.GetButtonUp(charController.playerCtrlMap[6]) && Aiming == false)
            {
                if (Armed)
                    Armed = false;
                else
                    Armed = true;
                EquipWeapon(Armed);
            }

            // Shoot Weapons
            if (Input.GetAxis(charController.playerCtrlMap[4]) >= 0.5f ||
                Input.GetButton(charController.playerCtrlMap[15]))
            {
                if (CanShoot && Weapon[ActiveWeapon] != null && Time.time >= (LastFireTimer + FireRateTimer))
                {
                    //Ranged Weapon

                    if (Aiming)
                    {
                        LastFireTimer = Time.time;
                        Weapon[ActiveWeapon].GetComponent<Weapon>().Shoot();

                        if (Weapon[ActiveWeapon].GetComponent<Weapon>().ActualBullets > 0)
                            charAnimator.SetTrigger("Shoot");
                    }
                }
            }

            // Aim
            if (Input.GetButton(charController.playerCtrlMap[8]) ||
                Mathf.Abs(Input.GetAxis(charController.playerCtrlMap[2])) > 0.3f ||
                Mathf.Abs(Input.GetAxis(charController.playerCtrlMap[3])) > 0.3f)
            {
                if (!UsingObject && Armed)
                {
                    if (!alwaysAim)
                    {
                        Aiming = true;


                        charAnimator.SetBool("RunStop", false);
                        charAnimator.SetBool("Aiming", true);
                    }
                }
            }
            //Stop Aiming
            else if (Input.GetButtonUp(charController.playerCtrlMap[8]) ||
                     Mathf.Abs(Input.GetAxis(charController.playerCtrlMap[2])) < 0.3f ||
                     Mathf.Abs(Input.GetAxis(charController.playerCtrlMap[3])) < 0.3f)
            {
                if (!alwaysAim)
                {
                    Aiming = false;
                    charAnimator.SetBool("Aiming", false);
                }
            }


            if (alwaysAim)
            {
                if (!UsingObject)
                    Aiming = true;
                else
                    Aiming = false;
            }
        }
    }


    private void EquipWeapon(bool bArmed)
    {
        charAnimator.SetBool("Armed", bArmed);
        Weapon[ActiveWeapon].SetActive(bArmed);

        if (!bArmed)
        {
            int RifleActlLayer = charAnimator.GetLayerIndex("RifleActions");
            charAnimator.SetLayerWeight(RifleActlLayer, 0.0f);
            int PartialActions = charAnimator.GetLayerIndex("PartialActions");
            charAnimator.SetLayerWeight(PartialActions, 0.0f);

            if (CharacterIKController)
                CharacterIKController.enabled = false;
        }
        else
        {
            if (Weapon[ActiveWeapon].GetComponent<Weapon>().Type == global::Weapon.WT.Rifle)
            {
                int RifleActlLayer = charAnimator.GetLayerIndex("RifleActions");
                charAnimator.SetLayerWeight(RifleActlLayer, 1.0f);
            }

            int PartAct = charAnimator.GetLayerIndex("PartialActions");
            charAnimator.SetLayerWeight(PartAct, 1.0f);

            if (CharacterIKController)
                CharacterIKController.enabled = true;
        }

        EnableArmIK(bArmed);
    }

    private void StartUsingGeneric(string Type)
    {
        Aiming = false;
        UsingObject = true;

        charController.m_CanMove = false;
        charAnimator.SetTrigger(Type);


        EnableArmIK(false);
    }


    public void SpawnTeleportFX()
    {
        Damaged = true;
        DamagedTimer = 1.0f;
    }

    public void PickupWeapon(int WeaponType)
    {
        GameObject NewWeapon = Instantiate(WeaponList[WeaponType], WeaponR.position, WeaponR.rotation) as GameObject;
        NewWeapon.transform.parent = WeaponR.transform;
        NewWeapon.transform.localRotation = Quaternion.Euler(90, 0, 0);
        NewWeapon.name = "Player_" + NewWeapon.GetComponent<Weapon>().WeaponName;
        actualWeaponTypes[ActiveWeapon] = WeaponType;

        bool replaceWeapon = true;

        for (int i = 0; i < playerWeaponLimit; i++)
        {
            if (Weapon[i] == null)
            {
                Weapon[i] = NewWeapon;
                replaceWeapon = false;


                break;
            }
        }

        if (replaceWeapon)
        {
            DestroyImmediate(Weapon[ActiveWeapon]);
            Weapon[ActiveWeapon] = NewWeapon;
        }

        InitializeWeapons();
    }


    private void InitializeWeapons()
    {
        Weapon ActualW = Weapon[ActiveWeapon].GetComponent<Weapon>();
        Weapon[ActiveWeapon].SetActive(true);
        //

        ActualW.ShootTarget = AimTarget;
        ActualW.Player = this.gameObject;
        FireRateTimer = ActualW.FireRate;


        //ArmIK
        if (useArmIK)
        {
            if (ActualW.gameObject.transform.Find("ArmIK"))
            {
                ArmIKTarget = ActualW.gameObject.transform.Find("ArmIK");
                if (GetComponent<CharacterIK>() == null)
                {
                    gameObject.AddComponent<CharacterIK>();
                    CharacterIKController = GetComponent<CharacterIK>();
                }
                else if (GetComponent<CharacterIK>())
                {
                    CharacterIKController = GetComponent<CharacterIK>();
                }

                if (CharacterIKController)
                {
                    CharacterIKController.leftHandTarget = ArmIKTarget;
                    CharacterIKController.ikActive = true;
                }
            }
            else
            {
                if (CharacterIKController != null)
                    CharacterIKController.ikActive = false;
            }
        }


        ActualW.Audio = WeaponR.GetComponent<AudioSource>();

        if (ActualW.Type == global::Weapon.WT.Rifle)
        {
            int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
            charAnimator.SetLayerWeight(PistolLayer, 0.0f);
            int PistolActLayer = charAnimator.GetLayerIndex("PistolActions");
            charAnimator.SetLayerWeight(PistolActLayer, 0.0f);
            charAnimator.SetBool("Armed", true);
        }
    }

    private void InitializeHUD()
    {
        if (HUDDamageFullScreen)
            HUDDamageFullScreen.GetComponent<UnityEngine.UI.Image>().color = new Vector4(1, 1, 1, 0);

        if (charController.playerNmb > 0)
        {
            RectTransform HUDHealtRect = HUDHealthBar.transform.parent.GetComponent<RectTransform>();
        }
    }


    public void StopUse()
    {
        charController.m_CanMove = true;
        charAnimator.SetTrigger("StopUse");
        UsingObject = false;

        EnableArmIK(true);
    }


    public void BulletPos(Vector3 BulletPosition)
    {
        LastHitPos = BulletPosition;
        LastHitPos.y = 0;
    }

    public void SetNewSpeed(float speedFactor) =>
        charController.m_MoveSpeedSpecialModifier = speedFactor;

    public void SetHealth(int HealthInt)
    {
        ActualHealth = HealthInt;

        if (ActualHealth > 1)
            HUDHealthBar.GetComponent<RectTransform>().localScale =
                new Vector3(Mathf.Clamp((1.0f / Health) * ActualHealth, 0.1f, 1.0f), 1.0f, 1.0f);
        else
            HUDHealthBar.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 1.0f, 1.0f);
    }


    public void ApplyDamage(int Damage)
    {
        if (ActualHealth > 0)
        {
            SetHealth(ActualHealth - Damage);

            Damaged = true;
            DamagedTimer = 1.0f;

            if (actualSplatVFX)
            {
                actualSplatVFX.transform.LookAt(LastHitPos);
                actualSplatVFX.Splat();
            }

            if (ActualHealth <= 0)
            {
                if (actualSplatVFX)
                    actualSplatVFX.transform.parent = null;

                Die(false);
            }
        }
    }

    public void Die(bool temperatureDeath)
    {
        EnableArmIK(false);
        isDead = true;
        charAnimator.SetBool("Dead", true);

        charController.m_isDead = true;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

        this.tag = "Untagged";
        charController.playerSelection.enabled = false;

        DestroyHUD();


        SendMessageUpwards("PlayerDied", charController.playerNmb, SendMessageOptions.DontRequireReceiver);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject i in enemies)
        {
            i.SendMessage("FindPlayers", SendMessageOptions.DontRequireReceiver);
        }


        AIUpdatePlayerCount();

        Destroy(charController);
        Destroy(GetComponent<Collider>());
    }

    public void DestroyHUD()
    {
        if (HUDDamageFullScreen != null)
        {
            if (HUDDamageFullScreen.transform.parent.gameObject != null)
                Destroy(HUDDamageFullScreen.transform.parent.gameObject);
        }
    }
}