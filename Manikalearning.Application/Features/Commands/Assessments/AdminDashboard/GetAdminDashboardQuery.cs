using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.AdminDashboard
{
    public record GetAdminDashboardQuery() : IRequest<AdminDashboardViewModel>;
}
