using UnityEngine;

namespace CodeBase.Characters.Player
{
    public class TopDownCamera : MonoBehaviour {

        [Header("Camera Basic Settings")] 
        [SerializeField] private float _followSpeed = 4.0f;
        [SerializeField] private Transform _targetToFollow;

        [HideInInspector]
        [SerializeField] float _targetHeight = 0.0f;
        [HideInInspector]
        [SerializeField] private float _targetHeightOffset = -1.0f;
        [HideInInspector]
        [SerializeField] private float _actualHeight = 0.0f;
   
        [SerializeField] private float _heightSpeed = 1.0f;
        [SerializeField] private Transform _cameraOffset;

        [Header("Shooting Shake Settings")]
        [SerializeField] private bool _isShaking = false;
        [SerializeField] private float _shakeFactor = 3f;
        [SerializeField] private float _shakeTimer = .2f;
        [SerializeField] private float _shakeSmoothness = 5f;
        [HideInInspector]
        public float actualShakeTimer = 0.2f;

        [Header("Explosion Shake Settings")]
        [SerializeField] private bool _isExpShaking = false;
        [SerializeField] private float _shakeExpFactor = 5f;
        [SerializeField] private float _shakeExpTimer = 1.0f;
        [SerializeField] private float _shakeExpSmoothness = 3f;
        
        [HideInInspector]
        public float CurrentExpShakeTimer = 1.0f;

        [Header("Movement Shake Settings")]
        private Vector3 randomShakePos = Vector3.zero;

        private bool showBlood = true;

        private void Start () => 
            actualShakeTimer = _shakeTimer;


        public Vector3 CalculateRandomShake(float shakeFac, bool isExplosion)
        {
            randomShakePos = new Vector3(Random.Range(-shakeFac, shakeFac), Random.Range(-shakeFac, shakeFac), Random.Range(-shakeFac, shakeFac));
            if (isExplosion)
                return randomShakePos * (CurrentExpShakeTimer / _shakeExpTimer);
            else
                return randomShakePos * (actualShakeTimer / _shakeTimer);
        }

        public void Shake(float factor, float duration)
        {
            _isShaking = true;
            _shakeFactor = factor;
            _shakeTimer = duration;
            actualShakeTimer = _shakeTimer;
        }

        public void ExplosionShake(float factor, float duration)
        {
            _isExpShaking = true;
            _shakeExpFactor = factor;
            _shakeExpTimer = duration;
            CurrentExpShakeTimer = _shakeExpTimer;
        }

        private void LateUpdate()
        {
            if (_targetToFollow)
            {
                transform.position = Vector3.Lerp(transform.position, _targetToFollow.position, _followSpeed * Time.deltaTime);

                _actualHeight = Mathf.Lerp(_actualHeight, _targetHeight + _targetHeightOffset, Time.deltaTime * _heightSpeed);

                _cameraOffset.localPosition = new Vector3(0.0f, 0.0f, _actualHeight);
            }

            if (_isShaking && !_isExpShaking)
            {
                if (actualShakeTimer >= 0.0f)
                {
                    actualShakeTimer -= Time.deltaTime;
                    Vector3 newPos = transform.localPosition + CalculateRandomShake(_shakeFactor, false);
                    transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, _shakeSmoothness * Time.deltaTime);
                }
                else
                {
                    _isShaking = false;
                    actualShakeTimer = _shakeTimer;
                }
            }

            else if (_isExpShaking)
            {
                if (CurrentExpShakeTimer >= 0.0f)
                {
                    CurrentExpShakeTimer -= Time.deltaTime;
                    Vector3 newPos = transform.localPosition + CalculateRandomShake(_shakeExpFactor, true);
                    transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, _shakeExpSmoothness * Time.deltaTime);
                }
                else
                {
                    _isExpShaking = false;
                    CurrentExpShakeTimer = _shakeExpTimer;
                }
            }

        }

        public void EnableBlood()
        {
            if (showBlood)
            {
                var newMask = GetComponentInChildren<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("Blood"));
                GetComponentInChildren<Camera>().cullingMask = newMask;
                showBlood = false;
            }
            else if (!showBlood)
            {
                var newMask = GetComponentInChildren<Camera>().cullingMask | (1 << LayerMask.NameToLayer("Blood"));
                GetComponentInChildren<Camera>().cullingMask = newMask;
                showBlood = true;
            }
        
        }
    }
}
