using ASOFT.Core.API.Httpss.ActionResults;
using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Https;
using Microsoft.AspNetCore.Mvc;

namespace ASOFT.Core.API.Controllers
{
    /// <summary>
    /// Base controller của ASOFT.
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    public abstract class ASOFTBaseController : ControllerBase
    {
 
        [NonAction]
        public virtual ObjectResult ASOFTObject(object value, bool error)
        {
            if (error)
            {
                return new ErrorObjectResultV2(new ErrorResponse(value));
            }
            else
            {
                return new SuccessObjectResultV2<object>(value);
            }
        }
            

        [NonAction]
        public virtual ObjectResult ASOFTStatus(bool success)
        {
            if (success)
            {
                return new SuccessObjectResultV2<bool>(success);
            }
            else
            {
                return new ErrorObjectResultV2(new ErrorResponse(success));
            }
        }

        [NonAction]
        public virtual ObjectResult ASOFTSuccess(object value)
        {
            return new SuccessObjectResultV2<object>(value);
        }

        [NonAction]
        public virtual ObjectResult ASOFTError(object errorValue)
        {
            return new ErrorObjectResultV2(new ErrorResponse(errorValue));
        }

        [NonAction]
        public virtual ObjectResult ASOFTError(object errorValue, int errorStatusCode)
        {
            return new ErrorObjectResultV2(new ErrorResponse(errorValue), errorStatusCode);
        }

        [NonAction]
        public virtual ObjectResult ASOFTForbidden(object errorValue)
        {
            return new ErrorObjectResultV2(new ErrorResponse(errorValue), ApiStatusCodes.Forbidden403);
        }
    }
}