using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Sections
{

    public record GetSectionsLookupQuery() : IRequest<ResponseDto<List<LookupDto>>>;
}
