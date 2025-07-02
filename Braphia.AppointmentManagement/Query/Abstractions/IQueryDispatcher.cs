namespace Braphia.AppointmentManagement.Query.Abstractions
{
    public interface IQueryDispatcher
    {
        Task<TModel> ExecuteAsync<TModel>(IQuery<TModel> query);
    }
}
