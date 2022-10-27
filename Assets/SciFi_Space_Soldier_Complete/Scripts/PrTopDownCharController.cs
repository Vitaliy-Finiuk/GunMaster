﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using SimpleInputNamespace;

//using PrUtils;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

public class PrTopDownCharController : MonoBehaviour {

    [Header("Multiplayer")]
    public int playerNmb = 1;
    public Renderer playerSelection;
    //Inputs
    [HideInInspector]
    public string[] playerCtrlMap = {"Horizontal", "Vertical", "LookX", "LookY","FireTrigger", "Reload",
        "EquipWeapon", "Sprint", "Aim", "ChangeWTrigger", "Roll", "Use", "Crouch", "ChangeWeapon", "Throw"  ,"Fire", "Mouse ScrollWheel"};

    [Header("Movement")]
    [SerializeField] float m_JumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
    [SerializeField] float m_MoveSpeedMultiplier = 1f;
    [HideInInspector]
    public float m_MoveSpeedSpecialModifier = 1f;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    private float m_GroundCheckDistance = 0.25f;

    public bool useRootMotion = true;

    public float PlayerRunSpeed = 1f;
    public float PlayerAimSpeed = 1f;
    public float PlayerSprintSpeed = 1f;
    public float PlayerCrouchSpeed = 0.75f;

    public float RunRotationSpeed = 100f;
    public float AimingRotationSpeed = 25f;

    public float AnimatorRunDampValue = 0.25f;
    public float AnimatorSprintDampValue = 0.2f;
    public float AnimatorAimingDampValue = 0.1f;

    Rigidbody m_Rigidbody;
    Animator charAnimator;
    bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    CapsuleCollider m_Capsule;
    bool m_Crouching;
    private bool crouch = false;

    private bool b_CanRotate = true;
    private bool m_Jump;
    private float lastJump = 0.0f;
    private bool b_canJump = true;
    [HideInInspector] public bool Sprinting = false;


    [HideInInspector] public bool m_isDead = false;
    [HideInInspector] public bool m_CanMove = true;

    [Header("Aiming")]
    public GameObject AimTargetVisual;
    public Transform AimFinalPos;
    public PrTopDownCamera CamScript;

    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    [HideInInspector]
    public Vector3 m_Move;					  // the world-relative desired move direction, calculated from the camForward and user input.
    private Vector3 smoothMove;

    [Header("Joystick / Keyboard")]
    public bool JoystickEnabled = true;
    private GameObject JoystickTarget;
    private GameObject JoystickLookRot;

    [Header("Mobile Joysctick")]
    [SerializeField] private Joystick _joystick;
    
   

    private PrTopDownCharInventory Inventory;


    void Start()
    {
        Inventory = GetComponent<PrTopDownCharInventory>();

        JoystickTarget = new GameObject();
        JoystickTarget.name = "JoystickTarget";
        JoystickTarget.transform.position = transform.position;
        JoystickTarget.transform.parent = transform.parent;

        JoystickLookRot = new GameObject();
        JoystickLookRot.name = "JoystickLookRotation";
        JoystickLookRot.transform.position = transform.position;
        JoystickLookRot.transform.parent = transform;

        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = CamScript.transform.GetComponentInChildren<Camera>().transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        charAnimator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;



        if (JoystickEnabled)
            ActivateJoystick(true);
        else
            ActivateJoystick(false);



    }

    public void StopMoving(string Case)
    {
        if (Case == "GameOver")
        {
            m_CanMove = false;
            b_CanRotate = false;

            charAnimator.SetTrigger("GameOver");
            Inventory.isDead = true;
        }
    }



  
    




    public void ActivateJoystick(bool IsOn)
    {
        if (IsOn)
            AimTargetVisual.SetActive(false);
        else
            AimTargetVisual.SetActive(true);
    }




    // Update is called once per frame
    void Update () {

        //Time.timeScale = Slowmotion;

        if (Input.GetKeyDown(KeyCode.K) && playerNmb <= 1)
        {
            if (JoystickEnabled)
                JoystickEnabled = false;
            else
                JoystickEnabled = true;

            ActivateJoystick(JoystickEnabled);
        }

        MouseTargetPos();

        if (!m_isDead && m_CanMove)
		{
           
            //Crouch
            if (Input.GetKey(KeyCode.LeftControl) && playerNmb <= 1|| Input.GetButton(playerCtrlMap[12]))
            {
                print(playerCtrlMap[13]);
                crouch = true;
               
            }            	
            else
            {
                crouch = false;
            }	

            float h = _joystick.xAxis.value;
            float v =  _joystick.yAxis.value;

            if (crouch && Inventory.Aiming)
            {
                h = 0;
                v = 0;
            }

            



            if (b_CanRotate)
            {
                if (Inventory.Aiming )
                {
                    if (!JoystickEnabled)
                        MouseAim(AimFinalPos.position);
                    else
                        JoystickLook(h, v);
                }
                else
                {
                    RunningLook(new Vector3(h, 0, v));
                }
            }

            m_Move = new Vector3(h, 0, v);// * m_MoveSpeedSpecialModifier;
            

            m_Move = m_Move.normalized * m_MoveSpeedSpecialModifier;
            //Rotate move in camera space
            m_Move = Quaternion.Euler(0, 0 - transform.eulerAngles.y + m_Cam.transform.parent.transform.eulerAngles.y, 0) * m_Move;

            //Move Player
            Move(m_Move, crouch, m_Jump);
			m_Jump = false;


            
        }
        else
        {
            m_ForwardAmount = 0.0f;
            m_TurnAmount = 0.0f;
            Inventory.Aiming = false;
            UpdateAnimator(Vector3.zero);
        }
        
    }
	
    private void RunningLook(Vector3 Direction)
    {
        if (Direction.magnitude >= 0.25f)
        {
            Direction = Quaternion.Euler(0, 0 + m_Cam.transform.parent.transform.eulerAngles.y, 0) * Direction;


                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Direction), Time.deltaTime * RunRotationSpeed);

            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
       
    }

    private void JoystickLook(float h, float v)
    {
        JoystickTarget.transform.rotation = transform.rotation;
        
        //Joystick Look input
        float LookX = Input.GetAxis(playerCtrlMap[2]);
        float LookY = Input.GetAxis(playerCtrlMap[3]);
                
        Vector3 JoystickLookVec = new Vector3(LookX, 0, LookY) * 10;
       
        JoystickLookVec = Quaternion.Euler(0, 0 + m_Cam.transform.parent.transform.eulerAngles.y, 0) * JoystickLookVec;
        
        JoystickTarget.transform.position = transform.position + JoystickLookVec * 5;

        if (Mathf.Abs(LookX) <= 0.2f && Mathf.Abs(LookY) <= 0.2f)
        {
            JoystickTarget.transform.localPosition += JoystickTarget.transform.forward * 2;
        }

        JoystickLookRot.transform.LookAt(JoystickTarget.transform.position);

        AimTargetVisual.transform.position = JoystickTarget.transform.position;
        AimTargetVisual.transform.LookAt(transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, JoystickLookRot.transform.rotation, Time.deltaTime * AimingRotationSpeed);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        

    }

    
    private void MouseTargetPos()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 2000f, 9)) {
			//Debug.Log("---------- Hit Something------------");
			Vector3 FinalPos = new Vector3( hit.point.x, 0,hit.point.z);

			AimTargetVisual.transform.position = FinalPos;
			AimTargetVisual.transform.LookAt(transform.position);
			
		}
	}
	
    private void MouseAim(Vector3 FinalPos)
    {
        JoystickLookRot.transform.LookAt(FinalPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, JoystickLookRot.transform.rotation, Time.deltaTime * AimingRotationSpeed);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

    }

    void PlayFootStepAudio()
	{
		
	}
	
	public void Move(Vector3 move, bool crouch, bool jump)
	{
        if (!Inventory.UsingObject)
        {
            CheckGroundStatus();

            m_TurnAmount = move.x;
            m_ForwardAmount = move.z;

       

            ScaleCapsuleForCrouching(crouch);
            PreventStandingInLowHeadroom();

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }
		
	}
	
	
	void ScaleCapsuleForCrouching(bool crouch)
	{
		if (m_IsGrounded && crouch)
		{
			if (m_Crouching) return;
			m_Capsule.height = m_Capsule.height / 1.5f;
			m_Capsule.center = m_Capsule.center / 1.5f;
			m_Crouching = true;
		}
		else
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength))
			{
				m_Crouching = true;
				return;
			}
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
			m_Crouching = false;
		}
	}
	
	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!m_Crouching)
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength))
			{
				m_Crouching = true;
			}
		}
	}
	
	
	void UpdateAnimator(Vector3 move)
	{
         // update the animator parameters
        charAnimator.SetFloat("Y", m_ForwardAmount, AnimatorAimingDampValue, Time.deltaTime);
		charAnimator.SetFloat("X", m_TurnAmount, AnimatorAimingDampValue, Time.deltaTime);
                
        if (!Sprinting)
            charAnimator.SetFloat("Speed", move.magnitude, AnimatorSprintDampValue, Time.deltaTime);
        else
            charAnimator.SetFloat("Speed", 2.0f, AnimatorRunDampValue, Time.deltaTime);

        charAnimator.SetBool("Crouch", m_Crouching);
		charAnimator.SetBool("OnGround", m_IsGrounded);
			
		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (m_IsGrounded && move.magnitude > 0)
		{
            
            if (Inventory.Aiming )
            {
                move *= PlayerAimSpeed;
                transform.Translate(move * Time.deltaTime);
                charAnimator.applyRootMotion = false;
            }
            else if (Inventory.UsingObject)
            {
                move = move * 0.0f;
                transform.Translate(Vector3.zero);
                charAnimator.applyRootMotion = false;
            }
            else
            {
                if (useRootMotion)
                    charAnimator.applyRootMotion = true;
                else
                {
     
                        if (Sprinting)
                        {
                            move *= PlayerSprintSpeed;
                        }
                        else if (crouch)
                        {
                            move *= PlayerCrouchSpeed;
                        }
                        else 
                        {
                            move *= PlayerRunSpeed;
                        }

                        transform.Translate(move * Time.deltaTime );
                        charAnimator.applyRootMotion = false;
                    
                    
                }
            }

            charAnimator.speed = m_AnimSpeedMultiplier ;
		}
		else
		{
			// don't use that while airborne
			charAnimator.speed = 1;
		}
	}
	
	
	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);
		
		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
	}
	
	

		
	//This function it´s used only for Aiming and Jumping states. Those anims doesn´t have root motion so we move the player by script
	public void OnAnimatorMove()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (m_IsGrounded && Time.deltaTime > 0)
		{
			Vector3 v = (charAnimator.deltaPosition * m_MoveSpeedMultiplier ) / Time.deltaTime;
			
			// we preserve the existing y part of the current velocity.
			v.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = v;
		}
	}

	void CheckGroundStatus()
	{
		RaycastHit hitInfo;

		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			m_IsGrounded = true;
			charAnimator.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			charAnimator.applyRootMotion = false;
		}
	}
    

}
