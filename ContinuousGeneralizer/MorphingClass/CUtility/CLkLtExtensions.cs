using System;
using System.Collections.Generic;
using System.Collections;
//using System.Collections.ObjectModel ;
using System.Linq;
using System.Text;

using MorphingClass.CGeometry ;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CUtility
{
    /// <summary>some extensions of LinkedList and List</summary>
    public static class CLkLtExtensions
    {
        
        public static void AppendRange<T>(this LinkedList<T> source, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                source.AddLast(item);
            }
        }


        public static void PrependRange<T>(this LinkedList<T> source, IEnumerable<T> items)
        {
            if (source.Count == 0)
            {
                foreach (T item in items)
                {
                    source.AddLast(item);
                }
            }
            else
            {
                LinkedListNode<T> first = source.First;
                foreach (T item in items)
                {
                    source.AddBefore(first, item);
                }
            }
        }


        public static int GetCountItem(this IEnumerable items)
        {
            if (null == items)
                return 0;

            var total = 0;

            foreach (var item in items)
            {
                if (item is IEnumerable)
                    total += (item as IEnumerable).GetCountItem();
                else
                    total++;
            }

            return total;
        }

        public static int GetCountCpt<T>(this IEnumerable<T> items) where T : CLineBase
        {
            if (null == items)
                return 0;

            var total = 0;

            foreach (var item in items)
            {
                if (item is IEnumerable<T>)
                    total += (item as IEnumerable<T>).GetCountCpt();
                else
                {
                    CLineBase pLineBase = item as CLineBase;
                    total += pLineBase.cptlt.Count;
                }
            }

            return total;
        }

        //public static T Last<T>(this List<T> items)
        //{
        //    return items[items.Count - 1];
        //}

        public static int LastT(this List<int> items)
        {
            return items[items.Count - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="TEnumerable_2"></param>
        /// <param name="orderFunc"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        /// <remarks>Each list being merged should be already sorted. This method will locate the equal elements with respect to the order of their lists. 
        /// For example, if elements Ti == Tj, and they are respectively from list i and list j (i < j), then Ti will be in front of Tj in the merged result. 
        /// The complexity is O(mn), where n is the number of lists being merged and m is the sum of the lengths of the lists. </remarks>
        public static IEnumerable<T> Merge<T, TOrder>(this IEnumerable<IEnumerable<T>> TEnumerable_2, Func<T, TOrder> orderFunc, IComparer<TOrder> cmp = null)
        {
            if (cmp == null)
            {
                cmp = Comparer<TOrder>.Default;
            }

            List<IEnumerator<T>> TEnumeratorLt = TEnumerable_2
               .Select(l => l.GetEnumerator())
               .Where(e => e.MoveNext())
               .ToList();

            while (TEnumeratorLt.Count > 0)
            {
                int intMinIndex;
                IEnumerator<T> TSmallest = TEnumeratorLt.GetMin(TElement => orderFunc(TElement.Current), out intMinIndex, cmp);
                yield return TSmallest.Current;

                if (TSmallest.MoveNext() == false)
                {
                    TEnumeratorLt.RemoveAt(intMinIndex);
                }
            }
        }

        /// <summary>
        /// Get the first min item in an IEnumerable, and return the index of it by minIndex
        /// </summary>
        /// <remarks>If there are more than one minimum elements, then this method will return the first one</remarks>
        public static T GetMin<T, TOrder>(this IEnumerable<T> self, Func<T, TOrder> orderFunc, out int minIndex, IComparer<TOrder> cmp = null)
        {
            if (self == null) throw new ArgumentNullException("self");

            IEnumerator<T> selfEnumerator = self.GetEnumerator();
            if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");

            if (cmp == null) cmp = Comparer<TOrder>.Default;

            T min = selfEnumerator.Current;
            minIndex = 0;
            int intCount = 1;
            while (selfEnumerator.MoveNext())
            {
                if (cmp.Compare(orderFunc(selfEnumerator.Current), orderFunc(min)) < 0)
                {
                    min = selfEnumerator.Current;
                    minIndex = intCount;
                }
                intCount++;
            }

            return min;
        }
    }

}
