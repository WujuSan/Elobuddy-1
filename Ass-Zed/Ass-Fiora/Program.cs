using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using Champion = Ass_Zed.Model.Champion;

namespace Ass_Zed
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameLoaded;
        }

        private static void GameLoaded(EventArgs args)
        {
            if (Player.Instance.ChampionName == "Zed")
            {
                new Champion().Init();
            }
        }
    }
}
