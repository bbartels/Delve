using System.Collections.Generic;
using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Delve.Demo.Dto;
using Delve.Demo.Models;
using Delve.Demo.Persistence;
using Delve.AspNetCore;
using Delve.Extensions;
using Delve.Models;

namespace Delve.Demo.Controller
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IUrlHelper urlHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        /// <summary>
        /// Action to query Users from a database.
        /// </summary>
        /// <param name="param">The <see cref="IResourceParameter{T}"/> which is 
        /// automatically parsed from the request.</param>
        /// <returns>The resulting collection of users.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers(IResourceParameter<User> param)
        {
            var usersDb = await _unitOfWork.Users.GetAsync(param);
            var users = _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(usersDb);


            //Adds Paginationheader to response
            this.AddPaginationHeader(param, usersDb, _urlHelper);

            //Shapes data on return.
            return Ok(usersDb.ShapeData(param));
        }
    }
}