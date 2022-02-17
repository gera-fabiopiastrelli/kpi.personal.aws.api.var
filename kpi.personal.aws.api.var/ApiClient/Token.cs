using System;

namespace kpi.personal.aws.api.var.ApiClient
{
    public class Token
    {
        public string Value { get; set; }

        public DateTime ExpiresAtUtc { get; set; }
    }
}
