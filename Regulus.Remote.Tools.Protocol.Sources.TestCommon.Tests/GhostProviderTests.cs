using NSubstitute;
using System;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon.Tests
{
    
    public class GhostProviderTests
    {
        static bool _HasSub = false;
        [NUnit.Framework.Test]
        public void AuroReleaseTest()
        {

            var protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase1();
            var es = new Regulus.Remote.Serializer(protocol.SerializeTypes);
            IInternalSerializable iniers = new Regulus.Remote.InternalSerializer();
            var exchanger = new OpCodeExchanger();

            var provider = new Regulus.Remote.GhostProviderQueryer(protocol, es, iniers, exchanger);

            provider.Start();

            IMethodable methodable = null;
            provider.QueryProvider<IMethodable>().Supply += (gpi) =>
            {
                methodable = gpi;
            };


            {
                var package = new Regulus.Remote.Packages.PackageLoadSoul();
                package.TypeId = protocol.GetMemberMap().GetInterface(typeof(IMethodable));
                package.EntityId = 1;
                package.ReturnType = false;
                exchanger.Responser.Invoke(ServerToClientOpCode.LoadSoul, iniers.Serialize(package).ToArray());

            }

            {
                var package = new Regulus.Remote.Packages.PackageLoadSoulCompile();
                package.TypeId = protocol.GetMemberMap().GetInterface(typeof(IMethodable));
                package.EntityId = 1;
                package.ReturnId = 0;
                exchanger.Responser.Invoke(ServerToClientOpCode.LoadSoulCompile, iniers.Serialize(package).ToArray());
            }
            

            _HasSub = false;
            _SetHasSub(methodable);

            {
                var package = new Regulus.Remote.Packages.PackageLoadSoul();
                package.TypeId = protocol.GetMemberMap().GetInterface(typeof(IMethodable));
                package.EntityId = 2;
                package.ReturnType = true;
                exchanger.Responser.Invoke(ServerToClientOpCode.LoadSoul, iniers.Serialize(package).ToArray());

            }

            {
                var package = new Regulus.Remote.Packages.PackageLoadSoulCompile();
                package.TypeId = protocol.GetMemberMap().GetInterface(typeof(IMethodable));
                package.EntityId = 2;
                package.ReturnId = 1;
                exchanger.Responser.Invoke(ServerToClientOpCode.LoadSoulCompile, iniers.Serialize(package).ToArray());
            }

            {
                var package = new Regulus.Remote.Packages.PackageUnloadSoul();
                package.EntityId = 1;

                exchanger.Responser.Invoke(ServerToClientOpCode.UnloadSoul, iniers.Serialize(package).ToArray());
            }


            GC.Collect(2, GCCollectionMode.Forced, true);
            
            {
                exchanger.Responser.Invoke(ServerToClientOpCode.Ping, iniers.Serialize(new byte[0]).ToArray());
            }

            var pkg = exchanger.IgnoreUntil(ClientToServerOpCode.Release);

            provider.Stop();
            NUnit.Framework.Assert.True(_HasSub);
            NUnit.Framework.Assert.AreNotEqual(null, pkg);
        }

        private static void _SetHasSub(IMethodable methodable)
        {
            methodable.GetValueSelf().OnValue += (gpi) => _HasSub = true;
            
        }

    }
}