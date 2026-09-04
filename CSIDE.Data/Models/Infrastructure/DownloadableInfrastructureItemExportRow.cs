using NodaTime;

namespace CSIDE.Data.Models.Infrastructure
{
    public sealed class DownloadableInfrastructureItemExportRow
    {
        public int Id { get; init; }
        public string? InfrastructureTypeName { get; init; }
        public bool InfrastructureTypeIsBridge { get; init; }
        public string? Description { get; init; }
        public LocalDate? InstallationDate { get; init; }
        public double? Height { get; init; }
        public double? Width { get; init; }
        public double? Length { get; init; }
        public double? Easting { get; init; }
        public double? Northing { get; init; }
        public string? RouteId { get; init; }
        public string? ParishName { get; init; }
        public string? MaintenanceTeamName { get; init; }
        public string? BeamMaterialName { get; init; }
        public bool? BeamMaterialIsWood { get; init; }
        public int? NumBeamTimbers { get; init; }
        public string? BeamTimbersSize { get; init; }
        public string? DeckingMaterialName { get; init; }
        public bool? DeckingMaterialIsWood { get; init; }
        public int? NumDeckingBoards { get; init; }
        public string? DeckingBoardsSize { get; init; }
        public decimal? DeckingBoardsLength { get; init; }
        public string? HandrailMaterialName { get; init; }
        public bool? HandrailMaterialIsWood { get; init; }
        public bool? HandrailsInPlace { get; init; }
        public string? HandrailTimbersSize { get; init; }
        public string? HandrailPostsMaterialName { get; init; }
        public bool? HandrailPostsMaterialIsWood { get; init; }
        public int? NumHandrailPostsTimbers { get; init; }
        public string? HandrailPostsTimbersSize { get; init; }
        public string? BankSeatMaterialName { get; init; }
        public bool? AntiSlipInstalled { get; init; }
        public bool? GateInstalled { get; init; }
        public bool? StileInstalled { get; init; }
        public bool? RampInstalled { get; init; }
        public bool? StepsInstalled { get; init; }
        public string? BeamConditionName { get; init; }
        public string? DeckingConditionName { get; init; }
        public string? HandrailConditionName { get; init; }
        public string? HandrailPostsConditionName { get; init; }
        public string? BankSeatConditionName { get; init; }
        public bool? Overgrown { get; init; }
        public bool? SignsOfBankErosion { get; init; }
        public bool? SeriouslyEroded { get; init; }
        public bool? HighUsage { get; init; }
        public bool? CoverBoardsInPlace { get; init; }
    }
}
