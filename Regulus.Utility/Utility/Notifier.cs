using System;

namespace Regulus.Utility
{
    public class Notifier
    {
        private bool _Have;
        event Action _Subscribe;
        public event Action Subscribe
        {
            add
            {
                if (_Have)
                    value();

                _Subscribe += value;
            }
            remove
            {
                _Subscribe -= value;
            }
        }

        protected void Invoke()
        {
            _Have = true;
            if (_Subscribe != null)
                _Subscribe();
        }
    }

    public class Notifier<T>
    {
        private bool _Have;
        private T _Info;
        private event Action<T> _Subscribe;
        public event Action<T> Subscribe
        {
            add
            {
                if (_Have)
                    value(_Info);

                _Subscribe += value;
            }
            remove
            {
                _Subscribe -= value;
            }
        }

        protected void Invoke(T info)
        {
            _Have = true;
            _Info = info;
            if (_Subscribe != null)
                _Subscribe(info);
        }
    }



}