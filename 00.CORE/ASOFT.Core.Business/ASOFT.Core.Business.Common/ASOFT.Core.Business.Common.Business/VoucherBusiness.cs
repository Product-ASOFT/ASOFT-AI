using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.Business.Common.Entities.Requests;
using ASOFT.Core.Business.Common.Entities.ViewModels;
using ASOFT.Core.Business.Common.ViewModels;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess;
using ASOFT.CRM.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business
{
    /// <summary>
    /// Class mặc định tạo voucher
    /// </summary>
    public class VoucherBusiness : IVoucherBusiness
    {
        private readonly ILogger _logger;
        private readonly IBusinessContext<AT1007> _voucherTypeContext;
        private readonly IBusinessContext<AT4444> _tableKeyContext;
        // [Tấn Thành] - [17/12/2020] - BEGIN ADD
        private readonly IVoucherQueries _voucherQueries;
        // [Tấn Thành] - [17/12/2020] - END ADD
        private readonly IBusinessContext<OOT0060> _Context;
        private readonly IBusinessContext<CRMT00000> _CRMContext;
        /// <summary>
        /// Class mặc định tạo voucher
        /// </summary>
        /// <param name="voucherTypeContext"></param>
        /// <param name="autoGenerateKeyContext"></param>
        /// <param name="loggerFactory"></param>
        public VoucherBusiness(IBusinessContext<AT1007> voucherTypeContext, IBusinessContext<AT4444> autoGenerateKeyContext, ILoggerFactory loggerFactory,
            IVoucherQueries voucherQueries, IBusinessContext<OOT0060> Context, IBusinessContext<CRMT00000> CRMContext)
        {
            _voucherTypeContext = Checker.NotNull(voucherTypeContext, nameof(voucherTypeContext));
            _tableKeyContext = Checker.NotNull(autoGenerateKeyContext, nameof(autoGenerateKeyContext));
            _logger = Checker.NotNull(loggerFactory, nameof(loggerFactory)).CreateLogger(GetType());
            // [Tấn Thành] - [17/12/2020] - BEGIN ADD
            _voucherQueries = Checker.NotNull(voucherQueries, nameof(voucherQueries));
            // [Tấn Thành] - [17/12/2020] - END ADD
            _Context = Checker.NotNull(Context, nameof(Context));
            _CRMContext = Checker.NotNull(CRMContext, nameof(CRMContext));
        }

        /// <summary>
        /// Tạo một voucher no
        /// </summary>
        /// <param name="voucherInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<string> CreateVoucherAsync(VoucherInfo voucherInfo,
                CancellationToken cancellationToken = default)
        {
            if (voucherInfo == null)
            {
                throw new ArgumentNullException(nameof(voucherInfo));
            }

            // Lấy voucher type
            var voucherType =
                await GetVoucherTypeAsync(voucherInfo.DivisionID, voucherInfo.VoucherTypeID, cancellationToken);

            if (voucherType == null)
            {
                _logger.LogWarning(
                    "Voucher type not found with DivisionID: '{DivisionID}' and VoucherTypeID: '{VoucherTypeID}'",
                    voucherInfo.DivisionID, voucherInfo.VoucherTypeID);
                return string.Empty;
            }

            var keyString = CreateKeyString(voucherInfo, voucherType);
            // Lấy table key
            var tableKey = await GetTableKeyAsync(voucherInfo.TableID, keyString, voucherInfo.DivisionID, cancellationToken);

            string voucherNo;
            var isLogInfo = _logger.IsEnabled(LogLevel.Information);
            if (tableKey != null)
            {
                voucherNo = GenerateVoucher(voucherType, tableKey, voucherInfo);

                if (isLogInfo)
                {
                    _logger.LogInformation("Voucher is created.");
                }

                return voucherNo;
            }

            // Tạo mới đối tượng table key
            tableKey = new AT4444
            {
                DivisionID = voucherInfo.DivisionID,
                KEYSTRING = keyString,
                TABLENAME = voucherInfo.TableID,
                LASTKEY = 0
            };

            if (isLogInfo)
            {
                _logger.LogInformation("Creating table key AT4444.");
            }

            await _tableKeyContext.AddAsync(tableKey, cancellationToken);
            await _tableKeyContext.UnitOfWork.CompleteAsync();

            voucherNo = GenerateVoucher(voucherType, tableKey, voucherInfo);

            if (isLogInfo)
            {
                _logger.LogInformation("Voucher is created.");
            }

            return voucherNo;
        }

        /// <summary>
        /// Cập nhật voucher no.
        /// </summary>
        /// <param name="voucherInfo"></param>
        /// <param name="voucherNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateVoucherAsync(VoucherInfo voucherInfo, string voucherNo,
            CancellationToken cancellationToken = default)
        {
            Checker.NotNull(voucherInfo, nameof(voucherInfo));
            Checker.NotEmpty(voucherNo, nameof(voucherInfo));

            // Lấy voucher type
            var voucherType =
                await GetVoucherTypeAsync(voucherInfo.DivisionID, voucherInfo.VoucherTypeID, cancellationToken);

            if (voucherType == null)
            {
                _logger.LogWarning(
                    "Voucher type not found with DivisionID: '{DivisionID}' and VoucherTypeID: '{VoucherTypeID}'",
                    voucherInfo.DivisionID, voucherInfo.VoucherTypeID);
                return;
            }

            var keyString = CreateKeyString(voucherInfo, voucherType);
            // Lấy table key
            var tableKey = await GetTableKeyAsync(voucherInfo.TableID, keyString, voucherInfo.DivisionID, cancellationToken);

            if (tableKey == null)
            {
                _logger.LogWarning("Table key AT4444 not found.");
                return;
            }

            var identifyModel = UpdateLastRecordIdKey(voucherType, tableKey, voucherInfo, voucherNo);
            tableKey.LASTKEY = identifyModel.LastKey;

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Updating table key with TableID: '{TableID}'", tableKey.TABLENAME);
            }
            await _tableKeyContext.UpdateAsync(tableKey, cancellationToken);
            await _tableKeyContext.UnitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Cập nhật voucher no.
        /// </summary>
        /// <param name="voucherInfo"></param>
        /// <param name="voucherNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UpdateVoucherAsyncNoTransaction(VoucherInfo voucherInfo, string voucherNo,
            CancellationToken cancellationToken = default)
        {
            Checker.NotNull(voucherInfo, nameof(voucherInfo));
            Checker.NotEmpty(voucherNo, nameof(voucherInfo));

            // Lấy voucher type
            var voucherType =
                await GetVoucherTypeAsync(voucherInfo.DivisionID, voucherInfo.VoucherTypeID, cancellationToken);

            if (voucherType == null)
            {
                _logger.LogWarning(
                    "Voucher type not found with DivisionID: '{DivisionID}' and VoucherTypeID: '{VoucherTypeID}'",
                    voucherInfo.DivisionID, voucherInfo.VoucherTypeID);
                return;
            }

            var keyString = CreateKeyString(voucherInfo, voucherType);
            // Lấy table key
            var tableKey = await GetTableKeyAsync(voucherInfo.TableID, keyString,voucherInfo.DivisionID, cancellationToken);

            if (tableKey == null)
            {
                _logger.LogWarning("Table key AT4444 not found.");
                return;
            }

            var identifyModel = UpdateLastRecordIdKey(voucherType, tableKey, voucherInfo, voucherNo);
            tableKey.LASTKEY = identifyModel.LastKey;

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Updating table key with TableID: '{TableID}'", tableKey.TABLENAME);
            }
            //await _tableKeyContext.UnitOfWork.ExecuteInTransactionAsync(async holder =>{
                await _tableKeyContext.UpdateAsync(tableKey, cancellationToken);
                //await _tableKeyContext.UnitOfWork.CompleteAsync();
            //});

        }

        /// <summary>
        /// Tạo key string.
        /// HuynhThu 12/05/2020
        /// Fix loi lay sai type.
        /// </summary>
        /// <param name="voucherInfo"></param>
        /// <param name="voucherType"></param>
        /// <returns></returns>
        protected virtual string CreateKeyString(VoucherInfo voucherInfo, AT1007 voucherType)
        {
            var s1 = voucherType.Enabled1 == 1
                ? GetStringByType(voucherType.S1Type, voucherInfo, voucherType.S1)
                : string.Empty;
            var s2 = voucherType.Enabled2 == 1
                ? GetStringByType(voucherType.S2Type, voucherInfo, voucherType.S2)
                : string.Empty;
            var s3 = voucherType.Enabled3 == 1
                ? GetStringByType(voucherType.S3Type, voucherInfo, voucherType.S3)
                : string.Empty;

            return $"{s1}{s2}{s3}";
        }

        /// <summary>
        /// Lấy table key
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="keyString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<AT4444> GetTableKeyAsync(string tableID, string keyString, string divisionID,
            CancellationToken cancellationToken)
            => await _tableKeyContext.QueryFirstOrDefaultAsync(
                    new FilterQuery<AT4444>(m => m.TABLENAME == tableID && m.KEYSTRING == keyString && m.DivisionID == divisionID))
                .ConfigureAwait(false);

        /// <summary>
        /// Lấy đối tượng loại chứng từ
        /// </summary>
        /// <param name="divisionID"></param>
        /// <param name="voucherTypeID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<AT1007> GetVoucherTypeAsync(string divisionID, string voucherTypeID,
            CancellationToken cancellationToken)
            => await _voucherTypeContext.QueryFirstOrDefaultAsync(new FilterQuery<AT1007>(m =>
                    (m.DivisionID == divisionID || m.DivisionID == "@@@") && m.VoucherTypeID == voucherTypeID))
                .ConfigureAwait(false);

        /// <summary>
        /// Trả về giá trị theo loại.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="voucherInfo"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetStringByType(byte? type, VoucherInfo voucherInfo, string defaultValue)
        {
            if (voucherInfo == null)
            {
                throw new ArgumentNullException(nameof(voucherInfo));
            }

            switch (type)
            {
                case 1: // by month (MM)
                    return voucherInfo.TranMonth.ToString().PadLeft(2, '0');
                //break;
                case 2: // by year (yyyy)
                    return voucherInfo.TranYear.ToString();
                //break;
                case 3: // by voucherType
                    return voucherInfo.VoucherTypeID;
                //break;
                case 4: // by Division
                    return voucherInfo.DivisionID;
                case 5: // by default string				
                    return defaultValue;
                //break;	
                case 6: // by short year
                    var yearString = $"{voucherInfo.TranYear:0000}";
                    return yearString.Substring(yearString.Length - 2, 2);
                //break;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Tạo voucher no.
        /// </summary>
        /// <param name="voucherType"></param>
        /// <param name="tableKey"></param>
        /// <param name="voucherInfo"></param>
        /// <returns></returns>
        public static string GenerateVoucher(AT1007 voucherType, AT4444 tableKey, VoucherInfo voucherInfo)
        {
            if (voucherType == null)
            {
                throw new ArgumentNullException(nameof(voucherType));
            }

            if (tableKey == null)
            {
                throw new ArgumentNullException(nameof(tableKey));
            }

            if (voucherInfo == null)
            {
                throw new ArgumentNullException(nameof(voucherInfo));
            }

            var identifyModel = CreateIdentifyModel(voucherType, tableKey);

            // Nếu chưa có thiết lập tăng mã tự động, hoặc có nhưng "tự động" = false
            if (0 == identifyModel.IsAutomatic)
            {
                return string.Empty;
            }

            // Kiểm tra các nhóm (s1, s2, s3) bị disable, nhóm nào bị disable thì thay bằng chuỗi empty
            var s1 = identifyModel.Enable1 == 1 && identifyModel.S1 != null
                ? GetStringByType(identifyModel.S1Type, voucherInfo, identifyModel.S1)
                : string.Empty;
            var s2 = identifyModel.Enable2 == 1 && identifyModel.S2 != null
                ? GetStringByType(identifyModel.S2Type, voucherInfo, identifyModel.S2)
                : string.Empty;
            var s3 = identifyModel.Enable3 == 1 && identifyModel.S3 != null
                ? GetStringByType(identifyModel.S3Type, voucherInfo, identifyModel.S3)
                : string.Empty;

            var separator = identifyModel.IsSeparated == 1 ? identifyModel.Separator : string.Empty;
            var idTemp = string.Format("{0}{1}{0}{2}{0}{3}",
                identifyModel.Separator,
                s1,
                s2,
                s3);

            var numberLength = identifyModel.Length - idTemp.Length;
            var newId = CreateIdNumber(numberLength, identifyModel.LastKey);
            var id = string.Empty;

            //Gen memberID
            switch (identifyModel.OutputOrder)
            {
                case 0: // NSSS
                    id = string.Format("{0}{1}{2}{3}",
                        newId,
                        !string.IsNullOrEmpty(s1) ? s1 + separator : string.Empty,
                        !string.IsNullOrEmpty(s2) ? s2 + separator : string.Empty,
                        !string.IsNullOrEmpty(s3) ? s3 + separator : string.Empty);
                    break;
                case 1: // SNSS
                    id = String.Format("{0}{1}{2}{3}",
                        !string.IsNullOrEmpty(s1) ? s1 + separator : string.Empty,
                        newId,
                        !string.IsNullOrEmpty(s1) ? s1 + separator : string.Empty,
                        !string.IsNullOrEmpty(s1) ? s1 + separator : string.Empty);
                    break;
                case 2: // SSNS
                    id = string.Format("{0}{1}{2}{3}",
                        !string.IsNullOrEmpty(s1) ? s1 + separator : string.Empty,
                        !string.IsNullOrEmpty(s2) ? s2 + separator : string.Empty,
                        newId,
                        !string.IsNullOrEmpty(s3) ? s3 + separator : string.Empty);
                    break;
                case 3: // SSSN
                    id = string.Format("{0}{1}{2}{3}",
                        !string.IsNullOrEmpty(s1) ? s1 + separator : string.Empty,
                        !string.IsNullOrEmpty(s2) ? s2 + separator : string.Empty,
                        !string.IsNullOrEmpty(s3) ? s3 + separator : string.Empty,
                        newId);
                    break;
            }

            return string.IsNullOrEmpty(id) ? string.Empty : id;
        }

        private AutoGenerateIdentifyModel UpdateLastRecordIdKey(AT1007 voucherType, AT4444 tableKey,
            VoucherInfo voucherInfo, string voucherNo)
        {
            if (voucherType == null)
            {
                throw new ArgumentNullException(nameof(voucherType));
            }

            if (tableKey == null)
            {
                throw new ArgumentNullException(nameof(tableKey));
            }

            if (voucherInfo == null)
            {
                throw new ArgumentNullException(nameof(voucherInfo));
            }

            Checker.NotEmpty(voucherNo, nameof(voucherNo));

            var setup = CreateIdentifyModel(voucherType, tableKey);

            if (!CheckFormatRecordId(setup, voucherNo, voucherInfo))
            {
                return null;
            }

            int newLastKey;

            if (string.IsNullOrEmpty(setup.S1) || string.IsNullOrEmpty(setup.S2) || string.IsNullOrEmpty(setup.S3) ||
                string.IsNullOrEmpty(setup.Separator))
            {
                var listS = new List<string>
                {
                    setup.S1,
                    setup.S2,
                    setup.S3
                };

                var separator = setup.IsSeparated == 1 ? setup.Separator : string.Empty;
                var idTemp = string.Empty;

                foreach (var item in listS)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        idTemp = idTemp + item + separator;
                    }
                }

                var numberLength = setup.Length - idTemp.Length;
                var newId = CreateIdNumber(numberLength, setup.LastKey);

                newLastKey = int.Parse(newId);
            }
            else
            {
                var parts = voucherNo.Split(setup.Separator.ToCharArray()).ToList();
                newLastKey = int.Parse(parts[setup.OutputOrder]);
            }

            if (setup.LastKey <= newLastKey)
            {
                //update key
                setup.LastKey = newLastKey;
                return setup;
            }

            return null;
        }

        /// <summary>
        /// Tạo identify model
        /// </summary>
        /// <param name="voucherType"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        private static AutoGenerateIdentifyModel CreateIdentifyModel(AT1007 voucherType, AT4444 tableKey)
        {
            return new AutoGenerateIdentifyModel
            {
                DivisionID = voucherType.DivisionID,
                S2Type = voucherType.S2Type,
                S1Type = voucherType.S1Type,
                S1 = voucherType.S1,
                Enable1 = Convert.ToInt32(voucherType.Enabled1.GetValueOrDefault()),
                Enable2 = Convert.ToInt32(voucherType.Enabled2.GetValueOrDefault()),
                Enable3 = Convert.ToInt32(voucherType.Enabled3.GetValueOrDefault()),
                IsAutomatic = voucherType.Auto,
                IsSeparated = voucherType.Separated,
                KeyString = tableKey.KEYSTRING,
                LastKey = tableKey.LASTKEY.GetValueOrDefault(),
                Length = Convert.ToInt32(voucherType.OutputLength.GetValueOrDefault()),
                OutputOrder = Convert.ToInt32(voucherType.OutputOrder.GetValueOrDefault()),
                S2 = voucherType.S2,
                S3 = voucherType.S3,
                S3Type = voucherType.S3Type,
                Separator = voucherType.Separator,
                TableName = tableKey.TABLENAME
            };
        }

        /// <summary>Create voucherNo Order
        /// </summary>
        /// <param name="orderLength"></param>
        /// <param name="lastKey"></param>
        /// <returns></returns>
        private static string CreateIdNumber(int orderLength, int? lastKey)
        {
            var numberBuilder = new StringBuilder();
            orderLength = orderLength - (lastKey + 1).ToString().Length;
            while (numberBuilder.Length < orderLength)
            {
                numberBuilder.Append("0");
            }

            return $"{numberBuilder}{lastKey + 1}";
        }

        /// <summary>Kiểm tra mã nhận được có đúng mẫu chưa?
        /// </summary>
        /// <param name="setup"></param>
        /// <param name="voucherNo"></param>
        /// <returns>true nếu mã đúng mẫu</returns>
        private static bool CheckFormatRecordId(AutoGenerateIdentifyModel setup, string voucherNo,
            VoucherInfo voucherInfo)
        {
            // Thử xét độ dài chuỗi
            if (voucherNo.Length != setup.Length)
            {
                return false;
            }

            if (setup.Separator == null)
            {
                setup.Separator = string.Empty;
            }

            if (string.IsNullOrEmpty(setup.S1) || string.IsNullOrEmpty(setup.S2) || string.IsNullOrEmpty(setup.S3) ||
                string.IsNullOrEmpty(setup.Separator))
            {
                return true;
            }

            var separator = string.IsNullOrEmpty(setup.Separator) ? string.Empty : setup.Separator;
            var parts = voucherNo.Split(separator.ToCharArray());

            // Nếu mã mới không đủ 4 phần như mẫu
            if (parts.Length < 4)
            {
                return false;
            }

            var s1 = setup.Enable1 == 1 && setup.S1 != null
                ? GetStringByType(setup.S1Type, voucherInfo, setup.S1)
                : string.Empty;
            var s2 = setup.Enable2 == 1 && setup.S2 != null
                ? GetStringByType(setup.S2Type, voucherInfo, setup.S2)
                : string.Empty;
            var s3 = setup.Enable3 == 1 && setup.S3 != null
                ? GetStringByType(setup.S3Type, voucherInfo, setup.S3)
                : string.Empty;

            switch (setup.OutputOrder)
            {
                case 0: // NSSS

                    return s1.Equals(parts[1])
                           && s2.Equals(parts[2])
                           && s3.Equals(parts[3])
                           && int.TryParse(parts[0], out _);
                case 1: // SNSS
                    return s1.Equals(parts[0])
                           && s2.Equals(parts[2])
                           && s3.Equals(parts[3])
                           && int.TryParse(parts[1], out _);
                case 2: // SSNS
                    return s1.Equals(parts[0])
                           && s2.Equals(parts[1])
                           && s3.Equals(parts[3])
                           && int.TryParse(parts[2], out _);
                case 3: // SSSN
                    return s1.Equals(parts[0])
                           && s2.Equals(parts[1])
                           && s3.Equals(parts[2])
                           && int.TryParse(parts[3], out _);
            }

            return false;
        }

        /// <summary>
        /// Lấy thiết lập sinh mã tự động cho các loại chứng từ nghiệp vụ
        /// </summary>
        /// <param name="divisionID"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành] Tạo mới [17/12/2019]
        /// </history>
        public async Task<OOT0060> GetSettingVoucherNoByDivisionID(string divisionID, CancellationToken cancellationToken = default)
        {
            try
            {
                OOT0060 result = new OOT0060();
                result = await _voucherQueries.GetSettingVoucherNoByDivisionID(divisionID, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    
        /// <summary>
        /// Lấy chuổi S theo Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="defaultString"></param>
        /// <param name="voucherTypeID"></param>
        /// <param name="divisionID"></param>
        /// <param name="tranMonth"></param>
        /// <param name="tranYear"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Created [17/12/2020]
        /// </history>
        private async Task<string> GetStringForType(byte? type, string defaultString, string voucherTypeID
            , string divisionID = null, string tranMonth = null, string tranYear = null)
        {
            switch (type)
            {
                case 1: // by month (MM)
                    return tranMonth.PadLeft(2, '0');
                case 2: // by year (yyyy)
                    return tranYear;
                case 3: // by voucherType
                    return voucherTypeID;
                case 4: // by Division
                    return divisionID;
                case 5: // by default string
                    return defaultString;
                case 6: // by short year
                    string yearString = string.Format("{0:0000}", tranYear);
                    return yearString.Substring(yearString.Length - 2, 2);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Create new key
        /// </summary>
        /// <param name="at1007"></param>
        /// <param name="tableName"></param>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Created [17/12/2020]
        /// </history>
        //private async Task<string> CreateKey(AT1007 at1007, string tableName, string moduleID = "")
        //{
        //    string voucherNo = string.Empty;
        //    string voucherNoTemp = string.Empty;
        //    if (at1007.S1.Length + at1007.S2.Length + at1007.S3.Length + (3 * at1007.Separator.Length) < int.Parse(at1007.OutputLength.ToString()))
        //    {
        //        string strIncrement = GetNewKeyString(at1007, tableName, moduleID);
        //        switch (at1007.OutputOrder)
        //        {
        //            case 0: //NSSS
        //                voucherNoTemp = GenerateKeys(strIncrement,
        //                    at1007.S1, at1007.S2, at1007.S3,
        //                    int.Parse(at1007.Separated.ToString()), at1007.separator);

        //                if (at1007.OutputLength >= voucherNoTemp.Length)
        //                {
        //                    voucherNo = voucherNoTemp;
        //                }
        //                break;

        //            case 1:  //SNSS
        //                voucherNoTemp = GenerateKeys(at1007.S1, strIncrement, at1007.S2, at1007.S3,
        //                    int.Parse(at1007.Separated.ToString()), at1007.separator);

        //                if (at1007.OutputLength >= voucherNoTemp.Length)
        //                {
        //                    voucherNo = voucherNoTemp;
        //                }
        //                break;
        //            case 2: //SSNS
        //                voucherNoTemp = GenerateKeys(at1007.S1, at1007.S2, strIncrement, at1007.S3,
        //                    int.Parse(at1007.Separated.ToString()), at1007.separator);

        //                if (at1007.OutputLength >= voucherNoTemp.Length)
        //                {
        //                    voucherNo = voucherNoTemp;
        //                }
        //                break;

        //            case 3: //SSSN
        //                voucherNoTemp = GenerateKeys(at1007.S1, at1007.S2, at1007.S3, strIncrement,
        //                    int.Parse(at1007.Separated.ToString()), at1007.separator);

        //                if (at1007.OutputLength >= voucherNoTemp.Length)
        //                {
        //                    voucherNo = voucherNoTemp;
        //                }
        //                break;
        //        }
        //    }
        //    return voucherNo;
        //}

        ///// <summary>
        ///// Get new key string
        ///// </summary>
        ///// <returns></returns>
        ///// <history>
        /////     [Tấn Thành]  Created [17/12/2020]
        ///// </history>
        //private async Task<string> GetNewKeyString(AT1007 at1007, string tableName, string moduleID = "")
        //{
        //    string strNewKey;
        //    int intZeroLen;
        //    string keyString = await GetKeyString(at1007);
        //    strNewKey = Convert.ToString(GetNewKey(at1007, tableName, moduleID));
        //    intZeroLen = int.Parse(at1007.OutputLength.ToString()) - strNewKey.Length - keyString.Length;
        //    if (at1007.Separated == 1)
        //    {
        //        if (at1007.S1 != "")
        //            intZeroLen -= 1;
        //        if (at1007.S2 != "")
        //            intZeroLen -= 1;
        //        if (at1007.S3 != "")
        //            intZeroLen -= 1;
        //    }
        //    if (intZeroLen < 0)
        //    {
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        string s = new string('0', intZeroLen);
        //        strNewKey = string.Format("{0}{1}", s, strNewKey);
        //        return strNewKey;
        //    }
        //}

        ///// <summary>
        ///// Get KeyString
        ///// </summary>
        ///// <param name="voucherTypeID"></param>
        ///// <param name="moduleID"></param>
        ///// <returns></returns>
        ///// <history>
        /////     [Tấn Thành]  Created [17/12/2020]
        ///// </history>
        //private async Task<string> GetKeyString(AT1007 at1007)
        //{
        //    string keyString = string.Empty;
        //    string S1 = (at1007.Enabled1 == 1) ? at1007.S1 : string.Empty;
        //    string S2 = (at1007.Enabled2 == 1) ? at1007.S2 : string.Empty;
        //    string S3 = (at1007.Enabled3 == 1) ? at1007.S3 : string.Empty;
        //    keyString = string.Format("{0}{1}{2}", S1, S2, S3);
        //    return keyString;
        //}

        ///// <summary>
        ///// Get new key
        ///// </summary>
        ///// <returns></returns>
        ///// <history>
        /////     [Tấn Thành]  Created [17/12/2020]
        ///// </history>
        //private async Task<int> GetNewKey(AT1007 at1007, string tableName, string moduleID = "")
        //{
        //    string keyString = await GetKeyString(at1007);
        //    int lastKey = 0;

        //    switch (moduleID)
        //    {
        //        case ASOFTConstants.MODULE_MT:
        //            MTT4444 mtt4444 = null;
        //            var mtt4444DAL = new MTT4444DAL();
        //            mtt4444 = mtt4444DAL.GetByManyKey(tableName, keyString);

        //            if (mtt4444 == null)
        //            {
        //                mtt4444 = new MTT4444();
        //                mtt4444.TableName = tableName;
        //                mtt4444.KeyString = keyString;
        //                mtt4444.Lastkey = lastKey;
        //                mtt4444DAL.Insert(mtt4444);
        //            }
        //            else
        //            {
        //                lastKey = mtt4444.Lastkey ?? 0;
        //            }
        //            break;
        //        default:
        //            AT4444 at4444 = null;
        //            var at4444DAL = new AT4444DAL();
        //            at4444 = at4444DAL.GetByManyKey(tableName, keyString, IsSpecialAutoGen: at1007.IsSpecialAutoGen);

        //            if (at4444 == null)
        //            {
        //                lastKey = 0;
        //                at4444 = new AT4444();
        //                at4444.TABLENAME = tableName;
        //                at4444.KEYSTRING = keyString;
        //                at4444.LASTKEY = lastKey;
        //                at4444DAL.Insert(at4444);
        //            }
        //            else
        //            {
        //                lastKey = at4444.LASTKEY ?? 0;
        //            }
        //            break;
        //    }
        //    return lastKey + 1;
        //}

        /// <summary>
        /// Lấy voucher
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetVoucherNo(GetVoucherTypeRequest request, CancellationToken cancellationToken)
        {
            var setting = await _Context.QueryFirstOrDefaultAsync(new FilterQuery<OOT0060>(m => m.DivisionID == request.DivisionID));
            var settingCRM = await _CRMContext.QueryFirstOrDefaultAsync(new FilterQuery<CRMT00000>(m => m.DivisionID == request.DivisionID));
            string type = string.Empty;
            switch (request.Type)
            {
                case BusinessConst.TASK:
                    type = setting.VoucherTask;
                    break;
                case BusinessConst.ISSUE:
                    type = setting.VoucherIssues;
                    break;
                case BusinessConst.RELEASE:
                    type = setting.VoucherRelease;
                    break;
                case BusinessConst.MILESTONE:
                    type = setting.VoucherMilestone;
                    break;
                case BusinessConst.REQUEST:
                    type = setting.VoucherRequest;
                    break;
                case BusinessConst.REQUESTSERVICE:
                    type = settingCRM.VoucherRequestService;
                    break;
                case BusinessConst.SUPPORTREQUEST:
                    type = settingCRM.VoucherSupportRequired;
                    break;
                case BusinessConst.CRMT2210:
                    type = settingCRM.VoucherSourceDataOnline;
                    break;
                default:
                    break;
            }
            return type;
        }
    }
}