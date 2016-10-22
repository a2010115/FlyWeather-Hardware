Imports System
Imports System.IO.Ports
Imports System.Runtime.InteropServices
Imports System.Runtime
Imports FlyScale.Form1.WinApi
Imports System.Windows.Forms
Imports System.Drawing
Public Class Form1

#Region "shadows"
    Private dwmMargins As MetroUI_Form.Dwm.MARGINS
    Private _marginOk As Boolean
    Private _aeroEnabled As Boolean = False

#Region "Props"
    Public ReadOnly Property AeroEnabled() As Boolean
        Get
            Return _aeroEnabled
        End Get
    End Property
#End Region

#Region "Methods"
    Public Shared Function LoWord(ByVal dwValue As Integer) As Integer
        Return dwValue And &HFFFF
    End Function

    Public Shared Function HiWord(ByVal dwValue As Integer) As Integer
        Return (dwValue >> 16) And &HFFFF
    End Function
#End Region

    Public Sub Form1_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        MetroUI_Form.Dwm.DwmExtendFrameIntoClientArea(Me.Handle, dwmMargins)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        Dim WM_NCCALCSIZE As Integer = &H83
        Dim WM_NCHITTEST As Integer = &H84
        Dim result As IntPtr = IntPtr.Zero

        Dim dwmHandled As Integer = MetroUI_Form.Dwm.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, result)

        If dwmHandled = 1 Then
            m.Result = result
            Return
        End If

        If m.Msg = WM_NCCALCSIZE AndAlso CType(m.WParam, Int32) = 1 Then
            Dim nccsp As MetroUI_Form.WinApi.NCCALCSIZE_PARAMS = DirectCast(Marshal.PtrToStructure(m.LParam, GetType(MetroUI_Form.WinApi.NCCALCSIZE_PARAMS)), MetroUI_Form.WinApi.NCCALCSIZE_PARAMS)
            nccsp.rect0.Top += 0
            nccsp.rect0.Bottom += 0
            nccsp.rect0.Left += 0
            nccsp.rect0.Right += 0

            If Not _marginOk Then
                dwmMargins.cyTopHeight = 0
                dwmMargins.cxLeftWidth = 0
                dwmMargins.cyBottomHeight = 3
                dwmMargins.cxRightWidth = 0
                _marginOk = True
            End If

            Marshal.StructureToPtr(nccsp, m.LParam, False)

            m.Result = IntPtr.Zero
        ElseIf m.Msg = WM_NCHITTEST AndAlso CType(m.Result, Int32) = 0 Then
            m.Result = HitTestNCA(m.HWnd, m.WParam, m.LParam)
        Else
            MyBase.WndProc(m)
        End If
    End Sub
    Private Function HitTestNCA(ByVal hwnd As IntPtr, ByVal wparam As IntPtr, ByVal lparam As IntPtr) As IntPtr
        Dim HTNOWHERE As Integer = 0
        Dim HTCLIENT As Integer = 1
        Dim HTCAPTION As Integer = 2
        Dim HTGROWBOX As Integer = 4
        Dim HTSIZE As Integer = HTGROWBOX
        Dim HTMINBUTTON As Integer = 8
        Dim HTMAXBUTTON As Integer = 9
        Dim HTLEFT As Integer = 10
        Dim HTRIGHT As Integer = 11
        Dim HTTOP As Integer = 12
        Dim HTTOPLEFT As Integer = 13
        Dim HTTOPRIGHT As Integer = 14
        Dim HTBOTTOM As Integer = 15
        Dim HTBOTTOMLEFT As Integer = 16
        Dim HTBOTTOMRIGHT As Integer = 17
        Dim HTREDUCE As Integer = HTMINBUTTON
        Dim HTZOOM As Integer = HTMAXBUTTON
        Dim HTSIZEFIRST As Integer = HTLEFT
        Dim HTSIZELAST As Integer = HTBOTTOMRIGHT

        Dim p As New Point(LoWord(CType(lparam, Int32)), HiWord(CType(lparam, Int32)))
        Dim topleft As Rectangle = RectangleToScreen(New Rectangle(0, 0, dwmMargins.cxLeftWidth, dwmMargins.cxLeftWidth))

        If topleft.Contains(p) Then
            Return New IntPtr(HTTOPLEFT)
        End If

        Dim topright As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, dwmMargins.cxRightWidth))

        If topright.Contains(p) Then
            Return New IntPtr(HTTOPRIGHT)
        End If

        Dim botleft As Rectangle = RectangleToScreen(New Rectangle(0, Height - dwmMargins.cyBottomHeight, dwmMargins.cxLeftWidth, dwmMargins.cyBottomHeight))

        If botleft.Contains(p) Then
            Return New IntPtr(HTBOTTOMLEFT)
        End If

        Dim botright As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, Height - dwmMargins.cyBottomHeight, dwmMargins.cxRightWidth, dwmMargins.cyBottomHeight))

        If botright.Contains(p) Then
            Return New IntPtr(HTBOTTOMRIGHT)
        End If

        Dim top As Rectangle = RectangleToScreen(New Rectangle(0, 0, Width, dwmMargins.cxLeftWidth))

        If top.Contains(p) Then
            Return New IntPtr(HTTOP)
        End If

        Dim cap As Rectangle = RectangleToScreen(New Rectangle(0, dwmMargins.cxLeftWidth, Width, dwmMargins.cyTopHeight - dwmMargins.cxLeftWidth))

        If cap.Contains(p) Then
            Return New IntPtr(HTCAPTION)
        End If

        Dim left As Rectangle = RectangleToScreen(New Rectangle(0, 0, dwmMargins.cxLeftWidth, Height))

        If left.Contains(p) Then
            Return New IntPtr(HTLEFT)
        End If

        Dim right As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, Height))

        If right.Contains(p) Then
            Return New IntPtr(HTRIGHT)
        End If

        Dim bottom As Rectangle = RectangleToScreen(New Rectangle(0, Height - dwmMargins.cyBottomHeight, Width, dwmMargins.cyBottomHeight))

        If bottom.Contains(p) Then
            Return New IntPtr(HTBOTTOM)
        End If

        Return New IntPtr(HTCLIENT)
    End Function
    Private Const BorderWidth As Integer = 16

    <DllImport("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function

    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTBORDER As Integer = 18
    Private Const HTBOTTOM As Integer = 15
    Private Const HTBOTTOMLEFT As Integer = 16
    Private Const HTBOTTOMRIGHT As Integer = 17
    Private Const HTCAPTION As Integer = 2
    Private Const HTLEFT As Integer = 10
    Private Const HTRIGHT As Integer = 11
    Private Const HTTOP As Integer = 12
    Private Const HTTOPLEFT As Integer = 13
    Private Const HTTOPRIGHT As Integer = 14
#End Region
    Dim fn
    Dim dataT(), dataP()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Point(1205, 680)
        Me.CheckForIllegalCrossThreadCalls = False
        Randomize()
        fn = "E:\FW" & CInt(100000 * Rnd()) & ".txt"
        Dim color1 As Color = Color.FromArgb(255, Color.DarkSlateGray.R - 20， Color.DarkSlateGray.G - 20， Color.DarkSlateGray.B - 20）
        PictureBox3.BackColor = color1
        PictureBox4.BackColor = color1
        PictureBox7.BackColor = color1
        Label1.BackColor = color1
        Label2.BackColor = color1
        Label3.BackColor = color1
        Label4.BackColor = color1
        Label6.BackColor = color1
        Label7.BackColor = color1

        ReDim dataT(PictureBox1.Width - 1)
        ReDim dataP(PictureBox1.Width - 1)
        b1 = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        b2 = New Bitmap(PictureBox1.Width, PictureBox1.Height)
    End Sub

    Private Sub ComboBox1_Click(sender As Object, e As EventArgs) Handles ComboBox1.Click
        ComboBox1.Items.Clear()
        Dim ports As String() = SerialPort.GetPortNames() '用命名空间SerialPort获取计算机的有效串口
        Dim port As String
        For Each port In ports
            ComboBox1.Items.Add(port) '向combobox中添加串口名称
        Next port
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        SerialPort1.Close()
        SerialPort1.PortName = ComboBox1.Text


        SerialPort1.Open()
    End Sub

    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived

        '——————————————————————————————————
        '串口接收缓冲
        Try


            If SerialPort1.BytesToRead > 0 Then
                Dim strIncoming = SerialPort1.ReadExisting.ToString '读取缓冲区中的数据

                If Not InStr(strIncoming, " ") > 0 And strIncoming.Length > 2 Then

                    If Mid(strIncoming, 1, 1) = "/" Then

                        If Mid(strIncoming, strIncoming.Length - 2, 1) = "/" Then

                            strIncoming = Replace(strIncoming, "/", "")

                            Dim Data As New List(Of String)
                            For Each message In strIncoming.Split(",")
                                Data.Add(message)
                            Next

                            If Data(0).Length > 5 Or Data(1).Length > 8 Then
                            Else
                                Label1.Text = Data(0) & "°C"
                                Label2.Text = Data(1) / 100 & "Pa"
                                updateSheet(Data)
                            End If

                        End If

                    End If

                End If

                SerialPort1.DiscardInBuffer()

            End If

            Threading.Thread.Sleep(1000)

        Catch ex As Exception

        End Try
        '——————————————————————————————————

    End Sub
    Dim maxT As Integer = 1, minT As Integer = False, maxP As Integer = 1, minP As Integer = False
    Dim lastTPoint As New Point(0, 0), lastPPoint As New Point(0, 0)

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Me.Size = New Point(1189, 622)
    End Sub

    Dim alldata(10)

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        Dim width = PictureBox1.Width - leftBroad - rightBroad, height = PictureBox1.Height - upBroad - downBroad
        Dim colCount = 6
        Dim b1 As New Bitmap(PictureBox1.Width, PictureBox1.Height)
        PictureBox1.Image = b1
        Dim b2 As New Bitmap(PictureBox1.Width, PictureBox1.Height)
        PictureBox6.Image = b2
        Dim g As Graphics = Graphics.FromImage(b1)
        Dim g2 As Graphics = Graphics.FromImage(b2)

        Dim a As New Rectangle
        Dim a2 As New Rectangle
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
        g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality

        a.Location = New Point(leftBroad, upBroad)
        a.Width = width
        a.Height = height
        a2.Location = New Point(leftBroad, upBroad)
        a2.Width = width
        a2.Height = height
        g.DrawPath(New Pen(Color.DarkSlateGray, 2), GetRoundedRectPath(a, 20))
        ' g.DrawRectangle(New Pen(Color.DarkSlateGray, 2), a)
        g2.DrawPath(New Pen(Color.DarkSlateGray, 2), GetRoundedRectPath(a2, 20))

        ' For i = 0 To colCount - 1
        'g.DrawLine(Pens.Black, New Point(leftBroad, upBroad + (i + 1) / colCount * (height)), New Point(leftBroad + width, upBroad + (i + 1) / colCount * (height)))
        'g2.DrawLine(Pens.Black, New Point(leftBroad, upBroad + (i + 1) / colCount * (height)), New Point(leftBroad + width, upBroad + (i + 1) / colCount * (height)))
        ' Next
        Timer2.Enabled = False
    End Sub

    Private Function GetRoundedRectPath(ByVal rect As Rectangle, ByVal radius As Integer) As System.Drawing.Drawing2D.GraphicsPath
        rect.Offset(-1, -1)
        Dim RoundRect As New Rectangle(rect.Location, New Size(radius - 1, radius - 1))

        Dim path As New System.Drawing.Drawing2D.GraphicsPath
        path.AddArc(RoundRect, 180, 90)     '左上角  
        RoundRect.X = rect.Right - radius   '右上角  
        path.AddArc(RoundRect, 270, 90)
        RoundRect.Y = rect.Bottom - radius  '右下角  
        path.AddArc(RoundRect, 0, 90)
        RoundRect.X = rect.Left             '左下角  
        path.AddArc(RoundRect, 90, 90)
        path.CloseFigure()
        Return path
    End Function

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click
        Me.Close()
        Me.Dispose()
    End Sub

    Dim nowX = 0
    Dim upBroad = 5, downBroad = 0, leftBroad = 5, rightBroad = 5
    Dim b1 As Bitmap
    Dim b2 As Bitmap
    Sub updateSheet(data)


        Dim t As Integer = data(0) * 10, p As Integer = data(1)
        Dim startX = PictureBox1.Location.X, startY = PictureBox1.Location.Y

        Dim width = PictureBox1.Width - leftBroad - rightBroad, height = PictureBox1.Height - upBroad - downBroad
        Dim colCount = 6

        PictureBox1.Image = b1

        PictureBox6.Image = b2
        Dim g As Graphics = Graphics.FromImage(b1)
        Dim g2 As Graphics = Graphics.FromImage(b2)
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
        g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality

        If lastTPoint.X >= PictureBox1.Width - 3 - rightBroad Or lastPPoint.X >= PictureBox1.Width - 3 - rightBroad Then
            lastTPoint = New Point(0, lastTPoint.Y)
            lastPPoint = New Point(0, lastPPoint.Y)
            Timer2.Enabled = True
            g.Clear(Color.White)
            g2.Clear(Color.White)

        End If

        Dim a As New Rectangle
        Dim a2 As New Rectangle

        a.Location = New Point(leftBroad, upBroad)
        a.Width = width
        a.Height = height
        a2.Location = New Point(leftBroad, upBroad)
        a2.Width = width
        a2.Height = height
        g.DrawPath(New Pen(Color.DarkSlateGray, 2), GetRoundedRectPath(a, 20))
        ' g.DrawRectangle(New Pen(Color.DarkSlateGray, 2), a)
        g2.DrawPath(New Pen(Color.DarkSlateGray, 2), GetRoundedRectPath(a2, 20))


        Dim setFullT = 400, setSmallT = 200, setFullP = 103000, setSmallP = 96000

        If lastTPoint.X = 0 And lastPPoint.X = 0 Then
            lastTPoint.X = leftBroad
            lastPPoint.X = leftBroad
            lastTPoint = New Point(lastTPoint.X + 1, ((setFullT - (t - setSmallT))) / setFullT * height + upBroad)
            lastPPoint = New Point(lastPPoint.X + 1, ((setFullP - setSmallP) - (p - setSmallP)) / (setFullP - setSmallP) * height + upBroad)
            Dim pent As New Pen(Color.Orange)
            pent.Width = 2
            pent.Color = Color.FromArgb(80, Color.Orange)
            g.DrawLine(pent, New Point(lastTPoint.X, ((setFullT - (t - setSmallT))) / setFullT * height + upBroad), New Point(lastTPoint.X, height + upBroad))
            pent.Color = Color.FromArgb(80, Color.RoyalBlue)
            g2.DrawLine(pent, New Point(lastPPoint.X, ((setFullP - setSmallP) - (p - setSmallP)) / (setFullP - setSmallP) * height + upBroad), New Point(lastPPoint.X, height + upBroad))
        End If

        Dim isChange As Boolean = False

        If minT = False Then
            minT = t
            maxT = t + 1
        Else
            If t < minT Then minT = t
            If t > maxT Then maxT = t
            isChange = True
        End If
        If minP = False Then
            minP = p
            maxP = p + 1
        Else
            If p < minP Then minP = p
            If p > maxP Then maxP = p
            isChange = True
        End If


        dataT(lastTPoint.X - leftBroad - 1) = t
        'Debug.WriteLine(lastTPoint.X - leftBroad - 1)
        dataP(lastPPoint.X - leftBroad - 1) = p

        My.Computer.FileSystem.WriteAllText(fn, Now.Date & "," & Now.Hour & "," & Now.Minute & "," & Now.Second & "," & t & "," & p & vbCrLf, True)

        Dim pen1 As New Pen(Color.Orange)
        pen1.Color = Color.FromArgb(255, Color.Orange)
        pen1.Width = 2
        Dim pen2 As New Pen(Color.RoyalBlue)
        pen2.Color = Color.FromArgb(255, Color.RoyalBlue)
        pen2.Width = 2

        g.DrawLine(pen1, lastTPoint, New Point(lastTPoint.X + 1, ((setFullT - (t - setSmallT))) / setFullT * height + upBroad))
        pen1.Color = Color.FromArgb(80, Color.Orange)
        g.DrawLine(pen1, New Point(lastTPoint.X + 1, ((setFullT - (t - setSmallT))) / setFullT * height + upBroad), New Point(lastTPoint.X + 1, height + upBroad))
        lastTPoint = New Point(lastTPoint.X + 1, ((setFullT - (t - setSmallT))) / setFullT * height + upBroad)


        '  ((setFullP - (p - setSmallP))) / setFullP * height + upBroad'
        g2.DrawLine(pen2, lastPPoint, New Point(lastPPoint.X + 1, ((setFullP - setSmallP) - (p - setSmallP)) / (setFullP - setSmallP) * height + upBroad))
        pen2.Color = Color.FromArgb(80, Color.RoyalBlue)
        g2.DrawLine(pen2, New Point(lastPPoint.X + 1, ((setFullP - setSmallP) - (p - setSmallP)) / (setFullP - setSmallP) * height + upBroad), New Point(lastPPoint.X + 1, height + upBroad))
        lastPPoint = New Point(lastPPoint.X + 1, ((setFullP - setSmallP) - (p - setSmallP)) / (setFullP - setSmallP) * height + upBroad)
        ' Debug.WriteLine(((setFullP - (p - setSmallP))) / setFullP * height)

    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Timer2.Enabled = True
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Try
            Label6.Text = dataT(e.X - leftBroad) / 10 & "°C"
        Catch ex As Exception
        End Try
    End Sub
    Private Sub PictureBox6_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox6.MouseMove
        Try
            Label6.Text = dataP(e.X - leftBroad) / 100 & "Pa"
        Catch ex As Exception
        End Try
    End Sub
End Class
