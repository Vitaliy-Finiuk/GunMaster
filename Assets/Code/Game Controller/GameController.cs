using UnityEngine;

public class GameController : MonoBehaviour {

	[SerializeField]
	private WeaponSet _playerWeaponSet;

	void Awake() {
		_playerWeaponSet = FindObjectOfType<WeaponSet>();
		Instance = this;
		WeaponSet = _playerWeaponSet;
	}

	public static GameController Instance { get; private set; }

	public static WeaponSet WeaponSet;
	public static PauseGame pauseGame;
	//public static CursorController cursorController;
	public static WeaponGUI weaponGUI;

	public static void AddSectionStatic() {
		Instance.AddSection();
	}

	public static void RemoveSectionStatic() {
		Instance.RemoveSection();
	}

	public void AddSection() {
		WeaponSet.AddSection();
		weaponGUI.Refresh();
	}

	public void RemoveSection() {
		WeaponSet.RemoveSection();
		weaponGUI.Refresh();
	}
}
