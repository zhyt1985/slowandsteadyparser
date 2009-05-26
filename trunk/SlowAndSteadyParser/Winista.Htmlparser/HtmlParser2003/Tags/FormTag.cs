// ***************************************************************
//  FormTag   version:  1.0   date: 12/18/2005
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
	/// <summary> Represents a FORM tag.</summary>
	/// <author>  ili
	/// </author>
	[Serializable]
	public class FormTag:CompositeTag
	{
		/// <summary> POSt action name</summary>
		public const System.String POST = "POST";
		
		/// <summary> GET action name.</summary>
		public const System.String GET = "GET";
		
		/// <summary> This is the derived form location, based on action.</summary>
		protected internal System.String mFormLocation;
		
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"FORM"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"HTML", "BODY", "TABLE"};

		/// <summary>
		/// 
		/// </summary>
		public FormTag()
		{
			mFormLocation = null;
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
		/// <summary> Return the set of tag names that cause this tag to finish.</summary>
		/// <returns> The names of following tags that stop further scanning.
		/// </returns>
		override public System.String[] Enders
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
		/// <summary> Get the list of input fields.</summary>
		/// <returns> Input elements in the form.
		/// </returns>
		virtual public NodeList FormInputs
		{
			get
			{
				return (SearchFor(typeof(InputTag), true));
			}
			
		}
		/// <summary> Get the list of text areas.</summary>
		/// <returns> Textarea elements in the form.
		/// </returns>
		virtual public NodeList FormTextareas
		{
			get
			{
				return (SearchFor(typeof(TextareaTag), true));
			}
			
		}

		/// <summary> Get the value of the action attribute.</summary>
		/// <returns> The submit url of the form.
		/// </returns>
		/// <summary> Set the form location. Modification of this element will cause the HTML rendering
		/// to change as well (in a call to toHTML()).
		/// </summary>
		virtual public System.String FormLocation
		{
			get
			{
				if (null == mFormLocation)
					// ... is it true that without an ACTION the default is to send it back to the same page?
					mFormLocation = ExtractFormLocn();
				
				return (mFormLocation);
			}
			
			set
			{
				mFormLocation = value;
				SetAttribute("ACTION", value);
			}
			
		}
		/// <summary> Returns the method of the form, GET or POST.</summary>
		/// <returns> String The method of the form (GET if nothing is specified).
		/// </returns>
		virtual public System.String FormMethod
		{
			get
			{
				System.String ret;
				
				ret = GetAttribute("METHOD");
				if (null == ret)
					ret = GET;
				
				return (ret);
			}
			
		}
		/// <summary> Get the value of the name attribute.</summary>
		/// <returns> String The name of the form
		/// </returns>
		virtual public System.String FormName
		{
			get
			{
				return (GetAttribute("NAME"));
			}
			
		}

		/// <summary> Get the input tag in the form corresponding to the given name</summary>
		/// <param name="name">The name of the input tag to be retrieved
		/// </param>
		/// <returns> Tag The input tag corresponding to the name provided
		/// </returns>
		public virtual InputTag GetInputTag(System.String name)
		{
			InputTag inputTag;
			bool found;
			System.String inputTagName;
			
			inputTag = null;
			found = false;
			for (ISimpleNodeIterator e = FormInputs.Elements(); e.HasMoreNodes() && !found; )
			{
				inputTag = (InputTag) e.NextNode();
				inputTagName = inputTag.GetAttribute("NAME");
				if (inputTagName != null && inputTagName.ToUpper().Equals(name.ToUpper()))
					found = true;
			}
			if (found)
				return (inputTag);
			else
				return (null);
		}
		
		/// <summary> Find the textarea tag matching the given name</summary>
		/// <param name="name">Name of the textarea tag to be found within the form.
		/// </param>
		/// <returns> The <code>TEXTAREA</code> tag with the matching name.
		/// </returns>
		public virtual TextareaTag getTextAreaTag(System.String name)
		{
			TextareaTag textareaTag = null;
			bool found = false;
			for (ISimpleNodeIterator e = FormTextareas.Elements(); e.HasMoreNodes() && !found; )
			{
				textareaTag = (TextareaTag) e.NextNode();
				System.String textAreaName = textareaTag.GetAttribute("NAME");
				if (textAreaName != null && textAreaName.Equals(name))
					found = true;
			}
			if (found)
				return (textareaTag);
			else
				return (null);
		}
		
		/// <summary> Return a string representation of the contents of this <code>FORM</code> tag suitable for debugging.</summary>
		/// <returns> A textual representation of the form tag.
		/// </returns>
		public override System.String ToString()
		{
			return "FORM TAG : Form at " + FormLocation + "; begins at : " + StartPosition + "; ends at : " + EndPosition;
		}
		
		/// <summary> Extract the <code>ACTION</code> attribute as an absolute URL.</summary>
		/// <returns> The URL the form is to be submitted to.
		/// </returns>
		public virtual System.String ExtractFormLocn()
		{
			System.String ret;
			
			ret = GetAttribute("ACTION");
			if (null == ret)
				ret = "";
			else if (null != Page)
				ret = Page.GetAbsoluteURL(ret);
			
			return (ret);
		}
	}
}
