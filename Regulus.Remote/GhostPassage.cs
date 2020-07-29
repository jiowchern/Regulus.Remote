namespace Regulus.Remote
{
    class SoulPassage
    {
        public readonly long Id;
        
        public readonly PassageCallback Handler;
        public SoulPassage(long id , PassageCallback callback)
        {
            Id = id;        
            Handler = callback;
        }
    }
}