namespace inzBackend.Models.CatalogueModels
{
    public record ParsedCatalogueRowDto(
        DateOnly EntryDate,
        string UserRef,
        string EntryVal,
        string Translation,
        string ComputedKey,
        string CatalogueName
    );
}
