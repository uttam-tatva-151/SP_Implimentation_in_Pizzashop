using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("audit_logs")]
    public partial class AuditLog
    {
        [Key]
        [Column("kot_id")]
        public int KotId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("isPrepared")]
        public bool IsPrepared { get; set; }

        [Column("createAt")]
        public TimeOnly CreateAt { get; set; }

        [Column("modifyAt")]
        public TimeOnly? ModifyAt { get; set; }

        [Column("cretaeBy")]
        public int CretaeBy { get; set; }

        [Column("modifyBy")]
        public int? ModifyBy { get; set; }

        [Required]
        [Column("isActive")]
        public bool? IsActive { get; set; }

        [ForeignKey("CretaeBy")]
        [InverseProperty("AuditLogCretaeByNavigations")]
        public virtual Userauthentication CretaeByNavigation { get; set; } = null!;

        [ForeignKey("ModifyBy")]
        [InverseProperty("AuditLogModifyByNavigations")]
        public virtual Userauthentication? ModifyByNavigation { get; set; }
    }
}
