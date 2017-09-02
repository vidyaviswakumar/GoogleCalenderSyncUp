using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GoogleCalenderSyncUp.Models
{
    public class CredentialModel
    {
    
    //     Gets the user identity.
        public string UserId { get; }

        public string UserName { get; set; }

        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public long? ExpiresInSeconds { get; set; }

        public string RefreshToken { get; set; }

        public string Scope { get; set; }

        public string IdToken { get; set; }

        public DateTime Issued { get; set; }

        public DateTime IssuedUtc { get; set; }

        DateTime Now { get; }

        DateTime UtcNow { get; }

    }

    public class CredentialDbContext : DbContext
    {

        public CredentialDbContext() : base("CredentialResponse")
        {
        }

        public DbSet<CredentialModel> responses { get; set; }
    }
}