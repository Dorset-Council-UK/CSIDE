using AutoMapper;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Surveys;
namespace CSIDE.Data.AutoMapper
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<BridgeSurvey, InfrastructureBridgeDetails>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InfrastructureId, opt => opt.Ignore())
                .ForMember(dest => dest.BeamCondition, opt => opt.Ignore())
                .ForMember(dest => dest.DeckingCondition, opt => opt.Ignore())
                .ForMember(dest => dest.HandrailCondition, opt => opt.Ignore())
                .ForMember(dest => dest.HandrailPostsCondition, opt => opt.Ignore())
                .ForMember(dest => dest.BankSeatCondition, opt => opt.Ignore())
                .ForMember(dest => dest.BeamMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.DeckingMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.HandrailMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.HandrailPostsMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.BankSeatMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.Infrastructure, opt => opt.Ignore());
        }
    }
}
