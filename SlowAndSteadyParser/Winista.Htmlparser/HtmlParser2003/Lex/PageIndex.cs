// ***************************************************************
//  PageIndex   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util.Sort;

namespace Winista.Text.HtmlParser.Lex
{
	/// <summary> A sorted array of integers, the positions of the first characters of each line.
	/// To facilitate processing the first element should be maintained at position 0.
	/// Facilities to add, remove, search and determine row and column are provided.
	/// This class provides similar functionality to a Vector but
	/// does not incur the overhead of an <code>Integer</code> object per element.
	/// </summary>
	/// 
	[Serializable]
	public class PageIndex : ISortable
	{
		/// <summary> Starting increment for allocations.</summary>
		protected internal const int mStartIncrement = 100;
		
		/// <summary> Increment for allocations.</summary>
		protected internal int mIncrement;
		
		/// <summary> The number of valid elements.</summary>
		protected internal int mCount;
		
		/// <summary> The elements.</summary>
		protected internal int[] mIndices;
		
		/// <summary> The page associated with this index.</summary>
		protected internal Page mPage;

		/// <summary> Create an empty index.</summary>
		/// <param name="page">The page associated with this index.
		/// </param>
		public PageIndex(Page page)
		{
			mPage = page;
			mIndices = new int[mIncrement];
			mCount = 0;
			mIncrement = mStartIncrement * 2;
		}

		/// <summary> Create an index with the elements given.</summary>
		/// <param name="page">The page associated with this index.
		/// </param>
		/// <param name="cursors">The initial elements of the index.
		/// NOTE: The list must be sorted in ascending order.
		/// </param>
		public PageIndex(Page page, int[] cursors)
		{
			mPage = page;
			mIndices = cursors;
			mCount = cursors.Length;
		}

		/// <summary> Get this index's page.</summary>
		/// <returns> The page associated with this index.
		/// </returns>
		virtual public Page Page
		{
			get
			{
				return (mPage);
			}
		}

		/// <summary> Get the count of elements.</summary>
		/// <returns> The number of valid elements.
		/// </returns>
		public virtual int Size()
		{
			return (mCount);
		}
		
		/// <summary> Get the capacity for elements without reallocation.</summary>
		/// <returns> The number of spaces for elements.
		/// </returns>
		public virtual int Capacity()
		{
			return (mIndices.Length);
		}
		
		/// <summary> Add an element to the list</summary>
		/// <param name="cursor">The element to add.
		/// </param>
		/// <returns> The position at which the element was inserted or
		/// the index of the existing element if it is a duplicate.
		/// </returns>
		public virtual int Add(Cursor cursor)
		{
			int position;
			int last;
			int ret;
			
			position = cursor.Position;
			if (0 == mCount)
			{
				ret = 0;
				InsertElementAt(position, ret);
			}
			else
			{
				last = mIndices[mCount - 1];
				if (position == last)
					ret = mCount - 1;
				else if (position > last)
				{
					ret = mCount;
					InsertElementAt(position, ret);
				}
				else
				{
					// find where it goes
					ret = SortImpl.Bsearch(this, cursor);
					
					// insert, but not twice
					if (!((ret < Size()) && (position == mIndices[ret])))
						InsertElementAt(position, ret);
				}
			}
			
			return (ret);
		}
		
		/// <summary> Add an element to the list</summary>
		/// <param name="cursor">The element to add.
		/// </param>
		/// <returns> The position at which the element was inserted or
		/// the index of the existing element if it is a duplicate.
		/// </returns>
		public virtual int Add(int cursor)
		{
			return (Add(new Cursor(Page, cursor)));
		}
		
		/// <summary> Remove an element from the list</summary>
		/// <param name="cursor">The element to remove.
		/// </param>
		public virtual void  Remove(Cursor cursor)
		{
			int i;
			
			// find it
			i = SortImpl.Bsearch(this, cursor);
			
			// remove
			if ((i < Size()) && (cursor.Position == mIndices[i]))
				RemoveElementAt(i);
		}
		
		/// <summary> Remove an element from the list</summary>
		/// <param name="cursor">The element to remove.
		/// </param>
		public virtual void Remove(int cursor)
		{
			Remove(new Cursor(Page, cursor));
		}
		
		/// <summary> Get an element from the list.</summary>
		/// <param name="index">The index of the element to get.
		/// </param>
		/// <returns> The element.
		/// </returns>
		public virtual int ElementAt(int index)
		{
			if (index >= mCount)
				// negative index is handled by array.. below
				throw new System.IndexOutOfRangeException("index " + index + " beyond current limit");
			else
				return (mIndices[index]);
		}
		
		/// <summary> Get the line number for a cursor.</summary>
		/// <param name="cursor">The character offset into the page.
		/// </param>
		/// <returns> The line number the character is in.
		/// </returns>
		public virtual int Row(Cursor cursor)
		{
			int ret;
			
			ret = SortImpl.Bsearch(this, cursor);
			// handle line transition, the search returns the index if it matches
			// exactly one of the line end positions, so we advance one line if
			// it's equal to the offset at the row index, since that position is
			// actually the beginning of the next line
			if ((ret < mCount) && (cursor.Position == mIndices[ret]))
				ret++;
			
			return (ret);
		}
		
		/// <summary> Get the line number for a position.</summary>
		/// <param name="cursor">The character offset into the page.
		/// </param>
		/// <returns> The line number the character is in.
		/// </returns>
		public virtual int Row(int cursor)
		{
			return (Row(new Cursor(Page, cursor)));
		}
		
		/// <summary> Get the column number for a cursor.</summary>
		/// <param name="cursor">The character offset into the page.
		/// </param>
		/// <returns> The character offset into the line this cursor is on.
		/// </returns>
		public virtual int Column(Cursor cursor)
		{
			int row;
			int previous;
			
			row = Row(cursor);
			if (0 != row)
				previous = this.ElementAt(row - 1);
			else
				previous = 0;
			
			return (cursor.Position - previous);
		}
		
		/// <summary> Get the column number for a position.</summary>
		/// <param name="cursor">The character offset into the page.
		/// </param>
		/// <returns> The character offset into the line this cursor is on.
		/// </returns>
		public virtual int Column(int cursor)
		{
			return (Column(new Cursor(Page, cursor)));
		}
		
		/// <summary> Get the elements as an array of int.</summary>
		/// <returns> A new array containing the elements,
		/// i.e. a snapshot of the index.
		/// </returns>
		public virtual int[] Get()
		{
			int[] ret = new int[Size()];
			Array.Copy(mIndices, 0, ret, 0, Size());
			
			return (ret);
		}
		
		/// <summary> Binary search for the element.</summary>
		/// <param name="cursor">The element to search for.
		/// </param>
		/// <returns> The index at which the element was found or is to be inserted.
		/// </returns>
		protected internal virtual int Bsearch(int cursor)
		{
			return (SortImpl.Bsearch(this, new Cursor(Page, cursor)));
		}
		
		/// <summary> Binary search for the element.</summary>
		/// <param name="cursor">The element to search for.
		/// </param>
		/// <param name="first">The index to start at.
		/// </param>
		/// <param name="last">The index to stop at.
		/// </param>
		/// <returns> The index at which the element was found or is to be inserted.
		/// </returns>
		protected internal virtual int Bsearch(int cursor, int first, int last)
		{
			return (SortImpl.Bsearch(this, new Cursor(Page, cursor), first, last));
		}
		
		/// <summary> Inserts an element into the list.
		/// The index must be a value greater than or equal to 0 and less than
		/// or equal to the current size of the array.
		/// </summary>
		/// <param name="cursor">The element to insert.
		/// </param>
		/// <param name="index">The index in the list to insert it at.
		/// </param>
		protected internal virtual void InsertElementAt(int cursor, int index)
		{
			if ((index >= Capacity()) || (Size() == Capacity()))
			{
				// allocate more space
				int[] new_values = new int[System.Math.Max(Capacity() + mIncrement, index + 1)];
				mIncrement *= 2;
				if (index < Capacity())
				{
					// copy and shift up in two pieces
					Array.Copy(mIndices, 0, new_values, 0, index);
					Array.Copy(mIndices, index, new_values, index + 1, Capacity() - index);
				}
				else
					Array.Copy(mIndices, 0, new_values, 0, Capacity());
				mIndices = new_values;
			}
			else if (index < Size())
				// shift up
				Array.Copy(mIndices, index, mIndices, index + 1, Capacity() - (index + 1));
			mIndices[index] = cursor;
			mCount++;
		}
		
		/// <summary> Remove an element from the list.</summary>
		/// <param name="index">The index of the item to remove.
		/// </param>
		protected internal virtual void RemoveElementAt(int index)
		{
			// shift
			Array.Copy(mIndices, index + 1, mIndices, index, Capacity() - (index + 1));
			mIndices[Capacity() - 1] = 0;
			mCount--;
		}
		
		//
		// Sortable interface
		//
		
		/// <summary> Returns the first index of the Sortable.</summary>
		/// <returns> The index of the first element.
		/// </returns>
		public virtual int First()
		{
			return (0);
		}
		
		/// <summary> Returns the last index of the Sortable.</summary>
		/// <returns> The index of the last element.
		/// If this were an array object this would be (object.length - 1).
		/// For an empty index this will return -1.
		/// </returns>
		public virtual int Last()
		{
			return (mCount - 1);
		}
		
		/// <summary> Fetch the object at the given index.</summary>
		/// <param name="index">The item number to get.
		/// </param>
		/// <param name="reuse">If this argument is not null, it is an object
		/// acquired from a previous fetch that is no longer needed and
		/// may be returned as the result if it makes mores sense to alter
		/// and return it than to fetch or create a new element. That is, the
		/// reuse object is garbage and may be used to avoid allocating a new
		/// object if that would normally be the strategy.
		/// </param>
		/// <returns> The Ordered object at that index.
		/// </returns>
		public virtual IOrdered Fetch(int index, IOrdered reuse)
		{
			Cursor ret;
			
			if (null != reuse)
			{
				ret = (Cursor) reuse;
				ret.mPosition = mIndices[index];
				ret.mPage = Page; // redundant
			}
			else
				ret = new Cursor(Page, mIndices[index]);
			
			return (ret);
		}
		
		/// <summary> Swaps the elements at the given indicies.</summary>
		/// <param name="i">One index.
		/// </param>
		/// <param name="j">The other index.
		/// </param>
		public virtual void Swap(int i, int j)
		{
			int temp = mIndices[i];
			mIndices[i] = mIndices[j];
			mIndices[j] = temp;
		}
	}
}
