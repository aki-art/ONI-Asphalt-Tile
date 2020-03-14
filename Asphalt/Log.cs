using System;

namespace Asphalt
{
    class Log
    {
        private static readonly string prefix = $"[ASPHALT]: ";

        // Displays a message in INFO format. Attempts to turn the given argument into String
        public static void Info(object arg)
        {
            try
            {
                Debug.Log(prefix + arg.ToString());
            }
            catch (Exception e)
            {
                Warn(e);
            }
        }

        // Displays a message in WARNING format. Attempts to turn the given argument into String
        public static void Warning(object arg)
        {
            try
            {
                Debug.LogWarning(prefix + arg.ToString());
            }
            catch (Exception e)
            {
                Warn(e);
            }
        }

        // Displays a message in ERROR format. Attempts to turn the given argument into String
        public static void Error(object arg)
        {
            try
            {
                Debug.LogError(prefix + arg.ToString());
            }
            catch (Exception e)
            {
                Warn(e);
            }
        }

        private static void Warn(Exception e)
        {
            Debug.LogWarning($"{prefix} Could not write to log: {e}");
        }
    }
}
