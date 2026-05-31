using Mirror;
using Mirror.RemoteCalls;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonDagger_FuryFrostbite : WeaponAddon
    {
        public int currentFury = 0;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            currentFury = 0;
            if (parent is WeaponSimple_Dagger dagger)
            {
                //dagger.OnUseFury += OnUseFury;
            }
            parent.Networkowner.OnSpecialAttack += OnAttackUnit;
        }

        private void OnAttackUnit(CombatBehaviour behaviour, DamageInstance damage, ProjectileBase projectile)
        {
            if (behaviour is UnitAvatar avatar)
            {
                if (projectile.swingId != "FURY" || !isEnabled || avatar.IsDead)
                    return;
                avatar.ApplyDebuff(SephiriaPrefabs.Frostbite, parent.Networkowner.unitAvatar);

                /*
                int percent = 20;
                for(int q = 0; q < currentFury; q++)
                {
                    avatar.ApplyDebuff(SephiriaPrefabs.Frostbite, parent.Networkowner.unitAvatar);
                    ApplyAdditionalDamage(avatar, damage, percent * (q + 1));
                }*/
            }
        }
        public override void OnParry(DamageInstance damage)
        {
            base.OnParry(damage);
            if (parent is WeaponSimple_Dagger dagger)
            {
                if (damage.origin is UnitAvatar avatar)
                {
                    foreach (var debuff in avatar.Debuffs)
                    {
                        if (debuff.ID == SephiriaPrefabs.Frostbite.ID)
                        {
                            dagger.InvokeAddFury(1);
                            return;
                        }
                    }
                }
            }
        }

        private void ApplyAdditionalDamage(UnitAvatar avatar, DamageInstance instance, int additionalDamagePercent)
        {
            if (instance.fromType == EDamageFromType.DirectAttack)
            {
                UnitAvatar unitAvatar = parent.Networkowner.unitAvatar;
                float num = (float)(unitAvatar.GetCustomStat(ECustomStat.IceDamage) * additionalDamagePercent) / 100f;

                num += num * (float)unitAvatar.GetCustomStat(ECustomStat.WeaponDamageBonus) / 100f;
                num += num * (float)unitAvatar.GetCustomStat(ECustomStat.SpecialAttackDamageBonus) / 100f;
                num += num * (float)unitAvatar.GetCustomStat(ECustomStat.FinalWeaponDamage) / 100f;
                DamageInstance damage = DamageInstance.GetDamage(unitAvatar, "Weapon_AdditionalElementalDamage_Ice", instance.damagePoint, instance.targetFactionLayers, num, instance.damageType, EDamageFromType.None, default(Vector2), 0, 0f);
                damage.elementalType = EDamageElementalType.Ice;
                avatar.ApplyDamage(damage);

                Vector2 vector2FromAngle = HorayUtility.GetVector2FromAngle((avatar.transform.position - parent.Networkowner.unitAvatar.transform.position).GetAngle() + UnityEngine.Random.Range(-20f, 20f));
                Vector2 vector = (Vector2)avatar.transform.position + UnityEngine.Random.insideUnitCircle * 0.2f;
                Core.LoggerMany($"Applying additional frostbite damage with {additionalDamagePercent}% bonus, spawning effect at {vector} with angle {vector2FromAngle.GetAngle()}");
                RpcCreateFx(vector, vector2FromAngle.GetAngle(), 0);
            }
        }

        private void OnUseFury()
        {
            if (parent is WeaponSimple_Dagger dagger)
            {
                currentFury = dagger.currentFury;

                if(currentFury > 0)
                {
                    dagger.NetworkcurrentFury = 0;
                    dagger.Networkowner.unitAvatar.DestroyEffectHUD("DaggerImmersion");
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            if (parent is WeaponSimple_Dagger dagger)
            {
                //dagger.OnUseFury -= OnUseFury;
            }
            parent.Networkowner.OnSpecialAttack -= OnAttackUnit;
        }
        [ClientRpc]
        private void RpcCreateFx(Vector3 position, float angle, float rangeBonus)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteVector3(position);
            writer.WriteFloat(angle);
            writer.WriteFloat(rangeBonus);

            ushort num = (ushort)((uint)"System.Void WeaponAddonDagger_FuryFrostbite::RpcCreateFx(UnityEngine.Vector3,System.Single,System.Single)".GetStableHashCode() & 0xFFFFu);
            SendRPCInternal("System.Void WeaponAddonDagger_FuryFrostbite::RpcCreateFx(UnityEngine.Vector3,System.Single,System.Single)", num, writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        public override bool Weaved()
        {
            return true;
        }

        protected void UserCode_RpcCreateFx__Vector3__Single__Single(Vector3 position, float angle, float rangeBonus)
        {
            if (!(SephiriaPrefabs.FreezeNormalSlash == null))
            {
                Core.LoggerMedium($"Creating frostbite slash effect at {position} with angle {angle} and range bonus {rangeBonus}");
                SpriteFx spriteFx = SpriteFx.Pool.Spawn(SephiriaPrefabs.FreezeNormalSlash, position);
                spriteFx.SetRotation(new Vector3(0f, 0f, angle + 70f + ((HorayUtility.GetRandomSign() == 1) ? 180f : 0f)));
                spriteFx.SetScale(Vector3.one * (1f + rangeBonus));
            }
        }

        protected static void InvokeUserCode_RpcCreateFx__Vector3__Single__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcCreateFx called on server.");
            }
            else
            {
                ((WeaponAddonDagger_FuryFrostbite)obj).UserCode_RpcCreateFx__Vector3__Single__Single(reader.ReadVector3(), reader.ReadFloat(), reader.ReadFloat());
            }
        }

        static WeaponAddonDagger_FuryFrostbite()
        {
            Core.LoggerMany("Registering RPCs for WeaponAddonDagger_FuryFrostbite...");
            RemoteProcedureCalls.RegisterRpc(typeof(WeaponAddonDagger_FuryFrostbite), "System.Void WeaponAddonDagger_FuryFrostbite::RpcCreateFx(UnityEngine.Vector3,System.Single,System.Single)", InvokeUserCode_RpcCreateFx__Vector3__Single__Single);
        }
    }
}
