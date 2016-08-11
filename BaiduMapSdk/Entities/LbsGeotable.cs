﻿using System;
using System.Collections.Generic;
using BaiduMapSdk.Common;

namespace BaiduMapSdk.Entities
{
    using LBSYunNetSDK;
    using LBSYunNetSDK.Options;
    using Newtonsoft.Json;
    using System.IO;
    using System.Web.Script.Serialization;
    using Utils;

    public class Photo
    {
        public long imgid;
        public string big;
        public string mid;
        public string sml;
    }

    public class LbsPoiBaseInfo
    {
        public uint geotable_id;
        public uint uid;
        public string province;
        public string district;
        public string create_time;
        public string city;
        public string title;
        public uint coord_type;
        public ICollection<double> location;
    }

    public class LbsGeotableBaseResponse<LbsPoiBaseInfo>
    {
        public uint status;
        public uint total;
        public uint size;
        public ICollection<LbsPoiBaseInfo> contents;
    }
    public class LbsGeotable
    {
        public string Name { get; set; }
        public ICollection<LbsGeotableColumn> Columns { get; set; }
        private LBSYunNet _lbsYunNet;
        public string TableId { get; set; }
        public string Ak { get; private set; }

        public string CreateGeotable(string ak)
        {
            Ak = ak;
            _lbsYunNet = new LBSYunNet(ak);
            var res = _lbsYunNet.GeotableCreate(Name, (int) BaiduGeoDatatypes.POINT, (int) BaiduIsPublisheds.Yes, 0);
            if (res.status == (int) StatusCode.Success)
            {
                TableId = res.id;
            }
            else if (res.status == (int) StatusCode.TableRepeat)
            {
                TableId = GetTableIdByName(Name);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Table id is: {0}, Status: {1}, Message: {2}", res.id, res.status, res.message);
            }

            if (TableId == null) return null;
            
            #region add columns

            foreach (var col in Columns)
            {
                var columnRes = _lbsYunNet.ColumnCreate(col.Name, col.Key, col.Type, col.MaxLength, col.DefaultValue, col.IsSortfilterField,
                    col.IsSearchField, col.IsIndexField, col.IsUniqueField, TableId);
                if (columnRes.status == (int) StatusCode.ColumnRepeat)
                {
                    var colId = GetColumnIdByName(TableId, col.Name);
                    columnRes = _lbsYunNet.ColumnUpdate(UInt32.Parse(colId), col.Key, col.Type, col.MaxLength,
                        col.IsSortfilterField,
                        col.IsSearchField, col.IsIndexField, col.IsUniqueField, UInt32.Parse(TableId), Name);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Column id is: {0}, Status: {1}, Message: {2}", columnRes.id, columnRes.status, columnRes.message);
                }
            }

            #endregion
            

            System.Diagnostics.Debug.WriteLine("Table id is: " + TableId);
            return TableId;
        }

        public string AddOneRecord(double lon, double lat, Dictionary<string, string> values)
        {
            var res = _lbsYunNet.PoiCreate(lon, lat, (int) BaiduGeoDatatypes.POINT, TableId, values);
            System.Diagnostics.Debug.WriteLine("Column id is: {0}, Status: {1}, Message: {2}", res.id, res.status, res.message);
            return res.id;
        }

        public TPoiResponse GetPoiInfo<TPoiResponse>(uint poiId) where TPoiResponse : class
        {
            var uri = new Uri("http://api.map.baidu.com/geosearch/v3/detail/" + poiId.ToString());
            var values = new Dictionary<string, string>();
            values.Add("ak", Ak);
            values.Add("geotable_id", TableId);
            var res = HttpUtils.GetResponse("GET", uri, values);
            var s = res.GetResponseStream();
            var sr = new StreamReader(s);
            string jsonStr = sr.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<TPoiResponse>(jsonStr);
        }

        private string DeleteAllRecord()
        {
            _lbsYunNet.PoiList(TableId);
            return null;
        }

        private string GetTableIdByName(string name)
        {
            var listRes = _lbsYunNet.GeotableList(Name);
            var table = listRes.geotables.Find(t => t.Name == name);
            return table.id;
        }

        private string GetColumnIdByName(string tableId, string name)
        {
            var res = _lbsYunNet.ColumnList(tableId);
            var col = res.columns.Find(c => c.name == name);
            return col.id;
        }
    }
}