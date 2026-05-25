using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Pallas
{
    public class Charm_PallasJoker : Charm_PallasEnhancer
    {
        public string damageId = "Charm_PallasJoker";
        public int[] countByLevels = new int[] { 2, 3, 4, 5, 6 };
        public static ItemPosition[] Directions = new ItemPosition[8]
        {
        new ItemPosition(-1, 0),
        new ItemPosition(1, 0),
        new ItemPosition(0, -1),
        new ItemPosition(0, 1),
        new ItemPosition(-1, -1),
        new ItemPosition(1, -1),
        new ItemPosition(-1, 1),
        new ItemPosition(1, 1)
        };
        protected override ItemPosition[] GetDirections()
            => Directions;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? countByLevels.SafeRandomAccess(0) + "→" + countByLevels.SafeRandomAccess(maxLevel) : countByLevels.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("COUNT", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            PallasEvents.OnPallasCardSpawn += OnPallasSpawnChance;
            PallasEvents.OnPallasAceSpawn += OnAceSpawnChance;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            PallasEvents.OnPallasCardSpawn -= OnPallasSpawnChance;
            PallasEvents.OnPallasAceSpawn -= OnAceSpawnChance;
        }

        private void OnPallasSpawnChance(Charm_PallasCard instance, int idx)
        {
            if (instance.NetworkAvatar != NetworkAvatar)
                return;
            float num = instance.defaultChance + instance.throwChanceByLevel.SafeRandomAccess(instance.CurrentLevelToIdx()) * Mathf.Clamp(NetworkAvatar.GetCustomStat(ECustomStat.Luck), 0, 9999);
            num *= instance.WeaponController.currentWeapon.AttackWeightPerSwing;
            num -= 100f;
            if (num < 0f || UnityEngine.Random.Range(0f, 1f) > num / 100f)
            {
                return;
            }

            int count = countByLevels.SafeRandomAccess(CurrentLevelToIdx());
            float anglePer = 8f;
            float startAngle = anglePer * (count - 1) * 0.5f;
            float angle = (WeaponController.aimedPositionClientside - WeaponController.transform.position).GetAngle();
            for (int q = 0; q < count; q++)
            {
                Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - startAngle + anglePer * q));
                Vector3 motionDataBegin = NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                Vector3 motionDataEnd = NetworkAvatar.transform.position + vector3FromAngle * 8f;
                bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                var d = CalculateDamage(instance);
                Bullet bullet = Bullet.Pool.Spawn(instance.bulletBigPrefab.GetRandom(), NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, d, instance.staggeringLevel, instance.externalForcePower, NetworkAvatar, NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.TopdownActor.CenterYPos, motionDataBegin, motionDataEnd, null, null);
                bullet.pierceCreatureCount = 2;
                Vector3 pos = NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                bullet.SetSpeedScale(2);

            }
        }
        private void OnAceSpawnChance(Charm_PallasAce instance, int idx)
        {
            if (instance.NetworkAvatar != NetworkAvatar)
                return;
            float num = instance.defaultChance + instance.throwChanceByLevel.SafeRandomAccess(instance.CurrentLevelToIdx()) * Mathf.Clamp(NetworkAvatar.GetCustomStat(ECustomStat.Luck), 0, 9999);
            num *= instance.WeaponController.currentWeapon.AttackWeightPerSwing;
            num -= 100f;
            if (num < 0f || UnityEngine.Random.Range(0f, 1f) > num / 100f)
            {
                return;
            }

            int count = countByLevels.SafeRandomAccess(CurrentLevelToIdx());
            float anglePer = 8f;
            float startAngle = anglePer * (count - 1) * 0.5f;
            float angle = (WeaponController.aimedPositionClientside - WeaponController.transform.position).GetAngle();
            for (int q = 0; q < count; q++)
            {
                Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - startAngle + anglePer * q));
                Vector3 motionDataBegin = NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                Vector3 motionDataEnd = NetworkAvatar.transform.position + vector3FromAngle * 8f;
                bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                var d = CalculateDamage(instance);
                Bullet bullet = Bullet.Pool.Spawn(instance.bulletBigPrefab.GetRandom(), NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, d, instance.staggeringLevel, instance.externalForcePower, NetworkAvatar, NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.TopdownActor.CenterYPos, motionDataBegin, motionDataEnd, null, null);
                bullet.pierceCreatureCount = 2;
                Vector3 pos = NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                bullet.SetSpeedScale(2);

            }
        }

        protected override void ClearStats(CustomPallasController card)
        {
            card.HasJoker = false;
        }
        protected override void SetStats(CustomPallasController card)
        {
            card.HasJoker = true;
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
