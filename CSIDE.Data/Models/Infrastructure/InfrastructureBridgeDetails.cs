using CSIDE.Data.Models.Surveys;

namespace CSIDE.Data.Models.Infrastructure
{
    public class InfrastructureBridgeDetails
    {
        public int Id { get; set; }
        public int InfrastructureId { get; set; }

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

        public InfrastructureItem? Infrastructure { get; set; }
    }
}
