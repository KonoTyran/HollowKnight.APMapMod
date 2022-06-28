using APMapMod.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using APMapMod.Map;
using Color = UnityEngine.Color;

namespace APMapMod.Settings
{
    public enum MapMode
    {
        FullMap,
        AllPins,
        PinsOverMap
    }

    public enum PoolGroupState
    {
        Off,
        On,
        Mixed
    }

    public enum IconVisibility
    {
        Own,
        Others,
        Both,
        None
    }

    public class SettingPair
    {
        public SettingPair(string poolGroup, PoolGroupState state)
        {
            this.poolGroup = poolGroup;
            this.state = state;
        }

        public string poolGroup;
        public PoolGroupState state;
    }

    [Serializable]
    public class LocalSettings
    {
        public Dictionary<string, bool> ObtainedVanillaItems = new();

        public bool showBenchPins = false;

        public bool ModEnabled = false;

        public MapMode mapMode = MapMode.FullMap;

        public bool randomizedOn = true;

        public bool othersOn = false;

        public bool NewSettings = true;

        public IconVisibility IconVisibility = IconVisibility.Both;

        public List<SettingPair> PoolGroupSettings = new();

        public void ToggleModEnabled()
        {
            ModEnabled = !ModEnabled;
        }

        public void ToggleFullMap()
        {
            switch (mapMode)
            {
                case MapMode.FullMap:
                    mapMode = MapMode.AllPins;
                    break;

                case MapMode.AllPins:
                    mapMode = MapMode.PinsOverMap;
                    break;

                case MapMode.PinsOverMap:
                    mapMode = MapMode.FullMap;
                    break;
            }
        }

        public void ToggleBench()
        {
            showBenchPins = !showBenchPins;
        }

        public void ToggleRandomizedOn()
        {
            randomizedOn = !randomizedOn;
        }

        public void ToggleOthersOn()
        {
            othersOn = !othersOn;
        }

        public void InitializePoolGroupSettings()
        {
            PoolGroupSettings = DataLoader.usedPoolGroups.Select(p => new SettingPair(p, PoolGroupState.On)).ToList();
        }

        public PoolGroupState GetPoolGroupSetting(string poolGroup)
        {
            var item = PoolGroupSettings.FirstOrDefault(s => s.poolGroup == poolGroup);

            if (item != null)
            {
                return item.state;
            }

            //APMapMod.Instance.LogWarn($"Tried to get a PoolGroup setting, but the key {poolGroup} was missing");

            return PoolGroupState.Off;
        }

        public void SetPoolGroupSetting(string poolGroup, PoolGroupState state)
        {
            var item = PoolGroupSettings.FirstOrDefault(s => s.poolGroup == poolGroup);

            if (item != null)
            {
                item.state = state;
            }
            //else
            //{
            //    APMapMod.Instance.LogWarn($"Tried to set a PoolGroup setting, but the key {poolGroup} was missing");
            //}
        }

        public void TogglePoolGroupSetting(string poolGroup)
        {
            var item = PoolGroupSettings.FirstOrDefault(s => s.poolGroup == poolGroup);

            if (item != null)
            {
                item.state = item.state switch
                {
                    PoolGroupState.Off => PoolGroupState.On,
                    PoolGroupState.On => PoolGroupState.Off,
                    PoolGroupState.Mixed => PoolGroupState.On,
                    _ => throw new NotImplementedException()
                };
            }
            //else
            //{
            //    APMapMod.Instance.LogWarn($"Tried to set a PoolGroup setting, but the key {poolGroup} was missing");
            //}
        }
        
        public void ToggleIconVisibility()
        {
            switch (IconVisibility)
            {
                case IconVisibility.Both:
                    IconVisibility = IconVisibility.None;
                    //APMapMod.Instance.CoOpMap.
                    break;

                case IconVisibility.None:
                    IconVisibility = IconVisibility.Own;
                    break;

                case IconVisibility.Own:
                    IconVisibility = IconVisibility.Others;
                    break;

                case IconVisibility.Others:
                    IconVisibility = IconVisibility.Both;
                    break;
            }
        }
    }
}