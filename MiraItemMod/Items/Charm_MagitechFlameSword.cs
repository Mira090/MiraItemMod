using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_MagitechFlameSword : Charm_StatusInstance
    {
        public static string Status = "MagitechFlameSword".ToUpperInvariant();

        public int addition = 0;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Status, 1);
            addition = NetworkAvatar.GetCustomStatUnsafe("ELECTRICSTACK");
            NetworkAvatar.AddCustomStatUnsafe("FLAMESWORDADDITIONALATTACK", addition);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Status, -1);
            NetworkAvatar.AddCustomStatUnsafe("FLAMESWORDADDITIONALATTACK", -addition);
            addition = 0;
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            NetworkAvatar.AddCustomStatUnsafe("FLAMESWORDADDITIONALATTACK", -addition);
            addition = 0;
            addition = NetworkAvatar.GetCustomStatUnsafe("ELECTRICSTACK");
            NetworkAvatar.AddCustomStatUnsafe("FLAMESWORDADDITIONALATTACK", addition);
        }
        ///メモ
        /// MagitechFlameSwordの処理はここではなく、Charm_EmberFlameSwordのパッチで
    }
}
