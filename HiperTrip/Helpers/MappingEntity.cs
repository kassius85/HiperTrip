using AutoMapper;
using Entities.DTOs;
using Entities.Models;

namespace HiperTrip.Helpers
{
    public class MappingEntity : Profile
    {
        public MappingEntity()
        {
            CreateMap<UsuarioDto, Usuario>();
            CreateMap<Usuario, UsuarioDto>();
        }
    }
}