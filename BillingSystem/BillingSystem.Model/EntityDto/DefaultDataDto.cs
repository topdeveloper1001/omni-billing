﻿using BillingSystem.Common;

namespace BillingSystem.Model.EntityDto
{
    public class DefaultDataDto
    {
        public SelectList Country { get; set; }
        public SelectList State { get; set; }
        public SelectList City { get; set; }
    }
}
