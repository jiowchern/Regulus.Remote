using System;
using System.Linq;

namespace Regulus.Serialization
{
    internal class DescriberProvider : ITypeDescriberProvider
    {
        private readonly ITypeDescriber[] _Describers;

        public DescriberProvider(params ITypeDescriber[] describers)
        {
            _Describers = describers;
            foreach (var typeDescriber in describers)
            {
                typeDescriber.SetMap(this);
            }
        }

        ITypeDescriber ITypeDescriberFinder.GetById(int id)
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

        ITypeDescriber ITypeDescriberFinder.GetByType(Type type)
        {
            try
            {
                return _Describers.First(d => d.Type == type);
            }
            catch (Exception e)
            {

                throw new Exception(string.Format("沒有類型{0}.", type.FullName));
            }
        }
    }
}