using CKMS.Contracts.Configuration;
using CKMS.Interfaces.Queue;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace CKMS.Library.Queue
{
    public class RabbitMQConnection : IRabbitMQ, IAsyncDisposable
    {
        private IChannel? _channel;
        private IConnection? _connection;
        private readonly ConnectionFactory _connnectionFactory;
        private readonly RabbitMqConfiguration _configuration;

        public RabbitMQConnection(IOptions<Application> options)
        {
            _configuration = options.Value.connection.rabbitMqConfiguration;
            _connnectionFactory = new ConnectionFactory()
            {
                UserName = _configuration.Username,
                Password = _configuration.Password,
                HostName = _configuration.HostName
            };
        }
        public async Task<IConnection?> GetConnectionAsync()
        {
            bool isConnected = await Connect();
            return _connection;
        }

        public async Task<IChannel?> GetChannelAsync()
        {
            bool isConnected = await Connect();
            return _channel;
        }
        public async Task<bool> Connect()
        {
            try
            {
                if (_connection is null || !_connection.IsOpen)
                {
                    _connection = await  _connnectionFactory.CreateConnectionAsync();
                    _connection.ConnectionShutdownAsync += Connection_ConnectionShutdown;

                    if (_channel is null || _channel.IsClosed)
                    {
                        _channel = await _connection.CreateChannelAsync();
                    }
                }
            }
            catch (BrokerUnreachableException e)
            {
            }

            return (_connection?.IsOpen ?? false) && (_channel?.IsOpen ?? false);
        }
        public async Task Reconnect()
        {

            await CloseConnectionAndChannel();
            var mres = new ManualResetEventSlim(false);
            while (!mres.Wait(3000))
            {
                try
                {
                    if (await Connect())
                    {
                        mres.Set();
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public async Task CloseConnectionAndChannel()
        {
            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    await _channel.CloseAsync();
                    _channel = null;
                }

                if (_connection != null && _connection.IsOpen)
                {
                    await _connection.CloseAsync();
                    _connection = null;
                }
            }
            catch (IOException ex)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }
        private async Task Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            await Reconnect();
        }


        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
            if (_connection != null)
            {
                _connection.ConnectionShutdownAsync -= Connection_ConnectionShutdown;
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
