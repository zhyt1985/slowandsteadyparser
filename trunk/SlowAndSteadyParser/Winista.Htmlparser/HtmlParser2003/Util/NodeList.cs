// ***************************************************************
//  NodeList   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Visitors;
using Winista.Text.HtmlParser.Filters;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary>
	/// Summary description for NodeList.
	/// </summary>
	public class NodeList
	{
		public class SimpleNodeIterator : ISimpleNodeIterator
		{
			public SimpleNodeIterator(NodeList enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}

			private void  InitBlock(NodeList enclosingInstance)
			{
				this.m_enclosingInstance = enclosingInstance;
			}

			private NodeList m_enclosingInstance;
			public NodeList Enclosing_Instance
			{
				get
				{
					return m_enclosingInstance;
				}
				
			}
			internal int count = 0;
			
			public virtual bool HasMoreNodes()
			{
				return count < Enclosing_Instance.m_iSize;
			}
			
			public virtual INode NextNode()
			{
				lock (Enclosing_Instance)
				{
					if (count < Enclosing_Instance.m_iSize)
					{
						return Enclosing_Instance.nodeData[count++];
					}
				}
				throw new System.ArgumentOutOfRangeException("Vector Enumeration");
			}
		}

		private const int INITIAL_CAPACITY = 10;
		//private static final int CAPACITY_INCREMENT=20;
		private INode[] nodeData;
		private int m_iSize;
		private int capacity;
		private int capacityIncrement;
		private int numberOfAdjustments;

		public NodeList()
		{
			m_iSize = 0;
			capacity = INITIAL_CAPACITY;
			nodeData = NewNodeArrayFor(capacity);
			capacityIncrement = capacity * 2;
			numberOfAdjustments = 0;
		}

		/// <summary> Create a one element node list.</summary>
		/// <param name="node">The initial node to add.
		/// </param>
		public NodeList(INode node):this()
		{
			Add(node);
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public int NumberOfAdjustments
		{
			get
			{
				return numberOfAdjustments;
			}
			
		}

		/// <summary>
		/// Add given node into the collection.
		/// </summary>
		/// <param name="node"></param>
		public virtual void Add(INode node)
		{
			if (m_iSize == capacity)
				AdjustVectorCapacity();
			nodeData[m_iSize++] = node;
		}
		
		/// <summary> Add another node list to this one.</summary>
		/// <param name="list">The list to add.
		/// </param>
		public virtual void Add(NodeList list)
		{
			for (int i = 0; i < list.m_iSize; i++)
				Add(list.nodeData[i]);
		}

		/// <summary> Insert the given node at the head of the list.</summary>
		/// <param name="node">The new first element.
		/// </param>
		public virtual void Prepend(INode node)
		{
			if (m_iSize == capacity)
				AdjustVectorCapacity();
			Array.Copy(nodeData, 0, nodeData, 1, m_iSize);
			m_iSize++;
			nodeData[0] = node;
		}
		
		private void  AdjustVectorCapacity()
		{
			capacity += capacityIncrement;
			capacityIncrement *= 2;
			INode[] oldData = nodeData;
			nodeData = NewNodeArrayFor(capacity);
			Array.Copy(oldData, 0, nodeData, 0, m_iSize);
			numberOfAdjustments++;
		}

		private INode[] NewNodeArrayFor(int capacity)
		{
			return new INode[capacity];
		}
		
		/// <summary>
		/// 
		/// </summary>
		public Int32 Count
		{
			get
			{
				return m_iSize;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual int Size()
		{
			return m_iSize;
		}
		
		/// <summary>
		/// Gets element at given index.
		/// </summary>
		public virtual INode this[Int32 idx]
		{
			get
			{
				return nodeData[idx];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual INode ElementAt(int i)
		{
			return nodeData[i];
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual ISimpleNodeIterator Elements()
		{
			return new SimpleNodeIterator(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual INode[] ToNodeArray()
		{
			INode[] nodeArray = NewNodeArrayFor(m_iSize);
			Array.Copy(nodeData, 0, nodeArray, 0, m_iSize);
			return nodeArray;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		public virtual void CopyToNodeArray(INode[] array)
		{
			Array.Copy(nodeData, 0, array, 0, m_iSize);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual System.String AsString()
		{
			System.Text.StringBuilder buff = new System.Text.StringBuilder();
			for (int i = 0; i < m_iSize; i++)
				buff.Append(nodeData[i].ToPlainTextString());
			return buff.ToString();
		}
		
		/// <summary> Convert this nodelist into the equivalent HTML.</summary>
		/// <deprecated> Use <see cref="ToHtml"></see>.
		/// </deprecated>
		/// <returns> The contents of the list as HTML text.
		/// </returns>
		public virtual System.String AsHtml()
		{
			return (ToHtml());
		}
		
		/// <summary> Convert this nodelist into the equivalent HTML.</summary>
		/// <returns> The contents of the list as HTML text.
		/// </returns>
		public virtual System.String ToHtml()
		{
			System.Text.StringBuilder buff = new System.Text.StringBuilder();
			for (int i = 0; i < m_iSize; i++)
				buff.Append(nodeData[i].ToHtml());
			return buff.ToString();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual INode Remove(int index)
		{
			INode ret;
			ret = nodeData[index];
			Array.Copy(nodeData, index + 1, nodeData, index, m_iSize - index - 1);
			nodeData[m_iSize - 1] = null;
			m_iSize--;
			return (ret);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void RemoveAll()
		{
			m_iSize = 0;
			capacity = INITIAL_CAPACITY;
			nodeData = NewNodeArrayFor(capacity);
			capacityIncrement = capacity * 2;
			numberOfAdjustments = 0;
		}

		/// <summary> Check to see if the NodeList contains the supplied Node.</summary>
		/// <param name="node">The node to look for.
		/// </param>
		/// <returns> True is the Node is in this NodeList.
		/// </returns>
		public virtual bool Contains(INode node)
		{
			return (- 1 != IndexOf(node));
		}

		/// <summary> Finds the index of the supplied Node.</summary>
		/// <param name="node">The node to look for.
		/// </param>
		/// <returns> The index of the node in the list or -1 if it isn't found.
		/// </returns>
		public virtual int IndexOf(INode node)
		{
			int ret;
			
			ret = - 1;
			for (int i = 0; (i < m_iSize) && (- 1 == ret); i++)
				if (nodeData[i].Equals(node))
					ret = i;
			
			return (ret);
		}

		/// <summary> Remove the supplied Node from the list.</summary>
		/// <param name="node">The node to remove.
		/// </param>
		/// <returns> True if the node was found and removed from the list.
		/// </returns>
		public virtual bool Remove(INode node)
		{
			int index;
			bool ret;
			
			ret = false;
			if (- 1 != (index = IndexOf(node)))
			{
				Remove(index);
				ret = true;
			}
			
			return (ret);
		}
		
		/// <summary> Return the contents of the list as a string.
		/// Suitable for debugging.
		/// </summary>
		/// <returns> A string representation of the list. 
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder text = new System.Text.StringBuilder();
			for (int i = 0; i < m_iSize; i++)
				text.Append(nodeData[i]);
			return (text.ToString());
		}
		
		/// <summary> Filter the list with the given filter non-recursively.</summary>
		/// <param name="filter">The filter to use.
		/// </param>
		/// <returns> A new node array containing the nodes accepted by the filter.
		/// This is a linear list and preserves the nested structure of the returned
		/// nodes only.
		/// </returns>
		public virtual NodeList ExtractAllNodesThatMatch(INodeFilter filter)
		{
			return (ExtractAllNodesThatMatch(filter, false));
		}
		
		/// <summary> Filter the list with the given filter.</summary>
		/// <param name="filter">The filter to use.
		/// </param>
		/// <param name="recursive">If <code>true<code> digs into the children recursively.
		/// </param>
		/// <returns> A new node array containing the nodes accepted by the filter.
		/// This is a linear list and preserves the nested structure of the returned
		/// nodes only.
		/// </returns>
		public virtual NodeList ExtractAllNodesThatMatch(INodeFilter filter, bool recursive)
		{
			INode node;
			NodeList children;
			NodeList ret;
			
			ret = new NodeList();
			for (int i = 0; i < m_iSize; i++)
			{
				node = nodeData[i];
				if (filter.Accept(node))
					ret.Add(node);
				if (recursive)
				{
					children = node.Children;
					if (null != children)
						ret.Add(children.ExtractAllNodesThatMatch(filter, recursive));
				}
			}
			
			return (ret);
		}
		
		/// <summary> Remove nodes not matching the given filter non-recursively.</summary>
		/// <param name="filter">The filter to use.
		/// </param>
		public virtual void KeepAllNodesThatMatch(INodeFilter filter)
		{
			KeepAllNodesThatMatch(filter, false);
		}
		
		/// <summary> Remove nodes not matching the given filter.</summary>
		/// <param name="filter">The filter to use.
		/// </param>
		/// <param name="recursive">If <code>true<code> digs into the children recursively.
		/// </param>
		public virtual void KeepAllNodesThatMatch(INodeFilter filter, bool recursive)
		{
			INode node;
			NodeList children;
			
			for (int i = 0; i < m_iSize; )
			{
				node = nodeData[i];
				if (!filter.Accept(node))
					Remove(i);
				else
				{
					if (recursive)
					{
						children = node.Children;
						if (null != children)
							children.KeepAllNodesThatMatch(filter, recursive);
					}
					i++;
				}
			}
		}
		
		/// <summary> Convenience method to search for nodes of the given type non-recursively.</summary>
		/// <param name="classType">The class to search for.
		/// </param>
		public virtual NodeList SearchFor(System.Type classType)
		{
			return (SearchFor(classType, false));
		}
		
		/// <summary> Convenience method to search for nodes of the given type.</summary>
		/// <param name="classType">The class to search for.
		/// </param>
		/// <param name="recursive">If <code>true<code> digs into the children recursively.
		/// </param>
		public virtual NodeList SearchFor(System.Type classType, bool recursive)
		{
			return (ExtractAllNodesThatMatch(new NodeClassFilter(classType), recursive));
		}
		
		/// <summary> Utility to apply a visitor to a node list.
		/// Provides for a visitor to modify the contents of a page and get the
		/// modified HTML as a string with code like this:
		/// <pre>
		/// Parser parser = new Parser ("http://whatever");
		/// NodeList list = parser.parse (null); // no filter
		/// list.visitAllNodesWith (visitor);
		/// System.out.println (list.toHtml ());
		/// </pre>
		/// </summary>
		public virtual void VisitAllNodesWith(NodeVisitor visitor)
		{
			INode node;
			
			visitor.BeginParsing();
			for (int i = 0; i < m_iSize; i++)
			{
				node = nodeData[i];
				node.Accept(visitor);
			}
			visitor.FinishedParsing();
		}
	}
}
