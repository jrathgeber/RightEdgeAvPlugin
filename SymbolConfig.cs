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


            // def getSymbolConfig(tickers, todaydate, maxdata):
            // tickers = ['SPI', 'ICON', 'CNET', 'RIOT', 'HMNY']
            // todaydate = "20180127"

            // f = open(r"C:\Users\Jason\AppData\Roaming\Yye Software\RightEdge\2010.1.0.0\SymbolConfig.xml", "r")

            //var f = File.OpenText("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\Samples\\RightEdgePluginsNew\\SymbolConfig1\\input.txt");
            //var copy = new StreamWriter("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\Samples\\RightEdgePluginsNew\\SymbolConfig1\\output.txt");

            var f = File.OpenText("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\Samples\\RightEdgePluginsNew\\SymbolConfig1\\SymbolConfigStart.xml");
            var copy = new StreamWriter("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\Samples\\RightEdgePluginsNew\\SymbolConfig1\\SymbolConfig.xml");


            // f = open(r"SymbolConfigStart.xml", "r")
            // copy = open("SymbolConfig.xml", "w")

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
             //f.close()
             //copy.close()


            //time.sleep(3)
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));


            //copyfile('SymbolConfig.xml', 'C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml')
            //File.Move("SymbolConfig.xml", "C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml");

            //Backup
            //copyfile('C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml', 'F:\\RightEdge\\MaxAlpha\\SymbolConfig' + todaydate + '.xml')
            //File.Move("C:\\Users\\Jason\\AppData\\Roaming\\Yye Software\\RightEdge\\2010.1.0.0\\SymbolConfig.xml", "F:\\RightEdge\\MaxAlpha\\SymbolConfig" + todaydate + ".xml");


        }
        
                
        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
            Console.WriteLine("Hello, World Right Edge");

            //os.chdir('C:\\dep\Mechanizd\\maxalpha\\')

            //daterun = time.strftime("%Y%m%d")
            var daterun = "20210621";

            Console.WriteLine("Date " + daterun);

            var maxdata = "hello";

            var tickerList = new List<String>() { "GME", "AMC", "TSLA", "VTNR" };
            
            Console.WriteLine("Ticker List : ");
            tickerList.ForEach(Console.WriteLine);
            //Console.WriteLine(tickerList);

            //Test It
            SymbolConfig p = new SymbolConfig();
            p.getSymbolConfig(tickerList, daterun, maxdata);
                                

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));

        }
    }
}
