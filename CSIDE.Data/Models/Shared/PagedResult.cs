namespace CSIDE.Data.Models.Shared;

/// <summary>
/// Generic paged result container that can be used across different entities
/// </summary>
/// <typeparam name="T">The type of items in the results</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The total number of results across all pages
    /// </summary>
    public int TotalResults { get; set; }
    
    /// <summary>
    /// The current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// The actual results for this page
    /// </summary>
    public ICollection<T> Results { get; set; } = [];
    
    /// <summary>
    /// Computed property: Total number of pages
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalResults / PageSize) : 0;
    
    /// <summary>
    /// Computed property: Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// Computed property: Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}