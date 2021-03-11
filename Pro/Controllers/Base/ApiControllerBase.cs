using Microsoft.AspNetCore.Mvc;

namespace Pro
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiControllerBase
    {
    }
}
//  Scaffold-DbContext "Server=.;Database=Pro;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB 
//  Scaffold-DbContext "Server=.;Database=Pro;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB -context GameDBContext -Project Pro -force
