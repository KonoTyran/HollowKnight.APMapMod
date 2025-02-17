﻿using System;
using GlobalEnums;
using ItemChanger;
using System.Collections.Generic;
using System.Linq;
using Archipelago.HollowKnight;

namespace APMapMod.Data
{
    public static class DataLoader
    {
        private static Dictionary<string, PinDef> _allPins;
        private static Dictionary<string, PinDef> _allPinsAM;
        private static Dictionary<string, string> _pinScenes;
        private static Dictionary<string, MapZone> _fixedMapZones;
        private static readonly Dictionary<string, PinDef> _usedPins = new();

        public static List<string> usedPoolGroups = new();

        //public static Dictionary<string, PinDef> newPins = new();

        public static List<string> sortedKnownGroups = new()
        {
            "Dreamers",
            "Skills",
            "Charms",
            "Keys",
            "Mask Shards",
            "Vessel Fragments",
            "Charm Notches",
            "Pale Ore",
            "Geo Chests",
            "Rancid Eggs",
            "Relics",
            "Whispering Roots",
            "Boss Essence",
            "Grubs",
            "Mimics",
            "Maps",
            "Stags",
            "Lifeblood Cocoons",
            "Grimmkin Flames",
            "Journal Entries",
            "Geo Rocks",
            "Boss Geo",
            "Soul Totems",
            "Lore Tablets",
            "Shops",
            "Levers",
            "Unknown"
        };

        public static PinDef[] GetPinArray()
        {
            return _allPins.Values.ToArray();
        }

        public static PinDef[] GetPinAMArray()
        {
            return _allPinsAM.Values.ToArray();
        }

        public static PinDef[] GetUsedPinArray()
        {
            return _usedPins.Values.ToArray();
        }

        public static PinDef GetUsedPinDef(string locationName)
        {
            if (_usedPins.TryGetValue(locationName, out PinDef pinDef))
            {
                return pinDef;
            }

            return default;
        }

        public static MapZone GetFixedMapZone()
        {
            if (_fixedMapZones.TryGetValue(StringUtils.CurrentNormalScene(), out MapZone mapZone))
            {
                return mapZone;
            }

            return default;
        }

        // Next five helper functions are based on BadMagic100's Rando4Stats RandoExtensions
        // MIT License

        // Copyright(c) 2022 BadMagic100

        // Permission is hereby granted, free of charge, to any person obtaining a copy
        // of this software and associated documentation files(the "Software"), to deal
        // in the Software without restriction, including without limitation the rights
        // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        // copies of the Software, and to permit persons to whom the Software is
        // furnished to do so, subject to the following conditions:

        // The above copyright notice and this permission notice shall be included in all
        // copies or substantial portions of the Software.
        //public static ItemPlacement RandoPlacement(this AbstractItem item)
        //{
        //    if (item.GetTag(out RandoItemTag tag))
        //    {
        //        return RandomizerMod.RandomizerMod.RS.Context.itemPlacements[tag.id];
        //    }
        //    return default;
        //}

        //public static string RandoItemName(this AbstractItem item)
        //{
        //    return item.RandoPlacement().Item.Name ?? "";
        //}

        //public static string RandoLocationName(this AbstractItem item)
        //{
        //    return item.RandoPlacement().Location.Name ?? "";
        //}

        //public static int RandoItemId(this AbstractItem item)
        //{
        //    if (item.GetTag(out RandoItemTag tag))
        //    {
        //        return tag.id;
        //    }
        //    return default;
        //}

        public static bool IsPersistent(this AbstractItem item)
        {
            return item.HasTag<ItemChanger.Tags.PersistentItemTag>();
        }

        public static bool CanPreviewItem(this AbstractPlacement placement)
        {
            return !placement.HasTag<ItemChanger.Tags.DisableItemPreviewTag>();
        }

        //public static string[] GetPreviewText(string abstractPlacementName)
        //{
        //    if (!ItemChanger.Internal.Ref.Settings.Placements.TryGetValue(abstractPlacementName, out AbstractPlacement placement)) return default;

        //    if (placement.GetTag(out ItemChanger.Tags.MultiPreviewRecordTag multiTag))
        //    {
        //        return multiTag.previewTexts;
        //    }

        //    if (placement.GetTag(out ItemChanger.Tags.PreviewRecordTag tag))
        //    {
        //        return new[] { tag.previewText };
        //    }

        //    return default;
        //}

        public static string GetPlacementGroup(this AbstractPlacement placement)
        {
            return GetPlacementGroup(placement.Name);
        }

        public static string GetPlacementGroup(string name)
        {
            switch (name.Split('-')[0])
            {
                case ItemNames.Lurien:
                case ItemNames.Monomon:
                case ItemNames.Herrah:
                case ItemNames.World_Sense:
                    return "Dreamers";
                    
                case ItemNames.Mothwing_Cloak:
                case ItemNames.Left_Mothwing_Cloak:
                case ItemNames.Right_Mothwing_Cloak:
                case LocationNames.Split_Mothwing_Cloak:
                case ItemNames.Mantis_Claw:
                case ItemNames.Left_Mantis_Claw:
                case ItemNames.Right_Mantis_Claw:
                case ItemNames.Crystal_Heart:
                case ItemNames.Left_Crystal_Heart:
                case ItemNames.Right_Crystal_Heart:
                case LocationNames.Split_Crystal_Heart:
                case ItemNames.Monarch_Wings:
                case ItemNames.Shade_Cloak:
                case ItemNames.Ismas_Tear:
                case ItemNames.Dream_Nail:
                case ItemNames.Awoken_Dream_Nail:
                case ItemNames.Dream_Gate:
                case ItemNames.Vengeful_Spirit:
                case ItemNames.Shade_Soul:
                case ItemNames.Desolate_Dive:
                case ItemNames.Descending_Dark:
                case ItemNames.Howling_Wraiths:
                case ItemNames.Abyss_Shriek:
                case ItemNames.Cyclone_Slash:
                case ItemNames.Dash_Slash:
                case ItemNames.Great_Slash:
                case ItemNames.Leftslash:
                case ItemNames.Upslash:
                case ItemNames.Rightslash:
                    return "Skills";
                    
                case "Baldur_Shell":
                case "Fury_of_the_Fallen":
                case "Lifeblood_Core":
                case "Defender's_Crest":
                case "Flukenest":
                case "Thorns_of_Agony":
                case "Mark_of_Pride":
                case "Sharp_Shadow":
                case "Spore_Shroom":
                case "Soul_Catcher":
                case "Soul_Eater":
                case "Glowing_Womb":
                case "Nailmaster's_Glory":
                case "Joni's_Blessing":
                case "Shape_of_Unn":
                case "Hiveblood":
                case "Dashmaster":
                case "Quick_Slash":
                case "Spell_Twister":
                case "Deep_Focus":
                case "Queen_Fragment":
                case "King_Fragment":
                case "Void_Heart":
                case "Dreamshield":
                case "Weaversong":
                case "Grimmchild":
                case "Carefree_Melody":
                case "Longnail":
                case "Gathering_Swarm":
                case "Steady_Body":
                case "Shaman_Stone":
                case "Quick_Focus":
                case "Lifeblood_Heart":
                case "Stalwart_Shell":
                case "Heavy_Blow":
                case "Sprintmaster":
                case "Grubsong":
                case "Grubberfly's_Elegy":
                case "Dream_Wielder":
                case "Wayward_Compass":
                case "Unbreakable_Heart":
                case "Unbreakable_Greed":
                case "Unbreakable_Strength":
                case "Fragile_Heart":
                case "Fragile_Greed":
                case "Fragile_Strength":
                    return "Charms";
                        
                case ItemNames.Simple_Key:
                case ItemNames.Shopkeepers_Key:
                case ItemNames.Love_Key:
                case ItemNames.Kings_Brand:
                case ItemNames.Godtuner:
                case ItemNames.Collectors_Map:
                case ItemNames.City_Crest:
                case ItemNames.Tram_Pass:
                case ItemNames.Elevator_Pass:
                    return "Keys";
                    
                case ItemNames.Mask_Shard:
                case ItemNames.Full_Mask:
                case ItemNames.Double_Mask_Shard:
                    return "Mask Shards";

                case ItemNames.Vessel_Fragment:
                case ItemNames.Double_Vessel_Fragment:
                case ItemNames.Full_Soul_Vessel:
                    return "Vessel Fragments";
                
                case ItemNames.Charm_Notch:
                    return "Charm Notches";
                    
                case ItemNames.Pale_Ore:
                    return "Pale Ore";
                    
                case "Geo_Chest":
                case ItemNames.Lumafly_Escape:
                    return "Geo Chests";
                    
                case "Rancid_Egg":
                    return "Rancid Eggs";
                    
                case "Wanderer's_Journal": 
                case "Hallownest_Seal":
                case "King's_Idol": 
                case "Arcane_Egg":
                    return "Relics";
                    
                case "Whispering_Root":
                    return "Whispering Roots";
                    
                case "Boss_Essence":
                    return "Boss Essence";
                    
                case "Grub":
                    return "Grubs";
                    
                case "Mimic_Grub":
                    return "Mimics";
                    
                case "Lifeblood_Cocoon":
                case ItemNames.Lifeblood_Cocoon_Small:
                case ItemNames.Lifeblood_Cocoon_Large:
                    return "Lifeblood Cocoons";
                case "Grimmkin_Flame":
                    return "Grimmkin Flames";
                    
                case "Hunter's_Journal":
                case "Journal_Entry":
                    return "Journal Entries";
                    
                case "Geo_Rock":
                    return "Geo Rocks";
                    
                case "Boss_Geo":
                    return "Boss Geo";
                    
                case "Soul_Totem":
                    return "Soul Totems";
                    
                case "Lore_Tablet":
                    return "Lore Tablets";
                    
                case LocationNames.Sly:
                case LocationNames.Sly_Key:
                case LocationNames.Salubra:
                case LocationNames.Iselda:
                case LocationNames.Leg_Eater:
                case LocationNames.Seer:
                case LocationNames.Grubfather:
                    return "Shops";
            }

            if (name.Contains("Map"))
                return "Maps"; 
            
            if (name.Contains("Stag"))
                return "Stags";
            
            return "Unknown";
        }

        public static bool IsRandomized(this AbstractPlacement placement)
        {

            var slotOptions = Archipelago.HollowKnight.Archipelago.Instance.SlotOptions;

            switch (placement.Name)
            {
                case ItemNames.Elevator_Pass:
                    return slotOptions.RandomizeElevatorPass;
                case ItemNames.Right_Mothwing_Cloak:
                case ItemNames.Left_Mothwing_Cloak:
                    return slotOptions.SplitMothwingCloak;
                case ItemNames.Right_Crystal_Heart:
                case ItemNames.Left_Crystal_Heart:
                    return slotOptions.SplitCrystalHeart;
                case ItemNames.Right_Mantis_Claw:
                case ItemNames.Left_Mantis_Claw:
                    return slotOptions.SplitMantisClaw;
            }

            switch (placement.GetPlacementGroup())
            {
                case "Dreamers":
                    return slotOptions.RandomizeDreamers;
                case "Skills":
                    return slotOptions.RandomizeSkills;
                case "Charms":
                    if (placement.Name == LocationNames.King_Fragment)
                        return slotOptions.WhitePalace is not WhitePalaceOption.Exclude;
                    return slotOptions.RandomizeCharms;
                case "Keys":
                    return slotOptions.RandomizeKeys;
                case "Mask Shards":
                    return slotOptions.RandomizeMaskShards;
                case "Vessel Fragments":
                    return slotOptions.RandomizeVesselFragments; 
                case "Charm Notches":
                    return slotOptions.RandomizeCharmNotches;
                case "Pale Ore":
                    return slotOptions.RandomizePaleOre;
                case "Geo Chests":
                    return placement.Name.Contains("Junk_Pit") ? slotOptions.RandomizeJunkPitChests : slotOptions.RandomizeGeoChests; 
                case "Rancid Eggs":
                    return slotOptions.RandomizeRancidEggs;
                case "Relics":
                    return slotOptions.RandomizeRelics;
                case "Whispering Roots":
                    return slotOptions.RandomizeWhisperingRoots;
                case "Boss Essence":
                    return slotOptions.RandomizeBossEssence; 
                case "Grubs":
                    return slotOptions.RandomizeGrubs;
                case "Mimics":
                    return slotOptions.RandomizeMimics; 
                case "Maps":
                    return slotOptions.RandomizeMaps;
                case "Stags":
                    return slotOptions.RandomizeStags;
                case "Lifeblood Cocoons":
                    return slotOptions.RandomizeLifebloodCocoons;
                case "Grimmkin Flames":
                    return slotOptions.RandomizeGrimmkinFlames;
                case "Journal Entries":
                    return slotOptions.RandomizeJournalEntries;
                case "Geo Rocks":
                    return slotOptions.RandomizeGeoRocks;
                case "Boss Geo":
                    return slotOptions.RandomizeBossGeo;
                case "Soul Totems":
                    if (placement.Name.Contains("White_Palace"))
                    {
                        return slotOptions.WhitePalace is WhitePalaceOption.NoPathOfPain or WhitePalaceOption.Include;
                    }
                    if (placement.Name.Contains("Path_Of_Pain"))
                    {
                        return slotOptions.WhitePalace == WhitePalaceOption.Include;
                    }
                    return slotOptions.RandomizeSoulTotems;
                case "Lore Tablets":
                    if (placement.Name.Contains("Palace"))
                    {
                        return slotOptions.WhitePalace == WhitePalaceOption.NoPathOfPain;
                    }
                    if (placement.Name.Contains("Path_Of_Pain"))
                    {
                        return slotOptions.WhitePalace == WhitePalaceOption.Include;
                    }
                    return slotOptions.RandomizeLoreTablets;
                case "Shops":
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool HasObtainedVanillaItem(PinDef pd)
        {
            return (pd.pdBool != null && PlayerData.instance.GetBool(pd.pdBool))
                        || (pd.pdInt != null && PlayerData.instance.GetInt(pd.pdInt) >= pd.pdIntValue)
                        || (pd.locationPoolGroup == "Whispering Roots" && PlayerData.instance.scenesEncounteredDreamPlantC.Contains(pd.sceneName))
                        || (pd.locationPoolGroup == "Grubs" && PlayerData.instance.scenesGrubRescued.Contains(pd.sceneName))
                        || (pd.locationPoolGroup == "Grimmkin Flames" && PlayerData.instance.scenesFlameCollected.Contains(pd.sceneName))
                        || APMapMod.LS.ObtainedVanillaItems.ContainsKey(pd.objectName + pd.sceneName);
        }

        public static void SetUsedPinDefs()
        {
            _usedPins.Clear();
            usedPoolGroups.Clear();
            HashSet<string> unsortedGroups = new();

            // AP INTEGRATION: Add pins from ItemChanger placements properly

            // Randomized placements
            foreach (KeyValuePair<string, AbstractPlacement> placement in ItemChanger.Internal.Ref.Settings.Placements)
            {
                if (placement.Value.Name.Contains("Vanilla") || placement.Value.Name == "Start") continue;

                IEnumerable<AbstractItem> items = placement.Value.Items.Where(x => !x.IsObtained() || x.IsPersistent());

                if (!items.Any()) continue;

                if (!_allPins.TryGetValue(placement.Value.Name, out PinDef pd))
                {
                    pd = new();

                    APMapMod.Instance.Log("Unknown placement. Making a 'best guess' for the placement");
                }

                //APMapMod.Instance.LogDebug($"new item pin def: {placement.Value.Name}");
                pd.name = placement.Value.Name;
                pd.sceneName = Finder.GetLocation(placement.Value.Name).sceneName;

                if (_pinScenes.ContainsKey(pd.sceneName))
                {
                    pd.pinScene = _pinScenes[pd.sceneName];
                }

                pd.randomized = placement.Value.IsRandomized();
                pd.locationPoolGroup = placement.Value.GetPlacementGroup();

                pd.randoItems = items;
                pd.canPreviewItem = placement.Value.CanPreviewItem();

                pd.pinLocationState = PinLocationState.UncheckedReachable;
                pd.placement = placement.Value;

                _usedPins.Add(placement.Value.Name, pd);

                unsortedGroups.Add(pd.locationPoolGroup);

                foreach (var item in items)
                {
                    if (item.IsPersistent())
                        pd.persistant = true;
                }
                
                //foreach (ItemDef i in pd.randoItems)
                //{
                //    unsortedGroups.Add(i.poolGroup);
                //}

                //APMapMod.Instance.Log(locationName);
                //APMapMod.Instance.Log(pinDef.locationPoolGroup);
            }

            //// Vanilla placements
            //foreach (GeneralizedPlacement placement in RandomizerMod.RandomizerMod.RS.Context.Vanilla)
            //{
            //    if (RandomizerMod.RandomizerData.Data.IsLocation(placement.Location.Name)
            //        && !RandomizerMod.RandomizerMod.RS.TrackerData.clearedLocations.Contains(placement.Location.Name)
            //        && placement.Location.Name != "Start"
            //        && placement.Location.Name != "Iselda"
            //        && _allPins.ContainsKey(placement.Location.Name)
            //        && !_usedPins.ContainsKey(placement.Location.Name))
            //    {
            //        PinDef pd = _allPins[placement.Location.Name];

            //        pd.name = placement.Location.Name;
            //        pd.sceneName = RandomizerMod.RandomizerData.Data.GetLocationDef(placement.Location.Name).SceneName;

            //        if (pd.sceneName == "Room_Colosseum_Bronze" || pd.sceneName == "Room_Colosseum_Silver")
            //        {
            //            pd.sceneName = "Room_Colosseum_01";
            //        }

            //        if (_pinScenes.ContainsKey(pd.sceneName))
            //        {
            //            pd.pinScene = _pinScenes[pd.sceneName];
            //        }

            //        pd.mapZone = StringUtils.ToMapZone(RandomizerMod.RandomizerData.Data.GetRoomDef(pd.pinScene ?? pd.sceneName).MapArea);

            //        if (!HasObtainedVanillaItem(pd))
            //        {
            //            pd.randomized = false;

            //            pd.pinLocationState = PinLocationState.NonRandomizedUnchecked;
            //            pd.locationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(placement.Location.Name).FriendlyName();

            //            _usedPins.Add(placement.Location.Name, pd);

            //            unsortedGroups.Add(pd.locationPoolGroup);

            //            //APMapMod.Instance.Log(placement.Location.Name);
            //        }
            //    }
            //}

            // Sort all the PoolGroups that have been used
            foreach (string poolGroup in sortedKnownGroups)
            {
                if (unsortedGroups.Contains(poolGroup))
                {
                    usedPoolGroups.Add(poolGroup);
                    unsortedGroups.Remove(poolGroup);
                }
            }

            usedPoolGroups.AddRange(unsortedGroups);

            // Interop
            if (Dependencies.HasDependency("AdditionalMaps"))
            {
                ApplyAdditionalMapsChanges();
            }
        }

        public static void ApplyAdditionalMapsChanges()
        {
            foreach (PinDef pinDefAM in GetPinAMArray())
            {
                if (_usedPins.TryGetValue(pinDefAM.name, out PinDef pinDef))
                {
                    pinDef.pinScene = pinDefAM.pinScene;
                    pinDef.mapZone = pinDefAM.mapZone;
                    pinDef.offsetX = pinDefAM.offsetX;
                    pinDef.offsetY = pinDefAM.offsetY;
                }
            }
        }

        public static void Load()
        {
            _allPins = JsonUtil.Deserialize<Dictionary<string, PinDef>>("APMapMod.Resources.pins.json");
            _allPinsAM = JsonUtil.Deserialize<Dictionary<string, PinDef>>("APMapMod.Resources.pinsAM.json");
            _pinScenes = JsonUtil.Deserialize<Dictionary<string, string>>("APMapMod.Resources.pinScenes.json");
            _fixedMapZones = JsonUtil.Deserialize<Dictionary<string, MapZone>>("APMapMod.Resources.fixedMapZones.json");
        }

        // For debugging pins
        //public static void LoadNew PinDef()
        //{
        //    newPins.Clear();
        //    newPins = JsonUtil.DeserializeFromExternalFile<Dictionary<string, PinDef>>("newPins.json");
        //}
    }
}