using System.Threading;
using UnityEngine;

namespace NRO.Util
{
    public static class MainThread
    {
        private static int _mainThreadId;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static bool IsMain => Thread.CurrentThread.ManagedThreadId == _mainThreadId;
    }
}
