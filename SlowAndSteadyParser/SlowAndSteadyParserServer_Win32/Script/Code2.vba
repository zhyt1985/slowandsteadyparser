
'//////// 数据解析脚本 For Aibang /////////
'//////// Ver 4.0             /////////
'//===Script Start===//
Dim Total As String
Dim Name As String
Dim Phone As String
Dim Address As String
Dim WWW As String
DIm NewURL As String


NewURL = "http://www.aibang.com/?area=bizsearch&cmd=bigmap&city=%E5%8C%97%E4%BA%AC&a=%E4%B8%9C%E5%8D%95&q=%E9%A4%90%E9%A6%86&as=5000&rc=2&apr=0%7C0&frm=in_sc_rank_rst&fd=4"
Web.Silent = True
Web.DownloadImages = False
Web.RunActiveX = False
Web.DownloadScripts = False
Web.Navigate(NewURL)	

Web.ElementPointReset()
'找到搜索结果外框
Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("div")+Web.ElementSeekClassName("sort_result"))
'设定内部循环指针
Web.ElementChildrenPointReset()
'内部指针移动四次
Web.ElementChildrenPointReset()
Web.ElementChildrenPointMoveNext()
Web.ElementChildrenPointMoveNext()
Web.ElementChildrenPointMoveNext()
'得到全部搜索结果数量
Total = Web.ElementChildrenPointInnerText
System.Diagnostics.Log.Debug("[Total]:"+ Total)

Do
	Web.ElementPointReset()
	'循环抓取店名
	While Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("a")+Web.ElementSeekClassName("select_biz"))
			'取得店名
			Name = Web.ElementPointInnerText
			'移动到信息区外部
			Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("td")+Web.ElementSeekClassName("td_top"))
			'设定内部循环指针
			Web.ElementChildrenPointReset()
			'清理变量
			Phone = ""
			WWW = ""
			Address = ""
			Do
				If Web.ElementChildrenPointTagName = "span" Then
					If Not Web.ElementChildrenPointRemoveHidden() Then
						Phone = Phone + Web.ElementChildrenPointInnerText
					End If
				ElseIf Web.ElementChildrenPointTagName = "a" And Web.ElementChildrenPointClassName = "s_www" Then
					WWW = Web.ElementChildrenPointInnerText
				End If
				'删掉内部元素
				Web.ElementChildrenPointOuterHTML = ""
			Loop While Web.ElementChildrenPointMoveNext()
			'剩下来的就是地址
			Address = Web.ElementPointInnerText
			'输出结果到调试窗口
			System.Diagnostics.Log.Debug("[Name]:"+ Name)
			System.Diagnostics.Log.Debug("[PhoneNumber]:"+ Phone)
			System.Diagnostics.Log.Debug("[Address]:"+ Address)
			System.Diagnostics.Log.Debug("[URL]:"+ WWW)
	End While
	'查找下一页链接
	Web.ElementPointReset()
	Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("a")+Web.ElementSeekInnerText("下一页", true))
	'从相对URL转换到绝对URL
	NewURL = Web.ConvertToAbsoluteURL(Web.CurrentURL(), Web.ElementPointHref)
	System.Diagnostics.Log.Debug("[New URL!!!]:"+NewURL)
	'如果存在下一页链接,就转到该页
	If Not Web.ElementPointIsNull() Then
		Web.Navigate(NewURL)
	End If
Loop While Not Web.ElementPointIsNull()
'//===Script End===//




'//////// 入库脚本 For Aibang /////////
'//////// Ver 4.0             /////////
'//===Script Start===//
Dim objConn As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=C:\ex\FrmSampl.mdb")

objConn.Open()

Dim objCmd As New OleDbCommand("SELECT * FROM tblOrders", objConn)
Dim objDataReader As OleDbDataReader = objCmd.ExecuteReader

objDataReader.Read()
Console.Write(objDataReader.Item("Customer") & ", " & objDataReader.Item("Employee"))

'//===Script End===//




'//////// 数据解析脚本 For Aibang /////////
'//////// Ver 5.0             /////////
'//===Script Start===//
Dim Total As String
Dim Name As String
Dim Phone As String
Dim Address As String
Dim WWW As String
DIm NewURL As String


NewURL = "http://www.aibang.com/?area=bizsearch&cmd=bigmap&city=%E5%8C%97%E4%BA%AC&a=%E4%B8%9C%E5%8D%95&q=%E9%A4%90%E9%A6%86&as=5000&rc=2&apr=0%7C0&frm=in_sc_rank_rst&fd=4"
Web.Silent = True
Web.DownloadImages = False
Web.RunActiveX = False
Web.DownloadScripts = False
Web.Navigate(NewURL)	

Web.ElementPointReset()
'找到搜索结果外框
Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("div")+Web.ElementSeekClassName("sort_result"))
'设定内部循环指针
Web.ElementChildrenPointReset()
'内部指针移动四次
Web.ElementChildrenPointReset()
Web.ElementChildrenPointMoveNext()
Web.ElementChildrenPointMoveNext()
Web.ElementChildrenPointMoveNext()
'得到全部搜索结果数量
Total = Web.ElementChildrenPointInnerText
Log.Info("[Total]:"+ Total)

Do
	Web.ElementPointReset()
	'循环抓取店名
	While Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("a")+Web.ElementSeekClassName("select_biz"))
			'取得店名
			Name = Web.ElementPointInnerText
			'移动到信息区外部
			Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("td")+Web.ElementSeekClassName("td_top"))
			'设定内部循环指针
			Web.ElementChildrenPointReset()
			'清理变量
			Phone = ""
			WWW = ""
			Address = ""
			Do
				If Web.ElementChildrenPointTagName = "span" Then
					If Not Web.ElementChildrenPointRemoveHidden() Then
						Phone = Phone + Web.ElementChildrenPointInnerText
					End If
				ElseIf Web.ElementChildrenPointTagName = "a" And Web.ElementChildrenPointClassName = "s_www" Then
					WWW = Web.ElementChildrenPointInnerText
				End If
				'删掉内部元素
				Web.ElementChildrenPointOuterHTML = ""
			Loop While Web.ElementChildrenPointMoveNext()
			'剩下来的就是地址
			Address = Web.ElementPointInnerText
			'输出结果到调试窗口
			Log.Info("[Name]:"+ Name)
			Log.Info("[PhoneNumber]:"+ Phone)
			Log.Info("[Address]:"+ Address)
			Log.Info("[URL]:"+ WWW)
	End While
	'查找下一页链接
	Web.ElementPointReset()
	Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("a")+Web.ElementSeekInnerText("下一页", true))
	'从相对URL转换到绝对URL
	NewURL = Web.ConvertToAbsoluteURL(Web.CurrentURL(), Web.ElementPointHref)
	Log.Info("[New URL!!!]:"+NewURL)
	'如果存在下一页链接,就转到该页
	If Not Web.ElementPointIsNull() Then
		Web.Navigate(NewURL)
	End If
Loop While Not Web.ElementPointIsNull()
'//===Script End===//




'//////// 入库脚本 For Aibang /////////
'//////// Ver 5.0             /////////
'//===Script Start===//
Dim objConn As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=C:\ex\FrmSampl.mdb")

objConn.Open()

Dim objCmd As New OleDbCommand("SELECT * FROM tblOrders", objConn)
Dim objDataReader As OleDbDataReader = objCmd.ExecuteReader

objDataReader.Read()
Console.Write(objDataReader.Item("Customer") & ", " & objDataReader.Item("Employee"))

'//===Script End===//

