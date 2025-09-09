// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Files.DataAccess.Interfaces;
using ASOFT.Core.Business.Files.Entities;
using ASOFT.Core.Business.Files.Entities.ViewModels;
using ASOFT.Core.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Files.DataAccess.Queries
{
    public class FileQueries : BusinessDataAccess, IFileQueries
    {
        public FileQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        { }

        private static readonly string SqlGetAttachList = @"
            SELECT count(*) OVER() as TotalRow,  ROW_NUMBER() OVER (ORDER BY M.CreateDate DESC,M.AttachName) AS RowNum, M.APK, M.DivisionID, M.AttachID, 
            M.AttachName, M.CreateDate,M.CreateUserID from CRMT00002 M
            inner join CRMT00002_REL R on M.AttachID = R.AttachID where R.RelatedToID = @APK ORDER BY M.CreateDate DESC,M.AttachName,M.DivisionID
            OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY ";

        /// <summary>
        /// Lấy danh sách file đính kèm của nghiệp vụ.
        /// </summary>
        /// <param name="apk"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(int TotalRow, IEnumerable<FileViewModel>)> GetAttachList(Guid apk,string divisionID, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@PageNumber", page, DbType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("@PageSize", pageSize, DbType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("@APK", apk.ToString(), DbType.String, ParameterDirection.Input);
            return await UseConnectionAsync(async connection =>
            {
                var list = await connection.QueryAsync<FileViewModel>(SqlGetAttachList, dynamicParameters);
                foreach (var item in list)
                {
                    var fileExtention = Path.GetExtension(item.AttachName);
                    item.Path = $"api/v2/core/common/files/getFile/{divisionID}/{item.APK}{fileExtention}";
                    item.ContentType = MimeTypes.GetMimeType(item.AttachName);
                }
                return (list.FirstOrDefault()?.TotalRow ?? 0, list);
            }, cancellationToken);
        }

    }
}
