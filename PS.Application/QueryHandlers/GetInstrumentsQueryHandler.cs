using MediatR;
using PS.Application.Queries;
using PS.Domain.Interfaces;

namespace PS.Application.QueryHandlers
{
    internal class GetInstrumentsQueryHandler : IRequestHandler<GetInstrumentsQuery, List<string>>
    {
        private readonly IInstrumentService _instrumentService;

        public GetInstrumentsQueryHandler(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }
        public Task<List<string>> Handle(GetInstrumentsQuery request, CancellationToken cancellationToken)
        {
            var result = Task.FromResult(_instrumentService.GetInstruments());
            return result;
        }
    }
}
