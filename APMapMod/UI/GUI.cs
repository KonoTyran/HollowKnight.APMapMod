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

            GUIController.Setup();
        }

        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.CloseQuickMap -= GameMap_CloseQuickMap;

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
    }
}