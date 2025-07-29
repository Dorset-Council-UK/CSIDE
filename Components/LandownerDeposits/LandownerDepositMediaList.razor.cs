using CSIDE.Data;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Components.LandownerDeposits
{
    public partial class LandownerDepositMediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<LandownerDepositMediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public LandownerDeposit? LandownerDeposit { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private LandownerDepositMediaType[]? MediaTypes;
        private IJSObjectReference? _jsModule;

        protected override async Task OnInitializedAsync()
        {
            using var context = contextFactory.CreateDbContext();
            MediaTypes = context.LandownerDepositMediaTypes.AsNoTracking().OrderBy(p => p.Id).ToArray();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/LandownerDeposits/LandownerDepositMediaList.razor.js");

            await _jsModule.InvokeVoidAsync("initGallery");

            await base.OnAfterRenderAsync(firstRender);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_jsModule is not null)
            {
                await _jsModule.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }

        private async Task AddMediaToLandownerDeposit(List<Media> UploadedMedia, LandownerDepositMediaType mediaType)
        {
            if (LandownerDeposit is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    context.Attach(LandownerDeposit);
                    foreach (Media media in UploadedMedia)
                    {
                        LandownerDeposit.LandownerDepositMedia.Add(new LandownerDepositMedia
                        {
                            LandownerDepositId = LandownerDeposit.Id,
                            LandownerDepositSecondaryId = LandownerDeposit.SecondaryId,
                            Media = media,
                            MediaTypeId = mediaType.Id,
                            MediaType = mediaType,
                        });
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error adding media to landowner deposit");
                }
            }
        }

        private async Task RefreshComponent()
        {
            if (LandownerDeposit is not null)
            {
                using var context = contextFactory.CreateDbContext();
                LandownerDeposit = await context.LandownerDeposits.FindAsync([LandownerDeposit.Id, LandownerDeposit.SecondaryId]);
                StateHasChanged();
            }
        }
    }
}
