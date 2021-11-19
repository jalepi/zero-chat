namespace ZeroChat.Client.ViewModels;

public class ApplicationViewModel
{
    private readonly ICollection<ChannelViewModel> _channels;

    public ApplicationViewModel(ICollection<ChannelViewModel> channels)
    {
        _channels = channels;
    }

    public ICollection<ChannelViewModel> Channels => _channels;
}
