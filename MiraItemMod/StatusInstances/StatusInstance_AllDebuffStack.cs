using MiraItemMod.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.StatusInstances
{
    public class StatusInstance_AllDebuffStack : StatusInstance
    {
        protected override void ApplyStatusInner(bool runtime)
        {
            base.ApplyStatusInner(runtime);
            if (CurrentTarget == null)
                return;
            CurrentTarget.AddCustomStatUnsafe("BURNSTACK", Value);
            CurrentTarget.AddCustomStatUnsafe("ELECTRICSTACK", Value);
            CurrentTarget.AddCustomStatUnsafe("POISONSTACK", Value);
            CurrentTarget.AddCustomStatUnsafe("WOUNDSTACK", Value);
        }

        protected override void RemoveStatusInner()
        {
            base.RemoveStatusInner();
            if (CurrentTarget == null)
                return;
            CurrentTarget.AddCustomStatUnsafe("BURNSTACK", -Value);
            CurrentTarget.AddCustomStatUnsafe("ELECTRICSTACK", -Value);
            CurrentTarget.AddCustomStatUnsafe("POISONSTACK", -Value);
            CurrentTarget.AddCustomStatUnsafe("WOUNDSTACK", -Value);
        }
    }
}
