using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_AutoBuff : Charm_Basic
    {
        public LocalizedString semanticName = new LocalizedString("ItemType_Charm_Magic");
        public string semantic = "GRIMOIRE";
        public int semanticDropWeight = 2;

        public float[] cooldownByLevel = new float[2] { 8f, 5f };

        public Sprite magicCharmIconSprite;

        public Charm_Magic magicCharm;

        private EquippedMagicInstance slot;

        private ActiveSkill_Buff skill;



        [Header("Cooldown")]
        private bool isInCooldown;

        private Timer cooldownTimer = new Timer(10f);

        public Timer castIntervalTimer = new Timer(4f);

        private bool isCasting;

        private Vector3 castingPosition = Vector3.zero;

        private SpriteFxAnimation_MagicCircle currentCastingCircle;

        private Timer currentCastingTimer = new Timer(0.6f);

        public TopdownActorRenderingMetadata TopdownActor;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? cooldownByLevel.SafeRandomAccess(0) + "→" + cooldownByLevel.SafeRandomAccess(maxLevel) : cooldownByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("ITEM_TYPE", semanticName.ToString()),
            new Loc.KeywordValue("DROP_PERCENT", (semanticDropWeight * 50).ToString()),
            new Loc.KeywordValue("COOLDOWN", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            castIntervalTimer.time = cooldownByLevel.SafeRandomAccess(CurrentLevelToIdx());
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight);
            UpdateMagic();
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight);
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
            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, (sbyte)(Item.YIdx + 1)));
            magicCharm = null;
            slot = null;
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled && newItemOwnInstance.Charm is Charm_Magic charm_Magic && charm_Magic.ContainedMagic.magicPrefab.GetComponent<ActiveSkill>() is ActiveSkill_Buff skill)
            {
                this.skill = skill;
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

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, (sbyte)(Item.YIdx + 1)));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled || !(newItemOwnInstance.Charm is Charm_Magic charm_Magic) || !(charm_Magic.ContainedMagic.magicPrefab.GetComponent<ActiveSkill>() is ActiveSkill_Buff))
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(magicCharmIconSprite, new Vector2Int(Item.XIdx, Item.YIdx + 1))
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

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(pos.x, (sbyte)(pos.y + 1)));
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled && newItemOwnInstance.Charm is Charm_Magic charm_Magic && charm_Magic.ContainedMagic.magicPrefab.GetComponent<ActiveSkill>() is ActiveSkill_Buff)
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
            magicCharmIconSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "MagicBookEmpty");
        }
        protected override void Update()
        {
            base.Update();
            if (!NetworkServer.active)
            {
                return;
            }
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
                    castingPosition = NetworkAvatar.transform.position;
                    magicCharm.FireCasting(NetworkAvatar.transform.position, castingPosition, TopdownActor.CenterYPos, 1, true, true, null);
                    //Core.Logger("cast!" + (skillObject == null));
                    if ((bool)currentCastingCircle)
                    {
                        currentCastingCircle.FX.SetParent(null);
                        currentCastingCircle.EndRequest = true;
                        currentCastingCircle = null;
                    }
                }

                return;
            }

            if (slot != null)
            {
                slot.Update(0);
            }

            if (NetworkAvatar == null || NetworkAvatar.IsDead || !NetworkAvatar.IsInBattle)
            {
                return;
            }

            //Core.Logger("waiting... " + castIntervalTimer.GetTimer());
            if (slot != null && slot.HasAnyAmmo && !slot.CurrentUsing && castIntervalTimer.Update(Time.deltaTime) && !isInCooldown)
            {
                isCasting = true;
                SpriteFx spriteFx = string.IsNullOrEmpty(magicCharm.ContainedMagic.castingCircleOverride) ? SpriteFx.Pool.Spawn(ActiveSkillDatabase.FindCastingCircleByClass(magicCharm.ContainedMagic.GetMajorClass()), NetworkAvatar.transform.position + new Vector3(0f, 0.001f)) : SpriteFx.Pool.Spawn(magicCharm.ContainedMagic.castingCircleOverride, NetworkAvatar.transform.position + new Vector3(0f, 0.001f));
                spriteFx.SetParent(NetworkAvatar.transform);
                spriteFx.SetBodyYPos(TopdownActor.CenterYPos);
                //Core.Logger("casting..." + (spriteFx == null));
                if ((bool)spriteFx.overrideAnimationTransition)
                {
                    currentCastingCircle = spriteFx.overrideAnimationTransition as SpriteFxAnimation_MagicCircle;
                }
            }
        }
    }
}
