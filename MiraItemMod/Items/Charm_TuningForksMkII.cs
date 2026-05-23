using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace SephiriaMod.Items
{
    public class Charm_TuningForksMkII : Charm_Basic
    {
        public int[] damagePercentByLevel = new int[6] { 100, 140, 190, 250, 320, 400 };
        public int[] damagePercentByLevel2 = new int[6] { 40, 50, 60, 80, 100, 120 };

        public Timer cooldownTimer = new Timer(3f, resetOnTime: false);

        public string damageId = "Charm_TuningForks";

        public Color damageColor = Color.red;

        private bool enhancedWeaponDamage;

        private SkillController skillController;

        private int grimoire = 3;
        private int remain;
        public static int staggeringLevel = 1;
        public static float externalForcePower = 1f;
        public static string bulletName = "Grimoire_Big_B";

        [Header("Value HUD")]
        public string valueHUD_ID;
        public static uint AssetId
        {
            get
            {
                _assetId ??= SephiriaPrefabs.PallasBigBullet.GetComponent<NetworkIdentity>().assetId + 1;
                return _assetId.Value;
            }
        }
        private static uint? _assetId = null;
        public static Sprite bulletSprite;
        public static Sprite[] bulletDestroySprite = new Sprite[5];
        public static GameObject bulletClient;
        public static AnimationSet OldSet;
        static Charm_TuningForksMkII()
        {
            Core.InstantiateNetworkClientPatch.OnGetPrefab += OnGetPrefab;
            Core.InstantiateNetworkClientPatch.OnInstantiate += OnInstantiate;
            Events.CustomBulletDestroyModuleName[bulletName] = "PallasCard_Big_B";
            Events.CustomBulletDestroyModule[bulletName] = (instance, ownerNetId, canBeTransparentOnMultiplayer, position, height, angle) => instance.CreateDestroyVisualOnClient(ModifyDestroySpriteFx, ownerNetId, canBeTransparentOnMultiplayer, position, height, angle);
        }

        public static void ModifyDespawnSpriteFx(SpriteFx fx)
        {
            fx.gameObject.name = "PallasCard_Big_BFx(Clone)";
            var animator2 = fx.animator2D;
            animator2.ChangeSet(OldSet);
        }
        public static void ModifyDestroySpriteFx(SpriteFx fx)
        {
            //Core.Logger("ModifyDestroySpriteFx: " + fx);
            fx.gameObject.name = bulletName + "Fx_Modified(Clone)";
            var animator2 = fx.animator2D;
            //Core.Logger("animator2D: " + fx.animator2D);
            //Core.Logger("currentSet: " + animator2.currentSet);
            if (OldSet == null)
                OldSet = animator2.currentSet;
            var set = animator2.currentSet.Copy();
            var stateinfo2 = set.sprites;
            //Core.Logger("sprites: " + stateinfo2);
            
            if (stateinfo2.Count > 0)
            {
                var sprites = stateinfo2[0].timeline;
                for (int q = 0; q < sprites.Count; q++)
                {
                    sprites[q].sprite = bulletDestroySprite.SafeRandomAccess(q);
                }
            }
            animator2.currentSet = set;
            animator2.ChangeSet(set);
            /*
            var key2 = animator2.GetCurrentBakedKeyFrame();
            //Core.Logger("key2: " + key2);
            if (key2 != null)
            {
                foreach (var time in key2.timeline.Values)
                {
                    for (int q = 0; q < time.Count; q++)
                    {
                        time[q] = bulletDestroySprite.SafeRandomAccess(q);
                    }
                }
            }*/
            fx.onDespawned = () => ModifyDespawnSpriteFx(fx);
        }

        public void Start()
        {
            bulletSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "GrimoireBullet");
            bulletDestroySprite[0] = SpriteLoader.LoadSprite(ModUtil.ProjectilePath + "GrimoireDestroyBullet0");
            bulletDestroySprite[1] = SpriteLoader.LoadSprite(ModUtil.ProjectilePath + "GrimoireDestroyBullet1");
            bulletDestroySprite[2] = SpriteLoader.LoadSprite(ModUtil.ProjectilePath + "GrimoireDestroyBullet2");
            bulletDestroySprite[3] = SpriteLoader.LoadSprite(ModUtil.ProjectilePath + "GrimoireDestroyBullet3");
            bulletDestroySprite[4] = SpriteLoader.LoadSprite(ModUtil.ProjectilePath + "GrimoireDestroyBullet4");

            if (bulletClient == null)
            {
                var ob = Instantiate(SephiriaPrefabs.PallasBigBullet);
                Destroy(ob.GetComponent<NetworkIdentity>());
                var bullet = ob.GetComponent<Bullet>();
                ModifyPrefab(bullet);
                bullet.enabled = false;
                bulletClient = ob;
                bulletClient.gameObject.SetActive(false);
            }
        }

        private static GameObject OnInstantiate(GameObject original, Vector3 position, Quaternion rotation)
        {
            if (original.name != SephiriaPrefabs.PallasBigBullet.name)
                return null;
            if (bulletClient == null)
                return null;
            //Core.Logger("OnInstantiate: " + original.name);
            bulletClient.gameObject.SetActive(true);
            var ob = Instantiate(bulletClient, position, rotation);
            ob.AddComponent<NetworkIdentity>().SetAssetId(AssetId);
            ob.GetComponent<Bullet>().enabled = true;

            bulletClient.gameObject.SetActive(false);
            return ob;
        }

        private static GameObject OnGetPrefab(uint assetId)
        {
            if (assetId == AssetId)
            {
                //Core.Logger("OnGetPrefab: " + assetId);
                return SephiriaPrefabs.PallasBigBullet;
            }
            return null;
        }
        public static void ModifyPrefab(Bullet bullet)
        {
            //Core.Logger("ModifyPrefab: " + bullet.gameObject.name);
            bullet.spawnedPrefabName = bulletName;
            var wrapper = bullet.transform.GetChild(0);
            var body = wrapper.GetChild(0);
            var animator = body.GetComponent<Animator2D_SpriteRenderer>();

            var set = animator.currentSet.Copy();
            var stateinfo = set.sprites;
            if (stateinfo.Count > 0)
            {
                var sprites = stateinfo[0].timeline;
                for (int q = 0; q < sprites.Count; q++)
                {
                    sprites[q].sprite = bulletSprite;
                }
            }
            animator.currentSet = set;
            animator.ChangeSet(set);
            /*
            var key = animator.GetCurrentBakedKeyFrame();
            foreach (var time in key.timeline.Values)
            {
                for (int q = 0; q < time.Count; q++)
                {
                    time[q] = bulletSprite;
                }
            }*/
        }

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? damagePercentByLevel.SafeRandomAccess(0) + "→" + damagePercentByLevel.SafeRandomAccess(maxLevel) : damagePercentByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? damagePercentByLevel2.SafeRandomAccess(0) + "→" + damagePercentByLevel2.SafeRandomAccess(maxLevel) : damagePercentByLevel2.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("DAMAGEPERCENT", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString()),
            new Loc.KeywordValue("DAMAGE", value2, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", grimoire.ToString())
            };
        }

        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            skillController = NetworkAvatar.GetComponent<SkillController>();
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            skillController.OnBeginCastMagicServerside += OnBeginCastMagicServerside;
            NetworkAvatar.OnAttackUnit += HandleAttackUnit;
            WeaponController.OnBaisAttackSwing += OnAttack;
            WeaponController.OnSpecialAttackSwing += OnAttack;
            cooldownTimer.Ratio = 1f;
        }
        public void OnAttack(int idx)
        {
            bool log = false;
            if (log)
                Core.Logger("OnAttack1");
            if (remain > 0 && !NetworkAvatar.IsDead && IsEffectEnabled)
            {
                remain--;
                if (log)
                    Core.Logger("OnAttack2");
                int count = grimoire + NetworkAvatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
                var bulletDamage = (NetworkAvatar.GetCustomStat(ECustomStat.FireDamage) + NetworkAvatar.GetCustomStat(ECustomStat.IceDamage) + NetworkAvatar.GetCustomStat(ECustomStat.LightningDamage)) / 3f;
                bulletDamage *= damagePercentByLevel2.SafeRandomAccess(CurrentLevelToIdx()) / 100f;
                bulletDamage += bulletDamage * (NetworkAvatar.GetCustomStat(ECustomStat.MagicDamageBonus) / 100f);
                float anglePer = 6f;
                float startAngle = anglePer * (count - 1) * 0.5f;
                float angle = (WeaponController.aimedPositionClientside - WeaponController.transform.position).GetAngle();
                if (log)
                    Core.Logger("OnAttack3");
                for (int q = 0; q < count; q++)
                {
                    if (log)
                        Core.Logger("OnAttack4");
                    Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - startAngle + anglePer * q));
                    Vector3 motionDataBegin = NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                    Vector3 motionDataEnd = NetworkAvatar.transform.position + vector3FromAngle * 8f;
                    bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                    //Core.Logger("Prefab: " + (BulletPrefab != null));
                    if (log)
                        Core.Logger("OnAttack5");
                    Bullet bullet = Bullet.Pool.Spawn(ModifyPrefab, AssetId, SephiriaPrefabs.PallasBigBullet, NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, bulletDamage, staggeringLevel, externalForcePower, NetworkAvatar, NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.TopdownActor.CenterYPos, motionDataBegin, motionDataEnd, null, null);

                    if (log)
                        Core.Logger("OnAttack6");
                    bullet.useCustomColorType = Bullet.ECustomColorType.Chaos;
                    bullet.pierceCreatureCount = 3;
                    if (log)
                        Core.Logger("OnAttack7");
                    //ModifyPrefab(bullet);
                    Vector3 pos = NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                    bullet.SetSpeedScale(1.5f);
                    if (log)
                        Core.Logger("OnAttack8");

                    if (bullet.MoveModule is BulletMoveModule_FireworkHoming bulletMoveModule_FireworkHoming)
                    {
                        //bulletMoveModule_FireworkHoming.TurnOnFakeTarget(pos);
                    }
                }
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            skillController.OnBeginCastMagicServerside -= OnBeginCastMagicServerside;
            NetworkAvatar.OnAttackUnit -= HandleAttackUnit;
            WeaponController.OnBaisAttackSwing -= OnAttack;
            WeaponController.OnSpecialAttackSwing -= OnAttack;
            enhancedWeaponDamage = false;
            remain = 0;
            NetworkAvatar.DestroyEffectHUD(GetCharmHUDID());
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!cooldownTimer.Check() && !enhancedWeaponDamage)
            {
                cooldownTimer.AddTimer(Time.deltaTime + Time.deltaTime * NetworkAvatar.GetCustomStat(ECustomStat.CooldownRecoverySpeed) / 100f);
            }
        }

        private void OnBeginCastMagicServerside()
        {
            if (!enhancedWeaponDamage && cooldownTimer.Check())
            {
                cooldownTimer.Ratio = 0f;
                enhancedWeaponDamage = true;
                NetworkAvatar.CreateEffectHUD(valueHUD_ID, GetCharmHUDID());
                remain = 3;
            }
            if (enhancedWeaponDamage)
            {
                remain = 3;
            }
        }

        private void HandleAttackUnit(UnitAvatar target, DamageInstance damageInstance)
        {
            if (!NetworkAvatar.IsDead && IsEffectEnabled && enhancedWeaponDamage && damageInstance.fromType == EDamageFromType.DirectAttack && !target.IsDead)
            {
                enhancedWeaponDamage = false;
                NetworkAvatar.DestroyEffectHUD(GetCharmHUDID());
                float num = (NetworkAvatar.GetCustomStat(ECustomStat.FireDamage) + NetworkAvatar.GetCustomStat(ECustomStat.IceDamage) + NetworkAvatar.GetCustomStat(ECustomStat.LightningDamage)) / 3f;
                float num2 = damagePercentByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 100f;
                num *= num2;
                num += num * (NetworkAvatar.GetCustomStat(ECustomStat.MagicDamageBonus) * 0.01f);
                DamageInstance damage = DamageInstance.GetDamage(NetworkAvatar, damageId, target.transform.position, 4294967295L, num, EDamageType.Slice, EDamageFromType.None, Vector2.zero, 0, 0f);
                damage.elementalType = EDamageElementalType.Chaos;
                damage.SetCustomColor(changeElementalToChaos: true, damageColor);
                target.ApplyDamage(damage);
            }
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
