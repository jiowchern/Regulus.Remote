namespace Regulus.Projects.TestProtocol.Common
{
    namespace MultipleNotices
    {
        public interface IMultipleNotices
        {
            Regulus.Remote.Notifier<INumber> Numbers1 { get; }
            Regulus.Remote.Notifier<INumber> Numbers2 { get; }
        }
    }
}
