using System;
using System.Linq;

namespace Regulus.Serialization
{
    public class TypeSet
    {
        private readonly ITypeDescriber[] _Describers;

        public TypeSet(ITypeDescriber[] describers)
        {
            _Describers = describers;            
        }

        public ITypeDescriber GetByType(Type type)
        {
            try
            {
                return _Describers.First(d => d.Type == type);
            }
            catch (Exception e)
            {
                
                throw new Exception(string.Format("沒有類型{0}." , type.FullName ));
            }
            
        }

        public ITypeDescriber GetById(int id)
        {
            try
            {
                return _Describers.First(d => d.Id == id);
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format("沒有Id {0}.", id));
            }
            
        }
    }
}