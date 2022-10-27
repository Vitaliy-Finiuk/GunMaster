using UnityEngine;

namespace CodeBase.WeaponsSystem.Weapons.Projectile
{
	public class OnCollisionDestroy : ProjectileModifier {

		public LayerMask collisionLayer;

		void OnCollisionEnter(Collision other) {
			if (collisionLayer == (collisionLayer | (1 << other.gameObject.layer)))
				Destroy(gameObject);
		}
	}
}