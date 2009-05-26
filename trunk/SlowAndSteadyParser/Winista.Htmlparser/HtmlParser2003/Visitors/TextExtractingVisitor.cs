// ***************************************************************
//  TextExtractingVisitor   version:  1.0   date: 12/25/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Visitors
{
	/// <summary> Extracts text from a web page.
	/// Usage:
	/// <code>
	/// Parser parser = new Parser(...);
	/// TextExtractingVisitor visitor = new TextExtractingVisitor();
	/// parser.VisitAllNodesWith(visitor);
	/// String textInPage = visitor.GetExtractedText();
	/// </code>
	/// </summary>
	public class TextExtractingVisitor : NodeVisitor
	{
		#region Class Member
		private System.Text.StringBuilder textAccumulator;
		private bool preTagBeingProcessed;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public TextExtractingVisitor()
		{
			textAccumulator = new System.Text.StringBuilder();
			preTagBeingProcessed = false;
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public System.String ExtractedText
		{
			get
			{
				return textAccumulator.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stringNode"></param>
		public override void VisitStringNode(IText stringNode)
		{
			System.String text = stringNode.GetText();
			if (!preTagBeingProcessed)
			{
				text = Translate.Decode(text);
				text = ReplaceNonBreakingSpaceWithOrdinarySpace(text);
			}
			textAccumulator.Append(text);
		}
		
		private System.String ReplaceNonBreakingSpaceWithOrdinarySpace(System.String text)
		{
			return text.Replace('\u00a0', ' ');
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void VisitTag(ITag tag)
		{
			if (IsPreTag(tag))
			{
				preTagBeingProcessed = true;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void VisitEndTag(ITag tag)
		{
			if (IsPreTag(tag))
			{
				preTagBeingProcessed = false;
			}
		}
		
		private bool IsPreTag(ITag tag)
		{
			return tag.TagName.Equals("PRE");
		}
	}
}
