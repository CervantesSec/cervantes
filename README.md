![Cervantes logo](https://raw.githubusercontent.com/CervantesSecurity/.github/main/profile/logo-horizontal2.png)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

[![GITHUB](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/CervantesSec)
[![TWITTER](https://img.shields.io/badge/Twitter-1DA1F2?style=for-the-badge&logo=twitter&logoColor=white)](https://twitter.com/Cervantes_Sec)
[![WEB](https://img.shields.io/badge/website-000000?style=for-the-badge&logo=About.me&logoColor=white)](https://www.cervantessec.org)
[![DISCORD](https://img.shields.io/badge/Discord-7289DA?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/BvzNjT3Qzc)
[![DOCS](https://img.shields.io/badge/-DOCS-success?style=for-the-badge&logo=readthedocs&logoColor=white)](https://docs.cervantessec.org/)

Cervantes is an open-source, collaborative platform designed specifically for pentesters and red teams. It serves as a comprehensive management tool, streamlining the organization of projects, clients, vulnerabilities, and reports in a single, centralized location.

By facilitating efficient data management and providing a unified workspace, Cervantes aims to significantly reduce the time and effort required in the coordination and execution of penetration testing activities.

## Supported

Cervantes is an [OWASP Foundation](https://owasp.org/www-project-cervantes/) Project

<img src="https://raw.githubusercontent.com/CervantesSec/.github/main/profile/owasp.png"  width="500" height="150">

## Technologies

![DOTNET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![CSHARP](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![RIDER](https://img.shields.io/badge/Rider-000000?style=for-the-badge&logo=Rider&logoColor=white)
![JS](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)
![HTML](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white)
![CSS](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white)

## Features
* OpenSource
* Multiplatform
* Multilanguage
* Team Collaboration
* Checklists
* OWASP Compliance Reports
* Built-in dashboards and analytics
* Manage your clients and Offensive Security projects
* One click reports creation
* And more

## Runtime requirements

- Docker
- Docker compose

## How to run it locally with Docker compose

* First you need to clone this repository

```sh
git clone https://github.com/CervantesSec/docker.git
```

* After that you need to start your docker containers:

```sh
docker-compose -p cervantes -f docker-compose.yml up -d
```

* After this, open your browser at http://localhost or https://localhost and you will see the Cervantes login page.

### Default User and Password

When you first launch the Cervantes application, a default user is created for you. The default username is `admin@cervantes.local`.

The password for this user is generated randomly during the creation of the application container and the first launch of the application. This means that the password is unique for each instance of the application and provides an additional layer of security.

<img src="https://raw.githubusercontent.com/CervantesSec/.github/main/profile/password-generation.png"  width="800" height="500">

Please note that it's important to change the default password as soon as possible to ensure the security of your application. You can do this by logging in with the default user and navigating to the user settings page.

Remember, the security of your application is paramount. Always use strong, unique passwords and change them regularly.


## How to run it locally from source

### Requirements

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgresSQL](https://www.postgresql.org/)
- [Atlassian Jira Server](https://www.atlassian.com/es/software/jira) (Optional only if you want to use Jira Integration)

#### How to run it locally

To install the Cervantes application from the source code, you can follow these steps:

* **Clone the Repository**: First, you need to clone the repository from GitHub. You can do this by running the following command in your terminal:

```bash
git clone https://github.com/CervantesSec/cervantes.git
```

* **Navigate to the Project Directory**: Once the repository is cloned, navigate to the project directory:

```bash
cd Cervantes
```

* **Edit appsettings.json**: To use the application you need to edit the appsettings.json file inside the Cervantes.Web folder.

**Database Connection String**
The database connection string is used to connect your application to your database. It usually includes the server name, database name, and authentication details. Here's an example of how it might look in your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=myServerAddress;Database=myDataBase;Username=myUsername;Password=myPassword"
  }
}
```

Replace `myServerAddress`, `myDataBase`, `myUsername`, and `myPassword` with your actual database details.

* **Install Dependencies**: The project uses .NET 8.0, so you need to have it installed on your machine. If you don't have it, you can download it from the [official .NET website](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Once .NET is installed, you can install the project dependencies by running:

```bash
dotnet restore
```

* **Build the Project**: After the dependencies are installed, you can build the project:

```bash
dotnet build
```

* **Run the Project**: Finally, you can run the project:

```bash
dotnet run --project Cervantes.Web/Cervantes.Web.csproj
```

The application should now be running at `http://localhost:5235`.

Please note that this is a basic installation guide and the actual process might vary depending on the project's specific configuration and requirements. For example, if the project uses a database, you might need to set up the database and update the connection string in the configuration file.

## How to contribute

Here are some things you could do to become a contributor:

- **★ Star this project on GitHub ★**
- Suggest new features or ideas
- Improve the code of the platform components
- Report security issues

Before you jump to make any changes make sure you have read the [contributing guidelines](CONTRIBUTING.md). This would save us all time. Thanks!



[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=YS42VF2N9GANA)

## Security

Please report Security issues via our [disclosure policy](https://github.com/CervantesSec/cervantes/blob/main/SECURITY.md).

## How to report bugs

If you have bugs to report please use the [issues](https://github.com/CervantesSec/cervantes/issues) tab on GitHub to submit the details.

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/mesquidar"><img src="https://avatars.githubusercontent.com/u/16049893?v=4?s=100" width="100px;" alt="Ruben Mesquida"/><br /><sub><b>Ruben Mesquida</b></sub></a><br /><a href="#business-mesquidar" title="Business development">💼</a> <a href="https://github.com/CervantesSec/cervantes/commits?author=mesquidar" title="Code">💻</a> <a href="https://github.com/CervantesSec/cervantes/commits?author=mesquidar" title="Documentation">📖</a> <a href="#translation-mesquidar" title="Translation">🌍</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

## License


This project is licensed under the GNU Affero General Public License (AGPL-3.0), except for specific components that remain licensed under the Apache License 2.0.

[![AGPL](https://img.shields.io/badge/license-AGPL--3.0-green)](https://www.gnu.org/licenses/agpl-3.0.html)
[![Apache](https://img.shields.io/badge/license-Apache--2.0-green)](https://www.apache.org/licenses/LICENSE-2.0.html)

### Summary:

AGPL-3.0 applies to the majority of the project.

Apache License 2.0 applies to the components listed in the NOTICE file.

For more details, see the full LICENSE and NOTICE files included in this repository.

## Copyright Notice

Copyright (C) 2025 Ruben Mesquida Gomila

OWASP Cervantes and all contributions are protected under their respective licenses. For more information on license terms, visit:

- https://www.gnu.org/licenses/agpl-3.0.html

- https://www.apache.org/licenses/LICENSE-2.0.html
