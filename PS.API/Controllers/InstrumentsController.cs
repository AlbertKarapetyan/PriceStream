using MediatR;
using Microsoft.AspNetCore.Mvc;
using PS.Application.Queries;

namespace PS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstrumentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstrumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetInstruments()
        {
            try
            {
                var instruments = await _mediator.Send(new GetInstrumentsQuery());
                return Ok(instruments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{instrument}/price")]
        public async Task<IActionResult> GetPrice(string instrument)
        {
            try
            {
                var price = await _mediator.Send(new GetPriceQuery(instrument));

                return Ok(new { Instrument = instrument, Price = price });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
