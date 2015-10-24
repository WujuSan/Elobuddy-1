using Ass_Fiora.Model.Enum;

namespace Ass_Fiora.Model.Languages
{
    class Deutsch : LanguageController
    {
        public Deutsch()
        {
            Dictionary.Add(EnumContext.Version, "Version: ");
            Dictionary.Add(EnumContext.Creator, "Von Vector");
            Dictionary.Add(EnumContext.Draw, "Zeichnen");
            Dictionary.Add(EnumContext.TurnOffDraws, "Alle Zeichnungen ausschalten");
            Dictionary.Add(EnumContext.Range, "Reichweite");
            Dictionary.Add(EnumContext.Combo, "Combo");
            Dictionary.Add(EnumContext.Use, "Benutzen");
            Dictionary.Add(EnumContext.Harass, "Harass");
            Dictionary.Add(EnumContext.MinimunMana, "Mindestens % Mana zum aktivieren");
            Dictionary.Add(EnumContext.Settings, "Einstellungen");
            Dictionary.Add(EnumContext.OnlyInPassiverange, "Nur in Passivreichweite");
            Dictionary.Add(EnumContext.LastHit, "LastHit");
            Dictionary.Add(EnumContext.LaneClear, "LaneClear");
            Dictionary.Add(EnumContext.NotificationMessage, " Loaded.");
        }
    }
}
