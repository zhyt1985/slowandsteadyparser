// ***************************************************************
//  CompositeTagScanner   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Scanners
{
	/// <summary> The main scanning logic for nested tags.
	/// When asked to scan, this class gathers nodes into a heirarchy of tags.
	/// </summary>
	[Serializable]
	public class CompositeTagScanner : TagScanner
	{
		/// <summary> Determine whether to use JVM or NodeList stack.
		/// This can be set to true to get the original behaviour of
		/// recursion into composite tags on the JVM stack.
		/// This may lead to StackOverFlowException problems in some cases
		/// i.e. Windows.
		/// </summary>
		private const bool mUseJVMStack = false;
		
		/// <summary> Determine whether unexpected end tags should cause stack roll-up.
		/// This can be set to true to get the original behaviour of gathering
		/// end tags into whatever tag is open.
		/// This can be expensive, but should only be needed in the presence of
		/// bad HTML.
		/// </summary>
		private const bool mLeaveEnds = false;

		public CompositeTagScanner()
		{
		}

		/// <summary> Collect the children.
		/// <p>An initial test is performed for an empty XML tag, in which case
		/// the start tag and end tag of the returned tag are the same and it has
		/// no children.<p>
		/// If it's not an empty XML tag, the lexer is repeatedly asked for
		/// subsequent nodes until an end tag is found or a node is encountered
		/// that matches the tag ender set or end tag ender set.
		/// In the latter case, a virtual end tag is created.
		/// Each node found that is not the end tag is added to
		/// the list of children. The end tag is special and not a child.<p>
		/// Nodes that also have a CompositeTagScanner as their scanner are
		/// recursed into, which provides the nested structure of an HTML page.
		/// This method operates in two possible modes, depending on a private boolean.
		/// It can recurse on the JVM stack, which has caused some overflow problems
		/// in the past, or it can use the supplied stack argument to nest scanning
		/// of child tags within itself. The former is left as an option in the code,
		/// mostly to help subsequent modifiers visualize what the internal nesting
		/// is doing.
		/// </summary>
		/// <param name="tag">The tag this scanner is responsible for.
		/// </param>
		/// <param name="lexer">The source of subsequent nodes.
		/// </param>
		/// <param name="stack">The parse stack. May contain pending tags that enclose
		/// this tag.
		/// </param>
		/// <returns> The resultant tag (may be unchanged).
		/// </returns>
		public override ITag Scan(ITag tag, Lexer lexer, NodeList stack)
		{
			INode node;
			ITag next;
			System.String name;
			IScanner scanner;
			ITag ret;
			
			ret = tag;
			
			if (ret.EmptyXmlTag)
			{
				ret.SetEndTag(ret);
			}
			else
				do 
				{
					node = lexer.NextNode(false);
					if (null != node)
					{
						if (node is ITag)
						{
							next = (ITag) node;
							name = next.TagName;
							// check for normal end tag
							if (next.IsEndTag() && name.Equals(ret.TagName))
							{
								ret.SetEndTag(next);
								node = null;
							}
							else if (IsTagToBeEndedFor(ret, next))
								// check DTD
							{
								// backup one node. insert a virtual end tag later
								lexer.Position = next.StartPosition;
								node = null;
							}
							else if (!next.IsEndTag())
							{
								// now recurse if there is a scanner for this type of tag
								scanner = next.ThisScanner;
								if (null != scanner)
								{
									if (mUseJVMStack)
									{
										// JVM stack recursion
										node = scanner.Scan(next, lexer, stack);
										AddChild(ret, node);
									}
									else
									{
										// fake recursion:
										if (scanner == this)
										{
											if (next.EmptyXmlTag)
											{
												next.SetEndTag(next);
												FinishTag(next, lexer);
												AddChild(ret, next);
											}
											else
											{
												stack.Add(ret);
												ret = next;
											}
										}
										else
										{
											// normal recursion if switching scanners
											node = scanner.Scan(next, lexer, stack);
											AddChild(ret, node);
										}
									}
								}
								else
									AddChild(ret, next);
							}
							else
							{
								if (!mUseJVMStack && !mLeaveEnds)
								{
									// Since all non-end tags are consumed by the
									// previous clause, we're here because we have an
									// end tag with no opening tag... this could be bad.
									// There are two cases...
									// 1) The tag hasn't been registered, in which case
									// we just add it as a simple child, like it's
									// opening tag
									// 2) There may be an opening tag further up the
									// parse stack that needs closing.
									// So, we ask the factory for a node like this one
									// (since end tags never have scanners) and see
									// if it's scanner is a composite tag scanner.
									// If it is we walk up the parse stack looking for
									// something that needs this end tag to finish it.
									// If there is something, we close off all the tags
									// walked over and continue on as if nothing
									// happened.
									System.Collections.ArrayList attributes = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
									attributes.Add(new TagAttribute(name, null));
									ITag opener = lexer.NodeFactory.CreateTagNode(lexer.Page, next.StartPosition, next.EndPosition, attributes);
									
									scanner = opener.ThisScanner;
									if ((null != scanner) && (scanner == this))
									{
										// uh-oh
										int index = - 1;
										for (int i = stack.Size() - 1; (- 1 == index) && (i >= 0); i--)
										{
											// short circuit here... assume everything on the stack has this as it's scanner
											// we'll need to stop if either of those conditions isn't met
											ITag boffo = (ITag) stack.ElementAt(i);
											if (name.Equals(boffo.TagName))
												index = i;
											else if (IsTagToBeEndedFor(boffo, next))
												// check DTD
												index = i;
										}
										if (- 1 != index)
										{
											// finish off the current one first
											FinishTag(ret, lexer);
											AddChild((ITag) stack.ElementAt(stack.Size() - 1), ret);
											for (int i = stack.Size() - 1; i > index; i--)
											{
												ITag fred = (ITag) stack.Remove(i);
												FinishTag(fred, lexer);
												AddChild((ITag) stack.ElementAt(i - 1), fred);
											}
											ret = (ITag) stack.Remove(index);
											node = null;
										}
										else
											AddChild(ret, next); // default behaviour
									}
									else
										AddChild(ret, next); // default behaviour
								}
								else
									AddChild(ret, next);
							}
						}
						else
						{
							AddChild(ret, node);
							node.DoSemanticAction();
						}
					}
					
					if (!mUseJVMStack)
					{
						// handle coming out of fake recursion
						if (null == node)
						{
							int depth = stack.Size();
							if (0 != depth)
							{
								node = stack.ElementAt(depth - 1);
								if (node is ITag)
								{
									ITag precursor = (ITag) node;
									scanner = precursor.ThisScanner;
									if (scanner == this)
									{
										stack.Remove(depth - 1);
										FinishTag(ret, lexer);
										AddChild(precursor, ret);
										ret = precursor;
									}
									else
										node = null; // normal recursion
								}
								else
									node = null; // normal recursion
							}
						}
					}
				}
				while (null != node);
			
			FinishTag(ret, lexer);
			
			return (ret);
		}
		
		/// <summary> Add a child to the given tag.</summary>
		/// <param name="parent">The parent tag.
		/// </param>
		/// <param name="child">The child node.
		/// </param>
		protected internal virtual void AddChild(ITag parent, INode child)
		{
			if (null == parent.Children)
			{
				parent.Children = new NodeList();
			}
			child.Parent = parent;
			parent.Children.Add(child);
		}
		
		/// <summary> Finish off a tag.
		/// Perhap add a virtual end tag.
		/// Set the end tag parent as this tag.
		/// Perform the semantic acton.
		/// </summary>
		/// <param name="tag">The tag to finish off.
		/// </param>
		/// <param name="lexer">A lexer positioned at the end of the tag.
		/// </param>
		protected internal virtual void FinishTag(ITag tag, Lexer lexer)
		{
			if (null == tag.GetEndTag())
			{
				tag.SetEndTag(CreateVirtualEndTag(tag, lexer, lexer.Page, lexer.Cursor.Position));
			}
			tag.GetEndTag().Parent = tag;
			tag.DoSemanticAction();
		}
		
		/// <summary> Creates an end tag with the same name as the given tag.</summary>
		/// <param name="tag">The tag to end.
		/// </param>
		/// <param name="lexer">The object containg the node factory.
		/// </param>
		/// <param name="page">The page the tag is on (virtually).
		/// </param>
		/// <param name="position">The offset into the page at which the tag is to
		/// be anchored.
		/// </param>
		/// <returns> An end tag with the name '"/" + tag.getTagName()' and a start
		/// and end position at the given position. The fact these positions are
		/// equal may be used to distinguish it as a virtual tag later on.
		/// </returns>
		protected internal virtual ITag CreateVirtualEndTag(ITag tag, Lexer lexer, Page page, int position)
		{
			ITag ret;
			System.String name;
			System.Collections.ArrayList attributes;
			
			name = "/" + tag.RawTagName;
			attributes = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			attributes.Add(new TagAttribute(name, (System.String) null));
			ret = lexer.NodeFactory.CreateTagNode(page, position, position, attributes);
			
			return (ret);
		}
		
		/// <summary> Determine if the current tag should be terminated by the given tag.
		/// Examines the 'enders' or 'end tag enders' lists of the current tag
		/// for a match with the given tag. Which list is chosen depends on whether
		/// tag is an end tag ('end tag enders') or not ('enders').
		/// </summary>
		/// <param name="current">The tag that might need to be ended.
		/// </param>
		/// <param name="tag">The candidate tag that might end the current one.
		/// </param>
		/// <returns> <code>true</code> if the name of the given tag is a member of
		/// the appropriate list.
		/// </returns>
		public bool IsTagToBeEndedFor(ITag current, ITag tag)
		{
			System.String name;
			System.String[] ends;
			bool ret;
			
			ret = false;
			
			name = tag.TagName;
			if (tag.IsEndTag())
				ends = current.EndTagEnders;
			else
				ends = current.Enders;
			for (int i = 0; i < ends.Length; i++)
				if (name.ToUpper().Equals(ends[i].ToUpper()))
				{
					ret = true;
					break;
				}
			
			return (ret);
		}
	}
}
