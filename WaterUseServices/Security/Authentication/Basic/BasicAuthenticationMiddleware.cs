﻿//------------------------------------------------------------------------------
//----- Authentication -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   
//
//discussion:   Authentication is the process of determining who you are, while Authorisation 
//              evolves around what you are allowed to do, i.e. permissions. Obviously before 
//              you can determine what a user is allowed to do, you need to know who they are, 
//              so when authorisation is required, you must also first authenticate the user in some way.  
//          
//              https://andrewlock.net/introduction-to-authentication-with-asp-net-core/

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using WaterUseAgent;

namespace WaterUseServices.Security.Authentication.Basic
{
    public class BasicAuthenticationMiddleware : AuthenticationMiddleware<BasicAuthenticationOptions>
    {
        private IWaterUseAgent _agent;
        public BasicAuthenticationMiddleware(RequestDelegate next, IOptions<BasicAuthenticationOptions> options, 
            ILoggerFactory loggerFactory, UrlEncoder encoder,IWaterUseAgent agent) : base(next, options, loggerFactory, encoder)
        {
            this._agent = agent;
        }

        protected override AuthenticationHandler<BasicAuthenticationOptions> CreateHandler()
        {
            return new BasicAuthenticationHandler(_agent);
        }
    }
}
