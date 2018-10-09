using System;
using System.Collections.Generic;
using System.Text;

namespace MorphingClass.CUtility
{
    /**
        * 操作矩阵的类 CMatrix

        * @author 周长发
        * @version 1.0
        */
    //public class CMatrix
    //{
    //    private int numCols = 0;			// 矩阵列数
    //    private int numRows = 0;			// 矩阵行数
    //    private double eps = 0.0;			// 缺省精度
    //    private double[] elements = null;	// 矩阵数据缓冲区

    //    /**
    //     * 属性: 矩阵列数
    //     */
    //    public int Cols
    //    {
    //        get
    //        {
    //            return numCols;
    //        }
    //    }

    //    /**
    //     * 属性: 矩阵行数
    //     */
    //    public int Rows
    //    {
    //        get
    //        {
    //            return numRows;
    //        }
    //    }

    //    /**
    //     * 索引器: 访问矩阵元素
    //     * @param row - 元素的行
    //     * @param col - 元素的列
    //     */
    //    public double this[int row, int col]
    //    {
    //        get
    //        {
    //            return elements[col + row * numCols];
    //        }
    //        set
    //        {
    //            elements[col + row * numCols] = value;
    //        }
    //    }

    //    /**
    //     * 属性: Eps
    //     */
    //    public double Eps
    //    {
    //        get
    //        {
    //            return eps;
    //        }
    //        set
    //        {
    //            eps = value;
    //        }
    //    }

    //    /**
    //     * 基本构造函数
    //     */
    //    public CMatrix()
    //    {
    //        numCols = 1;
    //        numRows = 1;
    //        Init(numRows, numCols);
    //    }

    //    /**
    //     * 指定行列构造函数
    //     * 
    //     * @param nRows - 指定的矩阵行数
    //     * @param nCols - 指定的矩阵列数
    //     */
    //    public CMatrix(int nRows, int nCols)
    //    {
    //        numRows = nRows;
    //        numCols = nCols;
    //        Init(numRows, numCols);
    //    }

    //    /**
    //     * 指定值构造函数
    //     * 
    //     * @param value - 二维数组，存储矩阵各元素的值
    //     */
    //    public CMatrix(double[,] value)
    //    {
    //        numRows = value.GetLength(0);
    //        numCols = value.GetLength(1);
    //        double[] data = new double[numRows * numCols];
    //        int k = 0;
    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //            {
    //                data[k++] = value[i, j];
    //            }
    //        }
    //        Init(numRows, numCols);
    //        SetData(data);
    //    }

    //    /**
    //     * 指定值构造函数
    //     * 
    //     * @param nRows - 指定的矩阵行数
    //     * @param nCols - 指定的矩阵列数
    //     * @param value - 一维数组，长度为nRows*nCols，存储矩阵各元素的值
    //     */
    //    public CMatrix(int nRows, int nCols, double[] value)
    //    {
    //        numRows = nRows;
    //        numCols = nCols;
    //        Init(numRows, numCols);
    //        SetData(value);
    //    }

    //    /**
    //     * 方阵构造函数
    //     * 
    //     * @param nSize - 方阵行列数
    //     */
    //    public CMatrix(int nSize)
    //    {
    //        numRows = nSize;
    //        numCols = nSize;
    //        Init(nSize, nSize);
    //    }

    //    /**
    //     * 方阵构造函数
    //     * 
    //     * @param nSize - 方阵行列数
    //     * @param value - 一维数组，长度为nRows*nRows，存储方阵各元素的值
    //     */
    //    public CMatrix(int nSize, double[] value)
    //    {
    //        numRows = nSize;
    //        numCols = nSize;
    //        Init(nSize, nSize);
    //        SetData(value);
    //    }

    //    /**
    //     * 拷贝构造函数
    //     * 
    //     * @param other - 源矩阵
    //     */
    //    public CMatrix(CMatrix other)
    //    {
    //        numCols = other.GetNumCols();
    //        numRows = other.GetNumRows();
    //        Init(numRows, numCols);
    //        SetData(other.elements);
    //    }

    //    /**
    //     * 初始化函数
    //     * 
    //     * @param nRows - 指定的矩阵行数
    //     * @param nCols - 指定的矩阵列数
    //     * @return bool, 成功返回true, 否则返回false
    //     */
    //    public bool Init(int nRows, int nCols)
    //    {
    //        numRows = nRows;
    //        numCols = nCols;
    //        int nSize = nCols * nRows;
    //        if (nSize < 0)
    //            return false;

    //        // 分配内存
    //        elements = new double[nSize];

    //        return true;
    //    }

    //    /**
    //     * 设置矩阵运算的精度
    //     * 
    //     * @param newEps - 新的精度值
    //     */
    //    public void SetEps(double newEps)
    //    {
    //        eps = newEps;
    //    }

    //    /**
    //     * 取矩阵的精度值
    //     * 
    //     * @return double型，矩阵的精度值
    //     */
    //    public double GetEps()
    //    {
    //        return eps;
    //    }

    //    /**
    //     * 重载 + 运算符
    //     * 
    //     * @return CMatrix对象
    //     */
    //    public static CMatrix operator +(CMatrix m1, CMatrix m2)
    //    {
    //        return m1.Add(m2);
    //    }

    //    /**
    //     * 重载 - 运算符
    //     * 
    //     * @return CMatrix对象
    //     */
    //    public static CMatrix operator -(CMatrix m1, CMatrix m2)
    //    {
    //        return m1.Subtract(m2);
    //    }

    //    /**
    //     * 重载 * 运算符
    //     * 
    //     * @return CMatrix对象
    //     */
    //    public static CMatrix operator *(CMatrix m1, CMatrix m2)
    //    {
    //        return m1.Multiply(m2);
    //    }

    //    /**
    //     * 重载 double[] 运算符
    //     * 
    //     * @return double[]对象
    //     */
    //    public static implicit operator double[](CMatrix m)
    //    {
    //        return m.elements;
    //    }

    //    /**
    //     * 将方阵初始化为单位矩阵
    //     * 
    //     * @param nSize - 方阵行列数
    //     * @return bool 型，初始化是否成功
    //     */
    //    public bool MakeUnitCMatrix(int nSize)
    //    {
    //        if (!Init(nSize, nSize))
    //            return false;

    //        for (int i = 0; i < nSize; ++i)
    //            for (int j = 0; j < nSize; ++j)
    //                if (i == j)
    //                    SetElement(i, j, 1);

    //        return true;
    //    }

    //    /**
    //     * 将矩阵各元素的值转化为字符串, 元素之间的分隔符为",", 行与行之间有回车换行符
    //     * @return string 型，转换得到的字符串
    //     */
    //    public override string ToString()
    //    {
    //        return ToString(",", true);
    //    }

    //    /**
    //     * 将矩阵各元素的值转化为字符串
    //     * 
    //     * @param sDelim - 元素之间的分隔符
    //     * @param bLineBreak - 行与行之间是否有回车换行符
    //     * @return string 型，转换得到的字符串
    //     */
    //    public string ToString(string sDelim, bool bLineBreak)
    //    {
    //        string s = "";

    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //            {
    //                string ss = GetElement(i, j).ToString("F");
    //                s += ss;

    //                if (bLineBreak)
    //                {
    //                    if (j != numCols - 1)
    //                        s += sDelim;
    //                }
    //                else
    //                {
    //                    if (i != numRows - 1 || j != numCols - 1)
    //                        s += sDelim;
    //                }
    //            }
    //            if (bLineBreak)
    //                if (i != numRows - 1)
    //                    s += "\r\n";
    //        }

    //        return s;
    //    }

    //    /**
    //     * 将矩阵指定行中各元素的值转化为字符串
    //     * 
    //     * @param nRow - 指定的矩阵行，nRow = 0表示第一行
    //     * @param sDelim - 元素之间的分隔符
    //     * @return string 型，转换得到的字符串
    //     */
    //    public string ToStringRow(int nRow, string sDelim)
    //    {
    //        string s = "";

    //        if (nRow >= numRows)
    //            return s;

    //        for (int j = 0; j < numCols; ++j)
    //        {
    //            string ss = GetElement(nRow, j).ToString("F");
    //            s += ss;
    //            if (j != numCols - 1)
    //                s += sDelim;
    //        }

    //        return s;
    //    }

    //    /**
    //     * 将矩阵指定列中各元素的值转化为字符串
    //     * 
    //     * @param nCol - 指定的矩阵行，nCol = 0表示第一列
    //     * @param sDelim - 元素之间的分隔符
    //     * @return string 型，转换得到的字符串
    //     */
    //    public string ToStringCol(int nCol, string sDelim /*= " "*/)
    //    {
    //        string s = "";

    //        if (nCol >= numCols)
    //            return s;

    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            string ss = GetElement(i, nCol).ToString("F");
    //            s += ss;
    //            if (i != numRows - 1)
    //                s += sDelim;
    //        }

    //        return s;
    //    }

    //    /**
    //     * 设置矩阵各元素的值
    //     * 
    //     * @param value - 一维数组，长度为numCols*numRows，存储
    //     *	              矩阵各元素的值
    //     */
    //    public void SetData(double[] value)
    //    {
    //        elements = (double[])value.Clone();
    //    }

    //    /**
    //     * 设置指定元素的值
    //     * 
    //     * @param nRow - 元素的行
    //     * @param nCol - 元素的列
    //     * @param value - 指定元素的值
    //     * @return bool 型，说明设置是否成功
    //     */
    //    public bool SetElement(int nRow, int nCol, double value)
    //    {
    //        if (nCol < 0 || nCol >= numCols || nRow < 0 || nRow >= numRows)
    //            return false;						// array bounds error

    //        elements[nCol + nRow * numCols] = value;

    //        return true;
    //    }

    //    /**
    //     * 获取指定元素的值
    //     * 
    //     * @param nRow - 元素的行
    //     * @param nCol - 元素的列
    //     * @return double 型，指定元素的值
    //     */
    //    public double GetElement(int nRow, int nCol)
    //    {
    //        return elements[nCol + nRow * numCols];
    //    }

    //    /**
    //     * 获取矩阵的列数
    //     * 
    //     * @return int 型，矩阵的列数
    //     */
    //    public int GetNumCols()
    //    {
    //        return numCols;
    //    }

    //    /**
    //     * 获取矩阵的行数
    //     * @return int 型，矩阵的行数
    //     */
    //    public int GetNumRows()
    //    {
    //        return numRows;
    //    }

    //    /**
    //     * 获取矩阵的数据
    //     * 
    //     * @return double型数组，指向矩阵各元素的数据缓冲区
    //     */
    //    public double[] GetData()
    //    {
    //        return elements;
    //    }

    //    /**
    //     * 获取指定行的向量
    //     * 
    //     * @param nRow - 向量所在的行
    //     * @param pVector - 指向向量中各元素的缓冲区
    //     * @return int 型，向量中元素的个数，即矩阵的列数
    //     */
    //    public int GetRowVector(int nRow, double[] pVector)
    //    {
    //        for (int j = 0; j < numCols; ++j)
    //            pVector[j] = GetElement(nRow, j);

    //        return numCols;
    //    }

    //    /**
    //     * 获取指定列的向量
    //     * 
    //     * @param nCol - 向量所在的列
    //     * @param pVector - 指向向量中各元素的缓冲区
    //     * @return int 型，向量中元素的个数，即矩阵的行数
    //     */
    //    public int GetColVector(int nCol, double[] pVector)
    //    {
    //        for (int i = 0; i < numRows; ++i)
    //            pVector[i] = GetElement(i, nCol);

    //        return numRows;
    //    }

    //    /**
    //     * 给矩阵赋值
    //     * 
    //     * @param other - 用于给矩阵赋值的源矩阵
    //     * @return CMatrix型，阵与other相等
    //     */
    //    public CMatrix SetValue(CMatrix other)
    //    {
    //        if (other != this)
    //        {
    //            Init(other.GetNumRows(), other.GetNumCols());
    //            SetData(other.elements);
    //        }

    //        // finally return a reference to ourselves
    //        return this;
    //    }

    //    /**
    //     * 判断矩阵否相等
    //     * 
    //     * @param other - 用于比较的矩阵
    //     * @return bool 型，两个矩阵相等则为true，否则为false
    //     */
    //    public override bool Equals(object other)
    //    {
    //        CMatrix CMatrix = other as CMatrix;
    //        if (CMatrix == null)
    //            return false;

    //        // 首先检查行列数是否相等
    //        if (numCols != CMatrix.GetNumCols() || numRows != CMatrix.GetNumRows())
    //            return false;

    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //            {
    //                if (Math.Abs(GetElement(i, j) - CMatrix.GetElement(i, j)) > eps)
    //                    return false;
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 因为重写了Equals，因此必须重写GetHashCode
    //     * 
    //     * @return int型，返回复数对象散列码
    //     */
    //    public override int GetHashCode()
    //    {
    //        double sum = 0;
    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //            {
    //                sum += Math.Abs(GetElement(i, j));
    //            }
    //        }
    //        return (int)Math.Sqrt(sum);
    //    }

    //    /**
    //     * 实现矩阵的加法
    //     * 
    //     * @param other - 与指定矩阵相加的矩阵
    //     * @return CMatrix型，指定矩阵与other相加之和
    //     * @如果矩阵的行/列数不匹配，则会抛出异常
    //     */
    //    public CMatrix Add(CMatrix other)
    //    {
    //        // 首先检查行列数是否相等
    //        if (numCols != other.GetNumCols() ||
    //            numRows != other.GetNumRows())
    //            throw new Exception("矩阵的行/列数不匹配。");

    //        // 构造结果矩阵
    //        CMatrix result = new CMatrix(this);		// 拷贝构造

    //        // 矩阵加法
    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //                result.SetElement(i, j, result.GetElement(i, j) + other.GetElement(i, j));
    //        }

    //        return result;
    //    }

    //    /**
    //     * 实现矩阵的减法
    //     * 
    //     * @param other - 与指定矩阵相减的矩阵
    //     * @return CMatrix型，指定矩阵与other相减之差
    //     * @如果矩阵的行/列数不匹配，则会抛出异常
    //     */
    //    public CMatrix Subtract(CMatrix other)
    //    {
    //        if (numCols != other.GetNumCols() ||
    //            numRows != other.GetNumRows())
    //            throw new Exception("矩阵的行/列数不匹配。");

    //        // 构造结果矩阵
    //        CMatrix result = new CMatrix(this);		// 拷贝构造

    //        // 进行减法操作
    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //                result.SetElement(i, j, result.GetElement(i, j) - other.GetElement(i, j));
    //        }

    //        return result;
    //    }

    //    /**
    //     * 实现矩阵的数乘
    //     * 
    //     * @param value - 与指定矩阵相乘的实数
    //     * @return CMatrix型，指定矩阵与value相乘之积
    //     */
    //    public CMatrix Multiply(double value)
    //    {
    //        // 构造目标矩阵
    //        CMatrix result = new CMatrix(this);		// copy ourselves

    //        // 进行数乘
    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //                result.SetElement(i, j, result.GetElement(i, j) * value);
    //        }

    //        return result;
    //    }

    //    /**
    //     * 实现矩阵的乘法
    //     * 
    //     * @param other - 与指定矩阵相乘的矩阵
    //     * @return CMatrix型，指定矩阵与other相乘之积
    //     * @如果矩阵的行/列数不匹配，则会抛出异常
    //     */
    //    public CMatrix Multiply(CMatrix other)
    //    {
    //        // 首先检查行列数是否符合要求
    //        if (numCols != other.GetNumRows())
    //            throw new Exception("矩阵的行/列数不匹配。");

    //        // ruct the object we are going to return
    //        CMatrix result = new CMatrix(numRows, other.GetNumCols());

    //        // 矩阵乘法，即
    //        //
    //        // [A][B][C]   [G][H]     [A*G + B*I + C*K][A*H + B*J + C*L]
    //        // [D][E][F] * [I][J] =   [D*G + E*I + F*K][D*H + E*J + F*L]
    //        //             [K][L]
    //        //
    //        double value;
    //        for (int i = 0; i < result.GetNumRows(); ++i)
    //        {
    //            for (int j = 0; j < other.GetNumCols(); ++j)
    //            {
    //                value = 0.0;
    //                for (int k = 0; k < numCols; ++k)
    //                {
    //                    value += GetElement(i, k) * other.GetElement(k, j);
    //                }

    //                result.SetElement(i, j, value);
    //            }
    //        }

    //        return result;
    //    }

    //    /**
    //     * 复矩阵的乘法
    //     * 
    //     * @param AR - 左边复矩阵的实部矩阵
    //     * @param AI - 左边复矩阵的虚部矩阵
    //     * @param BR - 右边复矩阵的实部矩阵
    //     * @param BI - 右边复矩阵的虚部矩阵
    //     * @param CR - 乘积复矩阵的实部矩阵
    //     * @param CI - 乘积复矩阵的虚部矩阵
    //     * @return bool型，复矩阵乘法是否成功
    //     */
    //    public bool Multiply(CMatrix AR, CMatrix AI, CMatrix BR, CMatrix BI, CMatrix CR, CMatrix CI)
    //    {
    //        // 首先检查行列数是否符合要求
    //        if (AR.GetNumCols() != AI.GetNumCols() ||
    //            AR.GetNumRows() != AI.GetNumRows() ||
    //            BR.GetNumCols() != BI.GetNumCols() ||
    //            BR.GetNumRows() != BI.GetNumRows() ||
    //            AR.GetNumCols() != BR.GetNumRows())
    //            return false;

    //        // 构造乘积矩阵实部矩阵和虚部矩阵
    //        CMatrix mtxCR = new CMatrix(AR.GetNumRows(), BR.GetNumCols());
    //        CMatrix mtxCI = new CMatrix(AR.GetNumRows(), BR.GetNumCols());
    //        // 复矩阵相乘
    //        for (int i = 0; i < AR.GetNumRows(); ++i)
    //        {
    //            for (int j = 0; j < BR.GetNumCols(); ++j)
    //            {
    //                double vr = 0;
    //                double vi = 0;
    //                for (int k = 0; k < AR.GetNumCols(); ++k)
    //                {
    //                    double p = AR.GetElement(i, k) * BR.GetElement(k, j);
    //                    double q = AI.GetElement(i, k) * BI.GetElement(k, j);
    //                    double s = (AR.GetElement(i, k) + AI.GetElement(i, k)) * (BR.GetElement(k, j) + BI.GetElement(k, j));
    //                    vr += p - q;
    //                    vi += s - p - q;
    //                }
    //                mtxCR.SetElement(i, j, vr);
    //                mtxCI.SetElement(i, j, vi);
    //            }
    //        }

    //        CR = mtxCR;
    //        CI = mtxCI;

    //        return true;
    //    }

    //    /**
    //     * 矩阵的转置
    //     * 
    //     * @return CMatrix型，指定矩阵转置矩阵
    //     */
    //    public CMatrix Transpose()
    //    {
    //        // 构造目标矩阵
    //        CMatrix Trans = new CMatrix(numCols, numRows);

    //        // 转置各元素
    //        for (int i = 0; i < numRows; ++i)
    //        {
    //            for (int j = 0; j < numCols; ++j)
    //                Trans.SetElement(j, i, GetElement(i, j));
    //        }

    //        return Trans;
    //    }

    //    /**
    //     * 实矩阵求逆的全选主元高斯－约当法
    //     * 
    //     * @return bool型，求逆是否成功
    //     */
    //    public bool InvertGaussJordan()
    //    {
    //        int i, j, k, l, u, v;
    //        double d = 0, p = 0;

    //        // 分配内存
    //        int[] pnRow = new int[numCols];
    //        int[] pnCol = new int[numCols];

    //        // 消元
    //        for (k = 0; k <= numCols - 1; k++)
    //        {
    //            d = 0.0;
    //            for (i = k; i <= numCols - 1; i++)
    //            {
    //                for (j = k; j <= numCols - 1; j++)
    //                {
    //                    l = i * numCols + j; p = Math.Abs(elements[l]);
    //                    if (p > d)
    //                    {
    //                        d = p;
    //                        pnRow[k] = i;
    //                        pnCol[k] = j;
    //                    }
    //                }
    //            }

    //            // 失败
    //            if (d == 0.0)
    //            {
    //                return false;
    //            }

    //            if (pnRow[k] != k)
    //            {
    //                for (j = 0; j <= numCols - 1; j++)
    //                {
    //                    u = k * numCols + j;
    //                    v = pnRow[k] * numCols + j;
    //                    p = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = p;
    //                }
    //            }

    //            if (pnCol[k] != k)
    //            {
    //                for (i = 0; i <= numCols - 1; i++)
    //                {
    //                    u = i * numCols + k;
    //                    v = i * numCols + pnCol[k];
    //                    p = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = p;
    //                }
    //            }

    //            l = k * numCols + k;
    //            elements[l] = 1.0 / elements[l];
    //            for (j = 0; j <= numCols - 1; j++)
    //            {
    //                if (j != k)
    //                {
    //                    u = k * numCols + j;
    //                    elements[u] = elements[u] * elements[l];
    //                }
    //            }

    //            for (i = 0; i <= numCols - 1; i++)
    //            {
    //                if (i != k)
    //                {
    //                    for (j = 0; j <= numCols - 1; j++)
    //                    {
    //                        if (j != k)
    //                        {
    //                            u = i * numCols + j;
    //                            elements[u] = elements[u] - elements[i * numCols + k] * elements[k * numCols + j];
    //                        }
    //                    }
    //                }
    //            }

    //            for (i = 0; i <= numCols - 1; i++)
    //            {
    //                if (i != k)
    //                {
    //                    u = i * numCols + k;
    //                    elements[u] = -elements[u] * elements[l];
    //                }
    //            }
    //        }

    //        // 调整恢复行列次序
    //        for (k = numCols - 1; k >= 0; k--)
    //        {
    //            if (pnCol[k] != k)
    //            {
    //                for (j = 0; j <= numCols - 1; j++)
    //                {
    //                    u = k * numCols + j;
    //                    v = pnCol[k] * numCols + j;
    //                    p = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = p;
    //                }
    //            }

    //            if (pnRow[k] != k)
    //            {
    //                for (i = 0; i <= numCols - 1; i++)
    //                {
    //                    u = i * numCols + k;
    //                    v = i * numCols + pnRow[k];
    //                    p = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = p;
    //                }
    //            }
    //        }

    //        // 成功返回
    //        return true;
    //    }

    //    /**
    //     * 复矩阵求逆的全选主元高斯－约当法
    //     * 
    //     * @param mtxImag - 复矩阵的虚部矩阵，当前矩阵为复矩阵的实部
    //     * @return bool型，求逆是否成功
    //     */
    //    public bool InvertGaussJordan(CMatrix mtxImag)
    //    {
    //        int i, j, k, l, u, v, w;
    //        double p, q, s, t, d, b;

    //        // 分配内存
    //        int[] pnRow = new int[numCols];
    //        int[] pnCol = new int[numCols];

    //        // 消元
    //        for (k = 0; k <= numCols - 1; k++)
    //        {
    //            d = 0.0;
    //            for (i = k; i <= numCols - 1; i++)
    //            {
    //                for (j = k; j <= numCols - 1; j++)
    //                {
    //                    u = i * numCols + j;
    //                    p = elements[u] * elements[u] + mtxImag.elements[u] * mtxImag.elements[u];
    //                    if (p > d)
    //                    {
    //                        d = p;
    //                        pnRow[k] = i;
    //                        pnCol[k] = j;
    //                    }
    //                }
    //            }

    //            // 失败
    //            if (d == 0.0)
    //            {
    //                return false;
    //            }

    //            if (pnRow[k] != k)
    //            {
    //                for (j = 0; j <= numCols - 1; j++)
    //                {
    //                    u = k * numCols + j;
    //                    v = pnRow[k] * numCols + j;
    //                    t = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = t;
    //                    t = mtxImag.elements[u];
    //                    mtxImag.elements[u] = mtxImag.elements[v];
    //                    mtxImag.elements[v] = t;
    //                }
    //            }

    //            if (pnCol[k] != k)
    //            {
    //                for (i = 0; i <= numCols - 1; i++)
    //                {
    //                    u = i * numCols + k;
    //                    v = i * numCols + pnCol[k];
    //                    t = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = t;
    //                    t = mtxImag.elements[u];
    //                    mtxImag.elements[u] = mtxImag.elements[v];
    //                    mtxImag.elements[v] = t;
    //                }
    //            }

    //            l = k * numCols + k;
    //            elements[l] = elements[l] / d; mtxImag.elements[l] = -mtxImag.elements[l] / d;
    //            for (j = 0; j <= numCols - 1; j++)
    //            {
    //                if (j != k)
    //                {
    //                    u = k * numCols + j;
    //                    p = elements[u] * elements[l];
    //                    q = mtxImag.elements[u] * mtxImag.elements[l];
    //                    s = (elements[u] + mtxImag.elements[u]) * (elements[l] + mtxImag.elements[l]);
    //                    elements[u] = p - q;
    //                    mtxImag.elements[u] = s - p - q;
    //                }
    //            }

    //            for (i = 0; i <= numCols - 1; i++)
    //            {
    //                if (i != k)
    //                {
    //                    v = i * numCols + k;
    //                    for (j = 0; j <= numCols - 1; j++)
    //                    {
    //                        if (j != k)
    //                        {
    //                            u = k * numCols + j;
    //                            w = i * numCols + j;
    //                            p = elements[u] * elements[v];
    //                            q = mtxImag.elements[u] * mtxImag.elements[v];
    //                            s = (elements[u] + mtxImag.elements[u]) * (elements[v] + mtxImag.elements[v]);
    //                            t = p - q;
    //                            b = s - p - q;
    //                            elements[w] = elements[w] - t;
    //                            mtxImag.elements[w] = mtxImag.elements[w] - b;
    //                        }
    //                    }
    //                }
    //            }

    //            for (i = 0; i <= numCols - 1; i++)
    //            {
    //                if (i != k)
    //                {
    //                    u = i * numCols + k;
    //                    p = elements[u] * elements[l];
    //                    q = mtxImag.elements[u] * mtxImag.elements[l];
    //                    s = (elements[u] + mtxImag.elements[u]) * (elements[l] + mtxImag.elements[l]);
    //                    elements[u] = q - p;
    //                    mtxImag.elements[u] = p + q - s;
    //                }
    //            }
    //        }

    //        // 调整恢复行列次序
    //        for (k = numCols - 1; k >= 0; k--)
    //        {
    //            if (pnCol[k] != k)
    //            {
    //                for (j = 0; j <= numCols - 1; j++)
    //                {
    //                    u = k * numCols + j;
    //                    v = pnCol[k] * numCols + j;
    //                    t = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = t;
    //                    t = mtxImag.elements[u];
    //                    mtxImag.elements[u] = mtxImag.elements[v];
    //                    mtxImag.elements[v] = t;
    //                }
    //            }

    //            if (pnRow[k] != k)
    //            {
    //                for (i = 0; i <= numCols - 1; i++)
    //                {
    //                    u = i * numCols + k;
    //                    v = i * numCols + pnRow[k];
    //                    t = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = t;
    //                    t = mtxImag.elements[u];
    //                    mtxImag.elements[u] = mtxImag.elements[v];
    //                    mtxImag.elements[v] = t;
    //                }
    //            }
    //        }

    //        // 成功返回
    //        return true;
    //    }

    //    /**
    //     * 对称正定矩阵的求逆
    //     * 
    //     * @return bool型，求逆是否成功
    //     */
    //    public bool InvertSsgj()
    //    {
    //        int i, j, k, m;
    //        double w, g;

    //        // 临时内存
    //        double[] pTmp = new double[numCols];

    //        // 逐列处理
    //        for (k = 0; k <= numCols - 1; k++)
    //        {
    //            w = elements[0];
    //            if (w == 0.0)
    //            {
    //                return false;
    //            }

    //            m = numCols - k - 1;
    //            for (i = 1; i <= numCols - 1; i++)
    //            {
    //                g = elements[i * numCols];
    //                pTmp[i] = g / w;
    //                if (i <= m)
    //                    pTmp[i] = -pTmp[i];
    //                for (j = 1; j <= i; j++)
    //                    elements[(i - 1) * numCols + j - 1] = elements[i * numCols + j] + g * pTmp[j];
    //            }

    //            elements[numCols * numCols - 1] = 1.0 / w;
    //            for (i = 1; i <= numCols - 1; i++)
    //                elements[(numCols - 1) * numCols + i - 1] = pTmp[i];
    //        }

    //        // 行列调整
    //        for (i = 0; i <= numCols - 2; i++)
    //            for (j = i + 1; j <= numCols - 1; j++)
    //                elements[i * numCols + j] = elements[j * numCols + i];

    //        return true;
    //    }

    //    /**
    //     * 托伯利兹矩阵求逆的埃兰特方法
    //     * 
    //     * @return bool型，求逆是否成功
    //     */
    //    public bool InvertTrench()
    //    {
    //        int i, j, k;
    //        double a, s;

    //        // 上三角元素
    //        double[] t = new double[numCols];
    //        // 下三角元素
    //        double[] tt = new double[numCols];

    //        // 上、下三角元素赋值
    //        for (i = 0; i < numCols; ++i)
    //        {
    //            t[i] = GetElement(0, i);
    //            tt[i] = GetElement(i, 0);
    //        }

    //        // 临时缓冲区
    //        double[] c = new double[numCols];
    //        double[] r = new double[numCols];
    //        double[] p = new double[numCols];

    //        // 非Toeplitz矩阵，返回
    //        if (t[0] == 0.0)
    //        {
    //            return false;
    //        }

    //        a = t[0];
    //        c[0] = tt[1] / t[0];
    //        r[0] = t[1] / t[0];

    //        for (k = 0; k <= numCols - 3; k++)
    //        {
    //            s = 0.0;
    //            for (j = 1; j <= k + 1; j++)
    //                s = s + c[k + 1 - j] * tt[j];

    //            s = (s - tt[k + 2]) / a;
    //            for (i = 0; i <= k; i++)
    //                p[i] = c[i] + s * r[k - i];

    //            c[k + 1] = -s;
    //            s = 0.0;
    //            for (j = 1; j <= k + 1; j++)
    //                s = s + r[k + 1 - j] * t[j];

    //            s = (s - t[k + 2]) / a;
    //            for (i = 0; i <= k; i++)
    //            {
    //                r[i] = r[i] + s * c[k - i];
    //                c[k - i] = p[k - i];
    //            }

    //            r[k + 1] = -s;
    //            a = 0.0;
    //            for (j = 1; j <= k + 2; j++)
    //                a = a + t[j] * c[j - 1];

    //            a = t[0] - a;

    //            // 求解失败
    //            if (a == 0.0)
    //            {
    //                return false;
    //            }
    //        }

    //        elements[0] = 1.0 / a;
    //        for (i = 0; i <= numCols - 2; i++)
    //        {
    //            k = i + 1;
    //            j = (i + 1) * numCols;
    //            elements[k] = -r[i] / a;
    //            elements[j] = -c[i] / a;
    //        }

    //        for (i = 0; i <= numCols - 2; i++)
    //        {
    //            for (j = 0; j <= numCols - 2; j++)
    //            {
    //                k = (i + 1) * numCols + j + 1;
    //                elements[k] = elements[i * numCols + j] - c[i] * elements[j + 1];
    //                elements[k] = elements[k] + c[numCols - j - 2] * elements[numCols - i - 1];
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 求行列式值的全选主元高斯消去法
    //     * 
    //     * @return double型，行列式的值
    //     */
    //    public double ComputeDetGauss()
    //    {
    //        int i, j, k, nis = 0, js = 0, l, u, v;
    //        double f, det, q, d;

    //        // 初值
    //        f = 1.0;
    //        det = 1.0;

    //        // 消元
    //        for (k = 0; k <= numCols - 2; k++)
    //        {
    //            q = 0.0;
    //            for (i = k; i <= numCols - 1; i++)
    //            {
    //                for (j = k; j <= numCols - 1; j++)
    //                {
    //                    l = i * numCols + j;
    //                    d = Math.Abs(elements[l]);
    //                    if (d > q)
    //                    {
    //                        q = d;
    //                        nis = i;
    //                        js = j;
    //                    }
    //                }
    //            }

    //            if (q == 0.0)
    //            {
    //                det = 0.0;
    //                return (det);
    //            }

    //            if (nis != k)
    //            {
    //                f = -f;
    //                for (j = k; j <= numCols - 1; j++)
    //                {
    //                    u = k * numCols + j;
    //                    v = nis * numCols + j;
    //                    d = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = d;
    //                }
    //            }

    //            if (js != k)
    //            {
    //                f = -f;
    //                for (i = k; i <= numCols - 1; i++)
    //                {
    //                    u = i * numCols + js;
    //                    v = i * numCols + k;
    //                    d = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = d;
    //                }
    //            }

    //            l = k * numCols + k;
    //            det = det * elements[l];
    //            for (i = k + 1; i <= numCols - 1; i++)
    //            {
    //                d = elements[i * numCols + k] / elements[l];
    //                for (j = k + 1; j <= numCols - 1; j++)
    //                {
    //                    u = i * numCols + j;
    //                    elements[u] = elements[u] - d * elements[k * numCols + j];
    //                }
    //            }
    //        }

    //        // 求值
    //        det = f * det * elements[numCols * numCols - 1];

    //        return (det);
    //    }

    //    /**
    //     * 求矩阵秩的全选主元高斯消去法
    //     * 
    //     * @return int型，矩阵的秩
    //     */
    //    public int ComputeRankGauss()
    //    {
    //        int i, j, k, nn, nis = 0, js = 0, l, ll, u, v;
    //        double q, d;

    //        // 秩小于等于行列数
    //        nn = numRows;
    //        if (numRows >= numCols)
    //            nn = numCols;

    //        k = 0;

    //        // 消元求解
    //        for (l = 0; l <= nn - 1; l++)
    //        {
    //            q = 0.0;
    //            for (i = l; i <= numRows - 1; i++)
    //            {
    //                for (j = l; j <= numCols - 1; j++)
    //                {
    //                    ll = i * numCols + j;
    //                    d = Math.Abs(elements[ll]);
    //                    if (d > q)
    //                    {
    //                        q = d;
    //                        nis = i;
    //                        js = j;
    //                    }
    //                }
    //            }

    //            if (q == 0.0)
    //                return (k);

    //            k = k + 1;
    //            if (nis != l)
    //            {
    //                for (j = l; j <= numCols - 1; j++)
    //                {
    //                    u = l * numCols + j;
    //                    v = nis * numCols + j;
    //                    d = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = d;
    //                }
    //            }
    //            if (js != l)
    //            {
    //                for (i = l; i <= numRows - 1; i++)
    //                {
    //                    u = i * numCols + js;
    //                    v = i * numCols + l;
    //                    d = elements[u];
    //                    elements[u] = elements[v];
    //                    elements[v] = d;
    //                }
    //            }

    //            ll = l * numCols + l;
    //            for (i = l + 1; i <= numCols - 1; i++)
    //            {
    //                d = elements[i * numCols + l] / elements[ll];
    //                for (j = l + 1; j <= numCols - 1; j++)
    //                {
    //                    u = i * numCols + j;
    //                    elements[u] = elements[u] - d * elements[l * numCols + j];
    //                }
    //            }
    //        }

    //        return (k);
    //    }

    //    /**
    //     * 对称正定矩阵的乔里斯基分解与行列式的求值
    //     * 
    //     * @param realDetValue - 返回行列式的值
    //     * @return bool型，求解是否成功
    //     */
    //    public bool ComputeDetCholesky(ref double realDetValue)
    //    {
    //        int i, j, k, u, l;
    //        double d;

    //        // 不满足求解要求
    //        if (elements[0] <= 0.0)
    //            return false;

    //        // 乔里斯基分解

    //        elements[0] = Math.Sqrt(elements[0]);
    //        d = elements[0];

    //        for (i = 1; i <= numCols - 1; i++)
    //        {
    //            u = i * numCols;
    //            elements[u] = elements[u] / elements[0];
    //        }

    //        for (j = 1; j <= numCols - 1; j++)
    //        {
    //            l = j * numCols + j;
    //            for (k = 0; k <= j - 1; k++)
    //            {
    //                u = j * numCols + k;
    //                elements[l] = elements[l] - elements[u] * elements[u];
    //            }

    //            if (elements[l] <= 0.0)
    //                return false;

    //            elements[l] = Math.Sqrt(elements[l]);
    //            d = d * elements[l];

    //            for (i = j + 1; i <= numCols - 1; i++)
    //            {
    //                u = i * numCols + j;
    //                for (k = 0; k <= j - 1; k++)
    //                    elements[u] = elements[u] - elements[i * numCols + k] * elements[j * numCols + k];

    //                elements[u] = elements[u] / elements[l];
    //            }
    //        }

    //        // 行列式求值
    //        realDetValue = d * d;

    //        // 下三角矩阵
    //        for (i = 0; i <= numCols - 2; i++)
    //            for (j = i + 1; j <= numCols - 1; j++)
    //                elements[i * numCols + j] = 0.0;

    //        return true;
    //    }

    //    /**
    //     * 矩阵的三角分解，分解成功后，原矩阵将成为Q矩阵
    //     * 
    //     * @param mtxL - 返回分解后的L矩阵
    //     * @param mtxU - 返回分解后的U矩阵
    //     * @return bool型，求解是否成功
    //     */
    //    public bool SplitLU(CMatrix mtxL, CMatrix mtxU)
    //    {
    //        int i, j, k, w, v, ll;

    //        // 初始化结果矩阵
    //        if (!mtxL.Init(numCols, numCols) ||
    //            !mtxU.Init(numCols, numCols))
    //            return false;

    //        for (k = 0; k <= numCols - 2; k++)
    //        {
    //            ll = k * numCols + k;
    //            if (elements[ll] == 0.0)
    //                return false;

    //            for (i = k + 1; i <= numCols - 1; i++)
    //            {
    //                w = i * numCols + k;
    //                elements[w] = elements[w] / elements[ll];
    //            }

    //            for (i = k + 1; i <= numCols - 1; i++)
    //            {
    //                w = i * numCols + k;
    //                for (j = k + 1; j <= numCols - 1; j++)
    //                {
    //                    v = i * numCols + j;
    //                    elements[v] = elements[v] - elements[w] * elements[k * numCols + j];
    //                }
    //            }
    //        }

    //        for (i = 0; i <= numCols - 1; i++)
    //        {
    //            for (j = 0; j < i; j++)
    //            {
    //                w = i * numCols + j;
    //                mtxL.elements[w] = elements[w];
    //                mtxU.elements[w] = 0.0;
    //            }

    //            w = i * numCols + i;
    //            mtxL.elements[w] = 1.0;
    //            mtxU.elements[w] = elements[w];

    //            for (j = i + 1; j <= numCols - 1; j++)
    //            {
    //                w = i * numCols + j;
    //                mtxL.elements[w] = 0.0;
    //                mtxU.elements[w] = elements[w];
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 一般实矩阵的QR分解，分解成功后，原矩阵将成为R矩阵
    //     * 
    //     * @param mtxQ - 返回分解后的Q矩阵
    //     * @return bool型，求解是否成功
    //     */
    //    public bool SplitQR(CMatrix mtxQ)
    //    {
    //        int i, j, k, l, nn, p, jj;
    //        double u, alpha, w, t;

    //        if (numRows < numCols)
    //            return false;

    //        // 初始化Q矩阵
    //        if (!mtxQ.Init(numRows, numRows))
    //            return false;

    //        // 对角线元素单位化
    //        for (i = 0; i <= numRows - 1; i++)
    //        {
    //            for (j = 0; j <= numRows - 1; j++)
    //            {
    //                l = i * numRows + j;
    //                mtxQ.elements[l] = 0.0;
    //                if (i == j)
    //                    mtxQ.elements[l] = 1.0;
    //            }
    //        }

    //        // 开始分解

    //        nn = numCols;
    //        if (numRows == numCols)
    //            nn = numRows - 1;

    //        for (k = 0; k <= nn - 1; k++)
    //        {
    //            u = 0.0;
    //            l = k * numCols + k;
    //            for (i = k; i <= numRows - 1; i++)
    //            {
    //                w = Math.Abs(elements[i * numCols + k]);
    //                if (w > u)
    //                    u = w;
    //            }

    //            alpha = 0.0;
    //            for (i = k; i <= numRows - 1; i++)
    //            {
    //                t = elements[i * numCols + k] / u;
    //                alpha = alpha + t * t;
    //            }

    //            if (elements[l] > 0.0)
    //                u = -u;

    //            alpha = u * Math.Sqrt(alpha);
    //            if (alpha == 0.0)
    //                return false;

    //            u = Math.Sqrt(2.0 * alpha * (alpha - elements[l]));
    //            if ((u + 1.0) != 1.0)
    //            {
    //                elements[l] = (elements[l] - alpha) / u;
    //                for (i = k + 1; i <= numRows - 1; i++)
    //                {
    //                    p = i * numCols + k;
    //                    elements[p] = elements[p] / u;
    //                }

    //                for (j = 0; j <= numRows - 1; j++)
    //                {
    //                    t = 0.0;
    //                    for (jj = k; jj <= numRows - 1; jj++)
    //                        t = t + elements[jj * numCols + k] * mtxQ.elements[jj * numRows + j];

    //                    for (i = k; i <= numRows - 1; i++)
    //                    {
    //                        p = i * numRows + j;
    //                        mtxQ.elements[p] = mtxQ.elements[p] - 2.0 * t * elements[i * numCols + k];
    //                    }
    //                }

    //                for (j = k + 1; j <= numCols - 1; j++)
    //                {
    //                    t = 0.0;

    //                    for (jj = k; jj <= numRows - 1; jj++)
    //                        t = t + elements[jj * numCols + k] * elements[jj * numCols + j];

    //                    for (i = k; i <= numRows - 1; i++)
    //                    {
    //                        p = i * numCols + j;
    //                        elements[p] = elements[p] - 2.0 * t * elements[i * numCols + k];
    //                    }
    //                }

    //                elements[l] = alpha;
    //                for (i = k + 1; i <= numRows - 1; i++)
    //                    elements[i * numCols + k] = 0.0;
    //            }
    //        }

    //        // 调整元素
    //        for (i = 0; i <= numRows - 2; i++)
    //        {
    //            for (j = i + 1; j <= numRows - 1; j++)
    //            {
    //                p = i * numRows + j;
    //                l = j * numRows + i;
    //                t = mtxQ.elements[p];
    //                mtxQ.elements[p] = mtxQ.elements[l];
    //                mtxQ.elements[l] = t;
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 一般实矩阵的奇异值分解，分解成功后，原矩阵对角线元素就是矩阵的奇异值
    //     * 
    //     * @param mtxU - 返回分解后的U矩阵
    //     * @param mtxV - 返回分解后的V矩阵
    //     * @param eps - 计算精度
    //     * @return bool型，求解是否成功
    //     */
    //    public bool SplitUV(CMatrix mtxU, CMatrix mtxV, double eps)
    //    {
    //        int i, j, k, l, it, ll, kk, ix, iy, mm, nn, iz, m1, ks;
    //        double d, dd, t, sm, sm1, em1, sk, ek, b, c, shh;
    //        double[] fg = new double[2];
    //        double[] cs = new double[2];

    //        int m = numRows;
    //        int n = numCols;

    //        // 初始化U, V矩阵
    //        if (!mtxU.Init(m, m) || !mtxV.Init(n, n))
    //            return false;

    //        // 临时缓冲区
    //        int ka = Math.Max(m, n) + 1;
    //        double[] s = new double[ka];
    //        double[] e = new double[ka];
    //        double[] w = new double[ka];

    //        // 指定迭代次数为60
    //        it = 60;
    //        k = n;

    //        if (m - 1 < n)
    //            k = m - 1;

    //        l = m;
    //        if (n - 2 < m)
    //            l = n - 2;
    //        if (l < 0)
    //            l = 0;

    //        // 循环迭代计算
    //        ll = k;
    //        if (l > k)
    //            ll = l;
    //        if (ll >= 1)
    //        {
    //            for (kk = 1; kk <= ll; kk++)
    //            {
    //                if (kk <= k)
    //                {
    //                    d = 0.0;
    //                    for (i = kk; i <= m; i++)
    //                    {
    //                        ix = (i - 1) * n + kk - 1;
    //                        d = d + elements[ix] * elements[ix];
    //                    }

    //                    s[kk - 1] = Math.Sqrt(d);
    //                    if (s[kk - 1] != 0.0)
    //                    {
    //                        ix = (kk - 1) * n + kk - 1;
    //                        if (elements[ix] != 0.0)
    //                        {
    //                            s[kk - 1] = Math.Abs(s[kk - 1]);
    //                            if (elements[ix] < 0.0)
    //                                s[kk - 1] = -s[kk - 1];
    //                        }

    //                        for (i = kk; i <= m; i++)
    //                        {
    //                            iy = (i - 1) * n + kk - 1;
    //                            elements[iy] = elements[iy] / s[kk - 1];
    //                        }

    //                        elements[ix] = 1.0 + elements[ix];
    //                    }

    //                    s[kk - 1] = -s[kk - 1];
    //                }

    //                if (n >= kk + 1)
    //                {
    //                    for (j = kk + 1; j <= n; j++)
    //                    {
    //                        if ((kk <= k) && (s[kk - 1] != 0.0))
    //                        {
    //                            d = 0.0;
    //                            for (i = kk; i <= m; i++)
    //                            {
    //                                ix = (i - 1) * n + kk - 1;
    //                                iy = (i - 1) * n + j - 1;
    //                                d = d + elements[ix] * elements[iy];
    //                            }

    //                            d = -d / elements[(kk - 1) * n + kk - 1];
    //                            for (i = kk; i <= m; i++)
    //                            {
    //                                ix = (i - 1) * n + j - 1;
    //                                iy = (i - 1) * n + kk - 1;
    //                                elements[ix] = elements[ix] + d * elements[iy];
    //                            }
    //                        }

    //                        e[j - 1] = elements[(kk - 1) * n + j - 1];
    //                    }
    //                }

    //                if (kk <= k)
    //                {
    //                    for (i = kk; i <= m; i++)
    //                    {
    //                        ix = (i - 1) * m + kk - 1;
    //                        iy = (i - 1) * n + kk - 1;
    //                        mtxU.elements[ix] = elements[iy];
    //                    }
    //                }

    //                if (kk <= l)
    //                {
    //                    d = 0.0;
    //                    for (i = kk + 1; i <= n; i++)
    //                        d = d + e[i - 1] * e[i - 1];

    //                    e[kk - 1] = Math.Sqrt(d);
    //                    if (e[kk - 1] != 0.0)
    //                    {
    //                        if (e[kk] != 0.0)
    //                        {
    //                            e[kk - 1] = Math.Abs(e[kk - 1]);
    //                            if (e[kk] < 0.0)
    //                                e[kk - 1] = -e[kk - 1];
    //                        }

    //                        for (i = kk + 1; i <= n; i++)
    //                            e[i - 1] = e[i - 1] / e[kk - 1];

    //                        e[kk] = 1.0 + e[kk];
    //                    }

    //                    e[kk - 1] = -e[kk - 1];
    //                    if ((kk + 1 <= m) && (e[kk - 1] != 0.0))
    //                    {
    //                        for (i = kk + 1; i <= m; i++)
    //                            w[i - 1] = 0.0;

    //                        for (j = kk + 1; j <= n; j++)
    //                            for (i = kk + 1; i <= m; i++)
    //                                w[i - 1] = w[i - 1] + e[j - 1] * elements[(i - 1) * n + j - 1];

    //                        for (j = kk + 1; j <= n; j++)
    //                        {
    //                            for (i = kk + 1; i <= m; i++)
    //                            {
    //                                ix = (i - 1) * n + j - 1;
    //                                elements[ix] = elements[ix] - w[i - 1] * e[j - 1] / e[kk];
    //                            }
    //                        }
    //                    }

    //                    for (i = kk + 1; i <= n; i++)
    //                        mtxV.elements[(i - 1) * n + kk - 1] = e[i - 1];
    //                }
    //            }
    //        }

    //        mm = n;
    //        if (m + 1 < n)
    //            mm = m + 1;
    //        if (k < n)
    //            s[k] = elements[k * n + k];
    //        if (m < mm)
    //            s[mm - 1] = 0.0;
    //        if (l + 1 < mm)
    //            e[l] = elements[l * n + mm - 1];

    //        e[mm - 1] = 0.0;
    //        nn = m;
    //        if (m > n)
    //            nn = n;
    //        if (nn >= k + 1)
    //        {
    //            for (j = k + 1; j <= nn; j++)
    //            {
    //                for (i = 1; i <= m; i++)
    //                    mtxU.elements[(i - 1) * m + j - 1] = 0.0;
    //                mtxU.elements[(j - 1) * m + j - 1] = 1.0;
    //            }
    //        }

    //        if (k >= 1)
    //        {
    //            for (ll = 1; ll <= k; ll++)
    //            {
    //                kk = k - ll + 1;
    //                iz = (kk - 1) * m + kk - 1;
    //                if (s[kk - 1] != 0.0)
    //                {
    //                    if (nn >= kk + 1)
    //                    {
    //                        for (j = kk + 1; j <= nn; j++)
    //                        {
    //                            d = 0.0;
    //                            for (i = kk; i <= m; i++)
    //                            {
    //                                ix = (i - 1) * m + kk - 1;
    //                                iy = (i - 1) * m + j - 1;
    //                                d = d + mtxU.elements[ix] * mtxU.elements[iy] / mtxU.elements[iz];
    //                            }

    //                            d = -d;
    //                            for (i = kk; i <= m; i++)
    //                            {
    //                                ix = (i - 1) * m + j - 1;
    //                                iy = (i - 1) * m + kk - 1;
    //                                mtxU.elements[ix] = mtxU.elements[ix] + d * mtxU.elements[iy];
    //                            }
    //                        }
    //                    }

    //                    for (i = kk; i <= m; i++)
    //                    {
    //                        ix = (i - 1) * m + kk - 1;
    //                        mtxU.elements[ix] = -mtxU.elements[ix];
    //                    }

    //                    mtxU.elements[iz] = 1.0 + mtxU.elements[iz];
    //                    if (kk - 1 >= 1)
    //                    {
    //                        for (i = 1; i <= kk - 1; i++)
    //                            mtxU.elements[(i - 1) * m + kk - 1] = 0.0;
    //                    }
    //                }
    //                else
    //                {
    //                    for (i = 1; i <= m; i++)
    //                        mtxU.elements[(i - 1) * m + kk - 1] = 0.0;
    //                    mtxU.elements[(kk - 1) * m + kk - 1] = 1.0;
    //                }
    //            }
    //        }

    //        for (ll = 1; ll <= n; ll++)
    //        {
    //            kk = n - ll + 1;
    //            iz = kk * n + kk - 1;

    //            if ((kk <= l) && (e[kk - 1] != 0.0))
    //            {
    //                for (j = kk + 1; j <= n; j++)
    //                {
    //                    d = 0.0;
    //                    for (i = kk + 1; i <= n; i++)
    //                    {
    //                        ix = (i - 1) * n + kk - 1;
    //                        iy = (i - 1) * n + j - 1;
    //                        d = d + mtxV.elements[ix] * mtxV.elements[iy] / mtxV.elements[iz];
    //                    }

    //                    d = -d;
    //                    for (i = kk + 1; i <= n; i++)
    //                    {
    //                        ix = (i - 1) * n + j - 1;
    //                        iy = (i - 1) * n + kk - 1;
    //                        mtxV.elements[ix] = mtxV.elements[ix] + d * mtxV.elements[iy];
    //                    }
    //                }
    //            }

    //            for (i = 1; i <= n; i++)
    //                mtxV.elements[(i - 1) * n + kk - 1] = 0.0;

    //            mtxV.elements[iz - n] = 1.0;
    //        }

    //        for (i = 1; i <= m; i++)
    //            for (j = 1; j <= n; j++)
    //                elements[(i - 1) * n + j - 1] = 0.0;

    //        m1 = mm;
    //        it = 60;
    //        while (true)
    //        {
    //            if (mm == 0)
    //            {
    //                ppp(elements, e, s, mtxV.elements, m, n);
    //                return true;
    //            }
    //            if (it == 0)
    //            {
    //                ppp(elements, e, s, mtxV.elements, m, n);
    //                return false;
    //            }

    //            kk = mm - 1;
    //            while ((kk != 0) && (Math.Abs(e[kk - 1]) != 0.0))
    //            {
    //                d = Math.Abs(s[kk - 1]) + Math.Abs(s[kk]);
    //                dd = Math.Abs(e[kk - 1]);
    //                if (dd > eps * d)
    //                    kk = kk - 1;
    //                else
    //                    e[kk - 1] = 0.0;
    //            }

    //            if (kk == mm - 1)
    //            {
    //                kk = kk + 1;
    //                if (s[kk - 1] < 0.0)
    //                {
    //                    s[kk - 1] = -s[kk - 1];
    //                    for (i = 1; i <= n; i++)
    //                    {
    //                        ix = (i - 1) * n + kk - 1;
    //                        mtxV.elements[ix] = -mtxV.elements[ix];
    //                    }
    //                }

    //                while ((kk != m1) && (s[kk - 1] < s[kk]))
    //                {
    //                    d = s[kk - 1];
    //                    s[kk - 1] = s[kk];
    //                    s[kk] = d;
    //                    if (kk < n)
    //                    {
    //                        for (i = 1; i <= n; i++)
    //                        {
    //                            ix = (i - 1) * n + kk - 1;
    //                            iy = (i - 1) * n + kk;
    //                            d = mtxV.elements[ix];
    //                            mtxV.elements[ix] = mtxV.elements[iy];
    //                            mtxV.elements[iy] = d;
    //                        }
    //                    }

    //                    if (kk < m)
    //                    {
    //                        for (i = 1; i <= m; i++)
    //                        {
    //                            ix = (i - 1) * m + kk - 1;
    //                            iy = (i - 1) * m + kk;
    //                            d = mtxU.elements[ix];
    //                            mtxU.elements[ix] = mtxU.elements[iy];
    //                            mtxU.elements[iy] = d;
    //                        }
    //                    }

    //                    kk = kk + 1;
    //                }

    //                it = 60;
    //                mm = mm - 1;
    //            }
    //            else
    //            {
    //                ks = mm;
    //                while ((ks > kk) && (Math.Abs(s[ks - 1]) != 0.0))
    //                {
    //                    d = 0.0;
    //                    if (ks != mm)
    //                        d = d + Math.Abs(e[ks - 1]);
    //                    if (ks != kk + 1)
    //                        d = d + Math.Abs(e[ks - 2]);

    //                    dd = Math.Abs(s[ks - 1]);
    //                    if (dd > eps * d)
    //                        ks = ks - 1;
    //                    else
    //                        s[ks - 1] = 0.0;
    //                }

    //                if (ks == kk)
    //                {
    //                    kk = kk + 1;
    //                    d = Math.Abs(s[mm - 1]);
    //                    t = Math.Abs(s[mm - 2]);
    //                    if (t > d)
    //                        d = t;

    //                    t = Math.Abs(e[mm - 2]);
    //                    if (t > d)
    //                        d = t;

    //                    t = Math.Abs(s[kk - 1]);
    //                    if (t > d)
    //                        d = t;

    //                    t = Math.Abs(e[kk - 1]);
    //                    if (t > d)
    //                        d = t;

    //                    sm = s[mm - 1] / d;
    //                    sm1 = s[mm - 2] / d;
    //                    em1 = e[mm - 2] / d;
    //                    sk = s[kk - 1] / d;
    //                    ek = e[kk - 1] / d;
    //                    b = ((sm1 + sm) * (sm1 - sm) + em1 * em1) / 2.0;
    //                    c = sm * em1;
    //                    c = c * c;
    //                    shh = 0.0;

    //                    if ((b != 0.0) || (c != 0.0))
    //                    {
    //                        shh = Math.Sqrt(b * b + c);
    //                        if (b < 0.0)
    //                            shh = -shh;

    //                        shh = c / (b + shh);
    //                    }

    //                    fg[0] = (sk + sm) * (sk - sm) - shh;
    //                    fg[1] = sk * ek;
    //                    for (i = kk; i <= mm - 1; i++)
    //                    {
    //                        sss(fg, cs);
    //                        if (i != kk)
    //                            e[i - 2] = fg[0];

    //                        fg[0] = cs[0] * s[i - 1] + cs[1] * e[i - 1];
    //                        e[i - 1] = cs[0] * e[i - 1] - cs[1] * s[i - 1];
    //                        fg[1] = cs[1] * s[i];
    //                        s[i] = cs[0] * s[i];

    //                        if ((cs[0] != 1.0) || (cs[1] != 0.0))
    //                        {
    //                            for (j = 1; j <= n; j++)
    //                            {
    //                                ix = (j - 1) * n + i - 1;
    //                                iy = (j - 1) * n + i;
    //                                d = cs[0] * mtxV.elements[ix] + cs[1] * mtxV.elements[iy];
    //                                mtxV.elements[iy] = -cs[1] * mtxV.elements[ix] + cs[0] * mtxV.elements[iy];
    //                                mtxV.elements[ix] = d;
    //                            }
    //                        }

    //                        sss(fg, cs);
    //                        s[i - 1] = fg[0];
    //                        fg[0] = cs[0] * e[i - 1] + cs[1] * s[i];
    //                        s[i] = -cs[1] * e[i - 1] + cs[0] * s[i];
    //                        fg[1] = cs[1] * e[i];
    //                        e[i] = cs[0] * e[i];

    //                        if (i < m)
    //                        {
    //                            if ((cs[0] != 1.0) || (cs[1] != 0.0))
    //                            {
    //                                for (j = 1; j <= m; j++)
    //                                {
    //                                    ix = (j - 1) * m + i - 1;
    //                                    iy = (j - 1) * m + i;
    //                                    d = cs[0] * mtxU.elements[ix] + cs[1] * mtxU.elements[iy];
    //                                    mtxU.elements[iy] = -cs[1] * mtxU.elements[ix] + cs[0] * mtxU.elements[iy];
    //                                    mtxU.elements[ix] = d;
    //                                }
    //                            }
    //                        }
    //                    }

    //                    e[mm - 2] = fg[0];
    //                    it = it - 1;
    //                }
    //                else
    //                {
    //                    if (ks == mm)
    //                    {
    //                        kk = kk + 1;
    //                        fg[1] = e[mm - 2];
    //                        e[mm - 2] = 0.0;
    //                        for (ll = kk; ll <= mm - 1; ll++)
    //                        {
    //                            i = mm + kk - ll - 1;
    //                            fg[0] = s[i - 1];
    //                            sss(fg, cs);
    //                            s[i - 1] = fg[0];
    //                            if (i != kk)
    //                            {
    //                                fg[1] = -cs[1] * e[i - 2];
    //                                e[i - 2] = cs[0] * e[i - 2];
    //                            }

    //                            if ((cs[0] != 1.0) || (cs[1] != 0.0))
    //                            {
    //                                for (j = 1; j <= n; j++)
    //                                {
    //                                    ix = (j - 1) * n + i - 1;
    //                                    iy = (j - 1) * n + mm - 1;
    //                                    d = cs[0] * mtxV.elements[ix] + cs[1] * mtxV.elements[iy];
    //                                    mtxV.elements[iy] = -cs[1] * mtxV.elements[ix] + cs[0] * mtxV.elements[iy];
    //                                    mtxV.elements[ix] = d;
    //                                }
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        kk = ks + 1;
    //                        fg[1] = e[kk - 2];
    //                        e[kk - 2] = 0.0;
    //                        for (i = kk; i <= mm; i++)
    //                        {
    //                            fg[0] = s[i - 1];
    //                            sss(fg, cs);
    //                            s[i - 1] = fg[0];
    //                            fg[1] = -cs[1] * e[i - 1];
    //                            e[i - 1] = cs[0] * e[i - 1];
    //                            if ((cs[0] != 1.0) || (cs[1] != 0.0))
    //                            {
    //                                for (j = 1; j <= m; j++)
    //                                {
    //                                    ix = (j - 1) * m + i - 1;
    //                                    iy = (j - 1) * m + kk - 2;
    //                                    d = cs[0] * mtxU.elements[ix] + cs[1] * mtxU.elements[iy];
    //                                    mtxU.elements[iy] = -cs[1] * mtxU.elements[ix] + cs[0] * mtxU.elements[iy];
    //                                    mtxU.elements[ix] = d;
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /**
    //     * 内部函数，由SplitUV函数调用
    //     */
    //    private void ppp(double[] a, double[] e, double[] s, double[] v, int m, int n)
    //    {
    //        int i, j, p, q;
    //        double d;

    //        if (m >= n)
    //            i = n;
    //        else
    //            i = m;

    //        for (j = 1; j <= i - 1; j++)
    //        {
    //            a[(j - 1) * n + j - 1] = s[j - 1];
    //            a[(j - 1) * n + j] = e[j - 1];
    //        }

    //        a[(i - 1) * n + i - 1] = s[i - 1];
    //        if (m < n)
    //            a[(i - 1) * n + i] = e[i - 1];

    //        for (i = 1; i <= n - 1; i++)
    //        {
    //            for (j = i + 1; j <= n; j++)
    //            {
    //                p = (i - 1) * n + j - 1;
    //                q = (j - 1) * n + i - 1;
    //                d = v[p];
    //                v[p] = v[q];
    //                v[q] = d;
    //            }
    //        }
    //    }

    //    /**
    //     * 内部函数，由SplitUV函数调用
    //     */
    //    private void sss(double[] fg, double[] cs)
    //    {
    //        double r, d;

    //        if ((Math.Abs(fg[0]) + Math.Abs(fg[1])) == 0.0)
    //        {
    //            cs[0] = 1.0;
    //            cs[1] = 0.0;
    //            d = 0.0;
    //        }
    //        else
    //        {
    //            d = Math.Sqrt(fg[0] * fg[0] + fg[1] * fg[1]);
    //            if (Math.Abs(fg[0]) > Math.Abs(fg[1]))
    //            {
    //                d = Math.Abs(d);
    //                if (fg[0] < 0.0)
    //                    d = -d;
    //            }
    //            if (Math.Abs(fg[1]) >= Math.Abs(fg[0]))
    //            {
    //                d = Math.Abs(d);
    //                if (fg[1] < 0.0)
    //                    d = -d;
    //            }

    //            cs[0] = fg[0] / d;
    //            cs[1] = fg[1] / d;
    //        }

    //        r = 1.0;
    //        if (Math.Abs(fg[0]) > Math.Abs(fg[1]))
    //            r = cs[1];
    //        else if (cs[0] != 0.0)
    //            r = 1.0 / cs[0];

    //        fg[0] = d;
    //        fg[1] = r;
    //    }

    //    /**
    //     * 求广义逆的奇异值分解法，分解成功后，原矩阵对角线元素就是矩阵的奇异值
    //     * 
    //     * @param mtxAP - 返回原矩阵的广义逆矩阵
    //     * @param mtxU - 返回分解后的U矩阵
    //     * @param mtxV - 返回分解后的V矩阵
    //     * @param eps - 计算精度
    //     * @return bool型，求解是否成功
    //     */
    //    public bool InvertUV(CMatrix mtxAP, CMatrix mtxU, CMatrix mtxV, double eps)
    //    {
    //        int i, j, k, l, t, p, q, f;

    //        // 调用奇异值分解
    //        if (!SplitUV(mtxU, mtxV, eps))
    //            return false;

    //        int m = numRows;
    //        int n = numCols;

    //        // 初始化广义逆矩阵
    //        if (!mtxAP.Init(n, m))
    //            return false;

    //        // 计算广义逆矩阵

    //        j = n;
    //        if (m < n)
    //            j = m;
    //        j = j - 1;
    //        k = 0;
    //        while ((k <= j) && (elements[k * n + k] != 0.0))
    //            k = k + 1;

    //        k = k - 1;
    //        for (i = 0; i <= n - 1; i++)
    //        {
    //            for (j = 0; j <= m - 1; j++)
    //            {
    //                t = i * m + j;
    //                mtxAP.elements[t] = 0.0;
    //                for (l = 0; l <= k; l++)
    //                {
    //                    f = l * n + i;
    //                    p = j * m + l;
    //                    q = l * n + l;
    //                    mtxAP.elements[t] = mtxAP.elements[t] + mtxV.elements[f] * mtxU.elements[p] / elements[q];
    //                }
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 约化对称矩阵为对称三对角阵的豪斯荷尔德变换法
    //     * 
    //     * @param mtxQ - 返回豪斯荷尔德变换的乘积矩阵Q
    //     * @param mtxT - 返回求得的对称三对角阵
    //     * @param dblB - 一维数组，长度为矩阵的阶数，返回对称三对角阵的主对角线元素
    //     * @param dblC - 一维数组，长度为矩阵的阶数，前n-1个元素返回对称三对角阵的
    //     *               次对角线元素
    //     * @return bool型，求解是否成功
    //     */
    //    public bool MakeSymTri(CMatrix mtxQ, CMatrix mtxT, double[] dblB, double[] dblC)
    //    {
    //        int i, j, k, u;
    //        double h, f, g, h2;

    //        // 初始化矩阵Q和T
    //        if (!mtxQ.Init(numCols, numCols) ||
    //            !mtxT.Init(numCols, numCols))
    //            return false;

    //        if (dblB == null || dblC == null)
    //            return false;

    //        for (i = 0; i <= numCols - 1; i++)
    //        {
    //            for (j = 0; j <= numCols - 1; j++)
    //            {
    //                u = i * numCols + j;
    //                mtxQ.elements[u] = elements[u];
    //            }
    //        }

    //        for (i = numCols - 1; i >= 1; i--)
    //        {
    //            h = 0.0;
    //            if (i > 1)
    //            {
    //                for (k = 0; k <= i - 1; k++)
    //                {
    //                    u = i * numCols + k;
    //                    h = h + mtxQ.elements[u] * mtxQ.elements[u];
    //                }
    //            }

    //            if (h == 0.0)
    //            {
    //                dblC[i] = 0.0;
    //                if (i == 1)
    //                    dblC[i] = mtxQ.elements[i * numCols + i - 1];
    //                dblB[i] = 0.0;
    //            }
    //            else
    //            {
    //                dblC[i] = Math.Sqrt(h);
    //                u = i * numCols + i - 1;
    //                if (mtxQ.elements[u] > 0.0)
    //                    dblC[i] = -dblC[i];

    //                h = h - mtxQ.elements[u] * dblC[i];
    //                mtxQ.elements[u] = mtxQ.elements[u] - dblC[i];
    //                f = 0.0;
    //                for (j = 0; j <= i - 1; j++)
    //                {
    //                    mtxQ.elements[j * numCols + i] = mtxQ.elements[i * numCols + j] / h;
    //                    g = 0.0;
    //                    for (k = 0; k <= j; k++)
    //                        g = g + mtxQ.elements[j * numCols + k] * mtxQ.elements[i * numCols + k];

    //                    if (j + 1 <= i - 1)
    //                        for (k = j + 1; k <= i - 1; k++)
    //                            g = g + mtxQ.elements[k * numCols + j] * mtxQ.elements[i * numCols + k];

    //                    dblC[j] = g / h;
    //                    f = f + g * mtxQ.elements[j * numCols + i];
    //                }

    //                h2 = f / (h + h);
    //                for (j = 0; j <= i - 1; j++)
    //                {
    //                    f = mtxQ.elements[i * numCols + j];
    //                    g = dblC[j] - h2 * f;
    //                    dblC[j] = g;
    //                    for (k = 0; k <= j; k++)
    //                    {
    //                        u = j * numCols + k;
    //                        mtxQ.elements[u] = mtxQ.elements[u] - f * dblC[k] - g * mtxQ.elements[i * numCols + k];
    //                    }
    //                }

    //                dblB[i] = h;
    //            }
    //        }

    //        for (i = 0; i <= numCols - 2; i++)
    //            dblC[i] = dblC[i + 1];

    //        dblC[numCols - 1] = 0.0;
    //        dblB[0] = 0.0;
    //        for (i = 0; i <= numCols - 1; i++)
    //        {
    //            if ((dblB[i] != (double)0.0) && (i - 1 >= 0))
    //            {
    //                for (j = 0; j <= i - 1; j++)
    //                {
    //                    g = 0.0;
    //                    for (k = 0; k <= i - 1; k++)
    //                        g = g + mtxQ.elements[i * numCols + k] * mtxQ.elements[k * numCols + j];

    //                    for (k = 0; k <= i - 1; k++)
    //                    {
    //                        u = k * numCols + j;
    //                        mtxQ.elements[u] = mtxQ.elements[u] - g * mtxQ.elements[k * numCols + i];
    //                    }
    //                }
    //            }

    //            u = i * numCols + i;
    //            dblB[i] = mtxQ.elements[u]; mtxQ.elements[u] = 1.0;
    //            if (i - 1 >= 0)
    //            {
    //                for (j = 0; j <= i - 1; j++)
    //                {
    //                    mtxQ.elements[i * numCols + j] = 0.0;
    //                    mtxQ.elements[j * numCols + i] = 0.0;
    //                }
    //            }
    //        }

    //        // 构造对称三对角矩阵
    //        for (i = 0; i < numCols; ++i)
    //        {
    //            for (j = 0; j < numCols; ++j)
    //            {
    //                mtxT.SetElement(i, j, 0);
    //                k = i - j;
    //                if (k == 0)
    //                    mtxT.SetElement(i, j, dblB[j]);
    //                else if (k == 1)
    //                    mtxT.SetElement(i, j, dblC[j]);
    //                else if (k == -1)
    //                    mtxT.SetElement(i, j, dblC[i]);
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 实对称三对角阵的全部特征值与特征向量的计算
    //     * 
    //     * @param dblB - 一维数组，长度为矩阵的阶数，传入对称三对角阵的主对角线元素；
    //     *			     返回时存放全部特征值。
    //     * @param dblC - 一维数组，长度为矩阵的阶数，前n-1个元素传入对称三对角阵的
    //     *               次对角线元素
    //     * @param mtxQ - 如果传入单位矩阵，则返回实对称三对角阵的特征值向量矩阵；
    //     *			     如果传入MakeSymTri函数求得的矩阵A的豪斯荷尔德变换的乘积
    //     *               矩阵Q，则返回矩阵A的特征值向量矩阵。其中第i列为与数组dblB
    //     *               中第j个特征值对应的特征向量。
    //     * @param nMaxIt - 迭代次数
    //     * @param eps - 计算精度
    //     * @return bool型，求解是否成功
    //     */
    //    public bool ComputeEvSymTri(double[] dblB, double[] dblC, CMatrix mtxQ, int nMaxIt, double eps)
    //    {
    //        int i, j, k, m, it, u, v;
    //        double d, f, h, g, p, r, e, s;

    //        // 初值
    //        int n = mtxQ.GetNumCols();
    //        dblC[n - 1] = 0.0;
    //        d = 0.0;
    //        f = 0.0;

    //        // 迭代计算

    //        for (j = 0; j <= n - 1; j++)
    //        {
    //            it = 0;
    //            h = eps * (Math.Abs(dblB[j]) + Math.Abs(dblC[j]));
    //            if (h > d)
    //                d = h;

    //            m = j;
    //            while ((m <= n - 1) && (Math.Abs(dblC[m]) > d))
    //                m = m + 1;

    //            if (m != j)
    //            {
    //                do
    //                {
    //                    if (it == nMaxIt)
    //                        return false;

    //                    it = it + 1;
    //                    g = dblB[j];
    //                    p = (dblB[j + 1] - g) / (2.0 * dblC[j]);
    //                    r = Math.Sqrt(p * p + 1.0);
    //                    if (p >= 0.0)
    //                        dblB[j] = dblC[j] / (p + r);
    //                    else
    //                        dblB[j] = dblC[j] / (p - r);

    //                    h = g - dblB[j];
    //                    for (i = j + 1; i <= n - 1; i++)
    //                        dblB[i] = dblB[i] - h;

    //                    f = f + h;
    //                    p = dblB[m];
    //                    e = 1.0;
    //                    s = 0.0;
    //                    for (i = m - 1; i >= j; i--)
    //                    {
    //                        g = e * dblC[i];
    //                        h = e * p;
    //                        if (Math.Abs(p) >= Math.Abs(dblC[i]))
    //                        {
    //                            e = dblC[i] / p;
    //                            r = Math.Sqrt(e * e + 1.0);
    //                            dblC[i + 1] = s * p * r;
    //                            s = e / r;
    //                            e = 1.0 / r;
    //                        }
    //                        else
    //                        {
    //                            e = p / dblC[i];
    //                            r = Math.Sqrt(e * e + 1.0);
    //                            dblC[i + 1] = s * dblC[i] * r;
    //                            s = 1.0 / r;
    //                            e = e / r;
    //                        }

    //                        p = e * dblB[i] - s * g;
    //                        dblB[i + 1] = h + s * (e * g + s * dblB[i]);
    //                        for (k = 0; k <= n - 1; k++)
    //                        {
    //                            u = k * n + i + 1;
    //                            v = u - 1;
    //                            h = mtxQ.elements[u];
    //                            mtxQ.elements[u] = s * mtxQ.elements[v] + e * h;
    //                            mtxQ.elements[v] = e * mtxQ.elements[v] - s * h;
    //                        }
    //                    }

    //                    dblC[j] = s * p;
    //                    dblB[j] = e * p;

    //                } while (Math.Abs(dblC[j]) > d);
    //            }

    //            dblB[j] = dblB[j] + f;
    //        }

    //        for (i = 0; i <= n - 1; i++)
    //        {
    //            k = i;
    //            p = dblB[i];
    //            if (i + 1 <= n - 1)
    //            {
    //                j = i + 1;
    //                while ((j <= n - 1) && (dblB[j] <= p))
    //                {
    //                    k = j;
    //                    p = dblB[j];
    //                    j = j + 1;
    //                }
    //            }

    //            if (k != i)
    //            {
    //                dblB[k] = dblB[i];
    //                dblB[i] = p;
    //                for (j = 0; j <= n - 1; j++)
    //                {
    //                    u = j * n + i;
    //                    v = j * n + k;
    //                    p = mtxQ.elements[u];
    //                    mtxQ.elements[u] = mtxQ.elements[v];
    //                    mtxQ.elements[v] = p;
    //                }
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 约化一般实矩阵为赫申伯格矩阵的初等相似变换法
    //     */
    //    public void MakeHberg()
    //    {
    //        int i = 0, j, k, u, v;
    //        double d, t;

    //        for (k = 1; k <= numCols - 2; k++)
    //        {
    //            d = 0.0;
    //            for (j = k; j <= numCols - 1; j++)
    //            {
    //                u = j * numCols + k - 1;
    //                t = elements[u];
    //                if (Math.Abs(t) > Math.Abs(d))
    //                {
    //                    d = t;
    //                    i = j;
    //                }
    //            }

    //            if (d != 0.0)
    //            {
    //                if (i != k)
    //                {
    //                    for (j = k - 1; j <= numCols - 1; j++)
    //                    {
    //                        u = i * numCols + j;
    //                        v = k * numCols + j;
    //                        t = elements[u];
    //                        elements[u] = elements[v];
    //                        elements[v] = t;
    //                    }

    //                    for (j = 0; j <= numCols - 1; j++)
    //                    {
    //                        u = j * numCols + i;
    //                        v = j * numCols + k;
    //                        t = elements[u];
    //                        elements[u] = elements[v];
    //                        elements[v] = t;
    //                    }
    //                }

    //                for (i = k + 1; i <= numCols - 1; i++)
    //                {
    //                    u = i * numCols + k - 1;
    //                    t = elements[u] / d;
    //                    elements[u] = 0.0;
    //                    for (j = k; j <= numCols - 1; j++)
    //                    {
    //                        v = i * numCols + j;
    //                        elements[v] = elements[v] - t * elements[k * numCols + j];
    //                    }

    //                    for (j = 0; j <= numCols - 1; j++)
    //                    {
    //                        v = j * numCols + k;
    //                        elements[v] = elements[v] + t * elements[j * numCols + i];
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /**
    //     * 求赫申伯格矩阵全部特征值的QR方法
    //     * 
    //     * @param dblU - 一维数组，长度为矩阵的阶数，返回时存放特征值的实部
    //     * @param dblV - 一维数组，长度为矩阵的阶数，返回时存放特征值的虚部
    //     * @param nMaxIt - 迭代次数
    //     * @param eps - 计算精度
    //     * @return bool型，求解是否成功
    //     */
    //    public bool ComputeEvHBerg(double[] dblU, double[] dblV, int nMaxIt, double eps)
    //    {
    //        int m, it, i, j, k, l, ii, jj, kk, ll;
    //        double b, c, w, g, xy, p, q, r, x, s, e, f, z, y;

    //        int n = numCols;

    //        it = 0;
    //        m = n;
    //        while (m != 0)
    //        {
    //            l = m - 1;
    //            while ((l > 0) && (Math.Abs(elements[l * n + l - 1]) >
    //                eps * (Math.Abs(elements[(l - 1) * n + l - 1]) + Math.Abs(elements[l * n + l]))))
    //                l = l - 1;

    //            ii = (m - 1) * n + m - 1;
    //            jj = (m - 1) * n + m - 2;
    //            kk = (m - 2) * n + m - 1;
    //            ll = (m - 2) * n + m - 2;
    //            if (l == m - 1)
    //            {
    //                dblU[m - 1] = elements[(m - 1) * n + m - 1];
    //                dblV[m - 1] = 0.0;
    //                m = m - 1;
    //                it = 0;
    //            }
    //            else if (l == m - 2)
    //            {
    //                b = -(elements[ii] + elements[ll]);
    //                c = elements[ii] * elements[ll] - elements[jj] * elements[kk];
    //                w = b * b - 4.0 * c;
    //                y = Math.Sqrt(Math.Abs(w));
    //                if (w > 0.0)
    //                {
    //                    xy = 1.0;
    //                    if (b < 0.0)
    //                        xy = -1.0;
    //                    dblU[m - 1] = (-b - xy * y) / 2.0;
    //                    dblU[m - 2] = c / dblU[m - 1];
    //                    dblV[m - 1] = 0.0; dblV[m - 2] = 0.0;
    //                }
    //                else
    //                {
    //                    dblU[m - 1] = -b / 2.0;
    //                    dblU[m - 2] = dblU[m - 1];
    //                    dblV[m - 1] = y / 2.0;
    //                    dblV[m - 2] = -dblV[m - 1];
    //                }

    //                m = m - 2;
    //                it = 0;
    //            }
    //            else
    //            {
    //                if (it >= nMaxIt)
    //                    return false;

    //                it = it + 1;
    //                for (j = l + 2; j <= m - 1; j++)
    //                    elements[j * n + j - 2] = 0.0;
    //                for (j = l + 3; j <= m - 1; j++)
    //                    elements[j * n + j - 3] = 0.0;
    //                for (k = l; k <= m - 2; k++)
    //                {
    //                    if (k != l)
    //                    {
    //                        p = elements[k * n + k - 1];
    //                        q = elements[(k + 1) * n + k - 1];
    //                        r = 0.0;
    //                        if (k != m - 2)
    //                            r = elements[(k + 2) * n + k - 1];
    //                    }
    //                    else
    //                    {
    //                        x = elements[ii] + elements[ll];
    //                        y = elements[ll] * elements[ii] - elements[kk] * elements[jj];
    //                        ii = l * n + l;
    //                        jj = l * n + l + 1;
    //                        kk = (l + 1) * n + l;
    //                        ll = (l + 1) * n + l + 1;
    //                        p = elements[ii] * (elements[ii] - x) + elements[jj] * elements[kk] + y;
    //                        q = elements[kk] * (elements[ii] + elements[ll] - x);
    //                        r = elements[kk] * elements[(l + 2) * n + l + 1];
    //                    }

    //                    if ((Math.Abs(p) + Math.Abs(q) + Math.Abs(r)) != 0.0)
    //                    {
    //                        xy = 1.0;
    //                        if (p < 0.0)
    //                            xy = -1.0;
    //                        s = xy * Math.Sqrt(p * p + q * q + r * r);
    //                        if (k != l)
    //                            elements[k * n + k - 1] = -s;
    //                        e = -q / s;
    //                        f = -r / s;
    //                        x = -p / s;
    //                        y = -x - f * r / (p + s);
    //                        g = e * r / (p + s);
    //                        z = -x - e * q / (p + s);
    //                        for (j = k; j <= m - 1; j++)
    //                        {
    //                            ii = k * n + j;
    //                            jj = (k + 1) * n + j;
    //                            p = x * elements[ii] + e * elements[jj];
    //                            q = e * elements[ii] + y * elements[jj];
    //                            r = f * elements[ii] + g * elements[jj];
    //                            if (k != m - 2)
    //                            {
    //                                kk = (k + 2) * n + j;
    //                                p = p + f * elements[kk];
    //                                q = q + g * elements[kk];
    //                                r = r + z * elements[kk];
    //                                elements[kk] = r;
    //                            }

    //                            elements[jj] = q; elements[ii] = p;
    //                        }

    //                        j = k + 3;
    //                        if (j >= m - 1)
    //                            j = m - 1;

    //                        for (i = l; i <= j; i++)
    //                        {
    //                            ii = i * n + k;
    //                            jj = i * n + k + 1;
    //                            p = x * elements[ii] + e * elements[jj];
    //                            q = e * elements[ii] + y * elements[jj];
    //                            r = f * elements[ii] + g * elements[jj];
    //                            if (k != m - 2)
    //                            {
    //                                kk = i * n + k + 2;
    //                                p = p + f * elements[kk];
    //                                q = q + g * elements[kk];
    //                                r = r + z * elements[kk];
    //                                elements[kk] = r;
    //                            }

    //                            elements[jj] = q;
    //                            elements[ii] = p;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        return true;
    //    }

    //    /**
    //     * 求实对称矩阵特征值与特征向量的雅可比法
    //     * 
    //     * @param dblEigenValue - 一维数组，长度为矩阵的阶数，返回时存放特征值
    //     * @param mtxEigenVector - 返回时存放特征向量矩阵，其中第i列为与数组
    //     *                         dblEigenValue中第j个特征值对应的特征向量
    //     * @param nMaxIt - 迭代次数
    //     * @param eps - 计算精度
    //     * @return bool型，求解是否成功
    //     */
    //    public bool ComputeEvJacobi(double[] dblEigenValue, CMatrix mtxEigenVector, int nMaxIt, double eps)
    //    {
    //        int i, j, p = 0, q = 0, u, w, t, s, l;
    //        double fm, cn, sn, omega, x, y, d;

    //        if (!mtxEigenVector.Init(numCols, numCols))
    //            return false;

    //        l = 1;
    //        for (i = 0; i <= numCols - 1; i++)
    //        {
    //            mtxEigenVector.elements[i * numCols + i] = 1.0;
    //            for (j = 0; j <= numCols - 1; j++)
    //                if (i != j)
    //                    mtxEigenVector.elements[i * numCols + j] = 0.0;
    //        }

    //        while (true)
    //        {
    //            fm = 0.0;
    //            for (i = 1; i <= numCols - 1; i++)
    //            {
    //                for (j = 0; j <= i - 1; j++)
    //                {
    //                    d = Math.Abs(elements[i * numCols + j]);
    //                    if ((i != j) && (d > fm))
    //                    {
    //                        fm = d;
    //                        p = i;
    //                        q = j;
    //                    }
    //                }
    //            }

    //            if (fm < eps)
    //            {
    //                for (i = 0; i < numCols; ++i)
    //                    dblEigenValue[i] = GetElement(i, i);
    //                return true;
    //            }

    //            if (l > nMaxIt)
    //                return false;

    //            l = l + 1;
    //            u = p * numCols + q;
    //            w = p * numCols + p;
    //            t = q * numCols + p;
    //            s = q * numCols + q;
    //            x = -elements[u];
    //            y = (elements[s] - elements[w]) / 2.0;
    //            omega = x / Math.Sqrt(x * x + y * y);

    //            if (y < 0.0)
    //                omega = -omega;

    //            sn = 1.0 + Math.Sqrt(1.0 - omega * omega);
    //            sn = omega / Math.Sqrt(2.0 * sn);
    //            cn = Math.Sqrt(1.0 - sn * sn);
    //            fm = elements[w];
    //            elements[w] = fm * cn * cn + elements[s] * sn * sn + elements[u] * omega;
    //            elements[s] = fm * sn * sn + elements[s] * cn * cn - elements[u] * omega;
    //            elements[u] = 0.0;
    //            elements[t] = 0.0;
    //            for (j = 0; j <= numCols - 1; j++)
    //            {
    //                if ((j != p) && (j != q))
    //                {
    //                    u = p * numCols + j; w = q * numCols + j;
    //                    fm = elements[u];
    //                    elements[u] = fm * cn + elements[w] * sn;
    //                    elements[w] = -fm * sn + elements[w] * cn;
    //                }
    //            }

    //            for (i = 0; i <= numCols - 1; i++)
    //            {
    //                if ((i != p) && (i != q))
    //                {
    //                    u = i * numCols + p;
    //                    w = i * numCols + q;
    //                    fm = elements[u];
    //                    elements[u] = fm * cn + elements[w] * sn;
    //                    elements[w] = -fm * sn + elements[w] * cn;
    //                }
    //            }

    //            for (i = 0; i <= numCols - 1; i++)
    //            {
    //                u = i * numCols + p;
    //                w = i * numCols + q;
    //                fm = mtxEigenVector.elements[u];
    //                mtxEigenVector.elements[u] = fm * cn + mtxEigenVector.elements[w] * sn;
    //                mtxEigenVector.elements[w] = -fm * sn + mtxEigenVector.elements[w] * cn;
    //            }
    //        }
    //    }

    //    /**
    //     * 求实对称矩阵特征值与特征向量的雅可比过关法
    //     * 
    //     * @param dblEigenValue - 一维数组，长度为矩阵的阶数，返回时存放特征值
    //     * @param mtxEigenVector - 返回时存放特征向量矩阵，其中第i列为与数组
    //     *                         dblEigenValue中第j个特征值对应的特征向量
    //     * @param eps - 计算精度
    //     * @return bool型，求解是否成功
    //     */
    //    public bool ComputeEvJacobi(double[] dblEigenValue, CMatrix mtxEigenVector, double eps)
    //    {
    //        int i, j, p, q, u, w, t, s;
    //        double ff, fm, cn, sn, omega, x, y, d;

    //        if (!mtxEigenVector.Init(numCols, numCols))
    //            return false;

    //        for (i = 0; i <= numCols - 1; i++)
    //        {
    //            mtxEigenVector.elements[i * numCols + i] = 1.0;
    //            for (j = 0; j <= numCols - 1; j++)
    //                if (i != j)
    //                    mtxEigenVector.elements[i * numCols + j] = 0.0;
    //        }

    //        ff = 0.0;
    //        for (i = 1; i <= numCols - 1; i++)
    //        {
    //            for (j = 0; j <= i - 1; j++)
    //            {
    //                d = elements[i * numCols + j];
    //                ff = ff + d * d;
    //            }
    //        }

    //        ff = Math.Sqrt(2.0 * ff);
    //        ff = ff / (1.0 * numCols);

    //        bool nextLoop = false;
    //        while (true)
    //        {
    //            for (i = 1; i <= numCols - 1; i++)
    //            {
    //                for (j = 0; j <= i - 1; j++)
    //                {
    //                    d = Math.Abs(elements[i * numCols + j]);
    //                    if (d > ff)
    //                    {
    //                        p = i;
    //                        q = j;

    //                        u = p * numCols + q;
    //                        w = p * numCols + p;
    //                        t = q * numCols + p;
    //                        s = q * numCols + q;
    //                        x = -elements[u];
    //                        y = (elements[s] - elements[w]) / 2.0;
    //                        omega = x / Math.Sqrt(x * x + y * y);
    //                        if (y < 0.0)
    //                            omega = -omega;

    //                        sn = 1.0 + Math.Sqrt(1.0 - omega * omega);
    //                        sn = omega / Math.Sqrt(2.0 * sn);
    //                        cn = Math.Sqrt(1.0 - sn * sn);
    //                        fm = elements[w];
    //                        elements[w] = fm * cn * cn + elements[s] * sn * sn + elements[u] * omega;
    //                        elements[s] = fm * sn * sn + elements[s] * cn * cn - elements[u] * omega;
    //                        elements[u] = 0.0; elements[t] = 0.0;

    //                        for (j = 0; j <= numCols - 1; j++)
    //                        {
    //                            if ((j != p) && (j != q))
    //                            {
    //                                u = p * numCols + j;
    //                                w = q * numCols + j;
    //                                fm = elements[u];
    //                                elements[u] = fm * cn + elements[w] * sn;
    //                                elements[w] = -fm * sn + elements[w] * cn;
    //                            }
    //                        }

    //                        for (i = 0; i <= numCols - 1; i++)
    //                        {
    //                            if ((i != p) && (i != q))
    //                            {
    //                                u = i * numCols + p;
    //                                w = i * numCols + q;
    //                                fm = elements[u];
    //                                elements[u] = fm * cn + elements[w] * sn;
    //                                elements[w] = -fm * sn + elements[w] * cn;
    //                            }
    //                        }

    //                        for (i = 0; i <= numCols - 1; i++)
    //                        {
    //                            u = i * numCols + p;
    //                            w = i * numCols + q;
    //                            fm = mtxEigenVector.elements[u];
    //                            mtxEigenVector.elements[u] = fm * cn + mtxEigenVector.elements[w] * sn;
    //                            mtxEigenVector.elements[w] = -fm * sn + mtxEigenVector.elements[w] * cn;
    //                        }

    //                        nextLoop = true;
    //                        break;
    //                    }
    //                }

    //                if (nextLoop)
    //                    break;
    //            }

    //            if (nextLoop)
    //            {
    //                nextLoop = false;
    //                continue;
    //            }

    //            nextLoop = false;

    //            // 如果达到精度要求，退出循环，返回结果
    //            if (ff < eps)
    //            {
    //                for (i = 0; i < numCols; ++i)
    //                    dblEigenValue[i] = GetElement(i, i);
    //                return true;
    //            }

    //            ff = ff / (1.0 * numCols);
    //        }
    //    }
    //}

}
