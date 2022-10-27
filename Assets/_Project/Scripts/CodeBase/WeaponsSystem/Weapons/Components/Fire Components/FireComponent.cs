using _Project.Scripts.CodeBase.WeaponsSystem.Weapons.Core;
using UnityEngine;

namespace _Project.Scripts.CodeBase.WeaponsSystem.Weapons.Components.Fire_Components
{
	[RequireComponent(typeof(WeaponModule))]
	public abstract class FireComponent : MonoBehaviour {

		public WeaponModuleParameters fireParameters;

		protected WeaponModule Module { get; private set; }

		void Awake() {
			Module = GetComponent<WeaponModule>();
		}
	
		public abstract WeaponProjectile[] OnPressFire();

		public abstract WeaponProjectile[] OnHoldFire();

		public abstract WeaponProjectile[] OnReleaseFire();
	}
}