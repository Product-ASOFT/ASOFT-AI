// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    29/05/2024      Minh Nhựt       Tạo mới
// ##################################################################

using System;
using System.Collections.Generic;

namespace ASOFT.A00.Entities.QueryParams
{
    public class CalendarQueryParams
    {
        public string DivisionID {  get; set; }
        public int IsDate {  get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate {  get; set; }
        public int TranMonth { get; set; }
        public int TranYear {  get; set; }
        public string? UserID {  get; set; }
        public List<string> CalendarBusiness {  get; set; }
    }
}
