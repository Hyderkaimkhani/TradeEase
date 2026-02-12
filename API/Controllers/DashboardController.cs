using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IAdminService adminService;

        public DashboardController(IAdminService adminService)
        {
            this.adminService = adminService;
        }
    }
}
