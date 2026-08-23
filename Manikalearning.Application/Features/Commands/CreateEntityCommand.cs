using MediatR;

namespace Manikalearning.Application.Features.Commands
{
    public record CreateEntityCommand<TEntity>(TEntity Entity) : IRequest<Guid> where TEntity : class;
}
