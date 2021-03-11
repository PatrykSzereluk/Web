using Pro.Models.Enums;
using Pro.Services.Identity;

namespace Pro.Models
{
    public class BaseResponseModel : BaseModel
    {
        public ResponseStatusCode StatusCode { get; set; }

    }
}
