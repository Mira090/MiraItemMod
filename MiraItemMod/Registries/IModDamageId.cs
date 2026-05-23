using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Registries
{
    public interface IModDamageId
    {
        public ModDamageId DamageId { get; }
        public DamageIdEntity DamageIdEntity { get; }
        public bool HasDamageId => DamageIdEntity != null;
    }
}
