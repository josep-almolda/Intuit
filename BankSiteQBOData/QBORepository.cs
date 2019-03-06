using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;

namespace BankSiteQBOData
{
    public static class QBORepository
    {
        public static ReadOnlyCollection<T> GetAll<T>(string realmId, ClaimsPrincipal principal)
        {
            ReadOnlyCollection<T> result = null;
            if (realmId != null)
            {
                try
                {
                    var oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);

                    // Create a ServiceContext with Auth tokens and realmId
                    var serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "23";

                    // Create a QuickBooks QueryService using ServiceContext
                    var querySvc = new QueryService<T>(serviceContext);
                    result = querySvc.ExecuteIdsQuery($"SELECT * FROM {typeof(T).Name}");

                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }

    }
}
