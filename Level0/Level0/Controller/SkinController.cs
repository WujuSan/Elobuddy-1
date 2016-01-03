using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LevelZero.Model;
using LevelZero.Model.Values;

namespace LevelZero.Controller
{
    class SkinController
    {
        public Feature feature;

        public SkinController(int QuantityOfSkins)
        {
            var feature = new Feature
            {
                NameFeature = "Skin Changer",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true, "skin.enable", "Enable Skin Changer"),
                    new ValueSlider(QuantityOfSkins, 0, 0, "skin.id", "Skin chooser")
                }
            };

            feature.ToMenu();
            this.feature = feature;

            Game.OnTick += OnTick;
        }

        private void OnTick(EventArgs args)
        {
            if (!feature.IsChecked("skin.enable")) return;

            if (Player.Instance.SkinId != feature.SliderValue("skin.id"))
            {
                Player.SetSkinId(feature.SliderValue("skin.id"));
            }
        }
    }
}
