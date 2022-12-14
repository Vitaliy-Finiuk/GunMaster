using System;
using System.Collections.Generic;
using CodeBase.Characters.Player;
using CodeBase.Game_Controller;
using CodeBase.WeaponsSystem.Weapons.Util;
using UnityEngine;

namespace CodeBase.WeaponsSystem.Weapons.Core
{
	[Serializable]
	public class TransformSequence : Sequence<Transform> { }

	public class WeaponSet : MonoBehaviour
	{

		public TopDownCharSettings CharController;
		public int SectionCount { get { return sections.Count; } }
		public List<WeaponProjectile> Projectiles { get; private set; }
		public Transform ModuleParent { get { return CreateModuleParent(); } }

		public List<WeaponSection> sections;
		public TransformSequence firePoints;

		private Transform _transform;

		private Transform _moduleParent;

		private void Awake() {
			_transform = transform;

			Projectiles = new List<WeaponProjectile>();

			if (SectionCount <= 0)
				Debug.LogError("ERROR: No initial section on this weapon!");

			for (int i = 0; i < SectionCount; i++) {
				if (i < SectionCount - 1)
					sections[i].AssignWeapon(this, sections[i + 1]);
				else
					sections[i].AssignWeapon(this);
			}
		}

		private void Start() => 
			firePoints.MoveNext();

		private void Update() => 
			ReadInput();

		private void ReadInput() {
			if (PlayerInput.Fire1 & CharController.Aiming)
				Fire();
			else
				Release();
		}

		public void Fire() {
			sections[0].TransitionModule.PressFire();
		}

		public void Release() {
			sections[0].TransitionModule.ReleaseFire();
		}

		public void ClearProjectiles() {
			foreach (WeaponProjectile projectile in Projectiles)
				Destroy(projectile.gameObject);
			Projectiles.Clear();
		}

		public void RegisterProjectile(IEnumerable<WeaponProjectile> projectiles) {
			if (projectiles == null)
				return;

			foreach (WeaponProjectile projectile in projectiles)
				RegisterProjectile(projectile);
		}

		public void RegisterProjectile(WeaponProjectile projectile) {
			if (projectile == null)
				return;

			Projectiles.Add(projectile);
			projectile.WeaponSet = this;
		}

		public Transform GetFirePoint() {
			Transform point = firePoints.Current;
			firePoints.MoveNext();
			return point;
		}

		private Transform CreateModuleParent() {
			if (_moduleParent == null) {
				_moduleParent = new GameObject("Modules").transform;
				_moduleParent.parent = _transform;
			}
			return _moduleParent;
		}

		public void AddSection() {
			WeaponSection newSection = new WeaponSection(sections[0]);
			sections[SectionCount - 1].AssignWeapon(this, newSection);
			sections.Add(newSection);
			newSection.AssignWeapon(this);
		}

		public void RemoveSection() {
			if (SectionCount > 1) {
				sections.RemoveAt(SectionCount - 1);
				sections[SectionCount - 1].AssignWeapon(this);
			}
		}
	}
}