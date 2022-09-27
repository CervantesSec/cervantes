﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Cervantes.CORE;
using Cervantes.Contracts;
using Cervantes.DAL;
using System.Linq;

namespace Cervantes.Application;

public class RoleManager : GenericManager<IdentityRole>, IRoleManager
{
    /// <summary>
    /// Role Manager Constructor
    /// </summary>
    /// <param name="context">contexto de datos</param>
    public RoleManager(IApplicationDbContext context) : base(context)
    {
    }


    public IQueryable<IdentityRole> GetByName(string name)
    {
        return Context.Set<IdentityRole>().Where(x => x.Name == name);
    }
}