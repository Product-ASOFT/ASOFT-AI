// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/03/2021      Đoàn Duy      	Tạo mới
// # 	08/03/2024		Minh Nhựt		Lỗi không tìm thấy bảng OT9020 trong sysTable 
// ##################################################################

using ASOFT.Core.Business.Common.Business.Helpers;
using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.Entities;
using ASOFT.Core.DataAccess.Entities.Admin;
using ASOFT.Core.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business
{
    public class HistoryBusiness<H> : IHistoryBusiness<H> where H : HistoryEntity, new()
    {
        private readonly IBusinessContext<CRMT0099> _crmt0099Context;
        private readonly IAdminContext<SysTable> _adminContext;
        private readonly IBusinessContext<H> _historyContext;
        private readonly IHistoryQueries<H> _historyQueries;
        private readonly ILogger _logger;

        public HistoryBusiness(IBusinessContext<CRMT0099> crmt0099Context, IAdminContext<SysTable> adminContext, IBusinessContext<H> historyContext, ILoggerFactory logger, IHistoryQueries<H> historyQueries)
        {
            _crmt0099Context = Checker.NotNull(crmt0099Context, nameof(crmt0099Context));
            _adminContext = Checker.NotNull(adminContext, nameof(adminContext));
            _historyContext = Checker.NotNull(historyContext, nameof(historyContext));
            _historyQueries = Checker.NotNull(historyQueries, nameof(historyQueries));
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }


        /// <summary>
        /// Add lịch sử
        /// </summary>
        /// <typeparam name="L"></typeparam>
        /// <param name="entity">Dữ liệu được thêm mới hoặc cập nhật</param>
        /// <param name="divisionID"></param>
        /// <param name="table">Bảng của chính</param>
        /// <param name="per">Loại lịch sử cần lưu</param>
        /// <param name="userID">Người tạo</param>
        /// <param name="dt">Các ID của detail được Xóa</param>
        /// <param name="parentTable">id bảng cha</param>
        /// <param name="valueParent">giá trị khóa chính của bảng cha</param>
        /// <param name="historyChange">Những thay đổi của entity dùng khi Update entity</param>
        /// <param name="screenID"></param>
        /// <param name="tableID"></param>
        /// <param name="moduleID"></param>
        /// <param name="useTransaction"></param>
        /// <param name="refName"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/04/2021]
        /// </history>
        public async Task AddHistory<L>(L entity, string divisionID, string table, ASOFTPermission per, string userID, List<string> dt = null, string parentTable = null
            , string valueParent = null, List<string> historyChange = null, string screenID = null, string tableID = null, string moduleID = null, bool useTransaction = false, string refName = null)
        {
            try
            {

                string Desciption = string.Empty;

                var entityType = entity.GetType();

                var tbDB = await _adminContext.QueryFirstOrDefaultAsync(new FilterQuery<SysTable>(m => m.TableName == (parentTable ?? table)));

                var tableData = await _adminContext.QueryFirstOrDefaultAsync(new FilterQuery<SysTable>(m => m.TableName == table));

                var date = DateTime.Now;

                string valuePK = valueParent ?? EntityHelper.GetPropertyValue(entityType, tbDB.PK, entity).ToString();

                //lưu lịch sử trường hợp addnew
                if (per == ASOFTPermission.AddNew)
                {
                    Desciption = Desciption + "'A00.AddNew' <br/>";
                    var ConvertTypeObj = EntityHelper.GetPropertyValue(entityType, "ConvertType", entity);
                    if (ConvertTypeObj != null)
                    {
                        var ConvertType = ConvertTypeObj.ToString();
                        //Nếu tồn tại 2 cột đó sẽ ghi nhận thêm chuyển đổi từ một loại quan hệ
                        if (!string.IsNullOrEmpty(ConvertType))
                        {
                            string InheritConvertID = EntityHelper.GetPropertyValue(entityType, "InheritConvertID", entity).ToString();
                            var crmt0099 = await _crmt0099Context.QueryFirstOrDefaultAsync(new FilterQuery<CRMT0099>(m => m.CodeMaster == "CRMT00000002" && m.ID == ConvertType));
                            Desciption = Desciption + "- 'A00.AddNewConvert' '" + crmt0099.LanguageID + "'<br/>- 'A00.InheritConvertID': " + InheritConvertID;
                        }
                    }
                }

                //Lưu lịch sử trường hợp Update 
                if (per == ASOFTPermission.Update)
                {
                    if (historyChange != null && historyChange.Count > 0)
                    {
                        foreach (string item in historyChange)
                        {
                            Desciption = Desciption + item + " <br/>";
                        }
                    }
                }

                //Lưu lịch sử trường hợp Update trạng thái
                if (per == ASOFTPermission.UpdateStatus)
                {
                    if (historyChange != null && historyChange.Count > 0)
                    {
                        foreach (string item in historyChange)
                        {
                            Desciption = Desciption + item + " <br/>";
                        }
                    }
                }

                //Lưu lịch sử trường hợp lưu update detail
                if (per == ASOFTPermission.UpdateDetail)
                {
                    if (historyChange != null && historyChange.Count > 0)
                    {
                        SysTable tbDBCh = await _adminContext.QueryFirstOrDefaultAsync(new FilterQuery<SysTable>(m => m.TableName == table));

                        if (tbDBCh.TypeREL != null)
                        {
                            string Desciption1 = "'A00.Update'<br/>";
                            foreach (string item in historyChange)
                            {
                                Desciption1 = Desciption1 + item + " <br/>";
                            }

                            H historyModel2 = new H
                            {
                                Description = Desciption1,
                                RelatedToID = tbDBCh.PK,
                                StatusID = 2,
                                RelatedToTypeID = tbDBCh.TypeREL,
                                CreateUserID = userID,
                                DivisionID = divisionID,
                                CreateDate = date
                            };

                            await _historyContext.AddAsync(historyModel2);
                        }

                        if (!string.IsNullOrEmpty(tbDBCh.RefLink))
                            Desciption = Desciption + "'A00.Update' " + "'A00." + table + "': " + EntityHelper.GetPropertyValue(entityType, refName ?? tbDB.RefLink, entity).ToString() + "<br/>";
                    }
                }

                //Lưu lịch sử trường hợp addnew detail
                if (per == ASOFTPermission.AddDetail || per == ASOFTPermission.Creation)
                {
                    string refLinkPk = table.Equals("CRMT90031") ? "NotesSubject" : table.Equals(moduleID + "T9020") ? "FollowersSubject" : tableData != null ? tableData.RefLink : tbDB.RefLink;

                    if (per == ASOFTPermission.Creation)
                    {
                        Desciption = Desciption + "'A00.Creation' ";
                        Desciption = Desciption + "'A00." + table + "': <br/>";
                        Desciption = Desciption + "- " + EntityHelper.GetPropertyValue(entityType, refName ?? refLinkPk, entity).ToString();
                    }
                    else
                    {
                        Desciption = Desciption + "'A00.AddNew' ";
                        Desciption = Desciption + "'A00." + table + "'";

                        if (!string.IsNullOrEmpty(refLinkPk))
                        {
                            Desciption = Desciption + ": " + EntityHelper.GetPropertyValue(entityType, refLinkPk, entity).ToString();
                        }
                    }

                }

                //Lưu lịch sử trường hợp xóa detail
                if (per == ASOFTPermission.Delete)
                {
                    Desciption = Desciption + "'A00.Delete' <br/>";
                    foreach (var item in dt)
                    {
                        string vlDelete = string.Empty;
                        vlDelete = item;

                        Desciption = Desciption + "- 'A00." + table + "': " + vlDelete + "<br/>";
                    }
                }

                if (!string.IsNullOrEmpty(Desciption))
                {
                    H historyModel = new H
                    {
                        DivisionID = divisionID,
                        Description = Desciption,
                        RelatedToID = valuePK,
                        StatusID = per == ASOFTPermission.AddNew ? 1 : 2,
                        RelatedToTypeID = tbDB != null ? tbDB.TypeREL : 0,
                        CreateUserID = userID,
                        ScreenID = screenID != null ? screenID : string.Empty,
                        TableID = tableID != null ? tableID : string.Empty,
                        CreateDate = date
                    };

                    if(historyModel.GetType().Name == "CRMT00003")
                    {
                        await _historyQueries.InstallHistory(historyModel);
                    }
                    else
                    {
                        if (useTransaction)
                        {
                            await _historyContext.UnitOfWork.ExecuteInTransactionAsync(async holer =>
                            {
                                await _historyContext.AddAsync(historyModel);
                                await _historyContext.UnitOfWork.CompleteAsync();

                            });
                        }
                        else
                        {

                            await _historyContext.AddAsync(historyModel);
                        }
                    }
                  
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while insert history to table {table}: " + ex.ToString());
                return;
            }
        }


    }
}
