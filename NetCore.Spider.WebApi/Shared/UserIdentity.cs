using NetCore.Jwt;
using NetCore.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Spider.WebApi.Shared
{
    public class UserIdentity : IUserIdentity
    {
        const string UserNameClaim = "uname";
        const string RoleClaim = JwtClaimConstants.RoleClaim;
        const string AgentIDClaim = "agtid";
        const string AgentNameClaim = "agtname";
        const string AgentTypeClaim = "agttyp";
        const string StaffIDClaim = "sid";
        const string StaffNameClaim = "sname";

        public string UserName { get; set; }

        public IList<string> Roles { get; set; }

        public int AgentID { get; set; }

        public string AgentName { get; set; }

        public string AgentType { get; set; }

        public int StaffID { get; set; }

        public string StaffName { get; set; }

        public void Bind(ClaimsPrincipal principal)
        {
            if (principal == null)
                return;

            Claim username = principal.FindFirst(UserNameClaim);
            if (username != null)
                this.UserName = username.Value;

            IEnumerable<Claim> roles = principal.FindAll(RoleClaim);
            if (roles != null)
                this.Roles = new ReadOnlyCollection<string>(roles.Select(r => r.Value).ToList());

            Claim agentId = principal.FindFirst(AgentIDClaim);
            if (agentId != null)
                this.AgentID = int.Parse(agentId.Value);

            Claim agentName = principal.FindFirst(AgentNameClaim);
            if (agentName != null)
                this.AgentName = agentName.Value;

            Claim agentType = principal.FindFirst(AgentTypeClaim);
            if (agentType != null)
                this.AgentType = agentType.Value;

            Claim staffId = principal.FindFirst(StaffIDClaim);
            if (staffId != null)
                this.StaffID = int.Parse(staffId.Value);

            Claim staffName = principal.FindFirst(StaffNameClaim);
            if (staffName != null)
                this.StaffName = staffName.Value;
        }

        public ClaimsPrincipal Build()
        {
            List<Claim> claims = new List<Claim>();

            if (!string.IsNullOrEmpty(UserName))
                claims.Add(new Claim(UserNameClaim, UserName, ClaimValueTypes.String));

            if (Roles != null)
            {
                foreach (string r in Roles.Where(r => !string.IsNullOrEmpty(r)).Distinct())
                {
                    claims.Add(new Claim(RoleClaim, r, ClaimValueTypes.String));
                }
            }

            if (AgentID != default(int))
                claims.Add(new Claim(AgentIDClaim, AgentID.ToString(), ClaimValueTypes.Integer32));

            if (!string.IsNullOrEmpty(AgentName))
                claims.Add(new Claim(AgentNameClaim, AgentName, ClaimValueTypes.String));

            if (!string.IsNullOrEmpty(AgentType))
                claims.Add(new Claim(AgentTypeClaim, AgentType, ClaimValueTypes.String));

            if (StaffID != default(int))
                claims.Add(new Claim(StaffIDClaim, StaffID.ToString(), ClaimValueTypes.Integer32));

            if (!string.IsNullOrEmpty(StaffName))
                claims.Add(new Claim(StaffNameClaim, StaffName, ClaimValueTypes.String));

            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
