using MiraItemMod.Utilities;
using Mirror;
using Mirror.RemoteCalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Machina
{
    public class Charm_Machina : Charm_MachinaBasic, IAttackableCharm
    {
        public static readonly string CooldownStat = "MachinaAttackSpeed".ToSephiriaUpperId();
        public static readonly string FrostToMachina = "FrostToMachina".ToSephiriaUpperId();

        public int[] damageByLevel = new int[10] { 70, 80, 100, 110, 130, 140, 160, 170, 190, 220};
        public Timer cooldownTimer = new Timer(0.5f);
        public bool isInCooldown;
        public virtual string DamageId => "Charm_MachinaTest";
        public virtual float AttackDashScale => 0.5f;
        public virtual int MpConsumed => 0;
        public virtual float RangeBonus => 0f;
        public NewWeaponFireData FireData
        {
            get
            {
                if(_melee == null)
                {
                    _melee = GetFireData();
                }
                return _melee;
            }
        }
        private NewWeaponFireData _melee;
        protected virtual void Awake()
        {

        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && !NetworkAvatar.IsDead && cooldownTimer.Update(Time.deltaTime + Time.deltaTime * NetworkAvatar.GetCustomStatUnsafe(CooldownStat) / 100f + Time.deltaTime * GetCooldownMultiplier()))
            {
                isInCooldown = false;
            }
        }
        protected virtual float GetCooldownMultiplier()
        {
            if (NetworkAvatar.GetCustomStatUnsafe(FrostToMachina) > 0 && IsInMachinaSlot)
                return NetworkAvatar.GetCustomStatUnsafe("CHARGINGCHARMBONUS") / 100f;
            return 0;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(maxLevel) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            string damage = "-";
            if (!ignoreAvatarStatus)
                damage = GetDamage(avatar, avatar.GetComponent<WeaponControllerSimple>(), LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[] { 
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString()),
                new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("DAMAGE", damage, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected virtual NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(0);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_SwordAndShield>(out var simple))
                return null;
            return simple.specialAttacks.FirstOrDefault();
        }

        protected void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (isInCooldown)
                return;
            if (damage.fromType != EDamageFromType.DirectAttack || damage.id == DamageId)
                return;
            if (FireData == null)
                return;
            isInCooldown = true;
            Attack();
        }
        protected void OnBaisAttackSwing(int idx)
        {
            if (isInCooldown)
                return;
            if (FireData == null)
                return;
            isInCooldown = true;
            Attack();
        }
        public virtual float GetDamage(UnitAvatar avatar)
        {
            return GetDamage(avatar, WeaponController, CurrentLevelToIdx());
        }
        public virtual float GetDamage(UnitAvatar avatar, WeaponControllerSimple weapon, int idx)
        {
            if (avatar == null || weapon == null)
                return 0;
            return weapon.currentWeapon.InvokeGetRelatedStatMultiplier(avatar, GetDamageElementalType(FireData) ?? EDamageElementalType.Physical, GetRelatedStatFormula(FireData), out var _) * damageByLevel.SafeRandomAccess(idx) / 100f;
        }
        public virtual EDamageElementalType? GetDamageElementalType(NewWeaponFireData fireData)
        {
            if (fireData == null)
                return EDamageElementalType.Physical;
            return fireData.damageElementalType;
        }
        /// <summary>
        /// (EMPTY), FIREDAMAGE, ICEDAMAGE, LIGHTNINGDAMAGE, PHYSICALDAMAGE, HIGHEST, LOWEST, AVERAGE, AVERAGEALL
        /// </summary>
        /// <param name="fireData"></param>
        /// <returns></returns>
        public virtual string GetRelatedStatFormula(NewWeaponFireData fireData)
        {
            if (fireData == null)
                return string.Empty;
            return fireData.relatedStatFormula;
        }
        public virtual Vector3 GetAimedDelta()
        {
            return WeaponController.attackDirection;
        }
        public virtual int GetTriggerCount()
        {
            int count = 1;
            var frostToMachina = NetworkAvatar.GetCustomStatUnsafe(FrostToMachina);
            if (frostToMachina > 0 && IsInMachinaSlot)
            {
                count += NetworkAvatar.GetCustomStatUnsafe("CHARGINGCHARMAMPLIFY");
            }
            return count;
        }
        public virtual void Attack(float percent = 100f)
        {
            List<CombatBehaviour> basicAttackSharedTargetList = WeaponController.currentWeapon.GetBasicAttackSharedTargetList(0);
            Attack(GetTriggerCount(), WeaponController.attackDirection, basicAttackSharedTargetList, percent);
        }
        public void Attack(CombatBehaviour target, float percent = 100)
        {
            List<CombatBehaviour> basicAttackSharedTargetList = new List<CombatBehaviour>() { target };
            Attack(GetTriggerCount(), target.transform.position, basicAttackSharedTargetList, percent);
        }
        public void Attack(Vector3 aimedDelta, float percent = 100)
        {
            Attack(GetTriggerCount(), aimedDelta, new List<CombatBehaviour>(), percent);
        }
        public virtual void Attack(int count, Vector3 aimedDelta, List<CombatBehaviour> sharedTarget, float percent)
        {
            if (WeaponController.currentWeapon == null)
                return;
            RpcAttack();

            Vector3 vector = FirePosition(WeaponController);
            float y = WeaponController.shoulder.Position.y;
            NewWeaponFireData attack = FireData;
            if (attack == null)
                return;
            //float damage = WeaponController.currentWeapon.InvokeGetRelatedStatMultiplier(NetworkAvatar, GetDamageElementalType(attack), GetRelatedStatFormula(attack), out var elemental);
            float damage = Charm_Basic.CalculateDamage(this);
            if (damage <= 0)
                return;
            damage = ModifyDamage(damage);
            if (AttackDashScale > 0f)
            {
                GameCamera.Instance.targetTracker.CreateCameraShaking(WeaponController.transform.position, EShakeCameraType.Continous, attack.cameraShakeVelocityOnFire, 0.08f, 0.0625f);
            }
            damage = damage * percent / 100f;
            float rangeBonus = (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponRange) / 100f + (float)NetworkAvatar.GetCustomStatUnsafe("MACHINARANGE") / 100f + RangeBonus;
            var temp = attack.damageElementalType;
            var elemental = GetDamageElementalType(FireData);
            if (elemental.HasValue)
                attack.damageElementalType = elemental.Value;
            attack.CreateAttack(EDamageFromType.DirectAttack, damage, DamageId, true, NetworkAvatar, vector, vector + aimedDelta, y, OnCreateAttack, sharedTarget, AttackDashScale, null, false, rangeBonus, 1f, MpConsumed, elemental);
            attack.damageElementalType = temp;

            if(count - 1 > 0)
            {
                this.Delay(0.05f, () =>
                {
                    if(IsEffectEnabled && NetworkAvatar != null && !NetworkAvatar.IsDead)
                    {
                        Attack(count - 1, aimedDelta, sharedTarget, percent);
                    }
                });
            }
        }
        protected virtual float ModifyDamage(float damage)
        {
            if (NetworkAvatar.GetCustomStatUnsafe(FrostToMachina) > 0 && IsInMachinaSlot)
            {
                damage += damage * (float)NetworkAvatar.GetCustomStatUnsafe("FROSTRELICDAMAGE") / 100f;
            }
            damage += damage * (float)NetworkAvatar.GetCustomStatUnsafe("MACHINADAMAGE") / 100f;
            damage += damage * (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponDamageBonus) / 100f;
            if (MpConsumed > 0)
            {
                damage += damage * ((float)NetworkAvatar.GetCustomStatUnsafe("MPSKILLDAMAGE") / 100f);
            }
            damage += damage * ((float)NetworkAvatar.GetCustomStat(ECustomStat.FinalWeaponDamage) / 100f);
            return damage;
        }

        protected virtual void OnCreateAttack(int idx, ProjectileBase projectile)
        {

        }
        public virtual Vector3 FirePosition(WeaponControllerSimple simple)
        {
            return simple.shoulder.swingPoint.position - new Vector3(0f, simple.shoulder.Position.y, 0f);
        }
        [ClientRpc]
        public void RpcAttack()
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            var func = "System.Void Charm_Machina::RpcAttack()";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        protected virtual void UserCode_RpcAttack()
        {
            if (Item == null)
                return;
            Core.LoggerFew("UserCode_RpcAttack: " + Item.Name);
            try
            {
                if (NetworkAvatar == null)
                    return;
                var weapon = NetworkAvatar.GetComponent<WeaponControllerSimple>();
                if (weapon == null)
                    return;
                float fxScale = 1f + (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponRange) / 100f + NetworkAvatar.GetCustomStatUnsafe("MACHINARANGE") / 100f + RangeBonus;
                NewWeaponFireData basicAttack = FireData;
                Core.LoggerFew("FireData: " + basicAttack);
                if (basicAttack == null)
                    return;
                bool flag = false;
                int ownerIndex = -1;
                foreach (PlayerSpawner playerSpawner in PlayerSpawner.MultiplayerList)
                {
                    if (playerSpawner && (weapon.gameObject == playerSpawner.gameObject || (NetworkAvatar.NetworkLeader && NetworkAvatar.NetworkLeader.gameObject == playerSpawner.gameObject)))
                    {
                        flag = true;
                        ownerIndex = (playerSpawner.isOwned ? 1 : 0);
                        break;
                    }
                }
                bool canBeTransparentOnMultiplayer = false;
                if (flag)
                {
                    canBeTransparentOnMultiplayer = true;
                }
                Vector3 position = this.FirePosition(weapon) + new Vector3(0f, weapon.shoulder.Position.y, 0f);
                basicAttack.CreateSwingFx(canBeTransparentOnMultiplayer, weapon.transform, position, weapon.shoulder.transform.eulerAngles, fxScale, ownerIndex, 0f);
            }
            catch(Exception e)
            {
                Core.LoggerWarning(e);
            }
        }

        protected static void InvokeUserCode_RpcAttack(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcAttack called on server.");
            }
            else
            {
                ((Charm_Machina)obj).UserCode_RpcAttack();
            }
        }


        static Charm_Machina()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(Charm_Machina), "System.Void Charm_Machina::RpcAttack()", InvokeUserCode_RpcAttack);
        }
    }
}
