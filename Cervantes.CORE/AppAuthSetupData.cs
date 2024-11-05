using AuthPermissions.BaseCode.SetupCode;

namespace Cervantes.CORE;

public static class AppAuthSetupData
{
    public static readonly List<BulkLoadRolesDto> RolesDefinition = new()
    {
        new("User", "Default User Role", "ClientsRead,ProjectsRead,ProjectMembersRead,ProjectNotesRead," +
                                         "ProjectNotesEdit,ProjectNotesAdd,ProjectNotesDelete," +
                                         "ProjectAttachmentsRead,ProjectAttachmentsAdd,ProjectAttachmentsDelete," +
                                         "ProjectExecutiveSummaryRead,ProjectExecutiveSummaryEdit," +
                                         "DocumentsRead,TasksRead,TasksEdit,TasksAdd,TasksDelete," +
                                         "VulnsRead,VulnsEdit,VulnsAdd,VulnsDelete,VulnsImport,VulnCategoriesRead," +
                                         "VulnAttachmentsRead,VulnAttachmentsAdd,VulnAttachmentsDelete,VulnAttachmentsDownload,VulnNotesRead,VulnNotesEdit,VulnNotesAdd,VulnNotesDelete," +
                                         "KnowledgeBaseRead,KnowledgeBaseEdit,KnowledgeBaseAdd,KnowledgeBaseDelete," +
                                         "ReportsRead,CalendarRead,WorkspacesRead,TargetsRead,TargetsEdit,TargetsAdd,TargetsDelete," +
                                         "TargetsServicesRead,TargetsServicesEdit,TargetsServicesAdd,TargetsServicesDelete," +
                                         "NotesRead,NotesEdit,NotesAdd,NotesDelete,TaskNotesRead,TaskNotesEdit,TaskNotesAdd,TaskNotesDelete," +
                                         "TaskAttachmentsRead,TaskAttachmentsAdd,TaskAttachmentsDelete,TaskAttachmentsDownload," +
                                         "TaskTargetsRead,TaskTargetsEdit,TaskTargetsAdd,TaskTargetsDelete," +
                                         "VulnTargetsRead,VulnTargetsEdit,VulnTargetsAdd,VulnTargetsDelete," +
                                         "JiraRead,JiraEdit,JiraAdd,JiraDelete,JiraCommentsRead,JiraCommentsAdd," +
                                         "VaultRead,VaultEdit,VaultAdd,VaultDelete,ChecklistsRead,ChecklistsEdit,ChecklistsAdd,ChecklistsDelete,AIServiceUsage"),
        new("Manager",  "Manager Role (Manage Projects, Reports, Tasks, etc.)", "ClientsRead,ClientsEdit,ClientsAdd,ClientsDelete," +
                                         "ProjectsRead,ProjectsEdit,ProjectsAdd,ProjectsDelete,ProjectMembersRead,ProjectMembersAdd,ProjectMembersDelete," +
                                         "ProjectNotesRead,ProjectNotesEdit,ProjectNotesAdd,ProjectNotesDelete,ProjectAttachmentsRead,ProjectAttachmentsAdd,ProjectAttachmentsDelete,ProjectAttachmentsDownload," +
                                         "ProjectExecutiveSummaryRead,ProjectExecutiveSummaryEdit,DocumentsRead,DocumentsEdit,DocumentsAdd,DocumentsDelete,TasksRead,TasksEdit,TasksAdd,TasksDelete," +
                                         "VulnsRead,VulnsEdit,VulnsAdd,VulnsDelete,VulnsImport,VulnCategoriesRead,VulnCategoriesEdit,VulnCategoriesAdd,VulnCategoriesDelete," +
                                         "VulnAttachmentsRead,VulnAttachmentsAdd,VulnAttachmentsDelete,VulnAttachmentsDownload,VulnNotesRead,VulnNotesEdit,VulnNotesAdd,VulnNotesDelete,"+
                                         "KnowledgeBaseRead,KnowledgeBaseEdit,KnowledgeBaseAdd,KnowledgeBaseDelete,KnowledgeBaseCategoryRead,KnowledgeBaseCategoryEdit,KnowledgeBaseCategoryDelete,KnowledgeBaseCategoryAdd,OrganizationRead,OrganizationEdit," +
                                         "ReportTemplatesRead,ReportTemplatesEdit,ReportTemplatesAdd,ReportTemplatesDelete,ReportComponentsRead,ReportComponentsEdit,ReportComponentsAdd,ReportComponentsDelete," +
                                         "ReportsRead,ReportsEdit,ReportsAdd,ReportsDelete,CalendarRead,WorkspacesRead," +
                                         "TargetsRead,TargetsEdit,TargetsAdd,TargetsDelete,TargetsServicesRead,TargetsServicesEdit,TargetsServicesAdd,TargetsServicesDelete,NotesRead,NotesEdit,NotesAdd,NotesDelete," +
                                         "TaskNotesRead,TaskNotesEdit,TaskNotesAdd,TaskNotesDelete,TaskAttachmentsRead,TaskAttachmentsAdd,TaskAttachmentsDelete,TaskAttachmentsDownload,TaskTargetsRead,TaskTargetsEdit,TaskTargetsAdd,TaskTargetsDelete," +
                                         "VulnTargetsRead,VulnTargetsEdit,VulnTargetsAdd,VulnTargetsDelete,JiraRead,JiraEdit,JiraAdd,JiraDelete,JiraCommentsRead,JiraCommentsAdd,VaultRead,VaultEdit,VaultAdd,VaultDelete,ChecklistsRead,ChecklistsEdit,ChecklistsAdd,ChecklistsDelete,AIServiceUsage"),
        new("Admin", "This allows the user to access every feature", "Admin"),
    };

    public static readonly List<BulkLoadUserWithRolesTenant> UsersWithRolesDefinition = new()
    {
        new ( "admin@cervantes.local", null, "Admin"),
    };
}