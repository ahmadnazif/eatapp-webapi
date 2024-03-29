﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Models
{
    public class Test
    {
        public string AppName { get; set; }
        public string EnvironmentName { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string ServerTimezoneId { get; set; }
        public string ServerTimezoneOffset { get; set; }
        public string CurrentUtcTime { get; set; }
        public string ContentRootPath { get; set; }
    }
}
