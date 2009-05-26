'//////// 数据入库脚本 For RLF /////////
'//////// Ver 1            /////////
'===== Scirpt Start ======
Dim Name As String
Dim Person As String
Dim Relationship As String
Dim URL As String
Dim Head As String
Dim HeadDetail As String
Dim Occupation As String
Dim OccupationDetail As String

Dim Relationlist As List(Of NameValueCollection)
Dim Headlist As List(Of NameValueCollection)
Dim Occupationlist As List(Of NameValueCollection)

Dim strsqlcmd As String
Dim cn As OleDbConnection
Dim cmd As OleDbCommand

'从Task中取得数据
Name = Task.GetHashString("Name")
Relationlist = Task.GetHashValue("Relationlist")
Headlist = Task.GetHashValue("Headlist")
Occupationlist = Task.GetHashValue("Occupationlist")

Try
	'打开数据库连接
	cn = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=RLF.MDB;")
	cn.Open()
	'从Relationlist中取得每一份Relation
	For Each Relation As NameValueCollection In Relationlist
		'将数据存入变量
		Person = Relation("Person")
		Relationship = Relation("Relationship")
		URL = Relation("URL")
			
		'插入person表
		strsqlcmd = "INSERT INTO Person(Name,URL, isused, isfinished, lasttime) VALUES('"+Person+"','"+URL+"',0,0,Now())"
		Try	
			cmd = New OleDbCommand(strsqlcmd, cn)
			cmd.ExecuteNonQuery()
		Catch ex As System.Data.OleDb.OleDbException
			Log.Info("插入person表有重复值:"+Person)
		Catch ex As Exception
			Log.Error("插入person表异常错误:", ex)
		End Try
		
		'插入relationship表
		strsqlcmd = "INSERT INTO Relationship(Person1,Person2,Relation) VALUES('"+Name+"','"+Person+"','"+Relationship+"')"
		Try	
			cmd = New OleDbCommand(strsqlcmd, cn)
			cmd.ExecuteNonQuery()
		Catch ex As System.Data.OleDb.OleDbException
			Log.Info("插入relationship表有重复值:"+Name+"|"+Person)
		Catch ex As Exception
			Log.Error("插入relationship表异常错误:", ex)
		End Try
	Next
	
	'从Occupationlist中取得每一份Head
	For Each OccupationData As NameValueCollection In Occupationlist
		'将数据存入变量
		Occupation = OccupationData("Occupation")
		OccupationDetail = OccupationData("OccupationDetail")			
		
		'插入head表
		strsqlcmd = "INSERT INTO Occupation(Name,Occupation,Detail) VALUES('"+Name+"','"+Occupation+"','"+OccupationDetail+"')"
		Try	
			cmd = New OleDbCommand(strsqlcmd, cn)
			cmd.ExecuteNonQuery()
		Catch ex As System.Data.OleDb.OleDbException
			Log.Info("插入Occupation表有重复值:"+Name+"|"+Occupation)
		Catch ex As Exception
			Log.Error("插入Occupation表异常错误:", ex)
		End Try
	Next
	
	'从Headlist中取得每一份Head
	For Each HeadData As NameValueCollection In Headlist
		'将数据存入变量
		Head = HeadData("Head")
		HeadDetail = HeadData("HeadDetail")			
		
		'插入head表
		strsqlcmd = "INSERT INTO Head(Name,Head,Detail) VALUES('"+Name+"','"+Head+"','"+HeadDetail+"')"
		Try	
			cmd = New OleDbCommand(strsqlcmd, cn)
			cmd.ExecuteNonQuery()
		Catch ex As System.Data.OleDb.OleDbException
			Log.Info("插入Head表有重复值:"+Name+"|"+Head)
		Catch ex As Exception
			Log.Error("插入Head表异常错误:", ex)
		End Try
	Next
		
		
	'更新seed数据
	Try
		cmd = New OleDbCommand("UPDATE [person] SET [isused]=1,[isfinished]=1, [lasttime]=Now() WHERE [Name] = '"+Name+"'", cn)
		cmd.ExecuteNonQuery()
	Catch ex As Exception
		Log.Error("更新seed表异常:", ex)
	End Try
Catch ex As Exception
	Log.Error("打开db出错:", ex)
Finally	
	cn.Close()
End Try
'===== Scirpt End ======