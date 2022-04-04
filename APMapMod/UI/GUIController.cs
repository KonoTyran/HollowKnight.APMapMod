using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace APMapMod.UI
{
    // All the following was modified from the GUI implementation of BenchwarpMod by homothetyhk
    public class GUIController : MonoBehaviour
    {
        public Dictionary<string, Texture2D> Images = new();

        public static GUIController Instance;

        private GameObject _pauseCanvas;
        private GameObject _mapCanvas;
        public Font TrajanBold { get; private set; }
        public Font TrajanNormal { get; private set; }
        public Font Perpetua { get; private set; }
        private Font Arial { get; set; }

        public static void Setup()
        {
            GameObject GUIObj = new("APMapMod GUI");
            Instance = GUIObj.AddComponent<GUIController>();
            DontDestroyOnLoad(GUIObj);
            Instance.LoadResources();
        }

        public static void Unload()
        {
            if (Instance != null)
            {
                Instance.StopAllCoroutines();

                Destroy(Instance._pauseCanvas);
                Destroy(Instance._mapCanvas);
                Destroy(Instance.gameObject);
            }
        }

        public void BuildMenus()
        {
            _pauseCanvas = new GameObject();
            _pauseCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler pauseScaler = _pauseCanvas.AddComponent<CanvasScaler>();
            pauseScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            pauseScaler.referenceResolution = new Vector2(1920f, 1080f);
            _pauseCanvas.AddComponent<GraphicRaycaster>();

            PauseMenu.BuildMenu(_pauseCanvas);

            DontDestroyOnLoad(_pauseCanvas);

            _mapCanvas = new GameObject();
            _mapCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler mapScaler = _mapCanvas.AddComponent<CanvasScaler>();
            mapScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            mapScaler.referenceResolution = new Vector2(1920f, 1080f);

            MapText.BuildText(_mapCanvas);

            DontDestroyOnLoad(_mapCanvas);

            _mapCanvas.SetActive(false);
        }

        public void Update()
        {
            try
            {
                PauseMenu.Update();
            }
            catch (Exception e)
            {
                APMapMod.Instance.LogError(e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        IEnumerator UpdateSelectedScene()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        IEnumerator UpdateSelectedPin()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        private void LoadResources()
        {
            TrajanBold = Modding.CanvasUtil.TrajanBold;
            TrajanNormal = Modding.CanvasUtil.TrajanNormal;
            Perpetua = Modding.CanvasUtil.GetFont("Perpetua");

            try
            {
                Arial = Font.CreateDynamicFontFromOSFont
                (
                    Font.GetOSInstalledFontNames().First(x => x.ToLower().Contains("arial")),
                    13
                );
            }
            catch
            {
                APMapMod.Instance.LogWarn("Unable to find Arial! Using Perpetua.");
                Arial = Modding.CanvasUtil.GetFont("Perpetua");
            }

            if (TrajanBold == null || TrajanNormal == null || Arial == null)
            {
                APMapMod.Instance.LogError("Could not find game fonts");
            }

            Assembly asm = Assembly.GetExecutingAssembly();

            foreach (string res in asm.GetManifestResourceNames())
            {
                if (!res.StartsWith("APMapMod.Resources.GUI.")) continue;

                try
                {
                    using Stream imageStream = asm.GetManifestResourceStream(res);
                    byte[] buffer = new byte[imageStream.Length];
                    imageStream.Read(buffer, 0, buffer.Length);

                    Texture2D tex = new(1, 1);
                    tex.LoadImage(buffer.ToArray());

                    string[] split = res.Split('.');
                    string internalName = split[split.Length - 2];

                    Images.Add(internalName, tex);
                }
                catch (Exception e)
                {
                    APMapMod.Instance.LogError("Failed to load image: " + res + "\n" + e);
                }
            }
        }
    }
}