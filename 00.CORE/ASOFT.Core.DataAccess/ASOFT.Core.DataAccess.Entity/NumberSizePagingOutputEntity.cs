using Newtonsoft.Json;

namespace ASOFT.Core.DataAccess.Entities
{
    public class NumberSizePagingOutputEntity : BaseEntity
    {
        [JsonIgnore] public int? RowNum { get; set; }

        [JsonIgnore] public int? TotalRow { get; set; }
    }
}