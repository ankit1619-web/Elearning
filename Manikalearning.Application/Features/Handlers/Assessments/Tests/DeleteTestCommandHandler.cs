using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class DeleteTestCommandHandler : IRequestHandler<DeleteTestCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Test> _testRepo;
        private readonly ILogger<DeleteTestCommandHandler> _logger;

        public DeleteTestCommandHandler(
            IRepository<Test> testRepo,
            ILogger<DeleteTestCommandHandler> logger)
        {
            _testRepo = testRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteTestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var test = await _testRepo.GetByIdAsync(request.Id);
                if (test == null)
                {
                    _logger.LogWarning("Delete failed. Test with ID {Id} not found.", request.Id);
                    return ResponseDto<bool>.Failure("Test not found.");
                }

                  _testRepo.Remove(test);
                await _testRepo.SaveChangesAsync();

                _logger.LogInformation("Test with ID {Id} deleted successfully.", request.Id);
                return ResponseDto<bool>.Success(true, "Test deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting Test with ID {Id}.", request.Id);
                return ResponseDto<bool>.Failure("An error occurred while deleting Test.");
            }
        }
    }
}
