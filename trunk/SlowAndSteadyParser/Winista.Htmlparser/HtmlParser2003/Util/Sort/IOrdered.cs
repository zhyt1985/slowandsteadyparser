// ***************************************************************
//  IOrdered   version:  1.0   date: 12/18/2005
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
	/// <summary> Describes an object that knows about ordering.
	/// Implementors must have a comparison function,
	/// which imposes a partial ordering on some
	/// collection of objects. Ordered objects can be passed to a
	/// sort method (such as Sort) to allow precise control
	/// over the sort order.
	/// <p>
	/// An set of elements S is partially ordered
	/// if and only if <code>e1.compare(e2)==0</code> implies that
	/// <code>e1.equals(e2)</code> for every e1 and e2 in S.
	/// <p>
	/// This all goes away in JDK 1.2.
	/// <p>
	/// For use with java.lang.Comparable from JDK 1.2:
	/// <pre>
	/// public int compare (Object o1, Object o2)
	/// {
	/// return (((Ordered)o1).compare (o2));
	/// }
	/// </pre>
	/// </summary>
	/// <seealso cref="Sort">
	/// </seealso>
	public interface IOrdered
	{
		/// <summary> Compares this object with another for order.
		/// Returns a negative integer, zero, or a positive integer
		/// as this object is less than, equal to, or greater
		/// than the second.
		/// <p>
		/// The implementor must ensure that
		/// <code>sgn(x.compare(y)) == -sgn(y.compare(x))</code>
		/// for all x and y. (This implies that <code>x.compare(y)</code>
		/// must throw an exception if and only if <code>y.compare(x)</code>
		/// throws an exception.)
		/// <p>
		/// The implementor must also ensure that the relation is transitive:
		/// <code>((x.compare(y)>0) && (y.compare(z)>0))</code>
		/// implies <code>x.compare(z)>0</code>.
		/// <p>
		/// Finally, the implementer must ensure that
		/// <code>x.compare(y)==0</code> implies that
		/// <code>sgn(x.compare(z))==sgn(y.compare(z))</code>
		/// for all z.
		/// </summary>
		/// <param name="that">The object to compare this object against.
		/// </param>
		/// <returns> A negative integer, zero, or a positive
		/// integer as this object is less than, equal to,
		/// or greater than the second.
		/// </returns>
		/// <exception cref="ClassCastException">The arguments type prevents it
		/// from being compared by this Ordered.
		/// </exception>
		int Compare(System.Object that);
	}
}
