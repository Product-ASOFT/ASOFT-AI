// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.API.Https;
using ASOFT.Core.API.Httpss.ApiResponse;
using ASOFT.Core.API.Paging;
using ASOFT.Core.API.Versions;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.Queries.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.Common.Security.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.API.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "Core")]
    public class CommentsController : CoreCommonBaseController
    {
        private readonly ICommentQueries _commentQueris;
        private readonly ICommentsBusiness _commentBusiness;
        private readonly INumberSizePagingAdapter _numberSizePagingAdapter;
        private readonly IPermissionQueries _permissionHelper;

        public CommentsController(ICommentQueries commentQueris, INumberSizePagingAdapter numberSizePagingAdapter, IPermissionQueries permissionHelper, ICommentsBusiness commentBusiness)
        {
            _commentQueris = Checker.NotNull(commentQueris, nameof(commentQueris));
            _commentBusiness = Checker.NotNull(commentBusiness, nameof(commentBusiness));
            _numberSizePagingAdapter = Checker.NotNull(numberSizePagingAdapter, nameof(numberSizePagingAdapter));
            _permissionHelper = Checker.NotNull(permissionHelper, nameof(permissionHelper));
        }

        /// <summary>
        /// Lấy danh sách ghi chú theo nghiệp vụ
        /// </summary>
        /// <param name="apk"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(ApiStatusCodes.Ok200, Type =
            typeof(SuccessResponse<NumberSizePagingEntity<CRMT90031ViewModel>>))]
        public async Task<IActionResult> GetCommentList([Required][FromQuery]Guid apk, [Required][FromQuery]int page, [Required][FromQuery]int pageSize, CancellationToken cancellationToken = default)
        {
            var (totalCount, comments) = await _commentQueris.GetCommentList(apk, page, pageSize, cancellationToken);
            var pagingModel = _numberSizePagingAdapter.Create(
               new NumberSizePagingEntity<CRMT90031ViewModel>(comments,
                   totalCount,
                   page,
                   pageSize));
            return ASOFTSuccess(pagingModel);
        }



        /// <summary>
        /// Thêm ghi chú
        /// </summary>
        /// <param name="command"></param>
        /// <param name="viewer"></param>
        /// <param name="mediator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(ApiStatusCodes.Ok200, Type =
            typeof(SuccessResponse<NumberSizePagingEntity<bool>>))]
        public async Task<IActionResult> InsertComment([Required][FromBody]CreateCommentRequest command, [FromServices]IIdentity viewer, CancellationToken cancellationToken = default)
        {
            command.PermissionInput.UserID = viewer.ID;
            //var permission = await _permissionHelper.GetPermissionByScreenAsync(command.PermissionInput);
            //if (permission == null)
            //{
            //    return ASOFTError(permission);
            //}
            //else if (permission.IsAddNew == 1)
            //{
            var sussces = await _commentBusiness.InsertComment(command, cancellationToken);
            if (sussces == null)
            {
                return ASOFTError(sussces);
            }
            return ASOFTSuccess(sussces);
            //}
            //else return ASOFTForbidden(permission.IsAddNew);
        }

        /// <summary>
        /// Xóa ghi chú
        /// </summary>
        /// <param name="command"></param>
        /// <param name="viewer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(ApiStatusCodes.Ok200, Type =
            typeof(SuccessResponse<NumberSizePagingEntity<bool>>))]
        public async Task<IActionResult> RemoveComment([Required][FromBody]RemoveCommentRequest command, [FromServices]IIdentity viewer, CancellationToken cancellationToken = default)
        {
            var result = await _commentBusiness.RemoveComment(command, cancellationToken);
            if (!result)
            {
                return ASOFTError(result);
            }
            return ASOFTSuccess(result);
        }
    }
}
