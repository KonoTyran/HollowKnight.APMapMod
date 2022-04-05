﻿using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using APMapMod.Settings;
using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using UnityEngine;
using Vasi;

namespace APMapMod.Map
{
    public class FullMap
    {
        public static void Hook()
        {
            ReplaceKnightBools();

            On.RoughMapRoom.OnEnable += RoughMapRoom_OnEnable;
            IL.GameMap.WorldMap += ModifyMapBools;
            IL.GameMap.SetupMap += ModifyMapBools;
            IL.RoughMapRoom.OnEnable += ModifyMapBools;
            On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
            ModHooks.GetPlayerBoolHook += BoolGetOverride;
        }

        public static void Unhook()
        {
            On.RoughMapRoom.OnEnable -= RoughMapRoom_OnEnable;
            IL.GameMap.WorldMap -= ModifyMapBools;
            IL.GameMap.SetupMap -= ModifyMapBools;
            IL.RoughMapRoom.OnEnable -= ModifyMapBools;
            On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
            ModHooks.GetPlayerBoolHook -= BoolGetOverride;
        }

        // The objects that make up the minimal map state
        private static readonly List<string> _persistentMapObjects = new()
        {
            "Crossroads_01",
            "Crossroads_02",
            "Crossroads_03",
            "Crossroads_07",
            "Crossroads_08",
            "Crossroads_10",
            "Crossroads_12",
            "Crossroads_13",
            "Crossroads_14",
            "Crossroads_16",
            "Crossroads_19",
            "Crossroads_21",
            "Crossroads_33",
            "Crossroads_39",
            "Crossroads_42",
            "Crossroads_48",
            "Waterways_01",
            "Waterways_02",
            "Waterways_04",
            "Waterways_04b",
            "Waterways_09",
            "Cliffs_01",
            "Cliffs_02",
            "Deepnest_East_03",
            "Deepnest_East_06",
            "Deepnest_East_07",
            "Fungus1_01",
            "Fungus1_01b",
            "Fungus1_02",
            "Fungus1_04",
            "Fungus1_06",
            "Fungus1_10",
            "Fungus1_19",
            "Fungus1_21",
            "Fungus1_30",
            "Fungus1_31",
            "Fungus1_32",
            "Fungus1_07",
            "Fungus3_01",
            "Fungus3_02",
            "Fungus3_25",
            "Fungus3_25b",
            "Fungus3_26",
            "Fungus3_27",
            "Fungus3_47",
            "Fungus2_01",
            "Fungus2_03",
            "Fungus2_04",
            "Fungus2_05",
            "Fungus2_06",
            "Fungus2_07",
            "Fungus2_08",
            "Fungus2_09",
            "Fungus2_10",
            "Fungus2_11",
            "Fungus2_18",
            "Fungus2_21",
            "Fungus1_24",
            "Fungus3_04",
            "Fungus3_05",
            "Fungus3_08",
            "Fungus3_10",
            "Fungus3_11",
            "Fungus3_34",
            "Deepnest_01b",
            "Deepnest_16",
            "Deepnest_17",
            "Fungus2_25",
            "Tutorial_01",
            "Town",
            "Crossroads_46b",
            "RestingGrounds_02",
            "RestingGrounds_04",
            "RestingGrounds_05",
            "Mines_01",
            "Mines_02",
            "Mines_03",
            "Mines_05",
            "Mines_11",
            "Mines_13",
            "Mines_19",
            "Mines_29",
            "Mines_30",
            "Abyss_03",
            "Abyss_04",
            "Abyss_05",
            "Abyss_18",
            "Ruins1_01",
            "Ruins1_02",
            "Ruins1_03",
            "Ruins1_05b",
            "Ruins1_05",
            "Ruins1_05c",
            "Ruins1_06",
            "Ruins1_09",
            "Ruins1_17",
            "Ruins1_27",
            "Ruins1_29",
            "Ruins1_28"
        };

        // We need to purge the map after turning RevealFullMap off. Essentially the opposite of SetupMap()
        public static void PurgeMap()
        {
            GameObject go_gameMap = GameManager.instance.gameMap;

            foreach (Transform areaObj in go_gameMap.transform)
            {
                if (areaObj.name == "Grub Pins"
                        || areaObj.name == "Dream_Gate_Pin"
                        || areaObj.name == "Compass Icon"
                        || areaObj.name == "Shade Pos"
                        || areaObj.name == "Flame Pins"
                        || areaObj.name == "Dreamer Pins"
                        || areaObj.name == "Map Markers"
                        || areaObj.name == "WHITE_PALACE"
                        || areaObj.name == "GODS_GLORY"
                        || areaObj.name == "AMM Custom Pin Group"
                        || areaObj.name == "AMM Custom Map Rooms") continue;

                foreach (Transform roomObj in areaObj.transform)
                {
                    roomObj.gameObject.SetActive(_persistentMapObjects.Contains(roomObj.name));

                    if (roomObj.name.Contains("Area Name"))
                    {
                        roomObj.gameObject.SetActive(true);
                    }

                    foreach (Transform roomObj2 in roomObj.transform)
                    {
                        if (roomObj2.name.Contains("Area Name"))
                        {
                            roomObj2.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        private class SpriteCopy : MonoBehaviour
        {
            public Sprite roughMap;
        }

        // Normally the game has no reason to "un-quill" the map, so the following allows us to do this
        // We keep a copy of the "rough map" sprite, and force-set that sprite every time before OnEnable() does its check
        private static void RoughMapRoom_OnEnable(On.RoughMapRoom.orig_OnEnable orig, RoughMapRoom self)
        {
            SpriteCopy spriteCopy = self.gameObject.GetComponent<SpriteCopy>();
            SpriteRenderer sr = self.gameObject.GetComponent<SpriteRenderer>();

            if (spriteCopy == null)
            {
                spriteCopy = self.gameObject.AddComponent<SpriteCopy>();
                spriteCopy.roughMap = self.gameObject.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                sr.sprite = spriteCopy.roughMap;
                self.fullSpriteDisplayed = false;
            }

            orig(self);
        }

        // Adds the prefix "AMM_" to each boolName occurrence directly in the original code
        private static void ModifyMapBools(ILContext il)
        {
            ILCursor cursor = new(il);

            while (cursor.TryGotoNext
            (
                i => i.MatchLdstr("hasQuill")
                || i.MatchLdstr("mapAllRooms")
                || i.MatchLdstr("mapAbyss")
                || i.MatchLdstr("mapCity")
                || i.MatchLdstr("mapCliffs")
                || i.MatchLdstr("mapCrossroads")
                || i.MatchLdstr("mapMines")
                || i.MatchLdstr("mapDeepnest")
                || i.MatchLdstr("mapFogCanyon")
                || i.MatchLdstr("mapFungalWastes")
                || i.MatchLdstr("mapGreenpath")
                || i.MatchLdstr("mapOutskirts")
                || i.MatchLdstr("mapRoyalGardens")
                || i.MatchLdstr("mapRestingGrounds")
                || i.MatchLdstr("mapWaterways")
            ))
            {
                string name = cursor.ToString().Split('\"')[1];
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "AMM_" + name);
            }
        }

        // This method adds the prefix "AMM_" to each boolName, so that we can control it in BoolGetOverride
        public static void ReplaceBool(PlayMakerFSM fsm, string stateName, int index)
        {
            string boolString = FsmUtil.GetAction<PlayerDataBoolTest>(fsm, stateName, index).boolName.ToString();
            FsmUtil.GetAction<PlayerDataBoolTest>(fsm, stateName, index).boolName = "AMM_" + boolString;
        }

        public static void ReplaceNumberOfBools(PlayMakerFSM fsm, string stateName, int number)
        {
            for (int i = 0; i < number; i++)
            {
                ReplaceBool(fsm, stateName, i);
            }
        }

        // Fixing some weird race condition issue where the hook below isn't added in time
        private static void ReplaceKnightBools()
        {
            if (HeroController.instance.gameObject != null)
            {
                PlayMakerFSM self = HeroController.instance.gameObject.LocateMyFSM("Map Control");

                ReplaceBool(self, "Button Down Check", 1);
                FsmUtil.GetAction<GetPlayerDataBool>(self, "Has Map?", 3).boolName = "AMM_hasMap";
            }
        }

        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            // Replace all instances of "hasMap" with "AMM_hasMap"
            if (self.gameObject.name == "Knight" && self.FsmName == "Map Control")
            {
                ReplaceBool(self, "Button Down Check", 1);
                FsmUtil.GetAction<GetPlayerDataBool>(self, "Has Map?", 3).boolName = "AMM_hasMap";
            }
            else if (self.gameObject.name == "Inventory" && self.FsmName == "Inventory Control")
            {
                ReplaceBool(self, "Next Map", 0);
                ReplaceBool(self, "Next Map 2", 0);
                ReplaceBool(self, "Next Map 3", 0);
            }

            // Patch the zoomed out map UI when we reveal the full map
            else if (self.gameObject.name == "World Map" && self.FsmName == "UI Control")
            {
                foreach (FsmState state in self.FsmStates)
                {
                    if (SettingsUtil.IsFSMMapState(state.Name))
                    {
                        ReplaceBool(self, state.Name, 0);
                        continue;
                    }

                    switch (state.Name)
                    {
                        case "Mi Right":
                        case "GP Up":
                        case "GP Right":
                        case "RG Up":
                        case "RG Right":
                        case "FG Left":
                        case "C Right":
                        case "Hive Down":
                        case "Wat Right":
                        case "Ab Right":
                            ReplaceNumberOfBools(self, state.Name, 1);
                            break;

                        case "QG Up":
                        case "Out Down":
                        case "Wat Down":
                            ReplaceNumberOfBools(self, state.Name, 2);
                            break;

                        case "FG Up":
                        case "C Up":
                        case "Out Up":
                        case "FW Up":
                        case "FW Down":
                            ReplaceNumberOfBools(self, state.Name, 3);
                            break;

                        case "QG Down":
                        case "C Down":
                        case "FW Left":
                        case "Wat Up":
                            ReplaceNumberOfBools(self, state.Name, 4);
                            break;

                        case "T Left":
                        case "T Right":
                        case "CR Right":
                        case "RG Left":
                        case "FG Down":
                        case "FW Right":
                        case "Wat Left":
                        case "Ab Left":
                            ReplaceNumberOfBools(self, state.Name, 5);
                            break;

                        case "CR Left":
                        case "RG Down":
                        case "FG Right":
                        case "C Left":
                        case "D Up":
                        case "Ab Up":
                            ReplaceNumberOfBools(self, state.Name, 6);
                            break;

                        case "CR Down":
                            ReplaceNumberOfBools(self, state.Name, 7);
                            break;

                        case "Mi Down":
                        case "GP Down":
                        case "QG Right":
                        case "Out Left":
                        case "D Right":
                        case "Pos Check":
                            ReplaceNumberOfBools(self, state.Name, 8);
                            break;

                        case "Cl Down":
                            ReplaceNumberOfBools(self, state.Name, 9);
                            break;

                        case "T Down":
                            ReplaceNumberOfBools(self, state.Name, 10);
                            break;

                        case "Activate":
                            FsmString[] boolStrings = { "AMM_visitedHive", "AMM_mapOutskirts" };
                            FsmUtil.GetAction<PlayerDataBoolAllTrue>(self, state.Name, 8).stringVariables = boolStrings;
                            break;
                    }
                }
            }

            // These are the map zone objects when zoomed out
            else if (SettingsUtil.IsFSMMapState(self.gameObject.name) && self.FsmName == "deactivate")
            {
                ReplaceBool(self, "Check", 0);
            }

            else if (self.FsmName == "Bench Control")
            {
                FsmUtil.GetAction<GetPlayerDataBool>(self, "Open Map", 0).boolName = "AMM_hasMap";
            }
        }

        public static bool BoolGetOverride(string boolName, bool orig)
        {
            // Always have a map when the mod is enabled
            if (boolName == "AMM_hasMap" && APMapMod.LS.ModEnabled)
            {
                return true;
            }

            if (boolName.StartsWith("AMM_"))
            {
                if (APMapMod.LS.ModEnabled &&
                    APMapMod.LS.mapMode == MapMode.FullMap)
                {
                    return true;
                }
                else
                {
                    return PlayerData.instance.GetBool(boolName.Remove(0, 4));
                }
            }

            return orig;
        }
    }
}