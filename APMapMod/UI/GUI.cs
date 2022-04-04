using APMapMod.Settings;
using UnityEngine.SceneManagement;

namespace APMapMod.UI
{
    public static class GUI
    {
        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.CloseQuickMap += GameMap_CloseQuickMap;
            On.HeroController.Pause += HeroController_Pause;
            On.HeroController.UnPause += HeroController_UnPause;

            GUIController.Setup();
        }

        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.CloseQuickMap -= GameMap_CloseQuickMap;
            On.HeroController.Pause -= HeroController_Pause;
            On.HeroController.UnPause -= HeroController_UnPause;

            GUIController.Unload();
        }
        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);
                
            GUIController.Instance.BuildMenus();
        }

        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            MapText.Show();
        }

        private static void GameMap_CloseQuickMap(On.GameMap.orig_CloseQuickMap orig, GameMap self)
        {
            orig(self);

            MapText.Hide();
        }

        private static void HeroController_Pause(On.HeroController.orig_Pause orig, HeroController self)
        {
            orig(self);
        }

        private static void HeroController_UnPause(On.HeroController.orig_UnPause orig, HeroController self)
        {
            orig(self);
        }
    }
}