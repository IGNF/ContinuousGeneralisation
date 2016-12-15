using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

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
        private List<CEdge>[,] _aCEdgeLtGrid;


        public CEdgeGrid(List<CEdge> CEdgeLt)
        {
            CEnvelope pEnvelope = CGeometricMethods.GetEnvelope(CEdgeLt);
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
        //    CGeometricMethods.SetSlope(ref CEdgeLt);
        //}

        private void FillCEdgeLtInGrid()
        {
            _aCEdgeLtGrid = new List<CEdge>[_intRowCount, _intColCount];
            for (int i = 0; i < _intRowCount; i++)
            {
                for (int j = 0; j < _intColCount; j++)
                {
                    _aCEdgeLtGrid[i, j] = new List<CEdge>();
                }
            }

            for (int i = 0; i < _CEdgeLt.Count; i++)
            {
                _CEdgeLt[i].CEdgeCellLtLt = new List<List<CEdge>>();
                FillCEdgeInGrid(_CEdgeLt[i], _dblCellWidth, _dblCellHeight, _pEnvelope, ref _aCEdgeLtGrid);
            }
        }

        /// <summary>Fill the edges into cells of the grid</summary>
        /// <remarks>
        /// Notice that the cells start from the left down corner of the grid.
        /// If an edge intersects the top edge of a cell, then the edge will be recorded in this cell, and the cell above this cell
        /// If an edge intersects the right edge of a cell, then the edge will be recorded in this cell, and the cell to the right of this cell
        /// If an edge intersects the top-right vertex of a cell, then the edge will be recorded in this cell, the cell above this cell, and the cell to the top right of this cell</remarks>
        private void FillCEdgeInGrid(CEdge cedge, double dblCellWidth, double dblCellHeight, CEnvelope pEnvelope, ref List<CEdge>[,] aCEdgeLtGrid)
        {
            double dblXDiff = cedge.ToCpt.X - cedge.FrCpt.X;
            //double dblYDiff = cedge.ToCpt.Y - cedge.FrCpt.Y;

            if (dblXDiff == 0)
            {
                HandleNoXDiff(cedge, dblCellWidth, dblCellHeight, pEnvelope, ref  aCEdgeLtGrid);
            }
            else if (dblXDiff > 0)
            {
                HandleWithXDiff(cedge, cedge, dblCellWidth, dblCellHeight, pEnvelope, ref  aCEdgeLtGrid);
            }
            else
            {
                CEdge auxcedge = new CEdge(cedge.ToCpt, cedge.FrCpt);
                auxcedge.SetSlope();
                HandleWithXDiff(cedge, auxcedge, dblCellWidth, dblCellHeight, pEnvelope, ref  aCEdgeLtGrid);
            }
        }

        private void HandleNoXDiff(CEdge cedge, double dblCellWidth, double dblCellHeight, CEnvelope pEnvelope, ref List<CEdge>[,] aCEdgeLtGrid)
        {
            int intFrRow = ComputeRow(cedge.FrCpt.Y);
            int intToRow = ComputeRow(cedge.ToCpt.Y);
            int intCol = ComputeCol(cedge.FrCpt.X);

            RecordIntoOneColumn(cedge, intCol, intFrRow, intToRow, ref aCEdgeLtGrid);
        }

        public int ComputeRow(double dblCoordinate)
        {
            int intRow = CGeometricMethods.ComputeRowOrCol(dblCoordinate, _pEnvelope.YMin, _dblCellHeight);
            return intRow;
        }

        public int ComputeCol(double dblCoordinate)
        {
            int intCol = CGeometricMethods.ComputeRowOrCol(dblCoordinate, _pEnvelope.XMin, _dblCellWidth);
            return intCol;
        }

        private void HandleWithXDiff(CEdge cedge, CEdge auxcedge, double dblCellWidth, double dblCellHeight, CEnvelope pEnvelope, ref List<CEdge>[,] aCEdgeLtGrid)
        {
            int intFrRow = Convert.ToInt32(Math.Truncate((auxcedge.FrCpt.Y - pEnvelope.YMin) / dblCellHeight));
            int intFrCol = Convert.ToInt32(Math.Truncate((auxcedge.FrCpt.X - pEnvelope.XMin) / dblCellWidth));

            int intToRow = Convert.ToInt32(Math.Truncate((auxcedge.ToCpt.Y - pEnvelope.YMin) / dblCellHeight));
            int intToCol = Convert.ToInt32(Math.Truncate((auxcedge.ToCpt.X - pEnvelope.XMin) / dblCellWidth));

            double dblYIncrement = dblCellWidth * auxcedge.dblSlope;
            double dblYIntersectVertical = auxcedge.FrCpt.Y + auxcedge.dblSlope * (GetCellXMin(intFrCol + 1) - auxcedge.FrCpt.X);
            int intLastRow = intFrRow;

            //the columns before the last column
            //Note that if the FrCpt and ToCpt are in the same column (i.e., intFrCol == intToCol), then this "for loop" will do nothing 
            for (int i = intFrCol; i < intToCol; i++)
            {
                int intRowIntersect = Convert.ToInt32(Math.Truncate((dblYIntersectVertical - pEnvelope.YMin) / dblCellHeight));
                RecordIntoOneColumn(cedge, i, intLastRow, intRowIntersect, ref aCEdgeLtGrid);

                dblYIntersectVertical += dblYIncrement;
                intLastRow = intRowIntersect;
            }

            //the last column.
            RecordIntoOneColumn(cedge, intToCol, intLastRow, intToRow, ref aCEdgeLtGrid);
        }

        private void RecordIntoOneColumn(CEdge cedge, int intCol, int intFrRow, int intToRow, ref List<CEdge>[,] aCEdgeLtGrid)
        {
            int intRowIncrement = 0;  //intRowIncrement can be -1, 0, or 1
            if ((intToRow - intFrRow) != 0)
            {
                intRowIncrement = (intToRow - intFrRow) / Math.Abs(intToRow - intFrRow);
            }

            aCEdgeLtGrid[intFrRow, intCol].Add(cedge);
            cedge.CEdgeCellLtLt.Add(aCEdgeLtGrid[intFrRow, intCol]);
            if (intRowIncrement != 0)
            {
                int intCurrentRow = intFrRow;
                do
                {
                    intCurrentRow += intRowIncrement;
                    aCEdgeLtGrid[intCurrentRow, intCol].Add(cedge);
                    cedge.CEdgeCellLtLt.Add(aCEdgeLtGrid[intCurrentRow, intCol]);
                } while (intCurrentRow != intToRow);
            }
        }

        public double GetCellXMin(int intCol)
        {
            double dblX = pEnvelope.XMin + intCol * _dblCellWidth;
            return dblX;
        }

        public double GetCellYMin(int intRow)
        {
            double dblY = pEnvelope.YMin + intRow * _dblCellHeight;
            return dblY;
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

        public List<CEdge>[,] aCEdgeLtGrid
        {
            get { return _aCEdgeLtGrid; }
            set { _aCEdgeLtGrid = value; }
        }
    }




}
