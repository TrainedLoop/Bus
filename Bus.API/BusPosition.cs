
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Bus.API
{
    public  class BusPosition
    {
        public DateTime Date { get; set; }
        public string Id { get; set; }
        public string Line { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double Speed { get; set; }

    }
}