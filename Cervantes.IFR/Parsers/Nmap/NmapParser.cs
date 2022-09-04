using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Cervantes.Contracts;


namespace Cervantes.IFR.Parsers.Nmap;

public class NmapParser: INmapParser
{

    private ITargetManager targetManager = null;
    private ITargetServicesManager targetServicesManager = null;
    
    public NmapParser(ITargetManager targetManager, ITargetServicesManager targetServicesManager)
    {
        this.targetManager = targetManager;
        this.targetServicesManager = targetServicesManager;
    }

    public void Parse(string path)
    {
        if (path == null)
        {
            return;
        }
        
        XElement nmap = XElement.Load(path);

        var hosts = nmap.Descendants("host");

        foreach (var host in hosts)
        {
            
        }
        
    }
}