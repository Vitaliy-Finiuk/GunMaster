using UnityEngine;

public class PlayerInput : MonoBehaviour{

	public static PlayerInput Instance { get; private set; }

	public static float Horizontal;
	public static float Vertical;
	public static float MouseX;
	public static float MouseY;
	public static bool Jump;
	public static bool Sprint;

	public static bool Fire1;

	void Awake() {
		Instance = this;
	}

	void Update() {
		ReadCharacterInput();
		ReadWeaponInput();
	}

	private void ReadCharacterInput() {
	}

	private void ReadWeaponInput() {
		
		Fire1 = Input.GetButton("Fire");
	}

	public static void Enable() {
		Instance.enabled = true;
	}

	public static void Disable() {
		Instance.enabled = false;
	}

	public static void SetEnabled(bool enabled) {
		Instance.enabled = enabled;
	}
}
