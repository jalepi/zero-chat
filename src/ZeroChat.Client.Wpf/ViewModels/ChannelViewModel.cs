using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroChat.Client.Wpf.ViewModels;

public class ChannelViewModel
{
    private readonly ICollection<ChannelMessageViewModel> _messages;

    public ChannelViewModel(ICollection<ChannelMessageViewModel> messages)
    {
        _messages = messages;
    }

    public string ChannelId { get; init; } = "";
    public ICollection<ChannelMessageViewModel> Messages => _messages;
}
