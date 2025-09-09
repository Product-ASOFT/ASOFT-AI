// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy        Tạo mới
// #    24/12/2024      Minh Nhựt       Bổ sung kiểm tra cập nhật đường dẫn khi API host trong môi trường Linux
// ##################################################################

using ASOFT.A00.Entities;
using ASOFT.Core.API.Controllers;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.API.Paging;
using ASOFT.Core.API.Versions;
using ASOFT.Core.Business.Files.Business.Interfaces;
using ASOFT.Core.Business.Files.DataAccess.Interfaces;
using ASOFT.Core.Business.Files.Entities;
using ASOFT.Core.Business.Files.Entities.Requests;
using ASOFT.Core.Business.Files.Entities.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.Common.Security.Identity;
using ASOFT.Core.DataAccess.Enums;
using ASOFT.Data.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ASOFT.Core.Business.Files.API.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [Route("api/v{version:api-version}/Core/Common/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Core")]
    public class FilesController : ASOFTBaseController
    {
        private readonly IFileQueries _fileQueris;
        private readonly IFileBusiness _fileBusiness;
        private readonly INumberSizePagingAdapter _numberSizePagingAdapter;
        private readonly IConfiguration _config;
        private readonly IConfigQueries _configQueries;
        private static long _fileSize = 50000000;
        public FilesController(IFileQueries fileQueris, INumberSizePagingAdapter numberSizePagingAdapter, IConfiguration config,
            IFileBusiness fileBusiness, IConfigQueries configQueries)
        {
            _fileQueris = Checker.NotNull(fileQueris, nameof(fileQueris));
            _numberSizePagingAdapter = Checker.NotNull(numberSizePagingAdapter, nameof(numberSizePagingAdapter));
            _config = Checker.NotNull(config, nameof(config));
            _fileBusiness = Checker.NotNull(fileBusiness, nameof(fileBusiness));
            _configQueries = Checker.NotNull(configQueries, nameof(configQueries));
            long.TryParse(_config["Attach:FileSize"], out _fileSize);
        }

        /// <summary>
        /// API upload files
        /// </summary>
        /// <param name="files"></param>
        /// <param name="divisionID"></param>
        /// <param name="tableID"></param>
        /// <param name="apk"></param>
        /// <param name="viewer"></param>
        /// <param name="mediator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm(Name = "files")] List<IFormFile> files,
            [FromForm(Name = "divisionID")] string divisionID,
            [FromForm(Name = "tableID")] string tableID,
            [FromForm(Name = "apk")] Guid apk,
            [FromServices]IIdentity viewer,
            [FromServices]IMediator mediator,
            CancellationToken cancellationToken = default)
        {
            var command = new UploadFileRequest
            {
                APK = apk,
                DivisionID = divisionID,
                Files = files,
                TableID = tableID,
                UserID = viewer.ID
            };
            var result = await _fileBusiness.UploadFile(command, cancellationToken);
            return ASOFTSuccess(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadFileNoAuth([FromForm(Name = "files")] List<IFormFile> files,
           [FromForm(Name = "divisionID")] string divisionID,
           [FromForm(Name = "tableID")] string tableID,
           [FromForm(Name = "apk")] Guid apk,
           [FromServices]IIdentity viewer,
           [FromServices]IMediator mediator,
           CancellationToken cancellationToken = default)
        {
            var command = new UploadFileRequest
            {
                APK = apk,
                DivisionID = divisionID,
                Files = files,
                TableID = tableID,
                UserID = viewer.ID
            };
            var result = await _fileBusiness.UploadFile(command, cancellationToken);
            return ASOFTSuccess(result);
        }

        /// <summary>
        /// API lấy danh sách đính kèm của phiếu
        /// </summary>
        /// <param name="apk"></param>
        /// <param name="divisionID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAttachList([FromQuery][Required]Guid apk, [FromQuery][Required]string divisionID, [FromQuery][Required]int pageNumber, [FromQuery][Required]int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                var (totalCount, list) = await _fileQueris.GetAttachList(apk, divisionID, pageNumber, pageSize, cancellationToken);
                var pagingModel = _numberSizePagingAdapter.Create(
                    new NumberSizePagingEntity<FileViewModel>(list,
                        totalCount,
                        pageNumber,
                        pageSize));
                return ASOFTSuccess(pagingModel);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Download([FromBody][Required]DownloadFileRequest command, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_FILES).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_FILES);
            }

            var filePath = Path.Combine(target, $"{command.APK.ToString()}{Path.GetExtension(command.FileName)}");
            string mimeType = MimeTypes.GetMimeType(command.FileName);

            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, command.FileName);
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }


        /// <summary>
        /// Lấy file đính kèm
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ActionName("getFile")]
        [HttpGet("{divisionID}/{fileName}")]
        public async Task<IActionResult> ViewImage([FromRoute][Required]string divisionID, [FromRoute][Required]string fileName, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_FILES).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_FILES);
            }
            var filePath = Path.Combine(target, $"{fileName}");
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        /// <summary>
        /// Lấy file đính kèm
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ActionName("getEContractFile")]
        [HttpGet("{divisionID}/{fileName}")]
        public async Task<IActionResult> gGetEContractFile([FromRoute][Required]string divisionID, [FromRoute][Required]string fileName, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + @"\Attached\EContract\").Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + @"\Attached\EContract\");
            }
            var filePath = Path.Combine(target, $"{fileName}");
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        [AllowAnonymous]
        [ActionName("getNewsFile")]
        [HttpGet("{divisionID}/{fileName}")]
        public async Task<IActionResult> GetNewsFile([FromRoute][Required] string fileName, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_NEWSFEED).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_NEWSFEED);
            }
            var filePath = Path.Combine(target, $"{fileName}");
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        /// <summary>
        /// Lấy hình ảnh của sản phẩm
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ActionName("getProductImage")]
        [HttpGet("{productID}/{fileName}")]
        public async Task<IActionResult> ViewProductImage([FromRoute][Required]string productID, [FromRoute][Required]string fileName, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_PRODUCT).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_PRODUCT);
            }
            var filePath = Path.Combine(target, productID,$"{fileName}");
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            else
            {
                var appActiveStr = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "AppActive");
                int appActive = 0;
                int.TryParse(appActiveStr, out appActive);
                fileName = appActive == 2 ? "user.png" : "default.png";
                if (checkIsUnixPath.Contains("/"))
                {
                    target = Path.Combine(erp9Path + @"\Content\Images").Replace("\\", "/");
                }
                else
                {
                    target = Path.Combine(erp9Path + @"\Content\Images");
                }
                filePath = Path.Combine(target, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var fileMemoryStream = new MemoryStream(fileBytes);
                    return File(fileMemoryStream, mimeType, fileName);
                }
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        /// <summary>
        /// Lấy hình ảnh đánh giá sản phẩm
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ActionName("getReviewImage")]
        [HttpGet("{productID}/{fileName}")]
        public async Task<IActionResult> ViewReviewImage([FromRoute][Required]string productID, [FromRoute][Required]string fileName, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + string.Format(ASOFTConstants.PATH_PRODUCT_REVIEW, productID)).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + string.Format(ASOFTConstants.PATH_PRODUCT_REVIEW, productID));
            }
            var filePath = Path.Combine(target, $"{fileName}");
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            else
            {
                var appActiveStr = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "AppActive");
                int appActive = 0;
                int.TryParse(appActiveStr, out appActive);
                fileName = appActive == 2 ? "user.png" : "default.png";
                if (checkIsUnixPath.Contains("/"))
                {
                    target = Path.Combine(erp9Path + @"\Content\Images").Replace("\\", "/");
                }
                else
                {
                    target = Path.Combine(erp9Path + @"\Content\Images");
                }
                filePath = Path.Combine(target, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var fileMemoryStream = new MemoryStream(fileBytes);
                    return File(fileMemoryStream, mimeType, fileName);
                }
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        /// <summary>
        /// Lấy file đính kèm
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ActionName("getCheckinImage")]
        [HttpGet("{divisionID}/{fileName}")]
        public async Task<IActionResult> ViewCheckinImage([FromRoute][Required]string divisionID, [FromRoute][Required]string fileName, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_CHECKIN).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_CHECKIN);
            }
            var filePath = Path.Combine(target, $"{fileName}");
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        /// <summary>
        /// Lấy Avatar
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ActionName("getAvatar")]
        [HttpGet("{userID}")]
        public async Task<IActionResult> GetAvatar([FromRoute][Required]string userID, CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            string fileName = userID + ".png";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_AVATARS).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_AVATARS);
            }
            var filePath = Path.Combine(target, fileName);
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            else
            {
                var appActiveStr = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "AppActive");
                int appActive = 0;
                int.TryParse(appActiveStr, out appActive);
                fileName = appActive == 2 ? "user.png" : "default.png";
                if (checkIsUnixPath.Contains("/"))
                {
                    target = Path.Combine(erp9Path + @"\Content\Images").Replace("\\", "/");
                }
                else
                {
                    target = Path.Combine(erp9Path + @"\Content\Images");
                }
                filePath = Path.Combine(target, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var fileMemoryStream = new MemoryStream(fileBytes);
                    return File(fileMemoryStream, mimeType, fileName);
                }
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        /// <summary>
        /// Lấy logo
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ActionName("getLogo")]
        [HttpGet]
        public async Task<IActionResult> GetLogo(CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_LOGO).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_LOGO);
            }
            string fileName = "customLogo_APP.png";
            var filePath = Path.Combine(target, fileName);
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            else
            {
                var appActiveStr = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "AppActive");
                int appActive = 0;
                int.TryParse(appActiveStr, out appActive);
                fileName = appActive == 2 ? "user.png" : "default.png";
                if (checkIsUnixPath.Contains("/"))
                {
                    target = Path.Combine(erp9Path + @"\Content\Images").Replace("\\", "/");
                }
                else
                {
                    target = Path.Combine(erp9Path + @"\Content\Images");
                }
                filePath = Path.Combine(target, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var fileMemoryStream = new MemoryStream(fileBytes);
                    return File(fileMemoryStream, mimeType, fileName);
                }
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        /// <summary>
        /// Lấy logo
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ActionName("getSlogan")]
        [HttpGet]
        public async Task<IActionResult> getSlogan(CancellationToken cancellationToken = default)
        {
            var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
            var erp9Path = "";
            var target = "";
            if (checkIsUnixPath.Contains("/"))
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_LOGO).Replace("\\", "/");
            }
            else
            {
                erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                target = Path.Combine(erp9Path + ASOFTConstants.PATH_LOGO);
            }
            string fileName = "customSlogan_APP.png";
            var filePath = Path.Combine(target, fileName);
            string mimeType = MimeTypes.GetMimeType(fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileMemoryStream = new MemoryStream(fileBytes);
                return File(fileMemoryStream, mimeType, fileName);
            }
            return ASOFTError(DefaultErrorCodes.NotFound);
        }

        [ActionName("changeAvatar")]
        [HttpPost]
        public async Task<IActionResult> ChangeAvatar([FromForm(Name = "file")] IFormFile file, [FromServices] IIdentity identity, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkIsUnixPath = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                var erp9Path = "";
                var target = "";
                if (checkIsUnixPath.Contains("/"))
                {
                    erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "APIPhysicalPath");
                    target = Path.Combine(erp9Path + ASOFTConstants.PATH_AVATARS).Replace("\\", "/");
                }
                else
                {
                    erp9Path = await _configQueries.GetConfigValue((int)GroupConfig.HostingNAPI, "WebPhysicalPath");
                    target = Path.Combine(erp9Path + ASOFTConstants.PATH_AVATARS);
                }
                Directory.CreateDirectory(target);
                var userID = identity.ID;
                string fileName = userID + ".png";
                var filePath = Path.Combine(target, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return ASOFTSuccess(true);
            }
            catch (Exception e)
            {
                return ASOFTError(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFile([FromBody][Required] RemoveFileRequest command, [FromServices]IIdentity viewer, 
           CancellationToken cancellationToken)
        {
            command.UserID = viewer.ID;
            var result = await _fileBusiness.RemoveFile(command, cancellationToken);
            if (result.IsSucceed)
            {
                return ASOFTSuccess(result.Success);
            }
            if(result.Error.StatusCode == StatusCodes.Status403Forbidden)
            {
                return ASOFTForbidden(result.Error);
            }
            return ASOFTError(result.Error);
        }
    }
}
