namespace SolarDawn.TempestReader
{
    public static class UserInput
    {
        public static void WaitForKeyStroke(IReadOnlyCollection<ConsoleKey> keys)
        {
            var keyMatch = true;

            do
            {
                while (Console.In.Peek() == -1)
                {
                    // Just waiting around for the user to press a key.
                    // Everything else is running on Async threads.
                }

                var keyPressed = Console.Read();
                var pressed = (ConsoleKey)keyPressed;
                if (keyPressed != -1 && keys.Contains(pressed))
                {
                    keyMatch = false;
                }
            }
            while (keyMatch);
        }
    }
}
