using System;

namespace ASOFT.Core.Business.Common.Entities.ViewModels
{
    [Flags]
    public enum ASOFTPermission
    {
        None = 0,
        AddNew = 1,
        Update = 2,
        Delete = 4,
        Print = 8,
        View = 16,
        Hidden = 64,
        AddDetail = 44, //Hỗ trợ lưu lịch sử
        UpdateDetail = 45, //Hỗ trợ lưu lịch sử
        IsExportExcel = 32,
        UpdateStatus = 128,
        Creation = 256,
    }
}
