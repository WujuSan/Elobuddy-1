using Ass_Zed.Model.Enum;

namespace Ass_Zed.Model.Languages
{
    class Espanol : LanguageController
    {
        public Espanol()
        {
            Dictionary.Add(EnumContext.Version, "Versión:");
            Dictionary.Add(EnumContext.Creator, "Por Vector");
            Dictionary.Add(EnumContext.Draw, "Dibujar");
            Dictionary.Add(EnumContext.TurnOffDraws, "Apague todos los dibujos");
            Dictionary.Add(EnumContext.Range, "Rango");
            Dictionary.Add(EnumContext.Combo, "Combo");
            Dictionary.Add(EnumContext.Use, "Usar");
            Dictionary.Add(EnumContext.Harass, "Acosar");
            Dictionary.Add(EnumContext.MinimunMana, "% Mínimo Mana para Activar");
            Dictionary.Add(EnumContext.Settings, "Ajustes");
            Dictionary.Add(EnumContext.ComboStyle, "Sólo en el rango pasivo");
            Dictionary.Add(EnumContext.LastHit, "Último golpe");
            Dictionary.Add(EnumContext.LaneClear, "Carril Clear");
            Dictionary.Add(EnumContext.NotificationMessage, "Cargado");
        }
    }
}
