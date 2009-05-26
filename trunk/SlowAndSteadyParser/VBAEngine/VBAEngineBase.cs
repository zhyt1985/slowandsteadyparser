using System;
using System.Diagnostics;
using System.Threading;

namespace SlowAndSteadyParser
{
	/// <summary>
	/// 基本的VBA脚本引擎对象
	/// </summary>
	/// <remarks>本对象实现了一个VBA脚本引擎的基本框架,
	/// 使用了外部对象 DelegateHelper 和 VBAWindowBase
	/// 使用了引用 Microsoft.VisualBasic.Vsa.dll 和 Microsoft.Vsa.dll
	/// </remarks>
	public class VBAEngineBase : Microsoft.VisualBasic.Vsa.VsaEngine , Microsoft.Vsa.IVsaSite, IDisposable
	{
        //Log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// 初始化脚本引擎
		/// </summary>
		public VBAEngineBase()
		{
            //生成随机的程序名称,以便得到不同命名空间
            this.AppName = this.AppName + SlowAndSteadyParser.RandomStr.GetRndStrOnlyFor(8, false, false);
            log.Debug("Engine Start:" + this.AppName);
			this.RootMoniker = this.AppName + "://vba";
			this.Site = this;
			this.RootNamespace = this.AppName + "runtimescript";
			this.GenerateDebugInfo = true;
			this.InitNew();
		}

		#region 对象属性设置代码群 ****************************************************************

		protected string strMainScriptText = null;
		/// <summary>
		/// 主模块源代码字符串
		/// </summary>
		public string MainScriptText
		{
			get{ return strMainScriptText ;}
			set{ strMainScriptText = value;}
		}

		protected bool bolEnable = true;
		/// <summary>
		/// 对象是否可用
		/// </summary>
		public bool Enable
		{
			get{ return bolEnable;}
			set{ bolEnable = value;}
		}
		protected string strAppName = "vbaenginebase";
		/// <summary>
		/// 应用名称
		/// </summary>
		public virtual string AppName 
		{
			get{ return strAppName ;}
			set{ strAppName = value;}
		}
		
		/// <summary>
		/// 引擎引用的外部引用DLL名称列表
		/// </summary>
		protected virtual string[] RefrenceDllNames
		{
			get
			{
				return new string[]{
									   "system.dll",
									   "mscorlib.dll",
									   "system.xml.dll",
									   "system.data.dll",
									   "Microsoft.VisualBasic.dll"
								   };
			}
		}

		#endregion

		/// <summary>
		/// 输出一行调试信息
		/// </summary>
		/// <param name="txt">文本内容</param>
        //public virtual void DebugPrintLine( string txt )
        //{
        //    System.Console.WriteLine( txt );
        //}
		/// <summary>
		/// 显示错误消息框
		/// </summary>
		/// <param name="msg">错误消息</param>
        //public virtual void AlertError( string msg )
        //{
        //    System.Windows.Forms.MessageBox.Show( null , msg , "脚本运行错误" , System.Windows.Forms.MessageBoxButtons.OK , System.Windows.Forms.MessageBoxIcon.Exclamation );

        //}

        /// <summary>
		/// 将设计时的脚本代码转换为运行时脚本代码
		/// </summary>
		/// <param name="strModuleName">模块名称</param>
		/// <param name="strText">设计时脚本代码</param>
		/// <returns>运行时脚本代码</returns>
		protected virtual string ToRuntimeScriptText( string strModuleName , string strText )
		{
			return @"Option Strict Off
Imports System
Imports System.Data
Imports System.Data.Oledb
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Text.RegularExpressions
Imports System.Xml
Imports Microsoft.VisualBasic
Module " + strModuleName 
				+ @"
Sub Main()
" + strText	+ @"
End Sub
End Module";
            
		}

		public static int FixLineNumber( int LineNumber )
		{
			return LineNumber - 12 ;
		}
		/// <summary>
		/// 向引擎添加代码模块
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="SourceCode">源代码</param>
		/// <returns>操作是否成功</returns>
		public bool AddSourceCode( string ModuleName , string SourceCode )
		{
			if( HasContent( SourceCode ))
			{
				Microsoft.Vsa.IVsaCodeItem CodeItem = ( Microsoft.Vsa.IVsaCodeItem ) InnerCreateItem( ModuleName , Microsoft.Vsa.VsaItemType.Code , Microsoft.Vsa.VsaItemFlag.None) ;
				CodeItem.SourceText = this.ToRuntimeScriptText( ModuleName , SourceCode );
				return true;
			}
			return false;
		}		
		/// <summary>
		///向脚本引擎添加引用 
		/// </summary>
		/// <param name="strDllName">引用使用的DLL文件名</param>
		public bool AddRefrence( string strDllName )
		{
			try
			{
                //log.Debug("向脚本引擎添加引用" + strDllName );
				Microsoft.Vsa.IVsaReferenceItem RefItem =  InnerCreateItem( strDllName ,Microsoft.Vsa.VsaItemType.Reference , Microsoft.Vsa.VsaItemFlag.None )  as Microsoft.Vsa.IVsaReferenceItem ;
				RefItem.AssemblyName = strDllName ;
				return true;
			}
			catch(Exception ext)
			{
				log.Debug("向脚本引擎添加引用 " + strDllName + " 错误\r\n", ext);
			}
			return false;
		}

		protected Microsoft.Vsa.IVsaItem InnerCreateItem( string vName , Microsoft.Vsa.VsaItemType vType , Microsoft.Vsa.VsaItemFlag vFlag )
		{
			Microsoft.Vsa.IVsaItem item = null ;
			for(int iCount = 0 ; iCount < this.Items.Count ; iCount ++)
			{
				if( this.Items[ iCount ].Name == vName )
				{
					item = this.Items[ iCount ];
					break;
				}
			}
			if( item == null)
				return this.Items.CreateItem( vName , vType , vFlag );
			if( myUnusedItems != null && myUnusedItems.Contains( item ))
				myUnusedItems.Remove( item );
			return item ;
		}
		/// <summary>
		/// 判断一个字符串是否有内容
		/// </summary>
		/// <param name="strData">字符串对象</param>
		/// <returns>若字符串不为空且存在非空白字符则返回True 否则返回False</returns>
		private bool HasContent( string strData )
		{
			if( strData != null && strData.Length > 0 )
			{
				foreach(char c in strData )
				{
					if( Char.IsWhiteSpace( c ) == false)
						return true;
				}
			}
			return false;
		}// bool HasContent()

		#region 全局对象处理代码群 ****************************************************************

		protected System.Collections.Hashtable myGlobalObjects = new System.Collections.Hashtable();
		/// <summary>
		/// 添加全局对象
		/// </summary>
		/// <param name="strName">对象名称</param>
		/// <param name="obj">对象引用</param>
		/// <param name="strTypeName">对象声明类型名称</param>
		public void AddGlobalObject( string strName , object obj , string strTypeName )
		{
			if( strName == null || strName.Trim().Length == 0 )
				throw new System.ArgumentException("对象名称不得为空");

			if( strTypeName == null)
				strTypeName = obj.GetType().FullName;

            if (!myGlobalObjects.ContainsKey(strName))
            {
                AddGlobalItem(strName, strTypeName);
            }

			myGlobalObjects[ strName ] = obj ;
		}

        protected virtual object InnerGetObject(string strName)
        {
            foreach (string strKey in myGlobalObjects.Keys)
            {
                if (string.Compare(strKey, strName, true) == 0)
                {
                    return myGlobalObjects[strKey];
                }
            }
            return null;
        }
		/// <summary>
		/// 向脚本添加全局对象
		/// </summary>
		/// <param name="strName">全局对象的名称</param>
		/// <param name="strTypeName">全局对象的类型名称</param>
		public void AddGlobalItem( string strName , string strTypeName)
		{
			Microsoft.Vsa.IVsaGlobalItem myGolItem = this.InnerCreateItem( strName,Microsoft.Vsa.VsaItemType.AppGlobal , Microsoft.Vsa.VsaItemFlag.None ) as Microsoft.Vsa.IVsaGlobalItem ;
			myGolItem.TypeString = strTypeName ;
		}

		#endregion

		#region 编译,启动和停止引擎的代码群 *******************************************************
		/// <summary>
		/// 未使用的模块列表
		/// </summary>
		protected System.Collections.ArrayList myUnusedItems = null;

		protected virtual void ResetItems()
		{
			// 添加标准引用库
			string[] dlls = this.RefrenceDllNames;
			foreach( string name in dlls )
				AddRefrence( name );
			if( HasContent( this.strMainScriptText ) )
				this.AddSourceCode("MainModule" , this.strMainScriptText );
		}

		/// <summary>
		/// 删除所有未使用的项目
		/// </summary>
		protected void RemoveUnusedItem()
		{
			if( myUnusedItems != null && myUnusedItems.Count > 0 )
			{
				foreach( object obj in myUnusedItems )
				{
					for(int iCount = this.Items.Count -1 ; iCount >= 0 ; iCount --)
					{
						if( this.Items[ iCount ] == obj )
						{
							this.Items.Remove( iCount );
							break;
						}
					}
				}
			}
		}
		/// <summary>
		/// 准备编译的事件
		/// </summary>
		public event System.EventHandler BeforeCompile = null;
		/// <summary>
		/// 准备编译,可重载该方法来设置脚本代码和全局对象
		/// </summary>
		/// <returns>操作是否成功</returns>
		protected virtual bool OnBeforeCompile()
		{
			if( BeforeCompile != null )
				BeforeCompile( this , null );
			return true;
		}
		/// <summary>
		/// 开始运行脚本
		/// </summary>
		protected virtual void StartRun()
		{
			RefreshMethodList();
		}
		/// <summary>
		/// 编译并启动脚本
		/// </summary>
		/// <returns>编译是否成功</returns>
		public bool CompileAndRun()
		{
			if( this.bolEnable == false)
			{
				return false;
			}
			this.StopScript(); //中断运行的脚本!
			myCompilerError = null;
			this.myGlobalObjects.Clear();
			this.ResetItems();
			if( this.OnBeforeCompile())
			{
				if( this.Compile())
				{
					this.Run();
					this.StartRun();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 停止运行脚本
		/// </summary>
		public virtual void StopScript()
		{
			if( this.IsRunning )
			{
				this.Reset();
				this.RevokeCache();
			}
		}

        public virtual void RefreshGlobeItem()
        {          
            this.StopScript();
            Thread.Sleep(200);
            this.myGlobalObjects.Clear();
            this.OnBeforeCompile();
            this.Run();
            this.StartRun();
        }

        public override void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

		#endregion

		#region VBA方法列表处理代码群 *************************************************************

		protected System.Collections.ArrayList myVBAMethods = new System.Collections.ArrayList();

		private class VBAScriptMethodItem
		{
			/// <summary>
			/// 模块名称
			/// </summary>
			public string ModuleName = null;
			/// <summary>
			/// 方法名称
			/// </summary>
			public string MethodName = null;
			/// <summary>
			/// 方法对象
			/// </summary>
			public System.Reflection.MethodInfo MethodObject = null;
			/// <summary>
			/// 方法返回值
			/// </summary>
			public System.Type ReturnType = null;
			/// <summary>
			/// 指向该方法的委托
			/// </summary>
			public System.Delegate MethodDelegate = null;
		}
		/// <summary>
		/// 刷新VBA方法列表
		/// </summary>
		protected void RefreshMethodList()
		{
			myVBAMethods.Clear();
			System.Reflection.Assembly myAssembly = this.Assembly ;
			foreach( object obj in this.Items )
			{
				if( obj is Microsoft.Vsa.IVsaCodeItem )
				{
					Microsoft.Vsa.IVsaCodeItem CodeItem = ( Microsoft.Vsa.IVsaCodeItem ) obj ;
					System.Type t = myAssembly.GetType( this.RootNamespace + "." + CodeItem.Name );
					System.Reflection.MethodInfo[] ms = t.GetMethods( System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static );
					foreach( System.Reflection.MethodInfo m in ms )
					{
						VBAScriptMethodItem MethodItem = new VBAScriptMethodItem();
						myVBAMethods.Add( MethodItem );
						MethodItem.ModuleName = CodeItem.Name ;
						MethodItem.MethodName = m.Name ;
						MethodItem.MethodObject = m ;
						MethodItem.ReturnType = m.ReturnType ;
						if( m.GetParameters().Length == 0 )
						{
							System.Type dt = SlowAndSteadyParser.DelegateHelper.GetDelegateType( m.ReturnType );
							if( dt != null)
								MethodItem.MethodDelegate = System.Delegate.CreateDelegate( dt , m );
						}
					}//foreach
				}
			}
		}

		private VBAScriptMethodItem GetMethodItem( string ModuleName , string MethodName )
		{
			if( this.bolEnable == false )
				return null;
			foreach( VBAScriptMethodItem MethodItem in myVBAMethods)
			{
				if( string.Compare( MethodItem.MethodName , MethodName ,true ) == 0 )
				{
					if( ModuleName == null || string.Compare( MethodItem.ModuleName , ModuleName , true ) == 0 )
					{
						return MethodItem ;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 获得所有定义的方法
		/// </summary>
		/// <returns>方法数组</returns>
		public System.Reflection.MethodInfo[]  GetScriptMethods()
		{
			System.Collections.ArrayList myList = new System.Collections.ArrayList();
			foreach( VBAScriptMethodItem MethodItem in myVBAMethods)
				myList.Add( MethodItem.MethodObject );
			return ( System.Reflection.MethodInfo[]) myList.ToArray( typeof( System.Reflection.MethodInfo ));
		}
		/// <summary>
		/// 获得指定模块中所有定义的方法
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <returns>方法数组</returns>
		public System.Reflection.MethodInfo[]  GetScriptMethods( string ModuleName )
		{
			System.Collections.ArrayList myList = new System.Collections.ArrayList();
			foreach( VBAScriptMethodItem MethodItem in myVBAMethods)
			{
				if( string.Compare( MethodItem.ModuleName , ModuleName ) == 0 )
					myList.Add( MethodItem.MethodObject );
			}
			return ( System.Reflection.MethodInfo[]) myList.ToArray( typeof( System.Reflection.MethodInfo ));
		}

		#endregion

		#region 查找和调用指定脚本方法的函数群 ****************************************************
		/// <summary>
		/// 判断是否定义了指定模块的指定名称的方法,模块名称和方法名称不区分大小写
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <returns>是否定义了指定方法</returns>
		public bool HasScriptMethod( string ModuleName , string MethodName )
		{
			return  GetMethodItem( ModuleName , MethodName ) != null;
		}
		/// <summary>
		/// 判断是否定义了指定名称的方法,方法名称不区分大小写
		/// </summary>
		/// <param name="MethodName">方法名称</param>
		/// <returns>是否定义了指定的方法</returns>
		public bool HasScriptMethod( string MethodName )
		{
			return GetMethodItem( null , MethodName ) != null;
		}
		/// <summary>
		/// 获得指定模块和指定名称的脚本方法对象,模块名称和方法名称不区分大小写
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <returns>脚本方法对象</returns>
		public System.Reflection.MethodInfo GetScriptMethod( string ModuleName , string MethodName )
		{
			VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
			if( MethodItem == null)
				return null;
			else
				return MethodItem.MethodObject ;
		}
		/// <summary>
		/// 获得指定名称的脚本方法对象,方法名称不区分大小写
		/// </summary>
		/// <param name="MethodName">方法名称</param>
		/// <returns>脚本方法对象</returns>
		public System.Reflection.MethodInfo GetScriptMethod( string MethodName )
		{
			VBAScriptMethodItem MethodItem = GetMethodItem( null , MethodName );
			if( MethodItem == null)
				return null;
			else
				return MethodItem.MethodObject ;
		}
		/// <summary>
		/// 无参数的执行指定名称的脚本方法并返回方法返回值,方法名称不区分大小写
		/// </summary>
		/// <param name="MethodName">方法名称</param>
		/// <returns>若执行成功则返回方法返回值,否则返回空引用</returns>
		public object RunScriptMethod( string MethodName )
		{
                VBAScriptMethodItem MethodItem = GetMethodItem(null, MethodName);
                if (MethodItem == null || MethodItem.MethodDelegate == null)
                    return null;
                else                    
                    return MethodItem.MethodDelegate.DynamicInvoke(null);
		}
		/// <summary>
		/// 无参数的执行指定名称的脚本方法并返回方法返回值,模块名称和方法名称不区分大小写
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法</param>
		/// <returns>若执行成功则返回方法返回值,否则返回空引用</returns>
		public object RunScriptMethod( string ModuleName , string MethodName )
		{
			VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
			if( MethodItem == null || MethodItem.MethodDelegate == null )
				return null;
			else
				return MethodItem.MethodDelegate.DynamicInvoke( null );		}
		/// <summary>
		/// 无参数的执行指定模块指定名称的脚本方法并返回方法返回值,模块名称和方法名称不区分大小写
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <param name="obj">调用的对象</param>
		/// <param name="Parameters">参数数组</param>
		/// <returns>若执行成功则返回方法返回值，否则返回空引用</returns>
		public object RunScriptMethod( string ModuleName , string MethodName , object obj , object[] Parameters )
		{
			VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
			if( MethodItem == null)
				return null;
			else
			{
				if( MethodItem.MethodDelegate != null)
					return MethodItem.MethodDelegate.DynamicInvoke( null );
				else
					return MethodItem.MethodObject.Invoke(obj , Parameters);
			}
		}
		/// <summary>
		/// 无参数的执行指定名称的脚本方法并返回方法返回值,模块名称和方法名称不区分大小写
		/// </summary>
		/// <param name="MethodName">方法名称</param>
		/// <param name="obj">调用的对象</param>
		/// <param name="Parameters">参数数组</param>
		/// <returns>若执行成功则返回方法返回值，否则返回空引用</returns>
		public object RunScriptMethod( string MethodName , object obj , object[] Parameters )
		{
			return RunScriptMethod( null , MethodName , obj , Parameters );
		}
		/// <summary>
		/// 无参数的执行指定模块指定名称代码方法并返回字符串值,模块名称和方法名称不区分大小写
		/// 若方法不存在或执行错误则返回默认值
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <param name="DefaultValue">默认值</param>
		/// <returns>执行结果</returns>
		public string GetScriptStringValue( string ModuleName , string MethodName , string DefaultValue )
		{
			if( this.bolEnable )
			{
				VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
				if( MethodItem != null && MethodItem.MethodDelegate != null)
				{
					try
					{
						return Convert.ToString( MethodItem.MethodDelegate.DynamicInvoke( null ));
					}
					catch{}
				}
			}
			return DefaultValue ;
		}
		/// <summary>
		/// 无参数的执行指定模块指定名称代码方法并返回整数值,模块名称和方法名称不区分大小写
		/// 若方法不存在或执行错误则返回默认值
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <param name="DefaultValue">默认值</param>
		/// <returns>执行结果</returns>
		public int GetScriptInt32Value( string ModuleName , string MethodName , int DefaultValue )
		{
			if( this.bolEnable )
			{
				VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
				if( MethodItem != null && MethodItem.MethodDelegate != null)
				{
					try
					{
						return Convert.ToInt32( MethodItem.MethodDelegate.DynamicInvoke( null ));
					}
					catch{}
				}
			}
			return DefaultValue ;
		}
		/// <summary>
		/// 无参数的执行指定模块指定名称代码方法并返回双精度浮点数值,模块名称和方法名称不区分大小写
		/// 若方法不存在或执行错误则返回默认值
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <param name="DefaultValue">默认值</param>
		/// <returns>执行结果</returns>
		public double GetScriptDoubleValue( string ModuleName , string MethodName , double DefaultValue )
		{
			if( this.bolEnable )
			{
				VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
				if( MethodItem != null && MethodItem.MethodDelegate != null)
				{
					try
					{
						return Convert.ToDouble( MethodItem.MethodDelegate.DynamicInvoke( null ));
					}
					catch{}
				}
			}
			return DefaultValue ;
		}
		/// <summary>
		/// 无参数的执行指定模块指定名称代码方法并返回单精度浮点数值,模块名称和方法名称不区分大小写
		/// 若方法不存在或执行错误则返回默认值
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <param name="DefaultValue">默认值</param>
		/// <returns>执行结果</returns>
        public float GetScriptSingleValue(string ModuleName, string MethodName, float DefaultValue)
		{
			if( this.bolEnable )
			{
				VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
				if( MethodItem != null && MethodItem.MethodDelegate != null)
				{
					try
					{
						return Convert.ToSingle( MethodItem.MethodDelegate.DynamicInvoke( null ));
					}
					catch{}
				}
			}
			return DefaultValue ;
		}
		/// <summary>
		/// 无参数的执行指定模块指定名称代码方法并返回布尔值,模块名称和方法名称不区分大小写
		/// 若方法不存在或执行错误则返回默认值
		/// </summary>
		/// <param name="ModuleName">模块名称</param>
		/// <param name="MethodName">方法名称</param>
		/// <param name="DefaultValue">默认值</param>
		/// <returns>执行结果</returns>
		public bool GetScriptBooleanValue( string ModuleName , string MethodName , bool DefaultValue )
		{
			if( this.bolEnable )
			{
				VBAScriptMethodItem MethodItem = GetMethodItem( ModuleName , MethodName );
				if( MethodItem != null && MethodItem.MethodDelegate != null)
				{
					try
					{
						return Convert.ToBoolean( MethodItem.MethodDelegate.DynamicInvoke( null ));
					}
					catch{}
				}
			}
			return DefaultValue ;
		}

 		#endregion

		#region IVsaSite 成员 *********************************************************************

		/// <summary>
		/// 内部方法,不要调用
		/// </summary>
		public object GetEventSourceInstance(string itemName, string eventSourceName)
		{
			return InnerGetObject( itemName );
		}

		/// <summary>
		/// 内部方法,不要调用
		/// </summary>
		public object GetGlobalInstance(string name)
		{
			return InnerGetObject( name );
		}

		/// <summary>
		/// 内部方法,不要调用
		/// </summary>
		public void Notify(string notify, object info)
		{
			// TODO:  添加 XDesignerVBAScriptEngine.Notify 实现
		}

		/// <summary>
		/// 内部方法,不要调用
		/// </summary>
		public bool OnCompilerError(Microsoft.Vsa.IVsaError error)
		{
			myCompilerError = new CompilerErrorInfo();
			myCompilerError.SourceItem = error.SourceItem ;
			if( error.SourceItem is Microsoft.Vsa.IVsaCodeItem )
				myCompilerError.ModuleName = error.SourceItem.Name ;
			myCompilerError.Line = FixLineNumber( error.Line );
			myCompilerError.ErrorDescription = error.Description ;
			myCompilerError.StartColumn = error.StartColumn ;
			myCompilerError.EndColumn = error.EndColumn ;
			myCompilerError.LineText = error.LineText ;
			myCompilerError.Severity = error.Severity ;			
            log.Fatal("编译错误:" + myCompilerError.ToString());
			return false;
		}

		/// <summary>
		/// 内部方法,不要调用
		/// </summary>
		public void GetCompiledState(out byte[] pe, out byte[] debugInfo)
		{
			pe = null;
			debugInfo = null;
		}

		#endregion

		#region 编译错误信息处理代码群 ************************************************************

		protected CompilerErrorInfo myCompilerError = null;
		/// <summary>
		/// 编译错误信息对象
		/// </summary>
		public CompilerErrorInfo CompilerError
		{
			get{ return myCompilerError ;}
		}
		/// <summary>
		/// 编译错误信息对象
		/// </summary>
		public class CompilerErrorInfo
		{
			public Microsoft.Vsa.IVsaItem SourceItem = null;
			/// <summary>
			/// 发生编译错误的模块名称
			/// </summary>
			public string ModuleName = null;
			/// <summary>
			/// 错误提示文本
			/// </summary>
			public string ErrorDescription = null;
			/// <summary>
			/// 发生错误的行号
			/// </summary>
			public int Line = 0 ;
			/// <summary>
			/// 发生错误的列号
			/// </summary>
			public int StartColumn = 0 ;
			/// <summary>
			/// 发生错误的结束列号
			/// </summary>
			public int EndColumn = 0 ;
			/// <summary>
			/// 编译错误所在行的代码文本
			/// </summary>
			public string LineText = null;
			/// <summary>
			/// 错误严重程度
			/// </summary>
			public int Severity = 0 ;

            public override string ToString()
            {
                return "第 " + this.Line + " 行脚本发生编译错误:" + this.ErrorDescription + "\r\n该行代码为:" + this.LineText.Trim();
            }
		}//public class CompilerErrorInfo

		#endregion

        #region IDisposable 成员

        private bool IsDisposed=false;

        public void Dispose()  
        {  
            Dispose(true);
            GC.SuppressFinalize(this);  
        }

        protected override void Dispose(bool Disposing)  
        {  
            if(!IsDisposed)  
            {  
                if(Disposing)  
                {
                    try
                    {
                        //清理托管资源
                        //base.Close();
                        log.Debug("Engine Close:" + this.AppName);
                    }
                    catch (System.StackOverflowException e)
                    {
                    }
                }  
                //清理非托管资源
            }  
            IsDisposed=true;  
        }

        ~VBAEngineBase()
        {  
            Dispose(false);  
        } 

        #endregion
    }
}