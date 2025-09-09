// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy        Tạo mới
// #    24/12/2024      Minh Nhựt       Bổ sung kiểm tra cập nhật đường dẫn khi API host trong môi trường Linux
// ##################################################################

using ASOFT.A00.Entities;
using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Files.Business.Interfaces;
using ASOFT.Core.Business.Files.Entities.Requests;
using ASOFT.Core.Business.Files.Entities.Respones;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Entities;
using ASOFT.Core.DataAccess.Entities.Admin;
using ASOFT.Core.DataAccess.Enums;
using ASOFT.Core.DataAccess;
using ASOFT.Data.Core.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRMT00002 = ASOFT.Core.DataAccess.Entities.CRMT00002;
using CRMT00002_REL = ASOFT.Core.DataAccess.Entities.CRMT00002_REL;

namespace ASOFT.Core.Business.Files.Business
{
    public class FilesBusiness : IFileBusiness
    {
        private readonly IBusinessContext<CRMT00002> _attachContext;
        private readonly IBusinessContext<CRMT00002_REL> _attachRelContext;
        private readonly IAdminContext<SysTable> _adminContext;
        private readonly IConfiguration _config;
        private readonly IPermissionQueries _permissionHelper;
        private readonly IConfigQueries _configQueries;
        private readonly IHistoryBusiness<OOT00003> _ooHistory;
        private readonly IHistoryBusiness<CRMT00003> _crmHistory;

        public FilesBusiness(IBusinessContext<CRMT00002> attachContext, IAdminContext<SysTable> adminContext,
            IConfiguration config, IBusinessContext<CRMT00002_REL> attachRelContext, IPermissionQueries permissionHelper, IConfigQueries configQueries,
            IHistoryBusiness<OOT00003> ooHistory, IHistoryBusiness<CRMT00003> crmHistory)
        {
            _attachContext = Checker.NotNull(attachContext, nameof(attachContext));
            _attachRelContext = Checker.NotNull(attachRelContext, nameof(attachRelContext));
            _adminContext = Checker.NotNull(adminContext, nameof(adminContext));
            _config = Checker.NotNull(config, nameof(config));
            _permissionHelper = Checker.NotNull(permissionHelper, nameof(permissionHelper));
            _configQueries = Checker.NotNull(configQueries, nameof(configQueries));
            _crmHistory = Checker.NotNull(crmHistory, nameof(crmHistory));
            _ooHistory = Checker.NotNull(ooHistory, nameof(ooHistory));
        }

        /// <summary>
        /// Xử lý upload file
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UploadRespone> UploadFile(UploadFileRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            int count = 0;
            var resultList = new List<FileUploadResult>();
            long size = request.Files.Sum(f => f.Length);
            //Thiết lập đường dẫn lưu file
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
            Directory.CreateDirectory(target);

            //Lấy sysTable tương ứng với nghiệp vụ
            var sysTable = await _adminContext.QueryFirstOrDefaultAsync(new FilterQuery<SysTable>(m => m.TableName == request.TableID));

            foreach (var file in request.Files)
            {
                if (file.Length <= 0)
                {
                    resultList.Add(new FileUploadResult
                    {
                        FileName = file.FileName,
                        Success = false
                    });
                    continue;
                }

                try
                {
                    //lưu dữ liệu xuống bảng CRMT00002
                    var apk = Guid.NewGuid();
                    var now = DateTime.Now;
                    var attach = new CRMT00002
                    {
                        APK = apk,
                        AttachName = file.FileName,
                        CreateDate = now,
                        DivisionID = request.DivisionID,
                        CreateUserID = request.UserID,
                        LastModifyDate = now,
                        LastModifyUserID = request.UserID
                    };
                    await _attachContext.UnitOfWork.ExecuteInTransactionAsync(
                        async holder =>
                        {
                            await _attachContext.AddAsync(attach);
                            await _attachContext.UnitOfWork.CompleteAsync();
                        }, cancellationToken);


                    //lưu dữ liệu xuống bảng CRMT00002_REL
                    var rel = new CRMT00002_REL
                    {
                        AttachID = attach.AttachID,
                        DivisionID = request.DivisionID,
                        RelatedToID = request.APK.ToString(),
                        RelatedToTypeID_REL = sysTable.TypeREL
                    };
                    await _attachRelContext.UnitOfWork.ExecuteInTransactionAsync(
                        async holder =>
                        {
                            await _attachRelContext.AddAsync(rel);

                            await AddHistory(request.APK.ToString(), request.TableID, attach, ASOFTPermission.AddDetail);

                            await _attachRelContext.UnitOfWork.CompleteAsync();
                        }, cancellationToken);

                    //Lưu file vào thư mục
                    var fileName = apk;
                    var fileExtension = Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(target, $"{fileName.ToString()}{fileExtension}");
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    resultList.Add(new FileUploadResult
                    {
                        FileName = file.FileName,
                        Success = true
                    });
                    count++;
                }
                catch (Exception e)
                {
                    resultList.Add(new FileUploadResult
                    {
                        FileName = file.FileName,
                        Success = false
                    });
                    continue;
                }

            }

            return new UploadRespone
            {
                Count = count,
                Size = size,
                ResultList = resultList
            };
        }

        /// <summary>
        /// Xử lý xóa file
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result<bool, ErrorModelV2>> RemoveFile(RemoveFileRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //Lấy quyền màn hình
            var permission = await _permissionHelper.GetPermissionByScreenAsync(new ScreenPermissionRequest
            {
                DivisionID = request.DivisionID,
                ScreenID = request.ScreenID,
                UserID = request.UserID
            });

            //Kiểm tra file nó tồn tại hay không
            var files = await _attachContext.QueryAsync(new FilterQuery<CRMT00002>(m => request.APKs.Contains(m.APK)));

            if (files == null || permission == null)
            {
                return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(code: DefaultErrorCodes.NotFound));
            }

            var listAttachID = files.Select(m =>
            {
                return m.AttachID;
            });
            //Kiểm tra rel
            var fileREL = await _attachRelContext.QueryAsync(new FilterQuery<CRMT00002_REL>(m => listAttachID.Contains(m.AttachID)));

            if (fileREL == null)
            {
                return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(code: DefaultErrorCodes.NotFound));
            }

            if (permission.IsDelete == 1)
            {
                await _attachRelContext.UnitOfWork.ExecuteInTransactionAsync(
               async holder =>
               {
                   await _attachRelContext.BulkDeleteAsync(fileREL);

                   var ids = files.Select(m => m.AttachName).ToList();
                   await AddHistory(request.APKMaster.ToString(), request.TableID, new CRMT00002 { DivisionID = request.DivisionID, CreateUserID = request.UserID}, ASOFTPermission.Delete, ids);


                   await _attachRelContext.UnitOfWork.CompleteAsync();
               }, cancellationToken);
                return Result<bool, ErrorModelV2>.FromSuccess(true);
            }
            return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(message: "No Permission", statusCode: StatusCodes.Status403Forbidden));

        }

        private async Task AddHistory(string apk, string table, CRMT00002 entity, ASOFTPermission per, List<string> ids = null)
        {
            if (table.StartsWith("OO"))
            {
                await _ooHistory.AddHistory(entity, entity.DivisionID, "Attach", per, entity.CreateUserID, moduleID: "OO", dt: ids, valueParent: apk, parentTable: "CRMT00002");
            }

            if (table.StartsWith("CRM"))
            {
                await _crmHistory.AddHistory(entity, entity.DivisionID, "Attach", per, entity.CreateUserID, moduleID: "CRM", dt: ids, valueParent: apk, parentTable: "CRMT00002");
            }
        }
    }
}
