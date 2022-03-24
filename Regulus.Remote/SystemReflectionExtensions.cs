namespace Regulus.Remote
{
    static class SystemReflectionExtensions
    {
        public static string GetPathName(this System.Reflection.MemberInfo info)
        {
            return $"{info.DeclaringType.FullName.Replace('.', '_').Replace('+','_')}_{info.Name}";
        }
    }
}
