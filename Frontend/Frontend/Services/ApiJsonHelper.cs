using System.Text.Json;

namespace Frontend.Services
{
    public static class ApiJsonHelper
    {
        public static string UnwrapArrayJson(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                    return json;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("$values", out var values))
                        return values.GetRawText();
                    foreach (var key in new[] { "data", "Data" })
                    {
                        if (!root.TryGetProperty(key, out var data)) continue;
                        if (data.ValueKind == JsonValueKind.Array)
                            return data.GetRawText();
                        if (data.ValueKind == JsonValueKind.Object &&
                            data.TryGetProperty("$values", out var inner))
                            return inner.GetRawText();
                    }
                }
                return "[]";
            }
            catch
            {
                return "[]";
            }
        }

        public static JsonElement UnwrapToArrayElement(string json)
        {
            var arrJson = UnwrapArrayJson(json);
            using var doc = JsonDocument.Parse(arrJson);
            return doc.RootElement.Clone();
        }

        public static string UnwrapObjectJson(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    foreach (var key in new[] { "data", "Data" })
                    {
                        if (root.TryGetProperty(key, out var data) &&
                            data.ValueKind == JsonValueKind.Object)
                            return data.GetRawText();
                    }
                }
                return json;
            }
            catch
            {
                return json;
            }
        }
    }
}
