'//////// 预定义脚本 For Aibang /////////
'//////// Ver 5.0             /////////
Dim s1 As String
Dim s2 As String

s1 = "国贸"
s2 = "银行"


Task.URl = "http://www.aibang.com/?area=bizsearch&cmd=bigmap&city=%E5%8C%97%E4%BA%AC&a="
Task.URL = Task.URL + Task.URLUTF8Encoding(s1)
Task.URL = Task.URL + "&q="
Task.URL = Task.URL + Task.URLUTF8Encoding(s2)
Task.URL = Task.URL + "&as=5000&rc=2&apr=0%7C0&frm=in_sc_rank_rst&fd=4"


'//////// 数据解析脚本 For Aibang /////////
'//////// Ver 7.0             /////////
'//===Script Start===//
Dim Total As String
Dim AibangURL As String
Dim Name As String
Dim Phone As String
Dim Address As String
Dim WWW As String
Dim NewURL As String
Dim Count As Integer

Dim IsNext As Boolean

Web.Silent = True
Web.DownloadImages = False
Web.RunActiveX = False
Web.DownloadScripts = False
Web.Navigate(Task.URL)	

'第一步，取得该关键词下所有POI条数

'设定当前查找范围设定为所有DIV
Web.ElementPointRangeByTagName("div")
	'找到搜索结果外框
	Web.ElementPointPushMoveNextCondition(Web.ElementSeekClassName("sort_result"))
	'设定内部游标
	Web.ElementPointPushRangeChildren()
		'向下移动3次
		Web.ElementPointMoveNext(3)
		'得到全部搜索结果数量
		Total = Web.ElementPointInnerText
		Log.Debug("[Total]:"+ Total)
	'弹出内部游标
	Web.ElementPointPopRange()
'弹出Body
Web.ElementPointPopRange()

'循环页面
Do
	'设定当前范围为所有td对象
	Web.ElementPointPushRangeByTagName("td")
	'找class = td_top, 同时含有"<A class=select_biz"的td对象
	While Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("td_top")+Web.ElementSeekInnerHTML("<A class=select_biz", false))			
		'设定内部游标
		Web.ElementChildrenPush()
			'移动到店名
			Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("a")+Web.ElementSeekClassName("select_biz"))
			'取得店名
			Name = Web.ElementPointInnerText
			'取得店URL
			AibangURL = Task.ConvertToAbsoluteURL(Web.CurrentURL(), Web.ElementPointHref)
		'弹出
		Web.ElementPointPopRange()
		
		'移动到下一个td
		Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("td_top"))
		'设定内部游标
		Web.ElementChildrenPush()
			'清理变量
			Phone = ""
			WWW = ""
			Address = ""
			Do
				'如果是Span,则为电话号码
				If Web.ElementPointTagName = "span" Then
					'移除隐藏的电话干扰项
					If Not Web.ElementPointRemoveHidden() Then
						Phone = Phone + Web.ElementPointInnerText
					End If
				'是网址(链接)
				ElseIf Web.ElementPointTagName = "a" And Web.ElementPointClassName = "s_www" Then
					WWW = Web.ElementPointInnerText
				End If
				'删掉信息区内部元素
				Web.ElementPointOuterHTML = ""
			Loop While Web.ElementPointMoveNext()
			'弹出内部游标
			Web.ElementChildrenPop()
			'剩下的是地址
			Address = Web.ElementPointInnerText
		'弹出
		Web.ElementPointPopRange()
		
		'输出结果到Log
		Log.Debug("[Name]:"+ Name)
		Log.Debug("[PhoneNumber]:"+ Phone)
		Log.Debug("[Address]:"+ Address)
		Log.Debug("[Web]:"+ WWW)
		Log.Debug("[AibangURL]:" + AibangURL)
		
		'计数器增加
		Count = Count + 1

	End While
	
	'查找下一页链接
	Web.ElementPointPushRangeLinks()
		Web.ElementPointMoveNextCondition(Web.ElementSeekInnerText("下一页", true))
		'从相对URL转换到绝对URL
		NewURL = Task.ConvertToAbsoluteURL(Web.CurrentURL(), Web.ElementPointHref)
		Log.Debug("[New URL!!!]:"+NewURL)
		'如果存在下一页链接,就转到该页
		If Not Web.ElementPointIsNull() Then
			Web.Navigate(NewURL)
			IsNext = True
		Else
			IsNext = False
		End If
	Web.ElementPointPopRange()
	
'循环到没有"下一页"这个链接(说明到头了)
Loop While IsNext
Log.Debug("[Count];"+Str(Count))
'//===Script End===//






'//////// 入库脚本 For Aibang /////////
'//////// Ver 5.0             /////////
'<Script>

Dim objConn As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=C:\ex\FrmSampl.mdb")

objConn.Open()

Dim objCmd As New OleDbCommand("SELECT * FROM tblOrders", objConn)
Dim objDataReader As OleDbDataReader = objCmd.ExecuteReader

objDataReader.Read()
Console.Write(objDataReader.Item("Customer") & ", " & objDataReader.Item("Employee"))

'</Script>


'<Script>
'5.0 AIbang 老抓取部分
	
	'循环抓取店名
	While 
			'取得店名
			Name = Web.ElementPointInnerText
			'移动到信息区外部
			Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("td")+Web.ElementSeekClassName("td_top"))
			'设定内部游标
			Web.ElementChildrenPush()
			'清理变量
			Phone = ""
			WWW = ""
			Address = ""
			Do
				'如果是Span,则为电话号码
				If Web.ElementPointTagName = "span" Then
					'移除隐藏的电话干扰项
					If Not Web.ElementPointRemoveHidden() Then
						Phone = Phone + Web.ElementPointInnerText
					End If
				'是网址(链接)
				ElseIf Web.ElementPointTagName = "a" And Web.ElementPointClassName = "s_www" Then
					WWW = Web.ElementPointInnerText
				End If
				'删掉信息区内部元素
				Web.ElementPointOuterHTML = ""
			Loop While Web.ElementPointMoveNext()
			'弹出内部游标
			Web.ElementChildrenPop()
			'剩下的是地址
			Address = Web.ElementPointInnerText
			'输出结果到调试窗口
			Log.Debug("[Name]:"+ Name)
			Log.Debug("[PhoneNumber]:"+ Phone)
			Log.Debug("[Address]:"+ Address)
			Log.Debug("[URL]:"+ WWW)
			'计数器增加
			Count = Count + 1
	End While
	'查找下一页链接
	Web.ElementPointReset()
	Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("a")+Web.ElementSeekInnerText("下一页", true))
	'从相对URL转换到绝对URL
	NewURL = Task.ConvertToAbsoluteURL(Web.CurrentURL(), Web.ElementPointHref)
	Log.Debug("[New URL!!!]:"+NewURL)
	'如果存在下一页链接,就转到该页
	If Not Web.ElementPointIsNull() Then
		Web.Navigate(NewURL)
	End If
'循环到没有"下一页"这个链接(说明到头了)
Loop While Not Web.ElementPointIsNull()
Log.Debug("[Count];"+Str(Count))
</Script>