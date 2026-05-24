using Mirror;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Charm_Basic;

namespace MiraItemMod.Items
{
    public class Charm_AutoMagicDarkCloud : Charm_StatusInstance
    {
        public float[] cooldownByLevel = new float[] { 6, 6, 5, 5, 4, 3 };
        public int[] cloud = new int[] { 30, 27, 24, 20, 16, 12 };

        public Sprite magicCharmIconSprite;

        public Charm_Magic magicCharm;

        private EquippedMagicInstance slot;


        [Header("Cooldown")]
        private bool isInCooldown;

        private Timer cooldownTimer = new Timer(10f);

        public Timer castIntervalTimer = new Timer(4f);

        private bool isCasting;

        private Vector3 castingPosition = Vector3.zero;

        private SpriteFxAnimation_MagicCircle currentCastingCircle;

        private Timer currentCastingTimer = new Timer(0.6f);

        public TopdownActorRenderingMetadata TopdownActor;

        private int offset = -1;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? cooldownByLevel.SafeRandomAccess(0) + "→" + cooldownByLevel.SafeRandomAccess(maxLevel) : cooldownByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? cloud.SafeRandomAccess(0) + "→" + cloud.SafeRandomAccess(maxLevel) : cloud.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("COOLDOWN", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CLOUD", value2, GetNegativeColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            castIntervalTimer.time = cooldownByLevel.SafeRandomAccess(CurrentLevelToIdx());
            UpdateMagic();
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
        }

        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            UpdateMagic();
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            int idx = LevelToIdx(newLevel);
            castIntervalTimer.time = cooldownByLevel.SafeRandomAccess(idx);
        }

        private void UpdateMagic()
        {
            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, (sbyte)(Item.YIdx + offset)));
            magicCharm = null;
            slot = null;
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled && newItemOwnInstance.Charm is Charm_Magic charm_Magic)
            {
                magicCharm = charm_Magic;
                slot = new EquippedMagicInstance(magicCharm);
            }
            Inventory.UpdatePing(Item.Position);
        }

        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, (sbyte)(Item.YIdx + offset)));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled || !(newItemOwnInstance.Charm is Charm_Magic charm_Magic))
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(magicCharmIconSprite, new Vector2Int(Item.XIdx, Item.YIdx + offset))
                };
            }

            return null;
        }

        public override int GetSubIconCount()
        {
            return 1;
        }

        public override Sprite GetSubIconImage(ItemPosition pos, bool isInstance, int idx)
        {
            if (!isInstance)
            {
                return null;
            }

            if (!NetworkAvatar)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(pos.x, (sbyte)(pos.y + offset)));
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled && newItemOwnInstance.Charm is Charm_Magic charm_Magic)
            {
                return charm_Magic.ContainedMagic.icon;
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return new Vector2(4f, -4f);
        }

        public override bool Weaved()
        {
            return true;
        }
        private void Start()
        {
            TopdownActor = NetworkAvatar.GetComponent<TopdownActorRenderingMetadata>();
            magicCharmIconSprite = AssetLoader.LoadSprite(ModUtil.MiscPath + "MagicBookEmpty");
        }
        protected override void Update()
        {
            base.Update();
            if (!NetworkServer.active)
            {
                return;
            }

            if (NetworkAvatar == null || NetworkAvatar.IsDead || !NetworkAvatar.IsInBattle)
                return;
            if (magicCharm == null)
                return;
            if (NetworkAvatar is PlayerAvatar player)
            {
                var combo = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.DarkCloud);
                if (combo == null)
                    return;

                if(combo is ComboEffect_DarkCloud darkCloud && darkCloud.darkCloud >= cloud.SafeRandomAccess(CurrentLevelToIdx()))
                {
                    if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
                    {
                        isInCooldown = false;
                    }
                    if (isCasting)
                    {
                        if (currentCastingTimer.Update(Time.deltaTime) || NetworkAvatar.HasQuickCast())
                        {
                            isCasting = false;
                            isInCooldown = true;
                            cooldownTimer.SetTimer(0f);
                            cooldownTimer.time = magicCharm.NetworkcooldownTimeInThisCycle;
                            magicCharm.FireCasting(NetworkAvatar.transform.position, castingPosition, TopdownActor.CenterYPos, 1, true, false, null);
                            player.GetSkillController().SetLastUsedMagicServerside(magicCharm);
                            //Core.Logger("cast!" + (skillObject == null));
                            if ((bool)currentCastingCircle)
                            {
                                currentCastingCircle.FX.SetParent(null);
                                currentCastingCircle.EndRequest = true;
                                currentCastingCircle = null;
                            }
                            darkCloud.NetworkdarkCloud -= cloud.SafeRandomAccess(CurrentLevelToIdx());
                            if (darkCloud.NetworkdarkCloud < 0)
                                darkCloud.NetworkdarkCloud = 0;
                        }

                        return;
                    }

                    if (slot != null)
                    {
                        slot.Update(0);
                    }

                    //Core.Logger("waiting... " + castIntervalTimer.GetTimer());
                    if (slot != null && slot.HasAnyAmmo && !slot.CurrentUsing && magicCharm.CanCast(magicCharm.NetworkAvatar, true, false) == ECanUseSkillResult.Succeeded && !isInCooldown)
                    {
                        if (castIntervalTimer.Update(Time.deltaTime))
                        {
                            isCasting = true;
                            //Core.Logger("casting..." + (typeof(PlayerInputController).GetMethod("GetAimedPosition", System.Reflection.BindingFlags.NonPublic)));
                            castingPosition = player.NetworkaimObject.transform.position;
                            //Core.Logger("casting..." + (castingPosition));
                            SpriteFx spriteFx = string.IsNullOrEmpty(magicCharm.ContainedMagic.castingCircleOverride) ? SpriteFx.Pool.Spawn(ActiveSkillDatabase.FindCastingCircleByClass(magicCharm.ContainedMagic.GetMajorClass()), NetworkAvatar.transform.position + new Vector3(0f, 0.001f)) : SpriteFx.Pool.Spawn(magicCharm.ContainedMagic.castingCircleOverride, NetworkAvatar.transform.position + new Vector3(0f, 0.001f));

                            if (Core.LogMedium)
                                Core.Logger("casting..." + (spriteFx == null));
                            spriteFx.SetParent(NetworkAvatar.transform);
                            spriteFx.SetBodyYPos(TopdownActor.CenterYPos);
                            if ((bool)spriteFx.overrideAnimationTransition)
                            {
                                currentCastingCircle = spriteFx.overrideAnimationTransition as SpriteFxAnimation_MagicCircle;
                            }
                        }
                    }
                }
            }
        }
    }
}
