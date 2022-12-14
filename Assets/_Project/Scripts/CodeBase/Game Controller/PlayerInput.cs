using UnityEngine;

namespace CodeBase.Game_Controller
{
	public class PlayerInput : MonoBehaviour{

		public static PlayerInput Instance { get; private set; }

		public static bool Fire1;

		private void Awake() => 
			Instance = this;

		private void Update() {
			ReadCharacterInput();
			ReadWeaponInput();
		}

		private void ReadCharacterInput() {
		}

		private void ReadWeaponInput() => 
			Fire1 = Input.GetButton("Fire");

		public static void Enable() => 
			Instance.enabled = true;

		public static void Disable() => 
			Instance.enabled = false;

		public static void SetEnabled(bool enabled) => 
			Instance.enabled = enabled;
	}
}
