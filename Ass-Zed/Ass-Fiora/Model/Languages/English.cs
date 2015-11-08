using Ass_Zed.Model.Enum;

namespace Ass_Zed.Model.Languages
{
    class English : LanguageController
    {
        public English()
        {
            Dictionary.Add(EnumContext.Version,            "Version: ");
            Dictionary.Add(EnumContext.Creator,            "By Vector");
            Dictionary.Add(EnumContext.Draw,               "Draw");
            Dictionary.Add(EnumContext.TurnOffDraws,       "Turn off all drawings");
            Dictionary.Add(EnumContext.Range,              "Range");
            Dictionary.Add(EnumContext.Combo,              "Combo");
            Dictionary.Add(EnumContext.Use,                "Use");
            Dictionary.Add(EnumContext.Harass,             "Harass");
            Dictionary.Add(EnumContext.MinimunMana,        "Minimum % Mana for Activate");
            Dictionary.Add(EnumContext.Settings,           "Settings");
            Dictionary.Add(EnumContext.ComboStyle, "Combo Style");
            Dictionary.Add(EnumContext.LastHit,            "Last Hit");
            Dictionary.Add(EnumContext.LaneClear,          "Lane Clear");
            Dictionary.Add(EnumContext.JungleClear, "Jungle Clear");
            Dictionary.Add(EnumContext.NotificationMessage," Loaded.");
        }
    }
}
