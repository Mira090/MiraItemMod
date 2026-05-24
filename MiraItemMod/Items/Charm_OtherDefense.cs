using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SocialPlatforms;

namespace MiraItemMod.Items
{
    public class Charm_OtherDefense : Charm_StatusInstance
    {
        public int[] defense = new int[] { 1, 2, 4, 8, 10, 12 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? defense.SafeRandomAccess(0) + "→" + defense.SafeRandomAccess(maxLevel) : defense.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
                new Loc.KeywordValue("DEFENSE", "+" + value, GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            foreach (var value in NetworkServer.connections.Values)
            {
                if (value != null && value.identity != null && value.identity.gameObject != null && value.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    if (player == NetworkAvatar)
                        continue;
                    player.AddCustomStat(ECustomStat.DamageReduction, defense.SafeRandomAccess(CurrentLevelToIdx()));
                }
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            foreach (var value in NetworkServer.connections.Values)
            {
                if (value != null && value.identity != null && value.identity.gameObject != null && value.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    if (player == NetworkAvatar)
                        continue;
                    player.AddCustomStat(ECustomStat.DamageReduction, -defense.SafeRandomAccess(CurrentLevelToIdx()));
                }
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            foreach (var value in NetworkServer.connections.Values)
            {
                if (value != null && value.identity != null && value.identity.gameObject != null && value.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    if (player == NetworkAvatar)
                        continue;
                    player.AddCustomStat(ECustomStat.DamageReduction, -defense.SafeRandomAccess(LevelToIdx(oldLevel)));
                    player.AddCustomStat(ECustomStat.DamageReduction, defense.SafeRandomAccess(LevelToIdx(newLevel)));
                }
            }
        }
    }
}
