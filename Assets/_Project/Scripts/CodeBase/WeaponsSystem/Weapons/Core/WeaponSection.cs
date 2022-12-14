using System;
using UnityEngine;

namespace CodeBase.WeaponsSystem.Weapons.Core
{
	[Serializable]
	public class WeaponSection {

		public WeaponSet WeaponSet { get; private set; }
		public WeaponSection NextSection { get; private set; }
	
		public WeaponModule TransitionModule { get; private set; }
		public WeaponModule ProjectileModule { get; private set; }
		public WeaponModule[] UpgradeModules { get; private set; }

		[SerializeField]
		private WeaponModule transitionModule;
		[SerializeField]
		private WeaponModule projectileModule;
		[SerializeField]
		private WeaponModule[] upgradeModules;

		public WeaponSection() { }

		public WeaponSection(WeaponSection copy) : this() {
			transitionModule = copy.transitionModule;
			projectileModule = copy.projectileModule;
			upgradeModules = copy.upgradeModules;
		}

		public void AssignWeapon(WeaponSet weaponSet, WeaponSection nextSection = null) {
			WeaponSet = weaponSet;
			NextSection = nextSection;
			SetTransitionModule(transitionModule);
			SetProjectileModule(projectileModule);
			SetUpgradeModules(upgradeModules);
		}

		public void SetTransitionModule(WeaponModule modulePrefab) {
			if (TransitionModule != null)
				GameObject.Destroy(TransitionModule.gameObject);

			TransitionModule = GameObject.Instantiate<WeaponModule>(modulePrefab);
			TransitionModule.transform.SetParent(WeaponSet.ModuleParent, false);
			TransitionModule.WeaponSection = this;
		}

		public void SetProjectileModule(WeaponModule modulePrefab) {
			if (ProjectileModule != null)
				GameObject.Destroy(ProjectileModule.gameObject);

			ProjectileModule = GameObject.Instantiate<WeaponModule>(modulePrefab);
			ProjectileModule.transform.SetParent(WeaponSet.ModuleParent, false);
			ProjectileModule.WeaponSection = this;
		}

		public void SetUpgradeModules(WeaponModule[] modulePrefabs) {
			if (UpgradeModules != null)
				foreach (WeaponModule upgradeModule in UpgradeModules)
					GameObject.Destroy(upgradeModule.gameObject);

			UpgradeModules = new WeaponModule[modulePrefabs.Length];
			for (int i = 0; i < modulePrefabs.Length; i++) {
				UpgradeModules[i] = GameObject.Instantiate<WeaponModule>(modulePrefabs[i]);
				UpgradeModules[i].transform.SetParent(WeaponSet.ModuleParent, false);
				UpgradeModules[i].WeaponSection = this;
			}
		}
	}
}