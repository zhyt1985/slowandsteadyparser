// ***************************************************************
//  AppletTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Nodes;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> AppletTag represents an &lt;Applet&gt; tag.
	/// It extends a basic tag by providing accessors to the class, codebase,
	/// archive and parameters.
	/// </summary>
	[Serializable]
	public class AppletTag:CompositeTag
	{
		#region Class Members
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"APPLET"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"BODY", "HTML"};
		#endregion

		/// <summary>
		/// Creates new instance of <see cref="AppletTag"></see>
		/// </summary>
		public AppletTag()
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

		/// <summary> Get the class name of the applet.</summary>
		/// <returns> The value of the <code>CODE</code> attribute.
		/// </returns>
		/// <summary> Set the <code>CODE<code> attribute.</summary>
		/// <param name="newAppletClass">The new applet class.
		/// </param>
		virtual public System.String AppletClass
		{
			get
			{
				return (GetAttribute("CODE"));
			}
			
			set
			{
				SetAttribute("CODE", value);
			}
			
		}

		/// <summary> Get the applet parameters.</summary>
		/// <returns> The list of parameter values (keys and values are String objects).
		/// </returns>
		/// <summary> Set the enclosed <code>PARM<code> children.</summary>
		/// <param name="newAppletParams">The new parameters.
		/// </param>
		virtual public System.Collections.Hashtable AppletParams
		{
			get
			{
				return (CreateAppletParamsTable());
			}
			
			set
			{
				NodeList kids;
				INode node;
				ITag tag;
				System.String paramName;
				System.String paramValue;
				System.Collections.ArrayList attributes;
				IText string_Renamed;
				
				kids = Children;
				if (null == kids)
					kids = new NodeList();
					// erase appletParams from kids
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
									if (node is IText)
									{
										string_Renamed = (IText) node;
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
				
				// add newAppletParams to kids
				for (System.Collections.IEnumerator e = value.Keys.GetEnumerator(); e.MoveNext(); )
				{
					attributes = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10)); // should the tag copy the attributes?
					paramName = ((System.String) e.Current);
					paramValue = ((System.String) value[paramName]);
					attributes.Add(new TagAttribute("PARAM", null));
					attributes.Add(new TagAttribute(" "));
					attributes.Add(new TagAttribute("VALUE", paramValue, '"'));
					attributes.Add(new TagAttribute(" "));
					attributes.Add(new TagAttribute("NAME", paramName, '"'));
					tag = new TagNode(null, 0, 0, attributes);
					kids.Add(tag);
				}
				
				//set kids as new children
				Children = kids;
			}
			
		}

		/// <summary> Get the jar file of the applet.</summary>
		/// <returns> The value of the <code>ARCHIVE</code> attribute, or <code>null</code> if it wasn't specified.
		/// </returns>
		/// <summary> Set the <code>ARCHIVE<code> attribute.</summary>
		/// <param name="newArchive">The new archive file.
		/// </param>
		virtual public System.String Archive
		{
			get
			{
				return (GetAttribute("ARCHIVE"));
			}
			
			set
			{
				SetAttribute("ARCHIVE", value);
			}
			
		}

		/// <summary> Get the code base of the applet.</summary>
		/// <returns> The value of the <code>CODEBASE</code> attribute, or <code>null</code> if it wasn't specified.
		/// </returns>
		/// <summary> Set the <code>CODEBASE<code> attribute.</summary>
		/// <param name="newCodeBase">The new applet code base.
		/// </param>
		virtual public System.String CodeBase
		{
			get
			{
				return (GetAttribute("CODEBASE"));
			}
			
			set
			{
				SetAttribute("CODEBASE", value);
			}
			
		}
		/// <summary> Get an enumeration over the (String) parameter names.</summary>
		/// <returns> An enumeration of the <code>PARAM<code> tag <code>NAME<code> attributes.
		/// </returns>
		virtual public System.Collections.IEnumerator ParameterNames
		{
			get
			{
				return (AppletParams.Keys.GetEnumerator());
			}
		}

		/// <summary> Extract the applet <code>PARAM</code> tags from the child list.</summary>
		/// <returns> The list of applet parameters (keys and values are String objects).
		/// </returns>
		public virtual System.Collections.Hashtable CreateAppletParamsTable()
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
					node = Children.ElementAt(i);
					if (node is ITag)
					{
						tag = (ITag) node;
						if (tag.TagName.Equals("PARAM"))
						{
							paramName = tag.GetAttribute("NAME");
							if (null != paramName && 0 != paramName.Length)
							{
								paramValue = tag.GetAttribute("VALUE");
								ret[paramName] = paramValue;
							}
						}
					}
				}
			
			return (ret);
		}
		
		/// <summary> Get the <code>PARAM<code> tag with the given name.
		/// <em>NOTE: This was called (erroneously) getAttribute() in previous versions.</em>
		/// </summary>
		/// <param name="key">The applet parameter name to get.
		/// </param>
		/// <returns> The value of the parameter or <code>null</code> if there is no parameter of that name.
		/// </returns>
		public virtual System.String GetParameter(System.String key)
		{
			return ((System.String) (AppletParams[key]));
		}
		
		/// <summary> Output a string representing this applet tag.</summary>
		/// <returns> A string showing the contents of the applet tag.
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
			ret.Append("Applet Tag\n");
			ret.Append("**********\n");
			ret.Append("Class Name = ");
			ret.Append(AppletClass);
			ret.Append("\n");
			ret.Append("Archive = ");
			ret.Append(Archive);
			ret.Append("\n");
			ret.Append("Codebase = ");
			ret.Append(CodeBase);
			ret.Append("\n");
			parameters = AppletParams;
			params_Renamed = parameters.Keys.GetEnumerator();
			if (null == params_Renamed)
				ret.Append("No Params found.\n");
			else
			{

				for (int cnt = 0; params_Renamed.MoveNext(); cnt++)
				{
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
			ret.Append("End of Applet Tag\n");
			ret.Append("*****************\n");
			
			return (ret.ToString());
		}
	}
}
