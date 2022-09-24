using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;
using Microsoft.AspNetCore.Hosting;


namespace Cervantes.IFR.Parsers.Nmap;

public class NmapParser: INmapParser
{

    private ITargetManager targetManager = null;
    private ITargetServicesManager targetServicesManager = null;
    private readonly IHostingEnvironment _appEnvironment;
    
    public NmapParser(ITargetManager targetManager, ITargetServicesManager targetServicesManager, IHostingEnvironment _appEnvironment)
    {
        this.targetManager = targetManager;
        this.targetServicesManager = targetServicesManager;
        this._appEnvironment = _appEnvironment;
    }

    public void Parse(Guid project, string user, string path)
    {
        if (path == null)
        {
            return;
        }

        var file = _appEnvironment.WebRootPath+path;
        XElement nmap = XElement.Load(file);

        var hosts = nmap.Descendants("host");

        foreach (var host in hosts)
        {
            var status = host.Element("status");
            
            if ( status.Attribute("state").Value != "up"){
               continue;
            }

            var address = host.Element("address");
            
            Target target = new Target
            {
                Name = address.Attribute("addr").Value,
                Description = "Imported from Nmap",
                Type = TargetType.IP,
                ProjectId = project,
                UserId = user
            };

            targetManager.Add(target);
            targetManager.Context.SaveChanges();

            var ports = host.Descendants("ports");
            var servport = ports.Elements("port");

            foreach (var port in servport)
            {
                
                var number = port.Attribute("portid").Value;
                var protocol = port.Attribute("protocol").Value;

                var service = port.Element("service");

                var name = service.Attribute("name").Value;

                var product = "";
                if (service.Attribute("product") != null)
                {
                    product = service.Attribute("product").Value;
                }

                 
                var version = "";
                if (service.Attribute("version") != null)
                {
                    version = service.Attribute("version").Value;
                }
                

                var info = "";
                if (service.Attribute("extrainfo") != null)
                {
                    info = service.Attribute("extrainfo").Value;
                }

                StringBuilder sb = new StringBuilder();
                if (port.Elements("script") != null)
                {
                    var scripts = port.Elements("script");
                    foreach (var script in scripts)
                    {
                        string id = "";
                        if (script.Attribute("id") != null)
                        {
                            id = script.Attribute("id").Value;
                        }
                        sb.AppendLine(id);
                        sb.Append(Environment.NewLine);

                        string output = "";
                        if (script.Attribute("output") != null)
                        {
                            output = script.Attribute("output").Value;
                        }

                        sb.AppendLine(output);
                        sb.Append(Environment.NewLine);

                    }
                    
                }
                

                TargetServices targetService = new TargetServices
                {
                    UserId = user,
                    TargetId = target.Id,
                    Name = product + " (" + name +")",
                    Description = info + Environment.NewLine + sb.ToString(),
                    Port = Int32.Parse(number),
                    Version = version
                    
                };

                targetServicesManager.Add(targetService);
                targetServicesManager.Context.SaveChanges();

            }



        }
        
    }
}