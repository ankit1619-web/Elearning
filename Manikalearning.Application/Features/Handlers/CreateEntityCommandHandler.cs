using Manikalearning.Application.Features.Commands;
using Manikalearning.Application.Interface;
using MediatR;

namespace Manikalearning.Application.Features.Handlers
{
    public class CreateEntityCommandHandler<TEntity> : IRequestHandler<CreateEntityCommand<TEntity>, Guid> where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;

        public CreateEntityCommandHandler(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        async Task<Guid> IRequestHandler<CreateEntityCommand<TEntity>, Guid>.Handle(CreateEntityCommand<TEntity> request, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(request.Entity);
            await _repository.SaveChangesAsync(); 
            var property = typeof(TEntity).GetProperty($"{typeof(TEntity).Name}Id");
            return property != null ? (Guid)property.GetValue(request.Entity)! : Guid.Empty;
        }
    }
}
