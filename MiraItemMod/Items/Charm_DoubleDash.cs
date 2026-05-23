using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_DoubleDash : Charm_StatusInstance
    {
        private bool dash = false;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnDashEndServerside += OnDashEndServerside;
        }
        private void OnDashEndServerside()
        {
            if (NetworkAvatar is PlayerAvatar player)
            {
                player.Delay(() =>
                {
                    if (!dash)
                    {
                        dash = true;
                        DungeonManager.Instance.Chat(NetworkAvatar as PlayerAvatar, "Mod", "/dash");
                        /*
                        if (NetworkAvatar.CurrentDashModule.currentDashCount > 0)
                            NetworkAvatar.CurrentDashModule.currentDashCount--;
                        NetworkAvatar.Dash(player.NetworkaimObject.transform.position);*/
                    }
                    else
                    {
                        dash = false;
                    }
                });
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnDashEndServerside -= OnDashEndServerside;
        }
    }
}
