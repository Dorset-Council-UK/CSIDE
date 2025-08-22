using CSIDE.Data.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.Models.Surveys
{
    public class BridgeSurvey : Survey
    {
        public int InfrastructureItemId { get; set; }
        [Precision(10, 2)]
        public decimal? Height { get; set; }
        [Precision(10, 2)]
        public decimal? Width { get; set; }
        [Precision(10, 2)]
        public decimal? Length { get; set; }
        public int? BeamConditionId { get; set; }
        public Condition? BeamCondition { get; set; }
        public int? DeckingConditionId { get; set; }
        public Condition? DeckingCondition { get; set; }
        public int? HandrailConditionId { get; set; }
        public Condition? HandrailCondition { get; set; }
        public int? HandrailPostsConditionId { get; set; }
        public Condition? HandrailPostsCondition { get; set; }
        public int? BankSeatConditionId { get; set; }
        public Condition? BankSeatCondition { get; set; }

        public int? BeamMaterialId { get; set; }
        public Material? BeamMaterial { get; set; }
        public int? DeckingMaterialId { get; set; }
        public Material? DeckingMaterial { get; set; }
        public int? HandrailMaterialId { get; set; }
        public Material? HandrailMaterial { get; set; }
        public int? HandrailPostsMaterialId { get; set; }
        public Material? HandrailPostsMaterial { get; set; }
        public int? BankSeatMaterialId { get; set; }
        public Material? BankSeatMaterial { get; set; }

        public int? NumBeamTimbers { get; set; }
        public int? NumDeckingBoards { get; set; }
        public int? NumHandrailPostsTimbers { get; set; }

        public string? BeamTimbersSize { get; set; }
        public string? DeckingBoardsSize { get; set; }
        public decimal? DeckingBoardsLength { get; set; }
        public string? HandrailTimbersSize { get; set; }
        public string? HandrailPostsTimbersSize { get; set; }

        public bool? HandrailsInPlace { get; set; }
        public bool? Overgrown { get; set; }
        public bool? SignsOfBankErosion { get; set; }
        public bool? SeriouslyEroded { get; set; }
        public bool? HighUsage { get; set; }
        public bool? CoverBoardsInPlace { get; set; }
        public bool? RampInstalled { get; set; }
        public bool? StepsInstalled { get; set; }
        public bool? AntiSlipInstalled { get; set; }
        public bool? GateInstalled { get; set; }
        public bool? StileInstalled { get; set; }

        public double? UpdatedX { get; set; }
        public double? UpdatedY { get; set; }
        public int? LocationAccuracy { get; set; }
        public InfrastructureItem? Infrastructure { get; set; }
    }
}
