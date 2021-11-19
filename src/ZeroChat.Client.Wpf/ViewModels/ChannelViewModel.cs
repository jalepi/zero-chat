using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroChat.Client.Commands;

namespace ZeroChat.Client.ViewModels;

public record ChannelViewModel(
    string ChannelId,
    ChannelComposeMessageCommand ComposeMessageCommand,
    ICollection<ChannelMessageViewModel> Messages) : BaseViewModel
{
    private string _composingText = "";
    public string ConsposingText
    {
        get => _composingText;
        set => SetProperty(ref _composingText, value);
    }
}
