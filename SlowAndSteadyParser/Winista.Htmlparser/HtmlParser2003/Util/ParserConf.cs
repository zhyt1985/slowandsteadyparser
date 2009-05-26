// ***************************************************************
//  NutchConf   version:  1.0   Date: 12/12/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Xml;

using Winista.Text.HtmlParser.Support;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary>
	/// Summary description for NutchConf.
	/// </summary>
	public class ParserConf
	{
		private String m_strRootConfPath = String.Empty;
		private System.Collections.IList m_collResourceNames = new System.Collections.ArrayList();
		private System.Collections.Specialized.NameValueCollection m_collProperties;
		private static ParserConf DEFAULT = new ParserConf();

		/// <summary>
		/// 
		/// </summary>
		private ParserConf()
		{
			m_collResourceNames.Add("htmlparser-default.xml");
			m_collResourceNames.Add("htmlparser-site.xml");
		}
		/// <summary>A new configuration. </summary>
		public ParserConf(String strRootPath)
		{
			m_strRootConfPath = strRootPath;
			m_collResourceNames.Add("htmlparser-default.xml");
			m_collResourceNames.Add("htmlparser-site.xml");
		}

		/// <summary>
		/// 
		/// </summary>
		public String RootPath
		{
			get
			{
				return this.m_strRootConfPath;
			}
			set
			{
				this.m_strRootConfPath = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static ParserConf GetConfiguration()
		{
			return DEFAULT;
		}

		/// <summary>Adds a resource name to the chain of resources read.  Such resources are
		/// located on the CLASSPATH.  The first resource is always
		/// <tt>nutch-default.xml</tt>, and the last is always
		/// <tt>nutch-site.xml</tt>.  New resources are inserted between these, so
		/// they can override defaults, but not site-specifics. 
		/// </summary>
		public virtual void  AddConfResource(System.String name)
		{
			lock (this)
			{
				AddConfResourceInternal(name);
			}
		}

		/// <summary>Adds a file to the chain of resources read.  The first resource is always
		/// <tt>nutch-default.xml</tt>, and the last is always
		/// <tt>nutch-site.xml</tt>.  New resources are inserted between these, so
		/// they can override defaults, but not site-specifics. 
		/// </summary>
		public virtual void AddConfResource(System.IO.FileInfo file)
		{
			lock (this)
			{
				AddConfResourceInternal(file.Name);
			}
		}
		
		private void  AddConfResourceInternal(System.Object name)
		{
			lock (this)
			{
				m_collResourceNames.Insert(m_collResourceNames.Count - 1, name); // add second to last
				m_collProperties = null; // trigger reload
			}
		}

		/// <summary>Returns the value of the <code>name</code> property, or null if no
		/// such property exists. 
		/// </summary>
		public virtual System.String GetPoperty(System.String name)
		{
			return Props.Get(name);
		}

		/// <summary>Sets the value of the <code>name</code> property. </summary>
		public virtual void  SetProperty(System.String name, System.Object value_Renamed)
		{
			Props[name] = value_Renamed.ToString();
		}

		/// <summary>Returns the value of the <code>name</code> property.  If no such property
		/// exists, then <code>defaultValue</code> is returned.
		/// </summary>
		public virtual System.String GetPoperty(System.String name, System.String defaultValue)
		{
			return Props[name] == null ? defaultValue:Props[name];
		}

		/// <summary>Returns the value of the <code>name</code> property as an integer.  If no
		/// such property is specified, or if the specified value is not a valid
		/// integer, then <code>defaultValue</code> is returned.
		/// </summary>
		public virtual int GetInt(System.String name, int defaultValue)
		{
			System.String valueString = GetPoperty(name);
			if (valueString == null)
			{
				return defaultValue;
			}

			try
			{
				return System.Int32.Parse(valueString);
			}
			catch
			{
				return defaultValue;
			}
		}
		
		/// <summary>Sets the value of the <code>name</code> property to an integer. </summary>
		public virtual void SetInt(System.String name, int val)
		{
			SetProperty(name, System.Convert.ToString(val));
		}
				
		/// <summary>Returns the value of the <code>name</code> property as a long.  If no
		/// such property is specified, or if the specified value is not a valid
		/// long, then <code>defaultValue</code> is returned.
		/// </summary>
		public virtual long GetLong(System.String name, long defaultValue)
		{
			System.String valueString = GetPoperty(name);
			if (valueString == null)
			{
				return defaultValue;
			}

			try
			{
				return System.Int64.Parse(valueString);
			}
			catch
			{
				return defaultValue;
			}
		}
		
		/// <summary>Returns the value of the <code>name</code> property as a float.  If no
		/// such property is specified, or if the specified value is not a valid
		/// float, then <code>defaultValue</code> is returned.
		/// </summary>
		public virtual float GetFloat(System.String name, float defaultValue)
		{
			System.String valueString = GetPoperty(name);
			if (valueString == null)
			{
				return defaultValue;
			}

			try
			{
				return System.Single.Parse(valueString);
			}
			catch
			{
				return defaultValue;
			}
		}
		
		/// <summary>Returns the value of the <code>name</code> property as an boolean.  If no
		/// such property is specified, or if the specified value is not a valid
		/// boolean, then <code>defaultValue</code> is returned.  Valid boolean values
		/// are "true" and "false".
		/// </summary>
		public virtual bool GetBoolean(System.String name, bool defaultValue)
		{
			System.String valueString = GetPoperty(name);
			if ("true".Equals(valueString))
			{
				return true;
			}
			else if ("false".Equals(valueString))
			{
				return false;
			}
			else
			{
				return defaultValue;
			}
		}
		
		/// <summary>Returns the value of the <code>name</code> property as an array of
		/// strings.  If no such property is specified, then <code>null</code>
		/// is returned.  Values are whitespace or comma delimted.
		/// </summary>
		public virtual System.String[] GetStrings(System.String name)
		{
			System.String valueString = GetPoperty(name);
			if (valueString == null)
			{
				return null;
			}
			Tokenizer tokenizer = new Tokenizer(valueString, ", \t\n\r\f");
			System.Collections.IList values = new System.Collections.ArrayList();
			while (tokenizer.HasMoreTokens())
			{
				values.Add(tokenizer.NextToken());
			}
			return (System.String[]) Support.ICollectionSupport.ToArray(values, new System.String[values.Count]);
		}
		
		/// <summary>Returns the value of the <code>name</code> property as a Class.  If no
		/// such property is specified, then <code>defaultValue</code> is returned.
		/// </summary>
		public virtual System.Type GetClass(System.String name, System.Type defaultValue)
		{
			System.String valueString = GetPoperty(name);
			if (valueString == null)
			{
				return defaultValue;
			}

			try
			{
				return System.Type.GetType(valueString);
			}
				//UPGRADE_NOTE: Exception 'java.lang.ClassNotFoundException' was converted to 'System.Exception' which has different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1100_3"'
			catch (System.Exception e)
			{
				throw new ParserException("Failed to get class type", e);
			}
		}
		
		/// <summary>Returns the value of the <code>name</code> property as a Class.  If no
		/// such property is specified, then <code>defaultValue</code> is returned.
		/// An error is thrown if the returned class does not implement the named
		/// interface. 
		/// </summary>
		public virtual System.Type GetClass(System.String propertyName, System.Type defaultValue, System.Type xface)
		{
			try
			{
				System.Type theClass = GetClass(propertyName, defaultValue);
				if (theClass != null && !xface.IsAssignableFrom(theClass))
				{
					throw new System.SystemException(theClass + " not " + xface.FullName);
				}
				return theClass;
			}
			catch (System.Exception e)
			{
				throw new ParserException("Failed to get class type", e);
			}
		}
		
		/// <summary>Sets the value of the <code>name</code> property to the name of a class.
		/// First checks that the class implements the named interface. 
		/// </summary>
		public virtual void SetClass(System.String propertyName, System.Type theClass, System.Type xface)
		{
			if (!xface.IsAssignableFrom(theClass))
			{
				throw new System.SystemException(theClass + " not " + xface.FullName);
			}
			SetProperty(propertyName, theClass.FullName);
		}

		private System.Collections.Specialized.NameValueCollection Props
		{
			get
			{
				lock (this)
				{
					if (m_collProperties == null)
					{
						System.Collections.Specialized.NameValueCollection defaults = new System.Collections.Specialized.NameValueCollection();
						System.Collections.Specialized.NameValueCollection newProps = null;new System.Collections.Specialized.NameValueCollection(defaults);
						System.Collections.IEnumerator i = m_collResourceNames.GetEnumerator();
						Int32 idx = 0;
						while (i.MoveNext())
						{
							if (idx == 0)
							{
								// load defaults
								LoadResource(defaults, i.Current as String, false);
								newProps = new System.Collections.Specialized.NameValueCollection(defaults);
							}
							else
							{
								if (idx == m_collResourceNames.Count - 1)
								{
									// load site
									LoadResource(newProps, i.Current as String, true);
								}
								else
								{
									// load intermediate
									LoadResource(newProps, i.Current as String, false);
								}
							}
							idx++;
						}
						m_collProperties = newProps;
					}
					return m_collProperties;
				}
			}
		}

		private void LoadResource(System.Collections.Specialized.NameValueCollection properties, String name, bool quietFail)
		{
			try
			{
				String strFilePath = name;
				if (String.Empty != m_strRootConfPath)
				{
					strFilePath = System.IO.Path.Combine(m_strRootConfPath, name);
				}
				
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
				doc.Load(strFilePath);

				if (doc == null)
				{
					if (quietFail)
					{
						return ;
					}
					throw new System.SystemException(name + " not found");
				}
				
				System.Xml.XmlElement root = (System.Xml.XmlElement) doc.DocumentElement;
				if (!"htmlparser-conf".Equals(root.Name))
				{
					System.Diagnostics.Trace.WriteLine("bad conf file: top-level element not <htmlparser-conf>");
				}
				System.Xml.XmlNodeList props = root.ChildNodes;
				for (int i = 0; i < props.Count; i++)
				{
					System.Xml.XmlNode propNode = props.Item(i);
					if (!(propNode is System.Xml.XmlElement))
					{
						continue;
					}
					System.Xml.XmlElement prop = (System.Xml.XmlElement) propNode;
					if (!"property".Equals(prop.Name))
					{
						System.Diagnostics.Trace.WriteLine("bad conf file: element not <property>");
					}

					System.Xml.XmlNodeList fields = prop.ChildNodes;
					System.String attr = null;
					System.String value_Renamed = null;
					for (int j = 0; j < fields.Count; j++)
					{
						System.Xml.XmlNode fieldNode = fields.Item(j);
						if (!(fieldNode is System.Xml.XmlElement))
						{
							continue;
						}
						System.Xml.XmlElement field = (System.Xml.XmlElement) fieldNode;
						if ("name".Equals(field.Name))
						{
							attr = ((System.Xml.XmlText) field.FirstChild).Data;
						}
						if ("value".Equals(field.Name) && field.HasChildNodes)
						{
							value_Renamed = ((System.Xml.XmlText) field.FirstChild).Data;
						}
					}
					if (attr != null && value_Renamed != null)
					{
						properties[attr] = value_Renamed;
					}
				}
			}
			catch (System.Exception e)
			{
				throw new ParserException("Error parsing configuration file or file is missing: ", e);
			}
		}
	}
}
