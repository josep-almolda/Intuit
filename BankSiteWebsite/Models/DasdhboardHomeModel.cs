using System;
using System.Collections.ObjectModel;
using Intuit.Ipp.Data;

namespace BankSiteWebsite.Models
{
    public class DasdhboardHomeModel
    {
        public ReadOnlyCollection<Invoice> Invoices { get; set; }
        public ReadOnlyCollection<SalesReceipt> SalesReceipts { get; set; }
        public ReadOnlyCollection<Customer> Customers { get; set; }
        public DateTime Date { get; set; }
    }
}