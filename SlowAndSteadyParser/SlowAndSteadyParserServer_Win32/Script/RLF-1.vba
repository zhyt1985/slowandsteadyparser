'//////// 任务生成脚本 For RLF /////////
'//////// Ver 2.0             /////////
'===== Scirpt Start ======
Dim Name As String

Dim cn As OleDbConnection
Dim cmd As OleDbCommand
Dim dr As OleDbDataReader

Try
	cn = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=RLF.MDB;")
	cn.Open()
	cmd = New OleDbCommand("SELECT TOP 1 * FROM [Person] WHERE [isused] = 0", cn)
	dr = cmd.ExecuteReader
	If dr.Read() Then
		Name = dr.Item("name")
		Task.URL = dr.Item("url")
		'更新该行数据		
		cmd = New OleDbCommand("UPDATE [person] SET [IsUsed]=1,[lasttime]=Now() WHERE [name] = '"+Name+"'", cn)
		cmd.ExecuteNonQuery()
	Else
		'没有可用的seed,标记该任务失败,不再执行后续任务
		Task.TaskFail()
	End If
Catch ex As Exception
	Throw ex
Finally
	If Not(dr Is Nothing) Then
		dr.Close()
	End If
	cn.Close()
End Try

If Not Name Is Nothing Then
	'将基本信息存在Task中
	Task.SetHashValue("Name", Name)
End If
'===== Scirpt End ======