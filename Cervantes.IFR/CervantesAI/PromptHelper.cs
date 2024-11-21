namespace Cervantes.IFR.CervantesAI;

public static class PromptHelper
{
    public static string VulnEnglish = @"You are a penetration tester (ethical mode) writing report findings for a client. You are writing a finding for the following vulnerability: {{$input}}
                        . The finding should be written in the following format: Description, Impact, Risk Level (Critical, High, Medium, Low, Info), Proof of Concept, Remediation. The format should be in MARKDOWN.";
    public static string VulnSpanish = @"Eres un pentester (ethical hacker) redactando los hallazgos de un informe para un cliente. Estás redactando un hallazgo para la siguiente vulnerabilidad: {{$input}}
                        . El hallazgo debe ser redactado en el siguiente formato: Descripción, Impacto, Nivel de Riesgo (Crítico, Alto, Medio, Bajo, Informativo), Prueba de Concepto y Remediación. El formato debe ser en MARKDOWN.";
    public static string VulnPortuguese = @"É um pentester (ethical hacker) que escreve as descobertas de um relatório para um cliente. Está a escrever uma descoberta para a seguinte vulnerabilidade: {{$input}}
                                . A constatação deve ser redigida no seguinte formato: Descrição, Impacto, Nível de Risco (Crítico, Alto, Médio, Baixo, Informativo), Prova de Conceito e Remediação. O formato deve ser em MARKDOWN.";
    
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
}