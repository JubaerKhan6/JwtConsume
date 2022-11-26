using JwtConsume.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtConsume.Service
{
    public interface IDashboardService
    {
        public Task<List<UserInfo>> GetUsers();
    }
    
}
