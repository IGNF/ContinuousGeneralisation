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

        public CTable(int fintRow, int fintCol, bool blnSetRowColIndex = false)
        {
            var newaCell = new CCell[fintRow, fintCol];
            if (blnSetRowColIndex == true)
            {
                for (int i = 0; i < fintRow; i++)
                {
                    for (int j = 0; j < fintCol; j++)
                    {
                        newaCell[i, j] = new CCell();
                        newaCell[i, j].intRowIndex = i;
                        newaCell[i, j].intColIndex = j;
                    }
                }
            }

            this.intRow = fintRow;
            this.intCol = fintCol;
            this.aCell = new CCell[fintRow, fintCol];
        }

        public double dblCost
        {
            get { return this.aCell[this.intRow - 1, this.intCol - 1].dblCost; }
        }

        public void PrintaCell()
        {
            Console.WriteLine("\n The cost of a table for dynamic programming:");
            for (int i = 0; i < this.intRow; i++)
            {
                for (int j = 0; j < this.intCol; j++)
                {
                    if (this.aCell[i, j] != null)
                    {
                        Console.Write(string.Format("{0,9}", this.aCell[i, j].dblCost) + "," + 
                            string.Format("{0,2}", this.aCell[i, j].intBackK1) + "    ");
                    }
                    else
                    {
                        Console.Write(string.Format("{0,9}", -1) + "," +
                            string.Format("{0,2}", -1) + "    ");
                    }
                }
                Console.WriteLine();
            }
        }

    }

    public class CCell
    {
        public int intBackK1 { get; set; }
        public int intBackK2 { get; set; }
        public double dblCost { get; set; }
        public double dblCostHelp { get; set; }
        public int intRowIndex { get; set; }
        public int intColIndex { get; set; }

        public CCell()
        {
        }

        public CCell(int intK1, int intK2, double fdblCost,  double fdblCostHelp = 0, 
            int fintRowIndex=-1, int fintColIndex = -1)
        {
            this.intBackK1 = intK1;
            this.intBackK2 = intK2;
            this.dblCost = fdblCost;
            this.dblCostHelp = fdblCostHelp;
            this.intRowIndex = fintRowIndex;
            this.intColIndex = fintColIndex;
        }
    }
}
