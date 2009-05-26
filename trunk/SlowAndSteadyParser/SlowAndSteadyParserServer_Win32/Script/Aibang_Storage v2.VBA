'//////// 数据入库脚本 For Aibang /////////
'//////// Ver 2            /////////
'===== Scirpt Start ======
Dim City As String
Dim Poi As String
Dim Category As String
Dim AibangId As String

Dim Name As String
Dim Phone As String
Dim Address As String
Dim WWW As String
Dim Count As Integer

Dim IsExist As Boolean

Dim Datalist As List(Of NameValueCollection)
Dim rnd As New Random()
Dim i As Double

Dim strsqlcmd As String
Dim cn As OleDbConnection
Dim cmd As OleDbCommand

'从Task中取得数据
City = Task.GetHashString("City")
Poi = Task.GetHashString("Poi")
Category = Task.GetHashString("Category")
Count = Task.GetHashInt("Count")
Datalist = Task.GetHashValue("Datalist")
i = 1
Try
	'打开数据库连接
	cn = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=AibangPOI.MDB;")
	cn.Open()
	'从Datalist中取得每一份Data
	For Each Data As NameValueCollection In Datalist
		'将数据存入变量
		Name = Data("Name")
		Phone = Data("Phone")
		Address = Data("Address")
		WWW = Data("WWW")
		AibangId = Data("AibangId")
		'插入poi表
		strsqlcmd = "INSERT INTO poi(AibangID,City,Name,Phone,Address,Web) VALUES('"+AibangId+"','"+City+"','"+Name+"','"+Phone+"','"+Address+"','"+WWW+"')"
		Try	
			IsExist = False	
			cmd = New OleDbCommand(strsqlcmd, cn)
			cmd.ExecuteNonQuery()
		Catch ex As System.Data.OleDb.OleDbException
			Log.Info("插入poi表有重复值:"+City+" "+Name)
			IsExist = True
		Catch ex As Exception
			Log.Error("插入poi表异常错误:", ex)
			IsExist = True
		End Try
		
		'生成seed插入seed表, 离原始poi越远的数据有越高的几率作为seed		
		If IsExist = False And rnd.NextDouble() < (i/Count)/3 Then
			strsqlcmd = "INSERT INTO seed(city, poi, category, isused, isfinished, lasttime) VALUES('"+City+"','"+Name+"','"+Category+"',0,0,Now())"
			Try		
				cmd = New OleDbCommand(strsqlcmd, cn)
				cmd.ExecuteNonQuery()
				Log.Info("生成新任务:"+City+"-"+Name+"-"+Category)
			Catch ex As System.Data.OleDb.OleDbException
				Log.Info("插入seed表有重复值:"+City+" "+Name)
			Catch ex As Exception
				Log.Error("插入seed表异常错误:", ex)
			End Try
		End If
		i = i + 1
	Next
	
	'更新seed数据
	Try
		cmd = New OleDbCommand("UPDATE [seed] SET [isused]=1,[isfinished]=1, [lasttime]=Now() WHERE [city] = '"+City+"' AND [Poi] = '"+Poi+"' AND [Category] = '"+Category+"'", cn)
		cmd.ExecuteNonQuery()
	Catch ex As Exception
		Log.Error("更新seed表异常:", ex)
	End Try
	'设置系统生成新任务
	Task.TaskGenerateNewTask()
Catch ex As Exception
	Log.Error("打开db出错:", ex)
Finally	
	cn.Close()
End Try


'===== Scirpt End ======
