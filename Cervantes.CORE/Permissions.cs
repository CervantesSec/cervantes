using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE;

public enum Permissions: ushort
{
    NotSet = 0, //error condition

    [Display(GroupName = "Admin", Name = "Access All the Features", 
        Description = "This allows the user to access every feature", AutoGenerateFilter = true)]
    Admin = ushort.MaxValue,

    [Display(GroupName = "Clients", Name = "View/Read Clients", Description = "Can read and view clients")]
    ClientsRead = 1,
    [Display(GroupName = "Clients", Name = "Edit Clients", Description = "Can edit existing clients")]
    ClientsEdit = 2,
    [Display(GroupName = "Clients", Name = "Delete Clients", Description = "Can delete existing clients")]
    ClientsDelete = 3,
    [Display(GroupName = "Clients", Name = "Add New Clients", Description = "Can add new clients")]
    ClientsAdd = 4,
    
    [Display(GroupName = "Projects", Name = "View/Read Projects", Description = "Can read and view projects")]
    ProjectsRead = 11,
    [Display(GroupName = "Projects", Name = "Edit Projects", Description = "Can edit the existing projects")]
    ProjectsEdit = 12,
    [Display(GroupName = "Projects", Name = "Delete Projects", Description = "Can delete the existing projects")]
    ProjectsDelete = 13,
    [Display(GroupName = "Projects", Name = "Add New Projects", Description = "Can add new projects")]
    ProjectsAdd = 14,
    
    [Display(GroupName = "Project Members", Name = "View/Read Project Members", Description = "Can read and view the project members")]
    ProjectMembersRead = 21,
    [Display(GroupName = "Project Members", Name = "Delete Project Members", Description = "Can delete members from the project")]
    ProjectMembersDelete = 22,
    [Display(GroupName = "Project Members", Name = "Add New Project Members", Description = "Can add new members to the project")]
    ProjectMembersAdd = 23,
    
    [Display(GroupName = "Project Notes", Name = "View/Read Project Notes", Description = "Can read and view the project notes")]
    ProjectNotesRead = 31,
    [Display(GroupName = "Project Notes", Name = "Edit Project Notes", Description = "Used in User.UserHasThisPermission in page")]
    ProjectNotesEdit = 32,
    [Display(GroupName = "Project Notes", Name = "Delete Project Notes", Description = "Used in User.UserHasThisPermission in page")]
    ProjectNotesDelete = 33,
    [Display(GroupName = "Project Notes", Name = "Add New Project Notes", Description = "Used in User.UserHasThisPermission in page")]
    ProjectNotesAdd = 34,
    
    [Display(GroupName = "Project Attachments", Name = "View/Read Project Attachments", Description = "Can read and view the project attachments")]
    ProjectAttachmentsRead = 41,
    [Display(GroupName = "Project Attachments", Name = "Edit Project Attachments", Description = "Can edit the project attachments")]
    ProjectAttachmentsEdit = 42,
    [Display(GroupName = "Project Attachments", Name = "Delete Project Attachments", Description = "Can delete the project attachments")]
    ProjectAttachmentsDelete = 43,
    [Display(GroupName = "Project Attachments", Name = "Add New Project Attachments", Description = "Can add new project attachments")]
    ProjectAttachmentsAdd = 44,
    [Display(GroupName = "Project Attachments", Name = "Download Project Attachments", Description = "Can download the project attachments")]
    ProjectAttachmentsDownload = 45,

    
    [Display(GroupName = "Project Executive Summary", Name = "View Project Executive Summary", Description = "Can read and view the project ececutive summary")]
    ProjectExecutiveSummaryRead = 51,
    [Display(GroupName = "Project Executive Summary", Name = "Edit Project Executive Summary", Description = "Can edit the project executive summary")]
    ProjectExecutiveSummaryEdit = 52,
    
    
    [Display(GroupName = "Documents", Name = "View/Read Documents", Description = "Can read and view documents")]
    DocumentsRead = 61,
    [Display(GroupName = "Documents", Name = "Display Documents", Description = "Can edit the documents")]
    DocumentsEdit = 62,
    [Display(GroupName = "Documents", Name = "Display Documents", Description = "Can delete the documents")]
    DocumentsDelete = 63,
    [Display(GroupName = "Documents", Name = "Display Documents", Description = "Can add new documents")]
    DocumentsAdd = 64,
    [Display(GroupName = "Documents", Name = "Display Documents", Description = "Can download the documents")]
    DocumentsDownload = 65,
    
    [Display(GroupName = "Tasks", Name = "View Tasks", Description = "Can read and view tasks")]
    TasksRead = 71,
    [Display(GroupName = "Tasks", Name = "Edit Tasks", Description = "Can edit the tasks")]
    TasksEdit = 72,
    [Display(GroupName = "Tasks", Name = "Delete Tasks", Description = "Can delete the tasks")]
    TasksDelete = 73,
    [Display(GroupName = "Tasks", Name = "Add New Tasks", Description = "Can add new tasks")]
    TasksAdd = 74,
    
    [Display(GroupName = "Vulns", Name = "View/Read Vulns", Description = "Can read and view vulnerabilities")]
    VulnsRead = 81,
    [Display(GroupName = "Vulns", Name = "Edit Vulns", Description = "Used in User.UserHasThisPermission in page")]
    VulnsEdit = 82,
    [Display(GroupName = "Vulns", Name = "Delete Vulns", Description = "Used in User.UserHasThisPermission in page")]
    VulnsDelete = 83,
    [Display(GroupName = "Vulns", Name = "Add New Vulns", Description = "Used in User.UserHasThisPermission in page")]
    VulnsAdd = 84,
    [Display(GroupName = "Vulns", Name = "Import New Vulns", Description = "Used in User.UserHasThisPermission in page")]
    VulnsImport = 85,
    [Display(GroupName = "Vulns", Name = "Export Vulns", Description = "Used in User.UserHasThisPermission in page")]
    VulnsExport = 86,
    
    [Display(GroupName = "Vuln Categories", Name = "View/Read Vuln Categories", Description = "Can read and view vulnerability categories")]
    VulnCategoriesRead = 91,
    [Display(GroupName = "Vuln Categories", Name = "Edit Vuln Categories", Description = "Can edit the vulnerability categories")]
    VulnCategoriesEdit = 92,
    [Display(GroupName = "Vuln Categories", Name = "Delete Vuln Categories", Description = "Can delete the vulnerability categories")]
    VulnCategoriesDelete = 93,
    [Display(GroupName = "Vuln Categories", Name = "Add New Vuln Categories", Description = "Can add new vulnerability categories")]
    VulnCategoriesAdd = 94,
    
    [Display(GroupName = "Vuln Attachments", Name = "View/Read Vuln Attachments", Description = "Can read and view the vulnerability attachments")]
    VulnAttachmentsRead = 101,
    [Display(GroupName = "Vuln Attachments", Name = "Edit Vuln Attachments", Description = "Can edit the vulnerability attachments")]
    VulnAttachmentsEdit = 102,
    [Display(GroupName = "Vuln Attachments", Name = "Delete Vuln Attachments", Description = "Can delete the vulnerability attachments")]
    VulnAttachmentsDelete = 103,
    [Display(GroupName = "Vuln Attachments", Name = "Add New Vuln Attachments", Description = "Can add new vulnerability attachments")]
    VulnAttachmentsAdd = 104,
    [Display(GroupName = "Vuln Attachments", Name = "Download Vuln Attachments", Description = "Can download the vulnerability attachments")]
    VulnAttachmentsDownload = 105,
    
    [Display(GroupName = "Vuln Notes", Name = "View/Read Vuln Notes", Description = "Can read and view the vulnerability notes")]
    VulnNotesRead = 111,
    [Display(GroupName = "Vuln Notes", Name = "Edit Vuln Notes", Description = "Can edit the vulnerability notes")]
    VulnNotesEdit = 112,
    [Display(GroupName = "Vuln Notes", Name = "Delete Vuln Notes", Description = "Can delete the vulnerability notes")]
    VulnNotesDelete = 113,
    [Display(GroupName = "Vuln Notes", Name = "Display Vuln Notes", Description = "Can add new vulnerability notes")]
    VulnNotesAdd = 114,
    
    [Display(GroupName = "KnowledgeBase", Name = "View/Read KnowledgeBase", Description = "Can read and view the knowledgebase pages")]
    KnowledgeBaseRead = 121,
    [Display(GroupName = "KnowledgeBase", Name = "Edit KnowledgeBase Pages", Description = "Can edit the knowledgebase pages")]
    KnowledgeBaseEdit = 122,
    [Display(GroupName = "KnowledgeBase", Name = "Delete KnowledgeBase Pages", Description = "Can delete the knowledgebase pages")]
    KnowledgeBaseDelete = 123,
    [Display(GroupName = "KnowledgeBase", Name = "Add New KnowledgeBase Pages", Description = "Can add new knowledgebase pages")]
    KnowledgeBaseAdd = 124,
    
    [Display(GroupName = "KnowledgeBaseCategory", Name = "View/Read KnowledgeBase Categories", Description = "Can read and view the knowledgebase categories")]
    KnowledgeBaseCategoryRead = 131,
    [Display(GroupName = "KnowledgeBaseCategory", Name = "Edit KnowledgeBase Categories", Description = "Can edit the knowledgebase categories")]
    KnowledgeBaseCategoryEdit = 132,
    [Display(GroupName = "KnowledgeBaseCategory", Name = "Delete KnowledgeBase Categories", Description = "Can delete the knowledgebase categories")]
    KnowledgeBaseCategoryDelete = 133,
    [Display(GroupName = "KnowledgeBaseCategory", Name = "Add New KnowledgeBase Categories", Description = "Can add new knowledgebase categories")]
    KnowledgeBaseCategoryAdd = 134,
    
    [Display(GroupName = "Logs", Name = "View/Read Logs", Description = "Can read and view the logs of the application")]
    LogsRead = 141,
    [Display(GroupName = "Logs", Name = "Delete Logs", Description = "Can delete the logs of the application")]
    LogsDelete = 142,
    
    [Display(GroupName = "Jobs", Name = "View The Current Jobs", Description = "Can read and view the background jobs running")]
    JobsRead = 151,
    
    [Display(GroupName = "Backup", Name = "Make Backup", Description = "Can make backups of the application data")]
    BackupRead = 161,
    [Display(GroupName = "Backup", Name = "Restore Backup", Description = "Can restore backups of the application data")]
    BackupRestore = 162,
    
    [Display(GroupName = "Organization", Name = "View/Read Organization", Description = "Can read and view the organization details")]
    OrganizationRead = 171,
    [Display(GroupName = "Organization", Name = "Edit Organization", Description = "Can edit the organization details")]
    OrganizationEdit = 172,
    
    [Display(GroupName = "ReportTemplates", Name = "View/Read Report Templates", Description = "Can read and view the report templates")]
    ReportTemplatesRead = 181,
    [Display(GroupName = "ReportTemplates", Name = "Edit Report Templates", Description = "Can edit the report templates")]
    ReportTemplatesEdit = 182,
    [Display(GroupName = "ReportTemplates", Name = "Delete Report Templates", Description = "Can delete the report templates")]
    ReportTemplatesDelete = 183,
    [Display(GroupName = "ReportTemplates", Name = "Add New Report Templates", Description = "Can add new report templates")]
    ReportTemplatesAdd = 184,
    
    [Display(GroupName = "ReportComponents", Name = "View/Read Report Components", Description = "Can read and view the report components")]
    ReportComponentsRead = 191,
    [Display(GroupName = "ReportComponents", Name = "Edit Report Components", Description = "Can edit the report components")]
    ReportComponentsEdit = 192,
    [Display(GroupName = "ReportComponents", Name = "Delete Report Components", Description = "Can delete the report components")]
    ReportComponentsDelete = 193,
    [Display(GroupName = "ReportComponents", Name = "Add New Report Components", Description = "Can add new report components")]
    ReportComponentsAdd = 194,
    
    [Display(GroupName = "Reports", Name = "View/Read Reports", Description = "Can read and view the reports")]
    ReportsRead = 201,
    [Display(GroupName = "Reports", Name = "Edit Reports", Description = "Can edit the reports")]
    ReportsEdit = 202,
    [Display(GroupName = "Reports", Name = "Delete Reports", Description = "Can delete the reports")]
    ReportsDelete = 203,
    [Display(GroupName = "Reports", Name = "Generate New Reports", Description = "Can generate new reports")]
    ReportsAdd = 204,
    [Display(GroupName = "Reports", Name = "Download/Export Reports", Description = "Can download/export the reports")]
    ReportsDownload= 205,
    
    [Display(GroupName = "Users", Name = "View/Read Users", Description = "Can read and view the users in the application")]
    UsersRead = 211,
    [Display(GroupName = "Users", Name = "Edit Users", Description = "Can edit the users in the application")]
    UsersEdit = 212,
    [Display(GroupName = "Users", Name = "Delete Users", Description = "Can delete the users in the application")]
    UsersDelete = 213,
    [Display(GroupName = "Users", Name = "Add New Users", Description = "Can add new users to the application")]
    UsersAdd = 214,
    
    [Display(GroupName = "Roles", Name = "View/Read Roles", Description = "Can read and view the roles in the application")]
    RolesRead = 221,
    [Display(GroupName = "Roles", Name = "Edit Roles", Description = "Can edit the roles in the application")]
    RolesEdit = 222,
    [Display(GroupName = "Roles", Name = "Delete Roles", Description = "Can delete the roles in the application")]
    RolesDelete = 223,
    [Display(GroupName = "Roles", Name = "Add New Roles", Description = "Can add new roles to the application")]
    RolesAdd = 224,
    
    [Display(GroupName = "Calendar", Name = "View Calendar", Description = "Can read and view the personal calendar")]
    CalendarRead = 231,
    
    [Display(GroupName = "Workspaces", Name = "View My Workspaces", Description = "Can read and view the personal workspaces")]
    WorkspacesRead = 241,
    
    [Display(GroupName = "Targets", Name = "View/Read Targets", Description = "Can read and view the targets")]
    TargetsRead = 251,
    [Display(GroupName = "Targets", Name = "Edit Targets", Description = "Can edit the targets")]
    TargetsEdit = 252,
    [Display(GroupName = "Targets", Name = "Delete Targets", Description = "Can delete the targets")]
    TargetsDelete = 253,
    [Display(GroupName = "Targets", Name = "Add New Targets", Description = "Can add new targets")]
    TargetsAdd = 254,
    [Display(GroupName = "Targets", Name = "Import Targets", Description = "Can import new targets")]
    TargetsImport = 255,
    
    [Display(GroupName = "Targets Services", Name = "View/Read Targets Services", Description = "Can read and view the targets services")]
    TargetsServicesRead = 261,
    [Display(GroupName = "Targets Services", Name = "Edit Targets Services", Description = "Can edit the targets services")]
    TargetsServicesEdit = 262,
    [Display(GroupName = "Targets Services", Name = "Delete Targets Services", Description = "Can delete the targets services")]
    TargetsServicesDelete = 263,
    [Display(GroupName = "Targets Services", Name = "Add New Targets Services", Description = "Can add new targets services")]
    TargetsServicesAdd = 264,
    
    [Display(GroupName = "Notes", Name = "View/Read Notes", Description = "Can read and view the personal notes")]
    NotesRead = 271,
    [Display(GroupName = "Notes", Name = "Edit Notes", Description = "Can edit the personal notes")]
    NotesEdit = 272,
    [Display(GroupName = "Notes", Name = "Delete Notes", Description = "Can delete the personal notes")]
    NotesDelete = 273,
    [Display(GroupName = "Notes", Name = "Add New Notes", Description = "Can add new personal notes")]
    NotesAdd = 274,
    
    [Display(GroupName = "Task Notes", Name = "View/Read Task Notes", Description = "Can read and view the task notes")]
    TaskNotesRead = 281,
    [Display(GroupName = "Task Notes", Name = "Edit Task Notes", Description = "Can edit the task notes")]
    TaskNotesEdit = 282,
    [Display(GroupName = "Task Notes", Name = "Delete Task Notes", Description = "Can delete the task notes")]
    TaskNotesDelete = 283,
    [Display(GroupName = "Task Notes", Name = "Add New Task Notes", Description = "Can add new task notes")]
    TaskNotesAdd = 284,
    
    [Display(GroupName = "Task Attachments", Name = "View/Read Task Attachments", Description = "Can read and view the task attachments")]
    TaskAttachmentsRead = 291,
    [Display(GroupName = "Task Attachments", Name = "Edit Task Attachments", Description = "Can edit the task attachments")]
    TaskAttachmentsEdit = 292,
    [Display(GroupName = "Task Attachments", Name = "Delete Task Attachments", Description = "Can delete the task attachments")]
    TaskAttachmentsDelete = 293,
    [Display(GroupName = "Task Attachments", Name = "Add New Task Attachments", Description = "Can add new task attachments")]
    TaskAttachmentsAdd = 294,
    [Display(GroupName = "Task Attachments", Name = "Download Task Attachments", Description = "Can download the task attachments")]
    TaskAttachmentsDownload = 295,
    
    [Display(GroupName = "Task Targets", Name = "View/Read Task Targets", Description = "Can read and view the task targets")]
    TaskTargetsRead = 301,
    [Display(GroupName = "Task Targets", Name = "Edit Task Targets", Description = "Can edit the task targets")]
    TaskTargetsEdit = 302,
    [Display(GroupName = "Task Targets", Name = "Delete Task Targets", Description = "Can delete the task targets")]
    TaskTargetsDelete = 303,
    [Display(GroupName = "Task Targets", Name = "Add New Task Targets", Description = "Can add new task targets")]
    TaskTargetsAdd = 304,
    
    
    [Display(GroupName = "Vuln Targets", Name = "View/Read Vuln Targets", Description = "Can read and view the vulnerability targets")]
    VulnTargetsRead = 311,
    [Display(GroupName = "Vuln Targets", Name = "Edit Vuln Targets", Description = "Can edit the vulnerability targets")]
    VulnTargetsEdit = 312,
    [Display(GroupName = "Vuln Targets", Name = "Delete Vuln Targets", Description = "Can delete the vulnerability targets")]
    VulnTargetsDelete = 313,
    [Display(GroupName = "Vuln Targets", Name = "Add New Vuln Targets", Description = "Can add new vulnerability targets")]
    VulnTargetsAdd = 314,
    
    [Display(GroupName = "Jira", Name = "View/Read Jira", Description = "Can read and view the Jira issues")]
    JiraRead = 321,
    [Display(GroupName = "Jira", Name = "Edit Jira", Description = "Can edit the Jira issues")]
    JiraEdit = 322,
    [Display(GroupName = "Jira", Name = "Delete Jira", Description = "Can delete the Jira issues")]
    JiraDelete = 323,
    [Display(GroupName = "Jira", Name = "Add New Jira", Description = "Can add new Jira issues")]
    JiraAdd = 324,
    
    [Display(GroupName = "Jira Comments", Name = "View/Read Jira Comments", Description = "Can read and view the Jira comments")]
    JiraCommentsRead = 331,
    [Display(GroupName = "Jira Comments", Name = "Edit Jira Comments", Description = "Can edit the Jira comments")]
    JiraCommentsEdit = 332,
    [Display(GroupName = "Jira Comments", Name = "Delete Jira Comments", Description = "Can delete the Jira comments")]
    JiraCommentsDelete = 333,
    [Display(GroupName = "Jira Comments", Name = "Add New Jira Comments", Description = "Can add new Jira comments")]
    JiraCommentsAdd = 334,

    [Display(GroupName = "Vault", Name = "View/Read link", Description = "Can read and view the Vault entries")]
    VaultRead = 341,
    [Display(GroupName = "Vault", Name = "Display link", Description = "Can edit the Vault entries")]
    VaultEdit = 342,
    [Display(GroupName = "Vault", Name = "Display link", Description = "Can delete the Vault entries")]
    VaultDelete = 343,
    [Display(GroupName = "Vault", Name = "Display link", Description = "Can add new Vault entries")]
    VaultAdd = 344,

    [Display(GroupName = "Checklists", Name = "View/Read Checklists", Description = "Can read and view the Checklists")]
    ChecklistsRead = 351,
    [Display(GroupName = "Checklists", Name = "Edit Checklists", Description = "Can edit the Checklists")]
    ChecklistsEdit = 352,
    [Display(GroupName = "Checklists", Name = "Delete Checklists", Description = "Can delete the Checklists")]
    ChecklistsDelete = 353,
    [Display(GroupName = "Checklists", Name = "Add New Checklists", Description = "Can add new Checklists")]
    ChecklistsAdd = 354,
    
    [Display(GroupName = "Checklist Templates", Name = "View/Read Checklist Templates", Description = "Can read and view checklist templates")]
    ChecklistTemplatesRead = 355,
    [Display(GroupName = "Checklist Templates", Name = "Edit Checklist Templates", Description = "Can edit existing checklist templates")]
    ChecklistTemplatesEdit = 356,
    [Display(GroupName = "Checklist Templates", Name = "Delete Checklist Templates", Description = "Can delete existing checklist templates")]
    ChecklistTemplatesDelete = 357,
    [Display(GroupName = "Checklist Templates", Name = "Add New Checklist Templates", Description = "Can create new checklist templates")]
    ChecklistTemplatesAdd = 358,

    
    [Display(GroupName = "Rss News", Name = "View/Read RSS News", Description = "Can read and view the RSS News")]
    RssNewsRead = 361,
    [Display(GroupName = "Rss Sources", Name = "View/Read RSS Sources", Description = "Can view the RSS Sources")]
    RssSourcesRead = 362,
    [Display(GroupName = "Rss Sources", Name = "Edit RSS Sources", Description = "Can edit the RSS Sources")]
    RssSourcesEdit = 363,
    [Display(GroupName = "Rss Sources", Name = "Delete RSS Sources", Description = "Can delete the RSS Sources")]
    RssSourcesDelete = 364,
    [Display(GroupName = "Rss Sources", Name = "Add RSS Sources", Description = "Can add new RSS Sources")]
    RssSourcesAdd = 365,
    [Display(GroupName = "Rss Categories", Name = "View/Read RSS Categories", Description = "Can read and view the RSS Categories")]
    RssCategoriesRead = 366,
    [Display(GroupName = "Rss Categories", Name = "Edit RSS Categories", Description = "Can edit the RSS Categories")]
    RssCategoriesEdit = 367,
    [Display(GroupName = "Rss Categories", Name = "Delete RSS Categories", Description = "Can delete the RSS Categories")]
    RssCategoriesDelete = 368,
    [Display(GroupName = "Rss Categories", Name = "Add RSS Categories", Description = "Can add new RSS Categories")]
    RssCategoriesAdd = 369,
    
    [Display(GroupName = "Vuln Custom Fields", Name = "Read Vuln Custom Fields", Description = "Can read/view Custom Fields")]
    VulnCustomFieldsRead = 380,
    [Display(GroupName = "Vuln Custom Fields", Name = "Add Vuln Custom Fields", Description = "Can add new Custom Fields")]
    VulnCustomFieldsAdd = 381,
    [Display(GroupName = "Vuln Custom Fields", Name = "Edit Vuln Custom Fields", Description = "Can edit Custom Fields")]
    VulnCustomFieldsEdit = 382,
    [Display(GroupName = "Vuln Custom Fields", Name = "Delete Vuln Custom Fields", Description = "Can delete Custom Fields")]
    VulnCustomFieldsDelete = 383,
    
    [Display(GroupName = "Project Custom Fields", Name = "Read Project Custom Fields", Description = "Can read Custom Fields")]
    ProjectCustomFieldsRead = 384,
    [Display(GroupName = "Project Custom Fields", Name = "Add Project Custom Fields", Description = "Can add new Custom Fields")]
    ProjectCustomFieldsAdd = 385,
    [Display(GroupName = "Project Custom Fields", Name = "Edit Project Custom Fields", Description = "Can edit Custom Fields")]
    ProjectCustomFieldsEdit = 386,
    [Display(GroupName = "Project Custom Fields", Name = "Delete Project Custom Fields", Description = "Can delete Custom Fields")]
    ProjectCustomFieldsDelete = 387,
    
    [Display(GroupName = "Client Custom Fields", Name = "Read Client Custom Fields", Description = "Can read Custom Fields")]
    ClientCustomFieldsRead = 388,
    [Display(GroupName = "Client Custom Fields", Name = "Add Client Custom Fields", Description = "Can add new Custom Fields")]
    ClientCustomFieldsAdd = 389,
    [Display(GroupName = "Client Custom Fields", Name = "Edit Client Custom Fields", Description = "Can edit Custom Fields")]
    ClientCustomFieldsEdit = 390,
    [Display(GroupName = "Client Custom Fields", Name = "Delete Client Custom Fields", Description = "Can delete Custom Fields")]
    ClientCustomFieldsDelete = 391,
    
    [Display(GroupName = "Target Custom Fields", Name = "Read Target Custom Fields", Description = "Can read Custom Fields")]
    TargetCustomFieldsRead = 392,
    [Display(GroupName = "Target Custom Fields", Name = "Add Target Custom Fields", Description = "Can add new Custom Fields")]
    TargetCustomFieldsAdd = 393,
    [Display(GroupName = "Target Custom Fields", Name = "Edit Target Custom Fields", Description = "Can edit Custom Fields")]
    TargetCustomFieldsEdit = 394,
    [Display(GroupName = "Target Custom Fields", Name = "Delete Target Custom Fields", Description = "Can delete Custom Fields")]
    TargetCustomFieldsDelete = 395,
    
    [Display(GroupName = "AI Service", Name = "Use AI Service", Description = "Can use the AI generation service")]
    AIServiceUsage = 60000,
    [Display(GroupName = "AI Service", Name = "Use AI Service", Description = "Can use the AI chat service")]
    AIChatUsage = 60001,

    [Display(GroupName = "CVE Management", Name = "View/Read CVEs", Description = "Can read and view CVE information")]
    CveRead = 450,
    [Display(GroupName = "CVE Management", Name = "Edit CVEs", Description = "Can edit CVE information")]
    CveEdit = 451,
    [Display(GroupName = "CVE Management", Name = "Delete CVEs", Description = "Can delete CVE information")]
    CveDelete = 452,
    [Display(GroupName = "CVE Management", Name = "Add New CVEs", Description = "Can add new CVE information")]
    CveCreate = 453,
    [Display(GroupName = "CVE Management", Name = "Import CVEs", Description = "Can import CVE data from external sources")]
    CveImport = 454,
    [Display(GroupName = "CVE Management", Name = "Export CVEs", Description = "Can export CVE data")]
    CveExport = 455,
    [Display(GroupName = "CVE Management", Name = "Sync CVEs", Description = "Can trigger CVE synchronization with external sources")]
    CveSync = 456,
    [Display(GroupName = "CVE Management", Name = "Manage CVE Sources", Description = "Can manage CVE data sources")]
    CveSources = 457,
    [Display(GroupName = "CVE Management", Name = "Manage CVE Subscriptions", Description = "Can manage CVE subscriptions and alerts")]
    CveSubscriptions = 458,
    [Display(GroupName = "CVE Management", Name = "View CVE Notifications", Description = "Can view CVE notifications and alerts")]
    CveNotifications = 459,
    [Display(GroupName = "CVE Management", Name = "Manage CVE Dashboard", Description = "Can access and customize CVE dashboard")]
    CveDashboard = 460,

    
}