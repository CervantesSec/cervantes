using Cervantes.CORE;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;

namespace Cervantes.Web.Models;

public class UserEditViewModel
{
    /// <summary>
    /// lista de usuarios
    /// </summary>
    public UserViewModel User { get; set; }

    /// <summary>
    /// lista de rol
    /// </summary>
    public RoleList Role { get; set; }

    /// <summary>
    /// opcion seleccionada
    /// </summary>
    public string Option { get; set; }

    public IList<SelectListItem> ItemList { get; set; }
}