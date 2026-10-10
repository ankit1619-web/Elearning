using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Questions
{
    public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Question> _questionRepo;
        // NOTE: IRepository<QuestionOption> is completely removed to prevent DbContext instance conflicts!

        public UpdateQuestionCommandHandler(IRepository<Question> questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || request.QuestionId == Guid.Empty)
                    return ResponseDto<bool>.Failure("Invalid request payload.");

                // 1. Fetch Question WITH Options (Tracked by single DbContext)
                var question = await _questionRepo.Entity()
                    .Include(q => q.Options)
                    .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId, cancellationToken);

                if (question == null)
                    return ResponseDto<bool>.Failure("Question not found in database.");

                // 2. Map Scalar Properties
                question.SubjectId = request.SubjectId == Guid.Empty ? null : request.SubjectId;
                question.SectionId = request.SectionId == Guid.Empty ? null : request.SectionId;
                question.TopicId = request.TopicId == Guid.Empty ? null : request.TopicId;
                question.SubTopicId = request.SubTopicId == Guid.Empty ? null : request.SubTopicId;
                question.ExamId = request.ExamId == Guid.Empty ? null : request.ExamId;
                question.PaperId = request.PaperId == Guid.Empty ? null : request.PaperId;
                question.PassageId = request.PassageId == Guid.Empty ? null : request.PassageId;

                question.QuestionType = request.QuestionType;
                question.QuestionText = request.QuestionText;
                question.QuestionTextHindi = request.QuestionTextHindi;
                question.Explanation = request.Explanation;
                question.ExplanationHindi = request.ExplanationHindi;
                question.ShortExplanation = request.ShortExplanation;
                question.ShortExplanationHindi = request.ShortExplanationHindi;
                question.Difficulty = request.Difficulty;
                question.Marks = request.Marks;
                question.NegativeMarks = request.NegativeMarks;
                question.IsPYQ = request.IsPYQ;
                question.PYQYear = request.IsPYQ ? request.PYQYear : null;
                question.PYQSession = request.IsPYQ ? request.PYQSession : null;
                question.SourceName = request.SourceName;
                question.IsApproved = request.IsApproved;
                question.IsActive = request.IsActive;
                question.UpdatedAt = DateTime.UtcNow;

                // Handle Radio Button Selection Index
                var incomingOptions = request.Options ?? new List<QuestionOptionDto>();
                if (request.CorrectOptionIndex.HasValue && request.CorrectOptionIndex.Value < incomingOptions.Count)
                {
                    for (int i = 0; i < incomingOptions.Count; i++)
                    {
                        incomingOptions[i].IsCorrect = (i == request.CorrectOptionIndex.Value);
                    }
                }

                // 3. Native Aggregate Collection Synchronization (No separate repositories!)
                var incomingOptionIds = incomingOptions
                    .Where(o => o.OptionId != Guid.Empty)
                    .Select(o => o.OptionId)
                    .ToHashSet();

                // A. Remove options that were deleted in the UI form
                var optionsToRemove = question.Options
                    .Where(o => !incomingOptionIds.Contains(o.OptionId))
                    .ToList();

                foreach (var optToRemove in optionsToRemove)
                {
                    question.Options.Remove(optToRemove); // EF Core tracks this as a DELETE operation automatically
                }

                // B. Update existing options or Add new ones
                int order = 1;
                string[] defaultLabels = { "A", "B", "C", "D", "E", "F", "G", "H" };

                foreach (var optDto in incomingOptions)
                {
                    string label = !string.IsNullOrWhiteSpace(optDto.OptionLabel)
                        ? optDto.OptionLabel
                        : (order <= defaultLabels.Length ? defaultLabels[order - 1] : order.ToString());

                    var existingOption = question.Options
                        .FirstOrDefault(o => o.OptionId != Guid.Empty && o.OptionId == optDto.OptionId);

                    if (existingOption != null)
                    {
                        // Update existing option in-place
                        existingOption.OptionLabel = label;
                        existingOption.OptionText = optDto.OptionText ?? "";
                        existingOption.OptionTextHindi = optDto.OptionTextHindi;
                        existingOption.IsCorrect = optDto.IsCorrect;
                        existingOption.DisplayOrder = order++;
                    }
                    else
                    {
                        // Add brand new option to collection
                        question.Options.Add(new QuestionOption
                        {
                            OptionId = Guid.NewGuid(),
                            QuestionId = question.QuestionId,
                            OptionLabel = label,
                            OptionText = optDto.OptionText ?? "",
                            OptionTextHindi = optDto.OptionTextHindi,
                            IsCorrect = optDto.IsCorrect,
                            DisplayOrder = order++
                        });
                    }
                }

                // 4. Single Unified Save (Managed entirely by Question Aggregate Root)
                await _questionRepo.SaveChangesAsync( );

                return ResponseDto<bool>.Success(true, "Question updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return ResponseDto<bool>.Failure($"Concurrency Error: {ex.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                return ResponseDto<bool>.Failure($"Database Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Unexpected Error: {ex.Message}");
            }
        }
    }
}