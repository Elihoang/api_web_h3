using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_WebH3.Models
{
    public class Email
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string SenderEmail { get; set; }

        [Required]
        [StringLength(255)]
        public string ReceiverEmail { get; set; }

        [Required]
        [StringLength(255)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [StringLength(50)]
        public string SourceType { get; set; } // Giá trị: "Assignment", "PasswordReset", "Contact", "Payment"

        [ForeignKey("Assignment")]
        public string? AssignmentId { get; set; }

        public Assignment? Assignment { get; set; } // Nullable để hỗ trợ các loại email khác

        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Sent"; // Sent, Failed, Pending
    }
}