// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindGuard.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the BindGuard type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Reflection;

using ProtoBuf;

#endregion

namespace Regulus.Remoting
{
	internal class BindGuard
	{
		public BindGuard(Type type)
		{
			_Check(type);
		}

		public BindGuard()
		{
			// TODO: Complete member initialization
		}

		private void _Check(Type type)
		{
			_CheckMethod(type.GetMethods());
			_CheckEvent(type.GetEvents());
		}

		private void _CheckEvent(EventInfo[] eventInfos)
		{
			foreach (var eventInfo in eventInfos)
			{
			}
		}

		private void _CheckMethod(MethodInfo[] methodInfos)
		{
			var names = new HashSet<string>();
			foreach (var method in methodInfos)
			{
				if (names.Contains(method.Name))
				{
					_ThrowErrorMethod(method, "The method name can not be repeated.");
				}
				else
				{
					names.Add(method.Name);
				}

				foreach (var args in method.GetParameters())
				{
					if (args.ParameterType.IsInterface)
					{
						_ThrowErrorMethod(method);
					}

					if (args.ParameterType.IsClass)
					{
						if (args.ParameterType.GetCustomAttributes(typeof (ProtoContractAttribute), true).Length == 0)
						{
							_ThrowErrorMethod(method);
						}
					}
				}
			}
		}

		private void _ThrowErrorMethod(MethodInfo method)
		{
			var message = string.Format("Invalid method [{0}].", method.Name);
			var expction = new Exception(message);
			throw expction;
		}

		private void _ThrowErrorMethod(MethodInfo method, string depiction)
		{
			var message = string.Format("Invalid method [{0}].{1}", method.Name, depiction);
			var expction = new Exception(message);
			throw expction;
		}

		internal void Check(Type type)
		{
			_Check(type);
		}
	}
}