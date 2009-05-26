// ***************************************************************
//  AbstractNode   version:  1.0   Date: 12/17/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Visitors;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Lex;

namespace Winista.Text.HtmlParser.Nodes
{
	/// <summary> The concrete base class for all types of nodes (tags, text remarks).
	/// This class provides basic functionality to hold the {@link Page}, the
	/// starting and ending position in the page, the parent and the list of
	/// {@link NodeList children}.
	/// </summary>
	[Serializable]
	public abstract class AbstractNode : INode, System.ICloneable
	{
		/// <summary> The page this node came from.</summary>
		protected internal Page mPage;
		
		/// <summary> The beginning position of the tag in the line</summary>
		protected internal int nodeBegin;
		
		/// <summary> The ending position of the tag in the line</summary>
		protected internal int nodeEnd;
		
		/// <summary> The parent of this node.</summary>
		protected internal INode parent;
		
		/// <summary> The children of this node.</summary>
		protected internal NodeList children;

		/// <summary> Create an abstract node with the page positions given.
		/// Remember the page and start & end cursor positions.
		/// </summary>
		/// <param name="page">The page this tag was read from.
		/// </param>
		/// <param name="start">The starting offset of this node within the page.
		/// </param>
		/// <param name="end">The ending offset of this node within the page.
		/// </param>
		public AbstractNode(Page page, int start, int end)
		{
			mPage = page;
			nodeBegin = start;
			nodeEnd = end;
			parent = null;
			children = null;
		}

		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the page this node came from.</summary>
		/// <returns> The page that supplied this node.
		/// </returns>
		/// <summary> Set the page this node came from.</summary>
		/// <param name="page">The page that supplied this node.
		/// </param>
		virtual public Page Page
		{
			get
			{
				return (mPage);
			}
			
			set
			{
				mPage = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Gets the starting position of the node.</summary>
		/// <returns> The start position.
		/// </returns>
		/// <summary> Sets the starting position of the node.</summary>
		/// <param name="position">The new start position.
		/// </param>
		virtual public int StartPosition
		{
			get
			{
				return (nodeBegin);
			}
			
			set
			{
				nodeBegin = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Gets the ending position of the node.</summary>
		/// <returns> The end position.
		/// </returns>
		/// <summary> Sets the ending position of the node.</summary>
		/// <param name="position">The new end position.
		/// </param>
		virtual public int EndPosition
		{
			get
			{
				return (nodeEnd);
			}
			
			set
			{
				nodeEnd = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the parent of this node.
		/// This will always return null when parsing without scanners,
		/// i.e. if semantic parsing was not performed.
		/// The object returned from this method can be safely cast to a <code>CompositeTag</code>.
		/// </summary>
		/// <returns> The parent of this node, if it's been set, <code>null</code> otherwise.
		/// </returns>
		/// <summary> Sets the parent of this node.</summary>
		/// <param name="node">The node that contains this node. Must be a <code>CompositeTag</code>.
		/// </param>
		virtual public INode Parent
		{
			get
			{
				return (parent);
			}
			
			set
			{
				parent = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the children of this node.</summary>
		/// <returns> The list of children contained by this node, if it's been set, <code>null</code> otherwise.
		/// </returns>
		/// <summary> Set the children of this node.</summary>
		/// <param name="children">The new list of children this node contains.
		/// </param>
		virtual public NodeList Children
		{
			get
			{
				return (children);
			}
			
			set
			{
				this.children = value;
			}
			
		}

		/// <summary> Get the first child of this node.</summary>
		/// <returns> The first child in the list of children contained by this node,
		/// <code>null</code> otherwise.
		/// </returns>
		virtual public INode FirstChild
		{
			get
			{
				if (children == null)
					return null;
				if (children.Size() == 0)
					return null;
				return children.ElementAt(0);
			}
			
		}
		/// <summary> Get the last child of this node.</summary>
		/// <returns> The last child in the list of children contained by this node,
		/// <code>null</code> otherwise.
		/// </returns>
		virtual public INode LastChild
		{
			get
			{
				if (children == null)
					return null;
				int numChildren = children.Size();
				if (numChildren == 0)
					return null;
				return children.ElementAt(numChildren - 1);
			}
			
		}
		/// <summary> Get the previous sibling to this node.</summary>
		/// <returns> The previous sibling to this node if one exists,
		/// <code>null</code> otherwise.
		/// </returns>
		virtual public INode PreviousSibling
		{
			get
			{
				INode parentNode = this.Parent;
				if (parentNode == null)
					//root node
					return null;
				NodeList siblings = parentNode.Children;
				if (siblings == null)
					//this should actually be an error
					return null;
				int numSiblings = siblings.Size();
				if (numSiblings < 2)
					//need at least one other node to have a chance of having any siblings
					return null;
				int positionInParent = - 1;
				for (int i = 0; i < numSiblings; i++)
				{
					if (siblings.ElementAt(i) == this)
					{
						positionInParent = i;
						break;
					}
				}
				if (positionInParent < 1)
					//no previous siblings
					return null;
				return siblings.ElementAt(positionInParent - 1);
			}
			
		}
		/// <summary> Get the next sibling to this node.</summary>
		/// <returns> The next sibling to this node if one exists,
		/// <code>null</code> otherwise.
		/// </returns>
		virtual public INode NextSibling
		{
			get
			{
				INode parentNode = this.Parent;
				if (parentNode == null)
					//root node
					return null;
				NodeList siblings = parentNode.Children;
				if (siblings == null)
					//this should actually be an error
					return null;
				int numSiblings = siblings.Size();
				if (numSiblings < 2)
					//need at least one other node to have a chance of having any siblings
					return null;
				int positionInParent = - 1;
				for (int i = 0; i < numSiblings; i++)
				{
					if (siblings.ElementAt(i) == this)
					{
						positionInParent = i;
						break;
					}
				}
				if (positionInParent == - 1)
					//this should actually be an error
					return null;
				if (positionInParent == (numSiblings - 1))
					//no next sibling
					return null;
				return siblings.ElementAt(positionInParent + 1);
			}
			
		}

		/// <summary> Clone this object.
		/// Exposes java.lang.Object clone as a public method.
		/// </summary>
		/// <returns> A clone of this object.
		/// </returns>
		/// <exception cref="CloneNotSupportedException">This shouldn't be thrown since
		/// the {@link Node} interface extends Cloneable.
		/// </exception>
		public virtual System.Object Clone()
		{
			return (base.MemberwiseClone());
		}
		
		/// <summary> Returns a string representation of the node.
		/// It allows a simple string transformation
		/// of a web page, regardless of node type.<br>
		/// Typical application code (for extracting only the text from a web page)
		/// would then be simplified to:<br>
		/// <pre>
		/// Node node;
		/// for (Enumeration e = parser.elements (); e.hasMoreElements (); )
		/// {
		/// node = (Node)e.nextElement();
		/// System.out.println (node.toPlainTextString ());
		/// // or do whatever processing you wish with the plain text string
		/// }
		/// </pre>
		/// </summary>
		/// <returns> The 'browser' content of this node.
		/// </returns>
		public abstract System.String ToPlainTextString();
		
		/// <summary> Return the HTML that generated this node.
		/// This method will make it easier when using html parser to reproduce html
		/// pages (with or without modifications).
		/// Applications reproducing html can use this method on nodes which are to
		/// be used or transferred as they were recieved, with the original html.
		/// </summary>
		/// <returns> The HTML code for this node.
		/// </returns>
		public abstract System.String ToHtml();
		
		/// <summary> Return a string representation of the node.
		/// Subclasses must define this method, and this is typically to be used in the manner<br>
		/// <pre>System.out.println(node)</pre>
		/// </summary>
		/// <returns> A textual representation of the node suitable for debugging
		/// </returns>
		public override abstract System.String ToString();
		
		/// <summary> Collect this node and its child nodes (if-applicable) into the collectionList parameter, provided the node
		/// satisfies the filtering criteria.<P>
		/// 
		/// This mechanism allows powerful filtering code to be written very easily,
		/// without bothering about collection of embedded tags separately.
		/// e.g. when we try to get all the links on a page, it is not possible to
		/// get it at the top-level, as many tags (like form tags), can contain
		/// links embedded in them. We could get the links out by checking if the
		/// current node is a <see cref="CompositeTag"></see>, and going through its children.
		/// So this method provides a convenient way to do this.<P>
		/// 
		/// Using collectInto(), programs get a lot shorter. Now, the code to
		/// extract all links from a page would look like:
		/// <pre>
		/// NodeList collectionList = new NodeList();
		/// NodeFilter filter = new TagNameFilter ("A");
		/// for (NodeIterator e = parser.elements(); e.hasMoreNodes();)
		/// e.nextNode().collectInto(collectionList, filter);
		/// </pre>
		/// Thus, collectionList will hold all the link nodes, irrespective of how
		/// deep the links are embedded.<P>
		/// 
		/// Another way to accomplish the same objective is:
		/// <pre>
		/// NodeList collectionList = new NodeList();
		/// NodeFilter filter = new TagClassFilter (LinkTag.class);
		/// for (NodeIterator e = parser.elements(); e.hasMoreNodes();)
		/// e.nextNode().collectInto(collectionList, filter);
		/// </pre>
		/// This is slightly less specific because the LinkTag class may be
		/// registered for more than one node name, e.g. &lt;LINK&gt; tags too.
		/// </summary>
		/// <param name="list">The node list to collect acceptable nodes into.
		/// </param>
		/// <param name="filter">The filter to determine which nodes are retained.
		/// </param>
		public virtual void CollectInto(NodeList list, INodeFilter filter)
		{
			if (filter.Accept(this))
			{
				list.Add(this);
			}
		}
		
		/// <summary> Visit this node.</summary>
		/// <param name="visitor">The visitor that is visiting this node.
		/// </param>
		public abstract void  Accept(NodeVisitor visitor);
		
		/// <summary> Returns the text of the node.</summary>
		/// <returns> The text of this node. The default is <code>null</code>.
		/// </returns>
		public virtual System.String GetText()
		{
			return null;
		}
		
		/// <summary> Sets the string contents of the node.</summary>
		/// <param name="text">The new text for the node.
		/// </param>
		public virtual void SetText(System.String text)
		{
		}
		
		/// <summary> Perform the meaning of this tag.
		/// The default action is to do nothing.
		/// </summary>
		/// <exception cref="ParserException"><em>Not used.</em> Provides for subclasses
		/// that may want to indicate an exceptional condition.
		/// </exception>
		public virtual void  DoSemanticAction()
		{
		}

	}
}
