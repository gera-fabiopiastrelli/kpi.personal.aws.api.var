using System;
using System.Threading.Tasks;

using kpi.personal.aws.api.var.ApiClient;
using kpi.personal.aws.api.var.ApiClient.RequestModel;
using kpi.personal.aws.api.var.ApiClient.ResponseModel;
using kpi.personal.aws.api.var.Events;

namespace kpi.personal.aws.api.var.Services
{
    public class OrderService : BaseService
    {
        public static async Task CreateKpiEventAsync(OrderEventArgs orderEventArgs, bool cancel)
        {
            // get stracture code
            int representativeCode = orderEventArgs.RepresentativeCode;

            // create kpi event
            KpiEventRequest kpiEvent = CreateKpiEventRequest(orderEventArgs);
            kpiEvent.EventEntityCode = orderEventArgs.OrderNumber;
            kpiEvent.Cycle = (int)(cancel ? orderEventArgs.CancellationCycle : orderEventArgs.ApprovalCycle);

            // send kpi event to update
            await CreateKpiEventAsync(cancel, representativeCode, kpiEvent, IndicadorPessoalPedidos, decimal.One);
            await CreateKpiEventAsync(cancel, representativeCode, kpiEvent, IndicadorPessoalFaturamento, (decimal)orderEventArgs.NetValue);
            await CreateKpiEventAsync(cancel, representativeCode, kpiEvent, IndicadorPessoalPontos, (decimal)orderEventArgs.PointsQuantity);
        }

        private static async Task<ApiResponse<KpiEventResponse>> CreateKpiEventAsync(bool cancel, int representativeCode, KpiEventRequest kpiEvent, int kpiCode, decimal value)
        {
            kpiEvent.AmountRealized = cancel ? -value : value;
            return await KpiEventApi.PostKpiEventAsync(representativeCode, kpiCode, kpiEvent);
        }
    }
}
