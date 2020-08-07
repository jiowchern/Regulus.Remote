using System;
using System.IO;

namespace Regulus.Utility
{
    public class LogFileRecorder
    {
        private readonly FileStream _File;
        private readonly BufferedStream _Buffer;
        private readonly StreamWriter _Writer;



        public LogFileRecorder(string name)
        {
            string path = string.Format("{0}_{1}.log", name, DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
            _File = new FileStream(path, FileMode.Create);
            _Buffer = new BufferedStream(_File);
            _Writer = new StreamWriter(_Buffer);
        }


        public void Record(string message)
        {
            _Writer.WriteLine(message);
        }

        public void Save()
        {
            _Writer.Flush();
            _Buffer.Flush();
            _File.Flush();
        }


        public void Close()
        {
            Save();

            _Writer.Close();
            _Buffer.Close();
            _File.Close();
        }
    }
}
