using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    public static class TeacherRoles
    { 
        public const string Teacher = "Teacher";
    }

     
    [Area("Teacher")]
    [Authorize(Roles = TeacherRoles.Teacher)]
    public abstract class TeacherBaseController : Controller
    {
        protected readonly IMediator Mediator;
        protected TeacherBaseController(IMediator mediator) { Mediator = mediator; }

        protected Guid TeacherId =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                ? id
                : throw new UnauthorizedAccessException("Teacher id claim missing.");
    }
}
