namespace CodeBase.WeaponsSystem.Weapons.Projectile
{
	public class InitialSize : ProjectileModifier {

		public float sizeFactor = 1;

		void Start() {
			_projectile.Size = _projectile.Parameters.size * sizeFactor;
		}
	}
}