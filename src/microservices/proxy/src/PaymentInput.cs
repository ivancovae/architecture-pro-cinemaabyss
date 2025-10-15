using System;
using System.ComponentModel.DataAnnotations;

namespace proxy
{
    public class PaymentInput
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public float amount { get; set; }
        public string timestamp { get; set; }
    }
}
