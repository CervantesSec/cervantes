using Cervantes.CORE.Entities;

namespace Cervantes.IFR.VulnTemplates;

public class VulnTemplatesMastgES
{
    public static Vuln InsecureDataStorage => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Almacenamiento Inseguro de Datos",
        Description = "La aplicación no almacena los datos sensibles de forma segura, lo que potencialmente los expone a accesos no autorizados.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar mecanismos de almacenamiento seguro como el cifrado para los datos sensibles almacenados localmente.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Acceso no autorizado a datos sensibles del usuario, posibles violaciones de privacidad y problemas de cumplimiento normativo.",
        ProofOfConcept = "1. Obtener acceso root/jailbreak al dispositivo.\n2. Navegar al directorio de datos de la aplicación.\n3. Localizar y acceder a archivos de datos sensibles no cifrados.",
        cve = "N/A"
    };

    public static Vuln InsecureCryptography => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Criptográfica Insegura",
        Description = "La aplicación utiliza algoritmos criptográficos débiles u obsoletos, o implementa algoritmos fuertes de manera incorrecta.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar algoritmos criptográficos fuertes y actualizados y seguir las mejores prácticas de la industria para su uso.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible descifrado de datos sensibles, compromiso de la integridad de las comunicaciones cifradas.",
        ProofOfConcept = "1. Decompilar la aplicación y analizar el código.\n2. Identificar el uso de algoritmos débiles (ej., MD5, SHA1) o tamaños de clave pequeños.\n3. Demostrar la factibilidad de romper el cifrado en un tiempo razonable.",
        cve = "N/A"
    };

    public static Vuln InsecureAuthentication => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Mecanismo de Autenticación Inseguro",
        Description = "El mecanismo de autenticación de la aplicación es débil o está implementado incorrectamente, permitiendo accesos no autorizados.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar mecanismos de autenticación seguros, como autenticación multifactor, y seguir las mejores prácticas de la plataforma para la autenticación local.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Acceso no autorizado a cuentas de usuario, posible filtración de datos y compromiso de la privacidad del usuario.",
        ProofOfConcept = "1. Analizar el flujo de autenticación de la aplicación.\n2. Identificar y explotar debilidades (ej., falta de límite de intentos, políticas de contraseñas débiles).\n3. Demostrar acceso no autorizado a una cuenta de usuario.",
        cve = "N/A"
    };

    public static Vuln InsecureNetworkCommunication => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Comunicación de Red Insegura",
        Description = "La aplicación transmite datos sensibles a través de canales inseguros o no valida correctamente los certificados del servidor.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar protocolos de comunicación de red seguros (ej., TLS) y asegurar la validación adecuada de certificados.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Interceptación de datos sensibles en tránsito, posibles ataques de intermediario (man-in-the-middle).",
        ProofOfConcept = "1. Configurar un proxy para interceptar el tráfico de la aplicación.\n2. Identificar transmisiones de datos no cifrados o validación incorrecta de certificados.\n3. Demostrar la capacidad de ver o modificar datos sensibles en tránsito.",
        cve = "N/A"
    };
    
    public static Vuln PrivacyViolation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Violación de Privacidad",
        Description = "La aplicación recopila, procesa o comparte datos de usuario sin el consentimiento adecuado o más allá de lo necesario para su funcionalidad.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar prácticas de minimización de datos, proporcionar políticas de privacidad claras y ofrecer control al usuario sobre sus datos.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Violación de la privacidad del usuario, posibles consecuencias legales y regulatorias, pérdida de confianza del usuario.",
        ProofOfConcept = "1. Analizar las prácticas de recopilación y compartición de datos de la aplicación.\n2. Identificar casos de recopilación o compartición excesiva de datos.\n3. Comparar el manejo real de datos con las políticas de privacidad establecidas y el consentimiento del usuario.",
        cve = "N/A"
    };

    public static Vuln InsecureDataLeakage => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Fuga de Datos Insegura",
        Description = "La aplicación filtra involuntariamente datos sensibles a través de varios canales como registros, copias de seguridad o portapapeles.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar prácticas adecuadas de manejo de datos, evitar el registro de información sensible y asegurar las copias de seguridad de la aplicación.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Exposición de datos sensibles del usuario, posibles violaciones de privacidad y brechas de seguridad.",
        ProofOfConcept = "1. Habilitar el registro detallado en el dispositivo.\n2. Realizar varias acciones en la aplicación.\n3. Examinar los registros en busca de datos sensibles como contraseñas o tokens de sesión.",
        cve = "N/A"
    };

    public static Vuln InsecureKeyManagement => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Gestión Insegura de Claves Criptográficas",
        Description = "La aplicación no gestiona adecuadamente las claves criptográficas, potencialmente exponiéndolas a accesos no autorizados.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar prácticas seguras de gestión de claves, utilizar almacenamiento de claves respaldado por hardware cuando esté disponible y evitar la codificación directa de claves.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Compromiso de datos cifrados, posibilidad de descifrado no autorizado de información sensible.",
        ProofOfConcept = "1. Decompilar la aplicación y analizar el código.\n2. Buscar claves criptográficas codificadas o métodos inseguros de almacenamiento de claves.\n3. Demostrar la capacidad de extraer o usar las claves sin autorización.",
        cve = "N/A"
    };

    public static Vuln InsecureLocalAuthentication => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Autenticación Local Insegura",
        Description = "El mecanismo de autenticación local de la aplicación (ej., biometría, PIN) está implementado incorrectamente o puede ser fácilmente evadido.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:P/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar mecanismos seguros de autenticación local siguiendo las mejores prácticas de la plataforma, usar técnicas criptográficas fuertes para almacenar secretos de autenticación.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Acceso local no autorizado a funcionalidades o datos sensibles de la aplicación.",
        ProofOfConcept = "1. Analizar la implementación de autenticación local.\n2. Intentar evadir la autenticación (ej., manipulando el almacenamiento local de datos).\n3. Demostrar acceso no autorizado a características protegidas de la aplicación.",
        cve = "N/A"
    };

    public static Vuln InsecureCertificatePinning => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Falta de Fijación de Certificados",
        Description = "La aplicación no implementa la fijación de certificados, haciéndola vulnerable a ataques de intermediario.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar la fijación de certificados para todos los puntos finales remotos bajo el control del desarrollador.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posibilidad de ataques de intermediario, interceptación de datos sensibles en tránsito.",
        ProofOfConcept = "1. Configurar un proxy con un certificado autofirmado.\n2. Intentar interceptar el tráfico HTTPS de la aplicación.\n3. Si tiene éxito, demostrar la capacidad de ver o modificar los datos interceptados.",
        cve = "N/A"
    };
    
    public static Vuln InsecureIPC => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Comunicación Entre Procesos Insegura",
        Description = "La aplicación utiliza mecanismos IPC inseguros, potencialmente exponiendo datos sensibles o funcionalidad a otras aplicaciones.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar mecanismos seguros de IPC, usar controles de acceso apropiados y validar todas las entradas IPC.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición de datos sensibles o funcionalidad a aplicaciones no autorizadas.",
        ProofOfConcept = "1. Analizar los mecanismos IPC de la aplicación (ej., intents, proveedores de contenido).\n2. Crear una aplicación de prueba de concepto que intente acceder a componentes expuestos.\n3. Demostrar acceso no autorizado a datos sensibles o funcionalidad.",
        cve = "N/A"
    };

    public static Vuln InsecureWebView => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Insegura de WebView",
        Description = "La implementación de WebView de la aplicación es insegura, potencialmente permitiendo la ejecución de scripts maliciosos o acceso a datos sensibles.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:H",
        Remediation = "Configuración segura de WebView, deshabilitar JavaScript si no es necesario, validar todo el contenido cargado y usar HTTPS para contenido remoto.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible ejecución de scripts maliciosos, acceso no autorizado a datos o funcionalidad de la aplicación.",
        ProofOfConcept = "1. Analizar la configuración de WebView en la aplicación.\n2. Si JavaScript está habilitado, intentar inyectar y ejecutar scripts maliciosos.\n3. Demostrar acceso no autorizado a datos sensibles o funcionalidad de la aplicación a través del WebView.",
        cve = "N/A"
    };

    public static Vuln InsecureDeepLinking => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Insegura de Enlaces Profundos",
        Description = "Los enlaces profundos o el manejo de esquemas URL de la aplicación son inseguros, potencialmente permitiendo acceso no autorizado a la funcionalidad de la aplicación.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar validación y sanitización adecuada de parámetros de enlaces profundos, usar esquemas específicos de la aplicación y requerir confirmación del usuario para acciones sensibles.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potencial acceso no autorizado a la funcionalidad de la aplicación, fuga de datos o ejecución de acciones no intencionadas.",
        ProofOfConcept = "1. Analizar la implementación de enlaces profundos de la aplicación.\n2. Crear enlaces profundos maliciosos dirigidos a funcionalidad sensible.\n3. Demostrar la capacidad de acceder a características restringidas o filtrar datos sensibles a través de enlaces profundos.",
        cve = "N/A"
    };
    
    public static Vuln InsecureSessionHandling => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Manejo Inseguro de Sesiones",
        Description = "La aplicación no gestiona adecuadamente las sesiones de usuario, potencialmente permitiendo el secuestro de sesiones o acceso no autorizado.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.0,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implementar gestión segura de sesiones, usar identificadores de sesión fuertes, implementar mecanismos adecuados de expiración e invalidación de sesiones.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Acceso no autorizado a cuentas de usuario, posibilidad de ataques de secuestro de sesión.",
        ProofOfConcept = "1. Analizar el mecanismo de gestión de sesiones de la aplicación.\n2. Intentar reutilizar tokens de sesión antiguos o manipular datos de sesión.\n3. Demostrar acceso no autorizado a la sesión de un usuario.",
        cve = "N/A"
    };

    public static Vuln InsecureTlsValidation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Validación TLS Insuficiente",
        Description = "La aplicación no valida adecuadamente los certificados TLS, haciéndola vulnerable a ataques de intermediario.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar validación adecuada de certificados TLS, incluyendo verificación de caducidad, revocación y nombre de host correcto.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posibilidad de ataques de intermediario, interceptación de datos sensibles en tránsito.",
        ProofOfConcept = "1. Configurar un proxy con un certificado TLS inválido.\n2. Intentar interceptar el tráfico HTTPS de la aplicación.\n3. Si tiene éxito, demostrar la capacidad de ver o modificar los datos interceptados.",
        cve = "N/A"
    };
    
    public static Vuln InsecureClipboardUsage => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Uso Inseguro del Portapapeles",
        Description = "La aplicación permite que datos sensibles sean copiados al portapapeles, potencialmente exponiéndolos a otras aplicaciones.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 4.3,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:N",
        Remediation = "Prevenir la copia de datos sensibles al portapapeles o implementar mecanismos seguros de manejo del portapapeles.",
        RemediationComplexity = RemediationComplexity.Low,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición de datos sensibles a aplicaciones o usuarios no autorizados.",
        ProofOfConcept = "1. Identificar campos que contengan datos sensibles en la aplicación.\n2. Intentar copiar estos datos al portapapeles.\n3. Demostrar la capacidad de pegar los datos sensibles en otra aplicación.",
        cve = "N/A"
    };

    public static Vuln InsecureDataCaching => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Almacenamiento en Caché Inseguro",
        Description = "La aplicación almacena datos sensibles en caché de manera insegura, potencialmente exponiéndolos a accesos no autorizados.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar mecanismos seguros de caché, evitar almacenar datos sensibles en caché y limpiar las cachés adecuadamente cuando la aplicación se cierra o el usuario cierra sesión.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición de datos sensibles en caché a usuarios o aplicaciones no autorizadas.",
        ProofOfConcept = "1. Usar la aplicación y realizar acciones que involucren datos sensibles.\n2. Analizar los directorios y archivos de caché de la aplicación.\n3. Demostrar la presencia de datos sensibles en archivos de caché no cifrados o fácilmente accesibles.",
        cve = "N/A"
    };

    public static Vuln InsecureBackupHandling => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Manejo Inseguro de Copias de Seguridad",
        Description = "La aplicación no asegura adecuadamente sus datos durante el proceso de copia de seguridad, potencialmente exponiendo información sensible.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar mecanismos seguros de copia de seguridad, excluir datos sensibles de las copias de seguridad o cifrar los datos de respaldo.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición de datos sensibles a través de copias de seguridad inseguras.",
        ProofOfConcept = "1. Habilitar la copia de seguridad de datos de la aplicación.\n2. Realizar una copia de seguridad del dispositivo.\n3. Analizar el contenido de la copia de seguridad y demostrar la presencia de datos sensibles no cifrados.",
        cve = "N/A"
    };

    public static Vuln InsufficientInputValidation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Validación de Entrada Insuficiente",
        Description = "La aplicación no valida adecuadamente la entrada del usuario, potencialmente conduciendo a ataques de inyección o comportamiento inesperado.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.6,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Implementar validación exhaustiva de entrada para todos los datos proporcionados por el usuario, usar consultas parametrizadas y sanitizar la entrada antes de su uso.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posibilidad de ataques de inyección, corrupción de datos o acceso no autorizado a sistemas backend.",
        ProofOfConcept = "1. Identificar campos de entrada en la aplicación.\n2. Probar varias entradas malformadas o maliciosas.\n3. Demostrar comportamiento inesperado o ataque de inyección exitoso.",
        cve = "N/A"
    };

    public static Vuln InsecureJailbreakRootDetection => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Detección Insuficiente de Jailbreak/Root",
        Description = "La aplicación no detecta o responde adecuadamente a dispositivos con jailbreak (iOS) o root (Android), potencialmente exponiendo funcionalidad o datos sensibles.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar mecanismos robustos de detección de jailbreak/root y responder apropiadamente cuando se detecta un entorno comprometido.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición de funcionalidad o datos sensibles de la aplicación en dispositivos comprometidos.",
        ProofOfConcept = "1. Usar un dispositivo o emulador con jailbreak/root.\n2. Ejecutar la aplicación e intentar acceder a características sensibles.\n3. Demostrar que la aplicación no detecta el entorno comprometido y permite funcionalidad completa.",
        cve = "N/A"
    };
    
    public static Vuln InsecureCodeObfuscation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Ofuscación de Código Insuficiente",
        Description = "El código de la aplicación no está adecuadamente ofuscado, facilitando a los atacantes la ingeniería inversa y la comprensión de la lógica de la aplicación.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.3,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:L",
        Remediation = "Implementar técnicas sólidas de ofuscación de código para dificultar la ingeniería inversa.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Facilita la ingeniería inversa de la lógica de la aplicación, posible exposición de algoritmos propietarios o mecanismos de seguridad.",
        ProofOfConcept = "1. Decompilar la aplicación usando herramientas apropiadas.\n2. Analizar el código decompilado para evaluar su legibilidad y comprensibilidad.\n3. Demostrar la facilidad de entender la lógica crítica de la aplicación o mecanismos de seguridad.",
        cve = "N/A"
    };

    public static Vuln InsecureRuntimeIntegrityChecks => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Falta de Verificaciones de Integridad en Tiempo de Ejecución",
        Description = "La aplicación no realiza verificaciones de integridad en tiempo de ejecución, haciéndola vulnerable a la inyección de código o manipulación durante la ejecución.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar verificaciones de integridad en tiempo de ejecución para detectar y responder a modificaciones de código o inyecciones durante la ejecución de la aplicación.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posibilidad de inyección de código en tiempo de ejecución, elusión de controles de seguridad o manipulación del comportamiento de la aplicación.",
        ProofOfConcept = "1. Usar una herramienta como Frida para inyectar código en la aplicación en ejecución.\n2. Modificar funcionalidad crítica de la aplicación o eludir verificaciones de seguridad.\n3. Demostrar que la aplicación no detecta ni responde a las modificaciones en tiempo de ejecución.",
        cve = "N/A"
    };

    public static Vuln InsecureAppPackaging => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Empaquetado Inseguro de la Aplicación",
        Description = "El paquete de la aplicación contiene información sensible o no está debidamente firmado, haciéndolo vulnerable a la manipulación o divulgación de información.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Asegurar la firma adecuada de la aplicación, eliminar información sensible del paquete e implementar verificaciones adicionales de integridad.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posibilidad de ataques de reempaquetado de la aplicación, exposición de información sensible en el paquete.",
        ProofOfConcept = "1. Extraer y analizar el paquete de la aplicación.\n2. Identificar información sensible en el paquete o debilidades en el proceso de firma.\n3. Demostrar la capacidad de modificar y reempaquetar la aplicación sin detección.",
        cve = "N/A"
    };

    public static Vuln InsecureMemoryManagement => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Gestión Insegura de Memoria",
        Description = "La aplicación no maneja adecuadamente los datos sensibles en memoria, potencialmente dejándolos vulnerables a volcados de memoria o ataques de canal lateral.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar prácticas seguras de gestión de memoria, como borrar datos sensibles después de su uso y utilizar asignaciones de memoria segura cuando estén disponibles.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible exposición de datos sensibles a través de volcados de memoria o ataques de canal lateral.",
        ProofOfConcept = "1. Usar herramientas de depuración para analizar la memoria de la aplicación durante la ejecución.\n2. Identificar instancias de datos sensibles (ej., claves de cifrado, contraseñas) persistentes en memoria.\n3. Demostrar la capacidad de extraer estos datos sensibles de un volcado de memoria.",
        cve = "N/A"
    };

    public static Vuln InsecureComponentUpgrade => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Mecanismo Inseguro de Actualización de Componentes",
        Description = "El mecanismo de actualización de componentes o plugins de la aplicación es inseguro, potencialmente permitiendo la instalación de actualizaciones maliciosas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implementar mecanismos seguros de actualización con verificaciones de integridad apropiadas, usar actualizaciones firmadas y verificar la fuente de las actualizaciones.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posibilidad de instalación de actualizaciones maliciosas, conduciendo a ejecución de código o robo de datos.",
        ProofOfConcept = "1. Interceptar el proceso de actualización de la aplicación.\n2. Reemplazar una actualización legítima con una versión modificada.\n3. Demostrar que la aplicación instala y ejecuta la actualización maliciosa sin verificación adecuada.",
        cve = "N/A"
    };
    
    public static Vuln InsecureDataResidency => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Prácticas Inseguras de Residencia de Datos",
        Description = "La aplicación no gestiona adecuadamente la residencia de datos, potencialmente violando requisitos legales o regulatorios para el almacenamiento y procesamiento de datos.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar controles adecuados de residencia de datos, asegurar que los datos se almacenen y procesen en cumplimiento con las regulaciones pertinentes, y proporcionar controles al usuario sobre las preferencias de ubicación de datos.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posibles violaciones legales y regulatorias, pérdida de confianza del usuario debido al manejo inadecuado de datos.",
        ProofOfConcept = "1. Analizar los mecanismos de almacenamiento y procesamiento de datos de la aplicación.\n2. Identificar casos donde los datos se almacenan o procesan en ubicaciones no conformes.\n3. Demostrar el incumplimiento de la aplicación con requisitos específicos de residencia de datos o preferencias del usuario.",
        cve = "N/A"
    };
    
    public static Vuln InsecureCloudSyncMechanism => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Mecanismo Inseguro de Sincronización en la Nube",
        Description = "El mecanismo de sincronización en la nube de la aplicación no está correctamente asegurado, potencialmente exponiendo datos sensibles durante el proceso de sincronización o permitiendo acceso no autorizado a datos sincronizados.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar cifrado de extremo a extremo para datos sincronizados, usar autenticación segura para procesos de sincronización y asegurar controles de acceso apropiados en datos almacenados en la nube.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible exposición de datos sensibles del usuario durante procesos de sincronización o acceso no autorizado a datos almacenados en la nube.",
        ProofOfConcept = "1. Interceptar el tráfico de sincronización en la nube de la aplicación.\n2. Analizar los datos transmitidos en busca de información sensible.\n3. Intentar acceder o modificar datos sincronizados por medios no autorizados.",
        cve = "N/A"
    };

    public static Vuln VulnerableThirdPartyLibrary => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Biblioteca de Terceros Vulnerable",
        Description = "La aplicación utiliza una biblioteca de terceros con vulnerabilidades de seguridad conocidas, potencialmente exponiendo la aplicación a varios ataques.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:H",
        Remediation = "Actualizar regularmente las bibliotecas de terceros a sus últimas versiones seguras, implementar un proceso para rastrear y abordar vulnerabilidades en dependencias.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posibilidad de varios ataques dependiendo de la vulnerabilidad específica, incluyendo robo de datos, ejecución de código o denegación de servicio.",
        ProofOfConcept = "1. Identificar bibliotecas de terceros utilizadas en la aplicación.\n2. Verificar versiones contra bases de datos de vulnerabilidades conocidas.\n3. Demostrar el impacto de una vulnerabilidad específica en una biblioteca desactualizada.",
        cve = "Depende de la biblioteca vulnerable específica"
    };
    
    public static Vuln InsecureDataExfiltration => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Prevención Insegura de Exfiltración de Datos",
        Description = "La aplicación no previene adecuadamente la exfiltración no autorizada de datos, permitiendo que datos sensibles sean extraídos a través de varios canales.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar técnicas adecuadas de prevención de pérdida de datos, restringir el acceso a datos sensibles y monitorear/controlar canales de salida de datos.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible extracción no autorizada de datos sensibles de la aplicación, conduciendo a brechas de datos o violaciones de privacidad.",
        ProofOfConcept = "1. Identificar posibles canales de exfiltración de datos (ej., capturas de pantalla, copiar/pegar, compartir archivos).\n2. Intentar extraer datos sensibles a través de estos canales.\n3. Demostrar exfiltración exitosa de datos protegidos.",
        cve = "N/A"
    };

    public static Vuln InsecureAPIVersioning => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Versionado Inseguro de API",
        Description = "La aplicación no maneja adecuadamente el versionado de API, potencialmente exponiéndola a vulnerabilidades en puntos finales de API obsoletos o causando problemas de funcionalidad.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.3,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:L",
        Remediation = "Implementar estrategias apropiadas de versionado de API, asegurar que la aplicación use la última versión de API y manejar graciosamente las incompatibilidades de versiones.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición a vulnerabilidades en puntos finales de API obsoletos, problemas de funcionalidad debido a desajustes de versiones.",
        ProofOfConcept = "1. Analizar las llamadas API de la aplicación y mecanismo de versionado.\n2. Intentar forzar a la aplicación a usar una versión obsoleta de API.\n3. Demostrar vulnerabilidades o problemas de funcionalidad con el uso de API obsoletas.",
        cve = "N/A"
    };
    
    public static Vuln InsecureQRCodeHandling => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Manejo Inseguro de Códigos QR",
        Description = "La aplicación no valida ni sanitiza adecuadamente los datos de códigos QR escaneados, potencialmente permitiendo ataques de inyección o acciones no deseadas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implementar validación estricta y sanitización de datos de códigos QR, usar listas blancas para formatos de datos aceptados y requerir confirmación del usuario para acciones sensibles activadas por códigos QR.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posibilidad de ejecución arbitraria de código, acceso no autorizado a datos o activación de acciones no deseadas dentro de la aplicación.",
        ProofOfConcept = "1. Generar códigos QR maliciosos con cargas útiles inyectadas.\n2. Escanear los códigos QR con la aplicación.\n3. Demostrar acciones no autorizadas o acceso a datos a través de datos de código QR malformados.",
        cve = "N/A"
    };

    public static Vuln InsecureNFCImplementation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Insegura de NFC",
        Description = "La implementación NFC de la aplicación es insegura, potencialmente permitiendo acceso no autorizado a datos o acciones maliciosas a través de comunicación NFC.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:A/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar protocolos seguros de comunicación NFC, validar y sanitizar todos los datos NFC, y usar cifrado para la transmisión de datos sensibles a través de NFC.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible acceso no autorizado a datos sensibles o activación de acciones no deseadas a través de interacciones NFC maliciosas.",
        ProofOfConcept = "1. Analizar la implementación NFC de la aplicación.\n2. Crear etiquetas NFC maliciosas o usar un emulador NFC para enviar datos manipulados.\n3. Demostrar fuga de datos o acciones no autorizadas a través de comunicación NFC.",
        cve = "N/A"
    };

    public static Vuln InsecureARImplementation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Insegura de Realidad Aumentada (AR)",
        Description = "Las características de AR de la aplicación están implementadas de manera insegura, potencialmente llevando a violaciones de privacidad o brechas de seguridad a través de interacciones AR.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar manejo seguro de datos AR, asegurar permisos adecuados para acceso a cámara y sensores, y validar fuentes de contenido AR.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posibles violaciones de privacidad a través de acceso no autorizado a la cámara o recolección de datos, brechas de seguridad a través de contenido AR malicioso.",
        ProofOfConcept = "1. Analizar la implementación AR y manejo de datos de la aplicación.\n2. Intentar eludir permisos relacionados con AR o inyectar contenido AR malicioso.\n3. Demostrar violaciones de privacidad o brechas de seguridad a través de características AR.",
        cve = "N/A"
    };

    public static Vuln InsecureIoTIntegration => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Integración Insegura de IoT",
        Description = "La integración de la aplicación con dispositivos IoT es insegura, potencialmente permitiendo control no autorizado de dispositivos conectados o fuga de datos.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar protocolos de comunicación seguros para la interacción con dispositivos IoT, usar autenticación fuerte para el emparejamiento de dispositivos y validar todos los comandos enviados a dispositivos IoT.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible control no autorizado de dispositivos IoT, fuga de datos desde dispositivos conectados o violaciones de privacidad.",
        ProofOfConcept = "1. Analizar los protocolos de comunicación con dispositivos IoT de la aplicación.\n2. Intentar interceptar o manipular comunicaciones entre la aplicación y dispositivos IoT.\n3. Demostrar control no autorizado de dispositivos o acceso a datos.",
        cve = "N/A"
    };

    public static Vuln InsecurePushNotifications => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Manejo Inseguro de Notificaciones Push",
        Description = "El manejo de notificaciones push de la aplicación es inseguro, potencialmente permitiendo fuga de datos sensibles o ejecución de cargas útiles maliciosas a través de notificaciones.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar manejo seguro de cargas útiles de notificaciones push, validar y sanitizar datos de notificaciones, y evitar enviar información sensible en notificaciones.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición de datos sensibles a través de notificaciones push o ejecución de cargas útiles maliciosas.",
        ProofOfConcept = "1. Analizar el mecanismo de manejo de notificaciones push de la aplicación.\n2. Enviar notificaciones push manipuladas con cargas útiles maliciosas o datos sensibles.\n3. Demostrar fuga de datos o acciones no autorizadas a través de notificaciones push.",
        cve = "N/A"
    };
    
    public static Vuln InsecureAppCloning => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Vulnerabilidad a la Clonación de Aplicaciones",
        Description = "La aplicación es vulnerable a ataques de clonación, potencialmente permitiendo que copias no autorizadas de la aplicación accedan a datos o funcionalidad sensible.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar medidas anti-clonación como vinculación de dispositivos, usar almacenamiento de claves respaldado por hardware e implementar verificaciones de integridad en tiempo de ejecución.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible acceso no autorizado a datos o funcionalidad sensible a través de aplicaciones clonadas, elusión de licencias o controles de acceso.",
        ProofOfConcept = "1. Intentar clonar la aplicación usando varias herramientas o técnicas.\n2. Ejecutar la aplicación clonada en un dispositivo diferente.\n3. Demostrar acceso a datos sensibles o funcionalidad a través de la aplicación clonada.",
        cve = "N/A"
    };

    public static Vuln InsecureScreenOverlay => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Vulnerabilidad a Ataques de Superposición de Pantalla",
        Description = "La aplicación es vulnerable a ataques de superposición de pantalla, potencialmente permitiendo que aplicaciones maliciosas capturen entrada sensible del usuario.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implementar mecanismos de detección de superposición, usar métodos seguros de entrada para datos sensibles y educar a los usuarios sobre los riesgos de otorgar permisos de superposición a aplicaciones desconocidas.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible captura de entrada sensible del usuario como contraseñas o información de tarjetas de crédito.",
        ProofOfConcept = "1. Crear una aplicación de prueba de concepto de superposición.\n2. Intentar capturar entrada de usuario en la aplicación objetivo usando la superposición.\n3. Demostrar la captura exitosa de información sensible.",
        cve = "N/A"
    };

    public static Vuln InsecureAppWidget => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Insegura de Widget de Aplicación",
        Description = "La implementación del widget de la aplicación es insegura, potencialmente exponiendo datos o funcionalidad sensible en la pantalla de inicio del dispositivo.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar manejo seguro de datos en widgets de aplicación, evitar mostrar información sensible y usar autenticación apropiada para acciones del widget.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible exposición de datos sensibles en la pantalla de inicio del dispositivo o acceso no autorizado a funcionalidad de la aplicación a través del widget.",
        ProofOfConcept = "1. Analizar la implementación del widget de la aplicación.\n2. Intentar acceder a datos sensibles o funcionalidad a través del widget sin autenticación apropiada.\n3. Demostrar fuga de datos o acciones no autorizadas a través del widget.",
        cve = "N/A"
    };
    
    public static Vuln InsecureEdgeComputingIntegration => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Integración Insegura de Computación de Borde",
        Description = "La integración de la aplicación con servicios de computación de borde es insegura, potencialmente exponiendo datos sensibles o permitiendo acceso no autorizado a información procesada en el borde.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar protocolos seguros de comunicación para interacciones de computación de borde, usar autenticación y cifrado fuerte, y validar todos los datos recibidos de servicios de borde.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible acceso no autorizado a datos sensibles procesados en el borde, manipulación de datos o violaciones de privacidad.",
        ProofOfConcept = "1. Analizar la comunicación de la aplicación con servicios de computación de borde.\n2. Intentar interceptar o manipular datos entre la aplicación y servicios de borde.\n3. Demostrar acceso no autorizado a información sensible o inyección de datos maliciosos.",
        cve = "N/A"
    };

    public static Vuln InsecureAIMLImplementation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Insegura de Modelos IA/ML",
        Description = "La implementación de modelos de IA/ML de la aplicación es insegura, potencialmente permitiendo envenenamiento del modelo, extracción de datos o ataques adversarios.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar despliegue seguro de modelos IA/ML, usar aprendizaje federado cuando sea apropiado, proteger contra ataques de inversión de modelo e inferencia de membresía, y actualizar regularmente los modelos.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible extracción de datos de entrenamiento sensibles, manipulación de salidas del modelo IA/ML o inferencia no autorizada sobre usuarios.",
        ProofOfConcept = "1. Analizar la implementación del modelo IA/ML de la aplicación.\n2. Intentar ataques de inversión de modelo o inferencia de membresía.\n3. Demostrar extracción de información sensible o ejemplos adversarios exitosos.",
        cve = "N/A"
    };
    
    public static Vuln InsecureQuantumResistantCrypto => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Falta de Criptografía Resistente a Quantum",
        Description = "La aplicación no implementa algoritmos criptográficos resistentes a quantum, potencialmente haciéndola vulnerable a futuros ataques de computación cuántica.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.9,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar algoritmos criptográficos post-cuánticos para protección de datos sensibles y mecanismos de intercambio de claves.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potencial vulnerabilidad futura a ataques de computación cuántica, posiblemente llevando al descifrado de datos sensibles.",
        ProofOfConcept = "1. Analizar las implementaciones criptográficas de la aplicación.\n2. Identificar uso de algoritmos no resistentes a quantum para operaciones sensibles críticas.\n3. Demostrar vulnerabilidad teórica a algoritmos cuánticos conocidos.",
        cve = "N/A"
    };

    public static Vuln InsecureVoiceUIIntegration => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Integración Insegura de Interfaz de Usuario por Voz",
        Description = "La integración de la aplicación con interfaces de usuario por voz (VUI) es insegura, potencialmente permitiendo comandos de voz no autorizados o exponiendo información sensible a través de interacciones por voz.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar autenticación de voz segura, validar y sanitizar comandos de voz, y evitar exponer información sensible a través de respuestas de voz.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible acceso no autorizado a funcionalidad de la aplicación, exposición de información sensible a través de canales de voz, o ejecución de comandos de voz maliciosos.",
        ProofOfConcept = "1. Analizar la implementación de la interfaz de voz de la aplicación.\n2. Intentar eludir la autenticación por voz o inyectar comandos de voz maliciosos.\n3. Demostrar acciones no autorizadas o acceso a datos a través de interacciones por voz.",
        cve = "N/A"
    };

    public static Vuln InsecureMultiDeviceSynchronization => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Sincronización Insegura Multi-Dispositivo",
        Description = "El mecanismo de sincronización multi-dispositivo de la aplicación es inseguro, potencialmente permitiendo acceso no autorizado a datos de usuario entre dispositivos o interceptación de datos de sincronización.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar cifrado de extremo a extremo para sincronización multi-dispositivo, usar mecanismos seguros de emparejamiento de dispositivos y validar la integridad de los datos sincronizados.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible acceso no autorizado a datos de usuario en múltiples dispositivos, interceptación de información sensible durante la sincronización o inyección de datos maliciosos entre dispositivos.",
        ProofOfConcept = "1. Analizar el mecanismo de sincronización multi-dispositivo de la aplicación.\n2. Intentar interceptar o manipular datos de sincronización entre dispositivos.\n3. Demostrar acceso no autorizado a datos sincronizados o inyección de contenido malicioso.",
        cve = "N/A"
    };
    
    public static Vuln InsecureBlockchainIntegration => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Integración Insegura de Blockchain",
        Description = "La integración de la aplicación con tecnología blockchain es insegura, potencialmente exponiendo claves criptográficas, permitiendo transacciones no autorizadas o comprometiendo la integridad de los datos blockchain.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Implementar gestión segura de claves para interacciones blockchain, usar módulos de seguridad hardware cuando sea posible, validar todas las transacciones blockchain y asegurar controles de acceso apropiados a funcionalidades blockchain.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible robo de activos de criptomonedas, transacciones blockchain no autorizadas o compromiso de la integridad de aplicaciones descentralizadas (DApp).",
        ProofOfConcept = "1. Analizar la integración blockchain de la aplicación, enfocándose en la gestión de claves y firma de transacciones.\n2. Intentar extraer claves privadas o manipular datos de transacción.\n3. Demostrar transacciones blockchain no autorizadas o compromiso de funcionalidad DApp.",
        cve = "N/A"
    };

    public static Vuln InsecureKeychainKeystore => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Uso Inseguro de Keychain/Keystore",
        Description = "El uso del keychain (iOS) o keystore (Android) de la plataforma es inseguro, potencialmente exponiendo credenciales sensibles o claves criptográficas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar acceso seguro al keychain/keystore, usar clases de protección o niveles de seguridad apropiados, y evitar almacenar datos altamente sensibles en el keychain/keystore si es posible.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible exposición de credenciales sensibles o claves criptográficas, llevando a acceso no autorizado o descifrado de datos.",
        ProofOfConcept = "1. Analizar el uso del keychain/keystore de la aplicación.\n2. Intentar extraer datos del keychain/keystore usando un dispositivo con jailbreak/root.\n3. Demostrar acceso a información sensible almacenada en el keychain/keystore.",
        cve = "N/A"
    };
    
    public static Vuln InsecureRandomNumberGeneration => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Generación Insegura de Números Aleatorios",
        Description = "La aplicación utiliza métodos débiles o predecibles de generación de números aleatorios, potencialmente comprometiendo operaciones criptográficas o funcionalidades críticas de seguridad.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Usar generadores de números aleatorios criptográficamente seguros proporcionados por la plataforma o bibliotecas bien verificadas. Evitar usar Math.random() o PRNGs débiles similares para operaciones críticas de seguridad.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible predicción de valores generados, llevando a cifrado comprometido, tokens de sesión u otros datos críticos de seguridad.",
        ProofOfConcept = "1. Identificar uso de generación de números aleatorios en el código de la aplicación.\n2. Analizar la calidad de aleatoriedad de los valores generados.\n3. Demostrar predictibilidad o sesgo en los números aleatorios generados.",
        cve = "N/A"
    };

    public static Vuln InsecureSSOImplementation => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Implementación Insegura de Inicio de Sesión Único (SSO)",
        Description = "La implementación de Inicio de Sesión Único (SSO) o federación de la aplicación es defectuosa, potencialmente permitiendo acceso no autorizado o toma de control de cuentas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar prácticas seguras de SSO, validar correctamente los tokens OAuth, usar URIs de redirección seguros e implementar gestión adecuada de sesiones después de la autenticación SSO.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible acceso no autorizado a cuentas de usuario, secuestro de sesión o elusión de mecanismos de autenticación.",
        ProofOfConcept = "1. Analizar la implementación de SSO y manejo de tokens de la aplicación.\n2. Intentar falsificar o manipular tokens SSO.\n3. Demostrar acceso no autorizado usando credenciales SSO manipuladas.",
        cve = "N/A"
    };

    public static Vuln InsecureVPNUsage => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Uso Inseguro de VPN",
        Description = "El uso o implementación de funcionalidad VPN de la aplicación es inseguro, potencialmente exponiendo el tráfico del usuario o permitiendo acceso no autorizado a redes protegidas.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implementar protocolos VPN seguros, validar correctamente certificados de servidor, usar cifrado fuerte para túneles VPN y asegurar el manejo adecuado de credenciales VPN.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Posible exposición del tráfico del usuario, acceso no autorizado a redes protegidas o compromiso de credenciales VPN.",
        ProofOfConcept = "1. Analizar la implementación y configuración VPN de la aplicación.\n2. Intentar interceptar o manipular tráfico VPN.\n3. Demostrar fuga de datos o acceso no autorizado a través de vulnerabilidades VPN.",
        cve = "N/A"
    };

    public static Vuln InsecureCustomURLScheme => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Manejo Inseguro de Esquemas URL Personalizados",
        Description = "La implementación de esquemas URL personalizados de la aplicación es insegura, potencialmente permitiendo que otras aplicaciones invoquen funcionalidad sensible o accedan a datos protegidos.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implementar validación y sanitización adecuada de parámetros de esquemas URL personalizados, usar esquemas específicos de la aplicación y requerir confirmación del usuario para acciones sensibles activadas por esquemas URL.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible acceso no autorizado a funcionalidad o datos de la aplicación a través de URLs personalizadas maliciosamente creadas.",
        ProofOfConcept = "1. Identificar esquemas URL personalizados utilizados por la aplicación.\n2. Crear URLs maliciosas que exploten estos esquemas.\n3. Demostrar acciones no autorizadas o acceso a datos a través de la explotación de esquemas URL personalizados.",
        cve = "N/A"
    };

    public static Vuln TimeOfCheckToTimeOfUse => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Vulnerabilidad de Tiempo entre Comprobación y Uso (TOCTOU)",
        Description = "La aplicación contiene condiciones de carrera entre la verificación de una condición y el uso de los resultados de esa verificación, potencialmente llevando a problemas de seguridad.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar mecanismos adecuados de sincronización, usar operaciones atómicas cuando sea posible y diseñar código para minimizar la ventana de tiempo entre verificaciones y uso.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posible escalación de privilegios, acceso no autorizado a recursos o corrupción de datos debido a condiciones de carrera.",
        ProofOfConcept = "1. Identificar potenciales vulnerabilidades TOCTOU en el código de la aplicación.\n2. Crear un exploit de prueba de concepto que compita entre la verificación y el uso.\n3. Demostrar acciones no autorizadas o acceso a datos a través de la explotación exitosa de la condición de carrera.",
        cve = "N/A"
    };
    
    public static Vuln InsecureAntiDebugging => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Medidas Anti-Depuración Insuficientes",
        Description = "La aplicación carece de técnicas robustas anti-depuración, facilitando a los atacantes analizar y modificar el comportamiento de la aplicación en tiempo de ejecución.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:H/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implementar múltiples capas de técnicas anti-depuración, incluyendo verificaciones de código nativo, verificaciones de tiempo y detección de entorno. Actualizar y ofuscar regularmente estas protecciones.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Facilita la ingeniería inversa y manipulación de la aplicación, potencialmente exponiendo lógica sensible o eludiendo controles de seguridad.",
        ProofOfConcept = "1. Intentar adjuntar un depurador a la aplicación en ejecución.\n2. Analizar las medidas anti-depuración de la aplicación.\n3. Demostrar la elusión exitosa de las técnicas anti-depuración existentes.",
        cve = "N/A"
    };

    public static Vuln OverPrivilegedApplication => new Vuln
    {
        Template = true,
        Language = Language.Español,
        Name = "Aplicación con Privilegios Excesivos",
        Description = "La aplicación solicita más permisos de los necesarios para su funcionalidad, potencialmente exponiendo a los usuarios a mayores riesgos de privacidad y seguridad.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.9,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Revisar y minimizar las solicitudes de permisos de la aplicación, implementar solicitudes de permisos en tiempo de ejecución y explicar claramente a los usuarios por qué se necesita cada permiso.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Posibles violaciones de privacidad, acceso no autorizado a datos del usuario o incremento de la superficie de ataque debido a permisos innecesarios.",
        ProofOfConcept = "1. Analizar el manifiesto de la aplicación o info.plist para los permisos solicitados.\n2. Identificar permisos que no son esenciales para la funcionalidad principal.\n3. Demostrar el posible mal uso de permisos excesivos.",
        cve = "N/A"
    };
}