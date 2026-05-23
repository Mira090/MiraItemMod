using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using static WeaponSimple_Crossbow;

namespace SephiriaMod.Items
{
    public class Charm_WoundWeapon : Charm_StatusInstance
    {
        private int[] woundStack = new int[] { 0, 1, 2, 4, 6, 8, 10 };
        private bool debuff = true;
        private int count = 0;
        private int cost = 3;

        private int basic = 0;
        public int MaxCostCount => 100 / cost;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? woundStack.SafeRandomAccess(0) + "→" + woundStack.SafeRandomAccess(maxLevel) : woundStack.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = "-" + count * cost + "%";
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("STACK", "+" + value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COST", "-" + cost + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", value2, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("WOUNDSTACK", woundStack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", cost * count);
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
            //NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
            WeaponController.OnSpecialAttack += OnSpecialAttack;
            Events.OnSubAttackCrossbow += OnSubAttackCrossbow;
            Events.OnSubAttackKatana += OnSubAttackKatana;
            Events.OnSubAttackGreatSword += OnSubAttackGreatSword;
            NetworkAvatar.OnAddedDebuffOnTarget += OnAddedDebuffOnTarget;

            basic = NetworkAvatar.GetCustomStat(ECustomStat.BasicAttackDamageBonus) + 100;
            NetworkAvatar.AddCustomStat(ECustomStat.BasicAttackDamageBonus, -basic);
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            NetworkAvatar.AddCustomStat(ECustomStat.BasicAttackDamageBonus, basic);
            basic = NetworkAvatar.GetCustomStat(ECustomStat.BasicAttackDamageBonus) + 100;
            NetworkAvatar.AddCustomStat(ECustomStat.BasicAttackDamageBonus, -basic);
        }

        private void OnSubAttackGreatSword(WeaponSimple_GreatSword weapon)
        {
            if (weapon.Networkowner.unitAvatar == NetworkAvatar)
            {
                if (weapon.GetSweepRequest() && weapon.Networkowner.unitAvatar.GetCustomStatUnsafe("WOUNDEXPLOSION") > 0)
                {
                    NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
                    count = 0;
                    SaveItemOnServer(SaveManager.CurrentRun);
                }
            }
        }

        private void OnSubAttackKatana(WeaponSimple_Katana weapon)
        {
            if (weapon.Networkowner.unitAvatar == NetworkAvatar)
            {
                if (weapon.Networkowner.mpConsumedAttackTriggerFromAnimationValue > 0)
                {
                    NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
                    count = 0;
                    SaveItemOnServer(SaveManager.CurrentRun);
                }
            }
        }

        private void OnSubAttackCrossbow(WeaponSimple_Crossbow weapon)
        {
            if(weapon.Networkowner.unitAvatar == NetworkAvatar)
            {
                if (weapon.specialAttackType == ESpecialAttackType.FireBullet)
                {
                    //NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
                    //count = 0;
                    //SaveItemOnServer(SaveManager.CurrentRun);
                }
                else if (weapon.specialAttackType == ESpecialAttackType.FastReload)
                {
                    NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
                    count = 0;
                    SaveItemOnServer(SaveManager.CurrentRun);
                }
                else if (weapon.specialAttackType == ESpecialAttackType.IceBuff && weapon.iceBuffCoolDownTimer.Check() && (bool)weapon.iceBuffPrefab)
                {
                    NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
                    count = 0;
                    SaveItemOnServer(SaveManager.CurrentRun);
                }
            }
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar unit, DamageInstance damage)
        {
            if (damage.id == "Weapon_SpecialAttack" || damage.id == "Weapon_SpecialAttack_Amethyst")
            {
                NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
                count = 0;
                SaveItemOnServer(SaveManager.CurrentRun);
            }
        }
        private void OnSpecialAttack(CombatBehaviour combat, DamageInstance damage, ProjectileBase projectile)
        {
            NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
            count = 0;
            SaveItemOnServer(SaveManager.CurrentRun);
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("WOUNDSTACK", -woundStack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", -cost * count);
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
            //NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            WeaponController.OnSpecialAttack -= OnSpecialAttack;
            Events.OnSubAttackCrossbow -= OnSubAttackCrossbow;
            Events.OnSubAttackKatana -= OnSubAttackKatana;
            Events.OnSubAttackGreatSword -= OnSubAttackGreatSword;
            NetworkAvatar.OnAddedDebuffOnTarget -= OnAddedDebuffOnTarget;

            NetworkAvatar.AddCustomStat(ECustomStat.BasicAttackDamageBonus, basic);
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe("WOUNDSTACK", -woundStack.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe("WOUNDSTACK", woundStack.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        private void OnAttackUnit(UnitAvatar unitAvatar, DamageInstance damageInstance)
        {
            if (IsEffectEnabled && debuff && damageInstance.fromType == EDamageFromType.DirectAttack)
            {
                //debuff = false;
                unitAvatar.ApplyDebuff(SephiriaPrefabs.Wound, NetworkAvatar);
            }
        }
        private void OnAddedDebuffOnTarget(CharacterDebuff debuff, string id)
        {
            if(count >= MaxCostCount)
                return;
            count++;
            NetworkAvatar.AddCustomStatUnsafe("SPECIALATTACKCOSTREDUCTION", cost);
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_WoundWeapon_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_WoundWeapon_{Item.InstanceID}_Stack", 0);
        }
    }
}
