using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeysUserConsole
{
    static class ClosuresHelper
    {
        

        static public void Cnv(string p, out object val ,Type source)
        {

            val = p;
            
            if (source == typeof(int))
            {
                int reault = int.MinValue;
                if(int.TryParse(p, out reault))
                {
                    
                }
                val = reault;
            }
            else if (source == typeof(float))
            {
                float reault = float.MinValue;
                if (float.TryParse(p, out reault))
                {
                    
                }
                val = reault;
            }
            
            

        }
    }       
        
}
