namespace Regulus.Utility.CommandExtension
{
    public static class CommandRegisterExtension
    {
        private static void _Empty<TR>(TR obj)
        {

        }
        public static void Register<TR>(this Regulus.Utility.Command instance, string command, System.Func<TR> executer)
        {
            instance.Register(command, executer, _Empty);
        }
        public static void Register<T1,TR>(this Regulus.Utility.Command instance, string command, System.Func<T1,TR> executer)
        {
            instance.Register(command, executer, _Empty);
        }
        public static void Register<T1,T2, TR>(this Regulus.Utility.Command instance, string command, System.Func<T1, T2, TR> executer)
        {
            instance.Register(command, executer, _Empty);
        }
        public static void Register<T1, T2,T3, TR>(this Regulus.Utility.Command instance, string command, System.Func<T1, T2, T3, TR> executer)
        {
            instance.Register(command, executer, _Empty);
        }

        public static void Register<T1, T2, T3,T4, TR>(this Regulus.Utility.Command instance, string command, System.Func<T1, T2, T3,T4, TR> executer)
        {
            instance.Register(command, executer, _Empty);
        }

    }
}
