//####################################################################
//# Copyright (C) 2010-2011, ASoft JSC.  All Rights Reserved. 
//#
//# History:
//#     Date Time       Updater         Comment
//#     22/11/2013      Đức Quý         Tạo mới
//#     09/07/2021      Văn Tài         Coding - Bổ sung các bảng kỳ ở các module, tách partial Message cho ASOFTContants.
//####################################################################

using System.Collections.Generic;
namespace ASOFT.A00.Entities
{
    /// <summary>
    /// Chứa các hằng số của hệ thống
    /// </summary>
    public static partial class ASOFTConstants
    {
        #region --- Module ----

        public const string DIVISION_COMMON = "@@@";

        public const string MODULE_00 = "OO";
        public const string MODULE_S = "S";
        public const string MODULE_CI = "CI";
        public const string MODULE_OP = "OP";
        public const string MODULE_WM = "WM";
        public const string MODULE_T = "T";
        public const string MODULE_FA = "FA";
        public const string MODULE_M = "M";
        public const string MODULE_HRM = "HRM";
        public const string MODULE_DM = "DM";
        public const string MODULE_SM = "SM";
        public const string MODULE_CS = "CS";
        public const string MODULE_MT = "MT";
        public const string MODULE_PS = "PS";
        public const string MODULE_BI = "BI";
        public const string MODULE_POS = "POS";
        public const string MODULE_RD = "RD";
        public const string MODULE_CRM = "CRM";
        public const string MODULE_DRM = "DRM";
        public const string MODULE_SO = "SO";
        public const string MODULE_KPI = "KPI";
        public const string MODULE_LM = "LM";
        public const string MODULE_PO = "PO";
        public const string MODULE_PA = "PA";
        public const string MODULE_OO = "OO";
        public const string MODULE_CSM = "CSM";
        public const string MODULE_EDM = "EDM";
        public const string MODULE_FN = "FN";
        public const string MODULE_NM = "NM";
        public const string MODULE_SHM = "SHM";
        public const string MODULE_ADM = "ADM";
        public const string MODULE_BEM = "BEM";
        public const string MODULE_QC = "QC";
        public const string MODULE_F2M = "F2M";

        #endregion

        #region Folder Name

        public const string FOLDER_LANGUAGES = "Languages";
        public const string FOLDER_MESSAGES = "Messages";
        public const string FOLDER_SQL = "SQLQueries";
        public const string FOLDER_CONFIG = "Configs";

        #endregion

        #region File Message

        public const string FILE_MESSAGE = "msg";
        public const string FILE_LANGUAGE = "lang";

        #endregion File Message

        #region File Extension


        public const string JS_FILE_EXTENSION = ".js";
        public const string PROPERTIES_FILE_EXTENSION = ".properties";
        public const string CONFIG_FILE_EXTENSION = ".config";
        public const string EXCEL_EXTENSION = ".xls";

        #endregion

        #region ---- Language ----

        public const string LANGUAGE_VN = "vi-VN";
        public const string LANGUAGE_EN = "en-US";
        public const string LANGUAGE_JP = "ja-JP";
        public const string LANGUAGE_CN = "zh-CN";

        public const string TYPE_LANGUAGE_MESSAGE = "M";

        public const string TYPE_LANGUAGE_FORM = "L";

        public const string FORM_LANGUAGE_COMMON = "A00";

        #endregion

        #region  Charactor Constants

        public const string CHAR_SHARP = "#";
        public const string CHAR_EQUAL = "=";
        public const string CHAR_DOT = ".";
        public const string CHAR_COMMA = ",";
        public const string CHAR_SLASH = "\\";
        public const string CHAR_NUMBER_AND_POINT = "0123456789.";
        public const string CHAR_NUMBER = "0123456789";
        public const string CHAR_NUMBERS = "0123456789.()-";
        public const string CHAR_DASH = "-";
        public const string CHAR_AT = "@";
        public const string CHAR_NUMBER_AND_SYMBOL = "-0123456789.,";
        public const string FORMAT_NUMBER_STYLE_01 = "#,##0";
        public const string FORMAT_NUMBER_STYLE_02 = "#,##0.0";
        public const string FORMAT_NUMBER_STYLE_03 = "#,##0.00";
        public const string FORMAT_NUMBER_STYLE_04 = "#,##0.000";
        public const string FORMAT_NUMBER_STYLE_05 = "#,##0.0000";
        public const string FORMAT_NUMBER_STYLE_06 = "#,##0.00000";
        public const string FORMAT_NUMBER_STYLE_07 = "#,##0.000000";
        public const string FORMAT_NUMBER_STYLE_08 = "#,##0.0000000";
        public const string FORMAT_NUMBER_STYLE_09 = "#,##0.00000000";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_01 = "{0:#,##0}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_02 = "{0:#,##0.0}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_03 = "{0:#,##0.00}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_04 = "{0:#,##0.000}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_05 = "{0:#,##0.0000}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_06 = "{0:#,##0.00000}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_07 = "{0:#,##0.000000}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_08 = "{0:#,##0.0000000}";
        public const string FORMAT_NUMBER_DECIMAL_STYLE_09 = "{0:#,##0.00000000}";

        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_01 = "#:#";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_02 = "#:#.#";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_03 = "#:#.##";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_04 = "#:#.###";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_05 = "#:#.####";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_06 = "#:#.#####";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_07 = "#:#.######";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_08 = "#:#.#######";
        public const string JS_FORMAT_NUMBER_DECIMAL_STYLE_09 = "#:#.########";

        public const string FORMAT_NUMBER_DB_STYLE_01 = "000";
        public const string FORMAT_NUMBER_DB_STYLE_02 = "000.0";
        public const string FORMAT_NUMBER_DB_STYLE_03 = "000.00";
        public const string FORMAT_NUMBER_DB_STYLE_04 = "000.000";
        public const string FORMAT_NUMBER_DB_STYLE_05 = "000.0000";
        public const string FORMAT_NUMBER_DB_STYLE_06 = "000.00000";
        public const string FORMAT_NUMBER_DB_STYLE_07 = "000.000000";
        public const string FORMAT_NUMBER_DB_STYLE_08 = "000.0000000";
        public const string FORMAT_NUMBER_DB_STYLE_09 = "000.00000000";

        public const string FORMAT_SHORT_DATE = "{0:dd/MM/yyyy}";
        public const string FORMAT_LONG_DATE = "{0:dd/MM/yyyy HH:mm:ss}";
        public const string FORMAT_DATE_MILLISECOND = "{0:dd/MM/yyyy HH:mm:ss.fff}";

        #endregion

        #region Period Format Constants

        public const string PERIOD_FORMAT = "{0:00}/{1:0000}";

        #endregion

        #region --- Customize Index ---

        public const string EIS = "EIS";

        /// <summary>
        /// BusinessArea CAFE
        /// </summary>
        public const int BUSSINESSAREA_CAFE = 1;

        /// <summary>
        /// BusinessArea Fashion Store
        /// </summary>
        public const int BUSSINESSAREA_FASHIONSTORE = 2;

        /// <summary>
        /// BusinessArea Mom & Baby Store
        /// </summary>
        public const int BUSSINESSAREA_MOMBABY = 3;

        /// <summary>
        /// BusinessArea Phone & Electric
        /// </summary>
        public const int BUSSINESSAREA_PHONEELECTRIC = 4;

        /// <summary>
        /// BusinessArea Comestic Store
        /// </summary>
        public const int BUSSINESSAREA_COMESTICSTORE = 5;

        /// <summary>
        /// BusinessArea Furniture & Appliances 
        /// </summary>
        public const int BUSSINESSAREA_FURNITUREAPPLIANCES = 6;

        /// <summary>
        /// BusinessArea Flowers & Gift
        /// </summary>
        public const int BUSSINESSAREA_FLOWERSGIFT = 7;

        /// <summary>
        /// BusinessArea Motorbike & Accessories 
        /// </summary>
        public const int BUSSINESSAREA_MOTORBIKEACCESSORIES = 8;

        /// <summary>
        /// BusinessArea Book & Stationery 
        /// </summary>
        public const int BUSSINESSAREA_BOOKSTATIONERY = 9;

        /// <summary>
        /// BusinessArea Mini supermarket
        /// </summary>
        public const int BUSSINESSAREA_MINISUPERMARKET = 10;

        /// <summary>
        /// BusinessArea Agricultural products & Food
        /// </summary>
        public const int BUSSINESSAREA_AGRICULTURALFOOD = 11;

        /// <summary>
        /// BusinessArea Drugstore
        /// </summary>
        public const int BUSSINESSAREA_DRUGSTORE = 12;

        /// <summary>
        /// BusinessArea Store construction materials
        /// </summary>
        public const int BUSSINESSAREA_STORECONSTRUCTIONMATERIAL = 13;

        /// <summary>
        /// BusinessArea Toys shop
        /// </summary>
        public const int BUSSINESSAREA_TOYSSHOP = 14;

        /// <summary>
        /// BusinessArea Other Store
        /// </summary>
        public const int BUSSINESSAREA_OTHERSTORE = 15;

        /// <summary>
        /// Customer index của KINGCOM 
        /// </summary>
        public const int CUSTOMERINDEX_KINGCOM = 25;
        /// <summary>
        /// Customer index của Phúc Long
        /// </summary>
        public const int CUSTOMERINDEX_PHUCLONG = 32;
        /// <summary>
        /// Customer index Xương Rồng
        /// </summary>
        public const int CUSTOMERINDEX_XUONGRONG = 34;
        /// <summary>
        /// Customer index EIS
        /// </summary>
        public const int CUSTOMERINDEX_EIS = 35;

        /// <summary>
        /// Customer index OO
        /// </summary>
        public const int CUSTOMERINDEX_MEIKO = 50;
        /// <summary>
        /// Customer index Hoang Tran
        /// </summary>
        public const int CUSTOMERINDEX_HOANGTRAN = 51;

        /// <summary>
        /// Customer index của TTTMQ3
        /// </summary>
        public const int CUSTOMERINDEX_TTTMQ3 = 56;

        /// <summary>
        /// Customer index của ANGEL
        /// </summary>
        public const int CUSTOMERINDEX_ANGEL = 57;

        /// <summary>
        /// Customer index của An Bình
        /// </summary>
        public const int CUSTOMERINDEX_ANBINH = 64;

        /// <summary>
        /// Customer index của EIMSKIP
        /// </summary>
        public const int CUSTOMERINDEX_EIMSKIP = 70;

        /// <summary>
        /// Customer index của DONGDUONG
        /// </summary>
        public const int CUSTOMERINDEX_DONGDUONG = 73;

        /// <summary>
        /// Customer index của GODREJ
        /// </summary>
        public const int CUSTOMERINDEX_GODREJ = 74;

        /// <summary>
        /// Customer index của MINHSANG
        /// </summary>
        public const int CUSTOMERINDEX_MINHSANG = 79;

        /// <summary>
        /// Customer index của TDCLA
        /// </summary>
        public const int CUSTOMERINDEX_TDCLA = 80;

        /// <summary>
        /// Customer index của GODREJ
        /// </summary>
        public const int CUSTOMERINDEX_NEWTOYO = 81;

        /// <summary>
        /// Customer index của KIM YEN
        /// </summary>
        public const int CUSTOMERINDEX_KIMYEN = 82;

        /// </summary>
        /// Customer index của OKIA
        /// </summary>
        public const int CUSTOMERINDEX_OKIA = 87;

        /// <summary>
        /// Customer Index of Viet First - Viet Nhat
        /// </summary>
        public const int CUSTOMERINDEX_VIETFIRST = 88;

        /// <summary>
        /// Customer Index of Minh Trị
        /// </summary>
        public const int CUSTOMERINDEX_MINHTRI = 89;

        /// <summary>
        /// Customer Index of BLUESKY
        /// </summary>
        public const int CUSTOMERINDEX_BLUESKY = 91;

        /// <summary>
        /// Customer Index of Asoft (yes this is asoft)
        /// </summary>
        public const int CUSTOMERINDEX_ASOFT = 92;

        /// <summary>
        /// Customer Index of Vườn sạch
        /// </summary>
        public const int CUSTOMERINDEX_VUONSACH = 97;

        /// <summary>
        /// Customer Index of ATTOM
        /// </summary>
        public const int CUSTOMERINDEX_ATTOM = 98;

        /// <summary>
        /// Customer Index of Huỳnh Gia
        /// </summary>
        public const int CUSTOMERINDEX_HUYNHGIA = 99;


        /// <summary>
        /// Customer Index of Huỳnh Gia
        /// </summary>
        public const int CUSTOMERINDEX_AIC = 101;

        /// <summary>
        /// Customer Index of Vietnamfood
        /// </summary>
        public const int CUSTOMERINDEX_VNF = 107;

        /// <summary>
        /// Customer Index of Nhân Ngọc
        /// </summary>
        public const int CUSTOMERINDEX_NHANNGOC = 108;

        /// <summary>
        /// Customer Index of Đức Tin
        /// </summary>
        public const int CUSTOMERINDEX_DUCTIN = 114;

        /// <summary>
        /// Customer Index of MTE
        /// </summary>
        public const int CUSTOMERINDEX_MTE = 115;

        /// <summary>
        /// Customer Index of Mai Thư
        /// </summary>
        public const int CUSTOMERINDEX_MAITHU = 117;

        /// <summary>
        /// Customer Index of BKE
        /// </summary>
        public const int CUSTOMERINDEX_BKE = 121;

        /// <summary>
        /// Customer Index of VINAPAPER
        /// </summary>
        public const int CUSTOMERINDEX_VNP = 128;

        /// <summary>
        /// Customer Index of CBD
        /// </summary>
        public const int CUSTOMERINDEX_CBD = 130;

        /// <summary>
        /// Customer Index of NQH - Nguyễn Quang Huy
        /// </summary>
        public const int CUSTOMERINDEX_NQH = 131;

        /// <summary>
        /// Customer Index của GREE
        /// </summary>
        public const int CUSTOMERINDEX_GREE = 162;

        /// <summary>
        /// Customer Index của GREE
        /// </summary>
        public const int CUSTOMERINDEX_NKC = 166;

        /// <summary>
        /// Customer Index của CaiMei
        /// </summary>
        public const int CUSTOMERINDEX_CAIMEI = 167;

        /// <summary>
        /// Customer Index của HỢP THANH PHÁT
        /// </summary>
        public const int CUSTOMERINDEX_HTP = 172;

        /// <summary>
        /// Customer Index của Nguyễn Quốc Dũng
        /// </summary>
        public const int CUSTOMERINDEX_NQD = 178;

        /// <summary>
        /// Customer Index của VIET IN
        /// </summary>
        public const int CUSTOMERINDEX_VIETIN = 184;

        /// <summary>
        /// Customer Index của Nhựa RELIABLE
        /// </summary>
        public const int CUSTOMERINDEX_RELIABLE = 186;

        /// <summary>
        /// Customer Index của SJK
        /// </summary>
        public const int CUSTOMERINDEX_SJK = 187;

        /// <summary>
        /// Customer Index của ESACO
        /// </summary>
        public const int CUSTOMERINDEX_ESACO = 188;

        /// <summary>
        /// Customer Index của THANH LIEM
        /// </summary>
        public const int CUSTOMERINDEX_THANHLIEM = 163;
        #endregion --- Customize Index ---

        #region ---- Regular Expression ----

        public const string EXPRESSION_ID = @"^([a-zA-Z0-9\-_.!@#$%^&*()]+$)";
        public const string EXPRESSION_EMAIL = @"^([a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$)";
        public const string EXPRESSION_NUMBER = @"^[0-9]\d*(\.\d+)?$";

        #endregion ---- Regular Expression ----

        #region ---- Regular PermisstionData ----

        #region Biến phân quyền module CRM
        public const string LEADID = @"LeadID";
        public const string CONTACTID = @"ContactID";
        public const string OBJECTID = @"ObjectID";
        public const string CAMPAINID = @"CampainID";
        public const string OPPORTUNITYID = @"OpportunityID";
        public const string QUOTATIONID = @"QuotationID";
        public const string SORDERID = @"SOrderID";
        public const string GROUPRECEIVERID = @"GroupReceiverID";
        public const string EVENTID = @"EventID";
        public const string REQUESTID = @"RequestID";
        public const string ATTACHID = @"AttachID";
        public const string ASSESSMENTSELFID = @"AssessmentSelfID";
        public const string BONUSFEATUREID = @"BonusFeatureID";
        public const string APPRAISALSELFID = @"AppraisalSelfID";
        public const string LICENSEMANAGEMENT = @"LicenseManagement";
        #endregion Biến phân quyền module CRM

        #region Biến phân quyền module OO
        public const string TASKID = @"TaskID";
        public const string PROJECTID = @"ProjectID";
        public const string ASSESSTASK = @"AssessTask";
        public const string PROJECTQUOTA = @"ProjectQuota";
        public const string INFORM = @"Inform";
        public const string ISSUEMANAGEMENT = @"IssueManagement";
        public const string HELPDESK = @"HelpDesk";
        public const string CALLSHISTORY = @"CallsHistory";
        public const string MILESTONEMANAGEMENT = @"MilestoneManagement";
        public const string RELEASEMANAGEMENT = @"ReleaseManagement";
        #endregion Biến phân quyền module OO

        #region Biến phân quyền module KPI
        public const string CALCULATEEFFECTIVESALARY = @"CalculateEffectiveSalary";
        #endregion Biến phân quyền module KPI

        #region Biến phân quyền module PO-SO-CI
        public const string PURCHASEREQUEST = @"PurchaseRequest";
        public const string PURCHASEORDERS = @"PurchaseOrders";
        public const string SUPPLIERQUOTE = @"SupplierQuote";
        public const string QUOTATION = @"Quotation";
        public const string SALEORDER = @"SaleOrder";
        public const string CONTRACT = @"Contract";
        public const string QUOTAID = @"QuotaID";
        #endregion Biến phân quyền module PO-SO-CI

        #region Biến phân quyền module HRM
        public const string SABBATICALPROFILEID = @"SabbaticalProfileID";
        public const string RECRUITPLANID = @"RecruitPlanID";
        public const string RECRUITPERIODID = @"RecruitPeriodID";
        public const string INTERVIEWSCHEDULEID = @"InterviewScheduleID";
        public const string INTERVIEWRESULTID = @"InterviewResultID";
        public const string RECDECISIONNO = @"RecDecisionNo";
        public const string COMFIRMATIONRECRUITMENTID = @"ComfirmationRecruitmentID";
        public const string BUDGETID = @"BudgetID";
        public const string TRAININGPLANID = @"TrainingPlanID";
        public const string TRAININGREQUESTID = @"TrainingRequestID";
        public const string TRAININGPROPOSEID = @"TrainingProposeID";
        public const string TRAININGSCHEDULEID = @"TrainingScheduleID";
        public const string TRAININGCOSTID = @"TrainingCostID";
        public const string TRAININGRESULTID = @"TrainingResultID";
        public const string SHIFTID = @"ShiftID";
        public const string SHIFTCHANGEID = @"ShiftChangeID";
        public const string TIMEKEEPINGID = @"TimekeepingID";
        public const string PERMISSIONFORMID = @"PermissionFormID";
        public const string PERMISSIONOUTFORMID = @"PermissionOutFormID";
        public const string OTFORMID = @"OTFormID";
        public const string ABNORMALID = @"AbnormalID";
        public const string PERMISSIONCATALOGID = @"PermissionCatalogID";
        public const string DISCID = @"DISCID";
        public const string EVALUATIONKITID = @"EvaluationKitID";
        #endregion Biến phân quyền module HRM

        #region Biến phân quyền module BEM
        public const string BEMT2000 = @"BEMT2000";
        public const string BEMT2010 = @"BEMT2010";
        #endregion Biến phân quyền module BEM
        #endregion ---- Regular PermisstionData ----

        #region ---- FW new ----

        public const int CONTENTMASTER = 2;
        public const int TYPEINPUT_POPUPMASTERDETAIL = 2;
        public const int TYPEINPUT_VIEWMASTERDETAIL = 3;
        public const int TYPEINPUT_VIEWNODETAIL = 4;
        public const int TYPEINPUT_VIEWMASTERDETAIL2 = 5;
        public const int CHECKBOX = 2;

        public const string ACTIONCONTENTMASTER = "contentmaster";
        public const string ACTIONPOPUPMASTERDETAIL = "popupmasterdetail";
        public const string ACTIONPOPUPLAYOUT = "popuplayout";
        public const string ACTIONVIEWMASTERDETAIL = "viewmasterdetail";
        public const string ACTIONVIEWMASTERDETAIL2 = "viewmasterdetail2";
        public const string ACTIONVIEWNODETAILS = "viewnodetails";

        //type dataType

        public const int PK_APK = 1;
        public const int PK_INT = 2;
        public const int PK_STRING = 3;
        public const int BIGINT = 4;
        public const int INT = 5;
        public const int TINYINT = 6;
        public const int STRING = 7;
        public const int DECIMAL = 8;
        public const int DATE = 9;
        public const int BOOL = 10;
        public const int TIME = 11;
        public const int TEXT = 12;
        public const int DATETIME = 13;
        public const int BINARY = 14;

        //function Common

        public const string TABLEFUNCTION = "sysFunctionCommon";

        public const string FUNCTIONEMAIL = "1";
        public const string FUNCTIONATTACH = "2";
        public const string FUNCTIONNOTE = "3";
        public const string FUNCTIONHISTORY = "4";

        public const string PARTIALEMAIL = "Email";
        public const string PARTIALATTACH = "Attach";
        public const string PARTIALNOTE = "Notes";
        public const string PARTIALHISTORY = "History";

        #endregion ---- FW new ----

        #region --- Lịch sử cuộc gọi ---
        // 11/12/2019 - [Vĩnh Tâm] - Begin add
        // Bổ sung biến constant cho phần Lịch sử cuộc gọi
        public const string ANSWERED = "ANSWERED";
        public const string BUSY = "BUSY";
        public const string MISSED = "MISSED";
        public const string NOANSWER = "NO ANSWER";
        public const string INBOUND = "Inbound";
        public const string LOCAL = "Local";
        public const string OUTBOUND = "Outbound";
        // 11/12/2019 - [Vĩnh Tâm] - End add
        #endregion --- Lịch sử cuộc gọi ---

        #region --- Trạng thái công việc - Các trạng thái mặc định của hệ thống ---
        // 26/03/2020 - [Vĩnh Tâm] - Begin add
        /// <summary>
        /// Chưa thực hiện
        /// </summary>
        public const string STATUS_TASK_UNEXECUTED = "TTCV0001";
        /// <summary>
        /// Đang thực hiện
        /// </summary>
        public const string STATUS_TASK_PROCESSING = "TTCV0002";
        /// <summary>
        /// Hoàn thành
        /// </summary>
        public const string STATUS_TASK_COMPLETED = "TTCV0003";
        /// <summary>
        /// Đóng
        /// </summary>
        public const string STATUS_TASK_CLOSED = "TTCV0004";
        /// <summary>
        /// Từ chối hoàn thành
        /// </summary>
        public const string STATUS_TASK_REJECT = "TTCV0005";
        /// <summary>
        /// Chờ xác nhận
        /// </summary>
        public const string STATUS_TASK_CONFIRM = "TTCV0006";
        /// <summary>
        /// Tạm ngưng
        /// </summary>
        public const string STATUS_TASK_PENDING = "TTCV0007";
        // 26/03/2020 - [Vĩnh Tâm] - End add
        #endregion --- Trạng thái công việc - Các trạng thái mặc định của hệ thống ---

        #region --- Tên các chuỗi kết nối DB ---
        // 16/04/2020 - [Vĩnh Tâm] - Begin add
        public const string DB_ADMIN = "ASOFT_ADMIN";
        public const string DB_ERP = "ASOFT_ERP";
        public const string DB_ADMIN_STD = "ASOFT_ADMIN_STD";
        public const string DB_ERP_STD = "ASOFT_ERP_STD";
        // 16/04/2020 - [Vĩnh Tâm] - End add
        #endregion --- Tên các chuỗi kết nối DB ---

        #region --- Đường dẫn đến các file hệ thống ---
        // 22/09/2020 - [Đình Ly] - Begin add
        public static readonly string PATH_ATTACHED = @"\Attached\";
        public static readonly string PATH_FILES = @"\Attached\Files\";
        public static readonly string PATH_EContract = @"\Attached\EContract\";
        public static readonly string PATH_CHAT = @"\Attached\Chats\";
        public static readonly string PATH_EMAILS = @"\Attached\Emails\";
        public static readonly string PATH_AVATARS = @"\Attached\Avatars\";
        public static readonly string PATH_CHECKIN = @"\Attached\Checkin\";
        public static readonly string PATH_DELIVERYADDRESS = @"\Attached\DeliveryAddress\"; 
        public static readonly string PATH_LOGO = @"\Content\Images\";
        public static readonly string PATH_PRODUCT = @"\Attached\Products\";
        public static readonly string PATH_NEWSFEED = @"\Attached\Newsfeed\";
        public static readonly string PATH_PRODUCT_REVIEW = @"\Attached\Products\{0}\Reviews";
        // 22/09/2020 - [Đình Ly] - End add

        /// <summary>
        ///     Đường dẫn chứa file đính kèm Văn bản ký số.
        /// </summary>
        /// <history>
        ///     [Văn Tài]   Created    [23/05/2022]
        /// </history>
        public const string PATH_ECONTRACT = @"Attached\EContract\";

        #endregion --- Đường dẫn đến các file hệ thống ---

        #region --- Table kỳ kế toán của các Module ---

        /// <summary>
        /// Kỳ kế toán của CI
        /// </summary>
        public const string TABLE_PERIOD_CT = "CT9999";

        /// <summary>
        /// Kỳ kế toán của MT
        /// </summary>
        public const string TABLE_PERIOD_MT = "MTT9999";

        /// <summary>
        /// Kỳ kế toán của POS
        /// </summary>
        public const string TABLE_PERIOD_POS = "POST9999";

        /// <summary>
        /// Kỳ kế toán của DRM
        /// </summary>
        public const string TABLE_PERIOD_DRM = "DRT9999";

        /// <summary>
        /// Kỳ kế toán của OO
        /// </summary>
        public const string TABLE_PERIOD_HRM = "HT9999";

        /// <summary>
        /// Kỳ kế toán của CRM
        /// </summary>
        public const string TABLE_PERIOD_CRM = "CRMV99999";

        /// <summary>
        /// Kỳ kế toán của SO
        /// </summary>
        public const string TABLE_PERIOD_SO = "OT9999";

        /// <summary>
        /// Kỳ kế toán của SO
        /// </summary>
        public const string TABLE_PERIOD_WM = "WT9999";

        /// <summary>
        /// Kỳ kế toán của LM
        /// </summary>
        public const string TABLE_PERIOD_LM = "LMT9999";

        /// <summary>
        /// Kỳ kế toán của T
        /// </summary>
        public const string TABLE_PERIOD_T = "AT9999";

        /// <summary>
        /// Kỳ kế toán của FN
        /// </summary>
        public const string TABLE_PERIOD_FN = "AT9999";

        /// <summary>
        /// Kỳ kế toán của OO
        /// </summary>
        public const string TABLE_PERIOD_OO = "OOT9999";

        /// <summary>
        /// Kỳ kế toán của BEM
        /// </summary>
        public const string TABLE_PERIOD_BEM = "BEMT9999";

        /// <summary>
        /// Kỳ kế toán của FA
        /// </summary>
        public const string TABLE_PERIOD_FT = "FT9999";

        /// <summary>
        /// Kỳ kế toán của OP
        /// </summary>
        public const string TABLE_PERIOD_OT = "OT9999";

        /// <summary>
        /// Kỳ kế toán của PO
        /// </summary>
        public const string TABLE_PERIOD_PT = "PT9999";

        #endregion --- Table kỳ kế toán của các Module ---
    }
}