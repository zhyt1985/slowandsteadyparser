// ***************************************************************
//  ScriptTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Scanners;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> A script tag.</summary>
	[Serializable]
	public class ScriptTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"SCRIPT"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"BODY", "HTML"};
		
		/// <summary> Script code if different from the page contents.</summary>
		protected internal System.String mCode;

		/// <summary>
		/// 
		/// </summary>
		public ScriptTag()
		{
			ThisScanner = new ScriptScanner();
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

		/// <summary> Get the <code>LANGUAGE</code> attribute, if any.</summary>
		/// <returns> The scripting language.
		/// </returns>
		/// <summary> Set the language of the script tag.</summary>
		virtual public System.String Language
		{
			get
			{
				return (GetAttribute("LANGUAGE"));
			}
			
			set
			{
				SetAttribute("LANGUAGE", value);
			}
			
		}

		/// <summary> Get the script code.
		/// Normally this is the contents of the children, but in the rare case that
		/// the script is encoded, this is the plaintext decrypted code.
		/// </summary>
		/// <returns> The plaintext or overridden code contents of the tag.
		/// </returns>
		/// <summary> Set the code contents.</summary>
		virtual public System.String ScriptCode
		{
			get
			{
				System.String ret;
				
				if (null != mCode)
					ret = mCode;
				else
					ret = ChildrenHTML;
				
				return (ret);
			}
			
			set
			{
				mCode = value;
			}
			
		}

		/// <summary> Get the <code>TYPE</code> attribute, if any.</summary>
		/// <returns> The script mime type.
		/// </returns>
		/// <summary> Set the mime type of the script tag.</summary>
		virtual public System.String Type
		{
			get
			{
				return (GetAttribute("TYPE"));
			}
			
			set
			{
				SetAttribute("TYPE", value);
			}
			
		}

		/// <summary> Places the script contents into the provided buffer.</summary>
		/// <param name="sb">The buffer to add the script to.
		/// </param>
		protected internal override void PutChildrenInto(System.Text.StringBuilder sb)
		{
			INode node;
			
			if (null != ScriptCode)
				sb.Append(ScriptCode);
			else
				for (ISimpleNodeIterator e = GetChildren(); e.HasMoreNodes(); )
				{
					node = e.NextNode();
					// eliminate virtual tags
					//            if (!(node.getStartPosition () == node.getEndPosition ()))
					sb.Append(node.ToHtml());
				}
		}
		
		/// <summary> Print the contents of the script tag suitable for debugging display.</summary>
		/// <returns> The script language or type and code as a string.
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Script Node : \n");
			if (Language != null || Type != null)
			{
				sb.Append("Properties -->\n");
				if (Language != null && Language.Length != 0)
					sb.Append("[Language : " + Language + "]\n");
				if (Type != null && Type.Length != 0)
					sb.Append("[Type : " + Type + "]\n");
			}
			sb.Append("\n");
			sb.Append("Code\n");
			sb.Append("****\n");
			sb.Append(ScriptCode + "\n");
			return sb.ToString();
		}
	}
}
