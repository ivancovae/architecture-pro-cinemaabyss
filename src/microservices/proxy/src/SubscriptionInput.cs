using System;
using System.ComponentModel.DataAnnotations;

namespace proxy
{
    public class SubscriptionInput
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string plan_type { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}
