using FMODUnity;
using MiraItemMod.Utilities;
using Mirror;
using Mirror.RemoteCalls;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_ElruNaptimePillowMkII : Charm_Basic
    {
        public int[] healByLevel = new int[3] { 1, 2, 3 };

        public GameObject healFxPrefab;

        public StudioEventEmitter healSound;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (healByLevel.SafeRandomAccess(0).ToString("0;#") + "→" + healByLevel.SafeRandomAccess(maxLevel)) : healByLevel.SafeRandomAccess(LevelToIdx(level)).ToString("0;#"));
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("HEAL", value + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = base.NetworkAvatar;
            networkAvatar.OnEndSpawnerBattle += OnEndSpawnerBattle;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = base.NetworkAvatar;
            networkAvatar.OnEndSpawnerBattle -= OnEndSpawnerBattle;
        }

        private void OnEndSpawnerBattle()
        {
            if (!base.NetworkAvatar.IsDead)
            {
                base.NetworkAvatar.HealPercent(healByLevel.SafeRandomAccess(CurrentLevelToIdx()));
                RpcHeal(base.NetworkAvatar.transform.position);
            }
        }

        [ClientRpc]
        private void RpcHeal(Vector3 pos)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteVector3(pos);
            var func = "System.Void Charm_ElruNaptimePillowMkII::RpcHeal(UnityEngine.Vector3)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        public override bool Weaved()
        {
            return true;
        }

        protected void UserCode_RpcHeal__Vector3(Vector3 pos)
        {
            if ((bool)healFxPrefab)
            {
                SpriteFx.Pool.Spawn(healFxPrefab, pos + Vector3.down * 0.0001f);
            }

            if (healSound != null)
            {
                healSound.transform.position = pos;
                healSound.Play();
            }
        }

        protected static void InvokeUserCode_RpcHeal__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcHeal called on server.");
            }
            else
            {
                ((Charm_ElruNaptimePillowMkII)obj).UserCode_RpcHeal__Vector3(reader.ReadVector3());
            }
        }

        static Charm_ElruNaptimePillowMkII()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(Charm_ElruNaptimePillowMkII), "System.Void Charm_ElruNaptimePillowMkII::RpcHeal(UnityEngine.Vector3)", InvokeUserCode_RpcHeal__Vector3);
        }
    }
}
