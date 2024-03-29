﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUserInterface
{
    public class WebsiteDataModel
    {
        public string WebsiteUrl { get; set; } = "";
        public string WebsiteData { get; set; } = "";
        public string ThreadElapsedTime { get; set; } = "";
        public long ElapsedTimeLong { get; set; } = 0;
        public bool getResults { get; set; } = false;
    }
}
