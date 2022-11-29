﻿using AutoMapper;
using MagicVilla2_WebAPI.Models;
using MagicVilla2_WebAPI.Models.Dto;

namespace MagicVilla2_WebAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Villa, VillaDTO>().ReverseMap();
        CreateMap<Villa, VillaCreateDTO>().ReverseMap();
        CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

        //CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
        //CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
        //CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
    }
}