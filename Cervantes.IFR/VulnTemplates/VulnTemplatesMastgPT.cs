using Cervantes.CORE.Entities;

namespace Cervantes.IFR.VulnTemplates;

public class VulnTemplatesMastgPT
{
    public static Vuln InsecureDataStorage => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Armazenamento Inseguro de Dados",
        Description =
            "O aplicativo não armazena dados sensíveis de forma segura, potencialmente os expondo a acessos não autorizados.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar mecanismos de armazenamento seguro como criptografia para os dados sensíveis armazenados localmente.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Acesso não autorizado a dados sensíveis do usuário, possíveis violações de privacidade e problemas de conformidade regulatória.",
        ProofOfConcept =
            "1. Obter acesso root/jailbreak ao dispositivo.\n2. Navegar ao diretório de dados do aplicativo.\n3. Localizar e acessar arquivos de dados sensíveis não criptografados.",
        cve = "N/A"
    };

    public static Vuln InsecureCryptography => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Criptográfica Insegura",
        Description =
            "O aplicativo utiliza algoritmos criptográficos fracos ou obsoletos, ou implementa algoritmos fortes de maneira incorreta.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar algoritmos criptográficos fortes e atualizados e seguir as melhores práticas da indústria para seu uso.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível descriptografia de dados sensíveis, comprometimento da integridade das comunicações criptografadas.",
        ProofOfConcept =
            "1. Descompilar o aplicativo e analisar o código.\n2. Identificar o uso de algoritmos fracos (ex., MD5, SHA1) ou tamanhos de chave pequenos.\n3. Demonstrar a viabilidade de quebrar a criptografia em um tempo razoável.",
        cve = "N/A"
    };

    public static Vuln InsecureAuthentication => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Mecanismo de Autenticação Inseguro",
        Description =
            "O mecanismo de autenticação do aplicativo é fraco ou está implementado incorretamente, permitindo acessos não autorizados.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar mecanismos de autenticação seguros, como autenticação multifator, e seguir as melhores práticas da plataforma para autenticação local.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Acesso não autorizado a contas de usuário, possível vazamento de dados e comprometimento da privacidade do usuário.",
        ProofOfConcept =
            "1. Analisar o fluxo de autenticação do aplicativo.\n2. Identificar e explorar fraquezas (ex., falta de limite de tentativas, políticas de senhas fracas).\n3. Demonstrar acesso não autorizado a uma conta de usuário.",
        cve = "N/A"
    };

    public static Vuln InsecureNetworkCommunication => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Comunicação de Rede Insegura",
        Description =
            "O aplicativo transmite dados sensíveis através de canais inseguros ou não valida corretamente os certificados do servidor.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar protocolos de comunicação de rede seguros (ex., TLS) e garantir a validação adequada de certificados.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Interceptação de dados sensíveis em trânsito, possíveis ataques de intermediário (man-in-the-middle).",
        ProofOfConcept =
            "1. Configurar um proxy para interceptar o tráfego do aplicativo.\n2. Identificar transmissões de dados não criptografados ou validação incorreta de certificados.\n3. Demonstrar a capacidade de ver ou modificar dados sensíveis em trânsito.",
        cve = "N/A"
    };

    public static Vuln PrivacyViolation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Violação de Privacidade",
        Description =
            "O aplicativo coleta, processa ou compartilha dados do usuário sem o consentimento adequado ou além do necessário para sua funcionalidade.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar práticas de minimização de dados, fornecer políticas de privacidade claras e oferecer controle ao usuário sobre seus dados.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Violação da privacidade do usuário, possíveis consequências legais e regulatórias, perda de confiança do usuário.",
        ProofOfConcept =
            "1. Analisar as práticas de coleta e compartilhamento de dados do aplicativo.\n2. Identificar casos de coleta ou compartilhamento excessivo de dados.\n3. Comparar o manuseio real de dados com as políticas de privacidade estabelecidas e o consentimento do usuário.",
        cve = "N/A"
    };

    public static Vuln InsecureDataLeakage => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Vazamento de Dados Inseguro",
        Description =
            "O aplicativo vaza involuntariamente dados sensíveis através de vários canais como registros, backups ou área de transferência.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar práticas adequadas de manuseio de dados, evitar o registro de informações sensíveis e proteger os backups do aplicativo.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Exposição de dados sensíveis do usuário, possíveis violações de privacidade e brechas de segurança.",
        ProofOfConcept =
            "1. Habilitar o registro detalhado no dispositivo.\n2. Realizar várias ações no aplicativo.\n3. Examinar os registros em busca de dados sensíveis como senhas ou tokens de sessão.",
        cve = "N/A"
    };

    public static Vuln InsecureKeyManagement => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Gestão Insegura de Chaves Criptográficas",
        Description =
            "O aplicativo não gerencia adequadamente as chaves criptográficas, potencialmente expondo-as a acessos não autorizados.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar práticas seguras de gestão de chaves, utilizar armazenamento de chaves baseado em hardware quando disponível e evitar a codificação direta de chaves.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Comprometimento de dados criptografados, possibilidade de descriptografia não autorizada de informações sensíveis.",
        ProofOfConcept =
            "1. Descompilar o aplicativo e analisar o código.\n2. Procurar chaves criptográficas codificadas ou métodos inseguros de armazenamento de chaves.\n3. Demonstrar a capacidade de extrair ou usar as chaves sem autorização.",
        cve = "N/A"
    };

    public static Vuln InsecureLocalAuthentication => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Autenticação Local Insegura",
        Description =
            "O mecanismo de autenticação local do aplicativo (ex., biometria, PIN) está implementado incorretamente ou pode ser facilmente burlado.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:P/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar mecanismos seguros de autenticação local seguindo as melhores práticas da plataforma, usar técnicas criptográficas fortes para armazenar segredos de autenticação.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Acesso local não autorizado a funcionalidades ou dados sensíveis do aplicativo.",
        ProofOfConcept =
            "1. Analisar a implementação de autenticação local.\n2. Tentar burlar a autenticação (ex., manipulando o armazenamento local de dados).\n3. Demonstrar acesso não autorizado a recursos protegidos do aplicativo.",
        cve = "N/A"
    };

    public static Vuln InsecureCertificatePinning => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Falta de Fixação de Certificados",
        Description =
            "O aplicativo não implementa a fixação de certificados, tornando-o vulnerável a ataques de intermediário.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar a fixação de certificados para todos os endpoints remotos sob controle do desenvolvedor.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Possibilidade de ataques de intermediário, interceptação de dados sensíveis em trânsito.",
        ProofOfConcept =
            "1. Configurar um proxy com um certificado autoassinado.\n2. Tentar interceptar o tráfego HTTPS do aplicativo.\n3. Se bem-sucedido, demonstrar a capacidade de ver ou modificar os dados interceptados.",
        cve = "N/A"
    };

    public static Vuln InsecureIPC => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Comunicação Entre Processos Insegura",
        Description =
            "O aplicativo utiliza mecanismos IPC inseguros, potencialmente expondo dados sensíveis ou funcionalidade a outros aplicativos.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar mecanismos seguros de IPC, usar controles de acesso apropriados e validar todas as entradas IPC.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Possível exposição de dados sensíveis ou funcionalidade a aplicativos não autorizados.",
        ProofOfConcept =
            "1. Analisar os mecanismos IPC do aplicativo (ex., intents, provedores de conteúdo).\n2. Criar um aplicativo de prova de conceito que tente acessar componentes expostos.\n3. Demonstrar acesso não autorizado a dados sensíveis ou funcionalidade.",
        cve = "N/A"
    };

    public static Vuln InsecureWebView => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Insegura de WebView",
        Description =
            "A implementação de WebView do aplicativo é insegura, potencialmente permitindo a execução de scripts maliciosos ou acesso a dados sensíveis.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:H",
        Remediation =
            "Configuração segura de WebView, desabilitar JavaScript se não for necessário, validar todo o conteúdo carregado e usar HTTPS para conteúdo remoto.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível execução de scripts maliciosos, acesso não autorizado a dados ou funcionalidade do aplicativo.",
        ProofOfConcept =
            "1. Analisar a configuração de WebView no aplicativo.\n2. Se JavaScript estiver habilitado, tentar injetar e executar scripts maliciosos.\n3. Demonstrar acesso não autorizado a dados sensíveis ou funcionalidade do aplicativo através do WebView.",
        cve = "N/A"
    };

    public static Vuln InsecureDeepLinking => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Insegura de Links Profundos",
        Description =
            "Os links profundos ou o tratamento de esquemas URL do aplicativo são inseguros, potencialmente permitindo acesso não autorizado à funcionalidade do aplicativo.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar validação e sanitização adequada de parâmetros de links profundos, usar esquemas específicos do aplicativo e requerer confirmação do usuário para ações sensíveis.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Potencial acesso não autorizado à funcionalidade do aplicativo, vazamento de dados ou execução de ações não intencionadas.",
        ProofOfConcept =
            "1. Analisar a implementação de links profundos do aplicativo.\n2. Criar links profundos maliciosos direcionados a funcionalidade sensível.\n3. Demonstrar a capacidade de acessar recursos restritos ou vazar dados sensíveis através de links profundos.",
        cve = "N/A"
    };

    public static Vuln InsecureSessionHandling => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Gestão Insegura de Sessões",
        Description =
            "O aplicativo não gerencia adequadamente as sessões de usuário, potencialmente permitindo o sequestro de sessões ou acesso não autorizado.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.0,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar gestão segura de sessões, usar identificadores de sessão fortes, implementar mecanismos adequados de expiração e invalidação de sessões.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Acesso não autorizado a contas de usuário, possibilidade de ataques de sequestro de sessão.",
        ProofOfConcept =
            "1. Analisar o mecanismo de gestão de sessões do aplicativo.\n2. Tentar reutilizar tokens de sessão antigos ou manipular dados de sessão.\n3. Demonstrar acesso não autorizado à sessão de um usuário.",
        cve = "N/A"
    };

    public static Vuln InsecureTlsValidation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Validação TLS Insuficiente",
        Description =
            "O aplicativo não valida adequadamente os certificados TLS, tornando-o vulnerável a ataques de intermediário.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar validação adequada de certificados TLS, incluindo verificação de validade, revogação e nome de host correto.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Possibilidade de ataques de intermediário, interceptação de dados sensíveis em trânsito.",
        ProofOfConcept =
            "1. Configurar um proxy com um certificado TLS inválido.\n2. Tentar interceptar o tráfego HTTPS do aplicativo.\n3. Se bem-sucedido, demonstrar a capacidade de ver ou modificar os dados interceptados.",
        cve = "N/A"
    };

    public static Vuln InsecureClipboardUsage => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Uso Inseguro da Área de Transferência",
        Description =
            "O aplicativo permite que dados sensíveis sejam copiados para a área de transferência, potencialmente expondo-os a outros aplicativos.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 4.3,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:N",
        Remediation =
            "Prevenir a cópia de dados sensíveis para a área de transferência ou implementar mecanismos seguros de manipulação da área de transferência.",
        RemediationComplexity = RemediationComplexity.Low,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Possível exposição de dados sensíveis a aplicativos ou usuários não autorizados.",
        ProofOfConcept =
            "1. Identificar campos que contenham dados sensíveis no aplicativo.\n2. Tentar copiar estes dados para a área de transferência.\n3. Demonstrar a capacidade de colar os dados sensíveis em outro aplicativo.",
        cve = "N/A"
    };

    public static Vuln InsecureDataCaching => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Cache Inseguro de Dados",
        Description =
            "O aplicativo armazena dados sensíveis em cache de maneira insegura, potencialmente expondo-os a acessos não autorizados.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar mecanismos seguros de cache, evitar armazenar dados sensíveis em cache e limpar os caches adequadamente quando o aplicativo é fechado ou o usuário faz logout.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Possível exposição de dados sensíveis em cache a usuários ou aplicativos não autorizados.",
        ProofOfConcept =
            "1. Usar o aplicativo e realizar ações que envolvam dados sensíveis.\n2. Analisar os diretórios e arquivos de cache do aplicativo.\n3. Demonstrar a presença de dados sensíveis em arquivos de cache não criptografados ou facilmente acessíveis.",
        cve = "N/A"
    };

    public static Vuln InsecureBackupHandling => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Gestão Insegura de Backups",
        Description =
            "O aplicativo não protege adequadamente seus dados durante o processo de backup, potencialmente expondo informações sensíveis.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar mecanismos seguros de backup, excluir dados sensíveis dos backups ou criptografar os dados de backup.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Possível exposição de dados sensíveis através de backups inseguros.",
        ProofOfConcept =
            "1. Habilitar o backup de dados do aplicativo.\n2. Realizar um backup do dispositivo.\n3. Analisar o conteúdo do backup e demonstrar a presença de dados sensíveis não criptografados.",
        cve = "N/A"
    };

    public static Vuln InsufficientInputValidation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Validação de Entrada Insuficiente",
        Description =
            "O aplicativo não valida adequadamente a entrada do usuário, potencialmente levando a ataques de injeção ou comportamento inesperado.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.6,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation =
            "Implementar validação exaustiva de entrada para todos os dados fornecidos pelo usuário, usar consultas parametrizadas e sanitizar a entrada antes de seu uso.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Possibilidade de ataques de injeção, corrupção de dados ou acesso não autorizado a sistemas backend.",
        ProofOfConcept =
            "1. Identificar campos de entrada no aplicativo.\n2. Testar várias entradas malformadas ou maliciosas.\n3. Demonstrar comportamento inesperado ou ataque de injeção bem-sucedido.",
        cve = "N/A"
    };

    public static Vuln InsecureJailbreakRootDetection => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Detecção Insuficiente de Jailbreak/Root",
        Description =
            "O aplicativo não detecta ou responde adequadamente a dispositivos com jailbreak (iOS) ou root (Android), potencialmente expondo funcionalidade ou dados sensíveis.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar mecanismos robustos de detecção de jailbreak/root e responder apropriadamente quando um ambiente comprometido é detectado.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Possível exposição de funcionalidade ou dados sensíveis do aplicativo em dispositivos comprometidos.",
        ProofOfConcept =
            "1. Usar um dispositivo ou emulador com jailbreak/root.\n2. Executar o aplicativo e tentar acessar recursos sensíveis.\n3. Demonstrar que o aplicativo não detecta o ambiente comprometido e permite funcionalidade completa.",
        cve = "N/A"
    };

    public static Vuln InsecureCodeObfuscation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Ofuscação de Código Insuficiente",
        Description =
            "O código do aplicativo não está adequadamente ofuscado, facilitando a engenharia reversa e a compreensão da lógica do aplicativo por atacantes.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.3,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:L",
        Remediation = "Implementar técnicas sólidas de ofuscação de código para dificultar a engenharia reversa.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Facilita a engenharia reversa da lógica do aplicativo, possível exposição de algoritmos proprietários ou mecanismos de segurança.",
        ProofOfConcept =
            "1. Descompilar o aplicativo usando ferramentas apropriadas.\n2. Analisar o código descompilado para avaliar sua legibilidade e compreensibilidade.\n3. Demonstrar a facilidade de entender a lógica crítica do aplicativo ou mecanismos de segurança.",
        cve = "N/A"
    };

    public static Vuln InsecureRuntimeIntegrityChecks => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Falta de Verificações de Integridade em Tempo de Execução",
        Description =
            "O aplicativo não realiza verificações de integridade em tempo de execução, tornando-o vulnerável à injeção de código ou manipulação durante a execução.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar verificações de integridade em tempo de execução para detectar e responder a modificações de código ou injeções durante a execução do aplicativo.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possibilidade de injeção de código em tempo de execução, evasão de controles de segurança ou manipulação do comportamento do aplicativo.",
        ProofOfConcept =
            "1. Usar uma ferramenta como Frida para injetar código no aplicativo em execução.\n2. Modificar funcionalidade crítica do aplicativo ou contornar verificações de segurança.\n3. Demonstrar que o aplicativo não detecta nem responde às modificações em tempo de execução.",
        cve = "N/A"
    };

    public static Vuln InsecureAppPackaging => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Empacotamento Inseguro do Aplicativo",
        Description =
            "O pacote do aplicativo contém informação sensível ou não está devidamente assinado, tornando-o vulnerável à manipulação ou divulgação de informação.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Garantir a assinatura adequada do aplicativo, remover informação sensível do pacote e implementar verificações adicionais de integridade.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possibilidade de ataques de reempacotamento do aplicativo, exposição de informação sensível no pacote.",
        ProofOfConcept =
            "1. Extrair e analisar o pacote do aplicativo.\n2. Identificar informação sensível no pacote ou fragilidades no processo de assinatura.\n3. Demonstrar a capacidade de modificar e reempacotar o aplicativo sem detecção.",
        cve = "N/A"
    };

    public static Vuln InsecureMemoryManagement => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Gestão Insegura de Memória",
        Description =
            "O aplicativo não manipula adequadamente os dados sensíveis em memória, potencialmente deixando-os vulneráveis a dumps de memória ou ataques de canal lateral.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar práticas seguras de gestão de memória, como apagar dados sensíveis após seu uso e utilizar alocações de memória segura quando disponíveis.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Possível exposição de dados sensíveis através de dumps de memória ou ataques de canal lateral.",
        ProofOfConcept =
            "1. Usar ferramentas de depuração para analisar a memória do aplicativo durante a execução.\n2. Identificar instâncias de dados sensíveis (ex., chaves de criptografia, senhas) persistentes em memória.\n3. Demonstrar a capacidade de extrair estes dados sensíveis de um dump de memória.",
        cve = "N/A"
    };

    public static Vuln InsecureComponentUpgrade => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Mecanismo Inseguro de Atualização de Componentes",
        Description =
            "O mecanismo de atualização de componentes ou plugins do aplicativo é inseguro, potencialmente permitindo a instalação de atualizações maliciosas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar mecanismos seguros de atualização com verificações de integridade apropriadas, usar atualizações assinadas e verificar a fonte das atualizações.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possibilidade de instalação de atualizações maliciosas, levando a execução de código ou roubo de dados.",
        ProofOfConcept =
            "1. Interceptar o processo de atualização do aplicativo.\n2. Substituir uma atualização legítima por uma versão modificada.\n3. Demonstrar que o aplicativo instala e executa a atualização maliciosa sem verificação adequada.",
        cve = "N/A"
    };

    public static Vuln InsecureDataResidency => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Práticas Inseguras de Residência de Dados",
        Description =
            "O aplicativo não gerencia adequadamente a residência de dados, potencialmente violando requisitos legais ou regulatórios para o armazenamento e processamento de dados.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar controles adequados de residência de dados, garantir que os dados sejam armazenados e processados em conformidade com as regulamentações pertinentes, e fornecer controles ao usuário sobre as preferências de localização de dados.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possíveis violações legais e regulatórias, perda de confiança do usuário devido ao manuseio inadequado de dados.",
        ProofOfConcept =
            "1. Analisar os mecanismos de armazenamento e processamento de dados do aplicativo.\n2. Identificar casos onde os dados são armazenados ou processados em localizações não conformes.\n3. Demonstrar o não cumprimento do aplicativo com requisitos específicos de residência de dados ou preferências do usuário.",
        cve = "N/A"
    };

    public static Vuln InsecureCloudSyncMechanism => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Mecanismo Inseguro de Sincronização na Nuvem",
        Description =
            "O mecanismo de sincronização na nuvem do aplicativo não está corretamente protegido, potencialmente expondo dados sensíveis durante o processo de sincronização ou permitindo acesso não autorizado a dados sincronizados.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar criptografia ponta a ponta para dados sincronizados, usar autenticação segura para processos de sincronização e garantir controles de acesso apropriados em dados armazenados na nuvem.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível exposição de dados sensíveis do usuário durante processos de sincronização ou acesso não autorizado a dados armazenados na nuvem.",
        ProofOfConcept =
            "1. Interceptar o tráfego de sincronização na nuvem do aplicativo.\n2. Analisar os dados transmitidos em busca de informação sensível.\n3. Tentar acessar ou modificar dados sincronizados por meios não autorizados.",
        cve = "N/A"
    };

    public static Vuln VulnerableThirdPartyLibrary => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Biblioteca de Terceiros Vulnerável",
        Description =
            "O aplicativo utiliza uma biblioteca de terceiros com vulnerabilidades de segurança conhecidas, potencialmente expondo o aplicativo a vários ataques.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:H",
        Remediation =
            "Atualizar regularmente as bibliotecas de terceiros para suas últimas versões seguras, implementar um processo para rastrear e abordar vulnerabilidades em dependências.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possibilidade de vários ataques dependendo da vulnerabilidade específica, incluindo roubo de dados, execução de código ou negação de serviço.",
        ProofOfConcept =
            "1. Identificar bibliotecas de terceiros utilizadas no aplicativo.\n2. Verificar versões contra bases de dados de vulnerabilidades conhecidas.\n3. Demonstrar o impacto de uma vulnerabilidade específica em uma biblioteca desatualizada.",
        cve = "Depende da biblioteca vulnerável específica"
    };

    public static Vuln InsecureDataExfiltration => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Prevenção Insegura de Exfiltração de Dados",
        Description =
            "O aplicativo não previne adequadamente a exfiltração não autorizada de dados, permitindo que dados sensíveis sejam extraídos através de vários canais.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar técnicas adequadas de prevenção de perda de dados, restringir o acesso a dados sensíveis e monitorar/controlar canais de saída de dados.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível extração não autorizada de dados sensíveis do aplicativo, levando a vazamentos de dados ou violações de privacidade.",
        ProofOfConcept =
            "1. Identificar possíveis canais de exfiltração de dados (ex., capturas de tela, copiar/colar, compartilhamento de arquivos).\n2. Tentar extrair dados sensíveis através destes canais.\n3. Demonstrar exfiltração bem-sucedida de dados protegidos.",
        cve = "N/A"
    };

    public static Vuln InsecureAPIVersioning => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Versionamento Inseguro de API",
        Description =
            "O aplicativo não gerencia adequadamente o versionamento de API, potencialmente expondo-o a vulnerabilidades em endpoints de API obsoletos ou causando problemas de funcionalidade.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.3,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:L",
        Remediation =
            "Implementar estratégias apropriadas de versionamento de API, garantir que o aplicativo use a última versão de API e lidar graciosamente com incompatibilidades de versões.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possível exposição a vulnerabilidades em endpoints de API obsoletos, problemas de funcionalidade devido a incompatibilidades de versões.",
        ProofOfConcept =
            "1. Analisar as chamadas API do aplicativo e mecanismo de versionamento.\n2. Tentar forçar o aplicativo a usar uma versão obsoleta de API.\n3. Demonstrar vulnerabilidades ou problemas de funcionalidade com o uso de APIs obsoletas.",
        cve = "N/A"
    };

    public static Vuln InsecureQRCodeHandling => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Tratamento Inseguro de Códigos QR",
        Description =
            "O aplicativo não valida nem sanitiza adequadamente os dados de códigos QR escaneados, potencialmente permitindo ataques de injeção ou ações indesejadas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar validação rigorosa e sanitização de dados de códigos QR, usar listas brancas para formatos de dados aceitos e requerer confirmação do usuário para ações sensíveis ativadas por códigos QR.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possibilidade de execução arbitrária de código, acesso não autorizado a dados ou ativação de ações indesejadas dentro do aplicativo.",
        ProofOfConcept =
            "1. Gerar códigos QR maliciosos com cargas úteis injetadas.\n2. Escanear os códigos QR com o aplicativo.\n3. Demonstrar ações não autorizadas ou acesso a dados através de dados de código QR malformados.",
        cve = "N/A"
    };

    public static Vuln InsecureNFCImplementation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Insegura de NFC",
        Description =
            "A implementação NFC do aplicativo é insegura, potencialmente permitindo acesso não autorizado a dados ou ações maliciosas através de comunicação NFC.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:A/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar protocolos seguros de comunicação NFC, validar e sanitizar todos os dados NFC, e usar criptografia para a transmissão de dados sensíveis através de NFC.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível acesso não autorizado a dados sensíveis ou ativação de ações indesejadas através de interações NFC maliciosas.",
        ProofOfConcept =
            "1. Analisar a implementação NFC do aplicativo.\n2. Criar tags NFC maliciosas ou usar um emulador NFC para enviar dados manipulados.\n3. Demonstrar vazamento de dados ou ações não autorizadas através de comunicação NFC.",
        cve = "N/A"
    };

    public static Vuln InsecureARImplementation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Insegura de Realidade Aumentada (AR)",
        Description =
            "Os recursos de AR do aplicativo estão implementados de maneira insegura, potencialmente levando a violações de privacidade ou brechas de segurança através de interações AR.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar manipulação segura de dados AR, garantir permissões adequadas para acesso à câmera e sensores, e validar fontes de conteúdo AR.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possíveis violações de privacidade através de acesso não autorizado à câmera ou coleta de dados, brechas de segurança através de conteúdo AR malicioso.",
        ProofOfConcept =
            "1. Analisar a implementação AR e manipulação de dados do aplicativo.\n2. Tentar burlar permissões relacionadas a AR ou injetar conteúdo AR malicioso.\n3. Demonstrar violações de privacidade ou brechas de segurança através de recursos AR.",
        cve = "N/A"
    };

    public static Vuln InsecureIoTIntegration => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Integração Insegura de IoT",
        Description =
            "A integração do aplicativo com dispositivos IoT é insegura, potencialmente permitindo controle não autorizado de dispositivos conectados ou vazamento de dados.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar protocolos de comunicação seguros para a interação com dispositivos IoT, usar autenticação forte para o pareamento de dispositivos e validar todos os comandos enviados a dispositivos IoT.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível controle não autorizado de dispositivos IoT, vazamento de dados de dispositivos conectados ou violações de privacidade.",
        ProofOfConcept =
            "1. Analisar os protocolos de comunicação com dispositivos IoT do aplicativo.\n2. Tentar interceptar ou manipular comunicações entre o aplicativo e dispositivos IoT.\n3. Demonstrar controle não autorizado de dispositivos ou acesso a dados.",
        cve = "N/A"
    };

    public static Vuln InsecurePushNotifications => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Tratamento Inseguro de Notificações Push",
        Description =
            "O tratamento de notificações push do aplicativo é inseguro, potencialmente permitindo vazamento de dados sensíveis ou execução de cargas maliciosas através de notificações.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar tratamento seguro de cargas de notificações push, validar e sanitizar dados de notificações, e evitar enviar informações sensíveis em notificações.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Possível exposição de dados sensíveis através de notificações push ou execução de cargas maliciosas.",
        ProofOfConcept =
            "1. Analisar o mecanismo de tratamento de notificações push do aplicativo.\n2. Enviar notificações push manipuladas com cargas maliciosas ou dados sensíveis.\n3. Demonstrar vazamento de dados ou ações não autorizadas através de notificações push.",
        cve = "N/A"
    };

    public static Vuln InsecureAppCloning => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Vulnerabilidade à Clonagem de Aplicativos",
        Description =
            "O aplicativo é vulnerável a ataques de clonagem, potencialmente permitindo que cópias não autorizadas do aplicativo acessem dados ou funcionalidades sensíveis.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar medidas anti-clonagem como vinculação de dispositivos, usar armazenamento de chaves baseado em hardware e implementar verificações de integridade em tempo de execução.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível acesso não autorizado a dados ou funcionalidades sensíveis através de aplicativos clonados, evasão de licenças ou controles de acesso.",
        ProofOfConcept =
            "1. Tentar clonar o aplicativo usando várias ferramentas ou técnicas.\n2. Executar o aplicativo clonado em um dispositivo diferente.\n3. Demonstrar acesso a dados sensíveis ou funcionalidades através do aplicativo clonado.",
        cve = "N/A"
    };

    public static Vuln InsecureScreenOverlay => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Vulnerabilidade a Ataques de Sobreposição de Tela",
        Description =
            "O aplicativo é vulnerável a ataques de sobreposição de tela, potencialmente permitindo que aplicativos maliciosos capturem entrada sensível do usuário.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar mecanismos de detecção de sobreposição, usar métodos seguros de entrada para dados sensíveis e educar os usuários sobre os riscos de conceder permissões de sobreposição a aplicativos desconhecidos.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Possível captura de entrada sensível do usuário como senhas ou informações de cartões de crédito.",
        ProofOfConcept =
            "1. Criar um aplicativo de prova de conceito de sobreposição.\n2. Tentar capturar entrada de usuário no aplicativo alvo usando a sobreposição.\n3. Demonstrar a captura bem-sucedida de informações sensíveis.",
        cve = "N/A"
    };

    public static Vuln InsecureAppWidget => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Insegura de Widget do Aplicativo",
        Description =
            "A implementação do widget do aplicativo é insegura, potencialmente expondo dados ou funcionalidades sensíveis na tela inicial do dispositivo.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar manipulação segura de dados em widgets do aplicativo, evitar mostrar informações sensíveis e usar autenticação apropriada para ações do widget.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possível exposição de dados sensíveis na tela inicial do dispositivo ou acesso não autorizado a funcionalidades do aplicativo através do widget.",
        ProofOfConcept =
            "1. Analisar a implementação do widget do aplicativo.\n2. Tentar acessar dados sensíveis ou funcionalidades através do widget sem autenticação apropriada.\n3. Demonstrar vazamento de dados ou ações não autorizadas através do widget.",
        cve = "N/A"
    };

    public static Vuln InsecureEdgeComputingIntegration => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Integração Insegura de Computação de Borda",
        Description =
            "A integração do aplicativo com serviços de computação de borda é insegura, potencialmente expondo dados sensíveis ou permitindo acesso não autorizado a informações processadas na borda.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar protocolos seguros de comunicação para interações de computação de borda, usar autenticação e criptografia forte, e validar todos os dados recebidos de serviços de borda.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível acesso não autorizado a dados sensíveis processados na borda, manipulação de dados ou violações de privacidade.",
        ProofOfConcept =
            "1. Analisar a comunicação do aplicativo com serviços de computação de borda.\n2. Tentar interceptar ou manipular dados entre o aplicativo e serviços de borda.\n3. Demonstrar acesso não autorizado a informações sensíveis ou injeção de dados maliciosos.",
        cve = "N/A"
    };

    public static Vuln InsecureAIMLImplementation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Insegura de Modelos IA/ML",
        Description =
            "A implementação de modelos de IA/ML do aplicativo é insegura, potencialmente permitindo envenenamento do modelo, extração de dados ou ataques adversários.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar implantação segura de modelos IA/ML, usar aprendizado federado quando apropriado, proteger contra ataques de inversão de modelo e inferência de associação, e atualizar regularmente os modelos.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível extração de dados de treinamento sensíveis, manipulação de saídas do modelo IA/ML ou inferência não autorizada sobre usuários.",
        ProofOfConcept =
            "1. Analisar a implementação do modelo IA/ML do aplicativo.\n2. Tentar ataques de inversão de modelo ou inferência de associação.\n3. Demonstrar extração de informação sensível ou exemplos adversários bem-sucedidos.",
        cve = "N/A"
    };

    public static Vuln InsecureQuantumResistantCrypto => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Falta de Criptografia Resistente a Quantum",
        Description =
            "O aplicativo não implementa algoritmos criptográficos resistentes a quantum, potencialmente tornando-o vulnerável a futuros ataques de computação quântica.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.9,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar algoritmos criptográficos pós-quânticos para proteção de dados sensíveis e mecanismos de troca de chaves.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Potencial vulnerabilidade futura a ataques de computação quântica, possivelmente levando à descriptografia de dados sensíveis.",
        ProofOfConcept =
            "1. Analisar as implementações criptográficas do aplicativo.\n2. Identificar uso de algoritmos não resistentes a quantum para operações sensíveis críticas.\n3. Demonstrar vulnerabilidade teórica a algoritmos quânticos conhecidos.",
        cve = "N/A"
    };

    public static Vuln InsecureVoiceUIIntegration => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Integração Insegura de Interface de Usuário por Voz",
        Description =
            "A integração do aplicativo com interfaces de usuário por voz (VUI) é insegura, potencialmente permitindo comandos de voz não autorizados ou expondo informações sensíveis através de interações por voz.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar autenticação de voz segura, validar e sanitizar comandos de voz, e evitar expor informações sensíveis através de respostas de voz.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível acesso não autorizado a funcionalidades do aplicativo, exposição de informações sensíveis através de canais de voz, ou execução de comandos de voz maliciosos.",
        ProofOfConcept =
            "1. Analisar a implementação da interface de voz do aplicativo.\n2. Tentar burlar a autenticação por voz ou injetar comandos de voz maliciosos.\n3. Demonstrar ações não autorizadas ou acesso a dados através de interações por voz.",
        cve = "N/A"
    };

    public static Vuln InsecureMultiDeviceSynchronization => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Sincronização Insegura Multi-Dispositivo",
        Description =
            "O mecanismo de sincronização multi-dispositivo do aplicativo é inseguro, potencialmente permitindo acesso não autorizado a dados de usuário entre dispositivos ou interceptação de dados de sincronização.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar criptografia ponta a ponta para sincronização multi-dispositivo, usar mecanismos seguros de pareamento de dispositivos e validar a integridade dos dados sincronizados.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível acesso não autorizado a dados de usuário em múltiplos dispositivos, interceptação de informações sensíveis durante a sincronização ou injeção de dados maliciosos entre dispositivos.",
        ProofOfConcept =
            "1. Analisar o mecanismo de sincronização multi-dispositivo do aplicativo.\n2. Tentar interceptar ou manipular dados de sincronização entre dispositivos.\n3. Demonstrar acesso não autorizado a dados sincronizados ou injeção de conteúdo malicioso.",
        cve = "N/A"
    };

    public static Vuln InsecureBlockchainIntegration => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Integração Insegura de Blockchain",
        Description =
            "A integração do aplicativo com tecnologia blockchain é insegura, potencialmente expondo chaves criptográficas, permitindo transações não autorizadas ou comprometendo a integridade dos dados blockchain.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation =
            "Implementar gestão segura de chaves para interações blockchain, usar módulos de segurança hardware quando possível, validar todas as transações blockchain e garantir controles de acesso apropriados a funcionalidades blockchain.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível roubo de ativos de criptomoedas, transações blockchain não autorizadas ou comprometimento da integridade de aplicações descentralizadas (DApp).",
        ProofOfConcept =
            "1. Analisar a integração blockchain do aplicativo, focando na gestão de chaves e assinatura de transações.\n2. Tentar extrair chaves privadas ou manipular dados de transação.\n3. Demonstrar transações blockchain não autorizadas ou comprometimento de funcionalidade DApp.",
        cve = "N/A"
    };

    public static Vuln InsecureKeychainKeystore => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Uso Inseguro de Keychain/Keystore",
        Description =
            "O uso do keychain (iOS) ou keystore (Android) da plataforma é inseguro, potencialmente expondo credenciais sensíveis ou chaves criptográficas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar acesso seguro ao keychain/keystore, usar classes de proteção ou níveis de segurança apropriados, e evitar armazenar dados altamente sensíveis no keychain/keystore se possível.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível exposição de credenciais sensíveis ou chaves criptográficas, levando a acesso não autorizado ou descriptografia de dados.",
        ProofOfConcept =
            "1. Analisar o uso do keychain/keystore do aplicativo.\n2. Tentar extrair dados do keychain/keystore usando um dispositivo com jailbreak/root.\n3. Demonstrar acesso a informações sensíveis armazenadas no keychain/keystore.",
        cve = "N/A"
    };

    public static Vuln InsecureRandomNumberGeneration => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Geração Insegura de Números Aleatórios",
        Description =
            "O aplicativo utiliza métodos fracos ou previsíveis de geração de números aleatórios, potencialmente comprometendo operações criptográficas ou funcionalidades críticas de segurança.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Usar geradores de números aleatórios criptograficamente seguros fornecidos pela plataforma ou bibliotecas bem verificadas. Evitar usar Math.random() ou PRNGs fracos similares para operações críticas de segurança.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível previsão de valores gerados, levando a criptografia comprometida, tokens de sessão ou outros dados críticos de segurança.",
        ProofOfConcept =
            "1. Identificar uso de geração de números aleatórios no código do aplicativo.\n2. Analisar a qualidade de aleatoriedade dos valores gerados.\n3. Demonstrar previsibilidade ou viés nos números aleatórios gerados.",
        cve = "N/A"
    };

    public static Vuln InsecureSSOImplementation => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Implementação Insegura de Login Único (SSO)",
        Description =
            "A implementação de Login Único (SSO) ou federação do aplicativo é defeituosa, potencialmente permitindo acesso não autorizado ou sequestro de contas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar práticas seguras de SSO, validar corretamente os tokens OAuth, usar URIs de redirecionamento seguros e implementar gestão adequada de sessões após a autenticação SSO.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível acesso não autorizado a contas de usuário, sequestro de sessão ou evasão de mecanismos de autenticação.",
        ProofOfConcept =
            "1. Analisar a implementação de SSO e manipulação de tokens do aplicativo.\n2. Tentar falsificar ou manipular tokens SSO.\n3. Demonstrar acesso não autorizado usando credenciais SSO manipuladas.",
        cve = "N/A"
    };

    public static Vuln InsecureVPNUsage => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Uso Inseguro de VPN",
        Description =
            "O uso ou implementação de funcionalidade VPN do aplicativo é inseguro, potencialmente expondo o tráfego do usuário ou permitindo acesso não autorizado a redes protegidas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar protocolos VPN seguros, validar corretamente certificados de servidor, usar criptografia forte para túneis VPN e garantir o tratamento adequado de credenciais VPN.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact =
            "Possível exposição do tráfego do usuário, acesso não autorizado a redes protegidas ou comprometimento de credenciais VPN.",
        ProofOfConcept =
            "1. Analisar a implementação e configuração VPN do aplicativo.\n2. Tentar interceptar ou manipular tráfego VPN.\n3. Demonstrar vazamento de dados ou acesso não autorizado através de vulnerabilidades VPN.",
        cve = "N/A"
    };

    public static Vuln InsecureCustomURLScheme => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Tratamento Inseguro de Esquemas URL Personalizados",
        Description =
            "A implementação de esquemas URL personalizados do aplicativo é insegura, potencialmente permitindo que outros aplicativos invoquem funcionalidade sensível ou acessem dados protegidos.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation =
            "Implementar validação e sanitização adequada de parâmetros de esquemas URL personalizados, usar esquemas específicos do aplicativo e requerer confirmação do usuário para ações sensíveis ativadas por esquemas URL.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possível acesso não autorizado a funcionalidade ou dados do aplicativo através de URLs personalizadas maliciosamente criadas.",
        ProofOfConcept =
            "1. Identificar esquemas URL personalizados utilizados pelo aplicativo.\n2. Criar URLs maliciosas que explorem estes esquemas.\n3. Demonstrar ações não autorizadas ou acesso a dados através da exploração de esquemas URL personalizados.",
        cve = "N/A"
    };

    public static Vuln TimeOfCheckToTimeOfUse => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Vulnerabilidade de Tempo entre Verificação e Uso (TOCTOU)",
        Description =
            "O aplicativo contém condições de corrida entre a verificação de uma condição e o uso dos resultados dessa verificação, potencialmente levando a problemas de segurança.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar mecanismos adequados de sincronização, usar operações atômicas quando possível e projetar código para minimizar a janela de tempo entre verificações e uso.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possível escalação de privilégios, acesso não autorizado a recursos ou corrupção de dados devido a condições de corrida.",
        ProofOfConcept =
            "1. Identificar potenciais vulnerabilidades TOCTOU no código do aplicativo.\n2. Criar um exploit de prova de conceito que compita entre a verificação e o uso.\n3. Demonstrar ações não autorizadas ou acesso a dados através da exploração bem-sucedida da condição de corrida.",
        cve = "N/A"
    };

    public static Vuln InsecureAntiDebugging => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Medidas Anti-Depuração Insuficientes",
        Description =
            "O aplicativo carece de técnicas robustas anti-depuração, facilitando aos atacantes analisar e modificar o comportamento do aplicativo em tempo de execução.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:H/UI:N/S:U/C:H/I:H/A:N",
        Remediation =
            "Implementar múltiplas camadas de técnicas anti-depuração, incluindo verificações de código nativo, verificações de tempo e detecção de ambiente. Atualizar e ofuscar regularmente estas proteções.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Facilita a engenharia reversa e manipulação do aplicativo, potencialmente expondo lógica sensível ou burlando controles de segurança.",
        ProofOfConcept =
            "1. Tentar anexar um depurador ao aplicativo em execução.\n2. Analisar as medidas anti-depuração do aplicativo.\n3. Demonstrar a evasão bem-sucedida das técnicas anti-depuração existentes.",
        cve = "N/A"
    };

    public static Vuln OverPrivilegedApplication => new Vuln
    {
        Template = true,
        Language = Language.Português,
        Name = "Aplicativo com Privilégios Excessivos",
        Description =
            "O aplicativo solicita mais permissões do que o necessário para sua funcionalidade, potencialmente expondo os usuários a maiores riscos de privacidade e segurança.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.9,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation =
            "Revisar e minimizar as solicitações de permissões do aplicativo, implementar solicitações de permissões em tempo de execução e explicar claramente aos usuários por que cada permissão é necessária.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact =
            "Possíveis violações de privacidade, acesso não autorizado a dados do usuário ou aumento da superfície de ataque devido a permissões desnecessárias.",
        ProofOfConcept =
            "1. Analisar o manifesto do aplicativo ou info.plist para as permissões solicitadas.\n2. Identificar permissões que não são essenciais para a funcionalidade principal.\n3. Demonstrar o possível uso indevido de permissões excessivas.",
        cve = "N/A"
    };
}