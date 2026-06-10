using Mirror;
using Mirror.RemoteCalls;
using MiraItemMod.Items.Jewelry;
using MiraItemMod.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace MiraItemMod.Items.Savvy
{
    public class Charm_SavvyAcademy : Charm_FireBulletInRange
    {
        public LocalizedString itemName = new LocalizedString("Item_JewelryCoin_Name");
        public int[] mp = new int[] { 10 };

        public bool release;
        public bool released;
        public int per = 20;
        private void Awake()
        {
            coolDownTimer.time = 8f;
            //activeText = new LocalizedString("CharmActive_Effect_IceBow");
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (idx == 0 && released)
                return null;
            if (idx == 1 && released)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (mp.SafeRandomAccess(0).ToString() + "→" + mp.SafeRandomAccess(maxLevel)) : mp.SafeRandomAccess(LevelToIdx(level)).ToString());//"+0;-#"
            
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("MP", value, Charm_Basic.GetNegativeColor(virtualLevelOffset)),
            new Loc.KeywordValue("COIN", itemName.ToString()),
            new Loc.KeywordValue("PER", per.ToString()),
            new Loc.KeywordValue("TIME", coolDownTimer.time.ToString("0.#"))
            };
        }
        protected override void FireCastingServer(Vector3 aimedPosition, UnitAvatar aimedTarget)
        {
            if (this.GetChance())
            {
                this.SetChance(false);
                NetworkestimatedFireChanceTime = (float)NetworkTime.time;

                if (released)
                    return;
                Inventory.AddItem(new ItemMetadata(ItemDatabase.GenerateInstanceID(Data.JewelryRandom), Data.JewelryCoin.Id, 1));
                NetworkAvatar.NetworkreservedMp += mp.SafeRandomAccess(CurrentLevelToIdx());
                SaveItemOnServer(SaveManager.CurrentRun);
            }
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            if (Item == null)
                return;
            base.SaveItemOnServer(saveData);
            saveData.SetBool($"CharmSaveData_Jewelry_{Item?.InstanceID}_Released", released);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            if (Item == null)
                return;
            base.LoadItemOnServer(saveData);
            released = saveData?.GetBool($"CharmSaveData_Jewelry_{Item?.InstanceID}_Released", false) ?? false;
            RpcReleased(released);
        }
        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            Debug.Log("[SavvyAcademy] OnDisconnected");
        }
        protected override void Update()
        {
            base.Update();
            if (released)
                return;
            if(base.isServer && release)
            {
                Debug.Log("[SavvyAcademy] Release");
                using (new GridInventory.Permission(Inventory))
                {
                    var list = new List<KeyValuePair<ItemPosition, Charm_Basic>>(Inventory.charms);
                    for(int q= 0;q < list.Count; q++)
                    {
                        var charm = list[q];
                        if (charm.Value is Charm_JewelryCoin coin)
                        {
                            Inventory.ForceRemoveItem(charm.Key.x, charm.Key.y);
                        }
                    }
                }
                Debug.Log("[SavvyAcademy] ReservedMp: " + NetworkAvatar.reservedMp);
                for (int q = 0; q < NetworkAvatar.reservedMp / per; q++)
                {
                    Debug.Log("[SavvyAcademy] Add Jewelry");
                    this.AddRandomJewelry(true);
                }
                release = false;
                released = true;
                SaveItemOnServer(SaveManager.CurrentRun);
                RpcReleased(released);
            }
        }

        [ClientRpc]
        private void RpcReleased(bool released)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteBool(released);
            var func = "System.Void Charm_SavvyAcademy::RpcRelease(System.Boolean)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        public override bool Weaved()
        {
            return true;
        }

        protected void UserCode_RpcReleased__Boolean(bool released)
        {
            this.released = released;
        }

        protected static void InvokeUserCode_RpcReleased__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC CreateBurnExplosionFx called on server.");
            }
            else
            {
                ((Charm_SavvyAcademy)obj).UserCode_RpcReleased__Boolean(reader.ReadBool());
            }
        }

        static Charm_SavvyAcademy()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(Charm_SavvyAcademy), "System.Void Charm_SavvyAcademy::RpcRelease(System.Boolean)", InvokeUserCode_RpcReleased__Boolean);
        }
    }
}
