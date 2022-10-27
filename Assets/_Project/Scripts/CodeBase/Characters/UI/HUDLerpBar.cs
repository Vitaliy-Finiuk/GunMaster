using UnityEngine;

namespace _Project.Scripts.CodeBase.Characters.UI
{
    public class HUDLerpBar : MonoBehaviour {
        [SerializeField] private GameObject _referenceGo;
        [SerializeField] private float _speed = 1.0f;
        private Vector3 ActualScale = Vector3.one; 

        private void Start () {
            if (_referenceGo)
                GetComponent<RectTransform>().localScale = _referenceGo.GetComponent<RectTransform>().localScale;
        }
	
        private void Update () {
            ActualScale = Vector3.Lerp(ActualScale, _referenceGo.GetComponent<RectTransform>().localScale, Time.deltaTime * _speed);
            GetComponent<RectTransform>().localScale = ActualScale;
        }
    }
}
