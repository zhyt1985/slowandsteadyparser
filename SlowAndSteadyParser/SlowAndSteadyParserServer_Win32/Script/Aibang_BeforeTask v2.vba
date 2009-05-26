'//////// 任务生成脚本 For Aibang /////////
'//////// Ver 2.0             /////////
'===== Scirpt Start ======
Dim City As String
Dim Poi As String
Dim Category As String

Dim cn As OleDbConnection
Dim cmd As OleDbCommand
Dim dr As OleDbDataReader

Try
	cn = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=AibangPOI.MDB;")
	cn.Open()
	cmd = New OleDbCommand("SELECT TOP 1 * FROM [seed] WHERE [isused] = 0", cn)
	dr = cmd.ExecuteReader
	If dr.Read() Then
		City = dr.Item("city")
		Poi = dr.Item("poi")
		Category = dr.Item("category")
		'更新该行数据		
		cmd = New OleDbCommand("UPDATE [seed] SET [IsUsed]=1,[lasttime]=Now() WHERE [city] = '"+City+"' AND [Poi] = '"+Poi+"' AND [Category] = '"+Category+"'", cn)
		cmd.ExecuteNonQuery()
	Else
		'没有nonused的seed,考虑used,没finished的seed
		dr.Close()
		cmd = New OleDbCommand("SELECT * FROM [seed] WHERE [isused] = 1 AND [isfinished] = 0 AND DateDiff('d', [lasttime], NOW()) >= 1 ORDER BY [lasttime]", cn)
		dr = cmd.ExecuteReader
		If dr.Read() Then
			City = dr.Item("city")
			Poi = dr.Item("poi")
			Category = dr.Item("category")
			'更新该行数据		
			cmd = New OleDbCommand("UPDATE [seed] SET [IsUsed]=1,[lasttime]=Now() WHERE [city] = '"+City+"' AND [Poi] = '"+Poi+"' AND [Category] = '"+Category+"'", cn)
			cmd.ExecuteNonQuery()
		Else
			'没有可用的seed,标记该任务失败,不再执行后续任务
			Task.TaskFail()
		End If
	End If
Catch ex As Exception
	'Log.Error("DatabaseError in Aibang Beforetask",ex)
	Throw ex
Finally
	dr.Close()
	cn.Close()
End Try

If Not City Is Nothing Then
	'将基本信息存在Task中
	Task.SetHashValue("City", City)
	Task.SetHashValue("Poi", Poi)
	Task.SetHashValue("Category", Category)
	'生成任务URL
	Task.URl = "http://www.aibang.com/?area=bizsearch&cmd=bigmap&city=" + Utility.UnicodeToUTF8(City)
	Task.URL = Task.URL + "&a=" + Utility.UnicodeToUTF8(Poi) + "&q=" + Utility.UnicodeToUTF8(Category)
	Task.URL = Task.URL + "&as=5000&rc=2&frm=in_sc_rank_rst"
End If
'===== Scirpt End ======