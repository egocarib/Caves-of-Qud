using ConsoleLib.Console;

namespace QudUX.Utilities
{
    public static class InputsUtilities
    {
        public static bool IsMouseEvent(this Keys k, string cmd)
        {
            if(k != Keys.MouseEvent) return false;

            return Keyboard.CurrentMouseEvent.Event == cmd;
        }

        public static bool Is(this Keys current, params Keys[] keys)
        {
            Keys combination = Keys.None;

            foreach(Keys k in keys)
            {
                combination |= k;
            }

            return current == combination;
        }

        public static bool IsControl(this Keys current, Keys key)
        {
            return current.Is(Keys.Control, key);
        }
    }
}