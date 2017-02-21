namespace Regulus.Serialization
{
    internal class TypeInstance
    {
        private readonly ITypeProvider _Provider;
        private readonly object _Instance;
        public readonly int Id;
        
        public TypeInstance(ITypeProvider provider, object instance, int id )
        {
            _Provider = provider;
            _Instance = instance;
            Id = id;        
        }

        public void SetReference(int field, object instance)
        {
            _Provider.SetField(field , _Instance , instance);
        }

        public object GetInstance()
        {
            return _Instance;
        }


        public System.Type FindFieldType(int field)
        {
            return _Provider.GetFieldType(field);
        }
    }
}