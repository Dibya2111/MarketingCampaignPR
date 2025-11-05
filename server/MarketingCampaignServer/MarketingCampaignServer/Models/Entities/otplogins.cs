using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingCampaignServer.Models.Entities
{
    [Table("otplogins")]
    public class otplogins
    {
        [Key]
        public long OtpId { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        [Required]
        [StringLength(6)]
        public string OtpCode { get; set; } = null!;
        
        public DateTime? GeneratedAt { get; set; }
        
        [Required]
        public DateTime ExpirationTime { get; set; }
        
        public bool? IsVerified { get; set; }
        
        public int? AttemptCount { get; set; }
        
        public long? CreatedByUserId { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public long? LastModifiedUserId { get; set; }
        
        public DateTime? LastModifiedDate { get; set; }
        
        public bool? IsActive { get; set; }
        
        public bool? IsDeleted { get; set; }

        [ForeignKey("UserId")]
        public virtual users? User { get; set; }
    }
}