using MediatR;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;

public class GetAppointmentsOfTodayQuery : IRequest<IEnumerable<AppointmentViewQueryModel>> { }
