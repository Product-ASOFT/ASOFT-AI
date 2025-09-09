using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ASOFT.CoreAI.Business
{
    public static class BuildQueryPrompt
    {
        public static List<T> QueryByFilters<T>(List<T> itemList, Dictionary<string, string> filters)
        {
            if (filters == null || filters.Count == 0)
                return itemList;

            var query = itemList.AsQueryable();

            var stringProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            string whereClause = "";
            var parameters = new List<object>();
            int paramIndex = 0;

            foreach (var filter in filters)
            {
                if (!stringProps.Contains(filter.Key))
                    continue;

                if (string.IsNullOrEmpty(filter.Value))
                    continue;

                if (!string.IsNullOrEmpty(whereClause))
                    whereClause += " AND ";

                // dùng IndexOf với StringComparison.IgnoreCase để tìm không phân biệt hoa thường
                whereClause += $"({filter.Key} != null AND {filter.Key}.IndexOf(@{paramIndex}, StringComparison.OrdinalIgnoreCase) >= 0)";
                parameters.Add(filter.Value);
                paramIndex++;
            }

            if (string.IsNullOrEmpty(whereClause))
                return itemList;

            try
            {
                return query.Where(whereClause, parameters.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi khi lọc dữ liệu với điều kiện động.", ex);
            }
        }

        public static Dictionary<string, string> ExtractConditionsFromResponse(string aiResponse)
        {
            // Regex để lấy đoạn JSON dạng mảng bắt đầu bằng [ và kết thúc bằng ]
            if (string.IsNullOrWhiteSpace(aiResponse))
                return new Dictionary<string, string>();

            // Regex lấy đoạn JSON mảng từ chuỗi trả về
            var match = Regex.Match(aiResponse, @"\[\s*(\{.*?\}\s*,?\s*)+\]", RegexOptions.Singleline);
            if (!match.Success)
                return new Dictionary<string, string>();

            string jsonArray = match.Value.Trim();

            try
            {
                var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonArray);
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                if (list == null)
                    return dict;

                foreach (var item in list)
                {
                    if (item == null)
                        continue;

                    foreach (var kvp in item)
                    {
                        if (!string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
                        {
                            dict[kvp.Key] = kvp.Value;
                        }
                    }
                }

                return dict;
            }
            catch (JsonException)
            {
                // Log hoặc xử lý lỗi nếu cần
                return new Dictionary<string, string>();
            }
        }
    }
}