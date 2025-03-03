using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Components.Infrastructure
{
    public partial class InfrastructureMediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<InfrastructureMediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public InfrastructureItem? InfrastructureItem { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private IJSObjectReference? _jsModule;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Infrastructure/InfrastructureMediaList.razor.js");

            await _jsModule.InvokeVoidAsync("initGallery");

            await base.OnAfterRenderAsync(firstRender);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            try
            {
                if (_jsModule != null)
                {
                    await _jsModule.DisposeAsync();
                }
            }
            catch (JSDisconnectedException)
            {
                // Ignore as it doesn't matter
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        private async Task AddMediaToInfrastructure(List<Media> UploadedMedia)
        {
            if (InfrastructureItem is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    context.Attach(InfrastructureItem);
                    foreach (Media media in UploadedMedia)
                    {
                        InfrastructureItem.InfrastructureMedia.Add(new InfrastructureMedia
                        {
                            InfrastructureItemId = InfrastructureItem.Id,
                            Media = media
                        });
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error adding media to infrastructure item");
                }

            }
        }

        private async Task RefreshComponent()
        {
            if (InfrastructureItem is not null)
            {
                using var context = contextFactory.CreateDbContext();
                InfrastructureItem = await context.Infrastructure.FindAsync([InfrastructureItem.Id]);
                StateHasChanged();
            }
        }
    }
}
