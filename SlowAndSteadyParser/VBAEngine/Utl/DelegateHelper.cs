using System;

namespace SlowAndSteadyParser
{
	/// <summary>
	/// 处理委托操作的例程模块,本对象不可实例化
	/// </summary>
	/// <remarks>编制 yyf 2006</remarks>
	public sealed class DelegateHelper
	{
		/// <summary>
		/// 无返回值的委托类型
		/// </summary>
		public delegate void	VoidDelegate();
		/// <summary>
		/// 返回长整型数据的委托类型
		/// </summary>
		public delegate long	Int64Delegate();
		/// <summary>
		/// 返回整型数据的委托类型
		/// </summary>
		public delegate int		Int32Delegate();
		/// <summary>
		/// 返回断整形的委托类型
		/// </summary>
		public delegate short   Int16Delegate();
		/// <summary>
		/// 返回无符号长整型的委托类型
		/// </summary>
		public delegate ulong   UInt64Delegate();
		/// <summary>
		/// 返回无符号整数的委托类型
		/// </summary>
		public delegate uint	UInt32Delegate();
		/// <summary>
		/// 返回无符号短整数的委托类型
		/// </summary>
		public delegate ushort	UInt16Delegate();
		/// <summary>
		/// 返回单个字节的委托类型
		/// </summary>
		public delegate byte	ByteDelegate();
		/// <summary>
		/// 返回单个有符号字节的委托类型
		/// </summary>
		public delegate sbyte	SByteDelegate();
		/// <summary>
		/// 返回字节数组的委托类型
		/// </summary>
		public delegate byte[]  BytesDelegate();
		/// <summary>
		/// 返回双精度浮点数的委托类型
		/// </summary>
		public delegate double	DoubleDelegate();
		/// <summary>
		/// 返回单精度浮点数的委托类型
		/// </summary>
		public delegate float	SingleDelegate();

		/// <summary>
		/// 返回布尔类型的委托类型
		/// </summary>
		public delegate bool	BooleanDelegate();
		/// <summary>
		/// 返回十进制数值的委托类型
		/// </summary>
		public delegate decimal DecimalDelegate();
		/// <summary>
		/// 返回单个字符的委托类型
		/// </summary>
		public delegate char	CharDelegate();
		/// <summary>
		/// 返回时间类型的委托类型
		/// </summary>
		public delegate System.DateTime DateTimeDelegate();
		/// <summary>
		/// 返回字符串的委托类型
		/// </summary>
		public delegate string	StringDelegate();
		/// <summary>
		/// 返回对象的委托类型
		/// </summary>
		public delegate object	ObjectDelegate();
		/// <summary>
		/// 返回类型的委托类型
		/// </summary>
		public delegate System.Type TypeDelegate();
		/// <summary>
		/// 返回流的委托类型
		/// </summary>
		public delegate System.IO.Stream StreamDelegate();
		/// <summary>
		/// 返回列表的委托类型
		/// </summary>
		public delegate System.Collections.ArrayList ArrayListDelegate();
		/// <summary>
		/// 返回集合的委托类型
		/// </summary>
		public delegate System.Collections.ICollection CollectionDelegate();
		/// <summary>
		/// 返回 IDisposable 的委托类型
		/// </summary>
		public delegate System.IDisposable DisposableDelegate();

//		public delegate System.Drawing.Point PointDelegate();
//		public delegate System.Drawing.Rectangle RectangleDelegate();
//		public delegate System.Drawing.Size SizeDelegate();

		/// <summary>
		/// 根据返回类型获得委托类型,若未支持该返回类型则返回空引用
		/// </summary>
		/// <param name="ReturnType">返回类型</param>
		/// <returns>委托类型</returns>
		public static System.Type GetDelegateType( System.Type ReturnType )
		{
			if( ReturnType == null)
				return null;

			System.Type DelegateType = null;
			
			if( ReturnType.Equals( typeof( void )))
				DelegateType = typeof( VoidDelegate );

			else if( ReturnType.Equals( typeof( long )))
				DelegateType = typeof( Int64Delegate );
			else if( ReturnType.Equals( typeof( int )))
				DelegateType = typeof( Int32Delegate ) ;
			else if( ReturnType.Equals( typeof( short)))
				DelegateType = typeof( Int16Delegate );
			else if( ReturnType.Equals( typeof( ulong )))
				DelegateType = typeof( UInt64Delegate );
			else if( ReturnType.Equals( typeof( uint )))
				DelegateType = typeof( UInt32Delegate ) ;
			else if( ReturnType.Equals( typeof( ushort)))
				DelegateType = typeof( UInt16Delegate );
			
			else if( ReturnType.Equals( typeof( string )))
				DelegateType = typeof( StringDelegate ) ;
			else if( ReturnType.Equals( typeof( double )))
				DelegateType = typeof( DoubleDelegate );
			else if( ReturnType.Equals( typeof( float )))
				DelegateType = typeof( SingleDelegate );
			else if( ReturnType.Equals( typeof( bool )))
				DelegateType = typeof( BooleanDelegate );
			else if( ReturnType.Equals( typeof( decimal )))
				DelegateType = typeof( DecimalDelegate );
			else if( ReturnType.Equals( typeof( byte[])))
				DelegateType = typeof( BytesDelegate );
			else if( ReturnType.Equals( typeof( byte)))
				DelegateType = typeof( ByteDelegate );
			else if( ReturnType.Equals( typeof( sbyte)))
				DelegateType = typeof( SByteDelegate );

			else if( ReturnType.Equals( typeof( char )))
				DelegateType = typeof( CharDelegate );
			else if( ReturnType.Equals( typeof( System.DateTime )))
				DelegateType = typeof( DateTimeDelegate );
			else if( ReturnType.Equals( typeof( object )))
				DelegateType = typeof( ObjectDelegate );

			else if( ReturnType.Equals( typeof( System.Type )))
				DelegateType = typeof( TypeDelegate);
			else if( ReturnType.Equals( typeof( System.IO.Stream )))
				DelegateType = typeof( StreamDelegate );
			else if( ReturnType.Equals( typeof( System.Collections.ArrayList )))
				DelegateType = typeof( ArrayListDelegate );
			else if( ReturnType.Equals( typeof( System.Collections.ICollection )))
				DelegateType = typeof( CollectionDelegate );
			else if( ReturnType.Equals( typeof( System.IDisposable )))
				DelegateType = typeof( DisposableDelegate );

			return DelegateType ;

		}//public static System.Type GetDelegateType( System.Type ReturnType )

		public static System.Delegate CreateDelegate( System.Reflection.MethodInfo method , object obj )
		{
			if( method == null)
				return null;
			if( method.GetParameters().Length == 0 )
				return null;
			System.Delegate Result = null;
			System.Type DelegateType = GetDelegateType( method.ReturnType );
			
			if( DelegateType == null)
				return null;

			if( method.IsStatic )
			{
				Result = System.Delegate.CreateDelegate( DelegateType , method );
			}
			else
			{
				Result = System.Delegate.CreateDelegate( DelegateType , obj , method.Name );
			}
			return Result ;
		}

		private DelegateHelper(){}

	}//public sealed class DelegateHelper
}