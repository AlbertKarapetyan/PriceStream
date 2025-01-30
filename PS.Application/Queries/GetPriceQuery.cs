using MediatR;

namespace PS.Application.Queries
{
    public record GetPriceQuery(string instrument) : IRequest<decimal>;
}
