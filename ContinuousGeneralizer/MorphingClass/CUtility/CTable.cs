using System;
using System.Collections.Generic;
using System.Text;
 using System.ComponentModel;

using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CUtility
{
    public class CTable
    {
        public CCell[,] aCell { get; set; }
        public int intRow { get; set; }
        public int intCol { get; set; }

        public CTable()
        {
        }

        public CTable(int intRow, int intCol)
        {
            this.intRow = intRow;
            this.intCol = intCol;
            this.aCell = new CCell[intRow, intCol];
        }

        public double dblEvaluation
        {
            get { return this.aCell[this.intRow - 1, this.intCol - 1].dblEvaluation; }
        }
    }

    public class CCell
    {
        public int intBackK1 { get; set; }
        public int intBackK2 { get; set; }
        public double dblEvaluation { get; set; }

        public CCell()
        {
        }

        public CCell(int intK1, int intK2, double dblCost)
        {
            this.intBackK1 = intK1;
            this.intBackK2 = intK2;
            this.dblEvaluation = dblCost;
        }
    }
}
