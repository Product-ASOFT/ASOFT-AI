namespace ASOFT.Core.Business.Common.Entities.ViewModels
{
    /// <summary>
    /// Kết quả phân quyền dữ liệu
    /// </summary>
    public class PermissionCondition
    {
        /// <summary>
        /// Trả về câu sql query điều kiện permission
        /// </summary>
        public string Condition;

        /// <summary>
        /// Trả về permisson 1 hoặc 0.
        /// </summary>
        public int Permission;
    }
}