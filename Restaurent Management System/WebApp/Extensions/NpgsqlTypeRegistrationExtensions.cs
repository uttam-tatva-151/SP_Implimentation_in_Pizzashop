using Npgsql;
using PMSCore.DTOs;

namespace PMSWebApp.Extensions;

public static class NpgsqlTypeRegistrationExtensions
{
    /// <summary>
    /// Registers all PostgreSQL composite types needed for the application.
    /// </summary>
    public static NpgsqlDataSourceBuilder RegisterAppComposites(this NpgsqlDataSourceBuilder builder)
    {
        builder.MapComposite<UpdateInvoiceItemModifierMappingDTO>("invoice_item_modifier_mapping_dto");
        // Add more composite mappings as needed.
        return builder;
    }
}
