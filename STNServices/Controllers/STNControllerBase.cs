using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using STNDB.Resources;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using STNAgent;

namespace STNServices.Controllers
{

    public abstract class STNControllerBase : WiM.Services.Controllers.ControllerBase
    {
        #region Constants
        protected const string AdminRole = "Admin";
        protected const string ManagerRole = "Manager";
        protected const string FieldRole = "Field";
        protected const string PublicRole = "Public";

        #endregion
        protected ISTNServicesAgent agent;

        public STNControllerBase(ISTNServicesAgent sa)
        {
            this.agent = sa;
        }
        public bool IsAuthorizedToEdit<T> () where T:class
        {

            if (User.IsInRole("Administrator")) return true;

            var username = LoggedInUser();

            switch (typeof(T).Name)
            {
                case "Source":

                default:
                    break;
            }

            return false;
        }
        public members LoggedInUser() {
            if (User == null) return null;
            return new members()
            {
                member_id = Convert.ToInt32( User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                   .Select(c => c.Value).SingleOrDefault()),
                fname = User.Claims.Where(c => c.Type == ClaimTypes.Name)
                   .Select(c => c.Value).SingleOrDefault(),
                lname = User.Claims.Where(c => c.Type == ClaimTypes.Surname)
                   .Select(c => c.Value).SingleOrDefault(),
                username = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault(),
                role_id = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.Anonymous)
                   .Select(c => c.Value).SingleOrDefault())
            };
        }
        
        private Dictionary<int, string> dbBadRequestErrors = new Dictionary<int, string>
        {
            //https://www.postgresql.org/docs/9.4/static/errcodes-appendix.html
            {23502, "One of the properties requires a value."},
            {23505, "One of the properties is marked as Unique index and there is already an entry with that value."},
            {23503, "One of the related features prevents you from performing this operation to the database." }
        };
        
        public void sm(List<WiM.Resources.Message> messages)
        {
            HttpContext.Items[WiM.Services.Middleware.X_MessagesExtensions.msgKey] = messages;
        }
    }
}
