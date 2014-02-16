using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class Ini
    {

        IniParser.Model.IniData _Data;
        
        public Ini(string stream)
        {
            _Data = new IniParser.Parser.IniDataParser().Parse(stream);            
        }
        
        public void Write(string Section,string Key,string Value)
        {
            _Data[Section][Key] = Value;
        }        
        
        public string Read(string Section,string Key)
        {
            return _Data[Section][Key];
        }
    }
}
