using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.IFR.VulnTemplates;

public class VulnTemplatesLoadService: IVulnTemplatesLoadService
{
    private IVulnManager vulnManager = null;
    private IVulnCweManager vulnCweManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private readonly ILogger<VulnTemplatesLoadService> _logger = null;

    public VulnTemplatesLoadService(IVulnManager vulnManager, IVulnCweManager vulnCweManager,
        IVulnCategoryManager vulnCategoryManager,ILogger<VulnTemplatesLoadService> logger)
    {
        this.vulnManager = vulnManager;
        this.vulnCweManager = vulnCweManager;
        this.vulnCategoryManager = vulnCategoryManager;
        _logger = logger;
    }

    public async Task CreateEnglish(string type)
    {
        switch (type)
        {
            case "All":
                await CreateEnglishGeneral();
                await CreateEnglishMastg();
                await CreateEnglishWstg();
                break;
            case "Wstg":
                await CreateEnglishWstg();
                break;
            case "Mastg":
                await CreateEnglishMastg();
                break;
            case "General":
                await CreateEnglishGeneral();
                break;
            
        }
        
    }
    
    public async Task CreateSpanish(string type)
    {
        switch (type)
        {
            case "All":
                await CreateSpanishGeneral();
                await CreateSpanishMastg();
                await CreateSpanishWstg();
                break;
            case "Wstg":
                await CreateSpanishWstg();
                break;
            case "Mastg":
                await CreateSpanishMastg();
                break;
            case "General":
                await CreateSpanishGeneral();
                break;
            
        }
    }
    
    public async Task CreatePortuguese(string type)
    {
        switch (type)
        {
            case "All":
                await CreatePortugueseWstg();
                await CreatePortugueseMastg();
                await CreatePortugueseGeneral();
                break;
            case "Wstg":
                await CreatePortugueseWstg();
                break;
            case "Mastg":
                await CreatePortugueseMastg();
                break;
            case "General":
                await CreatePortugueseGeneral();
                break;
            
        }
    }
    
    public async Task CreateEnglishGeneral()
    {
        
    }
    
    public async Task CreateEnglishMastg()
    {
        
    }
    
    public async Task CreateEnglishWstg()
    {
        
    }
    
    public async Task CreateSpanishGeneral()
    {
        
    }
    
    public async Task CreateSpanishMastg()
    {
        
    }
    
    public async Task CreateSpanishWstg()
    {
        
    }
    
    public async Task CreatePortugueseGeneral()
    {
        
    }
    
    public async Task CreatePortugueseMastg()
    {
        
    }
    
    public async Task CreatePortugueseWstg()
    {
        
    }
}