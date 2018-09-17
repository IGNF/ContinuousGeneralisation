'矩阵类
'该类具有矩阵的+,-,*,求逆，转置等运算功能
'作者：戴吾蛟
'创建时间：　2008-3-21
'修改时间：　
Public Class VBMatrix
    Private dblMatData(,) As Double '二维数组，用于存储矩阵中的元素
    Private intRow As Integer '行
    Private intCol As Integer '列
    Public Sub New()
    End Sub
    Public Sub New(ByVal row As Integer, ByVal col As Integer)
        dblMatData = New Double(row - 1, col - 1) {}
        intRow = row
        intCol = col
    End Sub
    'Default表示该属性为缺省属性，Item根据行列返回或设置矩阵中的元素
    '注意行和列都从０开始
    Default Public Property Item(ByVal row As Integer, ByVal col As Integer) As Double
        Get
            Return dblMatData(row, col)
        End Get
        Set(ByVal value As Double)
            dblMatData(row, col) = value
        End Set
    End Property

    '返回数据
    Public Property MatData() As Double(,)
        Get
            Return dblMatData
        End Get
        Set(ByVal value As Double(,))
            dblMatData = value
            intRow = value.GetUpperBound(0)
            intCol = value.GetUpperBound(1)
        End Set
    End Property

    '只读属性，返回矩阵的行
    Public ReadOnly Property Row() As Integer
        Get
            Return intRow
        End Get
    End Property
    '只读属性，返回矩阵的列
    Public ReadOnly Property Col() As Integer
        Get
            Return intCol
        End Get
    End Property
    '运算符方法（＋）,返回两个矩阵之和
    Public Shared Operator +(ByVal leftMat As VBMatrix, ByVal rightMat As VBMatrix) As VBMatrix
        Dim rowLeft, colLeft, rowRight, colRight As Integer
        rowLeft = leftMat.Row
        colLeft = leftMat.Col

        rowRight = rightMat.Row
        colRight = rightMat.Col

        If rowLeft <> rowRight Or colLeft <> colRight Then
            MsgBox("矩阵相加发生错误,原因:相加的两个矩阵的行或列不相等!")
            Return Nothing
        End If

        Dim addMat As New VBMatrix(rowLeft, colLeft)
        Dim i, j As Integer
        For i = 0 To rowLeft - 1
            For j = 0 To colLeft - 1
                addMat.dblMatData(i, j) = leftMat.dblMatData(i, j) + rightMat.dblMatData(i, j)
            Next j
        Next i
        Return addMat
    End Operator
    '运算符方法（－）　，返回两个矩阵之差
    Public Shared Operator -(ByVal leftMat As VBMatrix, ByVal rightMat As VBMatrix) As VBMatrix
        Dim rowLeft, colLeft, rowRight, colRight As Integer
        rowLeft = leftMat.Row
        colLeft = leftMat.Col

        rowRight = rightMat.Row
        colRight = rightMat.Col
        If rowLeft <> rowRight Or colLeft <> colRight Then
            MsgBox("矩阵相减发生错误,原因:相减的两个矩阵的行或列不相等!")
            Return Nothing
        End If

        Dim subMat As New VBMatrix(rowLeft, colLeft)
        Dim i, j As Integer
        For i = 0 To rowLeft - 1
            For j = 0 To colLeft - 1
                subMat.dblMatData(i, j) = leftMat.dblMatData(i, j) - rightMat.dblMatData(i, j)
            Next j
        Next i
        Return subMat
    End Operator
    '运算符方法*，返回两个矩阵之积
    Public Overloads Shared Operator *(ByVal leftMat As VBMatrix, ByVal rightMat As VBMatrix) As VBMatrix
        Dim rowleft, colleft, rowright, colright As Integer
        rowleft = leftMat.Row
        colleft = leftMat.Col

        rowright = rightMat.Row
        colright = rightMat.Col
        If colleft <> rowright Then
            MsgBox("矩阵相乘错误，原因：第一个矩阵与第二个矩阵的列行不等")
            Return Nothing
        End If
        Dim mulmat As New VBMatrix(rowleft, colright)
        Dim i, j, k As Integer, dblone, dblmul As Double
        k = 0
        dblone = 0
        dblmul = 0
        For i = 0 To rowleft - 1
            For k = 0 To colright - 1
                For j = 0 To rowright - 1
                    dblone = leftMat.dblMatData(i, j) * rightMat.dblMatData(j, k)
                    dblmul = dblone + dblmul
                    If j = rowright - 1 Then
                        mulmat(i, k) = dblmul
                        dblmul = 0
                    End If
                Next
            Next
        Next
        Return mulmat
    End Operator
    '重载运算符方法*，返回一个矩阵与一个常数之积
    Public Overloads Shared Operator *(ByVal leftMat As VBMatrix, ByVal dblNum As Double) As VBMatrix
        Dim rowmat, colmat As Integer
        rowmat = leftMat.Row
        colmat = leftMat.Col
        Dim matchen As New VBMatrix(rowmat, colmat)
        Dim i, j As Integer
        For i = 0 To rowmat - 1
            For j = 0 To colmat - 1
                matchen.dblMatData(i, j) = leftMat.dblMatData(i, j) * dblNum
            Next
        Next
        Return matchen
    End Operator

    ''重载运算符方法*，返回一个矩阵与一个常数之积
    'Public Overloads Shared Operator ==(ByVal leftMat As VBMatrix, ByVal rightMat As VBMatrix) As Boolean

    '    Dim rowmat, colmat As Integer
    '    rowmat = leftMat.Row
    '    colmat = leftMat.Col
    '    Dim matchen As New VBMatrix(rowmat, colmat)
    '    Dim i, j As Integer
    '    For i = 0 To rowmat - 1
    '        For j = 0 To colmat - 1
    '            matchen.dblMatData(i, j) = leftMat.dblMatData(i, j) * 2
    '        Next
    '    Next
    '    Return True

    'End Operator
    '返回矩阵的转置
    Public Function Trans() As VBMatrix
        Dim mat As New VBMatrix(Col, Row)
        Dim i, j As Integer
        For i = 0 To mat.Row - 1
            For j = 0 To mat.Col - 1
                mat.dblMatData(i, j) = dblMatData(j, i)
            Next
        Next
        Return mat
    End Function
    '交换函数
    Private Sub swap(ByRef a As Double, ByRef b As Double)
        Dim c As Double
        c = a
        a = b
        b = c
    End Sub
    '返回矩阵逆
    '采用的选全主元方法
    Public Function Inv(ByVal Matni As VBMatrix) As VBMatrix
        Dim i, j, k, n As Integer
        Dim Temp As Double
        If Matni.Row <> Matni.Col Then
            MsgBox("错误：矩阵行数和列数不等，不能求逆")
        End If
        n = Matni.Row
        Dim iw() As Integer
        Dim jw() As Integer
        Dim Maxnum As Double
        ReDim iw(n), jw(n)   '注意 redim(0 to n), as ***
        For k = 0 To n - 1
            Maxnum = 0
            For i = k To n - 1
                For j = k To n - 1
                    If Math.Abs(Matni(i, j)) > Maxnum Then
                        Maxnum = Math.Abs(Matni(i, j))
                        iw(k) = i
                        jw(k) = j
                    End If
                Next
            Next
            Dim l As Integer = 0
            If iw(k) <> k Then
                For l = 0 To n - 1
                    swap(Matni(k, l), Matni(iw(k), l))
                Next
            End If
            If jw(k) <> k Then
                For l = 0 To n - 1
                    swap(Matni(l, k), Matni(l, jw(k)))
                Next
            End If
        Next

        '交换行和交换列



        '在Matni矩阵右边增加一个单位阵并赋值
        Dim str(n - 1, 1) As Double
        ReDim str(n - 1, 2 * n - 1)
        For i = 0 To n - 1
            For j = 0 To n - 1
                str(i, j) = Matni(i, j)
            Next
        Next
        For i = 0 To n - 1
            For j = n To 2 * n - 1
                If i = j - n Then
                    str(i, j) = 1
                Else
                    str(i, j) = 0
                End If
            Next
        Next
        '用高斯消去法使矩阵变为单位阵，则右边的单位阵将变成原矩阵的逆阵
        For k = 0 To n - 2
            If str(k, k) = 0 Then
                For j = 0 To 2 * n - 1
                    swap(str(k, j), str(k + 1, j))
                Next
            End If
            For i = k + 1 To n - 1
                'If str(k, k) <> 1 Then
                '    For j = k To 2 * n - 1
                '        str(k, j) = str(k, j) / str(k, k)
                '    Next
                'End If
                Temp = str(i, k) / str(k, k)
                For j = 0 To 2 * n - 1
                    str(i, j) = str(i, j) - str(k, j) * Temp
                Next
            Next

        Next
        For k = n - 1 To 1 Step -1
            For i = k - 1 To 0 Step -1
                Temp = str(i, k) / str(k, k)
                For j = 2 * n - 1 To k Step -1             'i,0
                    str(i, j) = str(i, j) - str(k, j) * Temp
                Next
            Next
        Next



        '乘数
        For i = 0 To n - 1
            Dim dbls As Double
            dbls = str(i, i)
            For j = 0 To 2 * n - 1
                str(i, j) = str(i, j) / dbls
            Next
        Next
        '输出右边的矩阵
        For i = 0 To n - 1
            For j = 0 To n - 1
                Matni(i, j) = str(i, j + n)
            Next
        Next
        '进行行和列的恢复
        For k = n - 1 To 0 Step -1
            If jw(k) <> k Then
                For i = 0 To n - 1
                    swap(Matni(k, i), Matni(jw(k), i))
                Next
            End If
            If iw(k) <> k Then
                For i = 0 To n - 1
                    swap(Matni(i, k), Matni(i, iw(k)))
                Next
            End If
        Next

        Return Matni
    End Function
    '返回矩阵的子矩阵
    Public Function GetSubMatrix(ByVal intRowNum As Integer, ByVal intRowCount As Integer, ByVal intColNum As Integer, ByVal intColCount As Integer) As VBMatrix
        Dim mat As New VBMatrix(intRowCount, intColCount)
        Dim i, j As Integer
        For i = 0 To intRowCount - 1
            For j = 0 To intColCount - 1
                mat.dblMatData(i, j) = dblMatData(intRowNum + i, intColNum + j)
            Next
        Next
        Return mat
    End Function
End Class
