using System.Text.Json;

namespace Frontend.Services
{
  public static class AmsStudentDirectory
  {
    private static readonly object Lock = new();
    private static readonly Dictionary<string, string> EmailToStudentDoc = new(StringComparer.OrdinalIgnoreCase);

    public static void Upsert(string email, string studentDocumentSeries)
    {
      if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(studentDocumentSeries)) return;
      lock (Lock) { EmailToStudentDoc[email.Trim()] = studentDocumentSeries.Trim(); }
    }

    public static string? ResolveStudentDocByEmail(string? email)
    {
      if (string.IsNullOrWhiteSpace(email)) return null;
      lock (Lock) { return EmailToStudentDoc.TryGetValue(email.Trim(), out var v) ? v : null; }
    }

    public static void MergeUsersAndStudents(string usersJson, string studentsJson)
    {
      var users = AmsDataHelper.ToElementList(usersJson);
      var students = AmsDataHelper.ToElementList(studentsJson);
      foreach (var u in users)
      {
        var email = AmsDataHelper.GetString(u, "email", "Email");
        var userDoc = AmsDataHelper.GetString(u, "documentSeries", "DocumentSeries");
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userDoc)) continue;
        var st = students.FirstOrDefault(s =>
          string.Equals(AmsDataHelper.GetString(s, "userDocumentSeries", "UserDocumentSeries"), userDoc, StringComparison.OrdinalIgnoreCase));
        var stDoc = st.ValueKind != JsonValueKind.Undefined
          ? AmsDataHelper.GetString(st, "documentSeries", "DocumentSeries")
          : null;
        if (!string.IsNullOrEmpty(stDoc)) Upsert(email, stDoc);
      }
    }
  }
}
