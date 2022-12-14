using UnityEngine;

namespace CodeBase.UI
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