using CodeBase.WeaponsSystem.Weapons.Core;
using UnityEngine;

namespace CodeBase.WeaponsSystem.Weapons.Components.Transition_Components
{
	[RequireComponent(typeof(WeaponModule))]
	public abstract class TransitionComponent : MonoBehaviour {

		public WeaponModuleParameters transitionParameters;

		protected WeaponModule Module { get; private set; }

		void Awake() {
			Module = GetComponent<WeaponModule>();
		}

		public abstract WeaponProjectile[] OnTransition(WeaponProjectile projectile);
	}
}