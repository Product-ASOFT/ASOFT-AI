// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Queries.ViewModels;
using ASOFT.Core.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.DataAccess.Queries
{
    public class CommentQueries : BusinessDataAccess, ICommentQueries
    {
        private static readonly string SqlGetCommentList = @"
            SELECT COUNT(*) OVER () AS TotalRow, A.APK, A.NotesID, B.NotesSubject, 
            B.Description, B.CreateDate, B.CreateUserID, C.FullName AS CreateUserName
            FROM CRMT90031_REL A WITH (NOLOCK) 
                INNER JOIN CRMT90031 B WITH (NOLOCK) ON A.NotesID = B.NotesID
                LEFT JOIN AT1103 C WITH (NOLOCK) ON B.CreateUserID = C.EmployeeID
            WHERE A.RelatedToID = CONVERT(NVARCHAR(50),@APK) AND A.RelatedToTypeID_REL = 47
            ORDER BY A.NotesID DESC
            OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY ";

        public CommentQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        { }

        /// <summary>
        /// Lấy danh sách ghi chú
        /// </summary>
        /// <param name="apk"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(int TotalRow, IEnumerable<CRMT90031ViewModel>)> GetCommentList(Guid apk, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@PageNumber", page, DbType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("@PageSize", pageSize, DbType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("@APK", apk, DbType.Guid, ParameterDirection.Input);
            return await UseConnectionAsync(async connection =>
            {
                var comments = await connection.QueryAsync<CRMT90031ViewModel>(SqlGetCommentList, dynamicParameters);
                return (comments.FirstOrDefault()?.TotalRow ?? 0, comments);
            }, cancellationToken);
        }
    }
}
