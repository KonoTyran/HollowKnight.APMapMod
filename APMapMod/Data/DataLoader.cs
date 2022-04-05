using GlobalEnums;
using ItemChanger;
using System.Collections.Generic;
using System.Linq;

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

        //public static bool IsPersistent(this AbstractItem item)
        //{
        //    return item.HasTag<ItemChanger.Tags.PersistentItemTag>();
        //}

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

                IEnumerable<ItemDef> items = placement.Value.Items
                    .Where(x => !x.IsObtained())
                    .Select(x => new ItemDef(x));

                if (!items.Any()) continue;

                if (!_allPins.TryGetValue(placement.Value.Name, out PinDef pd))
                {
                    pd = new();

                    APMapMod.Instance.Log("Unknown placement. Making a 'best guess' for the placement");
                }

                pd.name = placement.Value.Name;
                pd.sceneName = Finder.GetLocation(placement.Value.Name).sceneName;

                if (_pinScenes.ContainsKey(pd.sceneName))
                {
                    pd.pinScene = _pinScenes[pd.sceneName];
                }

                pd.randomized = true;
                pd.randoItems = items;
                pd.canPreviewItem = placement.Value.CanPreviewItem();

                pd.pinLocationState = PinLocationState.UncheckedReachable;
                pd.locationPoolGroup = "Unknown";

                _usedPins.Add(placement.Value.Name, pd);

                unsortedGroups.Add(pd.locationPoolGroup);

                foreach (ItemDef i in pd.randoItems)
                {
                    unsortedGroups.Add(i.poolGroup);
                }

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