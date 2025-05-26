using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PMSData.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    cust_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cust_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    total_orders = table.Column<int>(type: "integer", nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    iscontinued = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("customers_pkey", x => x.cust_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_pkey", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "userauthentication",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    email_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    iscontinued = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("userauthentication_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_role",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    kot_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    isPrepared = table.Column<bool>(type: "boolean", nullable: false),
                    createAt = table.Column<TimeOnly>(type: "time without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyAt = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    cretaeBy = table.Column<int>(type: "integer", nullable: false),
                    modifyBy = table.Column<int>(type: "integer", nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("audit_logs_pkey", x => x.kot_id);
                    table.ForeignKey(
                        name: "fk_kot_createby",
                        column: x => x.cretaeBy,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_kot_modifyby",
                        column: x => x.modifyBy,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("categories_pkey", x => x.category_id);
                    table.ForeignKey(
                        name: "fk_category_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_category_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "contry_list",
                columns: table => new
                {
                    contry_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"Contry_list_contry_id_seq\"'::regclass)"),
                    contry_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    createAT = table.Column<TimeOnly>(type: "time without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyAt = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    CreateBY = table.Column<int>(type: "integer", nullable: false),
                    modifyBY = table.Column<int>(type: "integer", nullable: true),
                    IsContinue = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("Contry_list_pkey", x => x.contry_id);
                    table.ForeignKey(
                        name: "fk_contry_row_createby",
                        column: x => x.CreateBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_contry_row_modifyby",
                        column: x => x.modifyBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "modifiers_group",
                columns: table => new
                {
                    mg_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mg_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("modifiers_group_pkey", x => x.mg_id);
                    table.ForeignKey(
                        name: "fk_modifiers_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_modifiers_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "module",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("module_pkey", x => x.module_id);
                    table.ForeignKey(
                        name: "fk_module_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_module_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    deliver_on_time = table.Column<bool>(type: "boolean", nullable: false),
                    nu_of_persons = table.Column<int>(type: "integer", nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    iscontinued = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.order_id);
                    table.ForeignKey(
                        name: "fk_orders_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_orders_customer",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "cust_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("RefreshTokens_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users",
                        column: x => x.UserId,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ResetPasswordToken",
                columns: table => new
                {
                    TokenID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResetToken = table.Column<string>(type: "text", nullable: false),
                    userID = table.Column<int>(type: "integer", nullable: false),
                    IsContinue = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    CreateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ResetPasswordToken_pkey", x => x.TokenID);
                    table.ForeignKey(
                        name: "User ID",
                        column: x => x.userID,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "sections",
                columns: table => new
                {
                    section_id = table.Column<int>(type: "integer", nullable: false),
                    section_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifiedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createdby = table.Column<int>(type: "integer", nullable: false),
                    modifiedby = table.Column<int>(type: "integer", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sections_pkey", x => x.section_id);
                    table.ForeignKey(
                        name: "fk_sections_createdby",
                        column: x => x.createdby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_sections_modifiedby",
                        column: x => x.modifiedby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "taxes",
                columns: table => new
                {
                    tax_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tax_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    tax_type = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    tax_value = table.Column<decimal>(type: "numeric(7,3)", precision: 7, scale: 3, nullable: false),
                    isdefault = table.Column<bool>(type: "boolean", nullable: false),
                    isenabled = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    iscontinued = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("taxes_pkey", x => x.tax_id);
                    table.ForeignKey(
                        name: "fk_taxes_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_taxes_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "waiting_list",
                columns: table => new
                {
                    token_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    waiting_time = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    priority_level = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "2"),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    remarks = table.Column<string>(type: "text", nullable: true),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("waiting_list_pkey", x => x.token_id);
                    table.ForeignKey(
                        name: "waiting_list_createby_fkey",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "waiting_list_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "cust_id");
                    table.ForeignKey(
                        name: "waiting_list_modifyby_fkey",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    item_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    item_type = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    IsDefaultTAX = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    short_Code = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    photo_data = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("items_pkey", x => x.item_id);
                    table.ForeignKey(
                        name: "fk_items_category",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_items_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_items_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "state_list",
                columns: table => new
                {
                    state_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contry_id = table.Column<int>(type: "integer", nullable: false),
                    state_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    createAT = table.Column<TimeOnly>(type: "time without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyAT = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    createBY = table.Column<int>(type: "integer", nullable: false),
                    modifyBY = table.Column<int>(type: "integer", nullable: true),
                    IsContinue = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("state_list_pkey", x => x.state_id);
                    table.ForeignKey(
                        name: "fk_contry_id",
                        column: x => x.contry_id,
                        principalTable: "contry_list",
                        principalColumn: "contry_id");
                    table.ForeignKey(
                        name: "fk_state_row_createby",
                        column: x => x.createBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_state_row_modifyby",
                        column: x => x.modifyBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "modifiers",
                columns: table => new
                {
                    m_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mg_id = table.Column<int>(type: "integer", nullable: false),
                    m_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("modifiers_pkey", x => x.m_id);
                    table.ForeignKey(
                        name: "fk_modifiers_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_modifiers_group",
                        column: x => x.mg_id,
                        principalTable: "modifiers_group",
                        principalColumn: "mg_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_modifiers_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    p_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    module_id = table.Column<int>(type: "integer", nullable: false),
                    can_createandedit = table.Column<bool>(type: "boolean", nullable: false),
                    can_view = table.Column<bool>(type: "boolean", nullable: false),
                    can_delete = table.Column<bool>(type: "boolean", nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("permissions_pkey", x => x.p_id);
                    table.ForeignKey(
                        name: "fk_permissions_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_permissions_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_permissions_module",
                        column: x => x.module_id,
                        principalTable: "module",
                        principalColumn: "module_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_permissions_role",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feedback_form",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    food_rating = table.Column<short>(type: "smallint", nullable: true),
                    service_rating = table.Column<short>(type: "smallint", nullable: true),
                    ambiance_rating = table.Column<short>(type: "smallint", nullable: true),
                    feedback_description = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    isresolved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_form_pkey", x => x.feedback_id);
                    table.ForeignKey(
                        name: "feedback_form_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "feedback_form_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    invoice_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    createAt = table.Column<TimeOnly>(type: "time without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyAt = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    createBy = table.Column<int>(type: "integer", nullable: false),
                    modifyBy = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("invoices_pkey", x => x.invoice_id);
                    table.ForeignKey(
                        name: "fk_invoice_createby",
                        column: x => x.createBy,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_invoice_modifyby",
                        column: x => x.modifyBy,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "payment_details",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    actual_price = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    tax_id = table.Column<int[]>(type: "integer[]", nullable: false),
                    tax_value = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    payment_status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'Pending'::character varying")
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_details_pkey", x => x.payment_id);
                    table.ForeignKey(
                        name: "payment_details_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "tables",
                columns: table => new
                {
                    table_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    section_id = table.Column<int>(type: "integer", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    iscontinued = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tables_pkey", x => x.table_id);
                    table.ForeignKey(
                        name: "fk_tables_createby",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_tables_modifyby",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_tables_section",
                        column: x => x.section_id,
                        principalTable: "sections",
                        principalColumn: "section_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorites_items",
                columns: table => new
                {
                    fi_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("favorites_items_pkey", x => x.fi_id);
                    table.ForeignKey(
                        name: "favorites_items_item_id_fkey",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "item_id");
                    table.ForeignKey(
                        name: "favorites_items_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "city_list",
                columns: table => new
                {
                    city_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    state_id = table.Column<int>(type: "integer", nullable: false),
                    city_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cretateAT = table.Column<TimeOnly>(type: "time without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyAT = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    createBY = table.Column<int>(type: "integer", nullable: false),
                    modifyBY = table.Column<int>(type: "integer", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("city_list_pkey", x => x.city_id);
                    table.ForeignKey(
                        name: "fk_city_row_createby",
                        column: x => x.createBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_city_row_modifyby",
                        column: x => x.modifyBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_state_id",
                        column: x => x.state_id,
                        principalTable: "state_list",
                        principalColumn: "state_id");
                });

            migrationBuilder.CreateTable(
                name: "item_modifiers",
                columns: table => new
                {
                    im_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    m_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("item_modifiers_pkey", x => x.im_id);
                    table.ForeignKey(
                        name: "fk_item_modifiers_item",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_modifiers_modifier",
                        column: x => x.m_id,
                        principalTable: "modifiers",
                        principalColumn: "m_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    od_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    table_id = table.Column<int[]>(type: "integer[]", nullable: false),
                    payment_id = table.Column<int>(type: "integer", nullable: false),
                    employee_id = table.Column<int[]>(type: "integer[]", nullable: false),
                    feedback_id = table.Column<int>(type: "integer", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifiedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createdby = table.Column<int>(type: "integer", nullable: false),
                    modifiedby = table.Column<int>(type: "integer", nullable: true),
                    iscontinued = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_details_pkey", x => x.od_id);
                    table.ForeignKey(
                        name: "fk_created_by",
                        column: x => x.createdby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_feedback",
                        column: x => x.feedback_id,
                        principalTable: "feedback_form",
                        principalColumn: "feedback_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_modified_by",
                        column: x => x.modifiedby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_order",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payment",
                        column: x => x.payment_id,
                        principalTable: "payment_details",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "userdetails",
                columns: table => new
                {
                    user_details_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    zip_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    contry_id = table.Column<int>(type: "integer", nullable: false),
                    state_id = table.Column<int>(type: "integer", nullable: false),
                    city_id = table.Column<int>(type: "integer", nullable: false),
                    createBY = table.Column<int>(type: "integer", nullable: false),
                    modifyBY = table.Column<int>(type: "integer", nullable: true),
                    photo_data = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("userdetails_pkey", x => x.user_details_id);
                    table.ForeignKey(
                        name: "fk_city_id",
                        column: x => x.city_id,
                        principalTable: "city_list",
                        principalColumn: "city_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contry_id",
                        column: x => x.contry_id,
                        principalTable: "contry_list",
                        principalColumn: "contry_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_state_id",
                        column: x => x.state_id,
                        principalTable: "state_list",
                        principalColumn: "state_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user",
                        column: x => x.user_id,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_user_createby",
                        column: x => x.createBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_modifyby",
                        column: x => x.modifyBY,
                        principalTable: "userauthentication",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_items_list",
                columns: table => new
                {
                    items_list_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    item_modifer_id = table.Column<int>(type: "integer", nullable: false),
                    extra_comments = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    prices = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    createat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modifyat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createby = table.Column<int>(type: "integer", nullable: false),
                    modifyby = table.Column<int>(type: "integer", nullable: true),
                    iscontinued = table.Column<bool>(type: "boolean", nullable: false),
                    prepared_items = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_items_list_pkey", x => x.items_list_id);
                    table.ForeignKey(
                        name: "order_item_and_modifiers_list_item_modifier_id_fkey",
                        column: x => x.item_modifer_id,
                        principalTable: "item_modifiers",
                        principalColumn: "im_id");
                    table.ForeignKey(
                        name: "order_items_list_createby_fkey",
                        column: x => x.createby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "order_items_list_modifyby_fkey",
                        column: x => x.modifyby,
                        principalTable: "userauthentication",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "order_items_list_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_cretaeBy",
                table: "audit_logs",
                column: "cretaeBy");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_modifyBy",
                table: "audit_logs",
                column: "modifyBy");

            migrationBuilder.CreateIndex(
                name: "categories_category_name_key",
                table: "categories",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_createby",
                table: "categories",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_categories_modifyby",
                table: "categories",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "IX_city_list_createBY",
                table: "city_list",
                column: "createBY");

            migrationBuilder.CreateIndex(
                name: "IX_city_list_modifyBY",
                table: "city_list",
                column: "modifyBY");

            migrationBuilder.CreateIndex(
                name: "IX_city_list_state_id",
                table: "city_list",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "IX_contry_list_CreateBY",
                table: "contry_list",
                column: "CreateBY");

            migrationBuilder.CreateIndex(
                name: "IX_contry_list_modifyBY",
                table: "contry_list",
                column: "modifyBY");

            migrationBuilder.CreateIndex(
                name: "customers_email_id_key",
                table: "customers",
                column: "email_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "customers_phone_number_key",
                table: "customers",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_customers_email",
                table: "customers",
                column: "email_id");

            migrationBuilder.CreateIndex(
                name: "IX_favorites_items_item_id",
                table: "favorites_items",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_favorites_items_user_id",
                table: "favorites_items",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_form_order_id",
                table: "feedback_form",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_form_user_id",
                table: "feedback_form",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_createBy",
                table: "invoices",
                column: "createBy");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_modifyBy",
                table: "invoices",
                column: "modifyBy");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_order_id",
                table: "invoices",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "item_modifiers_item_id_m_id_key",
                table: "item_modifiers",
                columns: new[] { "item_id", "m_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_modifiers_m_id",
                table: "item_modifiers",
                column: "m_id");

            migrationBuilder.CreateIndex(
                name: "IX_items_createby",
                table: "items",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_items_modifyby",
                table: "items",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "unique_item_name_per_category",
                table: "items",
                columns: new[] { "category_id", "item_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modifiers_createby",
                table: "modifiers",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_modifiers_mg_id",
                table: "modifiers",
                column: "mg_id");

            migrationBuilder.CreateIndex(
                name: "IX_modifiers_modifyby",
                table: "modifiers",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "IX_modifiers_group_createby",
                table: "modifiers_group",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_modifiers_group_modifyby",
                table: "modifiers_group",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "modifiers_group_mg_name_key",
                table: "modifiers_group",
                column: "mg_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_module_createby",
                table: "module",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_module_modifyby",
                table: "module",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "module_module_name_key",
                table: "module",
                column: "module_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_details_createdby",
                table: "order_details",
                column: "createdby");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_feedback_id",
                table: "order_details",
                column: "feedback_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_modifiedby",
                table: "order_details",
                column: "modifiedby");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_order_id",
                table: "order_details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_payment_id",
                table: "order_details",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_list_createby",
                table: "order_items_list",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_list_item_modifer_id",
                table: "order_items_list",
                column: "item_modifer_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_list_modifyby",
                table: "order_items_list",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_list_order_id",
                table: "order_items_list",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "idx_orders_status",
                table: "orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_orders_createby",
                table: "orders",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_modifyby",
                table: "orders",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "idx_payments_order_id",
                table: "payment_details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_createby",
                table: "permissions",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_modifyby",
                table: "permissions",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_module_id",
                table: "permissions",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_role_id",
                table: "permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordToken_userID",
                table: "ResetPasswordToken",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "roles_role_name_key",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_role_name",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sections_createdby",
                table: "sections",
                column: "createdby");

            migrationBuilder.CreateIndex(
                name: "IX_sections_modifiedby",
                table: "sections",
                column: "modifiedby");

            migrationBuilder.CreateIndex(
                name: "IX_state_list_contry_id",
                table: "state_list",
                column: "contry_id");

            migrationBuilder.CreateIndex(
                name: "IX_state_list_createBY",
                table: "state_list",
                column: "createBY");

            migrationBuilder.CreateIndex(
                name: "IX_state_list_modifyBY",
                table: "state_list",
                column: "modifyBY");

            migrationBuilder.CreateIndex(
                name: "IX_tables_createby",
                table: "tables",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_tables_modifyby",
                table: "tables",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "IX_tables_section_id",
                table: "tables",
                column: "section_id");

            migrationBuilder.CreateIndex(
                name: "IX_taxes_createby",
                table: "taxes",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_taxes_modifyby",
                table: "taxes",
                column: "modifyby");

            migrationBuilder.CreateIndex(
                name: "IX_userauthentication_role_id",
                table: "userauthentication",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "userauthentication_email_id_key",
                table: "userauthentication",
                column: "email_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userdetails_city_id",
                table: "userdetails",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_userdetails_contry_id",
                table: "userdetails",
                column: "contry_id");

            migrationBuilder.CreateIndex(
                name: "IX_userdetails_createBY",
                table: "userdetails",
                column: "createBY");

            migrationBuilder.CreateIndex(
                name: "IX_userdetails_modifyBY",
                table: "userdetails",
                column: "modifyBY");

            migrationBuilder.CreateIndex(
                name: "IX_userdetails_state_id",
                table: "userdetails",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "IX_userdetails_user_id",
                table: "userdetails",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_waiting_list_createby",
                table: "waiting_list",
                column: "createby");

            migrationBuilder.CreateIndex(
                name: "IX_waiting_list_customer_id",
                table: "waiting_list",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_waiting_list_modifyby",
                table: "waiting_list",
                column: "modifyby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "favorites_items");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "order_items_list");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ResetPasswordToken");

            migrationBuilder.DropTable(
                name: "tables");

            migrationBuilder.DropTable(
                name: "taxes");

            migrationBuilder.DropTable(
                name: "userdetails");

            migrationBuilder.DropTable(
                name: "waiting_list");

            migrationBuilder.DropTable(
                name: "feedback_form");

            migrationBuilder.DropTable(
                name: "payment_details");

            migrationBuilder.DropTable(
                name: "item_modifiers");

            migrationBuilder.DropTable(
                name: "module");

            migrationBuilder.DropTable(
                name: "sections");

            migrationBuilder.DropTable(
                name: "city_list");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "modifiers");

            migrationBuilder.DropTable(
                name: "state_list");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "modifiers_group");

            migrationBuilder.DropTable(
                name: "contry_list");

            migrationBuilder.DropTable(
                name: "userauthentication");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
