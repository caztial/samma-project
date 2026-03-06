import * as signalR from '@microsoft/signalr';
import { SIGNALR_HUB_URL } from '../config';

/**
 * Connection states for SignalR hub.
 */
export const ConnectionState = {
  CONNECTING: 'connecting',
  CONNECTED: 'connected',
  DISCONNECTED: 'disconnected',
  RECONNECTING: 'reconnecting',
};

/**
 * Creates a SignalR hub connection for session real-time communication.
 *
 * @param {Object} options - Configuration options
 * @param {function} [options.getToken] - Function that returns the current auth token
 * @param {function} [options.onConnected] - Callback when connection is established
 * @param {function} [options.onDisconnected] - Callback when connection is lost
 * @param {function} [options.onReconnecting] - Callback when connection is reconnecting
 * @param {function} [options.onReconnected] - Callback when connection is re-established
 * @returns {Object} SignalR service with connection management methods
 */
export function createSignalRService({
  getToken,
  onConnected,
  onDisconnected,
  onReconnecting,
  onReconnected,
} = {}) {
  /** @type {signalR.HubConnection | null} */
  let connection = null;

  /** @type {ConnectionState} */
  let connectionState = ConnectionState.DISCONNECTED;

  /**
   * Creates a new HubConnection instance with authentication.
   */
  const createConnection = () => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(SIGNALR_HUB_URL, {
        accessTokenFactory: () => getToken?.() ?? '',
        withCredentials: true,
        skipNegotiation: false,
        transport: signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.WebSockets, // Allow both transports
      })
      .withKeepAliveInterval(30000) // 30 seconds
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    // Set up event handlers
    newConnection.onclose((error) => {
      connectionState = ConnectionState.DISCONNECTED;
      onDisconnected?.(error);
    });

    newConnection.onreconnecting((error) => {
      connectionState = ConnectionState.RECONNECTING;
      onReconnecting?.(error);
    });

    newConnection.onreconnected((connectionId) => {
      connectionState = ConnectionState.CONNECTED;
      onReconnected?.(connectionId);
    });

    return newConnection;
  };

  /**
   * Starts the SignalR connection.
   * @returns {Promise<void>}
   */
  const connect = async () => {
    if (connection && connection.state !== signalR.HubConnectionState.Disconnected) {
      return;
    }

    connectionState = ConnectionState.CONNECTING;

    try {
      connection = createConnection();
      await connection.start();
      connectionState = ConnectionState.CONNECTED;
      onConnected?.();
    } catch (error) {
      connectionState = ConnectionState.DISCONNECTED;
      throw error;
    }
  };

  /**
   * Stops the SignalR connection.
   * @returns {Promise<void>}
   */
  const disconnect = async () => {
    if (connection) {
      try {
        await connection.stop();
      } catch (error) {
        // Ignore errors on disconnect
      }
      connection = null;
      connectionState = ConnectionState.DISCONNECTED;
    }
  };

  /**
   * Joins a session group to receive session-specific events.
   * @param {string} sessionCode - The session code to join
   * @returns {Promise<void>}
   */
  const joinSessionGroup = async (sessionCode) => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('SignalR connection is not established');
    }
    await connection.invoke('JoinSessionGroup', sessionCode);
  };

  /**
   * Leaves a session group.
   * @param {string} sessionCode - The session code to leave
   * @returns {Promise<void>}
   */
  const leaveSessionGroup = async (sessionCode) => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
      return; // Already disconnected, nothing to leave
    }
    await connection.invoke('LeaveSessionGroup', sessionCode);
  };

  /**
   * Registers a handler for a specific server-to-client event.
   * @param {string} eventName - The name of the event
   * @param {function} handler - The handler function
   */
  const on = (eventName, handler) => {
    connection?.on(eventName, handler);
  };

  /**
   * Removes a handler for a specific server-to-client event.
   * @param {string} eventName - The name of the event
   * @param {function} handler - The handler function to remove
   */
  const off = (eventName, handler) => {
    connection?.off(eventName, handler);
  };

  /**
   * Gets the current connection state.
   * @returns {ConnectionState}
   */
  const getConnectionState = () => connectionState;

  /**
   * Gets the underlying HubConnection instance (for advanced use).
   * @returns {signalR.HubConnection | null}
   */
  const getConnection = () => connection;

  return {
    connect,
    disconnect,
    joinSessionGroup,
    leaveSessionGroup,
    on,
    off,
    getConnectionState,
    getConnection,
  };
}

export default createSignalRService;