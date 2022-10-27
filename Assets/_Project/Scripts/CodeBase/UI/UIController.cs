using UnityEngine;

namespace _Project.Scripts.CodeBase.UI
{
	public class UIController : MonoBehaviour {

		void Start() {
			Hide();
		}

		public void Show() {
			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}
	}
}