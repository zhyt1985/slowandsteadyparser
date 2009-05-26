Public Sub Main()
	web.Silent = True
	web.DownloadImages = false
	web.RunActiveX = false
	web.DownloadScripts = false
	web.Navigate("about:blank")
	web.SetCacheCookie("xiaonei.com","","__utmb=204579609; mop_uniq_ckid=116.70.14.201_1228836851_1611523592; __utma=204579609.1312867846.1228836852.1228836852.1228836873.2; __utmz=204579609.1228836852.1.1.utmccn=(direct)|utmcsr=(direct)|utmcmd=(none); hostid=200137046; xn_app_histo_24726=12012-6-8-4-3-19-2-32-7; XNESSESSIONID=abcvAn8lHbvsqbpRPyC4r; __utmc=204579609; syshomeforreg=1; isnewreg=1; ick=abcvAn8lHbvsqbpRPyC4rbaboon.xiaonei.com; _de=leomao; userid=200137046; univid=1007; gender=1; univyear=2005; WebOnLineNotice_200137046=1; societyguester=c8f4a4b136ff424a4b88ecd08d0c27546; kl=346ca1d88fc35be3d42eee0a17a191a1_200137046; xn_app_histo_200137046=7-6-3-8-4-25-2-19; whichcookie-20480=OEMIBGAKFAAA; FSESSIONINS=163976714.20480.0000")
	web.Navigate("xiaonei.com")
    System.Diagnostics.Log.Debug(web.CurrentCookies)
    
End Sub

Public Sub Main()
    web.RunActiveX = false
	web.DownloadScripts = false
    web.Navigate("http://www.aibang.com/?addr=东方明珠&q=星巴克&area=bizsearch&cmd=noscript&script=false&city=上海")
    web.RemoveAllHiddenElements()
    System.Diagnostics.Log.Debug(web.GetCurrentWebReferer())
End Sub

Public Sub Main()
    web.RunActiveX = false
	web.DownloadScripts = false
    web.Navigate("http://www.aibang.com/?addr=东方明珠&q=星巴克&area=bizsearch&cmd=noscript&script=false&city=上海")
    web.RemoveAllHiddenElements()
    web.ElementClass.SeekElementById("ddd");
End Sub

Public Sub Main()
    '爱帮网抓取测试(2)
	Web.Silent = True
	Web.DownloadImages = False
	Web.RunActiveX = False
	Web.DownloadScripts = False
	Web.Navigate("http://www.aibang.com/?addr=东方明珠&q=星巴克&area=bizsearch&cmd=noscript&script=false&city=上海")
	Web.RemoveAllHiddenElements()
	Web.ElementPointReset()
	While Web.ElementPointMoveNext()
		If ( Web.ElementPointTagName ="a" And Web.ElementPointClassName = "select_biz") Then
			System.Diagnostics.Log.Debug("[Names]:"+Web.ElementPointInnerText)
			Exit While
		End If
	End While
End Sub

Public Sub Main()
    '爱帮网抓取测试(2.5)
	Web.Silent = True
	Web.DownloadImages = False
	Web.RunActiveX = False
	Web.DownloadScripts = False
	Web.Navigate("http://www.aibang.com/?addr=东方明珠&q=星巴克&area=bizsearch&cmd=noscript&script=false&city=上海")
	Web.RemoveAllHiddenElements()
	Web.ElementPointReset()
	While Web.ElementPointMoveNext()
			System.Diagnostics.Log.Debug("[Tag Names]:"+Web.ElementPointTagName)
			System.Diagnostics.Log.Debug("[Class Names]:"+Web.ElementPointClassName)
			System.Diagnostics.Log.Debug("[Text]:"+Web.ElementPointInnerText)
	End While
End Sub

Public Sub Main()
    '爱帮网抓取测试(v3)
	Web.Silent = True
	Web.DownloadImages = False
	Web.RunActiveX = False
	Web.DownloadScripts = False
	Web.Navigate("http://www.aibang.com/?addr=东方明珠&q=星巴克&area=bizsearch&cmd=noscript&script=false&city=上海")
	Web.RemoveAllHiddenElements()
	Web.ElementPointReset()
	'循环抓取店名
	While Web.ElementPointMoveNextMultiCondition("a", "select_biz", "")
			System.Diagnostics.Log.Debug("[Name]:"+Web.ElementPointInnerText)
			'查找电话号码字段
			Web.ElementPointMoveNextMultiCondition("td", "td_top", "")
			System.Diagnostics.Log.Debug("[PhoneNumber&Address]:"+Web.ElementPointInnerText)
	End While
End Sub

Public Sub Main()
    '爱帮网抓取测试(v3.5)
    Dim Total As String
    Dim Name As String
    Dim Phone As String
    Dim Address As String
    Dim WWW As String
        
	Web.Silent = True
	Web.DownloadImages = False
	Web.RunActiveX = False
	Web.DownloadScripts = False
	Web.Navigate("http://www.aibang.com/?area=bizsearch&cmd=bigmap&city=%E5%8C%97%E4%BA%AC&a=%E4%B8%AD%E5%9B%BD%E4%BC%A0%E5%AA%92%E5%A4%A7%E5%AD%A6&q=%E9%A4%90%E9%A6%86&input=3&frm=frt")
		
		Web.RemoveAllHiddenElements()
		Web.ElementPointReset()
		'循环抓取店名
		While Web.ElementPointMoveNextMultiCondition("a", "select_biz", "")
				'取得店名
				Name = Web.ElementPointInnerText
				'移动到信息区外部
				Web.ElementPointMoveNextMultiCondition("td", "td_top", "")
				'设定内部循环指针
				Web.ElementChildrenPointReset()
				'清理变量
				Phone = ""
				WWW = ""
				Address = ""
				Do
					If Web.ElementChildrenPointTagName = "span" Then
						Phone = Phone + Web.ElementChildrenPointInnerText
					ElseIf Web.ElementChildrenPointTagName = "a" And Web.ElementChildrenPointClassName = "s_www" Then
						WWW = Web.ElementChildrenPointInnerText
					End If
					'删掉内部元素
					Web.ElementChildrenPointOuterHTML = ""
				Loop While Web.ElementChildrenPointMoveNext()
				'剩下来的就是地址
				Address = Web.ElementPointInnerText
				System.Diagnostics.Log.Debug("[Name]:"+ Name)
				System.Diagnostics.Log.Debug("[PhoneNumber]:"+ Phone)
				System.Diagnostics.Log.Debug("[Address]:"+ Address)
				System.Diagnostics.Log.Debug("[URL]:"+ WWW)
		End While

End Sub

Public Sub Main()
    '爱帮网抓取测试(v3.7)
    Dim Total As String
    Dim Name As String
    Dim Phone As String
    Dim Address As String
    Dim WWW As String
        
	Web.Silent = True
	Web.DownloadImages = False
	Web.RunActiveX = False
	Web.DownloadScripts = False
	Web.Navigate("http://www.aibang.com/?area=bizsearch&cmd=bigmap&city=%E5%8C%97%E4%BA%AC&a=%E4%B8%AD%E5%9B%BD%E4%BC%A0%E5%AA%92%E5%A4%A7%E5%AD%A6&q=%E9%A4%90%E9%A6%86&input=3&frm=frt")
		
		Web.RemoveAllHiddenElements()
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
						Phone = Phone + Web.ElementChildrenPointInnerText
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

End Sub

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
Dim objConn As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=C:\Program Files\Microsoft Office\Office10\Samples\Northwind.mdb")

objConn.Open()

Dim objCmd As New OleDbCommand("SELECT * FROM Products", objConn)
Dim objDataReader As OleDbDataReader = objCmd.ExecuteReader

objDataReader.Read()
Console.Write(objDataReader.Item("ProductName") & ", " & objDataReader.Item("UnitsInStock"))

'//===Script End===//
    

