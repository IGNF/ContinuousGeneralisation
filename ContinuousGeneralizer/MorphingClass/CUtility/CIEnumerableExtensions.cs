﻿using System;
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
    public static class CIEnumerableExtensions
    {
        
        public static void AppendRange<T>(this LinkedList<T> source, IEnumerable<T> items)
        {
            if (null == items)
                return;

            foreach (T item in items)
            {
                source.AddLast(item);
            }
        }


        //public static void PrependRange<T>(this LinkedList<T> source, IEnumerable<T> items)
        //{
        //    if (null == items)
        //        return;

        //    if (source.Count == 0)
        //    {
        //        foreach (T item in items)
        //        {
        //            source.AddLast(item);
        //        }
        //    }
        //    else
        //    {
        //        LinkedListNode<T> first = source.First;
        //        foreach (T item in items)
        //        {
        //            source.AddBefore(first, item);
        //        }
        //    }
        //}

        //public static void GetCEdgeLt<>(this IEnumerable<CPolyBase<CGeo>>)
        //{


        //    }

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


        public static IEnumerable<CPoint> GetAllCptEb<T, CGeo>(this IEnumerable<T> items)
            where T : CPolyBase<CGeo>
            where CGeo : class
        {
            foreach (var cpb in items)
            {
                foreach (var cpt in cpb.CptLt)
                {
                    yield return cpt;
                }
            }
        }



        public static int GetCountCpt<T, CGeo>(this IEnumerable<T> items) 
            where T : CPolyBase<CGeo>
            where CGeo : class
        {
            if (null == items)
                return 0;

            var total = 0;

            foreach (var item in items)
            {
                if (item is IEnumerable<T>)
                    total += (item as IEnumerable<T>).GetCountCpt<T, CGeo>();
                else
                {
                    CPolyBase<T> pLineBase = item as CPolyBase<T>;
                    total += pLineBase.CptLt.Count;
                }
            }

            return total;
        }

        public static T GetLastT<T>(this List<T> items)
        {
            return items[items.Count - 1];
        }


        public static void SetFirstT<T>(this List<T> items, T item)
        {
            items[0] = item;
        }

        public static T GetFirstT<T>(this List<T> items)
        {
            return items[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TEnumerable"></param>
        /// <returns></returns>
        /// <remarks>for a dictionary, this operation takes O(log n) time;
        /// see https://msdn.microsoft.com/en-us/library/1z8z05h2.aspx?f=255&MSPPError=-2147217396 </remarks>
        public static T GetFirstT<T>(this IEnumerable<T> TEnumerable)
        {
            IEnumerator<T> selfEnumerator = TEnumerable.GetEnumerator();
            if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");
            return selfEnumerator.Current;
        }


        public static void SetLast_T<T>(this List<T> items, T item)  //we add T at last to make it has the same number of characters as GetFirstElement
        {
            items[items.Count - 1] = item;
        }

        public static T GetLast_T<T>(this List<T> items)  //we add T at last to make it has the same number of characters as GetFirstElement
        {
            return items[items.Count - 1];
        }

        //public static void RemoveFirstT<T>(this List<T> items)
        //{
        //    items.RemoveAt(0);
        //}

        public static T RemoveLast_T<T>(this List<T> items)
        {
            T lastT = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return lastT;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TOrder"></typeparam>
        ///// <param name="TEnumerable"></param>
        ///// <param name="orderFunc"></param>
        ///// <param name="cmp"></param>
        ///// <returns></returns>
        ///// <remarks>TEnumerable is already ordered according to cmp</remarks>
        //public static IEnumerable<T> RemoveOrderedLaterDuplicate<T, TOrder>(this IEnumerable<T> TEnumerable, Func<T, TOrder> orderFunc, IComparer<TOrder> cmp = null)
        //{
        //    IEnumerator<T> selfEnumerator = TEnumerable.GetEnumerator();
        //    if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");

        //    if (cmp == null) { cmp = Comparer<TOrder>.Default; }

        //    T lastT = selfEnumerator.Current;
        //    yield return lastT;
        //    while (selfEnumerator.MoveNext())
        //    {
        //        if (cmp.Compare(orderFunc(selfEnumerator.Current), orderFunc(lastT)) != 0)
        //        {
        //            yield return selfEnumerator.Current;
        //            lastT = selfEnumerator.Current;
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TEnumerable"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveLaterDuplicate<T>(this IEnumerable<T> TEnumerable, IComparer<T> cmp = null)
        {
            IEnumerator<T> selfEnumerator = TEnumerable.GetEnumerator();
            if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");

            if (cmp == null) { cmp = Comparer<T>.Default; }


            SortedSet<T> ExistingSS = new SortedSet<T>(cmp);
            do
            {
                if (ExistingSS.Add(selfEnumerator.Current))
                {
                    yield return selfEnumerator.Current;
                }
            } while (selfEnumerator.MoveNext());


            //ss.

            //return new SortedSet<T>(TEnumerable, cmp);

        }

        public static void SetIndexID<T>(this IEnumerable<T> TEnumerable)
            where T : CGeometricBase<T>
        {
            IEnumerator<T> selfEnumerator = TEnumerable.GetEnumerator();
            int intIndexID = 0;
            while (selfEnumerator.MoveNext())
            {
                selfEnumerator.Current.indexID = intIndexID++;
            }
        }

        //public static void SetIndexID<CPoint>(this IEnumerable<CPoint> TEnumerable)
        //{
        //    IEnumerator<CPoint> selfEnumerator = TEnumerable.GetEnumerator();
        //    int intIndexID = 0;
        //    while (selfEnumerator.MoveNext())
        //    {
        //        selfEnumerator.Current.indexID = intIndexID++;
        //    }
        //}

        public static void EveryElementNew<T>(this List<T> ElementLt)
            where T : new()
        {
            for (int i = 0; i < ElementLt.Capacity; i++)
            {
                T Element = new T();
                ElementLt.Add(Element);
            }
        }

        public static void EveryElementValue<T>(this List<T> ElementLt, T value)
            where T : class
        {
            for (int i = 0; i < ElementLt.Capacity; i++)
            {
                ElementLt.Add(value);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TOrder"></typeparam>
        ///// <param name="TEnumerable_2"></param>
        ///// <param name="orderFunc"></param>
        ///// <param name="cmp"></param>
        ///// <returns></returns>
        ///// <remarks>Each list being merged should be already sorted. This method will locate the equal elements with respect to the order of their lists. 
        ///// For example, if elements Ti == Tj, and they are respectively from list i and list j (i < j), then Ti will be in front of Tj in the merged result. 
        ///// The complexity is O(mn), where n is the number of lists being merged and m is the sum of the lengths of the lists. 
        ///// The running time can be improved to O(mlog(n)) if we use a SortedSet</remarks>
        //public static IEnumerable<T> Merge<T, TOrder>(this IEnumerable<IEnumerable<T>> TEnumerable_2, Func<T, TOrder> orderFunc, IComparer<TOrder> cmp = null)
        //{
        //    if (cmp == null) { cmp = Comparer<TOrder>.Default; }
        //    List<IEnumerator<T>> TEnumeratorLt = TEnumerable_2
        //       .Select(l => l.GetEnumerator())
        //       .Where(e => e.MoveNext())
        //       .ToList();

        //    while (TEnumeratorLt.Count > 0)
        //    {
        //        int intMinIndex;
        //        IEnumerator<T> TSmallest = TEnumeratorLt.GetMin(TElement => orderFunc(TElement.Current), out intMinIndex, cmp);
        //        yield return TSmallest.Current;

        //        if (TSmallest.MoveNext() == false)
        //        {
        //            TEnumeratorLt.RemoveAt(intMinIndex);
        //        }
        //    }
        //}




        public static IEnumerable<TExpected> ToExpectedClass<TExpected, TCurrent>(this IEnumerable<TCurrent> TEnumerable) where TExpected : class 
        {
            var TEnumerator = TEnumerable.GetEnumerator();
            while (TEnumerator.MoveNext ())
            {
                yield return TEnumerator.Current as TExpected;
            }
        }

        ///// <summary>
        ///// we probably could make this more general
        ///// </summary>
        ///// <typeparam name="CGeo"></typeparam>
        ///// <typeparam name="TSpecified"></typeparam>
        ///// <typeparam name="TValue"></typeparam>
        ///// <param name="TEnumerable"></param>
        ///// <param name="TValueEnumerable"></param>
        ///// <param name="orderFunc"></param>
        ///// <param name="orderFuncConvert"></param>
        //public static void SetSpecifiedAttribute<CGeo, TSpecified, TValue>(this IEnumerable<CGeometricBase<CGeo>> TEnumerable, IEnumerable<TValue> TValueEnumerable, Func<TValue, TSpecified> orderFuncConvert)        
        ////public static void SetSpecifiedAttribute<T, TSpecified, TValue>(this IEnumerable<T> TEnumerable, IEnumerable<TValue> TValueEnumerable, Func<T, TSpecified> orderFunc, Func<TValue, TSpecified> orderFuncConvert)
        //{
        //    var TEnumerator = TEnumerable.GetEnumerator();
        //    var TValueEnumerator = TValueEnumerable.GetEnumerator();

        //    while (TEnumerator.MoveNext())
        //    {
        //        TValueEnumerator.MoveNext();

        //        TEnumerator.Current.intType = orderFuncConvert(TValueEnumerator.Current);
        //        //T CurrentT = TEnumerator.Current;
        //        //orderFunc(CurrentT) = orderFuncConvert(TValueEnumerator.Current);


        //    }
        //}


        public static List<List<T>> ToLtLt<T>(this IEnumerable<IEnumerable<T>> self)
        {
           return self.SubToLt().ToList();
        }

        private static IEnumerable<List<T>> SubToLt<T>(this IEnumerable<IEnumerable<T>> self)
        {
            foreach (var item in self)
            {
                yield return item.ToList();
            }
        }

        //public static LinkedList<T> ToLk<T>(this IEnumerable<T> self)
        //{
        //    if (self == null) throw new ArgumentNullException("self");

        //    IEnumerator<T> selfEnumerator = self.GetEnumerator();
        //    if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");

        //    LinkedList<T> resultLk = new LinkedList<T>();

        //    do
        //    {
        //        resultLk.AddLast(selfEnumerator.Current);
        //    } while (selfEnumerator.MoveNext());


        //    return resultLk;
        //}


        public static SortedDictionary<T_key, T_value> ToSD<T_key, T_value>(this IEnumerable<T_value> self, Func<T_value, T_key> orderFunc, IComparer<T_key> pCompare)
        {
            if (self == null) throw new ArgumentNullException("self");
            SortedDictionary<T_key, T_value> resultsd = new SortedDictionary<T_key, T_value>(pCompare);

            IEnumerator<T_value> selfEnumerator = self.GetEnumerator();

            while (selfEnumerator.MoveNext())
            {
                if (resultsd.ContainsKey(orderFunc(selfEnumerator.Current)) == false)
                {
                    resultsd.Add(orderFunc(selfEnumerator.Current), selfEnumerator.Current);
                }
            }

            return resultsd;
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

            if (cmp == null) { cmp = Comparer<TOrder>.Default; }

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