using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using STORE_Website.Data;
using STORE_Website.Models;

namespace STORE_Website.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<ApplicationUser> Create(ApplicationUser entity)
        {
            context.Users.Add(entity);
            Save();
            return entity;
        }
        public void Delete(ApplicationUser entity)
        {
            context.Users.Remove(entity);
        }
        public async Task<List<ApplicationUser>> GetAll()
        {
            List<ApplicationUser> users = userManager.Users.ToList();
            return users;
        }
        public async Task<ApplicationUser> GetOne(string id)
        {
            return userManager.Users.FirstOrDefault(u=> u.Id==id);
        }
        public async Task<ApplicationUser> Update(ApplicationUser entity)
        {
            context.Users.Update(entity);
            Save();
            return entity;
        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
