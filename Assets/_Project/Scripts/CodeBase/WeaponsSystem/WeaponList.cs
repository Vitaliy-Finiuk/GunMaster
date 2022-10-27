using UnityEngine;

namespace CodeBase.WeaponsSystem
{
    [CreateAssetMenu(fileName = "Data", menuName = "WeaponData/WeaponList", order = 1)]
    public class WeaponList : ScriptableObject
    {
        public string objectName = "Weapon List";
        public GameObject[] weapons;
    }
}