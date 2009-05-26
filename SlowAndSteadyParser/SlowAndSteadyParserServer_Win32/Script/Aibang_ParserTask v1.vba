'//////// 数据解析脚本 For Aibang /////////
'//////// Ver b1.0            /////////
'===== Scirpt Start ======

Dim Total As String
Dim AibangURL As String
Dim Name As String
Dim Phone As String
Dim Address As String
Dim WWW As String
Dim NewURL As String
Dim Count As Integer
Dim IsNextPage As Boolean

Web.Silent = True
Web.DownloadImages = False
Web.RunActiveX = False
Web.DownloadScripts = False
Web.Navigate(Task.URL)	

'第一步，取得该关键词下所有POI条数

'设定当前查找范围设定为所有DIV
Web.ElementPointPushRangeByTagName("div")
	'找到搜索结果外框
	Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("sort_result"))
	'设定内部游标
	Web.ElementPointPushRangeChildren()
		'向下移动4次
		Web.ElementPointMoveNext(4)
		'得到全部搜索结果数量
		Total = Web.ElementPointInnerText
		Log.Debug("[Total]:"+ Total)
	'弹出内部游标
	Web.ElementPointPopRange()
'弹出Body
Web.ElementPointPopRange()

'第二步,循环所有可用页面
Do
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
					AibangURL = Task.ConvertToAbsoluteURL(Web.GetCurrentURL(), Web.ElementPointHref)
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

			'输出结果到Log
			Log.Debug("[Name]:"+ Name)
			Log.Debug("[PhoneNumber]:"+ Phone)
			Log.Debug("[Address]:"+ Address)
			Log.Debug("[Web]:"+ WWW)
			Log.Debug("[AibangURL]:" + AibangURL)
			'计数器增加
			Count = Count + 1
		End While
	'弹出
	Web.ElementPointPopRange()
	
	'查找下一页链接
	Web.ElementPointPushRangeLinks()
		Web.ElementPointMoveNextCondition(Web.ElementSeekInnerText("下一页", true))
		'从相对URL转换到绝对URL
		NewURL = Task.ConvertToAbsoluteURL(Web.GetCurrentURL(), Web.ElementPointHref)
		Log.Debug("[New URL!!!]:"+NewURL)
		'如果存在下一页链接,就转到该页
		If Not Web.ElementPointIsNull() Then
			Web.Navigate(NewURL)
			IsNextPage = True
		Else
			IsNextPage = False
		End If
	Web.ElementPointPopRange()
	
'循环到没有"下一页"这个链接(说明到头了)
Loop While IsNextPage
Log.Debug("[Count];"+Str(Count))
'===== Scirpt End ======
