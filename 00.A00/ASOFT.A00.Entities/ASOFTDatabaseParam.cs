using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.A00.Entities
{
    public class ASOFTDatabaseParam
    {
        public string ComboBoxID { get; set; }

        public string ColumnName { get; set; }

        public object Value { get; set; }

        public DbType ValueType { get; set; }

        public bool UseInList { get; set; }
    }
}
