﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
	public interface IValue
	{
		object GetObject();
		void SetValue(byte[] val);

		void QueryValue(Action<object> action);

        void SetValue(Ghost.IGhost ghost);

        bool IsInterface();

        Type GetObjectType();
    }
    public static class Helper
    {
        public static Action<Value<T>> UnBox<T>(Action<T> callback)
        {

            return (val) => 
            {
                val.OnValue += callback;
            };
        }
    }
    sealed public class Value<T> : IValue 
	{
        public static implicit operator Value<T>(T value) 
        {
            return new Value<T>(value);
        }
        bool _Interface;
		T _Value;
		bool _Empty = true;
		public Value()
		{
            _Interface = typeof(T).IsInterface;
		}
		public Value(T val) : this()
		{
			_Empty = false;
			_Value = val;
			if (_OnValue != null)
			{
                _OnValue(_Value);				
			}				
		}
        event Action<T> _OnValue;
        public event Action<T> OnValue 
        {
            add 
            {                
                _OnValue += value;

                if (_Empty == false)
                {
                    value(_Value);
                }
            }

            remove 
            {
                _OnValue -= value;
            }
        }		

		object IValue.GetObject()
		{
			return _Value;
		}

        public void SetValue(T val)
        {
            _Empty = false;
            _Value = val;
            if (_OnValue != null)
            {
                _OnValue(_Value);
            }				
        }

        void IValue.SetValue(Regulus.Remoting.Ghost.IGhost val)
        {
            _Empty = false;

            _Value = (T)val;
            if (_OnValue != null)
            {
                _OnValue(_Value);
            }				
        }
		void IValue.SetValue(byte[] val)
		{
			_Empty = false;

            _Value = Regulus.Serializer.TypeHelper.Deserialize<T>(val);
            if (_OnValue != null)
			{
                _OnValue(_Value);
			}				
		}

        public bool TryGetValue(out T val)
        {
            if (_Empty == false)
            {
                val = _Value;
                return true;
            }
            val = default(T);
            return false;
        }
		void IValue.QueryValue(Action<object> action)
		{
			if (_Empty == false)
			{
				action.Invoke(_Value);
			}
			else
			{
				OnValue += (obj)=>{ action.Invoke(obj) ;};
			}
		}

        public static Value<T> Empty { get { return default(T); } }


        bool IValue.IsInterface()
        {
            return _Interface;
        }

        Type IValue.GetObjectType()
        {
            return typeof(T);
        }
    }
}
