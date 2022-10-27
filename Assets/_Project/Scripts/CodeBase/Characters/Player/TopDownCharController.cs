using SimpleInputNamespace;
using UnityEngine;

namespace CodeBase.Characters.Player
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]

	public class TopDownCharController : MonoBehaviour {

		[Header("Multiplayer")]
		public int playerNmb = 1;
		public Renderer playerSelection;
		//Inputs
		[HideInInspector]
		public string[] playerCtrlMap = {"Horizontal", "Vertical", "LookX", "LookY","FireTrigger", "Aim", "Use", "Crouch", "ChangeWeapon"  ,"Fire"};

		[Header("Movement")]
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[HideInInspector]
		public float m_MoveSpeedSpecialModifier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		private float m_GroundCheckDistance = 0.25f;

		public bool useRootMotion = true;

		public float PlayerRunSpeed = 1f;
		public float PlayerAimSpeed = 1f;
		public float PlayerCrouchSpeed = 0.75f;

		public float RunRotationSpeed = 100f;
		public float AimingRotationSpeed = 25f;

		public float AnimatorRunDampValue = 0.25f;
		public float AnimatorSprintDampValue = 0.2f;
		public float AnimatorAimingDampValue = 0.1f;

		private Rigidbody _rigidbody;
		private Animator _charAnimator;
		private bool _isGrounded;
		private float _origGroundCheckDistance;
		private const float Half = 0.5f;
		private float _turnAmount;
		private float _forwardAmount;
		private float _capsuleHeight;
		private Vector3 _capsuleCenter;
		private CapsuleCollider _capsuleCollider;
		private bool _crouching;
		private bool _crouch = false;

		private bool _canRotate = true;
		[HideInInspector] public bool Sprinting = false;


		[HideInInspector] public bool _isDead = false;
		[HideInInspector] public bool m_CanMove = true;

		[Header("Aiming")]
		public GameObject AimTargetVisual;
		public Transform AimFinalPos;
		public TopDownCamera CamScript;

		private Transform m_Cam;                  // A reference to the main camera in the scenes transform
		[HideInInspector]
		public Vector3 _Move;					  // the world-relative desired move direction, calculated from the camForward and user input.
		private Vector3 _smoothMove;

		[Header("Mobile Joysctick")]
		[SerializeField] private Joystick _joystick;
		private GameObject _joystickTarget;
		private GameObject _joystickLookRot;
		public bool JoystickEnabled = true;

		private TopDownCharSettings _settings;


		private void Start()
		{
			_settings = GetComponent<TopDownCharSettings>();

			_joystickTarget = new GameObject();
			_joystickTarget.name = "JoystickTarget";
			_joystickTarget.transform.position = transform.position;
			_joystickTarget.transform.parent = transform.parent;

			_joystickLookRot = new GameObject();
			_joystickLookRot.name = "JoystickLookRotation";
			_joystickLookRot.transform.position = transform.position;
			_joystickLookRot.transform.parent = transform;

			if (Camera.main != null)
				m_Cam = CamScript.transform.GetComponentInChildren<Camera>().transform;

			_charAnimator = GetComponent<Animator>();
			_rigidbody = GetComponent<Rigidbody>();
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_capsuleHeight = _capsuleCollider.height;
			_capsuleCenter = _capsuleCollider.center;

			_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			_origGroundCheckDistance = m_GroundCheckDistance;

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
				_canRotate = false;

				_charAnimator.SetTrigger("GameOver");
				_settings.isDead = true;
			}
		}
		
		public void ActivateJoystick(bool IsOn)
		{
			if (IsOn)
				AimTargetVisual.SetActive(false);
			else
				AimTargetVisual.SetActive(true);
		}


		private void Update () {

			MouseTargetPos();

			if (!_isDead && m_CanMove)
			{
           
				float h = _joystick.xAxis.value;
				float v =  _joystick.yAxis.value;

				if (_crouch && _settings.Aiming)
				{
					h = 0;
					v = 0;
				}

				if (_canRotate)
				{
					if (_settings.Aiming )
					{
						if (!JoystickEnabled)
							MouseAim(AimFinalPos.position);
						else
							JoystickLook(h, v);
					}
					else
						RunningLook(new Vector3(h, 0, v));
				}

				_Move = new Vector3(h, 0, v);// * m_MoveSpeedSpecialModifier;
            

				_Move = _Move.normalized * m_MoveSpeedSpecialModifier;
				_Move = Quaternion.Euler(0, 0 - transform.eulerAngles.y + m_Cam.transform.parent.transform.eulerAngles.y, 0) * _Move;

				Move(_Move, _crouch);
			}
			else
			{
				_forwardAmount = 0.0f;
				_turnAmount = 0.0f;
				_settings.Aiming = false;
				UpdateAnimator(Vector3.zero);
			}
        
		}
	

		private void JoystickLook(float h, float v)
		{
			_joystickTarget.transform.rotation = transform.rotation;
        
			//Joystick Look input
			float LookX = Input.GetAxis(playerCtrlMap[2]);
			float LookY = Input.GetAxis(playerCtrlMap[3]);
                
			Vector3 JoystickLookVec = new Vector3(LookX, 0, LookY) * 10;
       
			JoystickLookVec = Quaternion.Euler(0, 0 + m_Cam.transform.parent.transform.eulerAngles.y, 0) * JoystickLookVec;
        
			_joystickTarget.transform.position = transform.position + JoystickLookVec * 5;

			if (Mathf.Abs(LookX) <= 0.2f && Mathf.Abs(LookY) <= 0.2f)
			{
				_joystickTarget.transform.localPosition += _joystickTarget.transform.forward * 2;
			}

			_joystickLookRot.transform.LookAt(_joystickTarget.transform.position);

			AimTargetVisual.transform.position = _joystickTarget.transform.position;
			AimTargetVisual.transform.LookAt(transform.position);

			transform.rotation = Quaternion.Lerp(transform.rotation, _joystickLookRot.transform.rotation, Time.deltaTime * AimingRotationSpeed);
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		}

		#region Movement
		private void RunningLook(Vector3 Direction)
		{
			if (Direction.magnitude >= 0.25f)
			{
				Direction = Quaternion.Euler(0, 0 + m_Cam.transform.parent.transform.eulerAngles.y, 0) * Direction;

				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Direction), Time.deltaTime * RunRotationSpeed);

				transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
			}
		}

		public void Move(Vector3 move, bool crouch)
		{
			if (!_settings.UsingObject)
			{
				CheckGroundStatus();

				_turnAmount = move.x;
				_forwardAmount = move.z;

       

				ScaleCapsuleForCrouching(crouch);
				PreventStandingInLowHeadroom();

				UpdateAnimator(move);
			}
		
		}

		private void ScaleCapsuleForCrouching(bool crouch)
		{
			if (_isGrounded && crouch)
			{
				if (_crouching) return;
				_capsuleCollider.height = _capsuleCollider.height / 1.5f;
				_capsuleCollider.center = _capsuleCollider.center / 1.5f;
				_crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(_rigidbody.position + Vector3.up * _capsuleCollider.radius * Half, Vector3.up);
				float crouchRayLength = _capsuleHeight - _capsuleCollider.radius * Half;
				if (Physics.SphereCast(crouchRay, _capsuleCollider.radius * Half, crouchRayLength))
				{
					_crouching = true;
					return;
				}
				_capsuleCollider.height = _capsuleHeight;
				_capsuleCollider.center = _capsuleCenter;
				_crouching = false;
			}
		}

		private void PreventStandingInLowHeadroom()
		{
			if (!_crouching)
			{
				Ray crouchRay = new Ray(_rigidbody.position + Vector3.up * _capsuleCollider.radius * Half, Vector3.up);
				float crouchRayLength = _capsuleHeight - _capsuleCollider.radius * Half;
				if (Physics.SphereCast(crouchRay, _capsuleCollider.radius * Half, crouchRayLength)) 
					_crouching = true;
			}
		}

		private void CheckGroundStatus()
		{
			RaycastHit hitInfo;

			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				_isGrounded = true;
				_charAnimator.applyRootMotion = true;
			}
			else
			{
				_isGrounded = false;
				_charAnimator.applyRootMotion = false;
			}
		}
		#endregion

		#region Mouse

		private void MouseTargetPos()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 2000f, 9)) {
				Vector3 FinalPos = new Vector3( hit.point.x, 0,hit.point.z);

				AimTargetVisual.transform.position = FinalPos;
				AimTargetVisual.transform.LookAt(transform.position);
			
			}
		}
	
		private void MouseAim(Vector3 FinalPos)
		{
			_joystickLookRot.transform.LookAt(FinalPos);
			transform.rotation = Quaternion.Lerp(transform.rotation, _joystickLookRot.transform.rotation, Time.deltaTime * AimingRotationSpeed);
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

		}

		#endregion

		#region Animator

		private void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			_charAnimator.SetFloat("Y", _forwardAmount, AnimatorAimingDampValue, Time.deltaTime);
			_charAnimator.SetFloat("X", _turnAmount, AnimatorAimingDampValue, Time.deltaTime);
                
			if (!Sprinting)
				_charAnimator.SetFloat("Speed", move.magnitude, AnimatorSprintDampValue, Time.deltaTime);
			else
				_charAnimator.SetFloat("Speed", 2.0f, AnimatorRunDampValue, Time.deltaTime);

			_charAnimator.SetBool("Crouch", _crouching);
			_charAnimator.SetBool("OnGround", _isGrounded);
			
			if (_isGrounded && move.magnitude > 0)
			{
            
				if (_settings.Aiming )
				{
					move *= PlayerAimSpeed;
					transform.Translate(move * Time.deltaTime);
					_charAnimator.applyRootMotion = false;
				}
				else if (_settings.UsingObject)
				{
					move = move * 0.0f;
					transform.Translate(Vector3.zero);
					_charAnimator.applyRootMotion = false;
				}
				else
				{
					if (useRootMotion)
						_charAnimator.applyRootMotion = true;
					else
					{
						if (_crouch)
							move *= PlayerCrouchSpeed;
						else
							move *= PlayerRunSpeed;

						transform.Translate(move * Time.deltaTime );
						_charAnimator.applyRootMotion = false;
					}
				}

				_charAnimator.speed = m_AnimSpeedMultiplier ;
			}
			else
				_charAnimator.speed = 1;
		}
	
		public void OnAnimatorMove()
		{
			if (_isGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (_charAnimator.deltaPosition * m_MoveSpeedMultiplier ) / Time.deltaTime;
			
				v.y = _rigidbody.velocity.y;
				_rigidbody.velocity = v;
			}
		}

		#endregion
	}
}
