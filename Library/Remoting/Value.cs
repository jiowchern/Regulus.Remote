﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting
{
	public interface IValue
	{
		object GetObject();
		void SetValue(object val);

		void QueryValue(Action<object> action);
	}
    sealed public class Value<T> : IValue 
	{
        public static implicit operator Value<T>(T value)
        {
            return new Value<T>(value);
        }
		T _Value;
		bool _Empty = true;
		public Value()
		{
			
		}
		public Value(T val)
		{
			_Empty = false;
			_Value = val;
			if (OnValue != null)
			{
				OnValue(_Value);				
			}				
		}

		public event Action<T> OnValue;		

		object IValue.GetObject()
		{
			return _Value;
		}

		void IValue.SetValue(object val)
		{
			_Empty = false;
			_Value = (T)val ;
			if (OnValue != null)
			{
				OnValue(_Value);
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
	}
}
