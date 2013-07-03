

namespace Regulus.Utility
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Collections.Generic;
	using System.Text;

	

	[Serializable()]
	public class Map<TKey, TVal> : Dictionary<TKey, TVal>, IXmlSerializable, ISerializable
	{
		public class Converter : System.ComponentModel.TypeConverter
		{
			public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof(Regulus.Utility.Map<TKey, TKey>))
				{
					return true;
				}
				return base.CanConvertFrom(context, sourceType);
			}
			public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
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
				return base.ConvertFrom(context, culture, value);
			}
			public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof(Regulus.Utility.Map<TKey, TKey>))
				{
					return true;
				}
				return base.CanConvertFrom(context, destinationType);
			}

			public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				Object[] objs = new Object[0];
				if (value != null)
				{
					if (destinationType == typeof(Regulus.Utility.Map<TKey, TKey>))
					{
						Regulus.Utility.Map<TKey, TKey> qn = (Regulus.Utility.Map<TKey, TKey>)value;
						objs = new Object[qn.Count];
						int idx = 0;
						foreach (var page in qn)
						{
							objs[idx] = (Object)page;
							idx++;
						}

						return objs;
					}
				}
				return base.ConvertTo(context, culture, value, destinationType); 
			}
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

		static System.ComponentModel.TypeConverterAttribute conv = null;			
		public Map()
		{
			if(conv == null)
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
			int itemCount = info.GetInt32("ItemCount");
			for (int i = 0; i < itemCount; i++)
			{
				KeyValuePair<TKey, TVal> kvp = (KeyValuePair<TKey, TVal>)info.GetValue(String.Format("Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
				this.Add(kvp.Key, kvp.Value);
			}
		}
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ItemCount", this.Count);
			int itemIdx = 0;
			foreach (KeyValuePair<TKey, TVal> kvp in this)
			{
				info.AddValue(String.Format("Item{0}", itemIdx), kvp, typeof(KeyValuePair<TKey, TVal>));
				itemIdx++;
			}
		}

		#endregion
		#region IXmlSerializable Members

		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			//writer.WriteStartElement(DictionaryNodeName);
			foreach (KeyValuePair<TKey, TVal> kvp in this)
			{
				writer.WriteStartElement(ItemNodeName);
				writer.WriteStartElement(KeyNodeName);
				KeySerializer.Serialize(writer, kvp.Key);
				writer.WriteEndElement();
				writer.WriteStartElement(ValueNodeName);
				ValueSerializer.Serialize(writer, kvp.Value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			//writer.WriteEndElement();
		}

		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
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

			//reader.ReadStartElement(DictionaryNodeName);
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement(ItemNodeName);
				reader.ReadStartElement(KeyNodeName);
				TKey key = (TKey)KeySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement(ValueNodeName);
				TVal value = (TVal)ValueSerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadEndElement();
				this.Add(key, value);
				reader.MoveToContent();
			}
			//reader.ReadEndElement();

			reader.ReadEndElement(); // Read End Element to close Read of containing node
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion
		#region Private Properties
		protected XmlSerializer ValueSerializer
		{
			get
			{
				if (valueSerializer == null)
				{
					valueSerializer = new XmlSerializer(typeof(TVal));
				}
				return valueSerializer;
			}
		}

		private XmlSerializer KeySerializer
		{
			get
			{
				if (keySerializer == null)
				{
					keySerializer = new XmlSerializer(typeof(TKey));
				}
				return keySerializer;
			}
		}
		#endregion
		#region Private Members
		private XmlSerializer keySerializer = null;
		private XmlSerializer valueSerializer = null;
		#endregion
	}
}
