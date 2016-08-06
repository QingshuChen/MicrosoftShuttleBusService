﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

using LBSYunNetSDK.Results;
using LBSYunNetSDK.Options;

namespace LBSYunNetSDK
{
    public partial class LBSYunNet
    {
        #region geo POI... Point of interest
        #region Post
        public LBSYunNetSDKResult PoiCreate(double longitude, double latitude, UInt32 coordType, string geotableId, Dictionary<string,string> columnKeyValue = null,
            string title = null, string address = null, string tags = null)
        {
            columnKeyValue.Add("longitude", longitude.ToString());
            columnKeyValue.Add("latitude", latitude.ToString());
            columnKeyValue.Add("coord_type", coordType.ToString());
            columnKeyValue.Add("geotable_id", geotableId);
            columnKeyValue.Add("ak", _ak);
            HttpWebResponse response = GetResponse2(
                method: LBSYunNetSDKMethods.POST.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.create.ToString(),
                postData: columnKeyValue
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            LBSYunNetSDKResult re = jss.Deserialize<LBSYunNetSDKResult>(json);
            return re;
        }

        public LBSYunNetSDKResult PoiUpdate(UInt64 id, double longitude, double latitude, UInt32 coordType, string geotableId, Hashtable columnKeyValue = null,
            string title = null, string address = null, string tags = null)
        {
            //自定义唯一索引key	Value	用户自定义类型 page 32
            //columnKeyValue includ  index key value
            string paraUrlCoded = "longitude=" + longitude + "&latitude=" + latitude + "&coord_type=" + coordType + "&geotableId=" + geotableId
                + "&ak=" + _ak + "&id=" + id;
            if (!string.IsNullOrEmpty(title))
            {
                paraUrlCoded += ("&title=" + title);
            }
            if (!string.IsNullOrEmpty(address))
            {
                paraUrlCoded += ("&address=" + address);
            }
            if (!string.IsNullOrEmpty(tags))
            {
                paraUrlCoded += ("&tags=" + tags);
            }
            if (!string.IsNullOrEmpty(_sn))
            {
                paraUrlCoded += ("&sn=" + _sn);
            }
            if (columnKeyValue != null)
            {
                foreach (DictionaryEntry kv in columnKeyValue)
                {
                    paraUrlCoded += ("&" + kv.Key + "=" + kv.Value);
                }
            }
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            HttpWebResponse response = GetResponse(
                method: LBSYunNetSDKMethods.POST.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.update.ToString(),
                postData: postData
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            LBSYunNetSDKResult re = jss.Deserialize<LBSYunNetSDKResult>(json);
            return re;
        }

        public LBSYunNetSDKResult PoiDelete(UInt64 id, string geotableId, Hashtable indexKeyValue = null, string ids = null,
            string title = null, string bounds = null, string tags = null)
        {
            //自定义唯一索引key	Value	用户自定义类型 page 32
            //indexKeyValue includ  uniqueIndex key-value
            string paraUrlCoded = "ak=" + _ak + "&id=" + id;
            if (!string.IsNullOrEmpty(ids))
            {
                string p = @"^(\d+,*)+$";
                Regex reg = new Regex(p, RegexOptions.IgnoreCase);
                Match m = reg.Match(ids);
                if (!m.Success)
                {
                    throw new Exception("ids contains illegal characters.");
                }
                paraUrlCoded += ("&ids=" + ids);
            }
            if (!string.IsNullOrEmpty(title))
            {
                paraUrlCoded += ("&title=" + title);
            }
            if (!string.IsNullOrEmpty(bounds))
            {
                paraUrlCoded += ("&bounds=" + bounds);
            }
            if (!string.IsNullOrEmpty(tags))
            {
                paraUrlCoded += ("&tags=" + tags);
            }
            if (!string.IsNullOrEmpty(_sn))
            {
                paraUrlCoded += ("&sn=" + _sn);
            }
            if (indexKeyValue != null)
            {
                foreach (DictionaryEntry kv in indexKeyValue)
                {
                    paraUrlCoded += ("&" + kv.Key + "=" + kv.Value);
                }
            }
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            HttpWebResponse response = GetResponse(
                method: LBSYunNetSDKMethods.POST.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.delete.ToString(),
                postData: postData
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            LBSYunNetSDKResult re = jss.Deserialize<LBSYunNetSDKResult>(json);
            return re;
        }
        public LBSYunNetSDKResult PoiUpload(UInt32 geotableId, byte[] poi_list, UInt32 timestamp = 0)
        {
            string paraUrlCoded = "ak=" + _ak + "&geotableId=" + geotableId;

            if (timestamp != 0)
            {
                paraUrlCoded += ("&timestamp=" + timestamp);
            }


            byte[] postData = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            HttpWebResponse response = GetResponse(
                method: LBSYunNetSDKMethods.POST.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.upload.ToString(),
                postData: postData
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            LBSYunNetSDKResult re = jss.Deserialize<LBSYunNetSDKResult>(json);
            return re;
        }
        #endregion
        #region Get
        public string PoiListJson(string geotableId, UInt32 pageIndex = 0, UInt32 pageSize = 10, Hashtable indexKeyValue = null, string title = null, string bounds = null, string tags = null)
        {
            string paraUrlCoded = "ak=" + _ak + "&geotable_id=" + geotableId + "&page_index=" + pageIndex + "&page_size=" + pageSize;
            if (!string.IsNullOrEmpty(title))
            {
                paraUrlCoded += ("&title=" + title);
            }
            if (!string.IsNullOrEmpty(bounds))
            {
                paraUrlCoded += ("&bounds=" + bounds);
            }
            if (!string.IsNullOrEmpty(tags))
            {
                paraUrlCoded += ("&tags=" + tags);
            }
            if (!string.IsNullOrEmpty(_sn))
            {
                paraUrlCoded += ("&sn=" + _sn);
            }
            if (indexKeyValue != null)
            {
                foreach (DictionaryEntry kv in indexKeyValue)
                {
                    paraUrlCoded += ("&" + kv.Key + "=" + kv.Value);
                }
            }
            string getData = "?" + paraUrlCoded;
            HttpWebResponse response = GetResponse(
                method: LBSYunNetSDKMethods.GET.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.list.ToString(),
                getData: getData
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            return json;
            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //LBSYunNetSDKResult re = jss.Deserialize<LBSYunNetSDKResult>(json);
            //return re;
        }
        public IPoi PoiList(string geotableId, UInt32 pageIndex = 0, UInt32 pageSize = 10, Hashtable indexKeyValue = null, string title = null, string bounds = null, string tags = null)
        {
            string paraUrlCoded = "ak=" + _ak + "&geotable_id=" + geotableId + "&page_index=" + pageIndex + "&page_size=" + pageSize;
            if (!string.IsNullOrEmpty(title))
            {
                paraUrlCoded += ("&title=" + title);
            }
            if (!string.IsNullOrEmpty(bounds))
            {
                paraUrlCoded += ("&bounds=" + bounds);
            }
            if (!string.IsNullOrEmpty(tags))
            {
                paraUrlCoded += ("&tags=" + tags);
            }
            if (!string.IsNullOrEmpty(_sn))
            {
                paraUrlCoded += ("&sn=" + _sn);
            }
            if (indexKeyValue != null)
            {
                foreach (DictionaryEntry kv in indexKeyValue)
                {
                    paraUrlCoded += ("&" + kv.Key + "=" + kv.Value);
                }
            }
            string getData = "?" + paraUrlCoded;
            HttpWebResponse response = GetResponse(
                method: LBSYunNetSDKMethods.GET.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.list.ToString(),
                getData: getData
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            //return json;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            IPoi re = jss.Deserialize<IPoi>(json);
            return re;
        }

        public string PoiDetailJson(UInt64 id, int geotableId)
        {
            //百度地图LBS云存储APIv3.0接口说明文档.doc
            //geotable_id	 表主键	 int32	必须 page 33 ?
            string paraUrlCoded = "ak=" + _ak + "&geotable_id=" + geotableId + "&id=" + id;

            if (!string.IsNullOrEmpty(_sn))
            {
                paraUrlCoded += ("&sn=" + _sn);
            }
            string getData = "?" + paraUrlCoded;
            HttpWebResponse response = GetResponse(
                method: LBSYunNetSDKMethods.GET.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.detail.ToString(),
                getData: getData
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            return json;
        }

        public IPoi PoiDetail(UInt64 id, int geotableId)
        {
            //百度地图LBS云存储APIv3.0接口说明文档.doc
            //geotable_id	 表主键	 int32	必须 page 33 ?
            string paraUrlCoded = "ak=" + _ak + "&geotable_id=" + geotableId + "&id=" + id;

            if (!string.IsNullOrEmpty(_sn))
            {
                paraUrlCoded += ("&sn=" + _sn);
            }
            string getData = "?" + paraUrlCoded;
            HttpWebResponse response = GetResponse(
                method: LBSYunNetSDKMethods.GET.ToString(),
                entity: LBSYunNetSDKEntitys.poi.ToString(),
                operation: LBSYunNetSDKOperations.detail.ToString(),
                getData: getData
                );
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string json = sr.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            IPoi re = jss.Deserialize<IPoi>(json);
            return re;
        }
        #endregion
        #endregion
    }
}
