using Microsoft.AspNetCore.SignalR;

namespace Township_API.Hubs
{
    public class CardPreviewHub : Hub
    {
        public async Task UpdatePreview(PreviewCardDto card)
        {
            await Clients.Others.SendAsync("ReceivePreview", card);
        }

        public async Task UpdateLoading()
        {
            await Clients.Others.SendAsync("ReceiveLoading");
        }

        public async Task ClosePreview()
        {
            await Clients.Others.SendAsync("ReceiveClose");
        }
    }
    public class PreviewCardDto
    {
        public string? CardImageBase64 { get; set; }
        public string? CardName { get; set; }
    }
}