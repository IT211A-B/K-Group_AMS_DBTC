<h1 align="center">
	<img width="500" src="KCORD_logo_transparent.png" alt="KCORD">
	<br>
</h1>

# K-Cord Attendance Management System

The **K-Cord Project** is developed by the K-Group team with a collaboration with the 3rd-year student, focused on building a web-based Attendance Management System (AMS) as a fitting project for 2nd-year students and mobile with the help of 3rd-year students.

The AMS enables teachers to manage attendance and student schedules efficiently, while giving administrators (Department Heads and other higher-ups) a centralized platform to monitor teacher attendance and oversee both teacher and student performance.

## K-Group Team:

The Group Consist of 3 Members: 

- Project Manager: Cassandra Gayle R. Oraiz
- Backend: Carl Joshua P. Santos
- Frontend: Leila G. Bangoy

## Task View

View project tasks on [ClickUp](https://sharing.clickup.com/90161521612/l/h/2kz0q8yc-596/a4e5df5c49ba3a8).

## API set-up

**Install PostGreSQL**

Install [PostGreSQL](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads) if not installed. Click [PostGreSQL](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads) to download

**Set-Up JSON**

- Update appsettings.json by adding connection string (Base the syntax on the Dummy below, you may change the connection string's data on your accord)
```
{
  "ConnectionStrings": {
    "AttendanceDBString": "Host=localhost;Port=5433;Database=AttendanceDB;Username=postgres;Password=1234"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Open Package Manager Console**

- Open the Tools (*Upper Bar*) in visual studio code 2022
- Press NuGet Package Manager
- Open Package Manager Console

**Migrate Code-First Model Into the PostGreSQL DB**

- Add a migration Folder and migration and name InitialMigration for first Migration
```
Add-Migration InitialMigration
```

- Add (*Up in Migration*) Sequence for Student, Teacher, and User (Up, runs when applying migration)
```
migrationBuilder.Sql("CREATE SEQUENCE StudentSeq START WITH 1 INCREMENT BY 1;");

migrationBuilder.Sql("CREATE SEQUENCE TeacherSeq START WITH 1 INCREMENT BY 1;");

migrationBuilder.Sql("CREATE SEQUENCE UserSeq START WITH 1 INCREMENT BY 1;");
```

- Add (*Down in Migration*) Sequence for Student, Teacher, and User (Down, to Roll Back)
```
migrationBuilder.Sql("DROP SEQUENCE StudentSeq;");

migrationBuilder.Sql("DROP SEQUENCE TeacherSeq;");

migrationBuilder.Sql("DROP SEQUENCE UserSeq;");
```

- Update the migration to the PostGreSQL DB through the Package Manager 
Console 
``` 
Update-Database
```

**Check DB**

- Open PostGreSQL by typing **pgAdmin 4**
- Check Database Name if it exist
- Check Table Attributes by opening Schema\Tables\Attribute
