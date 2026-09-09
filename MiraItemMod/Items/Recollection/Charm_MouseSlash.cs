using MiraItemMod.Items.Machina;
using MiraItemMod.Utilities;
using Mirror;
using Mirror.RemoteCalls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Recollection
{
    public class Charm_MouseSlash : Charm_StatusInstance
    {
        public static readonly string CooldownStat = "RecollectionAttackSpeed".ToSephiriaUpperId();
        public static readonly string DamageStat = "RecollectionDamage".ToSephiriaUpperId();
        public Timer cooldownTimer = new Timer(1f);
        public bool isInCooldown;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSpecialAttack += OnSpecialAttackSwing;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSpecialAttack -= OnSpecialAttackSwing;
        }
        private void OnSpecialAttackSwing(CombatBehaviour combat, DamageInstance damage, ProjectileBase projectile)
        {
            if (isInCooldown)
                return;
            isInCooldown = true;
            RpcAttack(combat);
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
            return 0;
        }

        [ClientRpc]
        public void RpcAttack(CombatBehaviour target)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteNetworkBehaviour(target);
            var func = "System.Void Charm_MouseSlash::RpcAttack()";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        protected virtual void UserCode_RpcAttack(CombatBehaviour target)
        {
            if (!(NetworkAvatar is PlayerAvatar avatar))
                return;
            try
            {
                var race = RaceDatabase.FindById(30);
                var stage = race.stages.LastOrDefault();
                var battle = stage.firstFloor;
                if (battle is FullyDesignedFloorGenerator floor)
                {
                    var spawner = floor.props[1];
                    if (!spawner.TryGetComponent<BossEnvironment_QQBoss>(out var boss))
                        return;
                    avatar.StartCoroutine(MouseSlashAttackClientCoroutine(avatar, boss.dramaticDieMouseSlash.gameObject, target, false));
                    return;
                }
            }
            catch (Exception e)
            {
                Core.LoggerError(e);
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
                ((Charm_MouseSlash)obj).UserCode_RpcAttack(reader.ReadNetworkBehaviour<CombatBehaviour>());
            }
        }


        static Charm_MouseSlash()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(Charm_MouseSlash), "System.Void Charm_MouseSlash::RpcAttack()", InvokeUserCode_RpcAttack);
        }


        public static IEnumerator MouseSlashAttackClientCoroutine(PlayerAvatar avatar, GameObject prefab, CombatBehaviour target, bool camera)
        {
            var mouseSlashAttackDelay = 1f;

            if (camera)
            {
                GameCamera.Instance.targetTracker.SetDualTarget(target.transform.position, 0.5f, autoZoom: true, 48f);
                GameCamera.Instance.targetTracker.SetDualFollowSpeed(3f);
            }
            yield return new WaitForSeconds(0.5f);
            var pos = target.transform.position;
            GameObject gameObject = UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity);
            gameObject.SetActive(true);
            var mouseSlashInstance = gameObject.GetComponent<QQBossMouseSlash>();
            mouseSlashInstance.isDramaticDieObj = false;
            mouseSlashInstance.SetTargetTransform(target.transform);
            if (camera)
            {
                GameCamera.Instance.targetTracker.SetDualTarget(mouseSlashInstance.topdownActor.body, 0.5f, autoZoom: true, 48f);
                GameCamera.Instance.targetTracker.SetDualFollowSpeed(3f);
            }
            yield return new WaitForSeconds(Mathf.Max(0f, mouseSlashAttackDelay));
            if ((bool)mouseSlashInstance)
            {
                mouseSlashInstance.attackRequest = true;
                /*
                 * ダメージ処理
                if (base.isServer)
                {
                    if (hp > 0.5f)
                    {
                        SetHp(Mathf.Max(0.5f, hp - base.MaxHp * Mathf.Max(0f, mouseSlashSelfDamageMaxHpRatio)));
                    }

                    LocalApplySystemDamage(1f);
                    ClearPreparingLineGroundAreaAttacks();
                    ClearWindmillBulletHells();
                    StopPerimeterBulletGimmick(clearBullets: true);
                }*/
                if (avatar.isServer)
                {
                    ApplyDamage(avatar, pos);
                }
            }

            yield return new WaitForSeconds(1f);
            if (camera)
            {
                GameCamera.Instance.targetTracker.ResetDualTarget();
            }
            yield return new WaitForSeconds(1f);
            //UnityEngine.Object.Destroy(mouseSlashInstance.gameObject);
        }
        public static void ApplyDamage(PlayerAvatar avatar, Vector3 pos)
        {
            var results = Physics2D.BoxCastAll(pos, new Vector2(3, 15), -36, Vector2.zero, 0f, CombatManager.Topdown1FLayerMask);
            foreach (var result in results)
            {
                Hitbox component = result.transform.GetComponent<Hitbox>();
                if ((bool)component)
                {
                    CombatBehaviour combatBehaviour = component.GetCombatBehaviour(0);
                    if ((bool)combatBehaviour)
                    {
                        float d = avatar.GetCustomStat(ECustomStat.PhysicalDamage) * 8f;
                        d += d * avatar.GetCustomStatUnsafe(DamageStat) / 100f;
                        for (int q = 0; q < 5; q++)
                        {
                            DamageInstance damage = DamageInstance.GetDamage(avatar, "Mouse_Test", pos, avatar.GetHostileFactionLayers(EDamageFromType.None), d, EDamageType.Slice, EDamageFromType.None, Vector2.zero, 0, 0f);
                            combatBehaviour.ApplyDamage(damage);
                        }
                    }
                }
            }
        }
    }
}
