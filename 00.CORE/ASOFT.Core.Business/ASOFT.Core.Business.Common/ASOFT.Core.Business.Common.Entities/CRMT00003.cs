using ASOFT.Core.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASOFT.Core.Business.Common.Entities
{
    public class CRMT00003 : HistoryEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryID { get; set; }
    }
}
