using APMapMod.Data;
using APMapMod.Settings;
using System;
using System.Collections;
using System.Linq;
using Archipelago.HollowKnight.IC;
using ItemChanger;
using UnityEngine;
using Archipelago.MultiClient.Net.Enums;
using PBC = APMapMod.Map.PinBorderColor;
using PLS = APMapMod.Data.PinLocationState;

namespace APMapMod.Map
{
    public enum PinBorderColor
    {
        Normal,
        Previewed,
        OutOfLogic,
        Persistent
    }

    public class PinAnimatedSprite : MonoBehaviour
    {
        public PinDef PD { get; private set; } = null;
        
        SpriteRenderer SR => gameObject.GetComponent<SpriteRenderer>();

        private int spriteIndex = 0;

        private readonly Color _inactiveColor = Color.gray;
        
        private Color _origColor;

        public void SetPinData(PinDef pd)
        {
            PD = pd;
            _origColor = SR.color;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        private void OnEnable()
        {
            if (gameObject.activeSelf
                && PD != null
                && PD.randoItems != null
                && PD.randoItems.Count() > 1)
            {
                StartCoroutine("CycleSprite");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        private void OnDisable()
        {
            if (!gameObject.activeSelf)
            {
                StopAllCoroutines();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        private IEnumerator CycleSprite()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                spriteIndex = (spriteIndex + 1) % PD.randoItems.Count();
                SetSprite();
            }
        }

        public void ResetSpriteIndex()
        {
            spriteIndex = 0;
        }

        public void SetSprite()
        {
            if (!gameObject.activeSelf) return;

            // Non-randomized
            if (PD.pinLocationState == PLS.NonRandomizedUnchecked)
            {
                SR.sprite = SpriteManager.GetSpriteFromPool(PD.locationPoolGroup, PBC.Normal);
                
                return;
            }

            if (PD.randoItems == null || spriteIndex + 1 > PD.randoItems.Count()) return;

            // Set pool to display
            string pool = PD.locationPoolGroup;

            if (PD.pinLocationState == PLS.Previewed)
            {
                pool = PD.locationPoolGroup;
            }

            // Set border color of pin
            PBC pinBorderColor = PBC.Normal;

            AbstractItem item = PD.randoItems.ToArray()[spriteIndex];
            if (PD.pinLocationState == PLS.Previewed && PD.canPreviewItem)
            {
                pinBorderColor = PBC.Previewed;

                if (Finder.ItemNames.Contains(item.name))
                {
                    pool = DataLoader.GetPlacementGroup(item.name);
                }
                else if (item.GetTag<ArchipelagoItemTag>().Flags.HasFlag(ItemFlags.Advancement))
                {
                    pool = "Archipelago Progression";
                }
                else if (item.GetTag<ArchipelagoItemTag>().Flags.HasFlag(ItemFlags.NeverExclude))
                {
                    pool = "Archipelago Useful";
                }
                else
                {
                    pool = "Archipelago";
                }
            }

            if (item.IsPersistent() && item.WasEverObtained())
            {
                pinBorderColor = PBC.Persistent;

                if (item.name.StartsWith("Soul_Totem"))
                    pool = "Soul Totems";
                else if (item.name.StartsWith("Lifeblood_Cocoon"))
                    pool = "Lifeblood Cocoons";
            }


            SR.sprite = SpriteManager.GetSpriteFromPool(pool, pinBorderColor);
        }

        public void SetSizeAndColor()
        {
            // Size
            transform.localScale = PD.pinLocationState switch
            {
                PLS.UncheckedReachable
                or PLS.Previewed
                => GetPinScale() * new Vector2(1.45f, 1.45f),

                _ => GetPinScale() * new Vector2(1.015f, 1.015f),
            };

            // Color
            SR.color = PD.pinLocationState switch
            {
                PLS.UncheckedReachable
                or PLS.Previewed
                => _origColor,

                _ => _inactiveColor,
            };
        }

        public void SetSizeAndColorSelected()
        {
            transform.localScale = GetPinScale() * new Vector2(1.8f, 1.8f);
            SR.color = _origColor;
        }

        private float GetPinScale()
        {
            return APMapMod.GS.PinSize switch
            {
                PinSize.Small => 0.31f,
                PinSize.Medium => 0.37f,
                PinSize.Large => 0.42f,
                _ => throw new NotImplementedException()
            };
        }
    }
}
