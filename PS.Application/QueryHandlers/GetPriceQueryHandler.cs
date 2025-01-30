using MediatR;
using PS.Application.Queries;
using PS.Domain.Interfaces;

namespace PS.Application.QueryHandlers
{
    internal class GetPriceQueryHandler : IRequestHandler<GetPriceQuery, decimal>
    {
        private readonly IInstrumentService _instrumentService;

        public GetPriceQueryHandler(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        public Task<decimal> Handle(GetPriceQuery request, CancellationToken cancellationToken)
        {
            var result = Task.FromResult(_instrumentService.GetPrice(request.instrument));
            return result;
        }
    }
}
