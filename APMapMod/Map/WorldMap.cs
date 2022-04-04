using GlobalEnums;
using APMapMod.Data;
using APMapMod.Settings;
using APMapMod.Trackers;
using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace APMapMod.Map
{
    public static class WorldMap
    {
        public static GameObject goCustomPins = null;
        public static PinsCustom CustomPins => goCustomPins.GetComponent<PinsCustom>();

        public static GameObject goExtraRooms = null;

        public static void Hook()
        {
            On.GameMap.Start += GameMap_Start;
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GameMap.WorldMap += GameMap_WorldMap;
            On.GameMap.SetupMapMarkers += GameMap_SetupMapMarkers;
            On.GameMap.DisableMarkers += GameMap_DisableMarkers;
            On.GameManager.UpdateGameMap += GameManager_UpdateGameMap;
        }

        public static void Unhook()
        {
            On.GameMap.Start -= GameMap_Start;
            On.GameManager.SetGameMap -= GameManager_SetGameMap;
            On.GameMap.WorldMap -= GameMap_WorldMap;
            On.GameMap.SetupMapMarkers -= GameMap_SetupMapMarkers;
            On.GameMap.DisableMarkers -= GameMap_DisableMarkers;
            On.GameManager.UpdateGameMap -= GameManager_UpdateGameMap;
        }

        // Called every time when a new GameMap is created (once per save load)
        private static void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
        {
            orig(self);

            try
            {
                DataLoader.SetUsedPinDefs();

                if (APMapMod.LS.NewSettings || APMapMod.LS.PoolGroupSettings.Count == 0)
                {
                    APMapMod.LS.InitializePoolGroupSettings();
                    APMapMod.LS.NewSettings = true;
                }
            }
            catch (Exception e)
            {
                APMapMod.Instance.LogError(e);
            }
        }

        // Called every time after a new GameMap is created (once per save load)
        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject go_gameMap)
        {
            orig(self, go_gameMap);
            
            GameMap gameMap = go_gameMap.GetComponent<GameMap>();

            if (goCustomPins != null)
            {
                goCustomPins.GetComponent<PinsCustom>().DestroyPins();
                UnityEngine.Object.Destroy(goCustomPins);
            }

            APMapMod.Instance.Log("Adding Custom Pins...");

            goCustomPins = new GameObject($"MMS Custom Pin Group");
            goCustomPins.AddComponent<PinsCustom>();

            // Setting parent here is only for controlling local position,
            // not active/not active (need separate mechanism)
            goCustomPins.transform.SetParent(go_gameMap.transform);

            CustomPins.MakePins(gameMap);

            CustomPins.GetRandomizedOthersGroups();

            if (APMapMod.LS.NewSettings)
            {
                CustomPins.ResetPoolSettings();
            }

            CustomPins.UpdatePins(MapZone.NONE, new());

            APMapMod.Instance.Log("Adding Custom Pins done.");

            APMapMod.LS.NewSettings = false;
        }

        // Called every time we open the World Map
        private static void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);

            // Easiest way to force AdditionalMaps custom areas to show
            if (APMapMod.LS.ModEnabled
                && APMapMod.LS.mapMode == MapMode.FullMap)
            {
                foreach (Transform child in self.transform)
                {
                    if (child.name == "WHITE_PALACE"
                        || child.name == "GODS_GLORY")
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }

            try
            {
                UpdateMap(self, MapZone.NONE);
            }
            catch (Exception e)
            {
                APMapMod.Instance.LogError(e);
            }
        }

        // Following two behaviours necessary since GameMap is actually persistently active
        private static void GameMap_SetupMapMarkers(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);

            if (!APMapMod.LS.ModEnabled || goCustomPins == null) return;

            goCustomPins.SetActive(true);

            // For debugging purposes
            //if (goExtraRooms != null)
            //{
            //    goExtraRooms.SetActive(true);
            //}
        }

        private static void GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            if (goCustomPins != null)
            {
                goCustomPins.SetActive(false);
            }

            if (goExtraRooms != null)
            {
                goExtraRooms.SetActive(false);
            }

            orig(self);
        }

        // Remove the "Map Updated" idle animation, since it occurs when the return value is true
        public static bool GameManager_UpdateGameMap(On.GameManager.orig_UpdateGameMap orig, GameManager self)
        {
            orig(self);

            return false;
        }

        // The main method for updating map objects and pins when opening either World Map or Quick Map
        public static void UpdateMap(GameMap gameMap, MapZone mapZone)
        {
            ItemTracker.UpdateObtainedItems();

            HashSet<string> transitionPinScenes = new();

            FullMap.PurgeMap();
            gameMap.SetupMap();

            if (goCustomPins == null || !APMapMod.LS.ModEnabled) return;

            gameMap.panMinX = -29f;
            gameMap.panMaxX = 26f;
            gameMap.panMinY = -25f;
            gameMap.panMaxY = 20f;

            PinsVanilla.ForceDisablePins(gameMap.gameObject);

            CustomPins.UpdatePins(mapZone, transitionPinScenes);
            CustomPins.ResizePins("None selected");
            CustomPins.SetPinsActive();
            CustomPins.SetSprites();
        }
    }
}