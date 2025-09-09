using ASOFT.CoreAI.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace ASOFT.CoreAI.Business
{
    public static class RedisearchResultParser
    {
        /// <summary>
        /// Parse RedisResult của FT.SEARCH thành danh sách đối tượng có Id và các trường dữ liệu.
        /// </summary>
        public static List<RedisearchResultItem> Parse(RedisResult result)
        {
            if (result == null) return new List<RedisearchResultItem>();
            var resultArray = (RedisResult[])result;
            var items = new List<RedisearchResultItem>();

            if (resultArray.Length == 0) return items;

            // Phần tử đầu là tổng số kết quả
            int total = (int)resultArray[0];

            for (int i = 1; i < resultArray.Length; i += 2)
            {
                var id = (string)resultArray[i];
                var fieldsArray = (RedisResult[])resultArray[i + 1];

                // Tạo dictionary tạm để lưu key-value
                var fieldsDict = new Dictionary<string, string>();
                for (int j = 0; j < fieldsArray.Length; j += 2)
                {
                    string key = (string)fieldsArray[j];
                    string value = (string)fieldsArray[j + 1];
                    fieldsDict[key] = value;
                }

                // Map từ dictionary sang đối tượng FieldsData
                var fieldsData = new FieldsData
                {
                    Text = fieldsDict.ContainsKey("Text") ? fieldsDict["Text"] : null,
                    ReferenceDescription = fieldsDict.ContainsKey("ReferenceDescription") ? fieldsDict["ReferenceDescription"] : null,
                    ReferenceLink = fieldsDict.ContainsKey("ReferenceLink") ? fieldsDict["ReferenceLink"] : null
                };

                items.Add(new RedisearchResultItem
                {
                    Id = id,
                    FieldDatas = fieldsData,
                });
            }
            return items;
        }

        /// <summary>
        /// Chuyển RedisResult thành JSON string dễ đọc.
        /// </summary>
        public static string ToJson(RedisResult result, bool writeIndented = true)
        {
            var items = Parse(result);
            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = writeIndented });
        }
    }
}