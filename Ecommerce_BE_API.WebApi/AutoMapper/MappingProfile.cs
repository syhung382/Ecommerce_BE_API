using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce_BE_API.DbContext.Models;
using Ecommerce_BE_API.DbContext.Models.Utils;

namespace Ecommerce_BE_API.WebApi.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<MstNotificationsReq, MstNotifications>().IncludeAllDerived().ReverseMap();
      
        }
    }
}
