using System;

namespace RegulusLibraryTest
{
    internal class UnanalyzableCommandHandler
    {
        public bool Called;
        public UnanalyzableCommandHandler()
        {
        }

        internal void Run()
        {
            Called = true;
        }
    }
}