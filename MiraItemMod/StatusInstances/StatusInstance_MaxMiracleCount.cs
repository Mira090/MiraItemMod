using Mirror;
using Mirror.RemoteCalls;
using SephiriaMod.Items;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.StatusInstances
{
    public class StatusInstance_MaxMiracleCount : StatusInstance
    {
        protected override void SetTargetInner(UnitAvatar target)
        {
            base.SetTargetInner(target);
        }

        protected override void ApplyStatusInner(bool runtime)
        {
            base.ApplyStatusInner(runtime);
            if (CurrentTarget.gameObject.TryGetComponent<MiracleController>(out var miracle))
            {
                miracle.maxMiracleCount += Value;
                CurrentTarget.RpcSetMaxMiracleCount(miracle.maxMiracleCount);
            }
        }

        protected override void RemoveStatusInner()
        {
            base.RemoveStatusInner();
            if (CurrentTarget.gameObject.TryGetComponent<MiracleController>(out var miracle))
            {
                miracle.maxMiracleCount -= Value;
                CurrentTarget.RpcSetMaxMiracleCount(miracle.maxMiracleCount);
            }
        }

    }
}
