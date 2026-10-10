using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetSubjectsByStudentQueryHandler : IRequestHandler<GetSubjectsByStudentQuery, List<SubjectDto>>
    {
        private readonly IRepository<Course> _courseRepo; 
        private readonly IRepository<ExamCourse> _examCourseRepository;
        private readonly IRepository<Paper> _paperRepository;
        private readonly IRepository<Subject> _subjectRepo;

        public GetSubjectsByStudentQueryHandler(IRepository<Course> courseRepo,
            IRepository<ExamCourse> examCourseRepository,
            IRepository<Paper> paperRepository,
            IRepository<Subject> subjectRepo)
        {
            _examCourseRepository = examCourseRepository;
            _paperRepository = paperRepository;
            _courseRepo = courseRepo;
            _subjectRepo = subjectRepo;
        }

        public async Task<List<SubjectDto>> Handle(GetSubjectsByStudentQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var query = _courseRepo.Query()
               .Where(c => c.StudentCourses.Any(sc =>
                   sc.UserId == request.UserId &&
                   sc.IsActive &&
                   (sc.ExpiryDate == null || sc.ExpiryDate > now)));
             
             var purchasedCourseIds = query.Select(c => c.CourseId);    
            var purchasedExamIds = _examCourseRepository.Query()
                .Where(ec => purchasedCourseIds.Contains(ec.CourseId))
                .Select(ec => ec.ExamId);
           
            var purchasedPaperIds = _paperRepository.Query()
                .Where(p => purchasedExamIds.Contains(p.ExamId))
                .Select(p => p.PaperId);

            return await _subjectRepo.Query() 
                .Where(s => purchasedPaperIds.Contains(s.PaperId))
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SubjectDto
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName
                })
                .ToListAsync(cancellationToken);
        }
    }
}
