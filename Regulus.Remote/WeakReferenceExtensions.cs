namespace Regulus.Remote.Extensions
{
    static class WeakReferenceExtensions
    {
        public static IGhost GetTargetOrException(this System.WeakReference<IGhost> weak)
        {
            IGhost ghost;
            if(weak.TryGetTarget(out ghost))
                return ghost;
            throw new System.Exception($"ghost of is no longer there.");
        }
    }
}
