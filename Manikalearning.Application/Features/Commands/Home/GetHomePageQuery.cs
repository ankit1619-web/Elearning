using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Home
{
    public class GetHomePageQuery : IRequest<HomeDto>
    {
    }
}
