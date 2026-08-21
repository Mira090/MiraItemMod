using MiraItemMod.Utilities;
using Mirror;
using Mirror.RemoteCalls;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_VariableMaxLevelWhitePaper : Charm_WhitePaper
    {
        public virtual string StatusName => "STARGAZELEVEL";
        public virtual int ValiableMax => 16;
        public int AdditionalMaxLevel { get; private set; }
        public int originalMaxLevel;
        public override int GetSubIconCount()
        {
            return 0;
        }
        public override Sprite GetSubIconImage(ItemPosition pos, bool isInstance, int idx)
        {
            return null;
        }
        public override void OnPreSetEffectRefreshed()
        {

        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (NetworkAvatar != null)
            {
                if (!string.IsNullOrEmpty(StatusName))
                {
                    SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
                    RpcSetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
                }
            }
        }
        public virtual void SetAdditionalMaxLevel(int level)
        {
            if (originalMaxLevel + level > ValiableMax)
            {
                level = ValiableMax - originalMaxLevel;
            }
            AdditionalMaxLevel = level;
            maxLevel = originalMaxLevel + AdditionalMaxLevel;
        }
        protected virtual void SetAdditionalMaxLevelOnClient(int level)
        {
            if (originalMaxLevel + level > ValiableMax)
            {
                level = ValiableMax - originalMaxLevel;
            }
            AdditionalMaxLevel = level;
            maxLevel = originalMaxLevel + AdditionalMaxLevel;
            Core.LoggerMany($"SetMaxLevel({name}): " + maxLevel);
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (NetworkAvatar != null)
            {
                if (!string.IsNullOrEmpty(StatusName))
                {
                    SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
                    RpcSetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
                    Inventory.UpdatePing(Item.Position);
                }
            }
        }

        [ClientRpc]
        public void RpcSetAdditionalMaxLevel(int additional)
        {
            Core.LoggerFew("[Charm_VariableMaxLevelWhitePaper] RpcSetAdditionalMaxLevel: " + additional);

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteInt(additional);
            var func = "System.Void Charm_VariableMaxLevelWhitePaper::RpcSetAdditionalMaxLevel(System.Int32)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        public override bool Weaved()
        {
            return true;
        }

        protected void UserCode_RpcSetAdditionalMaxLevel__Int32(int additional)
        {
            Core.LoggerFew("[Charm_VariableMaxLevelWhitePaper] UserCode_RpcSetAdditionalMaxLevel__Int32: " + additional);
            SetAdditionalMaxLevelOnClient(additional);
        }

        protected static void InvokeUserCode_RpcSetAdditionalMaxLevel__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcSetAdditionalMaxLevel called on server.");
            }
            else
            {
                ((Charm_VariableMaxLevelWhitePaper)obj).UserCode_RpcSetAdditionalMaxLevel__Int32(reader.ReadInt());
            }
        }

        static Charm_VariableMaxLevelWhitePaper()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(Charm_VariableMaxLevelWhitePaper), "System.Void Charm_VariableMaxLevelWhitePaper::RpcSetAdditionalMaxLevel(System.Int32)", InvokeUserCode_RpcSetAdditionalMaxLevel__Int32);
        }
    }
}
