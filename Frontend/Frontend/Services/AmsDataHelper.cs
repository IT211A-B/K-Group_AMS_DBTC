using System.Globalization;
using System.Text.Json;

namespace Frontend.Services
{
  public static class AmsDataHelper
  {
    public static int? ParseDocSeriesNumericId(string? documentSeries)
    {
      if (string.IsNullOrWhiteSpace(documentSeries)) return null;
      var parts = documentSeries.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
      if (parts.Length < 3) return null;
      return int.TryParse(parts[^1], out var id) ? id : null;
    }

    public static string ComputeAttendanceStatus(DateTime scanTime, DateTime classStart)
    {
      var diffMins = (scanTime - classStart).TotalMinutes;
      if (diffMins < -30) return "Absent";
      if (diffMins >= -30 && diffMins < 0) return "Present";
      if (diffMins >= 0 && diffMins <= 15) return "Late";
      return "Absent";
    }

    public static string ComputeAttendanceStatus(TimeOnly scanTime, TimeOnly classStart, DateOnly? scanDate = null, DateOnly? classDate = null)
    {
      var d = scanDate ?? DateOnly.FromDateTime(DateTime.Now);
      var cd = classDate ?? d;
      return ComputeAttendanceStatus(
        cd.ToDateTime(scanTime),
        cd.ToDateTime(classStart));
    }

    public static string? GetString(JsonElement el, params string[] names)
    {
      foreach (var name in names)
      {
        if (!el.TryGetProperty(name, out var prop)) continue;
        return prop.ValueKind switch
        {
          JsonValueKind.String => prop.GetString(),
          JsonValueKind.Number => prop.GetRawText(),
          _ => null
        };
      }
      return null;
    }

    public static int GetInt(JsonElement el, params string[] names)
    {
      foreach (var name in names)
      {
        if (!el.TryGetProperty(name, out var prop)) continue;
        if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var n)) return n;
        if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out var s)) return s;
      }
      return 0;
    }

    public static bool TryParseTime(string? value, out TimeOnly time)
    {
      time = default;
      if (string.IsNullOrWhiteSpace(value)) return false;
      if (TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out time)) return true;
      if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
      {
        time = TimeOnly.FromDateTime(dt);
        return true;
      }
      return false;
    }

    public static DayOfWeek? ParseDayOfWeek(string? value)
    {
      if (string.IsNullOrWhiteSpace(value)) return null;
      if (Enum.TryParse<DayOfWeek>(value, true, out var d)) return d;
      return int.TryParse(value, out var n) && n >= 0 && n <= 6 ? (DayOfWeek)n : null;
    }

    public static List<JsonElement> ToElementList(string json)
    {
      var list = new List<JsonElement>();
      try
      {
        using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "[]" : json);
        var root = doc.RootElement;
        if (root.ValueKind == JsonValueKind.Array)
        {
          foreach (var item in root.EnumerateArray()) list.Add(item.Clone());
          return list;
        }
        if (root.TryGetProperty("data", out var data) || root.TryGetProperty("Data", out data))
        {
          if (data.ValueKind == JsonValueKind.Array)
          {
            foreach (var item in data.EnumerateArray()) list.Add(item.Clone());
            return list;
          }
          if (data.TryGetProperty("$values", out var values) && values.ValueKind == JsonValueKind.Array)
          {
            foreach (var item in values.EnumerateArray()) list.Add(item.Clone());
          }
        }
      }
      catch { }
      return list;
    }
  }
}
