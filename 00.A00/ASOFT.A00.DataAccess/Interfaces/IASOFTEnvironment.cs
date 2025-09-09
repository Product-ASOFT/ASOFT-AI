// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/01/2021      Tấn Thành       Tạo mới
// #################################################################


namespace ASOFT.A00.DataAccess.Interfaces
{
    public interface IASOFTEnvironment
    {
        /// <summary>
        /// Get tên Db Admin
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        string GetDbAdminName();

        /// <summary>
        /// Get chuỗi kết nối Db Admin
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        string GetConnectionAdmin();

        /// <summary>
        /// Get tên Db Admin
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        string GetDbErpName();

        /// <summary>
        /// Get chuỗi kết nối Db ERP
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        string GetConnectionErp();

        /// <summary>
        /// Set biến môi trường
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [25/01/2021]
        /// </history>
        void SetEnvironment();

    }
}
