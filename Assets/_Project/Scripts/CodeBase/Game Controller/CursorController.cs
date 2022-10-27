/*using UnityEngine;

public class CursorController : MonoBehaviour {
	
	public bool Hidden { get; private set; }

	/*void Awake() {
		GameController.cursorController = this;
	}#1#

	void Start() {
		//ShowCursor(false);
	}

	/*
	void Update() {
		if (Input.GetButtonDown("Use"))
			ToggleCursor();
	}
	#1#

	public void ToggleCursor() {
		//ShowCursor(Hidden);
	}

	/*public void ShowCursor(bool show) {
		if (show) {
			//Cursor.lockState = CursorLockMode.None;
			/*Cursor.visible = true;
			Hidden = false;#2#
		}
		else {
			//Cursor.lockState = CursorLockMode.Locked;
			/*Cursor.visible = false;
			Hidden = true;#2#
		}#1#
	}
}*/