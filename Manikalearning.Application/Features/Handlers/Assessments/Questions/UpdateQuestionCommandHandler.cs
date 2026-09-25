using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Questions
{
    public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Question> _questionRepo;
        private readonly IRepository<QuestionOption> _optionRepo;

        public UpdateQuestionCommandHandler(IRepository<Question> questionRepo, IRepository<QuestionOption> optionRepo)
        {
            _questionRepo = questionRepo;
            _optionRepo = optionRepo;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var question = await _questionRepo.Entity()
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId, cancellationToken);

            if (question == null)
                return ResponseDto<bool>.Failure("Question not found.");

            question.SubjectId = request.SubjectId;
            question.SectionId = request.SectionId;
            question.TopicId = request.TopicId;
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
            var existingOptions = question.Options.ToList();
            foreach (var opt in existingOptions)
            {
                _optionRepo.Remove(opt);
            }
            question.Options.Clear(); 

            int order = 1;
            foreach (var opt in request.Options)
            {
                question.Options.Add(new QuestionOption
                {
                    OptionId = opt.OptionId != Guid.Empty ? opt.OptionId : Guid.NewGuid(),
                    QuestionId = question.QuestionId,
                    OptionLabel = opt.OptionLabel,
                    OptionText = opt.OptionText,
                    OptionTextHindi = opt.OptionTextHindi ?? "",
                    IsCorrect = opt.IsCorrect,
                    DisplayOrder = order++
                });
            }

            _questionRepo.Update(question);
            await _questionRepo.SaveChangesAsync();

            return ResponseDto<bool>.Success(true, "Question updated successfully.");
        }
    }
}
