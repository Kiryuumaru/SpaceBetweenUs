using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public static class CommonHelpers
    {
        #region Serializer

        private const string UIDCaseSensetiveCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; // 62 kabuok
        private const string UIDNonCaseSensetiveCharset = "0123456789abcdefghijklmnopqrstuvwxyz"; // 36 kabuok

        public static string[] Split(string data, params int[] lengths)
        {
            int sizes = 0;
            foreach (int size in lengths)
            {
                sizes += size;
            }

            if (sizes != data.Length)
            {
                throw new Exception("Split error");
            }

            string[] datas = new string[lengths.Length];
            int lastIndex = 0;
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = data.Substring(lastIndex, lengths[i]);
                lastIndex += lengths[i];
            }
            return datas;
        }

        public static string EncodeDateTime(DateTime dateTime)
        {
            string data = "";
            data += dateTime.Year.ToString("0000");
            data += dateTime.Month.ToString("00");
            data += dateTime.Day.ToString("00");
            data += dateTime.Hour.ToString("00");
            data += dateTime.Minute.ToString("00");
            data += dateTime.Second.ToString("00");
            return data;
        }

        public static DateTime DecodeDateTime(string data, DateTime defaultValue)
        {
            try
            {
                if (data.Length != 14)
                {
                    return defaultValue;
                }

                string[] datas = Split(data, 4, 2, 2, 2, 2, 2);
                int year = Convert.ToInt32(datas[0]);
                int month = Convert.ToInt32(datas[1]);
                int day = Convert.ToInt32(datas[2]);
                int hour = Convert.ToInt32(datas[3]);
                int minute = Convert.ToInt32(datas[4]);
                int second = Convert.ToInt32(datas[5]);
                return new DateTime(year, month, day, hour, minute, second);
            }
            catch { return defaultValue; }
        }

        public static DateTime? DecodeDateTime(string data)
        {
            try
            {
                string[] datas = Split(data, 4, 2, 2, 2, 2, 2);
                int year = Convert.ToInt32(datas[0]);
                int month = Convert.ToInt32(datas[1]);
                int day = Convert.ToInt32(datas[2]);
                int hour = Convert.ToInt32(datas[3]);
                int minute = Convert.ToInt32(datas[4]);
                int second = Convert.ToInt32(datas[5]);
                return new DateTime(year, month, day, hour, minute, second);
            }
            catch { return null; }
        }

        public static string BlobGetValue(string blob, string key, string defaultValue = "")
        {
            string[] blobArray = DeserializeString(blob);
            if (blobArray == null)
            {
                return defaultValue;
            }
            else if (blobArray.Length <= 1)
            {
                return defaultValue;
            }
            else if (blobArray.Length % 2 != 0)
            {
                return defaultValue;
            }

            int keyIndex = blobArray.ToList().IndexOf(key);
            if (keyIndex != 1 && keyIndex % 2 == 0 && (keyIndex + 1) < blobArray.Length)
            {
                return blobArray[keyIndex + 1];
            }
            else
            {
                return defaultValue;
            }
        }

        public static string BlobSetValue(string blob, string key, string value)
        {
            string[] blobArray = DeserializeString(blob);
            if (blobArray == null)
            {
                blobArray = Array.Empty<string>();
            }
            else if (blobArray.Length <= 1)
            {
                blobArray = Array.Empty<string>();
            }
            else if (blobArray.Length % 2 != 0)
            {
                blobArray = Array.Empty<string>();
            }

            int keyIndex = blobArray.ToList().IndexOf(key);
            if (keyIndex != 1 && keyIndex % 2 == 0 && (keyIndex + 1) < blobArray.Length)
            {
                blobArray[keyIndex + 1] = value;
                return SerializeString(blobArray);
            }
            else
            {
                List<string> newBlobArray = blobArray.ToList();
                newBlobArray.Add(key);
                newBlobArray.Add(value);
                return SerializeString(newBlobArray.ToArray());
            }
        }

        public static string BlobDeleteValue(string blob, string key)
        {
            string[] blobArray = DeserializeString(blob);
            if (blobArray == null)
            {
                return blob;
            }
            else if (blobArray.Length <= 1)
            {
                return blob;
            }
            else if (blobArray.Length % 2 != 0)
            {
                return blob;
            }

            int keyIndex = blobArray.ToList().IndexOf(key);
            if (keyIndex != 1 && keyIndex % 2 == 0 && (keyIndex + 1) < blobArray.Length)
            {
                List<string> newBlobArray = blobArray.ToList();
                newBlobArray.RemoveAt(keyIndex);
                newBlobArray.RemoveAt(keyIndex);
                return SerializeString(newBlobArray.ToArray());
            }
            else
            {
                return blob;
            }
        }

        public static string BlobConvert(IEnumerable<(string Key, string Value)> list)
        {
            string blob = "";
            foreach ((string Key, string Value) in list)
            {
                blob = BlobSetValue(blob, Key, Value);
            }
            return blob;
        }

        public static List<(string Key, string Value)> BlobConvert(string blob)
        {
            List<(string Key, string Value)> list = new List<(string Key, string Value)>();
            string[] blobArray = DeserializeString(blob);
            if (blobArray == null)
            {
                return list;
            }
            else if (blobArray.Length <= 1)
            {
                return list;
            }
            else if (blobArray.Length % 2 != 0)
            {
                return list;
            }

            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            for (int i = 0; i < blobArray.Length; i++)
            {
                if (i != 1 && i % 2 == 0 && (i + 1) < blobArray.Length)
                {
                    keys.Add(blobArray[i]);
                }
                else
                {
                    values.Add(blobArray[i]);
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                list.Add((keys[i], values[i]));
            }
            return list;
        }

        public static string GenerateUID(int length = 10, bool isCaseSensetive = true)
        {
            string id = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                id += isCaseSensetive ?
                    UIDCaseSensetiveCharset[random.Next(UIDCaseSensetiveCharset.Length)] :
                    UIDNonCaseSensetiveCharset[random.Next(UIDNonCaseSensetiveCharset.Length)];
            }
            return id;
        }

        #endregion

        #region ByteSerializer

        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            using MemoryStream msi = new MemoryStream(bytes);
            using MemoryStream mso = new MemoryStream();
            using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
            {
                CopyTo(msi, gs);
            }

            return mso.ToArray();
        }

        public static string Unzip(byte[] bytes)
        {
            using MemoryStream msi = new MemoryStream(bytes);
            using MemoryStream mso = new MemoryStream();
            using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                CopyTo(gs, mso);
            }

            return Encoding.UTF8.GetString(mso.ToArray());
        }

        #endregion

        #region StringArraySerializer

        private const string NullIdentifier = "N";
        private const string EmptyIdentifier = "E";

        public static string SerializeString(params string[] datas)
        {
            if (datas == null)
            {
                return NullIdentifier;
            }

            if (datas.Length == 0)
            {
                return EmptyIdentifier;
            }

            int maxLength = datas.Max(i => i == null ? 0 : i.Length);
            int indexDigits = Math.Max(datas.Length.ToString().Length, Math.Max(maxLength.ToString().Length, Math.Max(NullIdentifier.Length, EmptyIdentifier.Length)));
            string serializedDataHeader = datas.Length.ToString("D" + indexDigits);
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i] == null)
                {
                    serializedDataHeader += (indexDigits - NullIdentifier.Length == 0 ? "" : 0.ToString("D0" + (indexDigits - NullIdentifier.Length))) + NullIdentifier;
                }
                else
                {
                    serializedDataHeader += datas[i].Length.ToString("D" + indexDigits);
                }
            }
            string dataBody = "";
            for (int i = 0; i < datas.Length; i++)
            {
                dataBody += datas[i];
            }
            return indexDigits.ToString() + serializedDataHeader + dataBody;
        }

        public static string[] DeserializeString(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            if (data.Equals(NullIdentifier))
            {
                return null;
            }

            if (data.Equals(EmptyIdentifier))
            {
                return Array.Empty<string>();
            }

            if (data.Length < 4)
            {
                return new string[] { "" };
            }

            try
            {
                int indexDigits = int.Parse(data[0].ToString());
                data = data.Substring(1);
                int indexCount = int.Parse(data.Substring(0, indexDigits));
                data = data.Substring(indexDigits);
                int[] lengths = new int[indexCount];
                for (int i = 0; i < lengths.Length; i++)
                {
                    string subData = data.Substring(0, indexDigits);
                    data = data.Substring(indexDigits);
                    if (subData.Equals((indexDigits - NullIdentifier.Length == 0 ? "" : 0.ToString("D0" + (indexDigits - NullIdentifier.Length))) + NullIdentifier))
                    {
                        lengths[i] = -1;
                    }
                    else
                    {
                        lengths[i] = int.Parse(subData);
                    }
                }
                string[] datas = new string[indexCount];
                for (int i = 0; i < datas.Length; i++)
                {
                    if (lengths[i] == -1)
                    {
                        datas[i] = null;
                    }
                    else
                    {
                        datas[i] = data.Substring(0, lengths[i]);
                        data = data.Substring(lengths[i]);
                    }
                }
                return datas;
            }
            catch { return null; }
        }

        #endregion

        #region Math

        public static double CalcVariance(IEnumerable<double> datas)
        {
            double mean = datas.Average();
            double sum = 0;
            foreach (double data in datas)
            {
                sum += Math.Pow(data - mean, 2);
            }

            return sum / (datas.Count() - 1);
        }

        public static double CalcStandardDeviation(IEnumerable<double> datas)
        {
            double mean = datas.Average();
            double sum = 0;
            foreach (double data in datas)
            {
                sum += Math.Pow(data - mean, 2);
            }

            return Math.Pow(sum / datas.Count(), 0.5);
        }

        #endregion

        #region Stringer

        public static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException(nameof(decimalPlaces)); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        #endregion
    }
}
