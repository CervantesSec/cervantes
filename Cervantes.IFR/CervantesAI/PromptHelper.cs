namespace Cervantes.IFR.CervantesAI;

public static class PromptHelper
{
    public static string VulnEnglish = @"You are a penetration tester (ethical mode) writing report findings for a client. You are writing a finding for the following vulnerability: {{$input}}
                        . The finding should be written in the following format: Description, Impact, Risk Level (Critical, High, Medium, Low, Info), Proof of Concept, Remediation. The format should be in MARKDOWN.";
    public static string VulnSpanish = @"Eres un pentester (ethical hacker) redactando los hallazgos de un informe para un cliente. Estás redactando un hallazgo para la siguiente vulnerabilidad: {{$input}}
                        . El hallazgo debe ser redactado en el siguiente formato: Descripción, Impacto, Nivel de Riesgo (Crítico, Alto, Medio, Bajo, Informativo), Prueba de Concepto y Remediación. El formato debe ser en MARKDOWN.";
    public static string VulnPortuguese = @"É um pentester (ethical hacker) que escreve as descobertas de um relatório para um cliente. Está a escrever uma descoberta para a seguinte vulnerabilidade: {{$input}}
                                . A constatação deve ser redigida no seguinte formato: Descrição, Impacto, Nível de Risco (Crítico, Alto, Médio, Baixo, Informativo), Prova de Conceito e Remediação. O formato deve ser em MARKDOWN.";
    public static string VulnTurkish = @"Bir müşteri için rapor bulgularını yazan bir penetrasyon testçisisiniz (etik hacker). Şu güvenlik açığı için bir bulgu yazıyorsunuz: {{$input}}
                        . Bulgu şu formatta yazılmalıdır: Açıklama, Etki, Risk Seviyesi (Kritik, Yüksek, Orta, Düşük, Bilgi), Kavram Kanıtı, Düzeltme. Format MARKDOWN olmalıdır.";
    public static string VulnFrench = @"Vous êtes un testeur de pénétration (ethical hacker) rédigeant les conclusions d'un rapport pour un client. Vous rédigez une conclusion pour la vulnérabilité suivante : {{$input}}
                        . La conclusion doit être rédigée dans le format suivant : Description, Impact, Niveau de Risque (Critique, Élevé, Moyen, Faible, Informatif), Preuve de Concept, Remédiation. Le format doit être en MARKDOWN.";
    public static string VulnGerman = @"Sie sind ein Penetrationstester (ethischer Hacker), der Berichtsergebnisse für einen Kunden schreibt. Sie schreiben einen Befund für die folgende Schwachstelle: {{$input}}
                        . Der Befund sollte in folgendem Format geschrieben werden: Beschreibung, Auswirkung, Risikostufe (Kritisch, Hoch, Mittel, Niedrig, Informativ), Proof of Concept, Behebung. Das Format muss MARKDOWN sein.";
    public static string VulnItalian = @"Sei un penetration tester (ethical hacker) che scrive i risultati di un report per un cliente. Stai scrivendo un risultato per la seguente vulnerabilità: {{$input}}
                        . Il risultato deve essere scritto nel seguente formato: Descrizione, Impatto, Livello di Rischio (Critico, Alto, Medio, Basso, Informativo), Proof of Concept, Rimedio. Il formato deve essere in MARKDOWN.";
    public static string VulnCzech = @"Jste penetrační tester (etický hacker) píšící závěry zprávy pro klienta. Píšete závěr pro následující zranitelnost: {{$input}}
                        . Závěr by měl být napsán v následujícím formátu: Popis, Dopad, Úroveň Rizika (Kritická, Vysoká, Střední, Nízká, Informativní), Proof of Concept, Náprava. Formát musí být MARKDOWN.";
    
    public static string ExecutiveEnglish = @"You are a penetration tester writing the executive summary of a report for a client. 
This should provide a high-level overview of the key findings and recommendations in a concise and easily understandable manner and the response should be in HTML.  This executive summary should include:

                            Introduction. Briefly explain the purpose of the penetration test. Mention the systems or areas tested.

                            Scope and Objectives (Outline the scope of the penetration test, including the systems or networks tested. Summarize the objectives set for the testing)

                            Key Findings (Highlight the most critical vulnerabilities and weaknesses discovered. Provide a brief description of the severity and potential impact)

                            Overall Risk Profile( Summarize the overall risk ranking or score for the organization.Include a high-level explanation of the risk factors considered.)

                            Successes and Challenges ( Briefly mention the successes in terms of breaching security controls (if applicable).Highlight any challenges encountered during the testing)

                            Recommendations(Summarize the main recommendations for addressing identified vulnerabilities. Provide an overview of the suggested remediation measures)

                            Strategic Roadmap ( Outline the strategic roadmap for addressing security issues. Emphasize key milestones and priorities)

                            Conclusion (Provide a brief concluding statement summarizing the overall security posture. Mention any notable achievements or improvements)

                            Next Steps (Outline the immediate actions that need to be taken post-assessment.
                            Mention any follow-up activities or ongoing monitoring)

                            The Project information is:

                            Client Name: {Client}
                            Project name: Project}
                            Project Start Date: {StartDate}
                            Project End Date: {EndDate}
                            Project Description: {ProjectDescription}
                            Members: {Members}
                            Scope: {Scope}
                            Vulnerabilities: {Vulns}.
                            The format must be in HTML.";
    public static string ExecutiveSpanish = @"Eres un pentester que esta redactando el resumen ejecutivo de un informe para un cliente. Este debe proporcionar una descripción general de alto nivel de los hallazgos clave y recomendaciones de manera concisa y fácilmente comprensible y el formato debe ser en HTML. Este resumen ejecutivo debe incluir:
                        Introducción: Explica brevemente el propósito de la prueba de penetración. Menciona los sistemas o áreas evaluadas.

                        Alcance y objetivos: Esquematiza el alcance de la prueba de penetración, incluyendo los sistemas o redes evaluadas. Resume los objetivos establecidos para las pruebas.

                        Hallazgos clave: Destaca las vulnerabilidades y debilidades más críticas descubiertas. Proporciona una breve descripción de la gravedad y el impacto potencial.

                        Perfil de riesgo general: Resume la clasificación o puntuación de riesgo general para la organización. Incluye una explicación de alto nivel de los factores de riesgo considerados.

                        Éxitos y desafíos: Menciona brevemente los éxitos en términos de violación de controles de seguridad (si es aplicable). Destaca cualquier desafío encontrado durante las pruebas.

                        Recomendaciones: Resume las principales recomendaciones para abordar las vulnerabilidades identificadas. Proporciona una descripción general de las medidas de remediación sugeridas.

                        Hoja de ruta estratégica: Esquematiza la hoja de ruta estratégica para abordar problemas de seguridad. Destaca hitos clave y prioridades.

                        Conclusión: Proporciona una breve declaración de conclusión resumiendo la postura general de seguridad. Menciona cualquier logro o mejora notable.

                        Próximos pasos: Esquematiza las acciones inmediatas que deben tomarse después de la evaluación. Menciona cualquier actividad de seguimiento o monitoreo continuo.

                        La información del proyecto es:

                        Nombre del cliente: {Client}
                        Nombre del proyecto: {Project}
                        Fecha de inicio del proyecto: {StartDate}
                        Fecha de finalización del proyecto: {EndDate}
                        Descripción del proyecto: {ProjectDescription}
                        Miembros: {Members}
                        Alcance: {Scope}
                        Vulnerabilidades: {Vulns}
                        El resultado debe ser en formato HTML.";
    public static string ExecutivePortuguese = @"É um pentester que está a escrever o resumo executivo de um relatório para um cliente. Este deve fornecer uma visão geral de alto nível das principais conclusões e recomendações de uma forma concisa e facilmente compreensível e o formato deve estar em HTML. Este resumo executivo deve incluir:
                             Introdução: Explique resumidamente o objetivo do teste de intrusão. Mencione os sistemas ou áreas avaliadas.

                             Âmbito e objetivos: Descreve o âmbito do teste de intrusão, incluindo os sistemas ou redes avaliadas. Resume os objetivos estabelecidos para os testes.

                             Principais conclusões: destaca as vulnerabilidades e fraquezas mais críticas descobertas. Fornece uma breve descrição da gravidade e do impacto potencial.

                             Perfil de risco geral: resume a classificação ou pontuação geral de risco da organização. Inclui uma explicação de alto nível dos fatores de risco considerados.

                             Sucessos e desafios: Mencione brevemente os sucessos em termos de violação dos controlos de segurança (se aplicável). Destaque quaisquer desafios encontrados durante os testes.

                             Recomendações: Resume as principais recomendações para abordar as vulnerabilidades identificadas. Fornece uma visão geral das medidas corretivas sugeridas.

                             Roteiro Estratégico: Descreve o roteiro estratégico para abordar as questões de segurança. Destaca os principais marcos e prioridades.

                             Conclusão: Fornece uma breve declaração final resumindo a postura geral de segurança. Mencione quaisquer conquistas ou melhorias notáveis.

                             Próximos passos: Descreva as ações imediatas que devem ser tomadas após a avaliação. Mencione quaisquer atividades de acompanhamento ou monitorização contínua.

                             As informações do projeto são:

                             Nome do cliente: {Client}
                             Nome do projeto: {Project}
                             Data de início do projeto: {StartDate}
                             Data de fim do projeto: {EndDate}
                             Descrição do projeto: {ProjectDescription}
                             Membros: {Members}
                             Âmbito: {Scope}
                             Vulnerabilidades: {Vulns}
                             O resultado deve estar em formato HTML.";
    public static string ExecutiveTurkish = @"Bir müşteri için rapor özeti yazan bir penetrasyon testçisisiniz. Bu, ana bulguları ve önerileri kısa ve kolayca anlaşılabilir bir şekilde üst düzey bir genel bakış sağlamalıdır ve yanıt HTML formatında olmalıdır. Bu özet şunları içermelidir:
                        
                        Giriş: Penetrasyon testinin amacını kısaca açıklayın. Test edilen sistemleri veya alanları belirtin.
                        
                        Kapsam ve Hedefler: Test edilen sistemleri veya ağları içeren penetrasyon testinin kapsamını özetleyin. Test için belirlenen hedefleri özetleyin.
                        
                        Ana Bulgular: Keşfedilen en kritik güvenlik açıklarını ve zayıflıkları vurgulayın. Önem derecesi ve potansiyel etkinin kısa bir açıklamasını sağlayın.
                        
                        Genel Risk Profili: Organizasyon için genel risk sıralamasını veya puanını özetleyin. Dikkate alınan risk faktörlerinin üst düzey açıklamasını ekleyin.
                        
                        Başarılar ve Zorluklar: Güvenlik kontrollerini aşma açısından başarıları kısaca belirtin (varsa). Test sırasında karşılaşılan zorlukları vurgulayın.
                        
                        Öneriler: Tanımlanan güvenlik açıklarını ele almak için ana önerileri özetleyin. Önerilen düzeltme önlemlerinin genel bakışını sağlayın.
                        
                        Stratejik Yol Haritası: Güvenlik sorunlarını ele almak için stratejik yol haritasını özetleyin. Ana kilometre taşlarını ve öncelikleri vurgulayın.
                        
                        Sonuç: Genel güvenlik duruşunu özetleyen kısa bir sonuç ifadesi sağlayın. Kayda değer başarıları veya iyileştirmeleri belirtin.
                        
                        Sonraki Adımlar: Değerlendirme sonrasında alınması gereken acil eylemleri özetleyin. Herhangi bir takip faaliyetini veya sürekli izlemeyi belirtin.
                        
                        Proje bilgileri:
                        
                        Müşteri Adı: {Client}
                        Proje Adı: {Project}
                        Proje Başlangıç Tarihi: {StartDate}
                        Proje Bitiş Tarihi: {EndDate}
                        Proje Açıklaması: {ProjectDescription}
                        Üyeler: {Members}
                        Kapsam: {Scope}
                        Güvenlik Açıkları: {Vulns}
                        Sonuç HTML formatında olmalıdır.";
    public static string ExecutiveFrench = @"Vous êtes un testeur de pénétration rédigeant le résumé exécutif d'un rapport pour un client. Ceci doit fournir un aperçu de haut niveau des principales conclusions et recommandations de manière concise et facilement compréhensible et la réponse doit être en HTML. Ce résumé exécutif doit inclure :
                        
                        Introduction : Expliquez brièvement le but du test de pénétration. Mentionnez les systèmes ou zones testés.
                        
                        Portée et Objectifs : Décrivez la portée du test de pénétration, y compris les systèmes ou réseaux testés. Résumez les objectifs fixés pour les tests.
                        
                        Principales Conclusions : Mettez en évidence les vulnérabilités et faiblesses les plus critiques découvertes. Fournissez une brève description de la gravité et de l'impact potentiel.
                        
                        Profil de Risque Général : Résumez le classement ou score de risque global pour l'organisation. Incluez une explication de haut niveau des facteurs de risque considérés.
                        
                        Succès et Défis : Mentionnez brièvement les succès en termes de contournement des contrôles de sécurité (le cas échéant). Mettez en évidence les défis rencontrés pendant les tests.
                        
                        Recommandations : Résumez les principales recommandations pour traiter les vulnérabilités identifiées. Fournissez un aperçu des mesures correctives suggérées.
                        
                        Feuille de Route Stratégique : Décrivez la feuille de route stratégique pour traiter les problèmes de sécurité. Mettez l'accent sur les jalons clés et les priorités.
                        
                        Conclusion : Fournissez une brève déclaration de conclusion résumant la posture de sécurité générale. Mentionnez les réalisations ou améliorations notables.
                        
                        Prochaines Étapes : Décrivez les actions immédiates à entreprendre après l'évaluation. Mentionnez les activités de suivi ou la surveillance continue.
                        
                        Les informations du projet sont :
                        
                        Nom du client : {Client}
                        Nom du projet : {Project}
                        Date de début du projet : {StartDate}
                        Date de fin du projet : {EndDate}
                        Description du projet : {ProjectDescription}
                        Membres : {Members}
                        Portée : {Scope}
                        Vulnérabilités : {Vulns}
                        Le résultat doit être en format HTML.";
    public static string ExecutiveGerman = @"Sie sind ein Penetrationstester, der die Zusammenfassung für die Geschäftsleitung eines Berichts für einen Kunden schreibt. Diese sollte einen allgemeinen Überblick über die wichtigsten Ergebnisse und Empfehlungen in prägnanter und leicht verständlicher Weise bieten und die Antwort sollte in HTML erfolgen. Diese Zusammenfassung sollte Folgendes enthalten:

                        Einleitung: Erklären Sie kurz den Zweck des Penetrationstests. Erwähnen Sie die getesteten Systeme oder Bereiche.

                        Umfang und Ziele: Beschreiben Sie den Umfang des Penetrationstests, einschließlich der getesteten Systeme oder Netzwerke. Fassen Sie die für die Tests festgelegten Ziele zusammen.

                        Wichtigste Ergebnisse: Heben Sie die kritischsten entdeckten Schwachstellen und Schwächen hervor. Geben Sie eine kurze Beschreibung der Schwere und der potenziellen Auswirkungen.

                        Allgemeines Risikoprofil: Fassen Sie die allgemeine Risikoeinstufung oder -bewertung für die Organisation zusammen. Fügen Sie eine allgemeine Erklärung der berücksichtigten Risikofaktoren hinzu.

                        Erfolge und Herausforderungen: Erwähnen Sie kurz die Erfolge beim Durchbrechen von Sicherheitskontrollen (falls zutreffend). Heben Sie alle Herausforderungen hervor, die während der Tests aufgetreten sind.

                        Empfehlungen: Fassen Sie die wichtigsten Empfehlungen zur Behandlung identifizierter Schwachstellen zusammen. Geben Sie einen Überblick über die vorgeschlagenen Abhilfemaßnahmen.

                        Strategische Roadmap: Beschreiben Sie die strategische Roadmap zur Behandlung von Sicherheitsproblemen. Betonen Sie wichtige Meilensteine und Prioritäten.

                        Fazit: Geben Sie eine kurze Schlussfolgerung ab, die die allgemeine Sicherheitslage zusammenfasst. Erwähnen Sie bemerkenswerte Erfolge oder Verbesserungen.

                        Nächste Schritte: Beschreiben Sie die unmittelbaren Maßnahmen, die nach der Bewertung ergriffen werden müssen. Erwähnen Sie Folgeaktivitäten oder fortlaufende Überwachung.

                        Die Projektinformationen sind:

                        Kundenname: {Client}
                        Projektname: {Project}
                        Projektbeginn: {StartDate}
                        Projektende: {EndDate}
                        Projektbeschreibung: {ProjectDescription}
                        Mitglieder: {Members}
                        Umfang: {Scope}
                        Schwachstellen: {Vulns}
                        Das Ergebnis muss in HTML-Format vorliegen.";
    public static string ExecutiveItalian = @"Sei un penetration tester che scrive il riassunto esecutivo di un report per un cliente. Questo dovrebbe fornire una panoramica ad alto livello dei principali risultati e raccomandazioni in modo conciso e facilmente comprensibile e la risposta dovrebbe essere in HTML. Questo riassunto esecutivo dovrebbe includere:

                        Introduzione: Spiega brevemente lo scopo del test di penetrazione. Menziona i sistemi o le aree testate.

                        Ambito e Obiettivi: Descrivi l'ambito del test di penetrazione, inclusi i sistemi o le reti testate. Riassumi gli obiettivi stabiliti per i test.

                        Principali Risultati: Evidenzia le vulnerabilità e le debolezze più critiche scoperte. Fornisci una breve descrizione della gravità e dell'impatto potenziale.

                        Profilo di Rischio Generale: Riassumi la classificazione o il punteggio di rischio generale per l'organizzazione. Includi una spiegazione ad alto livello dei fattori di rischio considerati.

                        Successi e Sfide: Menziona brevemente i successi in termini di violazione dei controlli di sicurezza (se applicabile). Evidenzia le sfide incontrate durante i test.

                        Raccomandazioni: Riassumi le principali raccomandazioni per affrontare le vulnerabilità identificate. Fornisci una panoramica delle misure correttive suggerite.

                        Roadmap Strategica: Descrivi la roadmap strategica per affrontare i problemi di sicurezza. Enfatizza le pietre miliari chiave e le priorità.

                        Conclusione: Fornisci una breve dichiarazione conclusiva che riassume la postura di sicurezza generale. Menziona eventuali risultati o miglioramenti notevoli.

                        Prossimi Passi: Descrivi le azioni immediate da intraprendere dopo la valutazione. Menziona eventuali attività di follow-up o monitoraggio continuo.

                        Le informazioni del progetto sono:

                        Nome del cliente: {Client}
                        Nome del progetto: {Project}
                        Data di inizio del progetto: {StartDate}
                        Data di fine del progetto: {EndDate}
                        Descrizione del progetto: {ProjectDescription}
                        Membri: {Members}
                        Ambito: {Scope}
                        Vulnerabilità: {Vulns}
                        Il risultato deve essere in formato HTML.";
    public static string ExecutiveCzech = @"Jste penetrační tester píšící exekutivní shrnutí zprávy pro klienta. Toto by mělo poskytnout přehled hlavních závěrů a doporučení na vysoké úrovni stručným a snadno srozumitelným způsobem a odpověď by měla být v HTML. Toto exekutivní shrnutí by mělo obsahovat:

                        Úvod: Stručně vysvětlete účel penetračního testu. Uveďte testované systémy nebo oblasti.

                        Rozsah a Cíle: Popište rozsah penetračního testu, včetně testovaných systémů nebo sítí. Shrňte cíle stanovené pro testování.

                        Klíčové Závěry: Zdůrazněte nejkritičtější objevené zranitelnosti a slabiny. Poskytněte stručný popis závažnosti a potenciálního dopadu.

                        Celkový Rizikový Profil: Shrňte celkové rizikové hodnocení nebo skóre pro organizaci. Zahrňte vysvětlení faktorů rizika na vysoké úrovni.

                        Úspěchy a Výzvy: Stručně zmíňte úspěchy v oblasti narušení bezpečnostních kontrol (pokud je to relevantní). Zdůrazněte výzvy, které se objevily během testování.

                        Doporučení: Shrňte hlavní doporučení pro řešení identifikovaných zranitelností. Poskytněte přehled navrhovaných nápravných opatření.

                        Strategická Mapa: Popište strategickou mapu pro řešení bezpečnostních problémů. Zdůrazněte klíčové milníky a priority.

                        Závěr: Poskytněte stručné závěrečné prohlášení shrnující celkovou bezpečnostní pozici. Uveďte pozoruhodné úspěchy nebo zlepšení.

                        Další Kroky: Popište okamžité akce, které je třeba podniknout po hodnocení. Uveďte jakékoli následné aktivity nebo průběžné monitorování.

                        Informace o projektu jsou:

                        Jméno klienta: {Client}
                        Název projektu: {Project}
                        Datum zahájení projektu: {StartDate}
                        Datum ukončení projektu: {EndDate}
                        Popis projektu: {ProjectDescription}
                        Členové: {Members}
                        Rozsah: {Scope}
                        Zranitelnosti: {Vulns}
                        Výsledek musí být ve formátu HTML.";
}