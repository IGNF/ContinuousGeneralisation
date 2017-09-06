using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CGeometry
{
    public class CEdgeGrid
    {
        private List<CEdge> _CEdgeLt;
        private CEnvelope _pEnvelope;
        private double _dblCellWidth;
        private double _dblCellHeight;
        private int _intRowCount;
        private int _intColCount;
        private List<CEdge>[,] _aCEdgeLtCell;


        public CEdgeGrid(List<CEdge> CEdgeLt)
        {
            CEnvelope pEnvelope = CGeoFunc.GetEnvelope(CEdgeLt);
            CEdgeLt.ForEach(cedge => cedge.SetSlope());
            double dblCellSize = Math.Sqrt(pEnvelope.Width * pEnvelope.Height / Convert.ToDouble(CEdgeLt.Count));  //dblRowCount*dblColCount==n

            _intRowCount = Convert.ToInt32(Math.Truncate(pEnvelope.Height / dblCellSize)) + 1;  //+1, so that the bordered point can be covered
            _intColCount = Convert.ToInt32(Math.Truncate(pEnvelope.Width / dblCellSize)) + 1;   //+1, so that the bordered point can be covered
            _pEnvelope = pEnvelope;
            _CEdgeLt = CEdgeLt;
            _dblCellWidth = dblCellSize;
            _dblCellHeight = dblCellSize;

            FillCEdgeLtInGrid();
        }

        //public void SetSlope(ref List <CEdge > CEdgeLt)
        //{
        //    CGeoFunc.SetSlope(ref CEdgeLt);
        //}

        private void FillCEdgeLtInGrid()
        {
            _aCEdgeLtCell = new List<CEdge>[_intRowCount, _intColCount];
            for (int i = 0; i < _intRowCount; i++)
            {
                for (int j = 0; j < _intColCount; j++)
                {
                    _aCEdgeLtCell[i, j] = new List<CEdge>();
                }
            }

            for (int i = 0; i < _CEdgeLt.Count; i++)
            {
                //_CEdgeLt[i].CEdgeCellLtLt = new List<List<CEdge>>();

                var intRowColVpLt = FindRowColVpLt(_CEdgeLt[i]);
                _CEdgeLt[i].RowColVpLt = intRowColVpLt;
                foreach (var RowColVp in intRowColVpLt)
                {
                    _aCEdgeLtCell[RowColVp.val1, RowColVp.val2].Add(_CEdgeLt[i]);
                }
            }
        }

        /// <summary>Fill the edges into cells of the grid</summary>
        /// <remarks>
        /// Notice that the cells start from the lower left corner of the grid.
        /// If an edge intersects the top edge of a cell, then the edge will be recorded in this cell, and the cell above this cell
        /// If an edge intersects the right edge of a cell, then the edge will be recorded in this cell, and the cell to the right of this cell
        /// If an edge intersects the top-right vertex of a cell, then the edge will be recorded in this cell, the cell above this cell, and the cell to the top right of this cell</remarks>
        public List<CValPair<int, int>> FindRowColVpLt(CEdge cedge) 
            //double dblCellWidth, double dblCellHeight, CEnvelope pEnvelope, ref List<CEdge>[,] aCEdgeLtCell)
        {
            var dblCellWidth = _dblCellWidth;
            var dblCellHeight = _dblCellHeight;
            var pEnvelope = _pEnvelope;
            var aCEdgeLtCell = _aCEdgeLtCell;

            double dblXDiff = cedge.ToCpt.X - cedge.FrCpt.X;
            //double dblYDiff = cedge.ToCpt.Y - cedge.FrCpt.Y;

            if (dblXDiff == 0)
            {
               return HandleNoXDiff(cedge, dblCellWidth, dblCellHeight, pEnvelope,  aCEdgeLtCell);
            }
            else if (dblXDiff > 0)
            {
                return HandleWithXDiff(cedge, cedge, dblCellWidth, dblCellHeight, pEnvelope,  aCEdgeLtCell);
            }
            else
            {
                CEdge cedgeIncrX = new CEdge(cedge.ToCpt, cedge.FrCpt);
                cedgeIncrX.SetSlope();
                return HandleWithXDiff(cedge, cedgeIncrX, dblCellWidth, dblCellHeight, pEnvelope,  aCEdgeLtCell);
            }
        }

        private List<CValPair<int, int>> HandleNoXDiff(CEdge cedge, double dblCellWidth, 
            double dblCellHeight, CEnvelope pEnvelope, List<CEdge>[,] aCEdgeLtCell)
        {
            int intFrRow = GetRow(cedge.FrCpt.Y);
            int intToRow = GetRow(cedge.ToCpt.Y);
            int intCol = GetCol(cedge.FrCpt.X);

           return RecordIntoOneColumn(cedge, intCol, intFrRow, intToRow, aCEdgeLtCell).ToList();
        }

        private List<CValPair<int, int>> HandleWithXDiff(CEdge cedge, CEdge cedgeIncrX, 
            double dblCellWidth, double dblCellHeight, CEnvelope pEnvelope, List<CEdge>[,] aCEdgeLtCell)
        {
            int intFrRow = Convert.ToInt32(Math.Truncate((cedgeIncrX.FrCpt.Y - pEnvelope.YMin) / dblCellHeight));
            int intFrCol = Convert.ToInt32(Math.Truncate((cedgeIncrX.FrCpt.X - pEnvelope.XMin) / dblCellWidth));

            int intToRow = Convert.ToInt32(Math.Truncate((cedgeIncrX.ToCpt.Y - pEnvelope.YMin) / dblCellHeight));
            int intToCol = Convert.ToInt32(Math.Truncate((cedgeIncrX.ToCpt.X - pEnvelope.XMin) / dblCellWidth));

            double dblYIncrement = dblCellWidth * cedgeIncrX.dblSlope;
            double dblYIntersectVertical = cedgeIncrX.FrCpt.Y + cedgeIncrX.dblSlope * (GetCellXMin(intFrCol + 1) - cedgeIncrX.FrCpt.X);
            int intLastRow = intFrRow;

            var intRowColVpLt = new List<CValPair<int, int>>();

            //the columns before the last column
            //Note that if the FrCpt and ToCpt are in the same column (i.e., intFrCol == intToCol), then this "for loop" will do nothing 
            for (int i = intFrCol; i < intToCol; i++)
            {
                int intRowIntersect = Convert.ToInt32(Math.Truncate((dblYIntersectVertical - pEnvelope.YMin) / dblCellHeight));
                intRowColVpLt.AddRange( RecordIntoOneColumn(cedge, i, intLastRow, intRowIntersect, aCEdgeLtCell));

                dblYIntersectVertical += dblYIncrement;
                intLastRow = intRowIntersect;
            }

            //the last column.
            intRowColVpLt.AddRange(RecordIntoOneColumn(cedge, intToCol, intLastRow, intToRow, aCEdgeLtCell));

            return intRowColVpLt;
        }

        private IEnumerable<CValPair<int,int>> RecordIntoOneColumn(CEdge cedge, int intCol, int intFrRow, int intToRow, List<CEdge>[,] aCEdgeLtCell)
        {
            int intRowIncrement = 0;  //intRowIncrement can be -1, 0, or 1
            if ((intToRow - intFrRow) != 0)
            {
                intRowIncrement = (intToRow - intFrRow) / Math.Abs(intToRow - intFrRow);
            }

            //aCEdgeLtCell[intFrRow, intCol].Add(cedge);
            //cedge.CEdgeCellLtLt.Add(aCEdgeLtCell[intFrRow, intCol]);
            //cedge.RowColVpLt.Add(new CValPair<int, int>(intFrRow, intCol));
            yield return new CValPair<int, int>(intFrRow, intCol);
            if (intRowIncrement != 0)
            {
                int intCurrentRow = intFrRow;
                do
                {
                    intCurrentRow += intRowIncrement;
                    //aCEdgeLtCell[intCurrentRow, intCol].Add(cedge);
                    //cedge.CEdgeCellLtLt.Add(aCEdgeLtCell[intCurrentRow, intCol]);
                    //cedge.RowColVpLt.Add(new CValPair<int, int>(intCurrentRow, intCol));
                    yield return new CValPair<int, int>(intCurrentRow, intCol);
                } while (intCurrentRow != intToRow);
            }
        }

        public double GetCellXMin(int intCol)
        {
            return pEnvelope.XMin + intCol * _dblCellWidth;
        }

        public double GetCellYMin(int intRow)
        {
            return pEnvelope.YMin + intRow * _dblCellHeight;
        }

        public int GetRow(double dblY)
        {
            return Convert.ToInt32(Math.Floor((dblY - pEnvelope.YMin) / _dblCellHeight));
        }

        public int GetCol(double dblX)
        {
            return Convert.ToInt32(Math.Floor((dblX - pEnvelope.XMin) / _dblCellWidth));
        }

        public List<CEdge> CEdgeLt
        {
            get { return _CEdgeLt; }
            set { _CEdgeLt = value; }
        }

        public CEnvelope pEnvelope
        {
            get { return _pEnvelope; }
            set { _pEnvelope = value; }
        }

        public double dblCellWidth
        {
            get { return _dblCellWidth; }
            set { _dblCellWidth = value; }
        }

        public double dblCellHeight
        {
            get { return _dblCellHeight; }
            set { _dblCellHeight = value; }
        }

        public int intRowCount
        {
            get { return _intRowCount; }
            set { _intRowCount = value; }
        }

        public int intColCount
        {
            get { return _intColCount; }
            set { _intColCount = value; }
        }

        public List<CEdge>[,] aCEdgeLtCell
        {
            get { return _aCEdgeLtCell; }
            set { _aCEdgeLtCell = value; }
        }
    }




}
