using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    internal class Teacherprofile
    {
    }
    public class TeacherProfileDto
    {
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string? Phone { get; set; }             
        public int Courses { get; set; }
        public int Students { get; set; }
        public int Questions { get; set; }
        public int Tests { get; set; }
    }

    public class TeacherProfileForm
    {
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }              
    }

    public record GetTeacherProfileQuery(Guid TeacherId) : IRequest<TeacherProfileDto?>;
    public record UpdateTeacherProfileCommand(Guid TeacherId, TeacherProfileForm Form) : IRequest<TeacherActionResult>;

    public class GetTeacherProfileQueryHandler : IRequestHandler<GetTeacherProfileQuery, TeacherProfileDto?>
    {
        private readonly IRepository<TUser> _users;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TStudentCourse> _sc;
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TTest> _tests;

        public GetTeacherProfileQueryHandler(IRepository<TUser> u, IRepository<TCourse> c, IRepository<TStudentCourse> sc,
            IRepository<TQuestion> q, IRepository<TTest> t)
        { _users = u; _courses = c; _sc = sc; _questions = q; _tests = t; }

        public async Task<TeacherProfileDto?> Handle(GetTeacherProfileQuery r, CancellationToken ct)
        {
            var u = await _users.Entity().AsNoTracking().Where(x => x.Id == r.TeacherId)
                .Select(x => new { x.FullName, x.Email, x.PhoneNumber }).FirstOrDefaultAsync(ct);    
            if (u == null) return null;

            var students = await (from sc in _sc.Entity().AsNoTracking()
                                  from c in _courses.Entity().AsNoTracking()
                                  where sc.CourseId == c.CourseId && c.CreatedBy == r.TeacherId && sc.IsActive
                                  select sc.UserId).Distinct().CountAsync(ct);

            return new TeacherProfileDto
            {
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.PhoneNumber,                                                            
                Courses = await _courses.Entity().AsNoTracking().CountAsync(c => c.CreatedBy == r.TeacherId, ct),
                Students = students,
                Questions = await _questions.Entity().AsNoTracking().CountAsync(q => q.CreatedBy == r.TeacherId, ct),
                Tests = await _tests.Entity().AsNoTracking().CountAsync(t => t.CreatedBy == r.TeacherId, ct)
            };
        }
    }

    public class UpdateTeacherProfileCommandHandler : IRequestHandler<UpdateTeacherProfileCommand, TeacherActionResult>
    {
        private readonly IRepository<TUser> _users;
        public UpdateTeacherProfileCommandHandler(IRepository<TUser> u) { _users = u; }

        public async Task<TeacherActionResult> Handle(UpdateTeacherProfileCommand r, CancellationToken ct)
        {
            var name = (r.Form.FullName ?? "").Trim();
            if (name.Length < 2) return TeacherActionResult.Fail("Enter your full name (at least 2 characters).");
            if (name.Length > 100) return TeacherActionResult.Fail("Name must be 100 characters or fewer.");

            var phone = string.IsNullOrWhiteSpace(r.Form.Phone) ? null : r.Form.Phone.Trim();     
            if (phone != null && (phone.Length > 20 || phone.Any(ch => !(char.IsDigit(ch) || ch is '+' or ' ' or '-'))))
                return TeacherActionResult.Fail("Enter a valid phone number.");                  

            var user = await _users.Entity().FirstOrDefaultAsync(x => x.Id == r.TeacherId, ct);
            if (user == null) return TeacherActionResult.Fail("Profile not found.");

            user.FullName = name;
            user.PhoneNumber = phone;                                                              
            try { await _users.UpdateRow(user); }
            catch (DbUpdateException) { return TeacherActionResult.Fail("Could not save your profile. Please try again."); }
            return TeacherActionResult.Ok("Profile updated.", r.TeacherId);
        }
    }
}
