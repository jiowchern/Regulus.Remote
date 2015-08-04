// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Value.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Threading;

#endregion

namespace Regulus.Remoting
{
	public interface IValue
	{
		object GetObject();

		void SetValue(byte[] val);

		void QueryValue(Action<object> action);

		void SetValue(IGhost ghost);

		bool IsInterface();

		Type GetObjectType();
	}

	public static class Helper
	{
		public static Action<Value<T>> UnBox<T>(Action<T> callback)
		{
			return val => { val.OnValue += callback; };
		}

		public static T Result<T>(this Value<T> value)
		{
			WaitHandle handle = new AutoResetEvent(false);
			var valueSpin = new ValueWaiter<T>(value);
			ThreadPool.QueueUserWorkItem(valueSpin.Run, handle);
			WaitHandle.WaitAll(new[]
			{
				handle
			});
			return valueSpin.Value;
		}
	}

	/// <summary>
	///     接收或傳送遠端來的資料
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class Value<T> : IValue
	{
		private event Action<T> _OnValue;

		/// <summary>
		///     如果有設定資料則會發生此事件
		/// </summary>
		public event Action<T> OnValue
		{
			add
			{
				this._OnValue += value;

				if (this._Empty == false)
				{
					value(this._Value);
				}
			}

			remove { this._OnValue -= value; }
		}

		private readonly bool _Interface;

		private bool _Empty = true;

		private T _Value;

		/// <summary>
		///     空物件
		/// </summary>
		public static Value<T> Empty
		{
			get { return default(T); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Value{T}"/> class. 
		///     建構子
		/// </summary>
		public Value()
		{
			this._Interface = typeof (T).IsInterface;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Value{T}"/> class. 
		///     預設已經填入資料
		/// </summary>
		/// <param name="val">
		/// </param>
		public Value(T val) : this()
		{
			this._Empty = false;
			this._Value = val;
			if (this._OnValue != null)
			{
				this._OnValue(this._Value);
			}
		}

		object IValue.GetObject()
		{
			return this._Value;
		}

		void IValue.SetValue(IGhost val)
		{
			this._Empty = false;

			this._Value = (T)val;
			if (this._OnValue != null)
			{
				this._OnValue(this._Value);
			}
		}

		void IValue.SetValue(byte[] val)
		{
			this._Empty = false;

			this._Value = TypeHelper.Deserialize<T>(val);
			if (this._OnValue != null)
			{
				this._OnValue(this._Value);
			}
		}

		void IValue.QueryValue(Action<object> action)
		{
			if (this._Empty == false)
			{
				action.Invoke(this._Value);
			}
			else
			{
				this.OnValue += obj => { action.Invoke(obj); };
			}
		}

		bool IValue.IsInterface()
		{
			return this._Interface;
		}

		Type IValue.GetObjectType()
		{
			return typeof (T);
		}

		public static implicit operator Value<T>(T value)
		{
			return new Value<T>(value);
		}

		public bool HasValue()
		{
			return this._Empty == false;
		}

		/// <summary>
		///     設定資料，將會發生OnValue事件
		/// </summary>
		/// <param name="val"></param>
		public void SetValue(T val)
		{
			this._Empty = false;
			this._Value = val;
			if (this._OnValue != null)
			{
				this._OnValue(this._Value);
			}
		}

		/// <summary>
		///     取得資料
		/// </summary>
		/// <param name="val"></param>
		/// <returns>如果有資料則傳回真</returns>
		public bool TryGetValue(out T val)
		{
			if (this._Empty == false)
			{
				val = this._Value;
				return true;
			}

			val = default(T);
			return false;
		}
	}
}