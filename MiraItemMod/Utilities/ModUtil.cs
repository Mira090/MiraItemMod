using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using SephiriaMod.Entities;
using SephiriaMod.Items;
using SephiriaMod.Registries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SephiriaMod.Utilities
{
    public static class ModUtil
    {
        public static readonly List<string> PlanetDamageIds = new List<string> { "Charm_Planet_Black", "Charm_Planet_Yellow", "Charm_Planet_White", "Charm_Planet_Red", "Charm_Planet_Blue", "Charm_Planet_Sky", "Charm_Planet_Gray" };

        public static readonly string ItemPath = "Item\\";
        public static readonly string ItemCategoryPath = "ItemCategory\\";
        public static readonly string MiraclePath = "Miracle\\";
        public static readonly string EffectHUDPath = "EffectHUD\\";
        public static readonly string MiscPath = "Misc\\";
        public static readonly string ProjectilePath = "Projectile\\";
        public static readonly string UIPath = "UI\\";
        public static readonly string WeaponPath = "Weapon\\";
        public static readonly string PassivePath = "Passive\\";

        public static readonly string KeywordPath = "Keyword\\";
        /// <summary>
        /// 「ToFileName」を「to_file_name」に変換する
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ToFileName(this string property)
        {
            var charas = property.ToCharArray();
            var sb = new StringBuilder();
            var offset = 'a' - 'A';
            foreach (var chara in charas)
            {
                if (chara >= 'A' && chara <= 'Z')
                {
                    if (sb.Length > 0)
                        sb.Append('_');
                    sb.Append((char)(chara + offset));
                }
                else
                {
                    sb.Append(chara);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 「ToFileName」を「TO_FILE_NAME」に変換する
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ToFileNameUpper(this string property)
        {
            return property.ToFileName().ToUpperInvariant();
        }
        /// <summary>
        /// 「ToFileName」を「TO_FILE_NAME」に変換する
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ToSephiriaId(this string property)
        {
            var charas = property.ToCharArray();
            var sb = new StringBuilder();
            foreach (var chara in charas)
            {
                if (chara >= 'A' && chara <= 'Z')
                {
                    if (sb.Length > 0)
                        sb.Append('_');
                    sb.Append(chara);
                }
                else
                {
                    sb.Append(chara);
                }
            }
            return sb.ToString().ToUpperInvariant();
        }
        /// <summary>
        /// 「To_File_Name」を「TOFILENAME」に変換する
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ToSephiriaUpperId(this string property)
        {
            return property.ToUpperInvariant().Replace("_", "");
        }
        public static void SetEnhancement(this Charm_PallasCard pallas, bool enhance)
        {
            if (enhance)
            {
                pallas.bulletDamage = 30;
                pallas.defaultChance = 100;
                pallas.throwChanceByLevel = new float[] { 0f, 5f, 10f, 15f, 20f};
                //pallas.throwChanceByLevel = new float[] {0f, 4f, 8f, 12f, 16f};
                pallas.throwIntervalTimer.time = 0.05f;
            }
            else
            {
                pallas.bulletDamage = 24;
                pallas.defaultChance = 50;
                pallas.throwChanceByLevel = new float[] { 0f, 2.5f, 5f, 7.5f, 10f };
                pallas.throwIntervalTimer.time = 0.1f;
            }
        }
        public static AnimationSet Copy(this AnimationSet instance)
        {
            var set = ScriptableObject.CreateInstance<AnimationSet>();
            set.name = instance.name + "_Copied";
            set.sprites = instance.sprites.Select(sprite => (AnimationSet.StateInfo)sprite.Clone()).ToList();
            return set;
        }
        public static T Spawn<T>(this ProjectileSpawner<T> spawner, Action<T> modify, uint assetId, GameObject key, Vector3 pos, bool canBeTransparentOnMultiplayer, EDamageFromType fromType, string damageId, float damage, int staggeringLevel, float externalForcePower, UnitAvatar owner, long targetTeam, float height, Vector2 motionDataBegin, Vector2 motionDataEnd, List<CombatBehaviour> sharedTarget, Action<CombatBehaviour, DamageInstance, ProjectileBase> onAttack, float rangeBonus = 0f, EDamageElementalType elementalType = EDamageElementalType.Physical, bool showSwingFx = true) where T : ProjectileBase
        {
            if (key == null)
            {
                Debug.LogError("키는 Null이 될 수 없습니다.");
                return null;
            }

            GameObject gameObject = UnityEngine.Object.Instantiate(key, pos, Quaternion.identity);
            gameObject.GetComponent<NetworkIdentity>().SetAssetId(assetId);
            T component = gameObject.GetComponent<T>();
            //Core.Logger("Msg1: " + gameObject.GetComponentCount());
            modify.Invoke(component);
            component.OnSpawn();
            component.Initialize(canBeTransparentOnMultiplayer, fromType, damageId, damage, staggeringLevel, externalForcePower, owner, targetTeam, height, motionDataBegin, motionDataEnd, sharedTarget, onAttack, rangeBonus, elementalType);
            component.showSwingFx = showSwingFx;
            //Core.Logger("Msg2: " + gameObject.GetComponentCount());
            NetworkServer.Spawn(gameObject);
            component.OnSpawnFinalized();
            //component.spawnedPrefabName = key.name;
            return component;
        }
        public static GameDataLoader GetGameDataLoader()
        {
            var instance = typeof(GameDataLoader);
            return (GameDataLoader)instance.GetField("instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(instance);
        }

        public static void CreateDestroyVisualOnClient(this BulletDestroyModule instance, Action<SpriteFx> consumer, uint ownerNetId, bool canBeTransparentOnMultiplayer, Vector3 position, float height, float angle)
        {
            if (!instance.destroySound.IsNull)
            {
                RuntimeManager.PlayOneShot(instance.destroySound, position);
            }

            if ((bool)instance.destroyFxPrefab)
            {
                for (int i = 0; i < instance.destroyFxCount; i++)
                {
                    GameObject key = instance.destroyFxPrefab;
                    if (instance.destroyFxPrefabList.Count > 0)
                    {
                        key = instance.destroyFxPrefabList[Random.Range(0, instance.destroyFxPrefabList.Count)];
                    }

                    Vector2 vector = Random.insideUnitCircle * instance.destroyFxErrorRadius;
                    SpriteFx spriteFx = SpriteFx.Pool.Spawn(key, position + (Vector3)vector);
                    spriteFx.SetBodyYPos(height);
                    float num = Random.Range((0f - instance.randomFxAngleError) * 0.5f, instance.randomFxAngleError * 0.5f);
                    if (instance.lookAtBulletDirection)
                    {
                        spriteFx.SetRotation(new Vector3(0f, 0f, angle + num));
                    }
                    else
                    {
                        spriteFx.SetRotation(new Vector3(0f, 0f, num));
                    }

                    if (instance.randomFxScaleX)
                    {
                        spriteFx.transform.localScale = new Vector3(Mathf.Sign(Random.Range(-1, 1)), 1f, 1f);
                    }

                    if (canBeTransparentOnMultiplayer && (bool)GameCamera.Instance && (bool)GameCamera.Instance.Observer)
                    {
                        int ownerIndex = GameCamera.Instance.Observer.netId == ownerNetId ? 1 : 0;
                        spriteFx.SetTransparentOnMultiplayer(isTransparent: true, ownerIndex);
                    }
                    consumer?.Invoke(spriteFx);
                }
            }

            if (instance.breakFxFragments.Count <= 0)
            {
                return;
            }

            foreach (Unit_LibraryLivingStatue_Fragment breakFxFragment in instance.breakFxFragments)
            {
                UnityEngine.Object.Instantiate(breakFxFragment.gameObject, position + breakFxFragment.transform.localPosition, Quaternion.identity).SetActive(value: true);
            }
        }
        public static ushort ToFunctionHashCode(this string function)
        {
            return (ushort)((uint)function.GetStableHashCode() & 0xFFFFu);
        }
        public static float GetEvasionPercent(int evasion)
        {
            if (evasion > 10000)
                evasion = 10000;
            return 100f * Mathf.Log(evasion / 6200f + 1f) * 0.8f;
        }
        public static bool IsJewelry(this ItemEntity entity)
        {
            return entity is ItemEntity_Jewelry;
        }
        public static void SetStatus(this UI_StatusTooltipOpener opener, StatusEntity entity)
        {
            opener.statHookID = entity.id;
            opener.statKeyword = entity.statKeyword;
            opener.SetKeyword(KeywordDatabase.GetEntity(opener.statKeyword));
            if(opener.TryGetComponent<UI_StatusName>(out var name))
            {
                name.statKeyword = entity.statKeyword;
                name.Initialize();
            }
        }
        public static bool HasQuickCast(this UnitAvatar avatar)
        {
            if (avatar == null)
                return false;
            return avatar.GetCustomStatUnsafe("MAGICQUICKCAST") > 0;
        }

        #region RPC Miracle Controller

        [ClientRpc]
        public static void RpcSetMaxMiracleCount(this UnitAvatar avatar, int count)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteInt(count);
            var func = "System.Void ModUtil::RpcSetMaxMiracleCount(System.Int32)";
            avatar.InvokeSendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        public static void UserCode_SetMaxMiracleCount__Int32(this UnitAvatar avatar, int count)
        {
            if (avatar.gameObject.TryGetComponent<MiracleController>(out var miracle))
            {
                miracle.maxMiracleCount = count;
                if(Core.LogMedium)
                Core.Logger($"Set {avatar.Name}'s maxMiracleCount: {count}");
            }
        }

        public static void InvokeUserCode_SetMaxMiracleCount__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC SetMaxMiracleCount called on server.");
            }
            else
            {
                ((UnitAvatar)obj).UserCode_SetMaxMiracleCount__Int32(reader.ReadInt());
            }
        }

        static ModUtil()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(UnitAvatar), "System.Void ModUtil::RpcSetMaxMiracleCount(System.Int32)", InvokeUserCode_SetMaxMiracleCount__Int32);
        }
        #endregion

        #region region Delay関数
        /// <summary>
        /// 1フレーム待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(callback));
        }
        /// <summary>
        /// Delay秒待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, float delay, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(delay, callback));
        }
        /// <summary>
        /// predicateを満たすまで待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="predicate"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, Func<bool> predicate, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(predicate, callback));
        }
        /// <summary>
        /// predicateを満たすまで待って、Delay秒待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="predicate"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, Func<bool> predicate, float delay, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(predicate, delay, callback));
        }
        /// <summary>
        /// コルーチンが終了するまで待った後に処理する
        /// </summary>
        /// <param name="script"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour script, IEnumerator delay, Action callback)
        {
            return script.StartCoroutine(DelayEnumerator(delay, callback));
        }
        private static IEnumerator DelayEnumerator(Action callback)
        {
            yield return null;
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(Func<bool> predicate, Action callback)
        {
            yield return new WaitUntil(predicate);
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(Func<bool> predicate, float delay, Action callback)
        {
            yield return new WaitUntil(predicate);
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
        private static IEnumerator DelayEnumerator(IEnumerator delay, Action callback)
        {
            yield return delay;
            callback.Invoke();
        }
        #endregion

        #region 乱数関連
        public static bool Percent(this int percent)
        {
            return Random.Range(0, 100) < percent;
        }
        public static bool Percent(this float percent)
        {
            return Random.Range(0, 100f) <= percent;
        }
        public static bool Chance(this float probability)
        {
            return Random.Range(0, 1f) <= probability;
        }
        #endregion

        public static string ToJapanese(this EItemType type)
        {
            return type switch
            {
                EItemType.Charm => new LocalizedString("ItemType_Charm").ToString(),
                EItemType.StoneTablet => new LocalizedString("ItemType_StoneTablet").ToString(),
                EItemType.Potion => new LocalizedString("ItemType_Potion").ToString(),
                EItemType.Scroll => new LocalizedString("ItemType_Scroll").ToString(),
                EItemType.ThrowingWeapon => new LocalizedString("ItemType_ThrowingWeapon").ToString(),
                EItemType.Identifiable => new LocalizedString("ItemType_Identifiable").ToString(),
                EItemType.Food => new LocalizedString("ItemType_Food").ToString(),
                EItemType.Misc => new LocalizedString("ItemType_Misc").ToString(),
                _ => new LocalizedString("ItemType_Misc").ToString() + "?",
            };
        }
        public static string ToJapanese(this EItemRarity type)
        {
            return type switch
            {
                EItemRarity.Common => new LocalizedString("ItemRarity_Common").ToString(),
                EItemRarity.Uncommon => new LocalizedString("ItemRarity_Uncommon").ToString(),
                EItemRarity.Rare => new LocalizedString("ItemRarity_Rare").ToString(),
                EItemRarity.Legend => new LocalizedString("ItemRarity_Legend").ToString(),
                EItemRarity.Eternal => new LocalizedString("ItemRarity_Eternal").ToString(),
                (EItemRarity)ECustomItemRarity.Sacrifice => new LocalizedString("ItemRarity_Sacrifice").ToString(),
                _ => "???",
            };
        }
        public static string ToJapanese(this EItemActiveType type)
        {
            return type switch
            {
                EItemActiveType.Default => "Default",
                EItemActiveType.Hidden => "Hidden",
                EItemActiveType.PocketDimensionShop => "PocketDimensionShop",
                EItemActiveType.Locked => "Locked",
                EItemActiveType.Disabled => "Disabled",
                EItemActiveType.TestOnly => "TestOnly",
                _ => "???",
            };
        }
        public static string ToNoTag(this string text)
        {
            if(string.IsNullOrWhiteSpace(text))
                return text;
            var sb = new StringBuilder();
            bool tag = false;
            foreach (var c in text)
            {
                if (c == '<')
                {
                    tag = true;
                }
                if (!tag)
                    sb.Append(c);
                if (c == '>')
                {
                    tag = false;
                }
            }
            return sb.ToString();
        }
        public static string GUIDToPath(this EventReference eventRef)
        {
            if (eventRef.IsNull)
                return null;
            string path;
            RuntimeManager.StudioSystem.lookupPath(eventRef.Guid, out path);
            return path;
        }
        public static string GUIDToPath(this FMOD.GUID guid)
        {
            string path;
            RuntimeManager.StudioSystem.lookupPath(guid, out path);
            return path;
        }
        public static bool IsMagicExecution(this DamageInstance damage)
        {
            if (!damage.useCustomColor)
                return false;
            return damage.color.a == 0 && damage.color.r == 1 && damage.color.g == 1 && damage.color.b == 1;
        }
        public static bool IsAssasination(this DamageInstance damage)
        {
            if (!damage.useCustomColor)
                return false;
            return damage.color.a == 0 && damage.color.r == 1 && damage.color.g == 0 && damage.color.b == 0;
        }
        public static bool IsFourGradation(this DamageInstance damage)
        {
            if (!damage.useCustomColor)
                return false;
            return damage.color == FourGradation;
        }
        public static bool IsExcavation(this DamageInstance damage)
        {
            if (!damage.useCustomColor)
                return false;
            return damage.color == Excavation;
        }
        public static bool IsExcavationFaild(this DamageInstance damage)
        {
            if (!damage.useCustomColor)
                return false;
            return damage.color == ExcavationFaild;
        }
        public static Color FourGradation = new Color(0, 1, 0, 0);
        public static Color FourGradationMagicExecution = new Color(0, 0, 1, 0);
        public static Color32 Excavation = new Color32(255, 255, 0, 0);
        public static Color32 ExcavationFaild = new Color32(255, 100, 0, 0);

        public static float GetCharmDamageBonus(this Charm_Basic charm, float damage)
        {
            return damage * charm.RequestCharmDamageBonusOnRoot() / 100f;
        }

        public static float CalculateDamage(Charm_ActiveMeteor charm)
        {
            if (charm.netIdentity == null || charm.netIdentity.netId == 0)
            {
                return 0f;
            }

            float num = 0f;
            if (charm is IAttackableCharm attackableCharm && attackableCharm.IsAttackableCharm())
            {
                num = charm.GetDamage(charm.NetworkAvatar);
            }

            int num2 = charm.RequestCharmDamageBonusOnRoot();
            return num + num * (float)num2 / 100f;
        }
        public static float CalculateDamage(Charm_SelfExplosion charm)
        {
            if (charm.netIdentity == null || charm.netIdentity.netId == 0)
            {
                return 0f;
            }

            float num = 0f;
            if (charm is IAttackableCharm attackableCharm && attackableCharm.IsAttackableCharm())
            {
                num = charm.GetDamage(charm.NetworkAvatar);
            }

            int num2 = charm.RequestCharmDamageBonusOnRoot();
            return num + num * (float)num2 / 100f;
        }

        public static T[] Array<T>(params T[] array)
            => array;
    }
}
