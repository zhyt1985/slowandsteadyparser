// ***************************************************************
//  Cursor   version:  1.0   date: 12/18/2005
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
	/// <summary>
	/// Summary description for Cursor.
	/// </summary>
	public class Cursor : IOrdered, System.ICloneable
	{
		/// <summary> This cursor's position.</summary>
		protected internal int mPosition;
		
		/// <summary> This cursor's page.</summary>
		protected internal Page mPage;

		/// <summary> Construct a <code>Cursor</code> from the page and position given.</summary>
		/// <param name="page">The page this cursor is on.
		/// </param>
		/// <param name="offset">The character offset within the page.
		/// </param>
		public Cursor(Page page, int offset)
		{
			mPage = page;
			mPosition = offset;
		}

		/// <summary> Get this cursor's page.</summary>
		/// <returns> The page associated with this cursor.
		/// </returns>
		virtual public Page Page
		{
			get
			{
				return (mPage);
			}
		}

		/// <summary> Get the position of this cursor.</summary>
		/// <returns> The cursor position.
		/// </returns>
		/// <summary> Set the position of this cursor.</summary>
		/// <param name="position">The new cursor position.
		/// </param>
		virtual public int Position
		{
			get
			{
				return (mPosition);
			}
			
			set
			{
				mPosition = value;
			}
		}

		/// <summary> Move the cursor position ahead one character.</summary>
		public virtual void Advance()
		{
			mPosition++;
		}
		
		/// <summary> Move the cursor position back one character.</summary>
		public virtual void  Retreat()
		{
			mPosition--;
			if (0 > mPosition)
				mPosition = 0;
		}
		
		/// <summary> Make a new cursor just like this one.</summary>
		/// <returns> The new cursor positioned where <code>this</code> one is,
		/// and referring to the same page.
		/// </returns>
		public virtual Cursor Dup()
		{
			try
			{
				return ((Cursor) Clone());
			}
				//UPGRADE_NOTE: Exception 'java.lang.CloneNotSupportedException' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
			catch (System.Exception cnse)
			{
				return (new Cursor(Page, Position));
			}
		}
		
		/// <summary> Return a string representation of this cursor</summary>
		/// <returns> A string of the form "n[r,c]", where n is the character position,
		/// r is the row (zero based) and c is the column (zero based) on the page.
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder ret;
			
			ret = new System.Text.StringBuilder(9 * 3 + 3); // three ints and delimiters
			ret.Append(Position);
			ret.Append("[");
			if (null != mPage)
				ret.Append(mPage.Row(this));
			else
				ret.Append("?");
			ret.Append(",");
			if (null != mPage)
				ret.Append(mPage.Column(this));
			else
				ret.Append("?");
			ret.Append("]");
			
			return (ret.ToString());
		}
		
		//
		// Ordered interface
		//
		
		/// <summary> Compare one reference to another.</summary>
		/// <param name="that">The object to compare this to.
		/// </param>
		/// <returns> A negative integer, zero, or a positive
		/// integer as this object is less than, equal to,
		/// or greater than that object.
		/// </returns>
		/// <seealso cref="IOrdered">
		/// </seealso>
		public virtual int Compare(System.Object that)
		{
			Cursor r = (Cursor) that;
			return (Position - r.Position);
		}

		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
