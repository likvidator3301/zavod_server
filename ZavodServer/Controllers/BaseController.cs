using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Filters;

namespace ZavodServer.Controllers
{
    [GoogleAuthorizeFilter]
    public class BaseController: ControllerBase
    {
    }
}