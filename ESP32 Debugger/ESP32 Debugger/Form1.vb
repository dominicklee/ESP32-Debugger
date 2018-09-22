Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub PartitionCalculatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PartitionCalculatorToolStripMenuItem.Click
        Me.Hide()
        partitionCalculator.Show()
    End Sub

    Private Sub TestToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestToolStripMenuItem.Click
        Dim offset As Integer = 0
        Dim X As Integer = 8000 'number to round
        Dim N As Integer = 4096    'rounding factor
        Dim offsetCap As Integer = Math.Round(X / N) * N
        MsgBox(offsetCap)
    End Sub
End Class
