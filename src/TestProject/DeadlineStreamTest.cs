using System.Net;
using System.Net.Http.Headers;
using TqkLibrary.Http;
using TqkLibrary.Http.Extensions;
using TqkLibrary.Http.Streams;

namespace TestProject
{
    [TestClass]
    public class DeadlineStreamTest
    {
        /// <summary>Never completes a read until its token is cancelled, like a half-open connection.</summary>
        class HangingStream : MemoryStream
        {
            public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
                return 0;
            }
        }

        [TestMethod]
        public async Task ReadAsync_ClockExpires_ThrowsTimeoutException()
        {
            CancellationTokenSource clock = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
            using DeadlineStream stream = new DeadlineStream(new HangingStream(), clock);

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => stream.CopyToAsync(Stream.Null));
        }

        [TestMethod]
        public async Task ReadAsync_CallerCancels_ThrowsOperationCanceled()
        {
            using CancellationTokenSource caller = new CancellationTokenSource();
            CancellationTokenSource clock = CancellationTokenSource.CreateLinkedTokenSource(caller.Token);
            clock.CancelAfter(TimeSpan.FromMinutes(1));
            using DeadlineStream stream = new DeadlineStream(new HangingStream(), clock, caller.Token);

            Task copy = stream.CopyToAsync(Stream.Null);
            caller.Cancel();

            Exception ex = await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => copy);
            Assert.IsNotInstanceOfType(ex, typeof(TimeoutException));
        }

        [TestMethod]
        public void Read_AfterDeadline_ThrowsTimeoutException()
        {
            CancellationTokenSource clock = new CancellationTokenSource();
            clock.Cancel();
            using DeadlineStream stream = new DeadlineStream(new MemoryStream(new byte[] { 1 }), clock);

            Assert.ThrowsException<TimeoutException>(() => stream.Read(new byte[1], 0, 1));
        }

        [TestMethod]
        public async Task ReadAsync_BeforeDeadline_ReturnsData()
        {
            using DeadlineStream stream = new DeadlineStream(new MemoryStream(new byte[] { 1, 2, 3 }), new CancellationTokenSource(TimeSpan.FromMinutes(1)));
            using MemoryStream dest = new MemoryStream();

            await stream.CopyToAsync(dest);

            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, dest.ToArray());
        }
    }
}
