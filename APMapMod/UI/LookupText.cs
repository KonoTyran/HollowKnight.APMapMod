﻿using APMapMod.CanvasUtil;
using APMapMod.Data;
using APMapMod.Map;
using APMapMod.Settings;
using RandomizerMod;
using System.Linq;
using UnityEngine;

namespace APMapMod.UI
{
    internal class LookupText
    {
        public static GameObject Canvas;

        private static CanvasPanel _infoPanel;
        private static CanvasPanel _instructionPanel;

        private static string selectedLocation = "None selected";
        private static bool worldMapOpen = false;

        public static bool LookupActive()
        {
            return APMapMod.LS.ModEnabled
                && (APMapMod.LS.mapMode == MapMode.FullMap
                    || APMapMod.LS.mapMode == MapMode.AllPins
                    || APMapMod.LS.mapMode == MapMode.PinsOverMap);
        }

        public static void ShowWorldMap()
        {
            if (Canvas == null || GameManager.instance.gameMap == null || _infoPanel == null) return;

            if (!LookupActive() || TransitionText.TransitionModeActive())
            {
                Hide();
                return;
            }

            GameMap gameMap = GameManager.instance.gameMap.GetComponent<GameMap>();

            if (gameMap != null)
            {
                gameMap.panMinX = -29f;
                gameMap.panMaxX = 26f;
                gameMap.panMinY = -25f;
                gameMap.panMaxY = 20f;
            }

            worldMapOpen = true;

            Canvas.SetActive(true);

            _infoPanel.SetActive(APMapMod.LS.lookupOn, APMapMod.LS.lookupOn);
        }

        public static void Hide()
        {
            if (Canvas == null || _infoPanel == null) return;

            worldMapOpen = false;

            Canvas.SetActive(false);
        }

        public static void Initialize()
        {
            selectedLocation = "None selected";
        }

        public static void BuildText(GameObject _canvas)
        {
            Canvas = _canvas;

            _instructionPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 20f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _instructionPanel.AddText("Control", "", new Vector2(-37f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperRight);

            _instructionPanel.SetActive(true, true);

            _infoPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["LookupBG"], new Vector2(1200f, 200f), new Vector2(GUIController.Instance.Images["LookupBG"].width, GUIController.Instance.Images["LookupBG"].height), new Rect(0f, 0f, GUIController.Instance.Images["LookupBG"].width, GUIController.Instance.Images["LookupBG"].height));
            _infoPanel.AddText("Info", Localization.Localize("None selected"), new Vector2(5f, 30f), new Vector2(GUIController.Instance.Images["LookupBG"].width - 20f, GUIController.Instance.Images["LookupBG"].height), GUIController.Instance.Perpetua, 19);

            _infoPanel.SetActive(false, false);

            SetTexts();
        }

        public static void Update()
        {
            if (Canvas == null
                || HeroController.instance == null
                || WorldMap.goCustomPins == null
                || WorldMap.CustomPins == null
                || !LookupActive()
                || TransitionText.TransitionModeActive()) return;

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.L))
            {
                APMapMod.LS.ToggleLookup();
                SetTexts();

                if (APMapMod.LS.lookupOn)
                {
                    _infoPanel.SetActive(true, true);

                    UpdateSelectedPin();
                }
                else
                {
                    _infoPanel.SetActive(false, false);

                    WorldMap.CustomPins.ResizePins("None selected");
                }
            }
        }

        // Called every 0.1 seconds
        public static void UpdateSelectedPinCoroutine()
        {
            if (Canvas == null
                || HeroController.instance == null
                || WorldMap.goCustomPins == null
                || WorldMap.CustomPins == null
                || !_infoPanel.Active
                || GameManager.instance.IsGamePaused()
                || !LookupActive()
                || !APMapMod.LS.lookupOn)
            {
                return;
            }

            if (WorldMap.CustomPins.GetPinClosestToMiddle(selectedLocation, out selectedLocation))
            {
                UpdateSelectedPin();
            }
        }

        public static void UpdateSelectedPin()
        {
            if (!worldMapOpen
                || WorldMap.goCustomPins == null
                || WorldMap.CustomPins == null) return;

            WorldMap.CustomPins.ResizePins(selectedLocation);
            SetTexts();
        }

        public static void SetTexts()
        {
            SetControlText();
            SetInstructionsText();
        }

        public static void SetControlText()
        {
            string controlText = $"{Localization.Localize("Toggle lookup")} (Ctrl-L): ";

            if (APMapMod.LS.lookupOn)
            {
                _instructionPanel.GetText("Control").SetTextColor(Color.green);
                controlText += Localization.Localize("On");
            }
            else
            {
                _instructionPanel.GetText("Control").SetTextColor(Color.white);
                controlText += Localization.Localize("Off");
            }

            _instructionPanel.GetText("Control").UpdateText(Localization.Localize(controlText));
        }

        public static void SetInstructionsText()
        {
            string instructionsText = $"{StringUtils.ToCleanName(selectedLocation)}";

            PinDef pd = DataLoader.GetUsedPinDef(selectedLocation);

            if (pd != null)
            {
                instructionsText += $"\n\n{Localization.Localize("Room")}: {pd.sceneName}";

                instructionsText += $"\n\n{Localization.Localize("Status")}:";

                instructionsText += pd.pinLocationState switch
                {
                    PinLocationState.UncheckedUnreachable => $" {Localization.Localize("Randomized, unchecked, unreachable")}",
                    PinLocationState.UncheckedReachable => $" {Localization.Localize("Randomized, unchecked, reachable")}",
                    PinLocationState.NonRandomizedUnchecked => $" {Localization.Localize("Not randomized, either unchecked or persistent")}",
                    PinLocationState.OutOfLogicReachable => $" {Localization.Localize("Randomized, unchecked, reachable through sequence break")}",
                    PinLocationState.Previewed => $" {Localization.Localize("Randomized, previewed")}",
                    PinLocationState.Cleared => $" {Localization.Localize("Cleared")}",
                    PinLocationState.ClearedPersistent => $" {Localization.Localize("Randomized, cleared, persistent")}",
                    _ => ""
                };

                if (DataLoader.IsInLogicLookup(selectedLocation))
                {
                    instructionsText += $"\n\n{Localization.Localize("Logic")}: {DataLoader.GetRawLogic(selectedLocation)}";
                }

                if (RandomizerMod.RandomizerMod.RS.TrackerData.previewedLocations.Contains(pd.name))
                {
                    instructionsText += $"\n\n{Localization.Localize("Previewed item(s)")}:";

                    string[] previewText = DataLoader.GetPreviewText(pd.name);

                    if (previewText == null) return;

                    foreach (string text in previewText)
                    {
                        instructionsText += $" {StringUtils.ToCleanPreviewText(text)},";
                    }

                    instructionsText = instructionsText.Substring(0, instructionsText.Length - 1);
                }
                
                if (APMapMod.LS.SpoilerOn
                    && pd.randoItems != null
                    && pd.randoItems.Any()
                    && (!RandomizerMod.RandomizerMod.RS.TrackerData.previewedLocations.Contains(pd.name)
                        || (RandomizerMod.RandomizerMod.RS.TrackerData.previewedLocations.Contains(pd.name)
                            && !pd.canPreviewItem)))
                {
                    instructionsText += $"\n\n{Localization.Localize("Spoiler item(s)")}:";

                    foreach (ItemDef item in pd.randoItems)
                    {
                        instructionsText += $" {StringUtils.ToCleanName(item.itemName)},";
                    }

                    instructionsText = instructionsText.Substring(0, instructionsText.Length - 1);
                }
            }

            _infoPanel.GetText("Info").UpdateText(Localization.Localize(instructionsText));
        }
    }
}
