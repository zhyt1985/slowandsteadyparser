// ***************************************************************
//  ParseUtils   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;
using Winista.Text.HtmlParser.Filters;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary>
	/// Summary description for ParseUtils.
	/// </summary>
	public class ParserUtils
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="occur"></param>
		/// <returns></returns>
		public static System.String RemoveChars(System.String s, char occur)
		{
			System.Text.StringBuilder newString = new System.Text.StringBuilder();
			char ch;
			for (int i = 0; i < s.Length; i++)
			{
				ch = s[i];
				if (ch != occur)
					newString.Append(ch);
			}
			return newString.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static System.String RemoveEscapeCharacters(System.String inputString)
		{
			inputString = ParserUtils.RemoveChars(inputString, '\r');
			inputString = ParserUtils.RemoveChars(inputString, '\n');
			inputString = ParserUtils.RemoveChars(inputString, '\t');
			return inputString;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static System.String RemoveTrailingBlanks(System.String text)
		{
			char ch = ' ';
			while (ch == ' ')
			{
				ch = text[text.Length - 1];
				if (ch == ' ')
					text = text.Substring(0, (text.Length - 1) - (0));
			}
			return text;
		}

		/// <summary> Search given node and pick up any objects of given type.</summary>
		/// <param name="node">The node to search.
		/// </param>
		/// <param name="type">The class to search for.
		/// </param>
		/// <returns> A node array with the matching nodes.
		/// </returns>
		public static INode[] FindTypeInNode(INode node, System.Type type)
		{
			INodeFilter filter;
			NodeList ret;
			
			ret = new NodeList();
			filter = new NodeClassFilter(type);
			node.CollectInto(ret, filter);
			
			return (ret.ToNodeArray());
		}
	}
}
