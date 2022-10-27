using UnityEngine;

namespace CodeBase.WeaponsSystem.Weapons.Projectile
{
	[RequireComponent(typeof(ParticleSystem))]
	public class DestroyAfterDuration : MonoBehaviour {

		private float _lifeTime;

		private ParticleSystem _system;

		private void Awake() {
			_system = GetComponent<ParticleSystem>();
		}

		private void Start() {
			_lifeTime = 0;
		}

		void Update() {
			_lifeTime += Time.deltaTime;
			if (_lifeTime >= _system.duration)
				Destroy(gameObject);
		}
	}
}
