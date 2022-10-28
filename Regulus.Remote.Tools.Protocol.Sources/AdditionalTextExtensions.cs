using Microsoft.CodeAnalysis;
using Regulus.Remote.Tools.Protocol.Sources;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    static class AdditionalTextExtensions
    {
        public static System.Collections.Generic.IEnumerable<Configuration> GetConfigurations(this System.Collections.Generic.IEnumerable<AdditionalText> texts)
        {
            foreach (var text in texts)
            {
                var cfg = text.GetConfiguration();
                if(cfg == null)
                    continue;
                yield return cfg;
            }
        }
        public static Configuration GetConfiguration(this AdditionalText text)
        {
            var filename = System.IO.Path.GetFileName(text.Path).ToLower();
            if (filename != Configuration.Filename)
                return null;

            
            
            var str = text.GetText()?.ToString();
            if (str == null)
                return new Configuration();

            var cfg = Tomlet.TomletMain.To<Configuration>(str);
            return cfg;
        }

        
    }
}