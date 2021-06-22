using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolConfig
{
    class SymbolConfig
    {

        private void symbolwrite (StreamWriter copy, string todaydate, List<String> symbols, string maxdata)
            {
            
                foreach (string symbol in symbols)
                    {
                    copy.WriteLine("                 <WatchListItem>\n");
                    copy.WriteLine("                  <IsFolder>false</IsFolder>\n");
                    copy.WriteLine("                  <BarConstruction>Default</BarConstruction>\n");
                    copy.WriteLine("                  <Symbol>\n");
                    copy.WriteLine("                   <StrikePrice>0</StrikePrice>\n");
                    copy.WriteLine("                   <ExpirationDate>0001-01-01T00:00:00</ExpirationDate>\n");
                    copy.WriteLine("                   <Name>" + symbol + "</Name>\n");
                    copy.WriteLine("                   <SymbolInformation>\n");
                    copy.WriteLine("                    <Margin>0</Margin>\n");
                    copy.WriteLine("                    <TickSize>0</TickSize>\n");
                    copy.WriteLine("                    <ContractSize>0</ContractSize>\n");
                    copy.WriteLine("                    <DecimalPlaces>2</DecimalPlaces>\n");
                    copy.WriteLine("                    <ShortMargin>0</ShortMargin>\n");
                    copy.WriteLine("                    <CustomHistoricalData>" + todaydate + "</CustomHistoricalData>\n");
                    copy.WriteLine("                    <CustomLiveData>" + maxdata + "</CustomLiveData>\n");
                    copy.WriteLine("                    </SymbolInformation>\n");
                    copy.WriteLine("                   </Symbol>\n");
                    copy.WriteLine("                  </WatchListItem>\n");
                }

            }

        private void copywrite(StreamWriter copy, string todaydate, List<String> symbols, string maxdata)
        {

                    copy.WriteLine("             <WatchListItem>\n");
                    copy.WriteLine("               <IsFolder>true</IsFolder>\n");
                    copy.WriteLine("               <BarConstruction>Default</BarConstruction>\n");
                    copy.WriteLine("               <Folder>\n");
                    copy.WriteLine("                <FolderName>" + todaydate +"</FolderName>\n");
                    copy.WriteLine("                <Contents>\n");
                    symbolwrite(copy, todaydate, symbols, maxdata);
                    copy.WriteLine("                 </Contents>\n");
                    copy.WriteLine("                 <Frequency>-1</Frequency>\n");
                    copy.WriteLine("                 <InheritsHistService>true</InheritsHistService>\n");
                    copy.WriteLine("                 <HistService />\n");
                    copy.WriteLine("                 <InheritsRealtimeService>true</InheritsRealtimeService>\n");
                    copy.WriteLine("                 <RealtimeService />\n");
                    copy.WriteLine("                 <InheritsBrokerService>true</InheritsBrokerService>\n");
                    copy.WriteLine("                 <BrokerService />\n");
                    copy.WriteLine("                 <InheritsSaveTicks>true</InheritsSaveTicks>\n");
                    copy.WriteLine("                 <SaveTicks>false</SaveTicks>\n");
                    copy.WriteLine("                 <InheritsSaveBars>true</InheritsSaveBars>\n");
                    copy.WriteLine("                 <SaveBars>true</SaveBars>\n");
                    copy.WriteLine("                 <DownloadStartDate>0001-01-01T00:00:00</DownloadStartDate>\n");
                    copy.WriteLine("                 <InheritsDownloadStartDate>true</InheritsDownloadStartDate>\n");
                    copy.WriteLine("                 <InheritsFrequency>true</InheritsFrequency>\n");
                    copy.WriteLine("               </Folder>\n");
                    copy.WriteLine("             </WatchListItem>\n");
        }


        private void getSymbolConfig( List<String> symbols, string todaydate, string maxdata)
        {

            
            // The real file - for me
            var f = File.OpenText("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml");


            // Testing / copies
            // var f = File.OpenText("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\Samples\\RightEdgePluginsNew\\SymbolConfig1\\SymbolConfigStart.xml");
            var copy = new StreamWriter("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\Samples\\RightEdgePluginsNew\\SymbolConfig1\\SymbolConfig.xml");


            // my crazy loop throught the right esge symbole config
            int x = 0;

            string line;
            while (null != (line = f.ReadLine()))
            {

                x = x * 2;
                if (x == 4)
                {
                    copywrite(copy, todaydate, symbols, maxdata);
                }

                if (line.Contains("MaxAlpha"))
                { 
                    x = 1;
                    
                }
                copy.WriteLine(line);

            }

            
            //print(line)
            f.Close();
            copy.Close();
            
            // Good idea to wait till all write operations are finnished
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));

            // Copy the file into the right place
            File.Copy("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\Samples\\RightEdgePluginsNew\\SymbolConfig1\\SymbolConfig.xml", "C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml", true);

            //Backup - this is a good idea
            //File.Copy('C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml', 'F:\\RightEdge\\MaxAlpha\\SymbolConfig' + todaydate + '.xml')
            //File.Move("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml", "F:\\RightEdge\\MaxAlpha\\SymbolConfig" + todaydate + ".xml");


        }


        // Run this method
        static void Main(string[] args)
        {

            Console.WriteLine("Hello, World, RightEdge, Is Here.");

            //daterun = time.strftime("%Y%m%d")
            var daterun = "20210623";

            Console.WriteLine("Date " + daterun);

            var maxdata = "dummy value";

            var tickerList = new List<String>() { "IBM", "AMC", "TSLA", "VTNR" };
            
            Console.WriteLine("Ticker List : ");
            tickerList.ForEach(Console.WriteLine);

            //Test It
            SymbolConfig p = new SymbolConfig();
            p.getSymbolConfig(tickerList, daterun, maxdata);
                                

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));

        }
    }
}
