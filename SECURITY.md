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
* **Project membership is required to change data.** Adding, editing or deleting project data (vulnerabilities, tasks, targets, notes, reports, etc.) also requires the user to be a member of that project.
* Some modules, such as the Vault and checklists, also require project membership to read.

Administrators decide what each role can read on the **Roles** page (`/roles`). The default `User` role includes `ReportsRead` and `VulnsRead`, so every user with that role can view the findings and reports of all clients. If your organization needs need-to-know separation, remove those permissions from the relevant roles.

A user with a read permission viewing data from a project they are not a member of is expected behavior. Ways to add, change or delete project data without being a member of the project are in scope, and we would love to hear about them.

## Exclusions

While researching, we'd like to ask you to refrain from:

* Denial of service
* Spamming
* Social engineering (including phishing) of Cervantes staff
* Any physical attempts against Cervantes property or cloud hosted environments

## Safe Harbor

Any activities conducted in a manner consistent with this policy will be considered authorized conduct and we will not initiate legal action against you. If legal action is initiated by a third party against you in connection with activities conducted under this policy, we will take steps to make it known that your actions were conducted in compliance with this policy.

Thank you for helping keep Cervantes and our users safe!