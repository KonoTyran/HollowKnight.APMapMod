using APMapMod.CanvasUtil;
using APMapMod.Map;
using APMapMod.Settings;
using UnityEngine;

namespace APMapMod.UI
{
    internal class MapText
    {
        public static GameObject Canvas;

        public static bool LockToggleEnable;

        private static CanvasPanel _mapDisplayPanel;
        private static CanvasPanel _refreshDisplayPanel;

        public static void Show()
        {
            if (Canvas == null) return;

            Canvas.SetActive(true);
            LockToggleEnable = false;
            RebuildText();
        }

        public static void Hide()
        {
            if (Canvas == null) return;

            Canvas.SetActive(false);
            LockToggleEnable = false;
        }

        public static void BuildText(GameObject _canvas)
        {
            Canvas = _canvas;
            _mapDisplayPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(0f, 1030f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _mapDisplayPanel.AddText("Player Icons", "", new Vector2(-540f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Randomized", "", new Vector2(-270f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Others", "", new Vector2(0f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Style", "", new Vector2(270f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Size", "", new Vector2(540f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);

            _refreshDisplayPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(0f, 1030f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _refreshDisplayPanel.AddText("Refresh", "", new Vector2(0f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);

            _mapDisplayPanel.SetActive(false, false);
            _refreshDisplayPanel.SetActive(false, false);

            SetTexts();
        }

        public static void RebuildText()
        {
            _mapDisplayPanel.Destroy();
            _refreshDisplayPanel.Destroy();

            BuildText(Canvas);
        }

        public static void SetTexts()
        {
            if (GameManager.instance.gameMap == null
                || WorldMap.goCustomPins == null
                || WorldMap.CustomPins == null) return;

            _mapDisplayPanel.SetActive(!LockToggleEnable && APMapMod.LS.ModEnabled, false);
            _refreshDisplayPanel.SetActive(LockToggleEnable, false);

            SetPlayerIcons();
            SetStyle();
            SetRandomized();
            SetOthers();
            SetSize();
            SetRefresh();
        }

        private static void SetPlayerIcons()
        {
            string playerIconsText = $"Player Icons (ctrl-1): ";
        
            if (APMapMod.LS.PlayerIconsOn)
            {
                _mapDisplayPanel.GetText("Player Icons").SetTextColor(Color.green);
                playerIconsText += "on";
            }
            else
            {
                _mapDisplayPanel.GetText("Player Icons").SetTextColor(Color.white);
                playerIconsText += "off";
            }
        
            _mapDisplayPanel.GetText("Player Icons").UpdateText(playerIconsText);
        }

        private static void SetRandomized()
        {
            string randomizedText = $"Randomized (ctrl-2): ";

            if (APMapMod.LS.randomizedOn)
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.green);
                randomizedText += "on";
            }
            else
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.white);
                randomizedText += "off";
            }

            if (WorldMap.CustomPins.IsRandomizedCustom())
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.yellow);
                randomizedText += $" (custom)";
            }

            _mapDisplayPanel.GetText("Randomized").UpdateText(randomizedText);
        }

        private static void SetOthers()
        {
            string othersText = $"Others (ctrl-3): ";

            if (APMapMod.LS.othersOn)
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.green);
                othersText += "on";
            }
            else
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.white);
                othersText += "off";
            }

            if (WorldMap.CustomPins.IsOthersCustom())
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.yellow);
                othersText += $" (custom)";
            }

            _mapDisplayPanel.GetText("Others").UpdateText(othersText);
        }

        private static void SetStyle()
        {
            string styleText = $"Style (ctrl-4): ";

            switch (APMapMod.GS.PinStyle)
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

            _mapDisplayPanel.GetText("Style").UpdateText(styleText);
        }

        private static void SetSize()
        {
            string sizeText = $"Size (ctrl-5): ";

            switch (APMapMod.GS.PinSize)
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

            _mapDisplayPanel.GetText("Size").UpdateText(sizeText);
        }

        private static void SetRefresh()
        {
            if (APMapMod.LS.ModEnabled)
            {
                _refreshDisplayPanel.GetText("Refresh").UpdateText("APMapMod enabled. Close map to refresh");
            }
            else
            {
                _refreshDisplayPanel.GetText("Refresh").UpdateText("APMapMod disabled. Close map to refresh");
            }
        }
    }
}