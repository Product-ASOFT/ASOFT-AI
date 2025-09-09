using System.ComponentModel.DataAnnotations;

namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// Number size paging view model
    /// </summary>
    public class NumberSizePagingViewModel : BaseViewModel
    {
        [Required] [Range(1, int.MaxValue)] public int PageNumber { get; set; }

        [Required] [Range(1, 100)] public int PageSize { get; set; }
    }
}