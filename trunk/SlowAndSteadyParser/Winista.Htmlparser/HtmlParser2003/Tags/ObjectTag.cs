// ***************************************************************
//  ObjectTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> ObjectTag represents an &lt;Object&gt; tag.
	/// It extends a basic tag by providing accessors to the
	/// type, codetype, codebase, classid, data, height, width, standby attributes and parameters.
	/// </summary>
	[Serializable]
	public class ObjectTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"OBJECT"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"BODY", "HTML"};

		public ObjectTag()
		{
		}

		/// <summary> Return the set of names handled by this tag.</summary>
		/// <returns> The names to be matched that create tags of this type.
		/// </returns>
		override public System.String[] Ids
		{
			get
			{
				return (mIds);
			}
			
		}
		/// <summary> Return the set of end tag names that cause this tag to finish.</summary>
		/// <returns> The names of following end tags that stop further scanning.
		/// </returns>
		override public System.String[] EndTagEnders
		{
			get
			{
				return (mEndTagEnders);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the classid of the object.</summary>
		/// <returns> The value of the <code>CLASSID</code> attribute.
		/// </returns>
		/// <summary> Set the <code>CLASSID<code> attribute.</summary>
		/// <param name="newClassId">The new classid.
		/// </param>
		virtual public System.String ObjectClassId
		{
			get
			{
				return GetAttribute("CLASSID");
			}
			
			set
			{
				SetAttribute("CLASSID", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the codebase of the object.</summary>
		/// <returns> The value of the <code>CODEBASE</code> attribute.
		/// </returns>
		/// <summary> Set the <code>CODEBASE<code> attribute.</summary>
		/// <param name="newCodeBase">The new codebase.
		/// </param>
		virtual public System.String ObjectCodeBase
		{
			get
			{
				return GetAttribute("CODEBASE");
			}
			
			set
			{
				SetAttribute("CODEBASE", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the codetype of the object.</summary>
		/// <returns> The value of the <code>CODETYPE</code> attribute.
		/// </returns>
		/// <summary> Set the <code>CODETYPE<code> attribute.</summary>
		/// <param name="newCodeType">The new codetype.
		/// </param>
		virtual public System.String ObjectCodeType
		{
			get
			{
				return GetAttribute("CODETYPE");
			}
			
			set
			{
				SetAttribute("CODETYPE", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the data of the object.</summary>
		/// <returns> The value of the <code>DATA</code> attribute.
		/// </returns>
		/// <summary> Set the <code>DATA<code> attribute.</summary>
		/// <param name="newData">The new data.
		/// </param>
		virtual public System.String ObjectData
		{
			get
			{
				return GetAttribute("DATA");
			}
			
			set
			{
				SetAttribute("DATA", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the height of the object.</summary>
		/// <returns> The value of the <code>HEIGHT</code> attribute.
		/// </returns>
		/// <summary> Set the <code>HEIGHT<code> attribute.</summary>
		/// <param name="newHeight">The new height.
		/// </param>
		virtual public System.String ObjectHeight
		{
			get
			{
				return GetAttribute("HEIGHT");
			}
			
			set
			{
				SetAttribute("HEIGHT", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the standby of the object.</summary>
		/// <returns> The value of the <code>STANDBY</code> attribute.
		/// </returns>
		/// <summary> Set the <code>STANDBY<code> attribute.</summary>
		/// <param name="newStandby">The new standby.
		/// </param>
		virtual public System.String ObjectStandby
		{
			get
			{
				return GetAttribute("STANDBY");
			}
			
			set
			{
				SetAttribute("STANDBY", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the type of the object.</summary>
		/// <returns> The value of the <code>TYPE</code> attribute.
		/// </returns>
		/// <summary> Set the <code>TYPE<code> attribute.</summary>
		/// <param name="newType">The new type.
		/// </param>
		virtual public System.String ObjectType
		{
			get
			{
				return GetAttribute("TYPE");
			}
			
			set
			{
				SetAttribute("TYPE", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the width of the object.</summary>
		/// <returns> The value of the <code>WIDTH</code> attribute.
		/// </returns>
		/// <summary> Set the <code>WIDTH<code> attribute.</summary>
		/// <param name="newWidth">The new width.
		/// </param>
		virtual public System.String ObjectWidth
		{
			get
			{
				return GetAttribute("WIDTH");
			}
			
			set
			{
				SetAttribute("WIDTH", value);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the object parameters.</summary>
		/// <returns> The list of parameter values (keys and values are String objects).
		/// </returns>
		/// <summary> Set the enclosed <code>PARAM<code> children.</summary>
		/// <param name="newObjectParams">The new parameters.
		/// </param>
		virtual public System.Collections.Hashtable ObjectParams
		{
			get
			{
				return CreateObjectParamsTable();
			}
			
			set
			{
				NodeList kids;
				INode node;
				ITag tag;
				System.String paramName;
				System.String paramValue;
				System.Collections.ArrayList attributes;
				TextNode string_Renamed;
				
				kids = Children;
				if (null == kids)
					kids = new NodeList();
					// erase objectParams from kids
				else
					for (int i = 0; i < kids.Size(); )
					{
						node = kids.ElementAt(i);
						if (node is ITag)
							if (((ITag) node).TagName.Equals("PARAM"))
							{
								kids.Remove(i);
								// remove whitespace too
								if (i < kids.Size())
								{
									node = kids.ElementAt(i);
									if (node is TextNode)
									{
										string_Renamed = (TextNode) node;
										if (0 == string_Renamed.GetText().Trim().Length)
											kids.Remove(i);
									}
								}
							}
							else
								i++;
						else
							i++;
					}
				
				// add newObjectParams to kids
				//UPGRADE_TODO: Method 'java.util.Enumeration.hasMoreElements' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationhasMoreElements'"
				for (System.Collections.IEnumerator e = value.Keys.GetEnumerator(); e.MoveNext(); )
				{
					attributes = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10)); // should the tag copy the attributes?
					//UPGRADE_TODO: Method 'java.util.Enumeration.nextElement' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationnextElement'"
					paramName = ((System.String) e.Current);
					paramValue = ((System.String) value[paramName]);
					attributes.Add(new TagAttribute("PARAM", null));
					attributes.Add(new TagAttribute(" "));
					attributes.Add(new TagAttribute("VALUE", paramValue, '"'));
					attributes.Add(new TagAttribute(" "));
					attributes.Add(new TagAttribute("NAME", paramName.ToUpper(), '"'));
					tag = new TagNode(null, 0, 0, attributes);
					kids.Add(tag);
				}
				
				//set kids as new children
				Children = kids;
			}
			
		}
		/// <summary> Get an enumeration over the (String) parameter names.</summary>
		/// <returns> An enumeration of the <code>PARAM<code> tag <code>NAME<code> attributes.
		/// </returns>
		virtual public System.Collections.IEnumerator ParameterNames
		{
			get
			{
				return ObjectParams.Keys.GetEnumerator();
			}
			
		}

		/// <summary> Extract the object <code>PARAM</code> tags from the child list.</summary>
		/// <returns> The list of object parameters (keys and values are String objects).
		/// </returns>
		public virtual System.Collections.Hashtable CreateObjectParamsTable()
		{
			NodeList kids;
			INode node;
			ITag tag;
			System.String paramName;
			System.String paramValue;
			System.Collections.Hashtable ret;
			
			ret = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
			kids = Children;
			if (null != kids)
				for (int i = 0; i < kids.Size(); i++)
				{
					node = children.ElementAt(i);
					if (node is ITag)
					{
						tag = (ITag) node;
						if (tag.TagName.Equals("PARAM"))
						{
							paramName = tag.GetAttribute("NAME");
							if (null != paramName && 0 != paramName.Length)
							{
								paramValue = tag.GetAttribute("VALUE");
								ret[paramName.ToUpper()] = paramValue;
							}
						}
					}
				}
			
			return (ret);
		}
		
		/// <summary> Get the <code>PARAM<code> tag with the given name.</summary>
		/// <param name="key">The object parameter name to get.
		/// </param>
		/// <returns> The value of the parameter or <code>null</code> if there is no parameter of that name.
		/// </returns>
		public virtual System.String GetParameter(System.String key)
		{
			return ((System.String) (ObjectParams[key.ToUpper()]));
		}
		
		/// <summary> Output a string representing this object tag.</summary>
		/// <returns> A string showing the contents of the object tag.
		/// </returns>
		public override System.String ToString()
		{
			System.Collections.Hashtable parameters;
			System.Collections.IEnumerator params_Renamed;
			System.String paramName;
			System.String paramValue;
			bool found;
			INode node;
			System.Text.StringBuilder ret;
			
			ret = new System.Text.StringBuilder(500);
			ret.Append("Object Tag\n");
			ret.Append("**********\n");
			ret.Append("ClassId = ");
			ret.Append(ObjectClassId);
			ret.Append("\n");
			ret.Append("CodeBase = ");
			ret.Append(ObjectCodeBase);
			ret.Append("\n");
			ret.Append("CodeType = ");
			ret.Append(ObjectCodeType);
			ret.Append("\n");
			ret.Append("Data = ");
			ret.Append(ObjectData);
			ret.Append("\n");
			ret.Append("Height = ");
			ret.Append(ObjectHeight);
			ret.Append("\n");
			ret.Append("Standby = ");
			ret.Append(ObjectStandby);
			ret.Append("\n");
			ret.Append("Type = ");
			ret.Append(ObjectType);
			ret.Append("\n");
			ret.Append("Width = ");
			ret.Append(ObjectWidth);
			ret.Append("\n");
			parameters = ObjectParams;
			params_Renamed = parameters.Keys.GetEnumerator();
			if (null == params_Renamed)
				ret.Append("No Params found.\n");
			else
			{
				//UPGRADE_TODO: Method 'java.util.Enumeration.hasMoreElements' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationhasMoreElements'"
				for (int cnt = 0; params_Renamed.MoveNext(); cnt++)
				{
					//UPGRADE_TODO: Method 'java.util.Enumeration.nextElement' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationnextElement'"
					paramName = ((System.String) params_Renamed.Current);
					paramValue = ((System.String) parameters[paramName]);
					ret.Append(cnt);
					ret.Append(": Parameter name = ");
					ret.Append(paramName);
					ret.Append(", Parameter value = ");
					ret.Append(paramValue);
					ret.Append("\n");
				}
			}
			found = false;
			for (ISimpleNodeIterator e = GetChildren(); e.HasMoreNodes(); )
			{
				node = e.NextNode();
				if (node is ITag)
					if (((ITag) node).TagName.Equals("PARAM"))
						continue;
				if (!found)
					ret.Append("Miscellaneous items :\n");
				else
					ret.Append(" ");
				found = true;
				ret.Append(node.ToString());
			}
			if (found)
				ret.Append("\n");
			ret.Append("End of Object Tag\n");
			ret.Append("*****************\n");
			
			return (ret.ToString());
		}
	}
}
