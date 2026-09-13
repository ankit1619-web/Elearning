using Manikalearning.Application.Features.Commands.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class PYQController : BaseController
    {
        public PYQController(IMediator mediator)
        : base(mediator)
        {
        }
        public IActionResult Index()
        {
            return View();
        }
         
        [HttpGet]
        public async Task<IActionResult> StartPYQTest(Guid? paperId, Guid? sectionId, Guid? topicId)
        { 
            var query = new GetFilteredPYQsQuery(paperId, sectionId, topicId);
            var testSession = await _mediator.Send(query);

            if (testSession == null || !testSession.Questions.Any())
            {
                return NotFound("No PYQs available for this selection.");
            } 
            return View("PYQTestInterface", testSession);
        }
        public IActionResult Reading()
        {
            return View();
        }
        public IActionResult Practice()
        {
            return View();
        }
        public IActionResult Explanation()
        {
            return View();
        }
    }
}
