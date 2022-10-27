using CodeBase.UI;
using UnityEngine;

namespace CodeBase.Game_Controller
{
	public class PauseGame : MonoBehaviour {

		public bool Paused { get; private set; }

		public UIController UIController;

		private void Awake() => 
			GameController.PauseGame = this;

		private void Update() {
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
