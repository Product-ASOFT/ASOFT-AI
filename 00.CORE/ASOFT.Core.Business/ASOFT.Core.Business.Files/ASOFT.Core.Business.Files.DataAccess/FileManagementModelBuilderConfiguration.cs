// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Đoàn Duy      Tạo mới
// ##################################################################

using ASOFT.Core.DataAccess;
using ASOFT.Core.DataAccess.ModelBuilderConfiguration;
using Microsoft.EntityFrameworkCore;

namespace ASOFT.Core.Business.Files.DataAccess
{
    /// <summary>
    /// Class config entity
    /// </summary>
    class FileManagementModelBuilderConfiguration : IModelBuilderConfiguration<BusinessDbContext>
    {
        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileManagementModelBuilderConfiguration).Assembly);
        }
    }
}
