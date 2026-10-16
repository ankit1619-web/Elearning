using Manikalearning.Application.Features.Handlers.Teacher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{ 
    [Route("Teacher/Lookup")]
    public class LookupController : TeacherBaseController
    {
        public LookupController(IMediator mediator) : base(mediator) { }

        [HttpGet("{kind}")]
        public async Task<IActionResult> Get(LookupKind kind, Guid? parentId, string? search)
        {
            var items = await Mediator.Send(new GetTeacherLookupQuery(kind, parentId, search));
            return Json(items.Select(i => new { id = i.Id, name = i.Name }));
        }
    }
}
