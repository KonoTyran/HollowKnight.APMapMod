﻿using APMapMod.Data;
using APMapMod.Map;
using APMapMod.Settings;
using APMapMod.Shop;
using APMapMod.Trackers;
using APMapMod.UI;
using Modding;
using System;
using System.Collections.Generic;
using System.Reflection;
using APMapMod.Util;
using Archipelago.MultiClient.Net;

namespace APMapMod
{
    public class APMapMod : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {
        public static APMapMod Instance;

        public bool ToggleButtonInsideMenu { get; }
        
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

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

            GUIController.Setup();
            
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

            if (GS.IconColorR == -1)
            {
                // default value lets randomize it!
                GS.IconColor= ColorUtil.GetRandomLightColor();
            }

            Log("Initialization complete.");
        }

        private void Hook()
        {
            Log("Activating mod");

            // Track when items are picked up/Geo Rocks are broken
            ItemTracker.Hook();
            GeoRockTracker.Hook();
            
            // Track when Hints are given in AP
            HintTracker.Hook();

            // Remove Map Markers from the Shop
            ShopChanger.Hook();

            // Modify overall Map behaviour
            WorldMap.Hook();

            // Modify overall Quick Map behaviour
            QuickMap.Hook();

            // Allow the full Map to be toggled
            FullMap.Hook();

            // Add a Pause Menu GUI, map text UI and transition helper text
            GUI.Hook();
            
            // Disable Vanilla Pins when mod is enabled
            PinsVanilla.Hook();

            // Immediately update Map on scene change
            Quill.Hook();

            // enable player icon tracking.
            CoOpMap.Hook();

            // Add keyboard shortcut control
            InputListener.InstantiateSingleton();
            
            Session = Archipelago.HollowKnight.Archipelago.Instance.session;
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
            CoOpMap.UnHook();
            InputListener.DestroySingleton();
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            return BetterMenu.GetMenuScreen(modListMenu, toggleDelegates);
        }
    }
}