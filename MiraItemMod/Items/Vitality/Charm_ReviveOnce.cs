using MiraItemMod.Utilities;
using Mirror;
using Mirror.RemoteCalls;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Vitality
{
    public class Charm_ReviveOnce : Charm_StatusInstance
    {
        public bool isInCooldown;
        public float remain = 1;
        public LocalizedString message = new LocalizedString("Item_ReviveOnce_Notice");
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("HP", remain.ToString())
            };
        }
        private void Awake()
        {
            effectHUD_ID = "ReviveOnce".ToSephiriaUpperId();
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnDamagedServerside += OnDamagedServerside;
            NetworkAvatar.OnEndSpawnerBattle += OnEndSpawnerBattle;

        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnDamagedServerside -= OnDamagedServerside;
            NetworkAvatar.OnEndSpawnerBattle -= OnEndSpawnerBattle;
        }

        private void OnDamagedServerside(DamageInstance damage)
        {
            if (isInCooldown)
                return;
            if (base.NetworkAvatar.hp > 0f)
                return;
            RpcBroadcastChintamani(NetworkAvatar);

            base.NetworkAvatar.Networkhp = 0f;
            base.NetworkAvatar.Heal(remain);
            base.NetworkAvatar.StartReviveInvulnerable();
            isInCooldown = true;
            RemoveEffectHUD();
        }
        private void OnEndSpawnerBattle()
        {
            isInCooldown = false;
            CreateEffectHUD();
            NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
        }
        [ClientRpc]
        public void RpcBroadcastChintamani(UnitAvatar avatar)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteNetworkBehaviour(avatar);
            var func = "System.Void Charm_ReviveOnce::RpcBroadcastChintamani(UnitAvatar)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        static Charm_ReviveOnce()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(DungeonManager), "System.Void Charm_ReviveOnce::RpcBroadcastChintamani(UnitAvatar)", InvokeUserCode_RpcBroadcastChintamani__UnitAvatar);
        }
        protected void UserCode_RpcBroadcastChintamani__UnitAvatar(UnitAvatar avatar)
        {
            if (avatar.isOwned)
            {
                GameLogWriter.Instance.WriteLog(message.ToString(), Color.yellow);
                UIManager.Instance.GetElement<UI_SystemMessage>().Open(message.ToString(), 2.7f);
            }
        }

        protected static void InvokeUserCode_RpcBroadcastChintamani__UnitAvatar(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC Charm_ReviveOnce::RpcBroadcastChintamani called on server.");
            }
            else
            {
                ((Charm_ReviveOnce)obj).UserCode_RpcBroadcastChintamani__UnitAvatar(reader.ReadNetworkBehaviour<UnitAvatar>());
            }
        }
    }
}
