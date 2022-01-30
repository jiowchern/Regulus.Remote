namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    namespace MultipleNotices
    {
        public interface IMultipleNotices
        {
            Regulus.Remote.Value<int> GetNumber1Count();
            Regulus.Remote.Value<int> GetNumber2Count();
            Regulus.Remote.Notifier<INumber> Numbers1{ get; }
            Regulus.Remote.Notifier<INumber> Numbers2 { get; }
        }
    }
}
