using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class DynamicConfigService
    {
        private readonly IONT1041Queries _ont1041Queries;
        public DynamicConfigService(IONT1041Queries ont1041Queries)
        {
            _ont1041Queries = ont1041Queries;
        }
        public async Task<List<ONT1041>> ConvertDynamicFieldConfig()
        {
            var dataFields = await _ont1041Queries.GetAllByParameterRoleAsync((int)ParameterRole.SaveData);
            if (dataFields == null || !dataFields.Any())
                return new List<ONT1041>();
            return dataFields.ToList();
        }
        public BEMT2005 BuildMaster(AiSection section, Dictionary<string, ONT1041> configMap, Guid apkMaster, string userID)
        {
            var fields = section.Master.DynamicFields;
            var master = new BEMT2005
            {
                APK = Guid.NewGuid(),
                APKMaster = apkMaster,
                SectionOrder = DynamicFieldHelper.GetInt(fields, "SectionOrder"),
                SectionTitle = DynamicFieldHelper.GetString(fields, "SectionTitle"),
                Signature = DynamicFieldHelper.GetString(fields, "Signature"),
                TotalAmount = DynamicFieldHelper.GetDecimalNullable(fields, "TotalAmount"),
                TotalCurrency = DynamicFieldHelper.GetString(fields, "TotalCurrency"),
                SectionType = DynamicFieldHelper.GetString(fields, "SectionType"),
                CreateUserID = userID,
                CreateDate = DateTime.Now,
                LastModifyUserID = userID,
                LastModifyDate = DateTime.Now,
            };
            return master;
        }
        public BuildDetailResult BuildDetails(AiSection section, BEMT2005 bmt2005, Dictionary<string, ONT1041> configMap, Guid apkMaster, Guid apkMasterBEMT2003, string userID, string fileName)
        {
            var result = new BuildDetailResult();

            var masterDict = new Dictionary<string, object?>();

            if (section.Master?.DynamicFields != null)
            {
                foreach (var m in section.Master.DynamicFields)
                {
                    masterDict[m.Key] = ConvertJToken(m.Value);
                }
            }

            foreach (var detail in section.Details)
            {
                #region Hàm tạo BEMT2006 từ AiSectionDetail và ánh xạ các trường động dựa trên configMap
                var entity = new BEMT2006
                {
                    APK = Guid.NewGuid(),
                    APKMaster = apkMaster,
                    APKMaster_BEMT2003 = apkMasterBEMT2003,
                    CreateUserID = userID,
                    CreateDate = DateTime.Now,
                    LastModifyUserID = userID,
                    LastModifyDate = DateTime.Now
                };

                ApplyDynamicFields(entity, detail.DynamicFields, configMap, fileName);
                string orderNo = detail.DynamicFields["OrderNo"]?.Value<string>() ?? string.Empty;
                entity.OrderNo = orderNo;

                result.Entities.Add(entity);
                #endregion

                #region hàm xử lý để tạo một dictionary kết hợp các trường động của master và detail, sau đó thêm vào result.Rows
                var detailDict = new Dictionary<string, object?>();
                foreach (var d in detail.DynamicFields)
                {
                    detailDict[d.Key] = ConvertJToken(d.Value);
                }

                var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

                foreach (var m in masterDict)
                    row[m.Key] = m.Value;

                foreach (var d in detailDict)
                    row[d.Key] = d.Value;

                row["OrderNo"] = orderNo;
                row["FileName"] = fileName;

                result.Rows.Add(row);
                #endregion
            }
            return result;
        }
        private static string? ConvertJToken(JToken? token)
        {
            if (token == null || token.Type == JTokenType.Null)
                return null;

            switch (token.Type)
            {
                case JTokenType.Date:
                    return token.Value<DateTime>().ToString("yyyy-MM-dd HH:mm:ss");
                default:
                    return token.ToString();
            }
        }

        public void ApplyDynamicFields(object entity, IDictionary<string, JToken> dynamicFields, Dictionary<string, ONT1041> configMap, string fileName)
        {
            if (entity == null || dynamicFields == null || configMap == null)
                return;

            foreach (var item in dynamicFields)
            {
                if (!configMap.TryGetValue(item.Key, out var config))
                    continue;

                var value = ConvertJToken(item.Value);
                if (item.Key == "FileName")
                {
                    value = fileName;
                }
                SetProperty(entity, config.ParameterID!, value);
            }
        }
        private static void SetProperty(object entity, string propertyName, object? value)
        {
            var prop = entity.GetType().GetProperty(propertyName);
            if (prop == null || !prop.CanWrite) return;

            prop.SetValue(entity, value);
        }
    }
}
