﻿using Intuit.Ipp.OAuth2PlatformClient;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using BankSiteQBOData;
using BankSiteWebsite.Models;
using IdentityModel;

namespace BankSiteWebsite.Controllers
{
    public class DashboardController : Controller
    {
        public static string clientid = ConfigurationManager.AppSettings["clientid"];
        public static string clientsecret = ConfigurationManager.AppSettings["clientsecret"];
        public static string redirectUrl = ConfigurationManager.AppSettings["redirectUrl"];
        public static string environment = ConfigurationManager.AppSettings["appEnvironment"];

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);

        /// <summary>
        /// Use the Index page of App controller to get all endpoints from discovery url
        /// </summary>
        public ActionResult Index()
        {
           ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (Session["realmId"] != null)
            {
                var realmId = Session["realmId"].ToString();
                var principal = User as ClaimsPrincipal;
                var oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);

                // Create a ServiceContext with Auth tokens and realmId
                var serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            }
            else
            {
                Session.Clear();
                Session.Abandon();
                Request.GetOwinContext().Authentication.SignOut("Cookies");
                return View();
            }

            return RedirectToAction("Home");
        }

        /// <summary>
        /// Start Auth flow
        /// </summary>
        public ActionResult InitiateAuth(string submitButton)
        {
            switch (submitButton)
            {
                case "Connect to QuickBooks":
                    List<OidcScopes> scopes = new List<OidcScopes>();
                    scopes.Add(OidcScopes.Accounting);
                    string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
                    return Redirect(authorizeUrl);
                default:
                    return (View());
            }
        }

        ///// <summary>
        ///// QBO API Request
        ///// </summary>
        //public ActionResult ApiCallService()
        //{
        //    if (Session["realmId"] != null)
        //    {
        //        string realmId = Session["realmId"].ToString();
        //        try
        //        {
        //            var principal = User as ClaimsPrincipal;
        //            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);

        //            // Create a ServiceContext with Auth tokens and realmId
        //            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
        //            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";

        //            // Create a QuickBooks QueryService using ServiceContext
        //            QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);
        //            CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();

        //            string output = "Company Name: " + companyInfo.CompanyName + " Company Address: " + companyInfo.CompanyAddr.Line1 + ", " + companyInfo.CompanyAddr.City + ", " + companyInfo.CompanyAddr.Country + " " + companyInfo.CompanyAddr.PostalCode;
        //            return View("ApiCallService", (object)("QBO API call Successful!! Response: " + output));
        //        }
        //        catch (Exception ex)
        //        {
        //            return View("ApiCallService", (object)("QBO API call Failed!" + " Error message: " + ex.Message));
        //        }
        //    }
        //    else
        //        return View("ApiCallService", (object)"QBO API call Failed!");        
        //}

        private List<Invoice> GetAllInvoices()
        {
            var result = new List<Invoice>();
            if (Session["realmId"] != null)
            {
                string realmId = Session["realmId"].ToString();
                try
                {
                    var principal = User as ClaimsPrincipal;
                    var oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);

                    // Create a ServiceContext with Auth tokens and realmId
                    var serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "23";

                    // Create a QuickBooks QueryService using ServiceContext
                    var querySvc = new QueryService<Invoice>(serviceContext);
                    result = querySvc.ExecuteIdsQuery("SELECT * FROM Invoice").ToList();

                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }

        public ActionResult Home()
        {
            var model = new DasdhboardHomeModel
            {
                Invoices = QBORepository.GetAll<Invoice>(Session["realmId"]?.ToString(), User as ClaimsPrincipal),
                SalesReceipts = QBORepository.GetAll<SalesReceipt>(Session["realmId"]?.ToString(), User as ClaimsPrincipal),
                Customers = QBORepository.GetAll<Customer>(Session["realmId"]?.ToString(), User as ClaimsPrincipal),
                Date = DateTime.Now
            };
                
                
            return View("Home", model);
        }

        /// <summary>
        /// Use the Index page of App controller to get all endpoints from discovery url
        /// </summary>
        public ActionResult Error()
        {
            return View("Error");
        }


    }
}