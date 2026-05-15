using System.Text.Json;

namespace Frontend.Services
{
  /// <summary>
  /// In-memory map built when an admin loads users/teachers (email → teacher document series).
  /// Used so teachers can resolve their courses without admin-only API access.
  /// </summary>
  public static class AmsTeacherDirectory
  {
    private static readonly object Lock = new();
    private static readonly Dictionary<string, string> EmailToTeacherDoc = new(StringComparer.OrdinalIgnoreCase);

    public static void Upsert(string email, string teacherDocumentSeries)
    {
      if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(teacherDocumentSeries)) return;
      lock (Lock) { EmailToTeacherDoc[email.Trim()] = teacherDocumentSeries.Trim(); }
    }

    public static string? ResolveTeacherDocSeries(string? email)
    {
      if (string.IsNullOrWhiteSpace(email)) return null;
      lock (Lock) { return EmailToTeacherDoc.TryGetValue(email.Trim(), out var v) ? v : null; }
    }

    public static void MergeTeachersWithUsers(string teachersJson, string usersJson)
    {
      var teachers = AmsDataHelper.ToElementList(teachersJson);
      var users = AmsDataHelper.ToElementList(usersJson);
      foreach (var t in teachers)
      {
        var userDoc = AmsDataHelper.GetString(t, "userDocumentSeries", "UserDocumentSeries");
        var teacherDoc = AmsDataHelper.GetString(t, "documentSeries", "DocumentSeries");
        if (string.IsNullOrEmpty(userDoc) || string.IsNullOrEmpty(teacherDoc)) continue;
        var user = users.FirstOrDefault(u =>
          string.Equals(AmsDataHelper.GetString(u, "documentSeries", "DocumentSeries"), userDoc, StringComparison.OrdinalIgnoreCase));
        var email = user.ValueKind != JsonValueKind.Undefined
          ? AmsDataHelper.GetString(user, "email", "Email")
          : null;
        if (!string.IsNullOrEmpty(email)) Upsert(email, teacherDoc);
      }
    }
  }
}
