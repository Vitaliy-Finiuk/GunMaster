using _Project.Scripts.CodeBase.WeaponsSystem.Weapons.Core;
using UnityEngine;

namespace _Project.Scripts.CodeBase.WeaponsSystem.Weapons.Projectile
{
	[RequireComponent(typeof(WeaponProjectile))]
	public class ProjectileModifier : MonoBehaviour {

		protected WeaponProjectile _projectile;
		protected Rigidbody _rigidbody;
		protected Transform _transform;

		void Awake() {
			_projectile = GetComponent<WeaponProjectile>();
			_rigidbody = GetComponent<Rigidbody>();
			_transform = transform;
			OnAwake();
		}

		protected virtual void OnAwake() { }

		public virtual void Simulate(float timeToSimulate) { }
	}
}