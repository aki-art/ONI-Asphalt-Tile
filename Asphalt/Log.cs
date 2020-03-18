using System;

namespace Asphalt
{
    class Log
    {
        private static readonly string prefix = $"[ASPHALT]: ";

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
        public static void Debuglog(object arg)
        {
#if DEBUG
            try
            {
               Debug.Log(prefix + arg.ToString());
            }
            catch (Exception e)
            {
                Warn(e);
            }
#endif
        }

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
