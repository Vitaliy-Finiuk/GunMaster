using _Project.Scripts.CodeBase.UI;
using UnityEngine;

namespace _Project.Scripts.CodeBase.Game_Controller
{
	public class PauseGame : MonoBehaviour {

		public bool Paused { get; private set; }

		public UIController UIController;

		void Awake() {
			GameController.pauseGame = this;
		}

		void Update() {
			if (Input.GetButtonDown("Use"))
				TogglePause();
		}

		public void TogglePause() {
			Pause(!Paused);
		}

		public void Pause(bool paused) {
			Paused = paused;
			if (paused) {
				Time.timeScale = 0.01f;
				UIController.Show();
				PlayerInput.Disable();
			}
			else {
				Time.timeScale = 1;
				UIController.Hide();
				PlayerInput.Enable();
			}
		}
	}
}
