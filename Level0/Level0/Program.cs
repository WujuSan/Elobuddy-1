using System;
using System.Collections.Generic;
using System.IO;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using LevelZero.Model;
using LevelZero.Model.Values;

namespace LevelZero
{
    class Program
    {
        private static Feature _drawFeature;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameLoaded;
        }

        private static void GameLoaded(EventArgs args)
        {
            Console.WriteLine("Version 1.3");

            var feature = new Feature {NameFeature = "Draw"};
            var MenuValueStyleList = new List<ValueAbstract>
            {
                new ValueCheckbox(false, "disable", "Disable"),
                new ValueCheckbox(false, "draw.q", "Draw Q"),
                new ValueCheckbox(false, "draw.w", "Draw W"),
                new ValueCheckbox(false, "draw.e", "Draw E"),
                new ValueCheckbox(false, "draw.r", "Draw R")
            };

            feature.MenuValueStyleList = MenuValueStyleList;

            string path = @"c:\temp\drawFeature.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(feature.ToJson());
                }
            }

            feature.ToMenu();

            _drawFeature = feature;

            Game.OnTick += Ontick;
        }

        private static void Ontick(EventArgs args)
        {
            if ((bool) _drawFeature.Find("disable"))
            {
                Console.WriteLine("Opa");
            }
        }
    }
}
