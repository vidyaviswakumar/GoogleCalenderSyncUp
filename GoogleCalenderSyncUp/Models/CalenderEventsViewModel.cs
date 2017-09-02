
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;


namespace GoogleCalenderSyncUp.Models
{
    public class CalenderEventsViewModel
    {
        [Key]
        public int EventId { get; set; }
        public string UserName { get; set; }
        public string AccessRole { get; set; }
        public string Description { get; set; }
        public string ETag { get; set; }
        public string Kind { get; set; }
        public string NextPageToken { get; set; }
        public string NextSyncToken { get; set; }
        public string Summary { get; set; }
        public string TimeZone { get; set; }
        public string UpdatedRaw { get; set; }
        public DateTime? Updated { get; set; }

    }
    public class GoogleCalenderEventsDbContext : DbContext
    {
        public GoogleCalenderEventsDbContext() : base("GoogleCalenderEvents")
{
        }

        public DbSet<CalenderEventsViewModel> events { get; set; }
    }
}