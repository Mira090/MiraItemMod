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
        public int[] damageByLevel = new int[10] { 45, 50, 60, 75, 90, 110, 140, 180, 230, 300};
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
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime + Time.deltaTime * NetworkAvatar.GetCustomStatUnsafe("MACHINACOOLDOWN") / 100f))
            {
                isInCooldown = false;
            }
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
        public virtual void Attack()
        {
            List<CombatBehaviour> basicAttackSharedTargetList = WeaponController.currentWeapon.GetBasicAttackSharedTargetList(0);
            Attack(WeaponController.attackDirection, basicAttackSharedTargetList);
        }
        public virtual void Attack(Vector3 aimedDelta, List<CombatBehaviour> sharedTarget)
        {
            if (WeaponController.currentWeapon == null)
                return;
            RpcAttack();

            Vector3 vector = FirePosition();
            float y = WeaponController.shoulder.Position.y;
            NewWeaponFireData attack = FireData;
            if (attack == null)
                return;
            //float damage = WeaponController.currentWeapon.InvokeGetRelatedStatMultiplier(NetworkAvatar, GetDamageElementalType(attack), GetRelatedStatFormula(attack), out var elemental);
            float damage = Charm_Basic.CalculateDamage(this);
            if (damage <= 0)
                return;
            damage += damage * (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponDamageBonus) / 100f;
            damage += damage * (float)NetworkAvatar.GetCustomStatUnsafe("MACHINADAMAGE") / 100f;
            if (MpConsumed > 0)
            {
                damage += damage * ((float)NetworkAvatar.GetCustomStatUnsafe("MPSKILLDAMAGE") / 100f);
            }
            damage += damage * ((float)NetworkAvatar.GetCustomStat(ECustomStat.FinalWeaponDamage) / 100f);
            //damage += damage * (float)this.GetAdditionalBasicAttackDamagePercent(idx) / 100f;
            if (AttackDashScale > 0f)
            {
                GameCamera.Instance.targetTracker.CreateCameraShaking(WeaponController.transform.position, EShakeCameraType.Continous, attack.cameraShakeVelocityOnFire, 0.08f, 0.0625f);
            }
            float rangeBonus = (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponRange) / 100f + (float)NetworkAvatar.GetCustomStatUnsafe("MACHINARANGE") / 100f + RangeBonus;
            attack.CreateAttack(EDamageFromType.DirectAttack, damage, DamageId, true, NetworkAvatar, vector, vector + aimedDelta, y, OnCreateAttack, sharedTarget, AttackDashScale, null, false, rangeBonus, 1f, MpConsumed, GetDamageElementalType(FireData));
        }

        protected virtual void OnCreateAttack(int idx, ProjectileBase projectile)
        {

        }
        public virtual Vector3 FirePosition()
        {
            return WeaponController.shoulder.swingPoint.position - new Vector3(0f, WeaponController.shoulder.Position.y, 0f);
        }
        [ClientRpc]
        public void RpcAttack()
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            var func = "System.Void Charm_MachinaTest::RpcAttack()";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        protected virtual void UserCode_RpcAttack()
        {
            try
            {
                if (NetworkAvatar == null || WeaponController == null)
                    return;
                float fxScale = 1f + (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponRange) / 100f + NetworkAvatar.GetCustomStatUnsafe("MACHINARANGE") / 100f + RangeBonus;
                NewWeaponFireData basicAttack = FireData;
                bool flag = false;
                int ownerIndex = -1;
                foreach (PlayerSpawner playerSpawner in PlayerSpawner.MultiplayerList)
                {
                    if (playerSpawner && (WeaponController.gameObject == playerSpawner.gameObject || (NetworkAvatar.NetworkLeader && NetworkAvatar.NetworkLeader.gameObject == playerSpawner.gameObject)))
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
                Vector3 position = this.FirePosition() + new Vector3(0f, WeaponController.shoulder.Position.y, 0f);
                basicAttack.CreateSwingFx(canBeTransparentOnMultiplayer, WeaponController.transform, position, WeaponController.shoulder.transform.eulerAngles, fxScale, ownerIndex, 0f);
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
            RemoteProcedureCalls.RegisterRpc(typeof(Charm_Machina), "System.Void Charm_MachinaTest::RpcAttack()", InvokeUserCode_RpcAttack);
        }
    }
}
