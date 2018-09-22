Imports System.Text.RegularExpressions

Public Class partitionCalculator

    Dim names As New List(Of String)
    Dim types As New List(Of String)
    Dim offsets As New List(Of String)
    Dim sizes As New List(Of String)

    Dim lastSize As String

    Private Sub partitionCalculator_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Form1.Show()
    End Sub

    Private Sub resetLists()
        ListBox1.Items.Clear()
        ListBox2.Items.Clear()
        ListBox3.Items.Clear()
        ListBox4.Items.Clear()

        names.AddRange(New String() {"nvs", "otadata", "app0", "app1", "eeprom", "spiffs"})
        For Each item In names
            ListBox1.Items.Add(item)
        Next

        types.AddRange(New String() {"data, nvs", "data, ota", "app, ota_0", "app, ota_1", "data, 0x99", "data, spiffs"})
        For Each item In types
            ListBox2.Items.Add(item)
        Next

        offsets.AddRange(New String() {"0x9000", "0xe000", "0x10000", "0x1A0000", "0x330000", "0x331000"})
        For Each item In offsets
            ListBox3.Items.Add(item)
        Next

        sizes.AddRange(New String() {"0x5000", "0x2000", "0x190000", "0x190000", "0x1000", "0x0CF000"})
        For Each item In sizes
            ListBox4.Items.Add(item)
        Next

    End Sub

    Private Sub partitionCalculator_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        resetLists()
        calculateStorage()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        ListBox2.SelectedIndex = ListBox1.SelectedIndex
        ListBox3.SelectedIndex = ListBox1.SelectedIndex
        ListBox4.SelectedIndex = ListBox1.SelectedIndex
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged
        ListBox1.SelectedIndex = ListBox2.SelectedIndex
        ListBox3.SelectedIndex = ListBox2.SelectedIndex
        ListBox4.SelectedIndex = ListBox2.SelectedIndex
    End Sub

    Private Sub ListBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox3.SelectedIndexChanged
        ListBox1.SelectedIndex = ListBox3.SelectedIndex
        ListBox2.SelectedIndex = ListBox3.SelectedIndex
        ListBox4.SelectedIndex = ListBox3.SelectedIndex
    End Sub

    Private Sub ListBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox4.SelectedIndexChanged
        ListBox1.SelectedIndex = ListBox4.SelectedIndex
        ListBox2.SelectedIndex = ListBox4.SelectedIndex
        ListBox3.SelectedIndex = ListBox4.SelectedIndex
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        resetLists()
        calculateStorage()

    End Sub

    Private Sub calculateStorage()
        Dim totalBytes As Integer = 0
        Dim flashBytes As Integer = 0
        Dim percent As Double = 0

        For Each item In ListBox4.Items
            Dim partSize As Integer = Convert.ToInt32(item, 16)
            totalBytes += partSize
        Next

        'Retrieve flash total storage
        If ComboBox1.Text = "1MB" Then
            flashBytes = 1048576
        ElseIf ComboBox1.Text = "2MB" Then
            flashBytes = 2097152
        ElseIf ComboBox1.Text = "4MB" Then
            flashBytes = 4194304
        ElseIf ComboBox1.Text = "8MB" Then
            flashBytes = 8388608
        ElseIf ComboBox1.Text = "16MB" Then
            flashBytes = 16777216
        ElseIf ComboBox1.Text = "32MB" Then
            flashBytes = 33554432
        Else
            MsgBox("Flash size invalid")
        End If

        percent = (totalBytes / flashBytes) * 100
        lblStorage.Text = "Total Storage: " & Math.Round(percent, 2) & "%"
        If percent > 100 Then
            ProgressBar1.Value = 100
            MsgBox("You exceeded the maximum storage of flash! Increase flash or decrease your partition.")
            If ListBox4.SelectedIndex <> -1 Then
                ListBox4.Items.Item(ListBox4.SelectedIndex) = lastSize
                calculateStorage()
            End If
        Else
            ProgressBar1.Value = percent
            'Recalculate offsets
            recalculateOffsets()
        End If

    End Sub

    Private Shared Function Num(ByVal value As String) As Integer
        Dim returnVal As String = String.Empty
        Dim collection As MatchCollection = Regex.Matches(value, "\d+")
        For Each m As Match In collection
            returnVal += m.ToString()
        Next
        Return Convert.ToInt32(returnVal)
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim b As Long
        Dim i As Long
        Dim hexS As String

        'Back up the current value
        lastSize = ListBox4.SelectedItem.ToString

        'Parses textbox with units and then overwrites selected item in ListBox4
        If TextBox1.Text.EndsWith("MB") Then
            b = Num(TextBox1.Text)
            i = Convert.ToInt32(b) * 1048576
            hexS = "0x" & i.ToString("X4")

        ElseIf TextBox1.Text.EndsWith("kB") Then
            b = Num(TextBox1.Text)
            i = Convert.ToInt32(b) * 1024
            hexS = "0x" & i.ToString("X4")

        ElseIf TextBox1.Text.EndsWith("B") Then
            b = Num(TextBox1.Text)
            i = Convert.ToInt32(b)
            hexS = "0x" & i.ToString("X4")

        Else
            'Assume bytes
            i = Convert.ToInt32(TextBox1.Text)
            hexS = "0x" & i.ToString("X4")

        End If

        ListBox4.Items.Item(ListBox4.SelectedIndex) = hexS
        TextBox1.Clear()


        MsgBox("Updated!")

        'Recalculate percentage storage
        calculateStorage()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Deletes that selected row if exists
        Dim selectedInd As Integer = ListBox1.SelectedIndex
        If selectedInd <> -1 Then
            ListBox1.Items.RemoveAt(selectedInd)
            ListBox2.Items.RemoveAt(selectedInd)
            ListBox3.Items.RemoveAt(selectedInd)
            ListBox4.Items.RemoveAt(selectedInd)
        End If

        'Recalculate percentage storage
        calculateStorage()
    End Sub

    Private Sub ListBox4_DoubleClick(sender As Object, e As EventArgs) Handles ListBox4.DoubleClick
        Dim Bs As Double = Convert.ToInt32(ListBox4.SelectedItem, 16)
        Dim kBs As Double = Math.Round((Convert.ToInt32(ListBox4.SelectedItem, 16) / 1024), 2)
        Dim MBs As Double = Math.Round((Convert.ToInt32(ListBox4.SelectedItem, 16) / 1048576), 2)
        MsgBox("This partition is " & Bs & " bytes, " & kBs & " kB, or approx " & MBs & " MB.")

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        calculateStorage()
    End Sub

    Private Sub recalculateOffsets()
        'Iterate through ListBox3 offsets. Ignore the first offset.

        For curIndx As Integer = 1 To ListBox3.Items.Count - 1

            'We look for (n-1)offset + (n-1)size. Then round it to the nearest 4096 (or 0x1000)
            Dim prevOffset As Integer = Convert.ToInt32(ListBox3.Items.Item(curIndx - 1), 16)  'get prev offset bytes
            Dim prevSize As Integer = Convert.ToInt32(ListBox4.Items.Item(curIndx - 1), 16)    'get prev size bytes

            Dim X As Long = prevOffset + prevSize
            Dim N As Integer = 4096    'rounding factor
            Dim offsetCap As Long = Math.Round(X / N) * N    'round it to a multiple of rounding factor

            'Now we convert this back to hex
            Dim hexS As String = "0x" & offsetCap.ToString("X4")
            ListBox3.Items.Item(curIndx) = hexS

        Next
    End Sub
End Class