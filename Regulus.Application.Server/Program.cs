using Regulus.Network;
using Regulus.Utility.WindowConsoleAppliction;
using System.Collections.Generic;
using System.IO;

namespace Regulus.Application.Server
{
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="protocol"></param>
        /// <param name="entry"></param>
        /// <param name="entryname"></param>
        /// <param name="mode"></param>
        private static void Main(int port, FileInfo protocol, FileInfo entry, string entryname, SOCKETMODE mode = SOCKETMODE.TCP)
        {

            List<string> command = new List<string>();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(entry.FullName);
            Remote.IEntry instance = assembly.CreateInstance(entryname) as Remote.IEntry;

            Remote.IProtocol p = Regulus.Remote.Protocol.ProtocolProvider.Create(System.Reflection.Assembly.LoadFrom(protocol.FullName));


            Remote.Soul.Console.Application app = new Regulus.Remote.Soul.Console.Application(instance, p, port, _CreateSocket(mode));

            app.Run();
        }

        private static IListenable _CreateSocket(SOCKETMODE mode)
        {
            if (mode == SOCKETMODE.WEB)
            {
                return new Regulus.Network.Web.Listener();
            }

            return new Regulus.Network.Tcp.Listener();
        }
    }
}
