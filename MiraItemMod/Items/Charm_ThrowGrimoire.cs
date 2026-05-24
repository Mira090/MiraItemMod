using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    [Obsolete]
    public class Charm_ThrowGrimoire : Charm_Basic
    {
        public string damageId = "Charm_ThrowGrimoire";

        public float[] throwChanceByLevel = new float[1] { 50f };

        public float defaultChance = 10f;

        public GameObject BulletPrefab => SephiriaPrefabs.PallasBigBullet;

        public float bulletDamage = 100f;

        public int staggeringLevel = 1;

        public float externalForcePower = 1f;

        public Timer throwIntervalTimer = new Timer(0.1f, resetOnTime: false);

        public Sprite bulletSprite;
        private void Start()
        {
            bulletSprite = AssetLoader.LoadSprite(ModUtil.MiscPath + "GrimoireBullet");
        }

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            float num = 1f;
            if ((bool)avatar && avatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                num = component.currentWeapon.AttackWeightPerSwing;
            }

            string value = showAllLevel ? (throwChanceByLevel.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (throwChanceByLevel.SafeRandomAccess(maxLevel) * num).ToString("0.#") + "%" : (throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * num).ToString("0.#") + "%";
            if ((bool)avatar)
            {
                float num2 = defaultChance * num;
                float num3 = num2 + throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * Mathf.Clamp(avatar.GetCustomStat(ECustomStat.Luck), 0, 9999) * num;
                return new Loc.KeywordValue[4]
                {
                new Loc.KeywordValue("DEFAULT", num2.ToString("0.#")),
                new Loc.KeywordValue("CHANCE", value, GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("DAMAGE", bulletDamage.ToString()),
                new Loc.KeywordValue("CURRENT", num3.ToString("0.#") + "%")
                };
            }

            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("DEFAULT", defaultChance.ToString()),
            new Loc.KeywordValue("CHANCE", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", bulletDamage.ToString())
            };
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if ((bool)WeaponController)
            {
                WeaponControllerSimple weaponController = WeaponController;
                weaponController.OnBeginAttackAnimation += OnBeginAttackAnimation;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if ((bool)WeaponController)
            {
                WeaponControllerSimple weaponController = WeaponController;
                weaponController.OnBeginAttackAnimation -= OnBeginAttackAnimation;
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!throwIntervalTimer.Check())
            {
                throwIntervalTimer.AddTimer(Time.deltaTime);
            }
        }

        private void OnBeginAttackAnimation(int idx)
        {
            if (!WeaponController || !WeaponController.currentWeapon || !throwIntervalTimer.Check())
            {
                return;
            }

            int count = UnityEngine.Random.Range(4, 10);
            float anglePer = 4f;
            float startAngle = anglePer * (count - 1) * 0.5f;
            float angle = (WeaponController.aimedPositionClientside - WeaponController.transform.position).GetAngle();
            for (int q = 0; q < count; q++)
            {
                Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - startAngle + anglePer * q));
                Vector3 motionDataBegin = NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                Vector3 motionDataEnd = NetworkAvatar.transform.position + vector3FromAngle * 8f;
                bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                //Core.Logger("Prefab: " + (BulletPrefab != null));
                Bullet bullet = Bullet.Pool.Spawn(BulletPrefab, NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, bulletDamage / count, staggeringLevel, externalForcePower, NetworkAvatar, NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.TopdownActor.CenterYPos, motionDataBegin, motionDataEnd, null, null);
                ModifyPrefab(bullet);
                Vector3 pos = NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                bullet.SetSpeedScale(3);

                if (bullet.MoveModule is BulletMoveModule_FireworkHoming bulletMoveModule_FireworkHoming)
                {
                    //bulletMoveModule_FireworkHoming.TurnOnFakeTarget(pos);
                }
            }

            throwIntervalTimer.Ratio = 0f;
        }
        public void ModifyPrefab(Bullet bullet)
        {
            //Core.Logger("0: " + (bullet != null));
            var wrapper = bullet.transform.GetChild(0);
            //Core.Logger("1: " + (wrapper != null));
            var body = wrapper.GetChild(0);
            //Core.Logger("2: " + (body != null));
            var animator = body.GetComponent<Animator2D_SpriteRenderer>();
            //Core.Logger("3: " + (animator != null));
            //Core.Logger("4: " + (animator.currentSet != null));
            var stateinfo = animator.currentSet.sprites;
            //Core.Logger("5: " + (stateinfo != null));
            if (stateinfo.Count > 0)
            {
                var sprites = stateinfo[0].timeline;
                //Core.Logger("6: " + (sprites.Count));
                for (int q = 0; q < sprites.Count; q++)
                {
                    sprites[q].sprite = bulletSprite;
                }
            }
            var key = animator.GetCurrentBakedKeyFrame();
            foreach(var time in key.timeline.Values)
            {
                for (int q = 0; q < time.Count; q++)
                {
                    time[q] = bulletSprite;
                }
            }
            var destroy = bullet.gameObject.GetComponent<BulletDestroyModule_DestroyImmediate>();
            //animator.ChangeSet(animator.currentSet);
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
