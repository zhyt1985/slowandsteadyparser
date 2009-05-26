// ***************************************************************
//  Sortable   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Util.Sort
{
	/// <summary> Provides a mechanism to abstract the sort process.
	/// Classes implementing this interface are collections of Ordered objects
	/// that are to be sorted by the Sort class and are
	/// not necessarily Vectors or Arrays of Ordered objects.
	/// </summary>
	/// <seealso cref="Sort">
	/// </seealso>
	public interface ISortable
	{
		/// <summary> Returns the first index of the Sortable.</summary>
		/// <returns> The index of the first element.
		/// </returns>
		int First();
		
		/// <summary> Returns the last index of the Sortable.</summary>
		/// <returns> The index of the last element.
		/// If this were an array object this would be (object.length - 1).
		/// </returns>
		int Last();
		
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
		IOrdered Fetch(int index, IOrdered reuse);
		
		/// <summary> Swaps the elements at the given indicies.</summary>
		/// <param name="i">One index.
		/// </param>
		/// <param name="j">The other index.
		/// </param>
		void  Swap(int i, int j);
	}
}
