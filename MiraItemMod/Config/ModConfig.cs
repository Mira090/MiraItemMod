using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Config
{
    [Serializable]
    public class ModConfig
    {
        public bool AddPassive = true;

        public bool AddWeapon = true;
        public bool AddObsoletedWeapon = false;
        
        public bool AddMiracle = true;
        public bool AddObsoletedMiracle = false;
        public bool ModifyMiracle = true;

        public bool AddItem = true;
        public bool AddVitality = true;
        public bool AddSkySong = true;
        public bool AddStargaze = true;
        public bool AddDrunk = true;
        public bool AddFortune = true;
        public bool AddSacrifice = true;
        public bool AddJewelry = true;
        public bool AddAcademy = true;
        public bool AddCurse = true;
        public bool AddNegotiation = true;
        public bool AddCompanion = true;
        public bool ModifyItem = true;

        public bool AddStoneTablet = true;
    }
}
