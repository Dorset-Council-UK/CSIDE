using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Surveys;

namespace CSIDE.Data.Mapping
{
    public static class InfrastructureMapper
    {
        public static InfrastructureBridgeDetails MapToInfrastructureBridgeDetails(this BridgeSurvey survey)
        {
            var details = new InfrastructureBridgeDetails();
            survey.MapTo(details);
            return details;
        }

        public static void MapTo(this BridgeSurvey survey, InfrastructureBridgeDetails target)
        {
            target.BeamConditionId = survey.BeamConditionId;
            target.DeckingConditionId = survey.DeckingConditionId;
            target.HandrailConditionId = survey.HandrailConditionId;
            target.HandrailPostsConditionId = survey.HandrailPostsConditionId;
            target.BankSeatConditionId = survey.BankSeatConditionId;
            target.BeamMaterialId = survey.BeamMaterialId;
            target.DeckingMaterialId = survey.DeckingMaterialId;
            target.HandrailMaterialId = survey.HandrailMaterialId;
            target.HandrailPostsMaterialId = survey.HandrailPostsMaterialId;
            target.BankSeatMaterialId = survey.BankSeatMaterialId;
            target.NumBeamTimbers = survey.NumBeamTimbers;
            target.NumDeckingBoards = survey.NumDeckingBoards;
            target.NumHandrailPostsTimbers = survey.NumHandrailPostsTimbers;
            target.BeamTimbersSize = survey.BeamTimbersSize;
            target.DeckingBoardsSize = survey.DeckingBoardsSize;
            target.DeckingBoardsLength = survey.DeckingBoardsLength;
            target.HandrailTimbersSize = survey.HandrailTimbersSize;
            target.HandrailPostsTimbersSize = survey.HandrailPostsTimbersSize;
            target.HandrailsInPlace = survey.HandrailsInPlace;
            target.Overgrown = survey.Overgrown;
            target.SignsOfBankErosion = survey.SignsOfBankErosion;
            target.SeriouslyEroded = survey.SeriouslyEroded;
            target.HighUsage = survey.HighUsage;
            target.CoverBoardsInPlace = survey.CoverBoardsInPlace;
            target.RampInstalled = survey.RampInstalled;
            target.StepsInstalled = survey.StepsInstalled;
            target.AntiSlipInstalled = survey.AntiSlipInstalled;
            target.GateInstalled = survey.GateInstalled;
            target.StileInstalled = survey.StileInstalled;
        }
    }
}
