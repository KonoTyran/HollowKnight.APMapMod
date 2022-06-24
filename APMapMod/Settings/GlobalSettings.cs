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

        public int IconColorR = 255, IconColorG = 255, IconColorB = 255;
        
        [JsonIgnore]
        public Color IconColor
        {
            get => new(IconColorR, IconColorG, IconColorB);
            set
            {
                IconColorR = Mathf.RoundToInt(value.r); 
                IconColorG = Mathf.RoundToInt(value.g); 
                IconColorB = Mathf.RoundToInt(value.b); 
                
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