﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CGeneralizationMethods
{
    public class CImaiIriSimplify
    {







        public static IEnumerable<CPoint> ImaiIriSimplify(List<CPoint> cptlt, double dblThresholdDis)
        {
            Console.WriteLine("dblThresholdDis: " + dblThresholdDis);
            if (cptlt.Count <= 1)
            {
                throw new ArgumentOutOfRangeException("There is no points for simplification!");
            }
            else if (cptlt.Count == 2)
            {
                yield return cptlt[0];
                yield return cptlt[1];
            }
            else
            {
                var CNodeLt = new List<CNode>(cptlt.Count);
                CNodeLt.EveryElementNew();
                CNodeLt.SetIndexID();
                for (int i = 0; i < cptlt.Count - 1; i++)
                {
                    CNodeLt[i].NbrCNodeLt = new List<CNode>();
                    //CNodeLt[i + 1].dblCost = 0; //the attribute dblCost is used temporarily
                    CNodeLt[i].NbrCNodeLt.Add(CNodeLt[i + 1]);
                    for (int j = i + 2; j < cptlt.Count; j++)
                    {
                        var subcptlt = cptlt.GetRange(i, j - i + 1);
                        var tplWithinTDis = CDPSimplify.IsWithinTDis(subcptlt, dblThresholdDis);
                        if (tplWithinTDis.Item1 == true)
                        {
                            //Console.WriteLine("i, j, dblThresholdDis:" + i.ToString() + "   "+ j.ToString() + "   " + tplWithinTDis.Item3);

                            //CNodeLt[j].dblCost = tplWithinTDis.Item3;
                            CNodeLt[i].NbrCNodeLt.Add(CNodeLt[j]);
                        }
                    }
                    //CNodeLt[i].NbrCNodeLt.Sort((cnode1, cnode2) => cnode2.dblCost.CompareTo(cnode1.dblCost));
                }

                BFS(CNodeLt[0], CNodeLt.Last());
                var currentCNode = CNodeLt[0];
                while (currentCNode != null)
                {
                    yield return cptlt[currentCNode.indexID];
                    currentCNode = currentCNode.NextCNode;
                }
            }
        }


        public static void BFS(CNode startCNode, CNode goalCNode)
        {
            var CNodeQueue = new Queue<CNode>();
            CNodeQueue.Enqueue(startCNode);
            bool isFoundGoal = false;

            while (CNodeQueue.Count > 0 && isFoundGoal == false)
            {
                var currentCNode = CNodeQueue.Dequeue();
                foreach (var nbrcnode in currentCNode.NbrCNodeLt)
                {
                    if (nbrcnode.strColor == "white")
                    {
                        nbrcnode.strColor = "gray";
                        nbrcnode.PrevCNode = currentCNode;

                        CNodeQueue.Enqueue(nbrcnode);
                    }

                    if (nbrcnode.GID == goalCNode.GID)
                    {
                        isFoundGoal = true;
                    }
                }
                currentCNode.strColor = "black";
            }

            // set NextCNode for each Node on the path
            var backCNode = goalCNode;
            while (backCNode.GID != startCNode.GID)
            {
                backCNode.PrevCNode.NextCNode = backCNode;

                backCNode = backCNode.PrevCNode;
            }

        }
    }

    


}
