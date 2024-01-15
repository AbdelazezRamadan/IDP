using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace IDP.Extensions
{
    public static class IdentityExtensions
    {
        public static Client ToEntity(this IdentityServer4.Models.Client model, IMapper mapper)
        {
            return mapper.Map<Client>(model);
        }

        public static IdentityResource ToEntity(this IdentityServer4.Models.IdentityResource model, IMapper mapper)
        {
            return mapper.Map<IdentityResource>(model);
        }

        public static ApiScope ToEntity(this IdentityServer4.Models.ApiScope model, IMapper mapper)
        {
            return mapper.Map<ApiScope>(model);
        }

        public static ApiResource ToEntity(this IdentityServer4.Models.ApiResource model, IMapper mapper)
        {
            return mapper.Map<ApiResource>(model);
        }
    }
}
