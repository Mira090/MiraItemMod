using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Config
{
    [Serializable]
    public class ModConfig
    {
        public int Version = 0;
        public int LogMode = 1;

        public bool AddPassive => _options.GetBool("AddPassive");

        public bool AddWeapon => _options.GetBool("AddWeapon");
        public bool AddObsoletedWeapon => _options.GetBool("AddObsoletedWeapon");

        public bool AddMiracle => _options.GetBool("AddMiracle");
        public bool AddObsoletedMiracle => _options.GetBool("AddObsoletedMiracle");
        public bool ModifyMiracle => _options.GetBool("ModifyMiracle");

        public bool AddItem => _options.GetBool("AddItem");
        public bool AddVitality => _options.GetBool("AddVitality");
        public bool AddSkySong => _options.GetBool("AddSkySong");
        public bool AddStargaze => _options.GetBool("AddStargaze");
        public bool AddDrunk => _options.GetBool("AddDrunk");
        public bool AddFortune => _options.GetBool("AddFortune");
        public bool AddSacrifice => _options.GetBool("AddSacrifice");
        public bool AddJewelry => _options.GetBool("AddJewelry");
        public bool AddAcademy => _options.GetBool("AddAcademy");
        public bool AddCurse => _options.GetBool("AddCurse");
        public bool AddNegotiation => _options.GetBool("AddNegotiation");
        public bool AddCompanion => _options.GetBool("AddCompanion");
        public bool AddAlchemy => _options.GetBool("AddAlchemy");
        public bool AddMachine => _options.GetBool("AddMachine");
        public bool ModifyItem => _options.GetBool("ModifyItem");
        public bool ModifyBloodEarrings => _options.GetBool("ModifyBloodEarrings");

        public bool AddStoneTablet => _options.GetBool("AddStoneTablet");

        public float GearOpacity => _options.GetFloat("GearOpacity");


        private ModOptions _options;
        public void SetOptions(ModOptions options)
        {
            _options = options;
            InitOptions();
        }
        private void InitOptions()
        {
            AddToggleOption("AddPassive");
            AddToggleOption("AddWeapon");
            AddToggleOption("AddObsoletedWeapon", false);
            AddToggleOption("AddMiracle");
            AddToggleOption("AddObsoletedMiracle", false);
            AddToggleOption("ModifyMiracle");
            AddToggleOption("AddItem");
            AddToggleOption("AddVitality");
            AddToggleOption("AddSkySong");
            AddToggleOption("AddStargaze");
            AddToggleOption("AddDrunk");
            AddToggleOption("AddFortune");
            AddToggleOption("AddSacrifice");
            AddToggleOption("AddJewelry");
            AddToggleOption("AddAcademy");
            AddToggleOption("AddCurse");
            AddToggleOption("AddNegotiation");
            AddToggleOption("AddCompanion");
            AddToggleOption("AddAlchemy");
            AddToggleOption("AddMachine");
            AddToggleOption("ModifyItem");
            AddToggleOption("ModifyBloodEarrings");
            AddToggleOption("AddStoneTablet");

            if (!_options.HasKey("GearOpacity"))
                _options.SetFloat("GearOpacity", 0.5f);
            _options.AddSlider("GearOpacity", 0, 1, 0.5f, 0, new LocalizedString("UI_Option_GearOpacity"));
        }
        private void AddToggleOption(string name, bool value = true)
        {
            if (!_options.HasKey(name))
                _options.SetBool(name, value);
            _options.AddToggle(name, value, new LocalizedString("UI_Option_" + name));
        }
    }
}
