using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Questions
{
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Question> _questionRepo;

        private readonly IRepository<QuestionOption> _optionRepo;

        public DeleteQuestionCommandHandler(IRepository<Question> questionRepo, IRepository<QuestionOption> optionRepo)
        {
            _questionRepo = questionRepo;
            _optionRepo = optionRepo;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var question = await _questionRepo.Entity().FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId, cancellationToken);
            if (question == null)
                return ResponseDto<bool>.Failure("Question not found.");
            var existingOptions = question.Options.ToList();
            foreach (var opt in existingOptions)
            {
                _optionRepo.Remove(opt);
            }
            question.Options.Clear(); 

            _questionRepo.Remove(question);
            await _questionRepo.SaveChangesAsync();

            return ResponseDto<bool>.Success(true, "Question deleted successfully.");
        }
    }
}
