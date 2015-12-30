using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LevelZero.Model.Enuns;
using LevelZero.Model.Values;

namespace LevelZero.Model
{
    class Feature
    {
        public string NameFeature { get; set; }
        public Menu FeatureMenu { get; set; }
        public List<ValueAbstract> MenuValueStyleList { get; set; }

        public Feature()
        {
        }

        public Feature(string json)
        {
            var obj = (Feature) new JavaScriptSerializer().DeserializeObject(json);
            NameFeature = obj.NameFeature;
            MenuValueStyleList = obj.MenuValueStyleList;
        }

        public Feature(string nameFeature, List<ValueAbstract> menuValueStyleList)
        {
            NameFeature = nameFeature;
            MenuValueStyleList = menuValueStyleList;
        }

        public Menu ToMenu()
        {
            if (FeatureMenu != null) return FeatureMenu;

            FeatureMenu = Globals.MENU.AddSubMenu(NameFeature, Globals.ADDON_NAME + " - " + NameFeature);
            FeatureMenu.AddGroupLabel(NameFeature + Player.Instance.ChampionName);

            foreach (var valueAbstract in MenuValueStyleList)
            {
                switch (valueAbstract.EnumMenuStyle)
                {
                    case EnumMenuStyle.Slider:
                        MenuValueStyleList.Find(x => x.Identifier == valueAbstract.Identifier).CurrentValue =
                            ((ValueSlider) valueAbstract).InitialValue;
                        var currentMenuFeatureAxuSlider = FeatureMenu.Add(NameFeature + "." + valueAbstract.Identifier,
                            new Slider(valueAbstract.DisplayName, ((ValueSlider) valueAbstract).InitialValue,
                                ((ValueSlider) valueAbstract).MinValue, ((ValueSlider) valueAbstract).MaxValue));
                        currentMenuFeatureAxuSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                            {
                                MenuValueStyleList.Find(x => x.Identifier == valueAbstract.Identifier).CurrentValue
                                    =
                                    currentMenuFeatureAxuSlider.CurrentValue;
                            };
                        break;
                    case EnumMenuStyle.Checkbox:
                        MenuValueStyleList.Find(x => x.Identifier == valueAbstract.Identifier).CurrentValue =
                            ((ValueCheckbox)valueAbstract).InitialValue;
                        var currentMenuFeatureAuxCheckbox = FeatureMenu.Add(NameFeature + "." + valueAbstract.Identifier,
                            new CheckBox(valueAbstract.DisplayName, ((ValueCheckbox) valueAbstract).InitialValue));
                        currentMenuFeatureAuxCheckbox.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                            {
                                MenuValueStyleList.Find(x => x.Identifier == valueAbstract.Identifier).CurrentValue
                                    =
                                    currentMenuFeatureAuxCheckbox.CurrentValue;
                            };
                        break;
                }
            }

            return FeatureMenu;
        }

        public string ToJson()
        {
            return new JavaScriptSerializer().Serialize(this);
        }

        public object Find(string identifier)
        {
            var foundObject = MenuValueStyleList.FirstOrDefault(o => o.Identifier == identifier);
            return foundObject != null ? foundObject.CurrentValue : null;
        }
    }
}
