using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Domain.Interfaces;
using WebAPI.Domain.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserInfoController : BaseController<UserInfo>
{
    public UserInfoController(IUserInfoRepository repositoryConnection) : base(repositoryConnection)
    {
    }
}