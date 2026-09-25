using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class CreateTestCommandHandler : IRequestHandler<CreateTestCommand, ResponseDto<Guid>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Test> _testRepo;
        private readonly ILogger<CreateTestCommandHandler> _logger;

        public CreateTestCommandHandler(
            IRepository<Test> testRepo, IHttpContextAccessor httpContextAccessor,
            ILogger<CreateTestCommandHandler> logger)
        {
            _testRepo = testRepo; 
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateTestCommand request, CancellationToken cancellationToken)
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
             
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("User   not authenticated or user ID is invalid.");
            }
            try
            {
                var entity = new Test
                {
                    TestId = Guid.NewGuid(),
                    TestName = request.TestName,
                    ExamId = request.ExamId,
                    PaperId = request.PaperId,
                    SubjectId = request.SubjectId,
                    SectionId = request.SectionId,
                    TopicId = request.TopicId,
                    TestType = request.TestType,
                    DurationMinutes = request.DurationMinutes,
                    TotalQuestions = request.TotalQuestions,
                    TotalMarks = request.TotalMarks,
                    NegativeMarking = request.NegativeMarking,
                    IsRandom = request.IsRandom,
                    ShowResultImmediately = request.ShowResultImmediately,
                    IsPublished = request.IsPublished,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                }; 
                await _testRepo.AddAsync(entity);
                await _testRepo.SaveChangesAsync();

                _logger.LogInformation("Test {Name} created successfully with ID {Id}.", entity.TestName, entity.TestId);
                return ResponseDto<Guid>.Success(entity.TestId, "Test created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating Test {Name}.", request.TestName);
                return ResponseDto<Guid>.Failure("An error occurred while saving Test.");
            }
        }
    }
}
