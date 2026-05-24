using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using static LibraryFloorGenerator;

namespace MiraItemMod.Utilities
{
    public static class ReflectionExtensions
    {
        public static SkillController GetSkillController(this PlayerAvatar player)
        {
            return typeof(PlayerAvatar).GetField("skillController", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) as SkillController;
        }
        public static Dictionary<int, ItemEntity> GetItemDictionary()
        {
            return typeof(ItemDatabase).GetField("itemDictionary", BindingFlags.Static | BindingFlags.NonPublic).GetValue(typeof(ItemDatabase)) as Dictionary<int, ItemEntity>;
        }
        public static bool GetSweepRequest(this WeaponSimple_GreatSword instance)
        {
            return (bool)typeof(WeaponSimple_GreatSword).GetField("sweepRequest", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static bool GetWritePermission(this GridInventory instance)
        {
            return (bool)typeof(GridInventory).GetField("writePermission", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static KeyFrameData GetCurrentBakedKeyFrame(this Animator2D_Basic instance)
        {
            return (KeyFrameData)typeof(Animator2D_Basic).GetField("currentBakedKeyFrame", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static bool GetEnhancedWeaponDamage(this Charm_TuningForks instance)
        {
            return (bool)typeof(Charm_TuningForks).GetField("enhancedWeaponDamage", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void SetAssetId(this NetworkIdentity instance, uint assetId)
        {
            var prop = instance.GetType().GetProperty(nameof(NetworkIdentity.assetId));
            prop.SetValue(instance, assetId);
        }
        public static void SetNetIdentity(this NetworkBehaviour instance, NetworkIdentity identity)
        {
            var prop = instance.GetType().GetProperty(nameof(NetworkBehaviour.netIdentity));
            prop.SetValue(instance, identity);
        }
        public static float GetGreatsworSwingSpeed(this PlayerAvatar instance)
        {
            return (float)typeof(PlayerAvatar).GetField("greatsworSwingSpeed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static WeaponControllerSimple GetWeaponController(this PlayerAvatar instance)
        {
            return (WeaponControllerSimple)typeof(PlayerAvatar).GetField("weaponController", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static PlayerAvatarCostume GetCurrentCostumeObject(this PlayerAvatar instance)
        {
            return (PlayerAvatarCostume)typeof(PlayerAvatar).GetField("currentCostumeObject", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void SetCurrentCostumeObject(this PlayerAvatar instance, PlayerAvatarCostume value)
        {
            instance.GetType().GetField("currentCostumeObject", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        public static UnitAvatar GetAvatar(this ChargingCharm instance)
        {
            return (UnitAvatar)instance.GetType().GetField("avatar", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static UnitAvatar GetTarget(this UI_MpBar instance)
        {
            return (UnitAvatar)instance.GetType().GetField("target", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static PlayerAvatar GetPlayer(this UI_MultiplayerHPBar instance)
        {
            return (PlayerAvatar)instance.GetType().GetField("player", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static KatanaGhost GetSummonGhost(this WeaponAddonKatana_SummonGhost instance)
        {
            return (KatanaGhost)instance.GetType().GetField("summonedGhost", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void InvokeOnEvade(this UnitAvatar instance, DamageInstance damage)
        {
            var type = typeof(UnitAvatar);
            var field = type.GetField(nameof(instance.OnEvade), BindingFlags.Instance | BindingFlags.NonPublic);
            var del = (Delegate)field.GetValue(instance);
            del.DynamicInvoke(damage);
        }
        public static void InvokeAddFury(this WeaponSimple_Dagger instance, int fury)
        {
            var type = typeof(WeaponSimple_Dagger);
            var method = type.GetMethod("AddFury", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { fury });
        }
        public static void InvokeCreateFuryChargedParticle(this WeaponSimple_Dagger instance, Color color)
        {
            var type = typeof(WeaponSimple_Dagger);
            var method = type.GetMethod("CreateFuryChargedParticle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { color });
        }
        public static void InvokeTargetFuryChargedMessage(this WeaponSimple_Dagger instance, NetworkConnectionToClient client)
        {
            var type = typeof(WeaponSimple_Dagger);
            var method = type.GetMethod("TargetFuryChargedMessage", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { client });
        }
        public static bool GetIsCooldown(this ComboEffect_FlameSword instance)
        {
            return (bool)instance.GetType().GetField("isCooldown", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void SetIsCooldown(this ComboEffect_FlameSword instance, bool value)
        {
            instance.GetType().GetField("isCooldown", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        public static void SetCooldownTimer(this ComboEffect_FlameSword instance, float value)
        {
            instance.GetType().GetField("cooldownTimer", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        public static IEnumerator InvokeCreateFlameSword(this ComboEffect_FlameSword instance, Vector3 motionTo)
        {
            var type = typeof(ComboEffect_FlameSword);
            var method = type.GetMethod("CreateFlameSword", BindingFlags.Instance | BindingFlags.NonPublic);
            return (IEnumerator)method.Invoke(instance, new object[] { motionTo });
        }
        public static void SetDefaultSpriteAsset(this TMP_Settings instance, TMP_SpriteAsset value)
        {
            instance.GetType().GetField("m_defaultSpriteAsset", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        public static GreenBat GetGreenbatObject(this Charm_SummonGreenBat instance)
        {
            return (GreenBat)instance.GetType().GetField("greenbatObject", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static Charm_SummonGreenBat GetCharm(this GreenBat instance)
        {
            return (Charm_SummonGreenBat)instance.GetType().GetField("charm", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static UnitAvatar GetUnitAvatar(this AvatarStatsHooker instance)
        {
            return (UnitAvatar)instance.GetType().GetField("unitAvatar", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static bool GetIsHitInvincibleEnabled(this UnitAvatar instance)
        {
            return (bool)typeof(UnitAvatar).GetField("isHitInvincibleEnabled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static float GetInvincibleTimer(this UnitAvatar instance)
        {
            return (float)typeof(UnitAvatar).GetField("invincibleTimer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static EInvincibleType GetInvincibleType(this UnitAvatar instance)
        {
            return (EInvincibleType)typeof(UnitAvatar).GetField("invincibleType", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static Timer GetRestoreFallingTimer(this UnitAvatar instance)
        {
            return (Timer)typeof(UnitAvatar).GetField("restoreFallingTimer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static bool GetChance(this Charm_Reddew instance)
        {
            return (bool)instance.GetType().GetField("chance", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static TextMeshProUGUI GetText(this UI_DamageParticle instance)
        {
            return (TextMeshProUGUI)instance.GetType().GetField("text", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void InvokeAttack(this CharacterDebuff_Electric instance, UnitAvatar target, int stack, float damageRatio)
        {
            var type = typeof(CharacterDebuff_Electric);
            var method = type.GetMethod("Attack", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { target, stack, damageRatio });
        }
        public static void InvokeAttack(this CharacterDebuff_Plasma instance, UnitAvatar target, int stack, float damageRatio)
        {
            var type = typeof(CharacterDebuff_Plasma);
            var method = type.GetMethod("Attack", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { target, stack, damageRatio });
        }
        public static void SetLastUsedMagicServerside(this SkillController instance, Charm_Magic value)
        {
            instance.GetType().GetField("lastUsedMagicServerside", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        public static ElementalParticle GetLoopElementalParticle(this CharacterDebuff instance)
        {
            return (ElementalParticle)typeof(CharacterDebuff).GetField("loopElementalParticle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static FreezeFx GetFreezeFx(this CharacterDebuff_Freeze instance)
        {
            return (FreezeFx)typeof(CharacterDebuff_Freeze).GetField("freezeFx", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static Timer GetStunCooldownTimer(this UnitAvatar instance)
        {
            return (Timer)typeof(UnitAvatar).GetField("stunCooldownTimer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void InvokeMakePool<T>(this ObjectPoolingFactory<T> instance, Transform parent, GameObject prefab) where T : ObjectPoolable
        {
            var type = typeof(ObjectPoolingFactory<T>);
            var method = type.GetMethod("MakePool", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { parent, prefab });
        }
        public static MiracleController GetActor(this UI_MiracleElement instance)
        {
            return (MiracleController)typeof(UI_MiracleElement).GetField("actor", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static MiracleMetadata GetEntity(this UI_MiracleElement instance)
        {
            return (MiracleMetadata)typeof(UI_MiracleElement).GetField("entity", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static MiracleSelector2 GetMiracleSelector(this UI_MiracleElement instance)
        {
            return (MiracleSelector2)typeof(UI_MiracleElement).GetField("miracleSelector", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void SetIsPicked(this FlameSwordPickLocal instance, bool value)
        {
            instance.GetType().GetField("isPicked", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        public static ComboEffect_FlameSword GetComboEffect(this FlameSwordPickLocal instance)
        {
            return (ComboEffect_FlameSword)typeof(FlameSwordPickLocal).GetField("comboEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static List<FlameSwordPickLocal> GetPickList(this ComboEffect_FlameSword instance)
        {
            return (List<FlameSwordPickLocal>)typeof(ComboEffect_FlameSword).GetField("pickList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static bool GetChance(this Charm_FireBulletInRange instance)
        {
            return (bool)typeof(Charm_FireBulletInRange).GetField("chance", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void SetChance(this Charm_FireBulletInRange instance, bool value)
        {
            typeof(Charm_FireBulletInRange).GetField("chance", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        public static void InvokeOnDamagedServerside(this UnitAvatar instance, DamageInstance damage)
        {
            var type = typeof(UnitAvatar);
            var field = type.GetField(nameof(instance.OnDamagedServerside), BindingFlags.Instance | BindingFlags.NonPublic);
            var del = (Delegate)field.GetValue(instance);
            del.DynamicInvoke(damage);
        }
        public static TMP_Text GetNameText(this UI_CharmTooltip instance)
        {
            return (TMP_Text)typeof(UI_CharmTooltip).GetField("nameText", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static TMP_Text GetTypeText(this UI_CharmTooltip instance)
        {
            return (TMP_Text)typeof(UI_CharmTooltip).GetField("typeText", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void InvokeSendRPCInternal(this NetworkBehaviour instance, string functionFullName, int functionHashCode, NetworkWriter writer, int channelId, bool includeOwner)
        {
            var type = typeof(NetworkBehaviour);
            var method = type.GetMethod("SendRPCInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { functionFullName, functionHashCode, writer, channelId, includeOwner });
        }
        public static void SetKeyword(this UI_StatusTooltipOpener instance, KeywordEntity value)
        {
            typeof(UI_StatusTooltipOpener).GetField("keyword", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }



        #region ECustomItemRarity関連

        public static Dictionary<EItemRarity, LocalizedString> GetRarityNames()
        {
            return (Dictionary<EItemRarity, LocalizedString>)typeof(ItemDatabase).GetField("rarityNames", BindingFlags.Static | BindingFlags.NonPublic).GetValue(typeof(ItemDatabase));
        }
        public static Dictionary<EItemRarity, Color> GetItemColorByRarity()
        {
            return (Dictionary<EItemRarity, Color>)typeof(ItemDatabase).GetField("itemColorByRarity", BindingFlags.Static | BindingFlags.NonPublic).GetValue(typeof(ItemDatabase));
        }
        public static List<UI_JournalPanel_SearchOptionButton> GetRarityOptionButtons(this UI_DimensionPocketPanel instance)
        {
            return (List<UI_JournalPanel_SearchOptionButton>)instance.GetType().GetField("rarityOptionButtons", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static List<UI_JournalPanel_SearchOptionButton> GetRarityOptionButtons(this UI_JournalContent_Item instance)
        {
            return (List<UI_JournalPanel_SearchOptionButton>)instance.GetType().GetField("rarityOptionButtons", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static MysticPot GetConnectedPot(this UI_MysticPotPanel instance)
        {
            return (MysticPot)instance.GetType().GetField("connectedPot", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void SetIsReadyToMix(this UI_MysticPotPanel instance, bool value)
        {
            instance.GetType().GetField("isReadyToMix", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        }
        #endregion
    }
}
