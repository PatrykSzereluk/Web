namespace Pro
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public abstract class ApiControllerBase
    {
    }
}
//  Scaffold-DbContext "Server=.;Database=Pro;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB 
//  Scaffold-DbContext "Server=.;Database=Pro;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/DB -context ProContext -Project Pro -force
