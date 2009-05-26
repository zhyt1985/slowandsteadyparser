'//////// 数据解析脚本 For 人立方 /////////
'//////// Ver 1            /////////
'===== Scirpt Start ======
Dim Person As String
Dim Relationship As String
Dim URL As String
Dim Head As String
Dim HeadDetail As String
Dim Occupation As String
Dim OccupationDetail As String

Dim RelationData As NameValueCollection
Dim HeadData As NameValueCollection
Dim OccupationData As NameValueCollection
Dim Relationlist As New List(Of NameValueCollection)
Dim Headlist As New List(Of NameValueCollection)
Dim Occupationlist As New List(Of NameValueCollection)

Web.Silent = True
Web.DownloadImages = False
Web.RunActiveX = False
Web.DownloadScripts = False
Web.Navigate(Task.URL)

	'设定当前范围为all
	Web.ElementPointPushRangeAll()		
		'找tagname = div, class = title-bar同时含有"关系"的td对象
		While Web.ElementPointMoveNextCondition(Web.ElementSeekTagName("div")+Web.ElementSeekClassName("title-bar"))
		
			If Web.ElementPointInnerText = "关系" Then
				Web.ElementPointMoveNext()
				'设定内部游标
				Web.ElementPointPushRangeChildren()				
					'移动到<div class=item>
					While Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("item"))
						'设定新的容器
						RelationData = New NameValueCollection()
						'设定内部游标
						Web.ElementPointPushRangeChildren()				
							'移动到<div class=relationship>
							Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("relationship"))
							'取得关系名
							Relationship = Web.ElementPointInnerText
							If Relationship = "" Then
								Relationship = "无关系"
							End if
							'移动到<div class=title>
							Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("title"))
							'设定内部游标
							Web.ElementPointPushRangeChildren()				
								'移动到<a>
								Web.ElementPointMoveNext()
								'取得人名
								Person = Web.ElementPointInnerText
								'取得网址
								URL = Utility.ConvertHrefToAbsoluteURL(Web.GetCurrentURL(), Web.ElementPointHref)
							'弹出
							Web.ElementPointPopRange()
						'弹出
						Web.ElementPointPopRange()
						'将个人数据存入RelationData
						RelationData.Add("Person", Person)
						Log.Info("[Person]: "+ Person)
						RelationData.Add("Relationship", Relationship)
						Log.Info("[Relation]: "+ Relationship)
						RelationData.Add("URL", URL)
						Log.Info("[URL]: "+ URL)
						'加入List中
						Relationlist.Add(RelationData)
					End While
				'弹出
				Web.ElementPointPopRange()
			End If
			
			If Web.ElementPointInnerText = "头衔" Then
				'移动到 <div class=panel>
				Web.ElementPointMoveNext()
				'设定内部游标
				Web.ElementPointPushRangeChildren()				
					'移动到<div class=clip>
					While Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("clip"))						
						'设定新的容器
						HeadData = New NameValueCollection()
						'设定内部游标
						Web.ElementPointPushRangeChildren()				
							'移动到<div class=btitle>
							Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("btitle"))
							'取得头衔
							Head = Web.ElementPointInnerText
							'移动到内容
							Web.ElementPointMoveNext()
							'取得头衔具体内容
							HeadDetail = Web.ElementPointInnerText
						'弹出
						Web.ElementPointPopRange()			
						'将头衔数据存入HeadData
						HeadData.Add("Head", Head)
						Log.Info("[Head]: "+ Head)
						HeadData.Add("HeadDetail", HeadDetail)
						Log.Info("[HeadDetail]: "+ HeadDetail)
						'加入List中
						Headlist.Add(HeadData)
					End While
				'弹出
				Web.ElementPointPopRange()
			End If
			
			If Web.ElementPointInnerText = "机构职务" Then
				'移动到 <div class=panel>
				Web.ElementPointMoveNext()
				'设定内部游标
				Web.ElementPointPushRangeChildren()				
					'移动到<div class=clip>
					While Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("clip"))						
						'设定新的容器
						OccupationData = New NameValueCollection()
						'设定内部游标
						Web.ElementPointPushRangeChildren()				
							'移动到<div class=btitle>
							Web.ElementPointMoveNextCondition(Web.ElementSeekClassName("btitle"))
							'取得头衔
							Occupation = Web.ElementPointInnerText
							'移动到内容
							Web.ElementPointMoveNext()
							'取得头衔具体内容
							OccupationDetail = Web.ElementPointInnerText
						'弹出
						Web.ElementPointPopRange()			
						'将头衔数据存入HeadData
						OccupationData.Add("Occupation", Occupation)
						Log.Info("[Occupation]: "+ Occupation)
						OccupationData.Add("OccupationDetail", OccupationDetail)
						Log.Info("[OccupationDetail]: "+ OccupationDetail)
						'加入List中
						Occupationlist.Add(OccupationData)
					End While
				'弹出
				Web.ElementPointPopRange()
			End If
			
		End While
	'弹出
	Web.ElementPointPopRange()

Task.SetHashValue("Relationlist",Relationlist)
Task.SetHashValue("Headlist", Headlist)
Task.SetHashValue("Occupationlist", Occupationlist)
'===== Scirpt End ======
