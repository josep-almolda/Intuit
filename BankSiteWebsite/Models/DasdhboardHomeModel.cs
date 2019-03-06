using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Intuit.Ipp.Data;

namespace BankSiteWebsite.Models
{
    public class DasdhboardHomeModel
    {
        public DasdhboardHomeModel()
        {
            Invoices = new ReadOnlyCollection<Invoice>(new List<Invoice>());    
        }

        public ReadOnlyCollection<Invoice> Invoices { get; set; }
        public DateTime Date { get; set; }
    }
}