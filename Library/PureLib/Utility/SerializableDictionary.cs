// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableDictionary.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Map type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace Regulus.Utility
{
	[Serializable]
	public class Map<TKey, TVal> : Dictionary<TKey, TVal>, IXmlSerializable, ISerializable
	{
		public class Converter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof (Map<TKey, TVal>))
				{
					return true;
				}

				return base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				var qn = new Map<TKey, TVal>();
				if (value != null)
				{
					var parts = (object[])value;
					foreach (var part in parts)
					{
						var kv = (KeyValuePair<TKey, TVal>)part;
						qn.Add(kv.Key, kv.Value);
					}

					return qn;
				}

				return base.ConvertFrom(context, culture, value);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof (Map<TKey, TVal>))
				{
					return true;
				}

				return base.CanConvertFrom(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, 
				Type destinationType)
			{
				var objs = new object[0];
				if (value != null)
				{
					if (destinationType == typeof (Map<TKey, TVal>))
					{
						var qn = (Map<TKey, TVal>)value;
						objs = new object[qn.Count];
						var idx = 0;
						foreach (var page in qn)
						{
							objs[idx] = page;
							idx++;
						}

						return objs;
					}
				}

				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		public static Map<TKey, TVal> ToMap(Dictionary<TKey, TVal> value)
		{
			var map = new Map<TKey, TVal>();
			foreach (var pair in value)
			{
				map.Add(pair.Key, pair.Value);
			}

			return map;
		}

		/*public static implicit operator Regulus.Utility.Map<TKey, TKey>(Object[] value)
		{
			Regulus.Utility.Map<TKey, TKey> qn = new Utility.Map<TKey, TKey>();
			if (value != null)
			{
				Object[] parts = (Object[])value;
				foreach (var part in parts)
				{
					System.Collections.Generic.KeyValuePair<TKey, TKey> kv = (System.Collections.Generic.KeyValuePair<TKey, TKey>)part;
					qn.Add(kv.Key, kv.Value);
				}
				return qn;
			}
			return qn;
		}*/
		#region Constants

		private const string DictionaryNodeName = "Regulus.Utility.Map";

		private const string ItemNodeName = "Item";

		private const string KeyNodeName = "Key";

		private const string ValueNodeName = "Value";

		#endregion

		#region Constructors

		private static readonly TypeConverterAttribute conv = null;

		public Map()
		{
			if (Map<TKey, TVal>.conv == null)
			{
			}
		}

		public Map(IDictionary<TKey, TVal> dictionary)
			: base(dictionary)
		{
		}

		public Map(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{
		}

		public Map(int capacity)
			: base(capacity)
		{
		}

		public Map(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer)
		{
		}

		public Map(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity, comparer)
		{
		}

		#endregion

		#region ISerializable Members

		protected Map(SerializationInfo info, StreamingContext context)
		{
			var itemCount = info.GetInt32("ItemCount");
			for (var i = 0; i < itemCount; i++)
			{
				var kvp = (KeyValuePair<TKey, TVal>)info.GetValue(string.Format("Item{0}", i), typeof (KeyValuePair<TKey, TVal>));
				this.Add(kvp.Key, kvp.Value);
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ItemCount", this.Count);
			var itemIdx = 0;
			foreach (var kvp in this)
			{
				info.AddValue(string.Format("Item{0}", itemIdx), kvp, typeof (KeyValuePair<TKey, TVal>));
				itemIdx++;
			}
		}

		#endregion

		#region IXmlSerializable Members

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			// writer.WriteStartElement(DictionaryNodeName);
			foreach (var kvp in this)
			{
				writer.WriteStartElement(Map<TKey, TVal>.ItemNodeName);
				writer.WriteStartElement(Map<TKey, TVal>.KeyNodeName);
				this.KeySerializer.Serialize(writer, kvp.Key);
				writer.WriteEndElement();
				writer.WriteStartElement(Map<TKey, TVal>.ValueNodeName);
				this.ValueSerializer.Serialize(writer, kvp.Value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}

			// writer.WriteEndElement();
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				return;
			}

			// Move past container
			if (!reader.Read())
			{
				throw new XmlException("Error in Deserialization of Dictionary");
			}

			// reader.ReadStartElement(DictionaryNodeName);
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement(Map<TKey, TVal>.ItemNodeName);
				reader.ReadStartElement(Map<TKey, TVal>.KeyNodeName);
				var key = (TKey)this.KeySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement(Map<TKey, TVal>.ValueNodeName);
				var value = (TVal)this.ValueSerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadEndElement();
				this.Add(key, value);
				reader.MoveToContent();
			}

			// reader.ReadEndElement();
			reader.ReadEndElement(); // Read End Element to close Read of containing node
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion

		#region Private Properties

		protected XmlSerializer ValueSerializer
		{
			get
			{
				if (this.valueSerializer == null)
				{
					this.valueSerializer = new XmlSerializer(typeof (TVal));
				}

				return this.valueSerializer;
			}
		}

		private XmlSerializer KeySerializer
		{
			get
			{
				if (this.keySerializer == null)
				{
					this.keySerializer = new XmlSerializer(typeof (TKey));
				}

				return this.keySerializer;
			}
		}

		#endregion

		#region Private Members

		private XmlSerializer keySerializer;

		private XmlSerializer valueSerializer;

		#endregion
	}
}