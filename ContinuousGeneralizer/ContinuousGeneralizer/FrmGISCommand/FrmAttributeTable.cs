using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace ContinuousGeneralizer.FrmGISCommand
{
    public partial class FrmAttributeTable : Form
    {
        private ILayer m_pLayer;
        private IMapControl4 m_mapControl;

        public FrmAttributeTable()
        {
            InitializeComponent();
        }

        public FrmAttributeTable(ref IMapControl4 mapControl, ILayer pLayer)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            m_pLayer = pLayer;
        }


        //创建空DataTable
        //首先传入ILayer，再查询到ITable，从ITable中的Fileds中获得每个Field，再根据Filed设置DataTable的DataColumn，由此创建一个只含图层字段的空DataTable。实现函数如下：
        /// <summary>
        /// 根据图层字段创建一个只含字段的空DataTable
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static DataTable CreateDataTableByLayer(ILayer pLayer, string tableName)
        {
            //创建一个DataTable表
            DataTable pDataTable = new DataTable(tableName);
            //取得ITable接口
            ITable pTable = pLayer as ITable;
            IField pField = null;
            DataColumn pDataColumn;
            //根据每个字段的属性建立DataColumn对象
            for (int i = 0; i < pTable.Fields.FieldCount; i++)
            {
                pField = pTable.Fields.get_Field(i);
                //新建一个DataColumn并设置其属性
                pDataColumn = new DataColumn(pField.Name);
                if (pField.Name == pTable.OIDFieldName)
                {
                    pDataColumn.Unique = true;//字段值是否唯一
                }
                //字段值是否允许为空
                pDataColumn.AllowDBNull = pField.IsNullable;
                //字段别名
                pDataColumn.Caption = pField.AliasName;
                //字段数据类型
                pDataColumn.DataType = System.Type.GetType(ParseFieldType(pField.Type));
                //字段默认值
                pDataColumn.DefaultValue = pField.DefaultValue;
                //当字段为String类型是设置字段长度
                if (pField.VarType == 8)
                {
                    pDataColumn.MaxLength = pField.Length;
                }
                //字段添加到表中
                pDataTable.Columns.Add(pDataColumn);
                pField = null;
                pDataColumn = null;
            }
            return pDataTable;
        }


        //因为GeoDatabase的数据类型与.NET的数据类型不同，故要进行转换。转换函数如下：
        /// <summary>
        /// 将GeoDatabase字段类型转换成.Net相应的数据类型
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        public static string ParseFieldType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDate:
                    return "System.DateTime";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "System.String";
                case esriFieldType.esriFieldTypeGUID:
                    return "System.String";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeOID:
                    return "System.String";
                case esriFieldType.esriFieldTypeRaster:
                    return "System.String";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Single";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeString:
                    return "System.String";
                default:
                    return "System.String";
            }
        }

        //装载DataTable数据
        //从上一步得到的DataTable还没有数据，只有字段信息。因此，我们要通过ICursor从ITable中逐一取出每一行数据，即IRow。
        //再创建DataTable中相应的DataRow，根据IRow设置DataRow信息，再将所有的DataRow添加到DataTable中，就完成了DataTable数据的装载。
        //为保证效率，一次最多只装载2000条数据到DataGridView。函数代码如下：
        /// <summary>
        /// 填充DataTable中的数据
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable CreateDataTable(ILayer pLayer, string tableName)
        {
            //创建空DataTable
            DataTable pDataTable = CreateDataTableByLayer(pLayer, tableName);
            //取得图层类型
            string shapeType = getShapeType(pLayer);
            //创建DataTable的行对象
            DataRow pDataRow = null;
            //从ILayer查询到ITable
            ITable pTable = pLayer as ITable;
            ICursor pCursor = pTable.Search(null, false);
            //取得ITable中的行信息
            IRow pRow = pCursor.NextRow();
            int n = 0;
            while (pRow != null)
            {
                //新建DataTable的行对象
                pDataRow = pDataTable.NewRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    //如果字段类型为esriFieldTypeGeometry，则根据图层类型设置字段值
                    if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDataRow[i] = shapeType;
                    }
                    //当图层类型为Anotation时，要素类中会有esriFieldTypeBlob类型的数据，
                    //其存储的是标注内容，如此情况需将对应的字段值设置为Element
                    else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDataRow[i] = "Element";
                    }
                    else
                    {
                        pDataRow[i] = pRow.get_Value(i);
                    }
                }
                //添加DataRow到DataTable
                pDataTable.Rows.Add(pDataRow);
                pDataRow = null;
                n++;
                //为保证效率，一次只装载最多条记录
                if (n == 2000)
                {
                    pRow = null;
                }
                else
                {
                    pRow = pCursor.NextRow();
                }
            }
            return pDataTable;
        }



        //上面的代码中涉及到一个获取图层类型的函数getShapeTape，此函数是通过ILayer判断图层类型的，代码如下：
        /// <summary>
        /// 获得图层的Shape类型
        /// </summary>
        /// <param name="pLayer">图层</param>
        /// <returns></returns>
        public static string getShapeType(ILayer pLayer)
        {
            IFeatureLayer pFeatLyr = (IFeatureLayer)pLayer;
            switch (pFeatLyr.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                default:
                    return "";
            }
        }

        //绑定DataTable到DataGridView
        //通过以上步骤，我们已经得到了一个含有图层属性数据的DataTable。现定义一个AttributeTableFrm类的成员变量：
        public DataTable attributeTable;
        //通过以下函数，我们很容易将其绑定到DataGridView控件中。
        /// <summary>
        /// 绑定DataTable到DataGridView
        /// </summary>
        /// <param name="player"></param>
        public void CreateAttributeTable()
        {
            ILayer player = m_pLayer;
            string tableName;
            tableName = getVaIDFeatureClassName(player.Name);
            attributeTable = CreateDataTable(player, tableName);
            this.dataGridView.DataSource = attributeTable;
            this.Text = "属性表[" + tableName + "] " + "记录数：" + attributeTable.Rows.Count.ToString();
        }


        //因为DataTable的表名不允许含有“.”，因此我们用“_”替换。函数如下：
        /// <summary>
        /// 替换数据表名中的点
        /// </summary>
        /// <param name="FCname"></param>
        /// <returns></returns>
        public static string getVaIDFeatureClassName(string FCname)
        {
            int dot = FCname.IndexOf(".");
            if (dot != -1)
            {
                return FCname.Replace(".", "_");
            }
            return FCname;
        }


        private void dataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            //MouseUp事件实在是解决托选(多选)属性表显示相应元素的绝佳选择，当用户在其它地方点击时“dataGridView.SelectedRows.Count==0”
            //不再执行其它代码
            if (dataGridView.SelectedRows.Count > 0)
            {
                int i, j;
                IMapControl4 mapControl = m_mapControl;
                IMap pMap = mapControl.Map;
                pMap.ClearSelection();

                //"FID"字段在ArcGIS中的属性序号(理论上：intpFIDIndex == 0)
                ILayer pLayer = m_pLayer;
                ITable pTable = pLayer as ITable;
                int intpFIDIndex = pTable.FindField("FID");


                DataGridViewSelectedRowCollection pDataGridViewSelectedRowCollection = dataGridView.SelectedRows;  //获取用户在属性表中选中的行
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                IFeatureCursor pFeatureCursor = pFeatureLayer.FeatureClass.Search(null, true);
                int intFeatureCount = pFeatureLayer.FeatureClass.FeatureCount(null);  //获取要素个数以限定循环次数
                IFeature pFeature = pFeatureCursor.NextFeature();
                for (i = 0; i < pDataGridViewSelectedRowCollection.Count; i++)
                {
                    DataGridViewRow pDataGridViewRow = pDataGridViewSelectedRowCollection[i];
                    int intFID = Convert.ToInt16(pDataGridViewRow.Cells["FID"].Value);

                    for (j = 0; j < intFeatureCount; j++)
                    {

                        //为什么不每找到一个要素就执行语句"pFeatureCursor = pFeatureLayer.FeatureClass.Search(null, true);"？？
                        //    因为ContinuousGeneralizer与ArcGIS的属性表往往是对应的(两个表都没有发生排序等变换属性表操作的情况下)，
                        //在搜索过程中，匹配某个元素成功后，两个表都直接搜索下一个元素(往往也是能匹配的)，这样可以节省大量搜索时间
                        if (pFeature == null)
                        {
                            pFeatureCursor = pFeatureLayer.FeatureClass.Search(null, true);
                            pFeature = pFeatureCursor.NextFeature();
                        }

                        int intpFID = Convert.ToInt16(pFeature.get_Value(intpFIDIndex));
                        if (intFID == intpFID)
                        {
                            pMap.SelectFeature(pLayer, pFeature);
                            break;
                        }
                        pFeature = pFeatureCursor.NextFeature();
                    }
                }
                mapControl.ActiveView.Refresh();   //这句比下面那句好用(在属性表选择行，又用工具栏上的选择工具选择时)
                //mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

            }
        }

        private void dataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //双击行表头，定位到该要素
            int i, j;
            IMapControl4 mapControl = m_mapControl;
            IMap pMap = mapControl.Map;
            pMap.ClearSelection();

            double dblMinValue = m_mapControl.FullExtent.Width / 100;

            //"FID"字段在ArcGIS中的属性序号(理论上：intpFIDIndex == 0)
            ILayer pLayer = m_pLayer;
            ITable pTable = pLayer as ITable;
            int intpFIDIndex = pTable.FindField("FID");

            DataGridViewSelectedRowCollection pDataGridViewSelectedRowCollection = dataGridView.SelectedRows;  //获取用户在属性表中选中的行
            IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
            IFeatureCursor pFeatureCursor = pFeatureLayer.FeatureClass.Search(null, true);
            int intFeatureCount = pFeatureLayer.FeatureClass.FeatureCount(null);  //获取要素个数以限定循环次数
            IFeature pFeature = pFeatureCursor.NextFeature();
            for (i = 0; i < pDataGridViewSelectedRowCollection.Count; i++)
            {
                DataGridViewRow pDataGridViewRow = pDataGridViewSelectedRowCollection[i];
                int intFID = Convert.ToInt32(pDataGridViewRow.Cells["FID"].Value);

                for (j = 0; j < intFeatureCount; j++)
                {
                    int intpFID = Convert.ToInt32(pFeature.get_Value(intpFIDIndex));
                    if (intFID == intpFID)
                    {
                        if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                        {
                            //由于点没有一个包络矩形，故定义一个包络矩形(注意：对于点的Extent来说Max=Min)
                            IEnvelope pEnvelope = new EnvelopeClass();
                            pEnvelope.XMax = pFeature.Extent.XMax + dblMinValue;
                            pEnvelope.XMin = pFeature.Extent.XMax - dblMinValue;
                            pEnvelope.YMax = pFeature.Extent.YMax + dblMinValue;
                            pEnvelope.YMin = pFeature.Extent.YMax - dblMinValue;
                            mapControl.Extent = pEnvelope;
                        }
                        else
                        {
                            mapControl.Extent = pFeature.Extent;
                        }
                        break;
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
            }
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

        }


    }
}