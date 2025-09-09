// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/03/2021      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.API.Httpss.Errors;
using ASOFT.Core.Business.Common.Business.EventListeners;
using ASOFT.Core.Business.Common.Business.Helpers;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Common.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Entities;
using ASOFT.Core.DataAccess.Entities.Admin;
using ASOFT.Core.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business
{
    public class CommonExcute<T, H> : ICommonExcute<T, H>
        where T : class
        where H : HistoryEntity, new()
    {
        private readonly IVoucherBusiness _voucherGenerator;
        private readonly IMessageContext _messageContext;
        private readonly IMediator _mediator;
        private readonly IBusinessContext<T> _entityContext;
        private readonly IAdminContext<SysTable> _adminContext;
        private readonly IHistoryBusiness<H> _historyBusiness;
        private readonly ILogger _logger;

        public CommonExcute(IVoucherBusiness voucherGenerator, IMessageContext messageContext, IMediator mediator, IBusinessContext<T> entityContext,
            IHistoryBusiness<H> historyBusiness, IAdminContext<SysTable> adminContext, ILoggerFactory logger)
        {
            _voucherGenerator = Checker.NotNull(voucherGenerator, nameof(voucherGenerator));
            _messageContext = Checker.NotNull(messageContext, nameof(messageContext));
            _mediator = Checker.NotNull(mediator, nameof(mediator));
            _entityContext = Checker.NotNull(entityContext, nameof(entityContext));
            _adminContext = Checker.NotNull(adminContext, nameof(adminContext));
            _historyBusiness = Checker.NotNull(historyBusiness, nameof(historyBusiness));
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }

        /// <summary>
        /// Luồng insert nghiệp vụ không có detail
        /// </summary>
        /// <param name="entity">Đối tượng nghiệp vụ</param>
        /// <param name="pkName">Tên của trường ID dùng để check tồn tại</param>
        /// <param name="tableID">Tên bảng</param>
        /// <param name="userID"></param>
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
        public async Task<Result<bool, ErrorModelV2>> InsertBusiness(T entity, string tableID, string userID, string pkName = null, string languageID = null, string voucherType = null,
            List<string> followerList = null, string followerTb = null, Func<Task> afterInsert = null, CancellationToken cancellationToken = default)
        {
            //Lấy kiểu dữ liệu của entity

            //Set ngôn ngữ nếu không có sẽ lấy mặc định của server
            var language = languageID ?? CultureInfo.CurrentUICulture.Name;
            try
            {
                var entityType = entity.GetType();
                var sysTable = await _adminContext.QueryFirstOrDefaultAsync(new FilterQuery<SysTable>(m => m.TableName == tableID));
                var pk = pkName ?? sysTable.PK;

                if (string.IsNullOrEmpty(pk))
                {
                    return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(code: DefaultErrorCodes.InvalidInputParameters));
                }

                //Lấy giá trị của trường pkName của
                var entityID = EntityHelper.GetPropertyValue(entityType, pk, entity).ToString();

                //Kiểm tra entity có tồn tại hay không
                var checkEntity = await _entityContext.QueryFirstOrDefaultAsync(new FilterQuery<T>(m => EF.Property<string>(m, pk) == entityID));

                if (checkEntity != null)
                {
                    //Trùng mã
                    var message = await _messageContext.GetByIDAsync("OOFML000010", language);
                    return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(code: DefaultErrorCodes.DuplicateID, message: message.Name));
                }

                var date = DateTime.Now;

                //Set các giá trị mặc đinh
                EntityHelper.SetPropertyValue(entityType, "CreateUserID", entity, userID);
                EntityHelper.SetPropertyValue(entityType, "CreateDate", entity, date);
                EntityHelper.SetPropertyValue(entityType, "LastModifyUserID", entity, userID);
                EntityHelper.SetPropertyValue(entityType, "LastModifyDate", entity, date);

                //Cập nhật voucher nếu có
               

                //Bắt đầu lưu dữ liệu
                await _entityContext.UnitOfWork.ExecuteInTransactionAsync(async holder =>
                {
                    //Lưu entity
                    await _entityContext.AddAsync(entity);
                    await _entityContext.UnitOfWork.CompleteAsync();

                    if (!string.IsNullOrEmpty(voucherType))
                    {
                        var voucherinfo = new VoucherInfo(EntityHelper.GetPropertyValue(entityType, "DivisionID", entity).ToString(), tableID, date.Month, date.Year, voucherType);
                        await _voucherGenerator.UpdateVoucherAsyncNoTransaction(voucherinfo, EntityHelper.GetPropertyValue(entityType, pk, entity).ToString(), cancellationToken);
                    }

                    //Thêm người theo dõi nếu có
                    if (followerList != null && followerList.Count > 0 && !string.IsNullOrEmpty(followerTb))
                    {
                        var followerListDistinct = followerList.Where(x => !string.IsNullOrEmpty(x)).Distinct();

                        foreach (var id in followerListDistinct)
                        {
                            var followerListener = new FollowerListener
                            {
                                APKMaster = new Guid(EntityHelper.GetPropertyValue(entityType, "APK", entity).ToString()),
                                CreateUserID = EntityHelper.GetPropertyValue(entityType, "CreateUserID", entity).ToString(),
                                DivisionID = EntityHelper.GetPropertyValue(entityType, "DivisionID", entity).ToString(),
                                TableID = tableID,
                                FollowerTable = followerTb,
                                FollowerID = id
                            };
                            await _mediator.Publish(followerListener);
                        }
                    }

                    //Sử lý sau khi instert thành công
                    if (afterInsert != null)
                    {
                        await afterInsert().ConfigureAwait(false);
                    }

                    await _entityContext.UnitOfWork.CompleteAsync();
                });
                var divisionID = EntityHelper.GetPropertyValue(entityType, "DivisionID", entity).ToString();
                try
                {
                    await _historyBusiness.AddHistory<T>(entity, divisionID, tableID, ASOFTPermission.AddNew, userID, useTransaction: true);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error while insert History to table {tableID}: " + e.ToString());
                }
                return Result<bool, ErrorModelV2>.FromSuccess(true);

            }
            catch (Exception e)
            {
                _logger.LogError($"Error while insert to table {tableID}: "+e.ToString());
                var message = await _messageContext.GetByIDAsync("00ML000068", language);
                return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(message: message.Name));
            }

        }

        /// <summary>
        /// Luồng insert nghiệp vụ không có detail
        /// </summary>
        /// <param name="pkValue"></param>
        /// <param name="pkName">Tên của trường ID dùng để check tồn tại</param>
        /// <param name="tableID">Tên bảng</param>
        /// <param name="userID"></param>
        /// <param name="screenID">mã màn hình để thêm lịch sử</param>
        /// <param name="moduleID">module dùng để lưu lịch sử</param>
        /// <param name="languageID">Ngôn ngữ</param>
        /// <param name="followerList">Danh sách người theo dõi cần thêm</param>
        /// <param name="followerTb">Bảng người theo dõi</param>
        /// <param name="beforeUpdate"></param>
        /// <param name="afterUpdate"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="refName"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/04/2021]
        /// </history>
        public async Task<Result<bool, ErrorModelV2>> UpdateBusiness(string pkValue, string pkName, string tableID, string userID, string screenID, string moduleID, string languageID = null,
            List<string> followerList = null, string followerTb = null, Func<T, Task> beforeUpdate = null, Func<T, Task> afterUpdate = null, CancellationToken cancellationToken = default, string refName = null)
        {
            //Set ngôn ngữ nếu không có sẽ lấy mặc định của server
            var language = languageID ?? CultureInfo.CurrentUICulture.Name;
            try
            {
                //Kiểm tra entity có tồn tại hay không
                var entity = await _entityContext.QueryFirstOrDefaultAsync(new FilterQuery<T>(m => EF.Property<string>(m, pkName).ToString() == pkValue));
                string jsonString = JsonSerializer.Serialize(entity);
                var oldEntity = JsonSerializer.Deserialize<T>(jsonString);

                if (entity == null)
                {
                    //Không tìm thấy dữ liệu!
                    var message = await _messageContext.GetByIDAsync("00ML000209", language);
                    return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(code: DefaultErrorCodes.NotFound, message: message.Name));
                }
                var entityType = entity.GetType();

                var date = DateTime.Now;

                if (beforeUpdate != null)
                {
                    await beforeUpdate(entity);
                    EntityHelper.SetPropertyValue(entityType, "LastModifyUserID", entity, userID);
                    EntityHelper.SetPropertyValue(entityType, "LastModifyDate", entity, date);
                }

                //Bắt đầu lưu dữ liệu
                await _entityContext.UnitOfWork.ExecuteInTransactionAsync(async holder =>
                {
                    //Lưu entity
                    await _entityContext.UpdateAsync(entity);
                    await _entityContext.UnitOfWork.CompleteAsync();

                    //Thêm người theo dõi nếu có
                    if (followerList != null && followerList.Count > 0 && !string.IsNullOrEmpty(followerTb))
                    {
                        var followerListDistinct = followerList.Where(x => !string.IsNullOrEmpty(x)).Distinct();

                        foreach (var id in followerListDistinct)
                        {
                            var followerListener = new FollowerListener
                            {
                                APKMaster = new Guid(EntityHelper.GetPropertyValue(entityType, "APK", entity).ToString()),
                                CreateUserID = EntityHelper.GetPropertyValue(entityType, "CreateUserID", entity).ToString(),
                                DivisionID = EntityHelper.GetPropertyValue(entityType, "DivisionID", entity).ToString(),
                                TableID = tableID,
                                FollowerTable = followerTb,
                                FollowerID = id
                            };
                            await _mediator.Publish(followerListener);
                        }
                    }

                    var changesResult = EntityHelper.CompareEntity(oldEntity, entity);

                    var changes = new List<string>();

                    foreach (var result in changesResult)
                    {
                        changes.Add("- '" + screenID + "." + result.Name + "." + moduleID + "': " + result.OldValue + " -> " + result.NewValue);
                    }

                    if(changes != null && changes.Count > 0)
                    await _historyBusiness.AddHistory<T>(entity, EntityHelper.GetPropertyValue(entityType, "DivisionID", entity).ToString(), tableID, ASOFTPermission.Update, userID, null, null, null, changes, screenID, null, moduleID, refName:refName);

                    //Sử lý sau khi instert thành công
                    if (afterUpdate != null)
                    {
                        await afterUpdate(entity).ConfigureAwait(false);
                    }

                    await _entityContext.UnitOfWork.CompleteAsync();
                });
                return Result<bool, ErrorModelV2>.FromSuccess(true);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while update to table {tableID}: " + e.ToString());
                var message = await _messageContext.GetByIDAsync("00ML000068", language);
                return Result<bool, ErrorModelV2>.FromError(new ErrorModelV2(message: message.Name));
            }
        }
    }
}
