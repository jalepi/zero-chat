using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;
using ZeroChat.Shared.Protocols;

namespace ZeroChat.Shared.Application.Tests;

[Collection("EndToEnd")]
public class EndToEndTests : IDisposable
{
    private readonly CancellationTokenSource cts = new();

    void IDisposable.Dispose()
    {
        cts.Cancel();
        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData("hello world")]
    public async Task RequestResponse_Test(string givenPayload)
    {
        // arrange
        var options = new BoundedChannelOptions(100);

        var channel = Channel.CreateBounded<RequestCall>(options);
        var backgroundService = new BackgroundService();

        var requestRunner = new RequestRunner(ConnectionString: "inproc://request");
        var requestOptions = new RequestOptions(ReceiveAsync: channel.Reader.ReadAsync);

        var responseRunner = new ResponseRunner(ConnectionString: "@inproc://request");
        var responseOptions = new ResponseOptions(HandleAsync: (request, ct) =>
        {
            var payload = new string(request.Payload);
            var response = new Response(payload);
            return ValueTask.FromResult(response);
        });

        Response? actualResponse = default;
        var requestCall = new RequestCall(
            Request: new Request(Topic: "SomeTopic", Payload: givenPayload),
            Callback: response => actualResponse = response);

        // act
        backgroundService.Start(requestRunner, requestOptions, cts.Token);
        backgroundService.Start(responseRunner, responseOptions, cts.Token);
        await Task.Delay(1000);

        await channel.Writer.WriteAsync(requestCall, cts.Token);
        await Task.Delay(1000);

        // assert
        Assert.NotNull(actualResponse!);

        Assert.Equal(
            expected: givenPayload,
            actual: actualResponse!.Payload);
    }

    [Theory]
    [InlineData("topic", "message", "topic", true)]
    [InlineData("topicA", "message", "topicB", false)]
    public async void PublishSubscribe_Test(string publishTopic, string publishPayload, string subscribeTopic, bool received)
    {
        // arrange
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        var channel = Channel.CreateBounded<Message>(options);

        var publisherRunner = new PublisherRunner(ConnectionString: "@inproc://message");
        var publisherOptions = new PublisherOptions(
            ReceiveAsync: channel.Reader.ReadAsync);

        Message? subscribeMessage = default;
        var subscriberRunner = new SubscriberRunner(ConnectionString: "inproc://message");
        var subscriberOptions = new SubscriberOptions(
            Topic: subscribeTopic,
            SendAsync: (message, ct) =>
            {
                subscribeMessage = message;
                return ValueTask.CompletedTask;
            });

        var backgroundService = new BackgroundService();
        var publishMessage = new Message(Topic: publishTopic, Payload: publishPayload);

        // act
        backgroundService.Start(publisherRunner, publisherOptions, cts.Token);
        await Task.Delay(500);

        backgroundService.Start(subscriberRunner, subscriberOptions, cts.Token);
        await Task.Delay(500);

        await channel.Writer.WriteAsync(publishMessage, cts.Token);
        await Task.Delay(500);

        // assert
        if (received)
        {
            Assert.NotNull(subscribeMessage!);
            Assert.Equal(
                expected: publishMessage.Payload,
                actual: subscribeMessage!.Payload);
        }
        else
        {
            Assert.Null(subscribeMessage);
        }
    }
}
