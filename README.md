![Cervantes logo](https://raw.githubusercontent.com/CervantesSecurity/.github/main/profile/logo-horizontal2.png)

[![GITHUB](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/CervantesSec)
[![TWITTER](https://img.shields.io/badge/Twitter-1DA1F2?style=for-the-badge&logo=twitter&logoColor=white)](https://twitter.com/Cervantes_Sec)
[![WEB](https://img.shields.io/badge/website-000000?style=for-the-badge&logo=About.me&logoColor=white)](https://www.cervantessec.org)
[![DISCORD](https://img.shields.io/badge/Discord-7289DA?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/BvzNjT3Qzc)
[![DOCS](https://img.shields.io/badge/-DOCS-success?style=for-the-badge&logo=readthedocs&logoColor=white)](https://docs.cervantessec.org/)

Cervantes is an opensource collaborative platform for pentesters or red teams who want to save time to manage their projects, clients, vulnerabilities and reports in one place.

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

## Try Cervantes 

There is a live demo running on [http://demo.cervantessec.org](http://demo.cervantessec.org).

The demo server has 3 users to show the different permission levels. The credentials for these users are:

| Username              | Password      | Role          |
|-----------------------|---------------|---------------|
| admin@cervantes.local | Admin123.     | Administrator |
| su@cervantes.local    | SuperUser123. | SuperUser     |
| user@cervantes.local  | User123.      | User          |

This demo has limited functionalities, file upload & report generation, logs... If you want to test the full functionalities use the dockerized version.

## Features
* OpenSource
* Multiplatform
* Multilanguage
* Team Collaboration
* Checklists
* OWASP Compliance Reports
* BuiltIn dashboards and analytics
* Manage your clients and Offensive Security projects
* One click reports creation
* And more

## Runtime requirements

- Docker
- Docker compose

## How to run it locally with Docker compose

1. First you need to clone this repository

```sh
git clone https://github.com/CervantesSec/docker.git
```

2. After that you need to start your docker containers:

```sh
docker-compose -p cervantes -f docker-compose.yml up -d
```

3. After this, open your browser at http://localhost


4. Default User is:

```sh
admin@cervantes.local - Admin123.
```

## How to run it locally from source
1. Install dotnet sdk from https://dotnet.microsoft.com/en-us/download


2. Install PostgreSQL https://www.postgresql.org/download/ 


3. Clone this repository

```sh
git clone https://github.com/CervantesSec/cervantes.git
```

4. In Cervantes.Web -> appsettings.json edit the DefaultConnection with your database parameters

```sh
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=cervantes;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "Cervantes.*": "Trace"
    }
  },
  "AllowedHosts": "*",
  "EmailConfiguration": {
    "Enabled": false,
    "Name": "Cervantes",
    "From": "cervantes@cervantes.local",
    "SmtpServer": "localhost",
    "SmtpPort": 1025,
    "SmtpUsername": "cervantes@cervantes.local",
    "SmtpPassword": "cervantes"
  },
  "JiraConfiguration": {
    "Enabled": false,
    "Auth": "Basic",
    "Url": "",
    "Project": "",
    "User": "",
    "Password": "",
    "ConsumerKey": "",
    "ConsumerSecret": "",
    "OAuthAccessToken": "",
    "OAuthTokenSecret": ""
    
  }
}
```

6. Run the project 

```sh
dotnet run --project /CERVANTES_PATH/Cervantes.Web/
```

7. After this, open your browser at http://localhost:5001


8. Default User is:

```sh
admin@cervantes.local - Admin123.
```

## How to contribute

Here are some things you could do to become a contributor:

- **★ Star this project on Github ★**
- Suggest new features or ideas
- Improve the code of the platform components
- Report security issues

Before you jump to make any changes make sure you have read the [contributing guidelines](CONTRIBUTING.md). This would save us all time. Thanks!



[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=YS42VF2N9GANA)

## Security

Please report Security issues via our [disclosure policy](https://github.com/CervantesSec/cervantes/blob/main/SECURITY.md).

## How to report bugs

If you have bugs to report please use the [issues](https://github.com/CervantesSec/cervantes/issues) tab on Github to submit the details.
