
namespace STORE_Website.Services
{
    public interface IReposirory<T> where T : class
    {
        Task<T> GetOne(int id);
        List<T> GetAll();
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        void Delete(T entity);
        void Save();
    }
}
