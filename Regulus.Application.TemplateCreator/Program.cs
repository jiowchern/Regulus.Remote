using System;

namespace Regulus.Application
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SlnNamespace"></param>
        /// <param name="LibraryDir"></param>
        /// <param name="OutputDir"></param>
        static  int Main(string SlnNamespace, System.IO.DirectoryInfo LibraryDir, System.IO.DirectoryInfo OutputDir)
        {            
            var task = new TemplateCreator(OutputDir, SlnNamespace, LibraryDir).Run();

            return task.GetAwaiter().GetResult()?0:1;
        }
    }
}
