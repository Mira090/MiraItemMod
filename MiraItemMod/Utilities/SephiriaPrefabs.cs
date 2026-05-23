using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace SephiriaMod.Utilities
{
    public class SephiriaPrefabs
    {
        public static CharacterDebuff Burn => CombatManager.Instance.burnDebuffPrefab;
        public static CharacterDebuff Electric => CombatManager.Instance.electricDebuffPrefab;
        public static CharacterDebuff Frostbite => CombatManager.Instance.frostbiteDebuffPrefab;
        public static CharacterDebuff Wound
        {
            get
            {
                if(_wound == null)
                    _wound = WeaponDatabase.FindWeaponById(1107).mainWeaponPrefab.GetComponent<WeaponAddonCommon_DebuffAttack>().debuffPrefab;
                return _wound;
            }
        }
        private static CharacterDebuff _wound;
        public static CharacterDebuff Poison
        {
            get
            {
                if (_poison == null)
                    _poison = WeaponDatabase.FindWeaponById(119).mainWeaponPrefab.GetComponent<WeaponAddonCommon_DebuffAttack>().debuffPrefab;
                return _poison;
            }
        }
        private static CharacterDebuff _poison;
        private static GameObject _pallasBigBullet;
        public static GameObject PallasBigBullet
        {
            get
            {
                if(_pallasBigBullet == null)
                {
                    _pallasBigBullet = ItemDatabase.FindItemById(1172).resourcePrefab.GetComponent<Charm_PallasCard>().bulletBigPrefab[0];
                }
                return _pallasBigBullet;
            }
        }
        private static GameObject _meteorBullet;
        public static GameObject MeteorBullet
        {
            get
            {
                if (_meteorBullet == null)
                {
                    _meteorBullet = ItemDatabase.FindItemById(1032).resourcePrefab.GetComponent<Charm_FlameGround_Meteor>().bulletPrefab;
                }
                return _meteorBullet;
            }
        }
        private static GameObject _explosionFx;
        public static GameObject ExplosionFx
        {
            get
            {
                if (_explosionFx == null && MeteorBullet.TryGetComponent<BulletDestroyModule_Explode>(out var module) && module.destroyFxPrefab)
                {
                    _explosionFx = module.destroyFxPrefab;
                }
                return _explosionFx;
            }
        }
        private static GameObject[] _pallasBigBullets;
        public static GameObject[] PallasBigBullets
        {
            get
            {
                if (_pallasBigBullets == null)
                {
                    _pallasBigBullets = ItemDatabase.FindItemById(1172).resourcePrefab.GetComponent<Charm_PallasCard>().bulletBigPrefab;
                }
                return _pallasBigBullets;
            }
        }
        private static GameObject[] _pallasSmallBullets;
        public static GameObject[] PallasSmallBullets
        {
            get
            {
                if (_pallasSmallBullets == null)
                {
                    _pallasSmallBullets = ItemDatabase.FindItemById(1172).resourcePrefab.GetComponent<Charm_PallasCard>().bulletSmallPrefab;
                }
                return _pallasSmallBullets;
            }
        }
        private static GameObject _freezeNormalSlash;
        public static GameObject FreezeNormalSlash
        {
            get
            {
                if (_freezeNormalSlash == null)
                {
                    _freezeNormalSlash = ItemDatabase.FindItemById(1268).resourcePrefab.GetComponent<Charm_FreezeNormalSlash>().slashFxPrefab;//永遠の冬
                }
                return _freezeNormalSlash;
            }
        }

        private static GameObject _money;
        public static GameObject Money
        {
            get
            {
                if(_money == null)
                {
                    _money = Resources.Load<GameObject>("Money");
                }
                return _money;
            }
        }
        private static GameObject _moneyBig;
        public static GameObject MoneyBig
        {
            get
            {
                if (_moneyBig == null)
                {
                    _moneyBig = Resources.Load<GameObject>("MoneyBig");
                }
                return _moneyBig;
            }
        }
        private static GameObject _moneyHuge;
        public static GameObject MoneyHuge
        {
            get
            {
                if (_moneyHuge == null)
                {
                    _moneyHuge = Resources.Load<GameObject>("MoneyHuge");
                }
                return _moneyHuge;
            }
        }
        public static void SpawnMoney(int money, Vector3 pos)
        {
            var i = money;
            while (i >= 1000)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(MoneyHuge, pos + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                gameObject.GetComponent<Money>().AddPhysicalForce(Random.insideUnitCircle * 7f, Random.Range(6f, 10f));
                NetworkServer.Spawn(gameObject);
                i -= 1000;
            }
            while (i >= 100)
            {
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(MoneyBig, pos + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                gameObject2.GetComponent<Money>().AddPhysicalForce(Random.insideUnitCircle * 7f, Random.Range(6f, 10f));
                NetworkServer.Spawn(gameObject2);
                i -= 100;
            }
            while (i > 0)
            {
                GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(Money, pos + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                gameObject3.GetComponent<Money>().AddPhysicalForce(Random.insideUnitCircle * 7f, Random.Range(6f, 10f));
                NetworkServer.Spawn(gameObject3);
                i -= 10;
            }
        }
        private static GameObject _sephiriteLvUp;
        public static GameObject SephiriteLvUp
        {
            get
            {
                if (_sephiriteLvUp == null)
                {
                    _sephiriteLvUp = Resources.Load<GameObject>("Sephirite/Sephirite_LVUP");
                }
                return _sephiriteLvUp;
            }
        }
    }
}
