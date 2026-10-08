# Security

No technology is perfect, and OWASP Cervantes believes that working with skilled security researchers across the globe is crucial in identifying weaknesses in any technology. If you believe you've found a security issue in our product, we encourage you to notify us. We welcome working with you to resolve the issue promptly.

## Disclosure Policy and Process

* Let us know as soon as possible upon discovery of a potential security issue, through either of these channels:
  * Email at ruben.mesquida@owasp.org.
  * Our Vulnerability Disclosure Program (VDP) on [Grayback](https://www.grayback.es/).
* That "security advisory" will also allow us to have a temporary private fork, to work on the fix in confidentiality.
* Once a fix is ready, we will coordinate a release.
* If you've contributed the fix, you will be credited for it.

> Make a good faith effort to avoid privacy violations, destruction of data, and interruption or degradation of our service. Only interact with accounts you own or with explicit permission of the account holder.

## Access control model

Before reporting an access control issue, please take into account how access works in Cervantes:

* **Roles grant read access to modules.** Each role has a set of permissions, such as `ReportsRead`, `VulnsRead` or `TasksRead`. A user with a read permission can view that module's data in every project, not only in the projects they are assigned to. This is intended: Cervantes is built for pentest teams that share knowledge across engagements.
* **Project membership is required to change project content.** Adding, editing or deleting project content (vulnerabilities, tasks, targets, notes, reports, attachments, etc.) also requires the user to be a member of that project. This applies to every role, including administrators.
* **Project lifecycle and membership are role capabilities.** Creating, editing or deleting a project, and adding or removing its members, are governed by role permissions (`ProjectsAdd`, `ProjectsEdit`, `ProjectsDelete`, `ProjectMembersAdd`, `ProjectMembersDelete`) and do not require membership. The default `Manager` and `Admin` roles can therefore manage any project and add any user, including themselves, to it. Every membership change is recorded in the audit log and, when email is enabled, the existing members of the project are notified. If your organization needs per-project leads, remove those permissions from the role on the **Roles** page.
* Some modules, such as the Vault, checklists and AI project chats, also require project membership to read. A user who holds the role permissions above can obtain that access by adding themselves to the project.

Administrators decide what each role can read on the **Roles** page (`/roles`). The default `User` role includes `ReportsRead` and `VulnsRead`, so every user with that role can view the findings and reports of all clients. If your organization needs need-to-know separation, remove those permissions from the relevant roles.

A user with a read permission viewing data from a project they are not a member of, or a user with the role permissions above managing a project or its members, is expected behavior. Ways to add, change or delete project content without being a member of the project, or to manage projects or their members without the corresponding role permission, are in scope, and we would love to hear about them.

## AI features

AI features are disabled by default. When an administrator enables them (the `AIConfiguration` section in `appsettings.json`), Cervantes sends project data to the AI provider the administrator configured:

* **Generation features** (vulnerability, executive summary and custom prompts) are available to roles with the `AIServiceUsage` permission. They send the vulnerability name; the project name, dates and description, the client name, member names, target names and vulnerability names with their risk and affected targets; or the text the user writes.
* **Chats** require the `AIChatUsage` permission. When a project or document chat is created, the project data or the document text is sent to the text embedding provider, and the relevant parts are sent to the chat provider with each message. Project chats include project, client, task, vulnerability (proof of concept included) and target details. They do not include vault entries, email addresses or phone numbers. Only project members can create a project chat.
* Chat context is stored in the Cervantes database, in the tables with the `sk_` prefix, until the chat is deleted.

Sending this data to the provider the administrator configured is expected behavior. Proofs of concept often contain credentials or session tokens captured during a test. For engagements with sensitive data, use a provider you control, such as a local model, or keep AI features disabled.

## Exclusions

While researching, we'd like to ask you to refrain from:

* Denial of service
* Spamming
* Social engineering (including phishing) of Cervantes staff
* Any physical attempts against Cervantes property or cloud hosted environments

## Safe Harbor

Any activities conducted in a manner consistent with this policy will be considered authorized conduct and we will not initiate legal action against you. If legal action is initiated by a third party against you in connection with activities conducted under this policy, we will take steps to make it known that your actions were conducted in compliance with this policy.

Thank you for helping keep Cervantes and our users safe!