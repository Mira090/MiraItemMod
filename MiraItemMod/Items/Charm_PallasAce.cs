using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_PallasAce : Charm_Basic
    {
        [Header("PallasCard")]
        public string damageId = "Charm_PallasCard";

        public float[] throwChanceByLevel = new float[] { 0f, 2.5f, 5f, 7.5f, 10f };

        public float defaultChance = 50f;

        public GameObject[] bulletBigPrefab => SephiriaPrefabs.PallasBigBullets;

        public GameObject[] bulletSmallPrefab => SephiriaPrefabs.PallasSmallBullets;

        public float bulletDamage = 24f;

        public int staggeringLevel = 1;

        public float externalForcePower = 1f;

        public Timer throwIntervalTimer = new Timer(0.1f, resetOnTime: false);

        private void Awake()
        {
            SetEnhancement(false);
        }
        public bool GetEnhancement(GridInventory inventory)
        {

            ItemPosition[] array = Charm_PallasJoker.Directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = inventory.FindItem(new ItemPosition(xIdx, yIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_PallasJoker)
                        return true;
                }
            }
            return false;
        }
        public void SetEnhancement(bool enhance)
        {
            if (enhance)
            {
                bulletDamage = 64;
                defaultChance = 100;
                throwChanceByLevel = new float[] { 0f, 5f, 10f, 15f, 20f };
                //pallas.throwChanceByLevel = new float[]{0f, 4f, 8f, 12f, 16f};
                throwIntervalTimer.time = 0.05f;
            }
            else
            {
                bulletDamage = 32;
                defaultChance = 50;
                throwChanceByLevel = new float[] { 0f, 2.5f, 5f, 7.5f, 10f };
                throwIntervalTimer.time = 0.1f;
            }
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            float num = 1f;
            if ((bool)avatar && avatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                num = component.currentWeapon.AttackWeightPerSwing;
            }

            if ((bool)avatar && !ignoreAvatarStatus)
            {
                if (!NetworkServer.active)
                {
                    SetEnhancement(GetEnhancement(NetworkAvatar.Inventory));
                }
                string value = showAllLevel ? (throwChanceByLevel.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (throwChanceByLevel.SafeRandomAccess(maxLevel) * num).ToString("0.#") + "%" : (throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * num).ToString("0.#") + "%";
                float num2 = defaultChance * num;
                float num3 = num2 + throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * Mathf.Clamp(avatar.GetCustomStat(ECustomStat.Luck), 0, 9999) * num;
                return new Loc.KeywordValue[4]
                {
                new Loc.KeywordValue("DEFAULT", num2.ToString("0.#")),
                new Loc.KeywordValue("CHANCE", value, GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("DAMAGE", bulletDamage.ToString()),
                new Loc.KeywordValue("CURRENT", num3.ToString("0.#") + "%")
                };
            }
            else
            {
                string value = showAllLevel ? (throwChanceByLevel.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (throwChanceByLevel.SafeRandomAccess(maxLevel) * num).ToString("0.#") + "%" : (throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * num).ToString("0.#") + "%";
                return new Loc.KeywordValue[4]
                {
            new Loc.KeywordValue("DEFAULT", defaultChance.ToString()),
            new Loc.KeywordValue("CHANCE", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", bulletDamage.ToString()),
            new Loc.KeywordValue("CURRENT", "-%")
                };
            }

        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSwingCreated_Dash += OnDashAttack;
        }
        private void OnDashAttack(ProjectileBase projectile)
        {
            Events.InvokeOnAceSpawnChance(0, this);
            if (!WeaponController || !WeaponController.currentWeapon || !throwIntervalTimer.Check())
            {
                return;
            }

            float num = defaultChance + throwChanceByLevel.SafeRandomAccess(CurrentLevelToIdx()) * Mathf.Clamp(NetworkAvatar.GetCustomStat(ECustomStat.Luck), 0, 9999);
            num *= WeaponController.currentWeapon.AttackWeightPerSwing;
            if (UnityEngine.Random.Range(0f, 1f) > num / 100f)
            {
                return;
            }

            int num2 = 3;
            float num3 = 30f;
            float num4 = num3 * (num2 - 1) * 0.5f;
            float angle = (WeaponController.aimedPositionClientside - WeaponController.transform.position).GetAngle();
            for (int i = 0; i < num2; i++)
            {
                Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - num4 + num3 * i));
                Vector3 vector = NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                Vector3 vector2 = NetworkAvatar.transform.position + vector3FromAngle * 8f;
                bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                Bullet bullet = Bullet.Pool.Spawn(flag ? bulletBigPrefab.GetRandom() : bulletSmallPrefab.GetRandom(), NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, bulletDamage / num2 * (flag ? 2f : 1f), staggeringLevel, externalForcePower, NetworkAvatar, NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.TopdownActor.CenterYPos, vector, vector2, null, null);
                bullet.TurnOnHoming(autoDetectTarget: true, null, UnityEngine.Random.Range(0.1f, 0.5f));
                Vector3 pos = NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                if (bullet.MoveModule is BulletMoveModule_FireworkHoming bulletMoveModule_FireworkHoming)
                {
                    bulletMoveModule_FireworkHoming.TurnOnFakeTarget(pos);
                }
            }

            throwIntervalTimer.Ratio = 0f;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSwingCreated_Dash -= OnDashAttack;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!throwIntervalTimer.Check())
            {
                throwIntervalTimer.AddTimer(Time.deltaTime);
            }
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
