// ***************************************************************
//  PrototypicalNodeFactory   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Nodes;
using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser
{
	/// <summary> <p>A node factory based on the prototype pattern.
	/// This factory uses the prototype pattern to generate new nodes.
	/// These are cloned as needed to form new <see cref="IText"></see>, <see cref="IRemark"></see> and
	/// <see cref="ITag"></see> nodes.</p>
	/// <p>Text and remark nodes are generated from prototypes accessed
	/// via the <see cref="SetTextPrototype"></see> and
	/// <see cref="SetRemarkPrototype"></see> properties respectively.
	/// Tag nodes are generated as follows:
	/// <p>Prototype tags, in the form of undifferentiated tags, are held in a hash
	/// table. On a request for a tag, the attributes are examined for the name
	/// of the tag to be created. If a prototype of that name has been registered
	/// (exists in the hash table), it is cloned and the clone is given the
	/// characteristics ({@link Attribute Attributes}, start and end position)
	/// of the requested tag.</p>
	/// <p>In the case that no tag has been registered under that name,
	/// a generic tag is created from the prototype accessed via the
	/// {@link #setTagPrototype(Tag) tagPrototype} property.</p>
	/// <p>The hash table of registered tags can be automatically populated with
	/// all the know tags from the Tags package when
	/// the factory is constructed, or it can start out empty and be populated
	/// explicitly.</p>
	/// <p>Here is an example of how to override all text issued from
	/// TextNode.ToPlainTextString()
	/// Text.ToPlainTextString()},
	/// in this case decoding (converting character references),
	/// which illustrates the use of setting the text prototype:
	/// <pre>
	/// PrototypicalNodeFactory factory = new PrototypicalNodeFactory ();
	/// factory.setTextPrototype (
	/// // create a inner class that is a subclass of TextNode
	/// new TextNode () {
	/// public String toPlainTextString()
	/// {
	/// String original = super.toPlainTextString ();
	/// return (Translate.Decode (original));
	/// }
	/// });
	/// Parser parser = new Parser ();
	/// parser.setNodeFactory (factory);
	/// </pre></p>
	/// <p>Here is an example of using a custom link tag, in this case just
	/// printing the URL, which illustrates registering a tag:
	/// <pre>
	/// 
	/// class PrintingLinkTag extends LinkTag
	/// {
	/// public void doSemanticAction ()
	/// throws
	/// ParserException
	/// {
	/// System.out.println (getLink ());
	/// }
	/// }
	/// PrototypicalNodeFactory factory = new PrototypicalNodeFactory ();
	/// factory.registerTag (new PrintingLinkTag ());
	/// Parser parser = new Parser ();
	/// parser.setNodeFactory (factory);
	/// </pre></p>
	/// </summary>
	/// 
	[Serializable]
	public class PrototypicalNodeFactory : INodeFactory
	{
		#region Class Memebers
		/// <summary> The prototypical text node.</summary>
		protected internal IText mText;
		
		/// <summary> The prototypical remark node.</summary>
		protected internal IRemark mRemark;
		
		/// <summary> The prototypical tag node.</summary>
		protected internal ITag mTag;
		
		/// <summary> The list of tags to return.
		/// The list is keyed by tag name.
		/// </summary>
		protected internal System.Collections.IDictionary mBlastocyst;
		#endregion

		/// <summary> Create a new factory with all tags registered.
		/// Equivalent to
		/// {@link #PrototypicalNodeFactory() PrototypicalNodeFactory(false)}.
		/// </summary>
		public PrototypicalNodeFactory():this(false)
		{
		}

		/// <summary> Create a new factory.</summary>
		/// <param name="empty">If <code>true</code>, creates an empty factory,
		/// otherwise create a new factory with all tags registered.
		/// </param>
		public PrototypicalNodeFactory(bool empty)
		{
			Clear();
			mText = new TextNode(null, 0, 0);
			mRemark = new RemarkNode(null, 0, 0);
			mTag = new TagNode(null, 0, 0, null);
			if (!empty)
			{
				RegisterTags();
			}
		}

		/// <summary> Create a new factory with the given tag as the only registered tag.</summary>
		/// <param name="tag">The single tag to register in the otherwise empty factory.
		/// </param>
		public PrototypicalNodeFactory(ITag tag):this(true)
		{
			RegisterTag(tag);
		}
		
		/// <summary> Create a new factory with the given tags registered.</summary>
		/// <param name="tags">The tags to register in the otherwise empty factory.
		/// </param>
		public PrototypicalNodeFactory(ITag[] tags):this(true)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				RegisterTag(tags[i]);
			}
		}

		/// <summary> Get the list of tag names.</summary>
		/// <returns> The names of the tags currently registered.
		/// </returns>
		virtual public Support.ISetSupport TagNames
		{
			get
			{
				return (new Support.HashSetSupport(mBlastocyst.Keys));
			}
			
		}

		/// <summary> Get the object that is cloned to generate text nodes.</summary>
		/// <returns> The prototype for <see cref="IText"></see> nodes.
		/// </returns>
		/// <summary> Set the object to be used to generate text nodes.</summary>
		virtual public IText TextPrototype
		{
			get
			{
				return (mText);
			}
			
			set
			{
				if (null == value)
					mText = new TextNode(null, 0, 0);
				else
					mText = value;
			}
			
		}

		/// <summary> Get the object that is cloned to generate remark nodes.</summary>
		/// <returns> The prototype for {@link Remark} nodes.
		/// </returns>
		/// <summary> Set the object to be used to generate remark nodes.</summary>
		virtual public IRemark RemarkPrototype
		{
			get
			{
				return (mRemark);
			}
			
			set
			{
				if (null == value)
					mRemark = new RemarkNode(null, 0, 0);
				else
					mRemark = value;
			}
			
		}

		/// <summary> Get the object that is cloned to generate tag nodes.
		/// Clones of this object are returned from {@link #createTagNode} when no
		/// specific tag is found in the list of registered tags.
		/// </summary>
		/// <returns> The prototype for <see cref="ITag"></see> nodes.
		/// </returns>
		/// <summary> Set the object to be used to generate tag nodes.
		/// Clones of this object are returned from <see cref="CreateTagNode"></see> when no
		/// specific tag is found in the list of registered tags.
		/// </summary>
		/// If <code>null</code> the prototype is set to the default
		/// ({@link TagNode}).
		/// </param>
		virtual public ITag TagPrototype
		{
			get
			{
				return (mTag);
			}
			
			set
			{
				if (null == value)
					mTag = new TagNode(null, 0, 0, null);
				else
					mTag = value;
			}
			
		}

		/// <summary> Adds a tag to the registry.</summary>
		/// <param name="id">The name under which to register the tag.
		/// <strong>For proper operation, the id should be uppercase so it
		/// will be matched by a Map lookup.</strong>
		/// </param>
		/// <param name="tag">The tag to be returned from a {@link #createTagNode} call.
		/// </param>
		/// <returns> The tag previously registered with that id if any,
		/// or <code>null</code> if none.
		/// </returns>
		public virtual ITag Put(System.String id, ITag tag)
		{
			System.Object tempObject;
			tempObject = mBlastocyst[id];
			mBlastocyst[id] = tag;
			return ((ITag) tempObject);
		}
		
		/// <summary> Gets a tag from the registry.</summary>
		/// <param name="id">The name of the tag to return.
		/// </param>
		/// <returns> The tag registered under the <code>id</code> name,
		/// or <code>null</code> if none.
		/// </returns>
		public virtual ITag Get(System.String id)
		{
			return ((ITag) mBlastocyst[id]);
		}
		
		/// <summary> Remove a tag from the registry.</summary>
		/// <param name="id">The name of the tag to remove.
		/// </param>
		/// <returns> The tag that was registered with that <code>id</code>,
		/// or <code>null</code> if none.
		/// </returns>
		public virtual ITag Remove(System.String id)
		{
			System.Object tempObject;
			tempObject = mBlastocyst[id];
			mBlastocyst.Remove(id);
			return ((ITag) tempObject);
		}
		
		/// <summary> Clean out the registry.</summary>
		public virtual void Clear()
		{
			mBlastocyst = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
		}
		
		/// <summary> Register a tag.
		/// Registers the given tag under every <see cref="ITag.Ids"></see> id} that the
		/// tag has (i.e. all names returned by {@link Tag#getIds() tag.getIds()}.
		/// <p><strong>For proper operation, the ids are converted to uppercase so
		/// they will be matched by a Map lookup.</strong>
		/// </p>
		/// </summary>
		/// <param name="tag">The tag to register.
		/// </param>
		public virtual void RegisterTag(ITag tag)
		{
			System.String[] ids;
			
			ids = tag.Ids;
			for (int i = 0; i < ids.Length; i++)
			{
				Put(ids[i].ToUpper(new System.Globalization.CultureInfo("en")), tag);
			}
		}
		
		/// <summary> Unregister a tag.
		/// Unregisters the given tag from every {@link Tag#getIds() id} the tag has.
		/// <p><strong>The ids are converted to uppercase to undo the operation
		/// of registerTag.</strong>
		/// </summary>
		/// <param name="tag">The tag to unregister.
		/// </param>
		public virtual void UnregisterTag(ITag tag)
		{
			System.String[] ids;
			
			ids = tag.Ids;
			for (int i = 0; i < ids.Length; i++)
			{
				Remove(ids[i].ToUpper(new System.Globalization.CultureInfo("en")));
			}
		}
		
		/// <summary> Register all known tags in the tag package.
		/// Registers tags from the tag package by
		/// calling RegisterTag(Tag).
		/// </summary>
		/// <returns> 'this' nodefactory as a convenience.
		/// </returns>
		public virtual PrototypicalNodeFactory RegisterTags()
		{
			RegisterTag(new ATag());
			RegisterTag(new AppletTag());
			RegisterTag(new BaseHrefTag());
			RegisterTag(new Bullet());
			RegisterTag(new BulletList());
			RegisterTag(new CodeTag());
			RegisterTag(new DefinitionList());
			RegisterTag(new DefinitionListBullet());
			RegisterTag(new DoctypeTag());
			RegisterTag(new FormTag());
			RegisterTag(new FrameSetTag());
			RegisterTag(new FrameTag());
			RegisterTag(new HeadingTag());
			RegisterTag(new IFrameTag());
			RegisterTag(new ImageTag());
			RegisterTag(new InputTag());
			RegisterTag(new JspTag());
			RegisterTag(new LabelTag());
			RegisterTag(new LinkTag());
			RegisterTag(new MetaTag());
			RegisterTag(new ObjectTag());
			RegisterTag(new OptionTag());
			RegisterTag(new ParagraphTag());
			RegisterTag(new ScriptTag());
			RegisterTag(new SelectTag());
			RegisterTag(new StyleTag());
			RegisterTag(new TableColumn());
			RegisterTag(new TableHeader());
			RegisterTag(new TableRow());
			RegisterTag(new TableTag());
			RegisterTag(new TextareaTag());
			RegisterTag(new TitleTag());
			RegisterTag(new Div());
			RegisterTag(new Span());
			RegisterTag(new BodyTag());
			RegisterTag(new HeadTag());
			RegisterTag(new Html());
			
			return (this);
		}
		
		#region NodeFactory interface
		
		/// <summary> Create a new string node.</summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the string.
		/// </param>
		/// <param name="end">The ending position of the string.
		/// </param>
		/// <returns> A text node comprising the indicated characters from the page.
		/// </returns>
		public virtual IText CreateStringNode(Page page, int start, int end)
		{
			IText ret;
			
			try
			{
				ret = (IText) (TextPrototype.Clone());
				ret.Page = page;
				ret.StartPosition = start;
				ret.EndPosition = end;
			}
			catch
			{
				ret = new TextNode(page, start, end);
			}
			
			return (ret);
		}
		
		/// <summary> Create a new remark node.</summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the remark.
		/// </param>
		/// <param name="end">The ending position of the remark.
		/// </param>
		/// <returns> A remark node comprising the indicated characters from the page.
		/// </returns>
		public virtual IRemark CreateRemarkNode(Page page, int start, int end)
		{
			IRemark ret;
			
			try
			{
				ret = (IRemark) (RemarkPrototype.Clone());
				ret.Page = page;
				ret.StartPosition = start;
				ret.EndPosition = end;
			}
			catch
			{
				ret = new RemarkNode(page, start, end);
			}
			
			return (ret);
		}
		
		/// <summary> Create a new tag node.
		/// Note that the attributes vector contains at least one element,
		/// which is the tag name (standalone attribute) at position zero.
		/// This can be used to decide which type of node to create, or
		/// gate other processing that may be appropriate.
		/// </summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the tag.
		/// </param>
		/// <param name="end">The ending positiong of the tag.
		/// </param>
		/// <param name="attributes">The attributes contained in this tag.
		/// </param>
		/// <returns> A tag node comprising the indicated characters from the page.
		/// </returns>
		public virtual ITag CreateTagNode(Page page, int start, int end, System.Collections.ArrayList attributes)
		{
			TagAttribute attribute;
			System.String id;
			ITag prototype;
			ITag ret;
			
			ret = null;
			
			if (0 != attributes.Count)
			{
				attribute = (TagAttribute) attributes[0];
				id = attribute.GetName();
				if (null != id)
				{
					try
					{
						id = id.ToUpper(new System.Globalization.CultureInfo("en"));
						if (!id.StartsWith("/"))
						{
							if (id.EndsWith("/"))
								id = id.Substring(0, (id.Length - 1) - (0));
							prototype = (ITag) mBlastocyst[id];
							if (null != prototype)
							{
								ret = (ITag) prototype.Clone();
								ret.Page = page;
								ret.StartPosition = start;
								ret.EndPosition = end;
								ret.AttributesEx = attributes;
							}
						}
					}
					catch
					{
						// default to creating a generic one
					}
				}
			}
			if (null == ret)
			{
				// generate a generic node
				try
				{
					ret = (ITag) TagPrototype.Clone();
					ret.Page = page;
					ret.StartPosition = start;
					ret.EndPosition = end;
					ret.AttributesEx = attributes;
				}
				catch
				{
					ret = new TagNode(page, start, end, attributes);
				}
			}
			
			return (ret);
		}
		#endregion
	}
}
