using System;

using kpi.personal.aws.api.var.Events;
using kpi.personal.aws.api.var.ApiClient.RequestModel;

namespace kpi.personal.aws.api.var.Services
{
    public class BaseService
    {
        // kpis codes
        protected const int IndicadorPessoalPedidos = 1000;
        protected const int IndicadorPessoalFaturamento = 1001;
        protected const int IndicadorPessoalPontos = 1002;

        static BaseService()
        {
        }

        protected static KpiEventRequest CreateKpiEventRequest(BaseEventArgs eventArgs)
        {
            return new KpiEventRequest()
            {
                EventGuid = eventArgs.Jti,
                EventType = eventArgs.Sub,
                EventDate = eventArgs.Iat
            };
        }
    }
}
