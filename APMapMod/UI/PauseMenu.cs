using APMapMod.CanvasUtil;
using APMapMod.Data;
using APMapMod.Map;
using APMapMod.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace APMapMod.UI
{
    // All the following was modified from the GUI implementation of BenchwarpMod by homothetyhk
    internal class PauseMenu
    {
        public static GameObject Canvas;

        private static readonly Dictionary<string, (UnityAction<string>, Vector2)> _mainButtons = new()
        {
            ["Spoilers"] = (SpoilersClicked, new Vector2(100f, 0f)),
            ["Randomized"] = (RandomizedClicked, new Vector2(200f, 0f)),
            ["Others"] = (OthersClicked, new Vector2(300f, 0f)),
            ["Style"] = (StyleClicked, new Vector2(0f, 30f)),
            ["Size"] = (SizeClicked, new Vector2(100f, 30f)),
            ["Mode"] = (ModeClicked, new Vector2(200f, 30f)),
        };

        private static CanvasPanel _mapControlPanel;

        public static void BuildMenu(GameObject _canvas)
        {
            Canvas = _canvas;

            _mapControlPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 865f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _mapControlPanel.AddText("MapModLabel", "APMapMod", new Vector2(0f, -25f), Vector2.zero, GUIController.Instance.TrajanNormal, 18);

            Rect buttonRect = new(0, 0, GUIController.Instance.Images["ButtonRect"].width, GUIController.Instance.Images["ButtonRect"].height);

            // Main settings
            // Toggle the mod on or off
            _mapControlPanel.AddButton
                (
                    "Enable",
                    GUIController.Instance.Images["ButtonRect"],
                    new Vector2(0f, 0f),
                    Vector2.zero,
                    EnableClicked,
                    buttonRect,
                    GUIController.Instance.TrajanBold,
                    "Enable",
                    fontSize: 10
                );

            if (!APMapMod.LS.ModEnabled)
            {
                UpdateEnable();

                if (GameManager.instance.IsGamePaused())
                {
                    _mapControlPanel.SetActive(true, false);
                }

                return;
            }

            foreach (KeyValuePair<string, (UnityAction<string>, Vector2)> pair in _mainButtons)
            {
                _mapControlPanel.AddButton
                (
                    pair.Key,
                    GUIController.Instance.Images["ButtonRect"],
                    pair.Value.Item2,
                    Vector2.zero,
                    pair.Value.Item1,
                    buttonRect,
                    GUIController.Instance.TrajanBold,
                    pair.Key,
                    fontSize: 10
                );
            }

            // New panel for pool buttons
            CanvasPanel pools = _mapControlPanel.AddPanel
            (
                "PoolsPanel",
                GUIController.Instance.Images["ButtonRectEmpty"],
                new Vector2(400f, 0f),
                Vector2.zero,
                new Rect(0f, 0f, GUIController.Instance.Images["DropdownBG"].width, 270f)
            );
            _mapControlPanel.AddButton
            (
                "PoolsToggle",
                GUIController.Instance.Images["ButtonRect"],
                new Vector2(300f, 30f),
                Vector2.zero,
                s => PoolsPanelClicked(),
                buttonRect,
                GUIController.Instance.TrajanBold,
                "Customize Pins",
                fontSize: 10
            );

            pools.SetActive(false, true);

            int poolGroupCounter = 0;

            // Pool buttons
            foreach (string group in DataLoader.usedPoolGroups)
            {
                float x_offset = (float)(poolGroupCounter) % 9 * 90;
                float y_offset = poolGroupCounter / 9 * 30;

                poolGroupCounter++;

                pools.AddButton
                (
                    group.ToString(),
                    GUIController.Instance.Images["ButtonRectEmpty"],
                    new Vector2(x_offset, y_offset),
                    Vector2.zero,
                    PoolClicked,
                    buttonRect,
                    GUIController.Instance.TrajanBold,
                    group,
                    fontSize: 10
                );
            }

            pools.AddButton
            (
                "Benches",
                GUIController.Instance.Images["ButtonRectEmpty"],
                new Vector2((float)(poolGroupCounter) % 9 * 90, poolGroupCounter / 9 * 30),
                Vector2.zero,
                BenchClicked,
                buttonRect,
                GUIController.Instance.TrajanBold,
                "Benches",
                fontSize: 10
            );

            //foreach (KeyValuePair<string, (UnityAction<string>, Vector2)> pair in _poolPanelAuxButtons)
            //{
            //    pools.AddButton
            //    (
            //        pair.Key,
            //        GUIController.Instance.Images["ButtonRectEmpty"],
            //        pair.Value.Item2,
            //        Vector2.zero,
            //        pair.Value.Item1,
            //        buttonRect,
            //        GUIController.Instance.TrajanBold,
            //        pair.Key,
            //        fontSize: 10
            //    );
            //}

            UpdateGUI();

            _mapControlPanel.SetActive(false, true); // collapse all subpanels

            if (GameManager.instance.IsGamePaused())
            {
                _mapControlPanel.SetActive(true, false);
            }
        }

        // Called every frame
        public static void Update()
        {
            if (_mapControlPanel == null || GameManager.instance == null)
            {
                return;
            }

            if (HeroController.instance == null || !GameManager.instance.IsGameplayScene() || !GameManager.instance.IsGamePaused())
            {
                // Any time we aren't at the Pause Menu / don't want to show the UI otherwise
                if (_mapControlPanel.Active) _mapControlPanel.SetActive(false, true);
                return;
            }

            // On the frame that we enter the Pause Menu
            if (!_mapControlPanel.Active)
            {
                _mapControlPanel.Destroy();
                BuildMenu(Canvas);
            }
        }

        // Update all the buttons (text, color)
        public static void UpdateGUI()
        {
            if (GameManager.instance.gameMap == null) return;

            UpdateEnable();
            UpdateSpoilers();
            UpdateRandomized();
            UpdateOthers();
            UpdateStyle();
            UpdateSize();
            UpdateMode();
            UpdatePoolsPanel();

            foreach (string group in DataLoader.usedPoolGroups)
            {
                UpdatePool(group);
            }

            UpdateBench();
        }

        public static void EnableClicked(string buttonName)
        {
            if (!(MapText.LockToggleEnable && MapText.Canvas.activeSelf))
            {
                APMapMod.LS.ToggleModEnabled();

                if (!GameManager.instance.IsGamePaused() && !HeroController.instance.controlReqlinquished)
                {
                    UIManager.instance.checkpointSprite.Show();
                    UIManager.instance.checkpointSprite.Hide();
                }

                _mapControlPanel.Destroy();
                BuildMenu(Canvas);
                MapText.LockToggleEnable = true;
                MapText.RebuildText();
            }
        }

        private static void UpdateEnable()
        {
            _mapControlPanel.GetButton("Enable").SetTextColor
                (
                    APMapMod.LS.ModEnabled ? Color.green : Color.red
                );
            _mapControlPanel.GetButton("Enable").UpdateText
                (
                    APMapMod.LS.ModEnabled? "Mod\nEnabled"
                    : "Mod\nDisabled"
                );
        }

        public static void SpoilersClicked(string buttonName)
        {
            APMapMod.LS.ToggleSpoilers();
            WorldMap.CustomPins.SetSprites();
            
            UpdateGUI();
            MapText.SetTexts();
        }

        private static void UpdateSpoilers()
        {
            _mapControlPanel.GetButton("Spoilers").SetTextColor
                (
                    APMapMod.LS.SpoilerOn ? Color.green : Color.white
                );
            _mapControlPanel.GetButton("Spoilers").UpdateText
                (
                    APMapMod.LS.SpoilerOn? "Spoilers:\non" : "Spoilers:\noff"
                );
        }

        public static void RandomizedClicked(string buttonName)
        {
            APMapMod.LS.ToggleRandomizedOn();
            WorldMap.CustomPins.ResetPoolSettings();
            WorldMap.CustomPins.SetPinsActive();

            UpdateGUI();
            MapText.SetTexts();
        }

        private static void UpdateRandomized()
        {
            if (WorldMap.CustomPins == null) return;

            string randomizedText = $"Randomized:\n";

            if (APMapMod.LS.randomizedOn)
            {
                _mapControlPanel.GetButton("Randomized").SetTextColor(Color.green);
                randomizedText += "on";
            }
            else
            {
                _mapControlPanel.GetButton("Randomized").SetTextColor(Color.white);
                randomizedText += "off";
            }

            if (WorldMap.CustomPins.IsRandomizedCustom())
            {
                _mapControlPanel.GetButton("Randomized").SetTextColor(Color.yellow);
                randomizedText += $" (custom)";
            }

            _mapControlPanel.GetButton("Randomized").UpdateText(randomizedText);
        }

        public static void OthersClicked(string buttonName)
        {
            APMapMod.LS.ToggleOthersOn();
            WorldMap.CustomPins.ResetPoolSettings();
            WorldMap.CustomPins.SetPinsActive();

            UpdateGUI();
            MapText.SetTexts();
        }

        private static void UpdateOthers()
        {
            if (WorldMap.CustomPins == null) return;

            string othersText = $"Others:\n";

            if (APMapMod.LS.othersOn)
            {
                _mapControlPanel.GetButton("Others").SetTextColor(Color.green);
                othersText += "on";
            }
            else
            {
                _mapControlPanel.GetButton("Others").SetTextColor(Color.white);
                othersText += "off";
            }

            if (WorldMap.CustomPins.IsOthersCustom())
            {
                _mapControlPanel.GetButton("Others").SetTextColor(Color.yellow);
                othersText += $" (custom)";
            }

            _mapControlPanel.GetButton("Others").UpdateText(othersText);
        }

        public static void StyleClicked(string buttonName)
        {
            APMapMod.GS.TogglePinStyle();
            WorldMap.CustomPins.SetSprites();

            UpdateGUI();
            MapText.SetTexts();
        }

        private static void UpdateStyle()
        {
            string styleText = $"Pin Style:\n";

            switch (APMapMod.GS.pinStyle)
            {
                case PinStyle.Normal:
                    styleText += "normal";
                    break;

                case PinStyle.Q_Marks_1:
                    styleText += $"q marks 1";
                    break;

                case PinStyle.Q_Marks_2:
                    styleText += $"q marks 2";
                    break;

                case PinStyle.Q_Marks_3:
                    styleText += $"q marks 3";
                    break;
            }

            _mapControlPanel.GetButton("Style").UpdateText(styleText);
        }

        public static void SizeClicked(string buttonName)
        {
            APMapMod.GS.TogglePinSize();

            if (WorldMap.CustomPins != null)
            {
                WorldMap.CustomPins.ResizePins("None selected");
            }

            UpdateGUI();
            MapText.SetTexts();
        }

        private static void UpdateSize()
        {
            string sizeText = $"Pin Size:\n";

            switch (APMapMod.GS.pinSize)
            {
                case PinSize.Small:
                    sizeText += "small";
                    break;

                case PinSize.Medium:
                    sizeText += "medium";
                    break;

                case PinSize.Large:
                    sizeText += "large";
                    break;
            }

            _mapControlPanel.GetButton("Size").UpdateText(sizeText);
        }

        public static void ModeClicked(string buttonName)
        {
            APMapMod.LS.ToggleFullMap();

            UpdateGUI();
            MapText.SetTexts();
        }

        private static void UpdateMode()
        {
            string modeText = $"Mode:\n";

            switch (APMapMod.LS.mapMode)
            {
                case MapMode.FullMap:
                    _mapControlPanel.GetButton("Mode").SetTextColor(Color.green);
                    modeText += "Full Map";
                    break;

                case MapMode.AllPins:
                    _mapControlPanel.GetButton("Mode").SetTextColor(Color.white);
                    modeText += "All Pins";
                    break;

                case MapMode.PinsOverMap:
                    _mapControlPanel.GetButton("Mode").SetTextColor(Color.white);
                    modeText += "Pins Over Map";
                    break;
            }

            _mapControlPanel.GetButton("Mode").UpdateText(modeText);
        }

        public static void PoolsPanelClicked()
        {
            _mapControlPanel.TogglePanel("PoolsPanel");

            UpdateGUI();
        }

        private static void UpdatePoolsPanel()
        {
            _mapControlPanel.GetButton("PoolsToggle").SetTextColor
                (
                _mapControlPanel.GetPanel("PoolsPanel").Active? Color.yellow : Color.white
                );;
        }

        public static void PoolClicked(string buttonName)
        {
            APMapMod.LS.TogglePoolGroupSetting(buttonName);

            WorldMap.CustomPins.GetRandomizedOthersGroups();

            UpdateGUI();
            MapText.SetTexts();
        }

        private static void UpdatePool(string poolGroup)
        {
            if (WorldMap.CustomPins == null) return;

            switch (APMapMod.LS.GetPoolGroupSetting(poolGroup))
            {
                case PoolGroupState.Off:
                    _mapControlPanel.GetPanel("PoolsPanel").GetButton(poolGroup).SetTextColor(Color.white);
                    break;
                case PoolGroupState.On:
                    _mapControlPanel.GetPanel("PoolsPanel").GetButton(poolGroup).SetTextColor(Color.green);
                    break;
                case PoolGroupState.Mixed:
                    _mapControlPanel.GetPanel("PoolsPanel").GetButton(poolGroup).SetTextColor(Color.yellow);
                    break;
            }
        }

        public static void BenchClicked(string buttonName)
        {
            if (!PlayerData.instance.hasPinBench) return;

            APMapMod.LS.ToggleBench();

            UpdateGUI();
        }

        public static void UpdateBench()
        {
            if (!PlayerData.instance.hasPinBench)
            {
                _mapControlPanel.GetPanel("PoolsPanel").GetButton("Benches").SetTextColor(Color.red);
                return;
            }

            _mapControlPanel.GetPanel("PoolsPanel").GetButton("Benches").SetTextColor
                (
                    APMapMod.LS.showBenchPins ? Color.green : Color.white
                );
        }
    }
}