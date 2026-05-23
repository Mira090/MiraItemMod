using Mirror;
using Mirror.RemoteCalls;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_SelfExplosion : Charm_FireBulletInRange, IAttackableCharm
    {

        public static string DamageId = "Charm_SelfExplosion";

        public GameObject ExplosionFxPrefab => SephiriaPrefabs.ExplosionFx;

        //public int[] damageByLevel = [189, 257, 375, 521, 749, 1004];
        public int[] damageByLevel = new int[] { 120, 150, 200, 270, 410, 510 };
        public int[] percent = new int[] { 160, 200, 250, 330, 480, 700 };
        public int[] count = new int[] { 1, 1, 1, 2, 2, 3 };

        public float explodeRadius = 3f;

        private RaycastHit2D[] allocatedHits = new RaycastHit2D[20];

        public float coolDownReduction = 1f;

        private void Awake()
        {
            coolDownTimer.time = 12f;
            activeText = new LocalizedString("CharmActive_Effect_IceBow");
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (damageByLevel.SafeRandomAccess(0).ToString() + "→" + damageByLevel.SafeRandomAccess(maxLevel)) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());//"+0;-#"
            string value2 = (showAllLevel ? (count.SafeRandomAccess(0).ToString() + "→" + count.SafeRandomAccess(maxLevel)) : count.SafeRandomAccess(LevelToIdx(level)).ToString());//"+0;-#"
            string value3 = (showAllLevel ? (percent.SafeRandomAccess(0).ToString() + "→" + percent.SafeRandomAccess(maxLevel)) : percent.SafeRandomAccess(LevelToIdx(level)).ToString());//"+0;-#"

            string value4 = "-";
            if(avatar != null && !ignoreAvatarStatus)
            {
                value4 = (damageByLevel.SafeRandomAccess(LevelToIdx(level)) + avatar.GetCustomStat(ECustomStat.FireDamage) * percent.SafeRandomAccess(LevelToIdx(level)) / 100).ToString();
            }

            return new Loc.KeywordValue[5]
            {
            new Loc.KeywordValue("DAMAGE", value4, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("BASE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", value3 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", value2, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("TIME", coolDownTimer.time.ToString("0.#"))
            };
        }
        protected override void Update()
        {
            if (base.isServer && IsEffectEnabled)
            {
                OnUpdate();
            }
            Update_Client();

            if (base.isServer && !this.GetChance() && coolDownTimer.Update(0))
            {
                this.SetChance(true);
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnitAsLeader += OnAttackUnitAsLeader;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitAsLeader -= OnAttackUnitAsLeader;
        }

        private void OnAttackUnitAsLeader(UnitAvatar avatar, UnitAvatar follower, DamageInstance damage)
        {
            if (avatar.monsterType == EMonsterType.Dummy || damage.id == DamageId)
                return;
            if (IsEffectEnabled && !this.GetChance())
            {
                coolDownTimer.AddTimer(coolDownReduction);
                NetworkestimatedFireChanceTime = estimatedFireChanceTime - coolDownReduction;
            }
        }
        protected override void FireCastingServer(Vector3 aimedPosition, UnitAvatar aimedTarget)
        {
            if (this.GetChance())
            {
                this.SetChance(false);
                NetworkestimatedFireChanceTime = (float)NetworkTime.time;

                var list = new List<UnitAvatar>(NetworkAvatar.Followers);

                int c = count.SafeRandomAccess(CurrentLevelToIdx());
                for (int q = 0; q < list.Count; q++)
                {
                    if (q >= c)
                        break;
                    CreateExplosion(list[q]);
                    Vector2 vector = list[q].transform.position;
                    DamageInstance damage = DamageInstance.GetDamage(base.NetworkAvatar, DamageId, vector, base.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), 999999, EDamageType.Projectile, EDamageFromType.None, Vector2.zero, 0, 0f);
                    damage.elementalType = EDamageElementalType.Fire;
                    damage.criticalChancePercent += NetworkAvatar.GetCustomStat("FOLLOWERCRITICAL") / 100f;
                    damage.victim = list[q];
                    list[q].Die(5, damage);
                }
            }
        }

        private void CreateExplosion(UnitAvatar avatar)
        {
            if (base.NetworkAvatar.IsDead)
            {
                return;
            }

            Vector2 vector = avatar.transform.position;
            int num = Physics2D.CircleCastNonAlloc(vector, explodeRadius, Vector2.zero, allocatedHits, 0f, CombatManager.Topdown1FLayerMask);
            for (int i = 0; i < num; i++)
            {
                Hitbox component = allocatedHits[i].transform.GetComponent<Hitbox>();
                if ((bool)component)
                {
                    CombatBehaviour combatBehaviour = component.GetCombatBehaviour(0);
                    if ((bool)combatBehaviour)
                    {
                        float d = ModUtil.CalculateDamage(this);
                        DamageInstance damage = DamageInstance.GetDamage(base.NetworkAvatar, DamageId, vector, base.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), d, EDamageType.Projectile, EDamageFromType.None, Vector2.zero, 0, 0f);
                        damage.elementalType = EDamageElementalType.Fire;
                        damage.criticalChancePercent += NetworkAvatar.GetCustomStat("FOLLOWERCRITICAL") / 100f;
                        combatBehaviour.ApplyDamage(damage);
                    }
                }
            }

            if ((bool)base.NetworkAvatar && base.NetworkAvatar.TryGetComponent<PlayerSpawner>(out var component2))
            {
                CreateBurnExplosionFx(avatar.transform.position, component2.currentPlayerIdx);
            }
            else
            {
                CreateBurnExplosionFx(avatar.transform.position, -1);
            }
        }
        public new float GetDamage(UnitAvatar avatar)
        {
            float d = damageByLevel.SafeRandomAccess(CurrentLevelToIdx()) + NetworkAvatar.GetCustomStat(ECustomStat.FireDamage) * percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f;
            d += d * NetworkAvatar.GetCustomStatUnsafe("FOLLOWERDAMAGE") / 100f;
            return d;
        }

        [ClientRpc]
        private void CreateBurnExplosionFx(Vector3 position, int playerIdx)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteVector3(position);
            writer.WriteInt(playerIdx);
            var func = "System.Void Charm_SelfExplosion::CreateBurnExplosionFx(UnityEngine.Vector3,System.Int32)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        public override bool Weaved()
        {
            return true;
        }

        protected void UserCode_CreateBurnExplosionFx__Vector3__Int32(Vector3 position, int playerIdx)
        {
            GameObject owner = ((!base.NetworkAvatar) ? null : base.NetworkAvatar.gameObject);
            var fx = SpriteFx.Pool.Spawn(ExplosionFxPrefab, position, owner);
            fx.SetScale(Vector3.one * explodeRadius);
        }

        protected static void InvokeUserCode_CreateBurnExplosionFx__Vector3__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC CreateBurnExplosionFx called on server.");
            }
            else
            {
                ((Charm_SelfExplosion)obj).UserCode_CreateBurnExplosionFx__Vector3__Int32(reader.ReadVector3(), reader.ReadInt());
            }
        }

        static Charm_SelfExplosion()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(Charm_SelfExplosion), "System.Void Charm_SelfExplosion::CreateBurnExplosionFx(UnityEngine.Vector3,System.Int32)", InvokeUserCode_CreateBurnExplosionFx__Vector3__Int32);
        }
    }
}
