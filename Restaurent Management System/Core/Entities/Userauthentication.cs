using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    [Table("userauthentication")]
    [Index("EmailId", Name = "userauthentication_email_id_key", IsUnique = true)]
    public partial class Userauthentication
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("user_name")]
        [StringLength(30)]
        public string UserName { get; set; } = null!;

        [Column("email_id")]
        [StringLength(100)]
        public string EmailId { get; set; } = null!;

        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("createat", TypeName = "timestamp without time zone")]
        public DateTime Createat { get; set; }

        [Column("modifyat", TypeName = "timestamp without time zone")]
        public DateTime? Modifyat { get; set; }

        [Required]
        [Column("iscontinued")]
        public bool? Iscontinued { get; set; }

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Category> CategoryCreatebyNavigations { get; set; } = new List<Category>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Category> CategoryModifybyNavigations { get; set; } = new List<Category>();

        [InverseProperty("CreateByNavigation")]
        public virtual ICollection<CityList> CityListCreateByNavigations { get; set; } = new List<CityList>();

        [InverseProperty("ModifyByNavigation")]
        public virtual ICollection<CityList> CityListModifyByNavigations { get; set; } = new List<CityList>();

        [InverseProperty("CreateByNavigation")]
        public virtual ICollection<ContryList> ContryListCreateByNavigations { get; set; } = new List<ContryList>();

        [InverseProperty("ModifyByNavigation")]
        public virtual ICollection<ContryList> ContryListModifyByNavigations { get; set; } = new List<ContryList>();

        [InverseProperty("CretaeByNavigation")]
        public virtual ICollection<ErrorLog> ErrorLogs { get; set; } = new List<ErrorLog>();

        [InverseProperty("User")]
        public virtual ICollection<FavoritesItem> FavoritesItems { get; set; } = new List<FavoritesItem>();

        [InverseProperty("CreateByNavigation")]
        public virtual ICollection<Invoice> InvoiceCreateByNavigations { get; set; } = new List<Invoice>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<InvoiceItemModifierMapping> InvoiceItemModifierMappings { get; set; } = new List<InvoiceItemModifierMapping>();

        [InverseProperty("ModifyByNavigation")]
        public virtual ICollection<Invoice> InvoiceModifyByNavigations { get; set; } = new List<Invoice>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Item> ItemCreatebyNavigations { get; set; } = new List<Item>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Item> ItemModifybyNavigations { get; set; } = new List<Item>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Modifier> ModifierCreatebyNavigations { get; set; } = new List<Modifier>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Modifier> ModifierModifybyNavigations { get; set; } = new List<Modifier>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<ModifiersGroup> ModifiersGroupCreatebyNavigations { get; set; } = new List<ModifiersGroup>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<ModifiersGroup> ModifiersGroupModifybyNavigations { get; set; } = new List<ModifiersGroup>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Module> ModuleCreatebyNavigations { get; set; } = new List<Module>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Module> ModuleModifybyNavigations { get; set; } = new List<Module>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Order> OrderCreatebyNavigations { get; set; } = new List<Order>();

        [InverseProperty("CreatedbyNavigation")]
        public virtual ICollection<OrderDetail> OrderDetailCreatedbyNavigations { get; set; } = new List<OrderDetail>();

        [InverseProperty("ModifiedbyNavigation")]
        public virtual ICollection<OrderDetail> OrderDetailModifiedbyNavigations { get; set; } = new List<OrderDetail>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Order> OrderModifybyNavigations { get; set; } = new List<Order>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Permission> PermissionCreatebyNavigations { get; set; } = new List<Permission>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Permission> PermissionModifybyNavigations { get; set; } = new List<Permission>();

        [InverseProperty("User")]
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        [InverseProperty("User")]
        public virtual ICollection<ResetPasswordToken> ResetPasswordTokens { get; set; } = new List<ResetPasswordToken>();

        [ForeignKey("RoleId")]
        [InverseProperty("Userauthentications")]
        public virtual Role Role { get; set; } = null!;

        [InverseProperty("CreatedbyNavigation")]
        public virtual ICollection<Section> SectionCreatedbyNavigations { get; set; } = new List<Section>();

        [InverseProperty("ModifiedbyNavigation")]
        public virtual ICollection<Section> SectionModifiedbyNavigations { get; set; } = new List<Section>();

        [InverseProperty("CreateByNavigation")]
        public virtual ICollection<StateList> StateListCreateByNavigations { get; set; } = new List<StateList>();

        [InverseProperty("ModifyByNavigation")]
        public virtual ICollection<StateList> StateListModifyByNavigations { get; set; } = new List<StateList>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Table> TableCreatebyNavigations { get; set; } = new List<Table>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Table> TableModifybyNavigations { get; set; } = new List<Table>();

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<Taxis> TaxisCreatebyNavigations { get; set; } = new List<Taxis>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<Taxis> TaxisModifybyNavigations { get; set; } = new List<Taxis>();

        [InverseProperty("CreateByNavigation")]
        public virtual ICollection<Userdetail> UserdetailCreateByNavigations { get; set; } = new List<Userdetail>();

        [InverseProperty("ModifyByNavigation")]
        public virtual ICollection<Userdetail> UserdetailModifyByNavigations { get; set; } = new List<Userdetail>();

        [InverseProperty("User")]
        public virtual Userdetail? Userdetail { get; set; }

        [InverseProperty("CreatebyNavigation")]
        public virtual ICollection<WaitingList> WaitingListCreatebyNavigations { get; set; } = new List<WaitingList>();

        [InverseProperty("ModifybyNavigation")]
        public virtual ICollection<WaitingList> WaitingListModifybyNavigations { get; set; } = new List<WaitingList>();
    }
}
