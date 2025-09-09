// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/03/2021      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Common.Business.Helpers;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Entities;
using ASOFT.Core.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business
{
    public class CommonExcuteWithDetail<M, D, H> : ICommonExcuteWithDetail<M, D, H>
        where M : class
        where D : class
        where H : HistoryEntity, new()
    {
        private readonly ICommonExcute<M, H> _commonExcute;
        private readonly IBusinessContext<M> _masterContext;
        private readonly IBusinessContext<D> _detailContext;
        private readonly IVoucherBusiness _voucherGenerator;
        private readonly IMessageContext _messageContext;
        private readonly IMediator _mediator;

        public CommonExcuteWithDetail(IBusinessContext<M> masterContext, IBusinessContext<D> detailContext, IVoucherBusiness voucherGenerator, IMessageContext messageContext, IMediator mediator,
            ICommonExcute<M, H> commonExcute)
        {
            _masterContext = Checker.NotNull(masterContext, nameof(masterContext));
            _detailContext = Checker.NotNull(detailContext, nameof(detailContext));
            _voucherGenerator = Checker.NotNull(voucherGenerator, nameof(voucherGenerator));
            _messageContext = Checker.NotNull(messageContext, nameof(messageContext));
            _mediator = Checker.NotNull(mediator, nameof(mediator));
            _commonExcute = Checker.NotNull(commonExcute, nameof(commonExcute));
        }

        /// <summary>
        /// Luồng insert nghiệp vụ master detail
        /// </summary>
        /// <param name="master">Đối tượng master</param>
        /// <param name="masterPKName">Tên trường khóa chính của master để check tồn tại</param>
        /// <param name="masterTb">Bảng master</param>
        /// <param name="details">Danh sách detail</param>
        /// <param name="userID">Người tạo</param>
        /// <param name="languageID">Ngôn ngữ</param>
        /// <param name="voucherType">Loại voucher</param>
        /// <param name="followerList">Danh sách người theo dõi cần thêm</param>
        /// <param name="followerTb">Bảng người theo dõi</param>
        /// <param name="afterInsert">Xử lý sau insert</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/04/2021]
        /// </history>
        public async Task<Result<bool, ErrorModelV2>> InsertMasterDetail(M master, string masterPKName, string masterTb, List<D> details, string userID, string languageID = null, string voucherType = null,
            List<string> followerList = null, string followerTb = null, Func<Task> afterInsert = null, CancellationToken cancellationToken = default)
        {
            return await _commonExcute.InsertBusiness(master, masterTb, userID, masterPKName, languageID, voucherType, followerList, followerTb,
                async () =>
                {
                    foreach (var item in details)
                    {
                        await _detailContext.AddAsync(item);
                    }
                    if (afterInsert != null)
                    {
                        await afterInsert();
                    }
                }, cancellationToken);
        }

        /// <summary>
        /// Luồng insert nghiệp vụ không có detail
        /// </summary>
        /// <param name="pkValue">giá trị khóa chính</param>
        /// <param name="pkName">Tên của trường ID dùng để check tồn tại</param>
        /// <param name="tableID">Tên bảng</param>
        /// <param name="details">danh sách details</param>
        /// <param name="userID"></param>
        /// <param name="screenID">mã màn hình để lưu lịch sử</param>
        /// <param name="moduleID">mã module để lưu lịch sử</param>
        /// <param name="languageID">Ngôn ngữ</param>
        /// <param name="followerList">Danh sách người theo dõi cần thêm</param>
        /// <param name="followerTb">Bảng người theo dõi</param>
        /// <param name="beforeUpdate"></param>
        /// <param name="afterUpdate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/04/2021]
        /// </history>
        public async Task<Result<bool, ErrorModelV2>> UpdateMasterDetail(string pkValue, string pkName, string tableID, List<D> details, string userID, string screenID, string moduleID, string languageID = null,
            List<string> followerList = null, string followerTb = null, Func<M, Task> beforeUpdate = null, Func<M, Task> afterUpdate = null, CancellationToken cancellationToken = default, string masterCol = null, string detailMasterCol = null)
        {
            return await _commonExcute.UpdateBusiness(pkValue, pkName, tableID, userID, screenID, moduleID, languageID, followerList, followerTb, beforeUpdate,
                async (master) =>
                {
                    var masterAPK = EntityHelper.GetPropertyValue(master.GetType(), masterCol??"APK", master).ToString();
                    var masterColName =  detailMasterCol ?? ( masterCol ?? "APKMaster");

                    var oldDetails = new List<D>();
                    try
                    {
                        oldDetails = await _detailContext.QueryAsync(new FilterQuery<D>(m => EF.Property<string>(m, masterColName).ToString() == masterAPK));
                    }
                    catch (Exception)
                    {
                        oldDetails = await _detailContext.QueryAsync(new FilterQuery<D>(m => EF.Property<string>(m, masterColName) == masterAPK));
                    }

                    await _detailContext.BulkDeleteAsync(oldDetails);

                    await _detailContext.BulkAddAsync(details);

                    if (afterUpdate != null)
                    {
                        await afterUpdate(master);
                    }
                }, cancellationToken);
        }
    }
}
