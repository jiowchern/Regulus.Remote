namespace Regulus.Remote
{
    internal class IdLandlord : Landlord<long>
    {
        public IdLandlord() : base(new LongProvider())
        {
        }
    }
}