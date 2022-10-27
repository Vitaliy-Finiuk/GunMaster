using CodeBase.Game_Controller;
using CodeBase.WeaponsSystem.Weapons.Core;
using UnityEngine;

namespace CodeBase.UI.Weapon_GUI
{
	public class WeaponGUI : MonoBehaviour {
	
		public Transform sectionsObject;
		public WeaponSectionGUI sectionGUIPrefab;
		public WeaponModule[] availableModules;

		private Transform[] _sectionGUIs;

		void Awake() {
			GameController.weaponGUI = this;
		}

		void OnEnable() {
			Refresh();
		}

		public void Refresh() {
			if (GameController.WeaponSet == null || GameController.WeaponSet.sections[0].WeaponSet == null)
				return;

			if (_sectionGUIs != null) {
				foreach (Transform section in _sectionGUIs)
					Destroy(section.gameObject);
			}

			_sectionGUIs = new Transform[GameController.WeaponSet.SectionCount];
			for (int i = 0; i < GameController.WeaponSet.SectionCount; i++) {
				CreateSectionGUI(GameController.WeaponSet.sections[i], i);
			}
		}

		private void CreateSectionGUI(WeaponSection section, int index) {
			WeaponSectionGUI sectionGUI = Instantiate(sectionGUIPrefab);
			_sectionGUIs[index] = sectionGUI.transform;
			_sectionGUIs[index].SetParent(sectionsObject, false);
			_sectionGUIs[index].SetAsLastSibling();
			sectionGUI.SetSection(section, index, availableModules);
		}
	}
}