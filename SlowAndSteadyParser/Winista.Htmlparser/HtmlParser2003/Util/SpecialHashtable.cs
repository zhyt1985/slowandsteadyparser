// ***************************************************************
//  SpecialHashtable   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary> Acts like a regular HashTable, except some values are translated in get(String).
	/// Specifically, <code>Tag.NULLVALUE</code> is translated to <code>null</code> and
	/// <code>Tag.NOTHING</code> is translated to <code>""</code>.
	/// This is done for backwards compatibility, users are expecting a HashTable,
	/// but Tag.toHTML needs to know when there is no attribute value (&lt;<TAG ATTRIBUTE&gt;)
	/// and when the value was not present (&lt;<TAG ATTRIBUTE=&gt;).
	/// </summary>
	[Serializable]
	public class SpecialHashtable:System.Collections.Hashtable
	{
		/// <summary> Special key for the tag name.</summary>
		public const System.String TAGNAME = "$<TAGNAME>$";
		
		/// <summary> Special value for a null attribute value.</summary>
		public const System.String NULLVALUE = "$<NULL>$";
		
		/// <summary> Special value for an empty attribute value.</summary>
		public const System.String NOTHING = "$<NOTHING>$";

		public SpecialHashtable() : base()
		{
		}

		/// <summary> Returns the value to which the specified key is mapped in this hashtable.
		/// This is translated to provide backwards compatibility.
		/// </summary>
		/// <returns> The translated value of the attribute. <em>This will be
		/// <code>null</code> if the attribute is a stand-alone attribute.</em>
		/// </returns>
		//UPGRADE_TODO: The following property was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Object this[System.Object key]
		{
			get
			{
				System.Object ret;
				
				ret = GetRaw(key);
				if ((System.Object) NULLVALUE == ret)
					ret = null;
				else if ((System.Object) NOTHING == ret)
					ret = "";
				
				return (ret);
			}
			
			set
			{
				base[key] = value;
			}
		}

		/// <summary> Constructs a new, empty hashtable with the specified initial capacity
		/// and default load factor, which is 0.75.
		/// </summary>
		public SpecialHashtable(int initialCapacity):base(initialCapacity)
		{
		}
		
		/// <summary> Constructs a new, empty hashtable with the specified initial capacity
		/// and the specified load factor.
		/// </summary>
		//UPGRADE_WARNING: Constructor 'java.util.Hashtable.Hashtable' was converted to 'System.Collections.Hashtable.Hashtable' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
		public SpecialHashtable(int initialCapacity, float loadFactor):base(initialCapacity, loadFactor)
		{
		}
		
		/// <summary> Returns the raw (untranslated) value to which the specified key is
		/// mapped in this hashtable.
		/// </summary>
		public virtual System.Object GetRaw(System.Object key)
		{
			return (base[key]);
		}
	}
}
