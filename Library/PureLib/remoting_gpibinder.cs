using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting
{
    using Extension;
    public interface IGPIBinder   : Regulus.Framework.IBootable
    {        
    }

    

    public class GPIBinder<T> : IGPIBinder
        where T : class
    {

        public delegate CommandParam OnBuilder(T source);
        public delegate void OnSourceHandler(T source);


        
        public struct Data
        {
            OnBuilder _Builder;
            string _Name;
            public Data(string name, OnBuilder builder)
            {
                _Name = name;
                _Builder = builder;
            }
            public OnBuilder Builder { get { return _Builder; } }
            public string Name { get { return _Name; } }

            public string UnregisterName { get {
                return new Regulus.Utility.Command.Analysis(_Name).Command;
            } }
        }

        public event OnSourceHandler SupplyEvent;
        public event OnSourceHandler UnsupplyEvent;
        Regulus.Utility.Command _Command;
        Regulus.Remoting.Ghost.INotifier<T> _Notice;
        
        struct Source
        {
            public int Sn;            
            public T GPI;
        }
        List<Source> _GPIs;
        List<Data> _Handlers;
        List<CommandRegister> _InvokeDatas;        
        public GPIBinder(Regulus.Remoting.Ghost.INotifier<T> notice ,Regulus.Utility.Command command )
        {
            _Command = command;
            _Notice = notice;            
            _GPIs = new List<Source>();
            _Handlers = new List<Data>();
            _InvokeDatas = new List<CommandRegister>();
        }

        void _Notice_Supply(T obj)
        {            
            int sn = _Checkin(obj);
            _Register(obj, sn);

            if (SupplyEvent != null)
                SupplyEvent(obj);                       
        }
        void _Notice_Unsupply(T obj)
        {
            if (UnsupplyEvent != null)
                UnsupplyEvent(obj);

            int sn = _Checkout(obj);
            _Unregister(sn);
        }

        private void _Register(T obj, int sn)
        {
            foreach (var handler in _Handlers)
            {
                var param = handler.Builder(obj);
                _Command.Register(_BuileName(sn, handler.Name), param);
            }

            foreach (var id in _InvokeDatas)
            {
                id.Register(obj);
            }
        }
        

        private void _Unregister(int sn)
        {
            foreach (var id in _InvokeDatas)
            {
                id.Unregister();
            }


            foreach (var handler in _Handlers)
            {
                _Command.Unregister(_BuileName(sn , handler.UnregisterName));
            }

            
        }

        private int _Checkin(T obj)
        {
            int sn = _GetSn();
            _GPIs.Add(new Source { GPI = obj, Sn = sn });
            return sn ; 
        }

        private int _Checkout(T obj)
        {
            Source source = _Find(obj);
            _GPIs.Remove(source);
            return source.Sn;
        }

        private Source _Find(T obj)
        {
            return (from source in _GPIs where source.GPI == obj select source).SingleOrDefault();
        }

        int _Sn;
        private int _GetSn()
        {
            return _Sn++;
        }

        private static string _BuileName(int sn, string name)
        {
            return sn.ToString() + name;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Notice.Supply += _Notice_Supply;
            _Notice.Unsupply += _Notice_Unsupply;            
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Notice.Unsupply -= _Notice_Unsupply;
            _Notice.Supply -= _Notice_Supply;


            foreach (var gpi in _GPIs.ToArray())
            {
                _Notice_Unsupply(gpi.GPI);
            }
        }

        public void Bind( string name , OnBuilder builder)
        {
            _Handlers.Add(new Data(name, builder));
        }

        public void Bind( System.Linq.Expressions.Expression<Action<T>> exp)
        {
            var name = _GetName(exp);            
            _InvokeDatas.Add(new CommandRegister<T>(name[0], name.Skip(1).ToArray(), _Command, exp));            
        }

        public void Bind<T1>(System.Linq.Expressions.Expression<Action<T,T1>> exp)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegister<T,T1>(name[0], name.Skip(1).ToArray(), _Command, exp));                        
        }
        public void Bind<T1, T2>(System.Linq.Expressions.Expression<Action<T, T1, T2>> exp)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegister<T, T1, T2>(name[0], name.Skip(1).ToArray(), _Command, exp));
            
        }

        public void Bind<T1, T2,T3>(System.Linq.Expressions.Expression<Action<T, T1, T2, T3>> exp)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegister<T, T1, T2,T3>(name[0], name.Skip(1).ToArray(), _Command, exp));            
        }

        public void Bind<T1, T2, T3, T4>(System.Linq.Expressions.Expression<Regulus.Remoting.Callback<T, T1, T2, T3, T4>> exp)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegister<T, T1, T2, T3, T4>(name[0], name.Skip(1).ToArray(), _Command, exp));            
        }

        public void Bind<TR>(System.Linq.Expressions.Expression<Func<T,TR>> exp , Action<TR> ret)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegisterReturn<T, TR>(name[0], name.Skip(1).ToArray(), _Command, exp , ret) );            
        }

        public void Bind<T1, TR>(System.Linq.Expressions.Expression<Func<T, T1, TR>> exp, Action<TR> ret)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegisterReturn<T, T1, TR>(name[0], name.Skip(1).ToArray(), _Command, exp, ret));
        }
        public void Bind<T1, T2, TR>(System.Linq.Expressions.Expression<Func<T, T1, T2, TR>> exp, Action<TR> ret)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegisterReturn<T, T1, T2, TR>(name[0], name.Skip(1).ToArray(), _Command, exp, ret));
        }

        public void Bind<T1, T2, T3, TR>(System.Linq.Expressions.Expression<Func<T, T1, T2, T3, TR>> exp, Action<TR> ret)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegisterReturn<T, T1, T2, T3, TR>(name[0], name.Skip(1).ToArray(), _Command, exp, ret));
        }

        public void Bind<T1, T2, T3, T4, TR>(System.Linq.Expressions.Expression<Regulus.Remoting.Callback<T,T1, T2, T3, T4, TR>> exp, Action<TR> ret)
        {
            var name = _GetName(exp);
            _InvokeDatas.Add(new CommandRegisterReturn<T, T1, T2, T3, T4, TR>(name[0], name.Skip(1).ToArray(), _Command, exp, ret));
        }

        private string[] _GetName(System.Linq.Expressions.LambdaExpression exp)
        {
            string methodName;

            if (exp.Body.NodeType != System.Linq.Expressions.ExpressionType.Call)
            {
                throw new ArgumentException();
            }

            var methodCall = exp.Body as System.Linq.Expressions.MethodCallExpression;
            var method = methodCall.Method;            
            methodName = method.Name;


            var argNames = (from par in exp.Parameters.Skip(1) select par.Name);
            if (method.ReturnType == null)
                return new string[] { methodName }.Concat(argNames).ToArray();

            return new string[] { methodName }.Concat(new string[] { "return" }).Concat(argNames).ToArray();

        }
    }
}
