using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Vitality
{
    public class Charm_ReviveOnce : Charm_StatusInstance
    {
        public bool isInCooldown;
        public float remain = 1;
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
            if ((bool)DungeonManager.Instance)
            {
                DungeonManager.Instance.RpcBroadcastChintamani(base.NetworkAvatar);
            }

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
    }
}
