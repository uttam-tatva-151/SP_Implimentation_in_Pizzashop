using Microsoft.EntityFrameworkCore;

namespace PMSData
{
    public partial class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<CityList> CityLists { get; set; }

        public virtual DbSet<ContryList> ContryLists { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

        public virtual DbSet<FavoritesItem> FavoritesItems { get; set; }

        public virtual DbSet<FeedbackForm> FeedbackForms { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<InvoiceItemModifierMapping> InvoiceItemModifierMappings { get; set; }

        public virtual DbSet<InvoiceTaxesMapping> InvoiceTaxesMappings { get; set; }

        public virtual DbSet<Item> Items { get; set; }

        public virtual DbSet<ItemModifierGroupsMapping> ItemModifierGroupsMappings { get; set; }

        public virtual DbSet<Modifier> Modifiers { get; set; }

        public virtual DbSet<ModifierModifierGroupRelation> ModifierModifierGroupRelations { get; set; }

        public virtual DbSet<ModifiersGroup> ModifiersGroups { get; set; }

        public virtual DbSet<Module> Modules { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }

        public virtual DbSet<Permission> Permissions { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Section> Sections { get; set; }

        public virtual DbSet<StateList> StateLists { get; set; }

        public virtual DbSet<Table> Tables { get; set; }

        public virtual DbSet<Taxis> Taxes { get; set; }

        public virtual DbSet<Userauthentication> Userauthentications { get; set; }

        public virtual DbSet<Userdetail> Userdetails { get; set; }

        public virtual DbSet<WaitingList> WaitingLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("categories_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.CategoryCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_category_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.CategoryModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_category_modifyby");
            });

            modelBuilder.Entity<CityList>(entity =>
            {
                entity.HasKey(e => e.CityId).HasName("city_list_pkey");

                entity.Property(e => e.CretateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.CityListCreateByNavigations)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_city_row_createby");

                entity.HasOne(d => d.ModifyByNavigation).WithMany(p => p.CityListModifyByNavigations).HasConstraintName("fk_city_row_modifyby");

                entity.HasOne(d => d.State).WithMany(p => p.CityLists)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_state_id");
            });

            modelBuilder.Entity<ContryList>(entity =>
            {
                entity.HasKey(e => e.ContryId).HasName("Contry_list_pkey");

                entity.Property(e => e.ContryId).HasDefaultValueSql("nextval('\"Contry_list_contry_id_seq\"'::regclass)");
                entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsContinue).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.ContryListCreateByNavigations)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_contry_row_createby");

                entity.HasOne(d => d.ModifyByNavigation).WithMany(p => p.ContryListModifyByNavigations).HasConstraintName("fk_contry_row_modifyby");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustId).HasName("customers_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.HasKey(e => e.ErrorId).HasName("audit_logs_pkey");

                entity.Property(e => e.ErrorId).HasDefaultValueSql("nextval('audit_logs_kot_id_seq'::regclass)");
                entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CretaeByNavigation).WithMany(p => p.ErrorLogs)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_kot_createby");
            });

            modelBuilder.Entity<FavoritesItem>(entity =>
            {
                entity.HasKey(e => e.FiId).HasName("favorites_items_pkey");

                entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.Item).WithMany(p => p.FavoritesItems).HasConstraintName("favorites_items_item_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.FavoritesItems)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("favorites_items_user_id_fkey");
            });

            modelBuilder.Entity<FeedbackForm>(entity =>
            {
                entity.HasKey(e => e.FeedbackId).HasName("feedback_form_pkey");

                entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId).HasName("invoices_pkey");

                entity.Property(e => e.InvoiceId).HasDefaultValueSql("nextval('invoices_invoice_id_seq'::regclass)");
                entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.InvoiceCreateByNavigations)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_invoice_createby");

                entity.HasOne(d => d.ModifyByNavigation).WithMany(p => p.InvoiceModifyByNavigations).HasConstraintName("fk_invoice_modifyby");

                entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_order_id");
            });

            modelBuilder.Entity<InvoiceItemModifierMapping>(entity =>
            {
                entity.HasKey(e => e.InvoiceItemModifierId).HasName("invoice_item_modifier_mapping_pkey");

                entity.Property(e => e.InvoiceItemModifierId).HasDefaultValueSql("nextval('invoice_item_modifier_mapping_item_modifier_id_seq'::regclass)");
                entity.Property(e => e.ModifiersQuantity).HasDefaultValueSql("0");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.InvoiceItemModifierMappings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Create_by");

                entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItemModifierMappings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Invoice_id");

                entity.HasOne(d => d.Item).WithMany(p => p.InvoiceItemModifierMappings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_items_id");

                entity.HasOne(d => d.Modifier).WithMany(p => p.InvoiceItemModifierMappings).HasConstraintName("fk_modifiers_id");

                entity.HasOne(d => d.Order).WithMany(p => p.InvoiceItemModifierMappings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Order_Id");
            });

            modelBuilder.Entity<InvoiceTaxesMapping>(entity =>
            {
                entity.HasKey(e => e.InvoiceTaxRelationId).HasName("invoice_taxes_mapping_pkey");

                entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceTaxesMappings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_invoice_id");

                entity.HasOne(d => d.Tax).WithMany(p => p.InvoiceTaxesMappings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tax_id");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.ItemId).HasName("items_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsAvailable).HasDefaultValueSql("true");
                entity.Property(e => e.IsDefaultTax).HasDefaultValueSql("true");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.Category).WithMany(p => p.Items).HasConstraintName("fk_items_category");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.ItemCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_items_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.ItemModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_items_modifyby");
            });

            modelBuilder.Entity<ItemModifierGroupsMapping>(entity =>
            {
                entity.HasKey(e => e.ImId).HasName("item_modifiers_pkey");

                entity.Property(e => e.ImId).HasDefaultValueSql("nextval('item_modifiers_im_id_seq'::regclass)");

                entity.HasOne(d => d.Item).WithMany(p => p.ItemModifierGroupsMappings).HasConstraintName("fk_item_modifiers_item");

                entity.HasOne(d => d.Mg).WithMany(p => p.ItemModifierGroupsMappings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_item_modifiers_modifier");
            });

            modelBuilder.Entity<Modifier>(entity =>
            {
                entity.HasKey(e => e.MId).HasName("modifiers_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.ModifierCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_modifiers_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.ModifierModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_modifiers_modifyby");
            });

            modelBuilder.Entity<ModifierModifierGroupRelation>(entity =>
            {
                entity.HasKey(e => e.RelationId).HasName("modifier_modifier_group_relation_pkey");

                entity.HasOne(d => d.Group).WithMany(p => p.ModifierModifierGroupRelations)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_modifier_group_id");

                entity.HasOne(d => d.Modifier).WithMany(p => p.ModifierModifierGroupRelations)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_modifier_id");
            });

            modelBuilder.Entity<ModifiersGroup>(entity =>
            {
                entity.HasKey(e => e.MgId).HasName("modifiers_group_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.ModifiersGroupCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_modifiers_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.ModifiersGroupModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_modifiers_modifyby");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.ModuleId).HasName("module_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.ModuleCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_module_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.ModuleModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_module_modifyby");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("orders_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.OrderCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_orders_createby");

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("fk_orders_customer");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.OrderModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_orders_modifyby");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OdId).HasName("order_details_pkey");

                entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Iscontinued).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.OrderDetailCreatedbyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_created_by");

                entity.HasOne(d => d.Feedback).WithMany(p => p.OrderDetails)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_feedback");

                entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.OrderDetailModifiedbyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_modified_by");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasConstraintName("fk_order");

                entity.HasOne(d => d.Payment).WithMany(p => p.OrderDetails)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_payment");
            });

            modelBuilder.Entity<PaymentDetail>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("payment_details_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.PaymentMethod).HasDefaultValueSql("'Cash'::character varying");
                entity.Property(e => e.PaymentStatus).HasDefaultValueSql("'Pending'::character varying");
                entity.Property(e => e.TotalPrice).HasDefaultValueSql("0.0");

                entity.HasOne(d => d.Order).WithMany(p => p.PaymentDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("payment_details_order_id_fkey");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PId).HasName("permissions_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.PermissionCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_permissions_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.PermissionModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_permissions_modifyby");

                entity.HasOne(d => d.Module).WithMany(p => p.Permissions).HasConstraintName("fk_permissions_module");

                entity.HasOne(d => d.Role).WithMany(p => p.Permissions).HasConstraintName("fk_permissions_role");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("RefreshTokens_pkey");

                entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RefreshTokens_Users");
            });

            modelBuilder.Entity<ResetPasswordToken>(entity =>
            {
                entity.HasKey(e => e.TokenId).HasName("ResetPasswordToken_pkey");

                entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsContinue).HasDefaultValueSql("true");

                entity.HasOne(d => d.User).WithMany(p => p.ResetPasswordTokens)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("User ID");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("roles_pkey");

                entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.SectionId).HasName("sections_pkey");

                entity.Property(e => e.SectionId).ValueGeneratedNever();
                entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.SectionCreatedbyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_sections_createdby");

                entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.SectionModifiedbyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_sections_modifiedby");
            });

            modelBuilder.Entity<StateList>(entity =>
            {
                entity.HasKey(e => e.StateId).HasName("state_list_pkey");

                entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsContinue).HasDefaultValueSql("true");

                entity.HasOne(d => d.Contry).WithMany(p => p.StateLists)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_contry_id");

                entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.StateListCreateByNavigations)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_state_row_createby");

                entity.HasOne(d => d.ModifyByNavigation).WithMany(p => p.StateListModifyByNavigations).HasConstraintName("fk_state_row_modifyby");
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.TableId).HasName("tables_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.TableCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_tables_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.TableModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_tables_modifyby");

                entity.HasOne(d => d.Section).WithMany(p => p.Tables).HasConstraintName("fk_tables_section");
            });

            modelBuilder.Entity<Taxis>(entity =>
            {
                entity.HasKey(e => e.TaxId).HasName("taxes_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isenabled).HasDefaultValueSql("true");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.TaxisCreatebyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_taxes_createby");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.TaxisModifybyNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_taxes_modifyby");
            });

            modelBuilder.Entity<Userauthentication>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("userauthentication_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Iscontinued).HasDefaultValueSql("true");

                entity.HasOne(d => d.Role).WithMany(p => p.Userauthentications).HasConstraintName("fk_role");
            });

            modelBuilder.Entity<Userdetail>(entity =>
            {
                entity.HasKey(e => e.UserDetailsId).HasName("userdetails_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");

                entity.HasOne(d => d.City).WithMany(p => p.Userdetails).HasConstraintName("fk_city_id");

                entity.HasOne(d => d.Contry).WithMany(p => p.Userdetails).HasConstraintName("fk_contry_id");

                entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.UserdetailCreateByNavigations).HasConstraintName("fk_user_createby");

                entity.HasOne(d => d.ModifyByNavigation).WithMany(p => p.UserdetailModifyByNavigations)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_user_modifyby");

                entity.HasOne(d => d.State).WithMany(p => p.Userdetails).HasConstraintName("fk_state_id");

                entity.HasOne(d => d.User).WithOne(p => p.Userdetail).HasConstraintName("fk_user");
            });

            modelBuilder.Entity<WaitingList>(entity =>
            {
                entity.HasKey(e => e.TokenId).HasName("waiting_list_pkey");

                entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Isactive).HasDefaultValueSql("true");
                entity.Property(e => e.PriorityLevel).HasDefaultValueSql("2");

                entity.HasOne(d => d.CreatebyNavigation).WithMany(p => p.WaitingListCreatebyNavigations)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("waiting_list_createby_fkey");

                entity.HasOne(d => d.Customer).WithMany(p => p.WaitingLists)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("waiting_list_customer_id_fkey");

                entity.HasOne(d => d.ModifybyNavigation).WithMany(p => p.WaitingListModifybyNavigations).HasConstraintName("waiting_list_modifyby_fkey");

                entity.HasOne(d => d.Section).WithMany(p => p.WaitingLists)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("waiting_list_section_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
