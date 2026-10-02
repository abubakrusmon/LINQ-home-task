using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Dataflow;

List<Student> students = new List<Student>
{
new Student { Id = 1, Name = "Alice", DateOfBirth = new DateTime(2000, 5, 15) },
new Student { Id = 2, Name = "Bob", DateOfBirth = new DateTime(1999, 8, 25) },
new Student { Id = 3, Name = "Charlie", DateOfBirth = new DateTime(2001, 3, 10) }
};


List<Course> courses = new List<Course>
{
new Course { Id = 101, Title = "Mathematics", Credits = 4 },
new Course { Id = 102, Title = "Computer Science", Credits = 3 },
new Course { Id = 103, Title = "Physics", Credits = 4 }
};


List<Enrollment> enrollments = new List<Enrollment>
{
new Enrollment { Id = 1, StudentId = 1, CourseId = 101, EnrollmentDate = new DateTime(2023, 1, 15) },
new Enrollment { Id = 2, StudentId = 1, CourseId = 102, EnrollmentDate = new DateTime(2023, 1, 20) },
new Enrollment { Id = 3, StudentId = 2, CourseId = 101, EnrollmentDate = new DateTime(2023, 1, 18) },
new Enrollment { Id = 4, StudentId = 3, CourseId = 103, EnrollmentDate = new DateTime(2023, 1, 22) },
new Enrollment { Id = 5, StudentId = 3, CourseId = 101, EnrollmentDate = new DateTime(2023, 1, 25) },
new Enrollment { Id = 6, StudentId = 3, CourseId = 102, EnrollmentDate = new DateTime(2023, 1, 30) }
};

// 1 Task
var m = courses.First(s => s.Title == "Mathematics").Id;
var task1 = students.Join(enrollments, s => s.Id , e => e.StudentId , (s,e) => new {s,e})
.Where(x => x.e.CourseId == m)
.Select(x => x.s);
foreach(var a in task1)
{
    System.Console.WriteLine(a.Name);
}

System.Console.WriteLine("----------------------------------------------------------");

// 2 Task
var m1 = students.First(s => s.Name == "Charlie").Id;
var task2 = courses.Join(enrollments,s => s.Id , e =>e.CourseId,(s,e) => new {s,e})
.Where(x => x.e.StudentId == m1)
.Select(x => x.s);
foreach(var a in task2)
{
    System.Console.WriteLine(a.Title);
}

System.Console.WriteLine("----------------------------------------------------------");

// 3 Task
var task3 = students
    .SelectMany(c => enrollments.Where(e => e.StudentId == c.Id), (c, e) => c)
    .GroupBy(s => s.Name)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key);

foreach (var a in task3)
{
    System.Console.WriteLine(a);
}

System.Console.WriteLine("----------------------------------------------------------");

// 4 Task
var rows = students
    .Join(enrollments, student => student.Id, enrollment => enrollment.StudentId,
    (student, enrollment) => new { student, enrollment })
    .Join(courses, pair => pair.enrollment.CourseId, course => course.Id,
        (pair, course) => new
        {
            StudentName = pair.student.Name,
            Age = DateTime.Now.Year - pair.student.DateOfBirth.Year,
            CourseTitle = course.Title
        });

var byRange = rows.GroupBy(row => row.Age < 26 ? "26" : "26+");

foreach (var rangeGroup in byRange)
{
    Console.WriteLine($"Диапазон: {rangeGroup.Key}");

    foreach (var courseGroup in rangeGroup.GroupBy(row => row.CourseTitle))
    {
        double avgAge = courseGroup.Average(row => row.Age);
        Console.WriteLine($"  {courseGroup.Key} - avarage {avgAge}");
    }
}

System.Console.WriteLine("----------------------------------------------------------");

// Task 5
