//####################################################################
//# Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved. 
//#
//# History:
//#     Date Time       Created         Comment
//#     02/11/2020      Đoàn Duy         Tạo mới
//####################################################################

using Newtonsoft.Json;
using System;

namespace ASOFT.Core.Business.Common.Entities.ViewModels
{
    public class FollowerModel
    {
        public Guid APK { get; set; }
        public Guid APKMaster { get; set; }
        public string DivisionID { get; set; }
        public string TableID { get; set; }
        public string TypeFollow { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public int RelatedToTypeID { get; set; }
        public string RelatedToID { get; set; }
        public string FollowerID01 { get; set; }
        public string FollowerName01 { get; set; }
        public string FollowerID02 { get; set; }
        public string FollowerName02 { get; set; }
        public string FollowerID03 { get; set; }
        public string FollowerName03 { get; set; }
        public string FollowerID04 { get; set; }
        public string FollowerName04 { get; set; }
        public string FollowerID05 { get; set; }
        public string FollowerName05 { get; set; }
        public string FollowerID06 { get; set; }
        public string FollowerName06 { get; set; }
        public string FollowerID07 { get; set; }
        public string FollowerName07 { get; set; }
        public string FollowerID08 { get; set; }
        public string FollowerName08 { get; set; }
        public string FollowerID09 { get; set; }
        public string FollowerName09 { get; set; }
        public string FollowerID10 { get; set; }
        public string FollowerName10 { get; set; }
        public string FollowerID11 { get; set; }
        public string FollowerName11 { get; set; }
        public string FollowerID12 { get; set; }
        public string FollowerName12 { get; set; }
        public string FollowerID13 { get; set; }
        public string FollowerName13 { get; set; }
        public string FollowerID14 { get; set; }
        public string FollowerName14 { get; set; }
        public string FollowerID15 { get; set; }
        public string FollowerName15 { get; set; }
        public string FollowerID16 { get; set; }
        public string FollowerName16 { get; set; }
        public string FollowerID17 { get; set; }
        public string FollowerName17 { get; set; }
        public string FollowerID18 { get; set; }
        public string FollowerName18 { get; set; }
        public string FollowerID19 { get; set; }
        public string FollowerName19 { get; set; }
        public string FollowerID20 { get; set; }
        public string FollowerName20 { get; set; }
        public string FollowerID21 { get; set; }
        public string FollowerName21 { get; set; }
        public string FollowerID22 { get; set; }
        public string FollowerName22 { get; set; }
        public string FollowerID23 { get; set; }
        public string FollowerName23 { get; set; }
        public string FollowerID24 { get; set; }
        public string FollowerName24 { get; set; }
        public string FollowerID25 { get; set; }
        public string FollowerName25 { get; set; }
        public string FollowerID26 { get; set; }
        public string FollowerName26 { get; set; }
        public string FollowerID27 { get; set; }
        public string FollowerName27 { get; set; }
        public string FollowerID28 { get; set; }
        public string FollowerName28 { get; set; }
        public string FollowerID29 { get; set; }
        public string FollowerName29 { get; set; }
        public string FollowerID30 { get; set; }
        public string FollowerName30 { get; set; }
        public string FollowerID31 { get; set; }
        public string FollowerName31 { get; set; }
        public string FollowerID32 { get; set; }
        public string FollowerName32 { get; set; }
        public string FollowerID33 { get; set; }
        public string FollowerName33 { get; set; }
        public string FollowerID34 { get; set; }
        public string FollowerName34 { get; set; }
        public string FollowerID35 { get; set; }
        public string FollowerName35 { get; set; }
        public string FollowerID36 { get; set; }
        public string FollowerName36 { get; set; }
        public string FollowerID37 { get; set; }
        public string FollowerName37 { get; set; }
        public string FollowerID38 { get; set; }
        public string FollowerName38 { get; set; }
        public string FollowerID39 { get; set; }
        public string FollowerName39 { get; set; }
        public string FollowerID40 { get; set; }
        public string FollowerName40 { get; set; }
        public string FollowerID41 { get; set; }
        public string FollowerName41 { get; set; }
        public string FollowerID42 { get; set; }
        public string FollowerName42 { get; set; }
        public string FollowerID43 { get; set; }
        public string FollowerName43 { get; set; }
        public string FollowerID44 { get; set; }
        public string FollowerName44 { get; set; }
        public string FollowerID45 { get; set; }
        public string FollowerName45 { get; set; }
        public string FollowerID46 { get; set; }
        public string FollowerName46 { get; set; }
        public string FollowerID47 { get; set; }
        public string FollowerName47 { get; set; }
        public string FollowerID48 { get; set; }
        public string FollowerName48 { get; set; }
        public string FollowerID49 { get; set; }
        public string FollowerName49 { get; set; }
        public string FollowerID50 { get; set; }
        public string FollowerName50 { get; set; }
        public string HashTags01 { get; set; }
        public string HashTags02 { get; set; }
        public string HashTags03 { get; set; }
        public string HashTags04 { get; set; }
        public string HashTags05 { get; set; }
        public string HashTags06 { get; set; }
        public string HashTags07 { get; set; }
        public string HashTags08 { get; set; }
        public string HashTags09 { get; set; }
        public string HashTags10 { get; set; }
        public string FollowerIdDisplay { get; set; }
        public string FollowerNameDisplay { get; set; }

        [JsonIgnore]
        public const string Column_APK = "@APK";
        [JsonIgnore]
        public const string Column_APKMaster = "@APKMaster";
        [JsonIgnore]
        public const string Column_DivisionID = "@DivisionID";
        [JsonIgnore]
        public const string Column_TableID = "@TableID";
        [JsonIgnore]
        public const string Column_TypeFollow = "@TypeFollow";
        [JsonIgnore]
        public const string Column_CreateDate = "@CreateDate";
        [JsonIgnore]
        public const string Column_CreateUserID = "@CreateUserID";
        [JsonIgnore]
        public const string Column_RelatedToTypeID = "@RelatedToTypeID";
        [JsonIgnore]
        public const string Column_RelatedToID = "@RelatedToID";
        [JsonIgnore]
        public const string Column_FollowerID01 = "@FollowerID01";
        [JsonIgnore]
        public const string Column_FollowerName01 = "@FollowerName01";
        [JsonIgnore]
        public const string Column_FollowerID02 = "@FollowerID02";
        [JsonIgnore]
        public const string Column_FollowerName02 = "@FollowerName02";
        [JsonIgnore]
        public const string Column_FollowerID03 = "@FollowerID03";
        [JsonIgnore]
        public const string Column_FollowerName03 = "@FollowerName03";
        [JsonIgnore]
        public const string Column_FollowerID04 = "@FollowerID04";
        [JsonIgnore]
        public const string Column_FollowerName04 = "@FollowerName04";
        [JsonIgnore]
        public const string Column_FollowerID05 = "@FollowerID05";
        [JsonIgnore]
        public const string Column_FollowerName05 = "@FollowerName05";
        [JsonIgnore]
        public const string Column_FollowerID06 = "@FollowerID06";
        [JsonIgnore]
        public const string Column_FollowerName06 = "@FollowerName06";
        [JsonIgnore]
        public const string Column_FollowerID07 = "@FollowerID07";
        [JsonIgnore]
        public const string Column_FollowerName07 = "@FollowerName07";
        [JsonIgnore]
        public const string Column_FollowerID08 = "@FollowerID08";
        [JsonIgnore]
        public const string Column_FollowerName08 = "@FollowerName08";
        [JsonIgnore]
        public const string Column_FollowerID09 = "@FollowerID09";
        [JsonIgnore]
        public const string Column_FollowerName09 = "@FollowerName09";
        [JsonIgnore]
        public const string Column_FollowerID10 = "@FollowerID10";
        [JsonIgnore]
        public const string Column_FollowerName10 = "@FollowerName10";
        [JsonIgnore]
        public const string Column_FollowerID11 = "@FollowerID11";
        [JsonIgnore]
        public const string Column_FollowerName11 = "@FollowerName11";
        [JsonIgnore]
        public const string Column_FollowerID12 = "@FollowerID12";
        [JsonIgnore]
        public const string Column_FollowerName12 = "@FollowerName12";
        [JsonIgnore]
        public const string Column_FollowerID13 = "@FollowerID13";
        [JsonIgnore]
        public const string Column_FollowerName13 = "@FollowerName13";
        [JsonIgnore]
        public const string Column_FollowerID14 = "@FollowerID14";
        [JsonIgnore]
        public const string Column_FollowerName14 = "@FollowerName14";
        [JsonIgnore]
        public const string Column_FollowerID15 = "@FollowerID15";
        [JsonIgnore]
        public const string Column_FollowerName15 = "@FollowerName15";
        [JsonIgnore]
        public const string Column_FollowerID16 = "@FollowerID16";
        [JsonIgnore]
        public const string Column_FollowerName16 = "@FollowerName16";
        [JsonIgnore]
        public const string Column_FollowerID17 = "@FollowerID17";
        [JsonIgnore]
        public const string Column_FollowerName17 = "@FollowerName17";
        [JsonIgnore]
        public const string Column_FollowerID18 = "@FollowerID18";
        [JsonIgnore]
        public const string Column_FollowerName18 = "@FollowerName18";
        [JsonIgnore]
        public const string Column_FollowerID19 = "@FollowerID19";
        [JsonIgnore]
        public const string Column_FollowerName19 = "@FollowerName19";
        [JsonIgnore]
        public const string Column_FollowerID20 = "@FollowerID20";
        [JsonIgnore]
        public const string Column_FollowerName20 = "@FollowerName20";
        [JsonIgnore]
        public const string Column_HashTags01 = "@HashTags01";
        [JsonIgnore]
        public const string Column_HashTags02 = "@HashTags02";
        [JsonIgnore]
        public const string Column_HashTags03 = "@HashTags03";
        [JsonIgnore]
        public const string Column_HashTags04 = "@HashTags04";
        [JsonIgnore]
        public const string Column_HashTags05 = "@HashTags05";
        [JsonIgnore]
        public const string Column_HashTags06 = "@HashTags06";
        [JsonIgnore]
        public const string Column_HashTags07 = "@HashTags07";
        [JsonIgnore]
        public const string Column_HashTags08 = "@HashTags08";
        [JsonIgnore]
        public const string Column_HashTags09 = "@HashTags09";
        [JsonIgnore]
        public const string Column_HashTags10 = "@HashTags10";
        [JsonIgnore]
        public const string Column_FollowerID21 = "@FollowerID21";
        [JsonIgnore]
        public const string Column_FollowerName21 = "@FollowerName21";
        [JsonIgnore]
        public const string Column_FollowerID22 = "@FollowerID22";
        [JsonIgnore]
        public const string Column_FollowerName22 = "@FollowerName22";
        [JsonIgnore]
        public const string Column_FollowerID23 = "@FollowerID23";
        [JsonIgnore]
        public const string Column_FollowerName23 = "@FollowerName23";
        [JsonIgnore]
        public const string Column_FollowerID24 = "@FollowerID24";
        [JsonIgnore]
        public const string Column_FollowerName24 = "@FollowerName24";
        [JsonIgnore]
        public const string Column_FollowerID25 = "@FollowerID25";
        [JsonIgnore]
        public const string Column_FollowerName25 = "@FollowerName25";
        [JsonIgnore]
        public const string Column_FollowerID26 = "@FollowerID26";
        [JsonIgnore]
        public const string Column_FollowerName26 = "@FollowerName26";
        [JsonIgnore]
        public const string Column_FollowerID27 = "@FollowerID27";
        [JsonIgnore]
        public const string Column_FollowerName27 = "@FollowerName27";
        [JsonIgnore]
        public const string Column_FollowerID28 = "@FollowerID28";
        [JsonIgnore]
        public const string Column_FollowerName28 = "@FollowerName28";
        [JsonIgnore]
        public const string Column_FollowerID29 = "@FollowerID29";
        [JsonIgnore]
        public const string Column_FollowerName29 = "@FollowerName29";
        [JsonIgnore]
        public const string Column_FollowerID30 = "@FollowerID30";
        [JsonIgnore]
        public const string Column_FollowerName30 = "@FollowerName30";
        [JsonIgnore]
        public const string Column_FollowerID31 = "@FollowerID31";
        [JsonIgnore]
        public const string Column_FollowerName31 = "@FollowerName31";
        [JsonIgnore]
        public const string Column_FollowerID32 = "@FollowerID32";
        [JsonIgnore]
        public const string Column_FollowerName32 = "@FollowerName32";
        [JsonIgnore]
        public const string Column_FollowerID33 = "@FollowerID33";
        [JsonIgnore]
        public const string Column_FollowerName33 = "@FollowerName33";
        [JsonIgnore]
        public const string Column_FollowerID34 = "@FollowerID34";
        [JsonIgnore]
        public const string Column_FollowerName34 = "@FollowerName34";
        [JsonIgnore]
        public const string Column_FollowerID35 = "@FollowerID35";
        [JsonIgnore]
        public const string Column_FollowerName35 = "@FollowerName35";
        [JsonIgnore]
        public const string Column_FollowerID36 = "@FollowerID36";
        [JsonIgnore]
        public const string Column_FollowerName36 = "@FollowerName36";
        [JsonIgnore]
        public const string Column_FollowerID37 = "@FollowerID37";
        [JsonIgnore]
        public const string Column_FollowerName37 = "@FollowerName37";
        [JsonIgnore]
        public const string Column_FollowerID38 = "@FollowerID38";
        [JsonIgnore]
        public const string Column_FollowerName38 = "@FollowerName38";
        [JsonIgnore]
        public const string Column_FollowerID39 = "@FollowerID39";
        [JsonIgnore]
        public const string Column_FollowerName39 = "@FollowerName39";
        [JsonIgnore]
        public const string Column_FollowerID40 = "@FollowerID40";
        [JsonIgnore]
        public const string Column_FollowerName40 = "@FollowerName40";
        [JsonIgnore]
        public const string Column_FollowerID41 = "@FollowerID41";
        [JsonIgnore]
        public const string Column_FollowerName41 = "@FollowerName41";
        [JsonIgnore]
        public const string Column_FollowerID42 = "@FollowerID42";
        [JsonIgnore]
        public const string Column_FollowerName42 = "@FollowerName42";
        [JsonIgnore]
        public const string Column_FollowerID43 = "@FollowerID43";
        [JsonIgnore]
        public const string Column_FollowerName43 = "@FollowerName43";
        [JsonIgnore]
        public const string Column_FollowerID44 = "@FollowerID44";
        [JsonIgnore]
        public const string Column_FollowerName44 = "@FollowerName44";
        [JsonIgnore]
        public const string Column_FollowerID45 = "@FollowerID45";
        [JsonIgnore]
        public const string Column_FollowerName45 = "@FollowerName45";
        [JsonIgnore]
        public const string Column_FollowerID46 = "@FollowerID46";
        [JsonIgnore]
        public const string Column_FollowerName46 = "@FollowerName46";
        [JsonIgnore]
        public const string Column_FollowerID47 = "@FollowerID47";
        [JsonIgnore]
        public const string Column_FollowerName47 = "@FollowerName47";
        [JsonIgnore]
        public const string Column_FollowerID48 = "@FollowerID48";
        [JsonIgnore]
        public const string Column_FollowerName48 = "@FollowerName48";
        [JsonIgnore]
        public const string Column_FollowerID49 = "@FollowerID49";
        [JsonIgnore]
        public const string Column_FollowerName49 = "@FollowerName49";
        [JsonIgnore]
        public const string Column_FollowerID50 = "@FollowerID50";
        [JsonIgnore]
        public const string Column_FollowerName50 = "@FollowerName50";
    }
}
