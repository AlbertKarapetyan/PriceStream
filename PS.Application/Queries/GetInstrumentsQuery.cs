using MediatR;

namespace PS.Application.Queries
{
    public record GetInstrumentsQuery : IRequest<List<string>>;
}
