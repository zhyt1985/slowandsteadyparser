// ***************************************************************
//  SortImpl   version:  1.0   date: 12/18/2005
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
	/// <summary> A quick sort algorithm to sort Vectors or arrays.
	/// Provides sort and binary search capabilities.
	/// <p>
	/// This all goes away in JDK 1.2.
	/// <p>
	/// </summary>
	/// <author>  James Gosling
	/// </author>
	/// <author>  Kevin A. Smith
	/// </author>
	/// <author>  Derrick Oswald
	/// </author>
	/// <version>  1.4, 11 June, 1997
	/// </version>
	public class SortImpl
	{
		public SortImpl()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary> This is a generic version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle vectors that are already
		/// sorted, and vectors with duplicate keys.
		/// Equivalent to:
		/// <pre>
		/// QuickSort (v, 0, v.size () - 1);
		/// </pre>
		/// </summary>
		/// <param name="v">A <code>Vector</code> of <code>Ordered</code> items.
		/// </param>
		/// <exception cref="ClassCastException">If the vector contains objects that
		/// are not <code>Ordered</code>.
		/// </exception>
		public static void  QuickSort(System.Collections.ArrayList v)
		{
			QuickSort(v, 0, v.Count - 1);
		}
		
		/// <summary> This is a generic version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle vectors that are already
		/// sorted, and vectors with duplicate keys.
		/// <p>
		/// If you think of a one dimensional vector as going from
		/// the lowest index on the left to the highest index on the right
		/// then the parameters to this function are lowest index or
		/// left and highest index or right.
		/// </summary>
		/// <param name="v">A <code>Vector</code> of <code>Ordered</code> items.
		/// </param>
		/// <param name="lo0">Left boundary of vector partition.
		/// </param>
		/// <param name="hi0">Right boundary of vector partition.
		/// </param>
		/// <exception cref="ClassCastException">If the vector contains objects that
		/// are not <code>Ordered</code>.
		/// </exception>
		public static void  QuickSort(System.Collections.ArrayList v, int lo0, int hi0)
		{
			int lo = lo0;
			int hi = hi0;
			IOrdered mid;
			
			if (hi0 > lo0)
			{
				// arbitrarily establish partition element as the midpoint of the vector
				mid = (IOrdered) v[(lo0 + hi0) / 2];
				
				// loop through the vector until indices cross
				while (lo <= hi)
				{
					// find the first element that is greater than or equal to
					// the partition element starting from the left index
					while ((lo < hi0) && (0 > ((IOrdered) v[lo]).Compare(mid)))
						++lo;
					
					// find an element that is smaller than or equal to
					// the partition element starting from the right index
					while ((hi > lo0) && (0 < ((IOrdered) v[hi]).Compare(mid)))
						--hi;
					
					// if the indexes have not crossed, swap
					if (lo <= hi)
						Swap(v, lo++, hi--);
				}
				
				// if the right index has not reached the left side of array
				// must now sort the left partition
				if (lo0 < hi)
					QuickSort(v, lo0, hi);
				
				// if the left index has not reached the right side of array
				// must now sort the right partition
				if (lo < hi0)
					QuickSort(v, lo, hi0);
			}
		}
		
		private static void Swap(System.Collections.ArrayList v, int i, int j)
		{
			System.Object o;
			
			o = v[i];
			v[i] = v[j];
			v[j] = o;
		}
		
		/// <summary> This is a generic version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle arrays that are already sorted,
		/// and arrays with duplicate keys.
		/// <p>
		/// Equivalent to:
		/// <pre>
		/// QuickSort (a, 0, a.length - 1);
		/// </pre>
		/// </summary>
		/// <param name="a">An array of <code>Ordered</code> items.
		/// </param>
		public static void  QuickSort(IOrdered[] a)
		{
			QuickSort(a, 0, a.Length - 1);
		}
		
		/// <summary> This is a generic version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle arrays that are already sorted,
		/// and arrays with duplicate keys.
		/// <p>
		/// If you think of a one dimensional array as going from
		/// the lowest index on the left to the highest index on the right
		/// then the parameters to this function are lowest index or
		/// left and highest index or right.
		/// </summary>
		/// <param name="a">An array of <code>Ordered</code> items.
		/// </param>
		/// <param name="lo0">Left boundary of array partition.
		/// </param>
		/// <param name="hi0">Right boundary of array partition.
		/// </param>
		public static void  QuickSort(IOrdered[] a, int lo0, int hi0)
		{
			int lo = lo0;
			int hi = hi0;
			IOrdered mid;
			
			if (hi0 > lo0)
			{
				// arbitrarily establish partition element as the midpoint of the array
				mid = a[(lo0 + hi0) / 2];
				
				// loop through the vector until indices cross
				while (lo <= hi)
				{
					// find the first element that is greater than or equal to
					// the partition element starting from the left index
					while ((lo < hi0) && (0 > a[lo].Compare(mid)))
						++lo;
					
					// find an element that is smaller than or equal to
					// the partition element starting from the right Index.
					while ((hi > lo0) && (0 < a[hi].Compare(mid)))
						--hi;
					
					// if the indexes have not crossed, swap
					if (lo <= hi)
						Swap(a, lo++, hi--);
				}
				
				// if the right index has not reached the left side of array
				// must now sort the left partition
				if (lo0 < hi)
					QuickSort(a, lo0, hi);
				
				// if the left index has not reached the right side of array
				// must now sort the right partition
				if (lo < hi0)
					QuickSort(a, lo, hi0);
			}
		}
		
		/// <summary> Swaps two elements of an array.</summary>
		/// <param name="a">The array of elements.
		/// </param>
		/// <param name="i">The index of one item to swap.
		/// </param>
		/// <param name="j">The index of the other item to swap.
		/// </param>
		private static void Swap(System.Object[] a, int i, int j)
		{
			System.Object o;
			o = a[i];
			a[i] = a[j];
			a[j] = o;
		}
		
		/// <summary> This is a string version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle arrays that are already sorted,
		/// and arrays with duplicate keys.
		/// <p>
		/// Equivalent to:
		/// <pre>
		/// QuickSort (a, 0, a.length - 1);
		/// </pre>
		/// </summary>
		/// <param name="a">An array of <code>String</code> items.
		/// </param>
		public static void  QuickSort(System.String[] a)
		{
			QuickSort(a, 0, a.Length - 1);
		}
		
		/// <summary> This is a string version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle arrays that are already sorted,
		/// and arrays with duplicate keys.
		/// <p>
		/// If you think of a one dimensional array as going from
		/// the lowest index on the left to the highest index on the right
		/// then the parameters to this function are lowest index or
		/// left and highest index or right.
		/// </summary>
		/// <param name="a">An array of <code>String</code> items.
		/// </param>
		/// <param name="lo0">Left boundary of array partition.
		/// </param>
		/// <param name="hi0">Right boundary of array partition.
		/// </param>
		public static void  QuickSort(System.String[] a, int lo0, int hi0)
		{
			int lo = lo0;
			int hi = hi0;
			System.String mid;
			
			if (hi0 > lo0)
			{
				// arbitrarily establish partition element as the midpoint of the array
				mid = a[(lo0 + hi0) / 2];
				
				// loop through the vector until indices cross
				while (lo <= hi)
				{
					// find the first element that is greater than or equal to
					// the partition element starting from the left index
					while ((lo < hi0) && (0 > String.CompareOrdinal(a[lo], mid)))
						++lo;
					
					// find an element that is smaller than or equal to
					// the partition element starting from the right Index.
					while ((hi > lo0) && (0 < String.CompareOrdinal(a[hi], mid)))
						--hi;
					
					// if the indexes have not crossed, swap
					if (lo <= hi)
						Swap(a, lo++, hi--);
				}
				
				// if the right index has not reached the left side of array
				// must now sort the left partition
				if (lo0 < hi)
					QuickSort(a, lo0, hi);
				
				// if the left index has not reached the right side of array
				// must now sort the right partition
				if (lo < hi0)
					QuickSort(a, lo, hi0);
			}
		}
		
		/// <summary> This is a generic version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle Sortable objects that are already
		/// sorted, and Sortable objects with duplicate keys.
		/// <p>
		/// </summary>
		/// <param name="sortable">A <code>Sortable</code> object.
		/// </param>
		/// <param name="lo0">Left boundary of partition.
		/// </param>
		/// <param name="hi0">Right boundary of partition.
		/// </param>
		public static void  QuickSort(ISortable sortable, int lo0, int hi0)
		{
			int lo = lo0;
			int hi = hi0;
			IOrdered mid;
			IOrdered test;
			
			if (hi0 > lo0)
			{
				// arbitrarily establish partition element as the midpoint of the vector
				mid = sortable.Fetch((lo0 + hi0) / 2, null);
				test = null;
				
				// loop through the vector until indices cross
				while (lo <= hi)
				{
					// find the first element that is greater than or equal to
					// the partition element starting from the left index
					while ((lo < hi0) && (0 > (test = sortable.Fetch(lo, test)).Compare(mid)))
						++lo;
					
					// find an element that is smaller than or equal to
					// the partition element starting from the right index
					while ((hi > lo0) && (0 < (test = sortable.Fetch(hi, test)).Compare(mid)))
						--hi;
					
					// if the indexes have not crossed, swap
					if (lo <= hi)
						sortable.Swap(lo++, hi--);
				}
				
				// if the right index has not reached the left side of array
				// must now sort the left partition
				if (lo0 < hi)
					QuickSort(sortable, lo0, hi);
				
				// if the left index has not reached the right side of array
				// must now sort the right partition
				if (lo < hi0)
					QuickSort(sortable, lo, hi0);
			}
		}
		
		/// <summary> This is a generic version of C.A.R Hoare's Quick Sort algorithm.
		/// This will handle Sortable objects that are already
		/// sorted, and Sortable objects with duplicate keys.
		/// <p>
		/// Equivalent to:
		/// <pre>
		/// QuickSort (sortable, sortable.first (), sortable.last ());
		/// </pre>
		/// </summary>
		/// <param name="sortable">A <code>Sortable</code> object.
		/// </param>
		public static void  QuickSort(ISortable sortable)
		{
			QuickSort(sortable, sortable.First(), sortable.Last());
		}
		
		/// <summary> Sort a Hashtable.</summary>
		/// <param name="h">A Hashtable with String or Ordered keys.
		/// </param>
		/// <returns> A sorted array of the keys.
		/// </returns>
		public static System.Object[] QuickSort(System.Collections.Hashtable h)
		{
			System.Collections.IEnumerator e;
			bool are_strings;
			System.Object[] ret;
			
			// make the array
			ret = new IOrdered[h.Count];
			e = h.Keys.GetEnumerator();
			are_strings = true; // until proven otherwise
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = e.Current;
				if (are_strings && !(ret[i] is System.String))
					are_strings = false;
			}
			
			// sort it
			if (are_strings)
				QuickSort((System.String[]) ret);
			else
				QuickSort((IOrdered[]) ret);
			
			return (ret);
		}
		
		/// <summary> Binary search for an object</summary>
		/// <param name="set_Renamed">The collection of <code>Ordered</code> objects.
		/// </param>
		/// <param name="ref_Renamed">The name to search for.
		/// </param>
		/// <param name="lo">The lower index within which to look.
		/// </param>
		/// <param name="hi">The upper index within which to look.
		/// </param>
		/// <returns> The index at which reference was found or is to be inserted.
		/// </returns>
		public static int Bsearch(ISortable set_Renamed, IOrdered ref_Renamed, int lo, int hi)
		{
			int num;
			int mid;
			IOrdered ordered;
			int half;
			int result;
			int ret;
			
			ret = - 1;
			
			num = (hi - lo) + 1;
			ordered = null;
			while ((- 1 == ret) && (lo <= hi))
			{
				half = num / 2;
				mid = lo + ((0 != (num & 1))?half:half - 1);
				ordered = set_Renamed.Fetch(mid, ordered);
				result = ref_Renamed.Compare(ordered);
				if (0 == result)
					ret = mid;
				else if (0 > result)
				{
					hi = mid - 1;
					num = ((0 != (num & 1))?half:half - 1);
				}
				else
				{
					lo = mid + 1;
					num = half;
				}
			}
			if (- 1 == ret)
				ret = lo;
			
			return (ret);
		}
		
		/// <summary> Binary search for an object</summary>
		/// <param name="set_Renamed">The collection of <code>Ordered</code> objects.
		/// </param>
		/// <param name="ref_Renamed">The name to search for.
		/// </param>
		/// <returns> The index at which reference was found or is to be inserted.
		/// </returns>
		public static int Bsearch(ISortable set_Renamed, IOrdered ref_Renamed)
		{
			return (Bsearch(set_Renamed, ref_Renamed, set_Renamed.First(), set_Renamed.Last()));
		}
		
		/// <summary> Binary search for an object</summary>
		/// <param name="vector">The vector of <code>Ordered</code> objects.
		/// </param>
		/// <param name="ref_Renamed">The name to search for.
		/// </param>
		/// <param name="lo">The lower index within which to look.
		/// </param>
		/// <param name="hi">The upper index within which to look.
		/// </param>
		/// <returns> The index at which reference was found or is to be inserted.
		/// </returns>
		public static int Bsearch(System.Collections.ArrayList vector, IOrdered ref_Renamed, int lo, int hi)
		{
			int num;
			int mid;
			int half;
			int result;
			int ret;
			
			ret = - 1;
			
			num = (hi - lo) + 1;
			while ((- 1 == ret) && (lo <= hi))
			{
				half = num / 2;
				mid = lo + ((0 != (num & 1))?half:half - 1);
				result = ref_Renamed.Compare(vector[mid]);
				if (0 == result)
					ret = mid;
				else if (0 > result)
				{
					hi = mid - 1;
					num = ((0 != (num & 1))?half:half - 1);
				}
				else
				{
					lo = mid + 1;
					num = half;
				}
			}
			if (- 1 == ret)
				ret = lo;
			
			return (ret);
		}
		
		/// <summary> Binary search for an object</summary>
		/// <param name="vector">The vector of <code>Ordered</code> objects.
		/// </param>
		/// <param name="ref_Renamed">The name to search for.
		/// </param>
		/// <returns> The index at which reference was found or is to be inserted.
		/// </returns>
		public static int bsearch(System.Collections.ArrayList vector, IOrdered ref_Renamed)
		{
			return (Bsearch(vector, ref_Renamed, 0, vector.Count - 1));
		}
		
		/// <summary> Binary search for an object</summary>
		/// <param name="array">The array of <code>Ordered</code> objects.
		/// </param>
		/// <param name="ref_Renamed">The name to search for.
		/// </param>
		/// <param name="lo">The lower index within which to look.
		/// </param>
		/// <param name="hi">The upper index within which to look.
		/// </param>
		/// <returns> The index at which reference was found or is to be inserted.
		/// </returns>
		public static int Bsearch(IOrdered[] array, IOrdered ref_Renamed, int lo, int hi)
		{
			int num;
			int mid;
			int half;
			int result;
			int ret;
			
			ret = - 1;
			
			num = (hi - lo) + 1;
			while ((- 1 == ret) && (lo <= hi))
			{
				half = num / 2;
				mid = lo + ((0 != (num & 1))?half:half - 1);
				result = ref_Renamed.Compare(array[mid]);
				if (0 == result)
					ret = mid;
				else if (0 > result)
				{
					hi = mid - 1;
					num = ((0 != (num & 1))?half:half - 1);
				}
				else
				{
					lo = mid + 1;
					num = half;
				}
			}
			if (- 1 == ret)
				ret = lo;
			
			return (ret);
		}
		
		/// <summary> Binary search for an object</summary>
		/// <param name="array">The array of <code>Ordered</code> objects.
		/// </param>
		/// <param name="ref_Renamed">The name to search for.
		/// </param>
		/// <returns> The index at which reference was found or is to be inserted.
		/// </returns>
		public static int Bsearch(IOrdered[] array, IOrdered ref_Renamed)
		{
			return (Bsearch(array, ref_Renamed, 0, array.Length - 1));
		}
	}
}
