namespace Regulus.Remote
{
    public static class MemoryPoolProvider
    {
        public static Regulus.Memorys.Pool Create()
        {
            return new Regulus.Memorys.Pool(
                    new Regulus.Memorys.ChunkSetting[] {
                        //new Regulus.Memorys.ChunkSetting(1, 1024),
                        //new Regulus.Memorys.ChunkSetting(2, 1024),
                        new Regulus.Memorys.ChunkSetting(4, 1024),
                        //new Regulus.Memorys.ChunkSetting(8, 1024),
                        //new Regulus.Memorys.ChunkSetting(16, 1024),
                        new Regulus.Memorys.ChunkSetting(32, 1024),
                        //new Regulus.Memorys.ChunkSetting(64, 1024),
                        //new Regulus.Memorys.ChunkSetting(128, 1024),
                        new Regulus.Memorys.ChunkSetting(256, 1024),
                        new Regulus.Memorys.ChunkSetting(512, 1024),
                        new Regulus.Memorys.ChunkSetting(1024, 128),
                        new Regulus.Memorys.ChunkSetting(2048, 128) }                        

                ); 
        }
        static Regulus.Memorys.Pool _Shared;
        public static Regulus.Memorys.Pool Shared { 
            get
            {
                if (_Shared == null)
                    _Shared = Create();
                return _Shared;
            }
        }
    }
}
