// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    03/07/2020      Thành Luân      Tạo mới
// ##################################################################
using ASOFT.Notification.Model;
using System;
using System.Collections.Generic;

namespace ASOFT.Core.Business.Notification.Entities
{
    public class OOT9002NotificationRequest
    {
        public Guid APK { get; set; }
        public Guid MessageAPK { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageURL { get; set; }
        public List<string> UserIDList { get; set; }
        public List<TokenInfo> RegistrationTokens { get; set; }
        public List<string> PlatformList { get; set; }
        public string CreateUserID { get; set; }
        public string CommandType { get; set; }
        public string ExtraData1 { get; set; }
        public string ExtraData2 { get; set; }
        public string ExtraData3 { get; set; }

    }
}