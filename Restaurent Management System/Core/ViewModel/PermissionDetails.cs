namespace PMSCore.ViewModel
{
    public class PermissionDetails
    {
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; } = false;
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public bool CanCreateandedit { get; set; }

        public bool CanView { get; set; }

        public bool CanDelete { get; set; }

        public int? EditorId { get; set; }
    }
}