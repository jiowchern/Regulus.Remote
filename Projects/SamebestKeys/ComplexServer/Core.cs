using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeys
{
    public class ComplexServer :Regulus.Game.ICore
    {
        Regulus.Utility.Updater _Updater;
        Regulus.Game.ICore _Complex;
        Storage _Storage;


        public ComplexServer()
        {
            _Storage = new Storage();
            _Complex = new Complex(_Storage);
            _Updater = new Utility.Updater();
        }
        void Game.ICore.ObtainController(Remoting.ISoulBinder binder)
        {
            _Complex.ObtainController(binder);
        }

        bool Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;    
        }

        void Framework.ILaunched.Launch()
        {

            

            _Updater.Add(_Storage);
            _Updater.Add(_Complex);


            var dataBuilder = new GameDataBuilder(GameData.Instance);
            var workPath = System.IO.Directory.GetCurrentDirectory();
            var files = System.IO.Directory.GetFiles(workPath, "*.map.txt");
            foreach (var file in files)
            {
                var stream = System.IO.File.ReadAllBytes(file);
                dataBuilder.Build(stream);            
            }
            
            
    
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
