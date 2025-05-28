namespace PMSCore.Beans
{
    public static class Constants
    {
        #region  Success Messages
        public const string SUCCESS_READ = "{0} fetch successfully";
        public const string SUCCESS_ADDED = "{0} added successfully.";
        public const string SUCCESS_UPDATED = "{0} updated successfully.";
        public const string SUCCESS_DELETED = "{0} deleted successfully.";
        public const string GENERAL_SUCCESS = "Operation completed successfully.";
        public const string SUCCESS_LOGIN = "Login successfully.";
        public const string SUCCESS_LOGOUT = "Logout successfully.";
        public const string SUCCESS_FORGOT_PASSWORD = "Password reset link sent to your email.";
        public const string SUCCESS_PASSWORD_CHANGED = "Password changed successfully.";
        public const string SUCCESS_EMAIL_SENT = "Email sent successfully.";
        public const string SUCCESS_ASSIGN_TABLE = "Successfully Assign Table to customer.";
        public const string SUCCESS_TOKEN_REFRESHED = "Token refreshed successfully.";
        public const string ANALYSIS_COMPLETED = "Records analysis has been successfully completed.";
        public const string CANCEL_ORDER_CONFIRMATION = "Are you sure you want to cancel the Order?";
        public const string COMPLETE_ORDER_CONFIRMATION = "Are you sure you want to complete the Order?";
        public const string LOGOUT_CONFIRMATION = "Are you sure you want to Logout this ID?";

        #endregion
        #region  Error Messages
        public const string ERROR_ADDING = "An error occurred while adding {0}. Please try again.";
        public const string ERROR_UPDATING = "An error occurred while updating {0}. Please try again.";
        public const string ERROR_DELETING = "An error occurred while deleting {0}. Please try again.";
         public const string ERROR_NOT_FOUND = "{0} not found.";
        public const string GENERAL_ERROR = "An error occurred. Please try again.";
        public const string ERROR_LOGIN = "Login failed. Please check your Email.";
        public const string ERROR_PASSWORD_MISMATCH = "Password is Incorrect.";
        public const string ERROR_FORGOT_PASSWORD = "An error occurred while sending the password reset link. Please try again.";
        public const string ERROR_EMAIL_SENDING = "An error occurred while sending the email. Please try again.";
        public const string ERROR_TABLE_ASSIGNING = "An error occurred while assigning table. Please try again.";
        public const string ERROR_SENDING_LOGIN_DETAILS_EMAIL = "Error to send Login Details to user's MailId.";

        #endregion
        #region  Warning Messages
        public const string WARNING_INVALID_INPUT = "Invalid input provided for {0}.";
        public const string WARNING_INVALID_CREDENTIALS = "Invalid credentials.";
        public const string WARNING_ALL_READY_EXISTS = "{0} already exists.";
        public const string WARNING_DELETE_CONFIRMATION = "Are you sure want to delete this {0}?";
        public const string WARNING_MULTIPLE_DELETE_CONFIRMATION = "Are you sure want to delete this selected {0}s?";
        public const string WARNING_EMAIL_NOT_FOUND = "Email not found.";
        public const string WARNING_EMAIL_ALL_READY_EXISTS = "Email already exists.";
        public const string WARNING_EDITOR_ID_NOT_FOUND = "Editor ID not found.";
        public const string WARNING_NOTHING_SELECTED = "Please select at least one {0}.";
        public const string WARNING_ACTION_NOT_ALLOWED = "Action not allowed for you.";
        public const string WARNING_RESET_TOKEN_EXPIRED = "Reset Password Link Expired";
        public const string NO_ITEM_IN_BILL = "No items available. Add your first item and save It!!!";
        #endregion
        #region  Info Messages
        public const string INFO_LOADING = "Loading {0}...";
        public const string INFO_NO_RECORDS_FOUND = "No {0} records found.";
        #endregion
        #region  Module/Entity Name
        public const string MENU = "Menu";
        public const string AREA = "Area";
        public const string CATEGORY = "Category";
        public const string ITEM = "Item";
        public const string MODIFIER = "Modifier";
        public const string USER = "User";
        public const string ORDER = "Order";
        public const string CUSTOMER = "Customer";
        public const string MODIFIER_GROUP = "ModifierGroup";
        public const string CUSTOMER_HISTORY = "Customer History";
        public const string SECTION = "Section";
        public const string TABLE = "Table";
        public const string TAXES = "Taxes";
        public const string STATE = "State";
        public const string CITY = "City";
        public const string ORDERDETAILS = "Order Details";
        public const string ROLE = "Role";
        public const string PAYMENT_DETAILS = "Payment Details";
        public const string PASSWORD = "Password";
        public const string KOT = "KOT";
        public const string CATEGORY_LIST = "Category List";
        public const string MODIFIER_GROUP_LIST = "ModifierGroup List";
        public const string ITEM_LIST = "Item List";
        public const string USER_LIST = "User List";
        public const string ORDER_LIST = "Order List";
        public const string ROLE_LIST = "Role List";
        public const string KOT_LIST = "KOT List";
        public const string MODIFIER_LIST = "Modifier List";
        public const string RESET_PASSWORD_TOKEN = "Reset Password Token";
        public const string WAITING_TOKEN = "Waiting Token";
        public const string INVOICE = "Invoice";
        public const string SECTION_LIST = "Section List";
        public const string PERMISSION_LIST = "Permission List";
        public const string COUNTRY_LIST = "Country List";
        public const string STATE_LIST = "State List";
        public const string CITY_LIST = "City List";
        public const string MAPPING_RELATIONS = "Mapping Relations";
        public const string DEFAULT_ENTITY = "Entity";
        public const string PAGINATION = "Pagination Details";
        #endregion
        #region Item Type
        public const string VEG_ITEM = "Veg";
        public const string NON_VEG_ITEM = "Non-Veg";
        public const string VEGAN_ITEM = "Vegan";
        #endregion
        #region  Email Subject
        public const string EMAIL_SUBJECT_FORGOT_PASSWORD = "Password Reset Request";
        public const string EMAIL_SUBJECT_ADD_USER = "New User Registration";
        #endregion
        #region Order Status

        public const string ORDER_PENDING = "Pending";
        public const string ORDER_IN_PROGRESS = "InProgress";
        public const string ORDER_SERVED = "Served";
        public const string ORDER_COMPLETED = "Completed";
        public const string ORDER_CANCELLED = "Cancelled";
        public const string ORDER_ON_HOLD = "OnHold";
        public const string ORDER_FAILED = "Failed";

        #endregion

        #region Column Order 
        public const string ASC_ORDER = "asc";
        public const string DESC_ORDER = "desc";
        #endregion
        #region Payment Status
        public const string PAYMENT_PENDING = "Pending";
        public const string PAYMENT_COMPLETED = "Completed";
        public const string PAYMENT_FAILED = "Failed";
        public const string PAYMENT_REFUNDED = "Refunded";
        #endregion
        #region Payment Method

        public const string PAYMENT_METHOD_CARD = "Card";
        public const string PAYMENT_METHOD_CASH = "Cash";
        public const string PAYMENT_METHOD_UPI = "UPI";

        #endregion
        #region Table Status

        public const string TABLE_OCCUPIED = "Occupied";
        public const string TABLE_AVAILABLE = "Available";

        #endregion
        #region Tax Type
        public const string TAX_TYPE_PERCENTAGE = "Percentage";
        public const string TAX_TYPE_FLAT_AMOUNT = "Flat Amount";
    public const string DEFAULT_ITEM_TAX = "Other";
        #endregion

        #region  Layouts
        public const string LOGIN_LAYOUT = "_LoginLayout";
        public const string MAIN_LAYOUT = "_Layout";
        public const string ORDER_APP_LAYOUT = "_OrderAppLayout";
        public const string LAYOUT_VARIABLE_NAME = "LayoutName";
        #endregion

        #region  Tokens
        public const string ACCESS_TOKEN = "AccessToken";
        public const string REFRESH_TOKEN = "RefreshToken";
        public const string INVALID_ACCESS_TOKEN = "Invalid access token. Possible tampering detected.";
        public const string INVALID_REFRESH_TOKEN = "Invalid refresh token";
        public const string SESSION_EMAIL = "Email";
        public const string SESSION_USERNAME = "UserName";

        #endregion

        #region Views and Controller

        public const string DASHBOARD_VIEW = "Index";
        public const string USER_LIST_VIEW = "UserList";
        public const string ADD_USER_VIEW = "AddUser";
        public const string LOGIN_VIEW = "Index";
        public const string CUTOMER_VIEW = "Customer";
        public const string MENU_VIEW = "Menu";
        public const string PERMISSION_VIEW = "Permissions";
        public const string ORDER_VIEW = "Order";
        public const string ERROR_VIEW = "HttpStatusCodeHandler";
        public const string HOME_CONTROLLER = "Home";
        public const string ROLE_CONTROLLER = "RoleAndPermissions";
        public const string LOGIN_CONTROLLER = "Login";
        public const string USER_CONTROLLER = "User";
        public const string ORDER_CONTROLLER = "Orders";
        public const string MENU_CONTROLLER = "Menu";
        public const string CUTOMER_CONTROLLER = "Customers";
        public const string ERROR_CONTROLLER = "ErrorHandler";

        public const string ERROR_HANDLER_HTTP_STATUS_CODE_HANDLER_ROUTE = "/ErrorHandler/HttpStatusCodeHandler/{0}";
        public const string ERROR_HANDLER_HTTP_STATUS_CODE_500_ROUTE = "/ErrorHandler/HttpStatusCodeHandler/500";
        public const string ERROR_HANDLER_HTTP_STATUS_CODE_404_ROUTE = "/ErrorHandler/HttpStatusCodeHandler/404";
        public const string ERROR_HANDLER_ROUTE = "/ErrorHandler";

        #endregion

        #region Partial Views
        public const string PARTIAL_DASHBOARD_GRID = "_parial_Dashboard_Grid";
        public const string PARTIAL_ADD_ITEM_GRID = "_partial_AddItemGrid";
        public const string PARTIAL_ADD_EDIT_MODIFIER_GRID = "_partial_Add_Edit_ModifierGrid";
        public const string PARTIAL_ADD_MODIFIER_GRID = "_partial_Add_ModifierGrid";
        public const string PARTIAL_ALL_READY_EXISTING_MODIFIERS = "_partial_AllReadyExistingModifiers";
        public const string PARTIAL_CATEGORY_LIST_GRID = "_partial_CategoryListGrid";
        public const string PARTIAL_EDIT_ITEM_GRID = "_partial_EditItemGrid";
        public const string PARTIAL_EDIT_MODIFIER_GRID = "_partial_Edit_ModifierGrid";
        public const string PARTIAL_ITEM_LIST_GRID = "_partial_ItemListGrid";
        public const string PARTIAL_INVOICE = "Invoice";
        public const string PARTIAL_MODIFIERS_GROUP_LIST_GRID = "_partial_ModifiersGroupListGrid";
        public const string PARTIAL_MODIFIERS_LIST_GRID = "_partial_ModifiersListGrid";
        public const string PARTIAL_KOTS_GRID = "_partial_KOTsGrid";
        public const string PARTIAL_ORDER_TABLE = "_partial_OrderTable";
        public const string PARTIAL_SECTION_LIST_GRID = "_partial_SectionListGrid";
        public const string PARTIAL_TABLES_LIST_GRID = "_partial_TablesListGrid";
        public const string PARTIAL_TAX_LIST = "_partial_TaxList";
        public const string PARTIAL_USER_GRID = "_partial_UserGrid";
        public const string PARTIAL_CUSTOMER_TABLE_GRID = "_partial_CustomerTableGrid";
        public const string PARTIAL_CUSTOMER_HISTORY_GRID = "_partial_Customer_HistoryGrid";
        public const string PARTIAL_ORDER_APP_MENU_ITEM_LIST_GRID = "_partial_OrderAppMenu_ItemListGrid";
        public const string PARTIAL_ORDER_APP_MENU_ITEM_MODIFIER_GROUP_RELATION_GRID = "_partial_OrderAppMenu_ItemModifierGroupRelation";
        public const string PARTIAL_ORDER_APP_MENU_ORDER_PLACE = "_partial_OrderAppMenu_OrderPlace";
        public const string PARTIAL_WAITING_LIST_GRID = "_partial_watingListGrid";
        public const string PARTIAL_WAITING_TOKEN = "_partial_WatingToken";
        public const string PARTIAL_ORDER_APP_KOTS_GRID = "_partial_KOTsGrid";
        public const string PARTIAL_TABLE_VIEW_LIST_GRID = "_partial_TableView_ListGrid";
        public const string PARTIAL_WAITING_LIST_AT_ASSIGN_TABLE = "_partial_waitiningListAtAssignTable";
        public const string PARTIAL_WATING_TOKEN = "_partial_WatingToken";
        #endregion


        #region Authorization Permissions

        public const string CREATE_AND_EDIT_PERMISSION = "can_createandedit";
        public const string VIEW_PERMISSION = "can_view";
        public const string DELETE_PERMISSION = "can_delete";

        #endregion

        #region  Module Names

        public const string USERS_MODULE = "Users";
        public const string ROLE_AND_PERMISSION_MODULE = "RoleAndPermission";
        public const string MENU_MODULE = "Menu";
        public const string TABLE_AND_SECTION_MODULE = "TableAndSection";
        public const string TAX_AND_FEE_MODULE = "TaxAndFee";
        public const string ORDER_MODULE = "Order";
        public const string CUSTOMERS_MODULE = "Customers";

        #endregion

        #region  Static Files Related

        public const string IMAGE_TYPE = "image/jpeg";
        public const string FORGOT_PASSWORD_FILE = "ForgotPasswordFormat.html";
        public const string EXPORT_FILE_GENERATION_ERROR = "An error occurred while generating the export file. Please try again later.";
        public const string EXPORT_FILE_GENERATION_SUCCESS = "The export file was generated successfully.";
        public const string CUSTOMER_DETAILS_EXPORT_FILENAME = "Customer_Details_Data_Format.xlsx";
        public const string ORDER_SALES_DATA_FORMAT_FILE = "Order_Sales_Data_Format.xlsx";

        public const string TEMPLATE_NOT_FOUND = "Template not found.";

        public const string DATE_FORMATE = "yyyy-MM-dd";
        public const string EXCEL_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetsml.sheet";
        public const string PDF_CONTENT_TYPE = "application/pdf";
        public const string IMAGE_FORMATE = "data:image/jpeg;base64";
        #endregion

        #region  Roles
        public const string ADMIN_ROLE = "admin";
        public const string CHEF_ROLE = "chef";
        public const string ACCOUNT_MANAGER_ROLE = "account manager";
        public const string GUEST_ROLE = "Guest"; // Default role
        #endregion

        #region sort column
        public const string SORT_BY_DATE = "Date";


        #endregion

        #region Configuration Strings
        public const string JWT_CONFIG = "JwtConfig";
        public const string EMAIL_CONFIG = "EmailSettings";
        public const string DATABASE_DEFAULT_CONNECTION = "DefaultConnection";
        public const string MAX_FILE_UPLOAD_SIZE = "FileUpload:MaxMultipartBodyLengthInBytes";
        public const string DEFAULT_ROUTE_CONFIG = "RouteSettings";
        public const int SESSION_IDLE_TIME_OUT_HOURS = 10;
        #endregion




    }
}
