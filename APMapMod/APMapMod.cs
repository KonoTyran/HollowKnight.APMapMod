using APMapMod.Data;
using APMapMod.Map;
using APMapMod.Settings;
using APMapMod.Shop;
using APMapMod.Trackers;
using APMapMod.UI;
using Modding;
using System;
using System.Collections.Generic;
using System.Reflection;
using Archipelago.MultiClient.Net;
using UnityEngine;

namespace APMapMod
{
    public class APMapMod : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        public static APMapMod Instance;

        public override string GetVersion() => "0.2.0";

        public override int LoadPriority() => 10;

        public static LocalSettings LS = new();
        public void OnLoadLocal(LocalSettings ls) => LS = ls;
        public LocalSettings OnSaveLocal() => LS;

        public static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        internal ArchipelagoSession Session;

        internal CoOpMap CoOpMap;

        public override void Initialize()
        {
            Log("Initializing...");

            Instance = this;

            Dependencies.GetDependencies();

            foreach (KeyValuePair<string, Assembly> pair in Dependencies.strictDependencies)
            {
                if (pair.Value == null)
                {
                    Log($"{pair.Key} is not installed. APMapMod disabled");
                    return;
                }
            }

            foreach (KeyValuePair<string, Assembly> pair in Dependencies.optionalDependencies)
            {
                if (pair.Value == null)
                {
                    Log($"{pair.Key} is not installed. Some features are disabled.");
                }
            }

            try
            {
                SpriteManager.LoadEmbeddedPngs("APMapMod.Resources.Pins");
            }
            catch (Exception e)
            {
                LogError($"Error loading sprites!\n{e}");
                throw;
            }

            try
            {
                DataLoader.Load();
            }
            catch (Exception e)
            {
                LogError($"Error loading data!\n{e}");
                throw;
            }
            
            Archipelago.HollowKnight.Archipelago.OnArchipelagoGameStarted += Hook;
            Archipelago.HollowKnight.Archipelago.OnArchipelagoGameEnded += Unhook;
            On.GameManager.SetGameMap += GameManager_SetGameMap;

            Log("Initialization complete.");
        }

        private void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject goGameMap)
        {
            orig(self, goGameMap);

            Log("Fetching MultiClient Session.");
            try
            {
                var ap = Archipelago.HollowKnight.Archipelago.Instance;
                var apType = ap.GetType();
                var prop = apType.GetField("session", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                Session = (ArchipelagoSession) prop.GetValue(ap);
                Log("Success, enabling Co-Op integration.");
            }
            catch (Exception)
            {
                Log("Error Fetching Session, disabling Co-Op integration.");
            }
            
            CoOpMap = goGameMap.AddComponent<CoOpMap>();
        }

        private void Hook()
        {
            Log("Activating mod");

            // Track when items are picked up/Geo Rocks are broken
            ItemTracker.Hook();
            GeoRockTracker.Hook();

            // Remove Map Markers from the Shop
            ShopChanger.Hook();

            // Modify overall Map behaviour
            WorldMap.Hook();

            // Modify overall Quick Map behaviour
            QuickMap.Hook();

            // Allow the full Map to be toggled
            FullMap.Hook();

            // Disable Vanilla Pins when mod is enabled
            PinsVanilla.Hook();

            // Immediately update Map on scene change
            Quill.Hook();

            // Add a Pause Menu GUI, map text UI and transition helper text
            GUI.Hook();

            // Add keyboard shortcut control
            InputListener.InstantiateSingleton();
        }

        private void Unhook()
        {
            ItemTracker.Unhook();
            GeoRockTracker.Unhook();
            ShopChanger.Unhook();
            WorldMap.Unhook();
            QuickMap.Unhook();
            FullMap.Unhook();
            PinsVanilla.Unhook();
            Quill.Unhook();
            GUI.Unhook();
            InputListener.DestroySingleton();
        }
    }
}