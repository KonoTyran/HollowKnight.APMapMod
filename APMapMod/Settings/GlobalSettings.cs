using APMapMod.Map;
using APMapMod.Util;
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

        public (int r, int g, int b) IconColor = Color.white.ToTuple();

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