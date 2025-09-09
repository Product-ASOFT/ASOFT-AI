using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.Notification.Model
{
    public class TokenInfo
    {
        public string NotifyToken { get; set; }
        public int UnReadChat { get; set; }
        public int UnReadNoti { get; set; }
    }
}
