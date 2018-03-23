using System;
using System.Reflection;

namespace MisturTee.TestMeFool
{
    public class TestingUtils
    {
        public static void ResetRoutesRepository()
        {
            //CallPrivateVoidMethod(typeof(RoutesRepository));
        }

        public static void ResetSentinel()
        {
            //CallPrivateVoidMethod(typeof(Sentinel));
        }

        // ReSharper disable once UnusedMember.Local
        private static void CallPrivateVoidMethod(Type type)
        {
            var methodInfo = type
                .GetMethod("Reset", BindingFlags.Static | BindingFlags.NonPublic);
            var unused = (string)methodInfo.Invoke(new object(), new object[] { });
        }
    }
}
