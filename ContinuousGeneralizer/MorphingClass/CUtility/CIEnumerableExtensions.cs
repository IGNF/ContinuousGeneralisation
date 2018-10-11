using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;

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

        //public static IEnumerable<CGeo> IGeosToCGeoEB<IGeo, CGeo>(this IEnumerable<IGeo> items)
        //    where CGeo: CGeoBase<CGeo>, new()
        //    //where CGeo : new()
        //{
        //    int intID = 0;
        //    foreach (var item in items)
        //    {
        //        var ss = new CGeo();
        //        ss =new CGeo ()

        //       yield return new CGeo();    // constructor has to be parameterless!
        //    }
        //}

        public static IEnumerable<CPolygon> IGeosToCGeoEB(this IEnumerable<IPolygon4> items)
        {
            int intID = 0;
            foreach (var item in items)
            {
               yield return new CPolygon(intID++, item);    // constructor has to be parameterless!
            }
        }

        public static IEnumerable<CPoint> GetAllCptEb<T, CGeo>(this IEnumerable<T> items)
            where T : CPolyBase
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



        public static int GetCountCpt<T>(this IEnumerable<T> items) 
            where T : CPolyBase
        {
            if (null == items)
                return 0;

            var total = 0;

            foreach (var item in items)
            {
                if (item is IEnumerable<T>)
                    total += (item as IEnumerable<T>).GetCountCpt<T>();
                else
                {
                    var pLineBase = item as CPolyBase;
                    total += pLineBase.CptLt.Count;
                }
            }

            return total;
        }


        public static void SetFirstT<T>(this List<T> items, T item)
        {
            items[0] = item;
        }
        


        public static void SetLastT<T>(this List<T> items, T item)  //we add T at last to make it has the same number of characters as GetFirstElement
        {
            items[items.Count - 1] = item;
        }


        //public static void RemoveFirstT<T>(this List<T> items)
        //{
        //    items.RemoveAt(0);
        //}

        public static T RemoveLastT<T>(this List<T> items)
        {
            T lastT = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return lastT;
        }

        public static IEnumerable<T> CopyEbWithoutFirstLastT<T>(this IEnumerable<T> itmes, bool blnRemainFirst, bool blnRemainLast)
        {
            if (blnRemainFirst==true && blnRemainLast == true)
            {
                throw new ArgumentException("There is no need to use this function.");
            }

            IEnumerator<T> selfEnumerator = itmes.GetEnumerator();
            if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");

            if (blnRemainFirst == true)
            {
                yield return selfEnumerator.Current;
            }

            selfEnumerator.MoveNext();
            var lastelement = selfEnumerator.Current;

            while (selfEnumerator.MoveNext())
            {
                yield return lastelement;
                lastelement = selfEnumerator.Current;
            }

            if (blnRemainLast==true )
            {
                yield return lastelement;
            }
        }


        /// <summary>
        /// Please stop use this function. Instead, use HashSet based on GID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TEnumerable"></param>
        public static void SetIndexID<T>(this IEnumerable<T> TEnumerable)
            where T : CBasicBase
        {
            IEnumerator<T> selfEnumerator = TEnumerable.GetEnumerator();
            int intIndexID = 0;
            while (selfEnumerator.MoveNext())
            {
                selfEnumerator.Current.indexID = intIndexID++;
            }
        }

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




        public static IEnumerable<TExpected> AsExpectedClassEb<TExpected, TCurrent>(this IEnumerable<TCurrent> TEnumerable) 
            where TExpected : class 
        {
            var TEnumerator = TEnumerable.GetEnumerator();
            while (TEnumerator.MoveNext ())
            {
                yield return TEnumerator.Current as TExpected;
            }
        }

        public static List<List<T>> ToLtLt<T>(this IEnumerable<IEnumerable<T>> self)
        {
            return self.Select(innerEb => innerEb.ToList()).ToList();

           //return self.SubToLt().ToList();
        }

        //private static IEnumerable<List<T>> SubToLt<T>(this IEnumerable<IEnumerable<T>> self)
        //{
        //    foreach (var item in self)
        //    {
        //        yield return item.ToList();
        //    }
        //}

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


        public static SortedDictionary<T_key, T_value> ToSD<T_key, T_value>(
            this IEnumerable<T_value> self, Func<T_value, T_key> orderFunc, IComparer<T_key> pCmp)
        {
            if (self == null) throw new ArgumentNullException("self");
            SortedDictionary<T_key, T_value> resultsd = new SortedDictionary<T_key, T_value>(pCmp);

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
        /// this method works for usual data types; high efficiency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        public static bool AreAnyDuplicates_BasicType<T>(this IEnumerable<T> list, IEqualityComparer<T> cmp = null)
        {
            if (cmp == null) { cmp = EqualityComparer<T>.Default; }
            var hashset = new HashSet<T>(cmp);
            return list.Any(e => !hashset.Add(e));
        }

        /// <summary>
        /// this method works for all data types; low efficiency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        public static bool AreAnyDuplicates<T>(this IEnumerable<T> Tself, IComparer<T> cmp = null)
        {
            if (cmp == null) { cmp = Comparer<T>.Default; }
            var TSS = new SortedSet<T>(cmp);
            foreach (var item in Tself)
            {
                if (TSS.Add(item) == false)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the first min item in an IEnumerable, and return the index of it by minIndex
        /// </summary>
        /// <remarks>If there are more than one minimum elements, then this method will return the first one</remarks>
        public static (T, int) GetMin<T, TOrder>(this IEnumerable<T> self, Func<T, TOrder> orderFunc,  IComparer<TOrder> cmp = null)
        {
            if (self == null) throw new ArgumentNullException("self");

            IEnumerator<T> selfEnumerator = self.GetEnumerator();
            if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");

            if (cmp == null) { cmp = Comparer<TOrder>.Default; }

            T min = selfEnumerator.Current;
            int intMinIndex = 0;
            int intCount = 1;
            while (selfEnumerator.MoveNext())
            {
                if (cmp.Compare(orderFunc(selfEnumerator.Current), orderFunc(min)) < 0)
                {
                    min = selfEnumerator.Current;
                    intMinIndex = intCount;
                }
                intCount++;
            }

            return (min, intMinIndex);
        }
    }

}
