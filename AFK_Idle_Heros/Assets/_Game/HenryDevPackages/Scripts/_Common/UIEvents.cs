using System;

namespace HenryDev.Events
{
    public static class UIEvents
    {
        public static Action<bool> SET_UI_BLOCK;
        public static Action TRANSITION_IN;
        public static Action TRANSITION_OUT;
    }

    public static class UIConstants
    {
        public static float TRANSITION_DURATION = 1f;
    }
}
