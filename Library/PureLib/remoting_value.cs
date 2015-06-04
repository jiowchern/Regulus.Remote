using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

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

        public static T Result<T>(this Regulus.Remoting.Value<T> value)
        {
            System.Threading.WaitHandle handle = new System.Threading.AutoResetEvent(false);
            var valueSpin = new ValueWaiter<T>(value);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(valueSpin.Run), handle);
            System.Threading.WaitHandle.WaitAll(new[] { handle });
            return valueSpin.Value;
        }
    }

    /// <summary>
    /// 
    /// 接收或傳送遠端來的資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed public class Value<T> : IValue 
	{
        public static implicit operator Value<T>(T value) 
        {
            return new Value<T>(value);
        }
        bool _Interface;
		T _Value;
		bool _Empty = true;

        /// <summary>
        /// 建構子
        /// </summary>
        public Value()
        {
            _Interface = typeof(T).IsInterface;
        }

        /// <summary>
        /// 預設已經填入資料
        /// </summary>
        /// <param name="val"></param>
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

        /// <summary>
        /// 如果有設定資料則會發生此事件
        /// </summary>
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
        /// <summary>
        /// 設定資料，將會發生OnValue事件
        /// </summary>
        /// <param name="val"></param>
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
        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="val"></param>
        /// <returns>如果有資料則傳回真</returns>
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
        /// <summary>
        /// 空物件
        /// </summary>
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
