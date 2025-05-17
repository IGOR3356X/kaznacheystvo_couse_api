namespace KaznacheystvoCalendar.Interfaces;

public interface IGenericRepository<T> where T : class
{
    public Task<List<T>> GetAllAsync();
    public Task<T?> GetByIdAsync(int id);
    public Task<T> CreateAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(T entity);
    public IQueryable<T> GetQueryable();
    public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    public Task<bool> DeleteRangeAsync(IEnumerable<T> entities);
}