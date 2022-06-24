using Newtonsoft.Json;
using UnityEngine;

namespace APMapMod.Settings
{
    public enum PinSize
    {
        Small,
        Medium,
        Large
    }

    public enum PinStyle
    {
        Normal,
        Q_Marks_1,
        Q_Marks_2,
        Q_Marks_3
    }

    public class GlobalSettings
    {
        public PinStyle PinStyle = PinStyle.Normal;

        public PinSize PinSize = PinSize.Medium;

        public bool PersistentOn = false;

        public int IconColorR = -1, IconColorG = -1, IconColorB = -1;
        
        [JsonIgnore]
        public Color IconColor
        {
            get => new(IconColorR / 255f, IconColorG / 255f, IconColorB / 255f);
            set
            {
                IconColorR = Mathf.RoundToInt(value.r * 255);
                IconColorG = Mathf.RoundToInt(value.g * 255); 
                IconColorB = Mathf.RoundToInt(value.b * 255); 
                
            }
        }

        public void TogglePinStyle()
        {
            switch (PinStyle)
            {
                case PinStyle.Normal:
                case PinStyle.Q_Marks_1:
                case PinStyle.Q_Marks_2:
                    PinStyle += 1;
                    break;
                default:
                    PinStyle = PinStyle.Normal;
                    break;
            }
        }

        public void TogglePinSize()
        {
            switch (PinSize)
            {
                case PinSize.Small:
                case PinSize.Medium:
                    PinSize += 1;
                    break;
                default:
                    PinSize = PinSize.Small;
                    break;
            }
        }

        public void TogglePersistentOn()
        {
            PersistentOn = !PersistentOn;
        }
    }
}