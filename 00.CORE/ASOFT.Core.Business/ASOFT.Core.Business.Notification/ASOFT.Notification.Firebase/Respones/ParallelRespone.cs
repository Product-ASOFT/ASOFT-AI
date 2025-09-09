using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASOFT.Notification.Firebase.Respones
{
    public class ParallelRespone
    {
        public List<string> TokenList { get; set; }
        public BatchResponse FcmRespones { get; set; }
    }
}
