using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class GetTestsQueryHandler : IRequestHandler<GetTestsQuery, ResponseDto<List<TestDto>>>
    {
        private readonly IRepository<Test> _testRepo;
        private readonly ILogger<GetTestsQueryHandler> _logger;

        public GetTestsQueryHandler(
            IRepository<Test> testRepo,
            ILogger<GetTestsQueryHandler> logger)
        {
            _testRepo = testRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<List<TestDto>>> Handle(GetTestsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _testRepo.Entity().AsNoTracking();

                if (request.SubjectId.HasValue && request.SubjectId.Value != Guid.Empty)
                {
                    query = query.Where(t => t.SubjectId == request.SubjectId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(t => t.TestName.Contains(request.Search));
                }

                var tests = query.Select(t => new TestDto
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    ExamId = t.ExamId,
                    PaperId = t.PaperId,
                    SubjectId = t.SubjectId,
                   // SubjectName = t.Subject != null ? t.Subject.Name : null,
                    SectionId = t.SectionId,
                   // SectionName = t.Section != null ? t.Section.Name : null,
                    TopicId = t.TopicId,
                   // TopicName = t.Topic != null ? t.Topic.Name : null,
                    TestType = t.TestType,
                    DurationMinutes = t.DurationMinutes,
                    TotalQuestions = t.TotalQuestions,
                    TotalMarks = t.TotalMarks,
                    NegativeMarking = t.NegativeMarking,
                    IsRandom = t.IsRandom,
                    ShowResultImmediately = t.ShowResultImmediately,
                    IsPublished = t.IsPublished,
                    CreatedAt = t.CreatedAt
                }).ToList();

                _logger.LogInformation("Successfully fetched {Count} tests.", tests.Count);
                return ResponseDto<List<TestDto>>.Success(tests, "Tests retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tests list.");
                return ResponseDto<List<TestDto>>.Failure("Failed to retrieve tests list.");
            }
        }
    }
}
