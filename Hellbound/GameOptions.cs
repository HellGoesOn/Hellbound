using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public static class GameOptions
    {
        private static int resolutionMultiplier = 4;

        public static int ResolutionMultiplier 
        {
            get => resolutionMultiplier;
            set
            {
                resolutionMultiplier = value;
                if(resolutionMultiplier <= 0)
                    resolutionMultiplier = 1;
                ScreenWidth = Renderer.PreferedWidth * resolutionMultiplier;
                ScreenHeight = Renderer.PreferedHeight * resolutionMultiplier;
            }
        }
        public static int ScreenWidth { get; set; } = 320;
        public static int ScreenHeight { get; set; } = 180;
        public static float GeneralVolume { get; set; } = 0.25f;
        public static float MusicVolume { get; set; } = 0.05f;
        public static float OldMusicVolume { get; set; } = 0.05f;

        public static Vector2 Resolution => new Vector2(ScreenWidth, ScreenHeight) * resolutionMultiplier;

        public static string WorldDirectory => Environment.CurrentDirectory + "\\Content\\Scenes\\";
        public static string PrefabDirectory => Environment.CurrentDirectory + "\\Content\\Prefabs\\";
    }
}
