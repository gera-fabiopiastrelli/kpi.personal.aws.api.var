using System;
using System.Threading.Tasks;

using kpi.personal.aws.api.var.ApiClient.RequestModel;
using kpi.personal.aws.api.var.ApiClient.ResponseModel;

namespace kpi.personal.aws.api.var.ApiClient
{
    public class KpiEventApi : BaseApiClient
    {
        private KpiEventApi()
        {
        }

        public static async Task<ApiResponse<KpiEventResponse>> PostKpiEventAsync(int representativeCode, int kpiCode, KpiEventRequest kpiEvent)
        {
            return await PostAsync<KpiEventResponse, KpiEventRequest>(string.Format("sellers/{0}/kpis/{1}/event", representativeCode, kpiCode), kpiEvent);
        }
    }
}
