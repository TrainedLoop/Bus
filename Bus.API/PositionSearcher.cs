using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bus.API
{
    public static class PositionSearcher
    {
        private static List<BusPosition> BusPositionList = new List<BusPosition>();
        private static List<BusPosition> BusPositionListTemp = new List<BusPosition>();
        public static DateTime LastSearch = DateTime.MinValue;
        public readonly static int MaxRefreshTime = 5;
        public readonly static int SecondIntevalToRefresh = 60;
        private static Task getInfoTask = new Task(GetInfo);


        public static List<BusPosition> GetLineInfo(string line)
        {

            if (LastSearch.AddSeconds(SecondIntevalToRefresh) < DateTime.Now && (getInfoTask.Status == TaskStatus.Created || getInfoTask.Status == TaskStatus.RanToCompletion))
            {
                getInfoTask = new Task(GetInfo);                
                getInfoTask.Start();
            }
            if (BusPositionList.Count == 0 || LastSearch.AddMinutes(MaxRefreshTime) < DateTime.Now)
                getInfoTask.Wait();

            return BusPositionList.Where(i => i.Line == line && i.Date.AddMinutes(MaxRefreshTime) < DateTime.Now).ToList();
        }


        private static void GetInfo()
        {

            var jsonText = new WebClient().DownloadString("http://dadosabertos.rio.rj.gov.br/apiTransporte/apresentacao/rest/index.cfm/obterTodasPosicoes.json");
            var jsonDesc = Json.JsonParser.FromJson(jsonText);
            dynamic JsonDescList = jsonDesc.Where(i => i.Key == "DATA").FirstOrDefault().Value;
            foreach (var item in JsonDescList)
            {
                BusPositionListTemp.Add(
                    new BusPosition
                    {
                        Date = DateTime.Parse(item[0], new CultureInfo("en-US")),
                        Id = item[1].ToString(),
                        Line = item[2].ToString(),
                        Lat = Convert.ToDouble(item[3], new CultureInfo("en-US")),
                        Lng = Convert.ToDouble(item[4], new CultureInfo("en-US")),
                        Speed = Convert.ToDouble(item[5], new CultureInfo("en-US"))
                    });
            }
            BusPositionList = BusPositionListTemp;
            LastSearch = DateTime.Now;

        }




    }
}
