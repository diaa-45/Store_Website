using STORE_Website.Models;

namespace STORE_Website.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetOne(string id);
        Task<List<ApplicationUser>> GetAll();
        Task<ApplicationUser> Create(ApplicationUser entity);
        Task<ApplicationUser> Update(ApplicationUser entity);
        void Delete(ApplicationUser entity);
        void Save();
    }
}
