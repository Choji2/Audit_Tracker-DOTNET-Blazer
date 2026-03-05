using Authentication;
using Audit_Tracker_Blazor.Data.INV_DB;
using Microsoft.EntityFrameworkCore;


namespace Audit_Tracker_Blazor.Services.DB_Services
{
    public class Admin_Services
    {
        public IDbContextFactory<AuthenticationContext> _dbcontext;
        public ILogger<AuthenticationContext> _logger;

        public Admin_Services(IDbContextFactory<AuthenticationContext> dbcontext, ILogger<AuthenticationContext> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        #region GET

        public async Task<List<Audit_Admins>> GetAllUsers()
        {
            var context = await _dbcontext.CreateDbContextAsync();

            return await context.Admins.ToListAsync();
        }

        public async Task<Audit_Admins> GetAdmin(string username)
        {
            var context = await _dbcontext.CreateDbContextAsync();
            var result = await context.Admins.FirstOrDefaultAsync(x => x.Username == username);
            return result;
        }



        public async Task<List<Roles>> GetAllRoles()
        {
            var context = await _dbcontext.CreateDbContextAsync();
            return await context.Roles.ToListAsync();
        }

        public async Task<Roles> GetRole(int ID)
        {
            var context = await _dbcontext.CreateDbContextAsync();
            return await context.Roles.FirstOrDefaultAsync(x => x.ID == ID);
        }

        #endregion


        #region Update


        public async Task UpdateUser(Audit_Admins new_user)
        {
            var old_user = await GetAdmin(new_user.Name);

            var context = await _dbcontext.CreateDbContextAsync();

            old_user.Name = new_user.Name;
            old_user.RoleID = new_user.RoleID;

            context.Admins.Update(old_user);
            context.SaveChanges();
        }
        #endregion


        #region Delete User
        public async Task DeleteUser(string username)
        {
            var user = await GetAdmin(username);
            var context = await _dbcontext.CreateDbContextAsync();
            context.Admins.Remove(user);
            context.SaveChanges();
        }
        #endregion


        #region Add Admin
        public async Task CreateAdmin(Audit_Admins new_user)
        {
            var context = await _dbcontext.CreateDbContextAsync();
            context.Admins.Add(new_user);
            context.SaveChanges();

        }
        #endregion

    }
}
