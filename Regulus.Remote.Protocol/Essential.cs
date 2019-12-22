using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Protocol
{
    public class Essential
    {
       
        

        public Essential(params Assembly[] assemblies)
        {

            
            Assemblys = assemblies.ToArray();
            Common = assemblies.First();
            
        }

        public readonly Assembly[] Assemblys ;

        public readonly Assembly Common;

        public static Essential CreateFromDomain(Assembly common)
        {
            var e = new Essential(common , _Find("Regulus.Utility"), _Find("Regulus.Remote"), _Find("Regulus.Serialization"), _Find("netstandard"), _Find("System.Runtime"),  typeof(object).Assembly , typeof(System.Linq.Expressions.LambdaExpression).Assembly , _Find("System.Collections"));
            return e;                      
        }

        public static Essential Create(Assembly[] inputAssemblys)
        {
            
            var e = new Essential(inputAssemblys);
            return e;
        }

        static Assembly _Find(string name)
        {
            var domain = System.AppDomain.CurrentDomain;
            var assemblys = domain.GetAssemblies();
            var assembly = assemblys.Where(asm => asm.GetName().Name == name).FirstOrDefault();

            if (assembly == null)
            {
                var path = domain.BaseDirectory;
                var fullPath = System.IO.Path.Combine(path, $"{name}.dll");
                try
                {
                    assembly = System.Reflection.Assembly.LoadFile(fullPath);
                    Regulus.Utility.Log.Instance.WriteInfo($"Loaded Assembly {assembly.Location}");
                    return assembly;
                }
                catch (System.IO.FileLoadException fle)
                {
                    throw new System.Exception($"找不到指定dll，{fullPath}。");
                }
                catch (System.BadImageFormatException bfe)
                {
                    throw new System.Exception($"dll版本太舊，{fullPath}。");
                }
            }
            Regulus.Utility.Log.Instance.WriteInfo($"Loaded Assembly {assembly.Location}");
            return assembly;
        }
    }
}