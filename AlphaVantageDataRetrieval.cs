using System;
using System.Net;
using System.IO;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using RightEdge.Common;
using RightEdge.Common.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Windows.Forms;

namespace RightEdge.DataRetrieval
{
    public class AlphaVanatageDataRetrieval : IService, IBarDataRetrieval
    {

        class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-mm-dd";
            }
        }


        class CustomDateTimeConverterIntra : IsoDateTimeConverter
        {
            public CustomDateTimeConverterIntra()
            {
                base.DateTimeFormat = "yyyy-mm-dd hh:mm:ss";
            }
        }


        public class StockHistory
        {

            [JsonProperty("Meta Data")]
            public MetaData md { get; set; }


            [JsonProperty("Time Series (Daily)")]
            public Dictionary<string, TimeSeries> tsd { get; set; }

        }


        public class TimeSeriesDaily
        {

            [JsonProperty(ItemConverterType = typeof(CustomDateTimeConverter))]
            public TimeSeries ts { get; set; }

        }

        public class TimeSeries
        {
            [JsonProperty("1. open")]
            public string Open { get; set; }

            [JsonProperty("2. high")]
            public string High { get; set; }

            [JsonProperty("3. low")]
            public string Low { get; set; }

            [JsonProperty("4. close")]
            public string Close { get; set; }

            [JsonProperty("5. volume")]
            public string Volume { get; set; }

        }

        public class MetaData
        {
            [JsonProperty("1. Information")]
            public string Info { get; set; }

            [JsonProperty("2. Symbol")]
            public string Symbol { get; set; }

            [JsonProperty("3. Last Refreshed")]
            public string Last { get; set; }

            [JsonProperty("4. Output Size")]
            public string Size { get; set; }

            [JsonProperty("5. Time Zone")]
            public string TimeZone { get; set; }

        }


        public class StockQuote
        {

            [JsonProperty("Realtime Global Securities Quote")]
            public RealTime rt { get; set; }

        }


        public class RealTime
        {
            [JsonProperty("01. Symbol")]
            public string Info { get; set; }

            [JsonProperty("02. Exchange Name")]
            public string Symbol { get; set; }

            [JsonProperty("03. Latest Price")]
            public string Last { get; set; }

            [JsonProperty("04. Open (Current Trading Day)")]
            public string OPen { get; set; }

            [JsonProperty("05. High (Current Trading Day)")]
            public string High { get; set; }

            [JsonProperty("06. Low (Current Trading Day)")]
            public string Low { get; set; }

            [JsonProperty("07. Close (Previous Trading Day)")]
            public string Close { get; set; }

            [JsonProperty("08. Price Change")]
            public string Change { get; set; }

            [JsonProperty("09. Price Change Percentage")]
            public string Percent { get; set; }

            [JsonProperty("10. Volume (Current Trading Day)")]
            public string Volume { get; set; }

            [JsonProperty("11. Last Updated")]
            public string Updated { get; set; }


            /*
            "Realtime Global Securities Quote": {
            "01. Symbol": "MSFT",
            "02. Exchange Name": "NASDAQ",
            "03. Latest Price": "70.0400",
            "04. Open (Current Trading Day)": "70.5400",
            "05. High (Current Trading Day)": "70.5900",
            "06. Low (Current Trading Day)": "69.7100",
            "07. Close (Previous Trading Day)": "70.2700",
            "08. Price Change": "-0.2300",
            "09. Price Change Percentage": "-0.33%",
            "10. Volume (Current Trading Day)": "13.32M",
            "11. Last Updated": "Jun 22, 1:09PM EDT"
            */

        }


        public class IntraDayData1m
        {

            [JsonProperty("Meta Data")]
            public IntraDayMeta md { get; set; }


            [JsonProperty("Time Series (1min)")]
            public Dictionary<string, TimeSeries> tsi1 { get; set; }


        }


        public class IntraDayData5m
        {

            [JsonProperty("Meta Data")]
            public IntraDayMeta md { get; set; }


            [JsonProperty("Time Series (5min)")]
            public Dictionary<string, TimeSeries> tsi5 { get; set; }


        }



        public class TimeSeriesIntraDay
        {

            [JsonProperty(ItemConverterType = typeof(CustomDateTimeConverterIntra))]
            public TimeSeries ts { get; set; }

        }


        public class IntraDayMeta
        {
            [JsonProperty("1. Information")]
            public string Info { get; set; }

            [JsonProperty("2. Symbol")]
            public string Symbol { get; set; }

            [JsonProperty("3. Last Refreshed")]
            public string Last { get; set; }

            [JsonProperty("4. Interval")]
            public string Interval { get; set; }

            [JsonProperty("5. Output Size")]
            public string Size { get; set; }

            [JsonProperty("6. Time Zone")]
            public string TimeZone { get; set; }

        }



        string lastError = "";
        bool useAdjustedClose = true;

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IService Members

        public event EventHandler<ServiceEventArgs> ServiceEvent;

        public bool Initialize(SerializableDictionary<string, string> settings)
        {
            string adjustedClose = "";

            if (settings.TryGetValue("UseAdjustedClose", out adjustedClose))
            {
                useAdjustedClose = Convert.ToBoolean(adjustedClose);
            }

            return true;
        }

        public bool HasCustomSettings()
        {
            return true;
        }

        public bool ShowCustomSettingsForm(ref SerializableDictionary<string, string> settings)
        {
            UseAdjustedCloseForm dlg = new UseAdjustedCloseForm();

            string adjustedClose = "";

            if (settings.TryGetValue("UseAdjustedClose", out adjustedClose))
            {
                useAdjustedClose = Convert.ToBoolean(adjustedClose);
            }

            dlg.UseAdjustedClose = useAdjustedClose;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                useAdjustedClose = dlg.UseAdjustedClose;
                settings["UseAdjustedClose"] = useAdjustedClose.ToString();
            }

            return true;
        }

        public string ServiceName()
        {
            return "AlphaVantage";
        }

        public string Author()
        {
            return "Jason Rathgeber";
        }

        public string Description()
        {
            return "Retrieves end of day data from the Alpha Vanatage web site.";
        }

        public string CompanyName()
        {
            return "Jason Rathgeber .com";
        }

        public string Version()
        {
            return "1.0";
        }

        public string id()
        {
            return "{FFE9F113-23A8-4F93-8A46-834D17405468}";
        }

        public bool NeedsServerAddress()
        {
            return false;
        }

        public bool NeedsPort()
        {
            return false;
        }

        public bool NeedsAuthentication()
        {
            return false;
        }

        public bool SupportsMultipleInstances()
        {
            return true;
        }

        public string ServerAddress
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public int Port
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public string UserName
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public string Password
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public bool BarDataAvailable
        {
            get { return true; }
        }

        public bool TickDataAvailable
        {
            get { return false; }
        }

        public bool BrokerFunctionsAvailable
        {
            get { return false; }
        }

        public bool SymbolSourceAvailable
        {
            get { return true; }
        }

        public IBarDataRetrieval GetBarDataInterface()
        {
            return this;
        }

        public ITickRetrieval GetTickDataInterface()
        {
            return null;
        }

        public IBroker GetBrokerInterface()
        {
            return null;
        }

        public ISymbolSource GetSymbolSource()
        {
            return new IndexSymbolSource();
        }

        public bool Connect(ServiceConnectOptions connectOptions)
        {
            return true;
        }

        public bool Disconnect()
        {
            return true;
        }

        public string GetError()
        {
            return lastError;
        }

        #endregion

        #region IBarDataRetrieval Members


        public List<BarData> RetrieveData(Symbol symbol, int frequency, DateTime startDate, DateTime endDate, BarConstructionType barConstruction)
        {


            if (frequency != (int)BarFrequency.Daily && frequency != (int)BarFrequency.FiveMinute && frequency != (int)BarFrequency.OneMinute)
            {
                return new List<BarData>();
            }

            if (startDate == DateTime.MinValue)
            {
                // I think this is about as far back as Yahoo will go
                startDate = new DateTime(1960, 1, 1);
            }


            int startMonth = startDate.Month - 1;
            int endMonth = endDate.Month - 1;


            // Alphavantage API Demo
            // https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=MSFT&apikey=XXPUTYOURKEYHERE

            string baseURL = "https://www.alphavantage.co/query?";
            string url = baseURL + "function=TIME_SERIES_DAILY&symbol=" + symbol.Name + "&apikey=XXPUTYOURKEYHERE";



            if (frequency == (int)BarFrequency.Daily)
            {
                // Use Above
            }

            if (startDate.Date == DateTime.Today)
            {
                // Alphavantage 
                // https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=GSAT&apikey=XXPUTYOURKEYHERE

                baseURL = "https://www.alphavantage.co/query?";
                url = baseURL + "function=GLOBAL_QUOTE&symbol=" + symbol.Name + "&apikey=XXPUTYOURKEYHERE";

            }

            if (frequency == (int)BarFrequency.FiveMinute)
            {
                // Alphavantage 
                // https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=MSFT&interval=5min&apikey=XXPUTYOURKEYHERE

                baseURL = "https://www.alphavantage.co/query?";
                url = baseURL + "function=TIME_SERIES_INTRADAY&symbol=" + symbol.Name + "&interval=5min&outputsize=full&apikey=XXPUTYOURKEYHERE";

            }


            if (frequency == (int)BarFrequency.OneMinute)
            {
                // Alphavantage 
                // https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=MSFT&interval=1min&apikey=XXPUTYOURKEYHERE

                baseURL = "https://www.alphavantage.co/query?";
                url = baseURL + "function=TIME_SERIES_INTRADAY&symbol=" + symbol.Name + "&interval=1min&outputsize=full&apikey=XXPUTYOURKEYHERE";

            }

            
            WebResponse result = null;
            WebRequest req = WebRequest.Create(url);
            result = req.GetResponse();
            Stream receiveStream = result.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader sr = new StreamReader(receiveStream, encode);

            List<BarData> bars = new List<BarData>();

            if (frequency == (int)BarFrequency.OneMinute)
            {
                PutRawDataInBarCollectionIntra1m(sr, bars);

            }
           else
           if (frequency == (int)BarFrequency.FiveMinute)
            {
                PutRawDataInBarCollectionIntra5m(sr, bars);

            }
            else if (startDate.Date == DateTime.Today)
            {
                PutRawDataInBarCollectionRT(sr, bars);
            }
            else
            {
                PutRawDataInBarCollectionJSON(sr, bars);
            }


            sr.Close();

            List<BarData> ret = new List<BarData>();
            //	Sort bars and remove duplicates
            foreach (BarData bar in bars.OrderBy(b => b.BarStartTime))
            {
                if (ret.Count > 0 && ret[ret.Count - 1].BarStartTime == bar.BarStartTime)
                {
                    //	Avoid duplicate bars (but take the "later" one)
                    ret[ret.Count - 1] = bar;
                }
                else
                {
                    ret.Add(bar);
                }
            }

            return ret;

            //return bars.OrderBy(b => b.BarStartTime).ToList();

        }


        private void PutRawDataInBarCollectionJSON(StreamReader jsonWebResults, List<BarData> bars)
        {
            StockHistory jsonDeSerialize;

            try
            {

            // deserialize JSON directly
            using (jsonWebResults)
            {
                JsonSerializer serializer = new JsonSerializer();
                jsonDeSerialize = (StockHistory)serializer.Deserialize(jsonWebResults, typeof(StockHistory));
            }

            foreach (KeyValuePair<string, TimeSeries> entry in jsonDeSerialize.tsd)
            {
                IFormatProvider culture = new CultureInfo("en-US");

                // need to set culture info for non-US folks since we're getting
                // data from a US source
                if (entry.Value.Open != null)
                {
                    BarData bar = new BarData();
                    bar.BarStartTime = DateTime.Parse(entry.Key, culture);

                    bar.Open = double.Parse(entry.Value.Open, NumberStyles.Number, culture);
                    bar.High = double.Parse(entry.Value.High, NumberStyles.Number, culture);
                    bar.Low = double.Parse(entry.Value.Low, NumberStyles.Number, culture);
                    bar.Close = double.Parse(entry.Value.Close, NumberStyles.Number, culture);
                    bar.Volume = ulong.Parse(entry.Value.Volume, NumberStyles.Number, culture);

                    if(bar.Close < bar.Low)
                    { bar.Low = bar.Close; }

                        if (bar.Open != 0.0 && bar.High != 0.0 && bar.Low != 0.0 && bar.Close != 0.0  )
                        {
                            bars.Add(bar);
                        }
                }
                else
                {
                    Trace.WriteLine("Error parsing data from source.  Source line = ?");
                }
            }

            }
            catch (Exception e)
            {

                Trace.WriteLine("Error parsing data from source.  Source line = ?");
            }


        }


        private void PutRawDataInBarCollectionRT(StreamReader jsonWebResults, List<BarData> bars)
        {
            StockQuote jsonDeSerialize;

            try
            {

                // deserialize JSON directly
                using (jsonWebResults)
                {

                    JsonSerializer serializer = new JsonSerializer();
                    jsonDeSerialize = (StockQuote)serializer.Deserialize(jsonWebResults, typeof(StockQuote));

                }

                IFormatProvider culture = new CultureInfo("en-US");

                // need to set culture info for non-US folks since we're getting
                // data from a US source
                if (jsonDeSerialize.rt.OPen != null)
                {


                    BarData bar = new BarData();
                    //bar.BarStartTime = DateTime.Parse(entry.Key, culture);

                    bar.Open = double.Parse(jsonDeSerialize.rt.OPen, NumberStyles.Number, culture);
                    //MessageBox.Show("H4");
                    bar.High = double.Parse(jsonDeSerialize.rt.High, NumberStyles.Number, culture);
                    //MessageBox.Show("H5");
                    bar.Low = double.Parse(jsonDeSerialize.rt.Low, NumberStyles.Number, culture);
                    //MessageBox.Show("H6");
                    bar.Close = double.Parse(jsonDeSerialize.rt.Close, NumberStyles.Number, culture);
                    //MessageBox.Show("H7");
                    //bar.Volume = ulong.Parse(jsonDeSerialize.rt.Volume, NumberStyles.Number, culture);
                    bar.Volume = 0;

                    if (bar.Open != 0.0 && bar.High != 0.0 && bar.Low != 0.0 && bar.Close != 0.0)
                    {
                        bars.Add(bar);
                    }
                }
                else
                {
                    Trace.WriteLine("Error parsing data from source.  Source line = ?");
                }

            }
            catch (Exception e)
            {

                // System.FormatException

                Trace.WriteLine("Error parsing data from source.  Source line = ?");
            }
        }
        private void PutRawDataInBarCollectionIntra1m(StreamReader jsonWebResults, List<BarData> bars)
        {
            IntraDayData1m jsonDeSerialize;

            // deserialize JSON directly
            using (jsonWebResults)
            {
                JsonSerializer serializer = new JsonSerializer();
                jsonDeSerialize = (IntraDayData1m)serializer.Deserialize(jsonWebResults, typeof(IntraDayData1m));
            }


            foreach (KeyValuePair<string, TimeSeries> entry in jsonDeSerialize.tsi1)
            {
                IFormatProvider culture = new CultureInfo("en-US");

                // need to set culture info for non-US folks since we're getting
                // data from a US source
                if (1 == 1)
                {
                    BarData bar = new BarData();
                    bar.BarStartTime = DateTime.Parse(entry.Key, culture);

                    bar.Open = double.Parse(entry.Value.Open, NumberStyles.Number, culture);
                    bar.High = double.Parse(entry.Value.High, NumberStyles.Number, culture);
                    bar.Low = double.Parse(entry.Value.Low, NumberStyles.Number, culture);
                    bar.Close = double.Parse(entry.Value.Close, NumberStyles.Number, culture);
                    bar.Volume = ulong.Parse(entry.Value.Volume, NumberStyles.Number, culture);

                    bars.Add(bar);
                }
                else
                {
                    Trace.WriteLine("Error parsing data from source.  Source line = ?");
                }
            }

        }

        private void PutRawDataInBarCollectionIntra5m(StreamReader jsonWebResults, List<BarData> bars)
        {
            IntraDayData5m jsonDeSerialize;

            // deserialize JSON directly
            using (jsonWebResults)
            {
                JsonSerializer serializer = new JsonSerializer();
                jsonDeSerialize = (IntraDayData5m)serializer.Deserialize(jsonWebResults, typeof(IntraDayData5m));
            }


            foreach (KeyValuePair<string, TimeSeries> entry in jsonDeSerialize.tsi5)
            {
                IFormatProvider culture = new CultureInfo("en-US");

                // need to set culture info for non-US folks since we're getting
                // data from a US source
                if (1 == 1)
                {
                    BarData bar = new BarData();
                    bar.BarStartTime = DateTime.Parse(entry.Key, culture);

                    bar.Open = double.Parse(entry.Value.Open, NumberStyles.Number, culture);
                    bar.High = double.Parse(entry.Value.High, NumberStyles.Number, culture);
                    bar.Low = double.Parse(entry.Value.Low, NumberStyles.Number, culture);
                    bar.Close = double.Parse(entry.Value.Close, NumberStyles.Number, culture);
                    bar.Volume = ulong.Parse(entry.Value.Volume, NumberStyles.Number, culture);

                    bars.Add(bar);
                }
                else
                {
                    Trace.WriteLine("Error parsing data from source.  Source line = ?");
                }
            }

        }

        public IService GetService()
        {
            return this;
        }

    }

    #endregion
}
