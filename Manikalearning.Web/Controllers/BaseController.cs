using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public abstract class BaseController : Controller
    {
        protected readonly IMediator _mediator;

        protected BaseController(IMediator mediator)
        {
            _mediator = mediator
                ?? throw new ArgumentNullException(nameof(mediator));
        }
    }
}
