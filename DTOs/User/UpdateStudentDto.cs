namespace API_WebH3.DTOs.User
{
   public class UpdateStudentDto
   {
      public string FullName { get; set; }
      public string Email { get; set; }
      public string? Password { get; set; }
      public string? BirthDate { get; set; }
   }
}