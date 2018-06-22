﻿using BillingSystem.Model;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class TabView
    {
        public List<Tabs> TabsList { get; set; }
        public Tabs CurrentTab { get; set; }
        public List<Screen> ScreenList { get; set; }
    }
}