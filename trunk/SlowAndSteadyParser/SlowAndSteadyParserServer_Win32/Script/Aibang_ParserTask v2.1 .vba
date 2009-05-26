'//////// 数据解析脚本 For Aibang /////////
'//////// Ver 2.1            /////////
'===== Scirpt Start ======
Dim AibangURL As String
Dim Name As String
Dim Phone As String
Dim Address As String
Dim WWW As String
Dim NewURL As String
Dim Count As Integer
Dim IsNextPage As Boolean
Dim Data As New NameValueCollection
Dim Datalist As New List(Of NameValueCollection)
Dim AibangIdParser As NameValueCollection
Dim AibangId As String

Web.Silent = True
Web.DownloadImages = False
Web.RunActiveX = False
Web.DownloadScripts = False
Web.Navigate("http://www.aibang.com")
Web.NavigateDelay(Task.URL, 16000)

'循环所有可用页面
Do
	'检查是否弹出验证码/Ban IP
	If Web.Title = "爱帮生活搜索-提示" Then
		Task.TaskRestartADSL()
		Return
	End If
	
	'设定当前范围为所有td对象
	Web.ElementPointPushRangeByTagName("td")
	
		'找class = td_top同时含有"select_biz"的td对象
		While Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("td_top")+Web.ElementSeekInnerHTML("select_biz", false))				
			'设定内部游标
			Web.ElementPointPushRangeChildren()
				'移动到<Span>
				Web.ElementPointMoveNext()
				'设定内部游标
				Web.ElementPointPushRangeChildren()				
					'移动到<A>(店名)
					Web.ElementPointMoveNext()					
					'取得店名
					Name = Web.ElementPointInnerText
					'取得店URL
					AibangURL = Task.ConvertHrefToAbsoluteURL(Web.GetCurrentURL(), Web.ElementPointHref)
					'从AibangURL中提取AibangId
					AibangIdParser = Task.ConvertURLToQueryString(AibangURL)
					AibangId = AibangIdParser("id")
				'弹出
				Web.ElementPointPopRange()
			'弹出
			Web.ElementPointPopRange()
			
			'移动到下一个td_top
			Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("td_top"))
			'设定内部游标
			Web.ElementPointPushRangeChildren()
				'清理变量
				Phone = ""
				WWW = ""
				Address = ""
				While Web.ElementPointMoveNext()
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
				End While
			'弹出
			Web.ElementPointPopRange()
			'剩下的是地址
			Address = Web.ElementPointInnerText
			'输出结果
			Log.Debug("[Name]:"+ Name)
			Data.Add("Name", Name)
			Log.Debug("[PhoneNumber]:"+ Phone)
			Data.Add("Phone", Phone)
			Log.Debug("[Address]:"+ Address)
			Data.Add("Address", Address)
			Log.Debug("[Web]:"+ WWW)
			Data.Add("Web", WWW)
			Log.Debug("[AibangId]:" + AibangId)
			Data.Add("AibangId", AibangId)
			'计数器增加
			Count = Count + 1
			'将当前Poi数据存入datalist数组
			Datalist.Add(Data)
			'设定新的Data
			Data = New NameValueCollection()
		End While
	'弹出
	Web.ElementPointPopRange()
	
	'查找下一页链接
	Web.ElementPointPushRangeLinks()
		Web.ElementPointMoveNextCondition(Web.ElementSeekInnerText("下一页", true))
		'从相对URL转换到绝对URL
		NewURL = Task.ConvertHrefToAbsoluteURL(Web.GetCurrentURL(), Web.ElementPointHref)
		Log.Debug("[New URL!!!]:"+NewURL)
		'如果存在下一页链接,就转到该页
		If Not Web.ElementPointIsNull() Then
			Web.NavigateDelay(NewURL, 16000)
			IsNextPage = True
		Else
			IsNextPage = False
		End If
	Web.ElementPointPopRange()
'循环到没有"下一页"这个链接(说明到头了)
Loop While IsNextPage
Log.Debug("[Count];"+Str(Count))
'把抓取得到的所有数据存入Task
Task.SetHashValue("Count",Count)
Task.SetHashValue("Datalist", Datalist)
'===== Scirpt End ======
