using Main_SD;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;

namespace Audit_Tracker_Blazor.Services.Authentication;

public class Authorize_Policy
{
    public static void AddCustomPolicies(AuthorizationOptions options)
    {
        options.AddPolicy($"{SD.Master}", policy =>
            policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == ClaimTypes.Role && 
                    SD.Master.Equals(c.Value.ToString()))
                    ));

        options.AddPolicy($"{SD.Auditor}", policy =>
            policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == ClaimTypes.Role &&
                    SD.Auditor.Equals(c.Value.ToString()))
                    ));
    }

}


   
