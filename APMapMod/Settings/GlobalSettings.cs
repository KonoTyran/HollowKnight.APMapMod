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
        public PinStyle pinStyle = PinStyle.Normal;

        public PinSize pinSize = PinSize.Medium;

        public bool persistentOn = false;

        public void TogglePinStyle()
        {
            switch (pinStyle)
            {
                case PinStyle.Normal:
                case PinStyle.Q_Marks_1:
                case PinStyle.Q_Marks_2:
                    pinStyle += 1;
                    break;
                default:
                    pinStyle = PinStyle.Normal;
                    break;
            }
        }

        public void TogglePinSize()
        {
            switch (pinSize)
            {
                case PinSize.Small:
                case PinSize.Medium:
                    pinSize += 1;
                    break;
                default:
                    pinSize = PinSize.Small;
                    break;
            }
        }

        public void TogglePersistentOn()
        {
            persistentOn = !persistentOn;
        }
    }
}