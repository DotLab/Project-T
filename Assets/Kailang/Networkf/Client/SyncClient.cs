using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;

public static class SyncClient {
	public static Int64 Address { get; private set; }
	public static Int32 Port { get; private set; }

	public static Socket Client { get; private set; }

	public static bool IsRunning { get; private set; }

	public const int BufferSize = 1024;

	public static Queue<byte[]> MessageQueue { get; private set; }

	public static Thread MainThread { get; private set; }

	public static void Start (Int64 address, Int32 port) {
		MessageQueue = new Queue<byte[]>();

		Address = address;
		Port = port;

		MainThread = new Thread(() => {
			Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Client.Connect(new IPAddress(address), port);

			var buffer = new byte[BufferSize];

			IsRunning = true;
			while (IsRunning) {
				// Length
				int count = Client.Receive(buffer);
				if (count != 4)
					continue;

				int length = BitConverter.ToInt32(buffer, 0);
				if (length < 0)
					continue;

				// Message
				var message = new byte[length];

				int received = 0;
				while (received < length) {
					int recv = Client.Receive(message, received, length - received, SocketFlags.None);
					if (recv == 0)
						break;

					received += recv;
				}

				// Hash
				count = Client.Receive(buffer);
				if (count != 32)
					continue;

				var sha = SHA256Managed.Create();
				byte[] hash = sha.ComputeHash(message);
				for (int i = 0; i < 32; i++)
					if (hash[i] != buffer[i])
						continue;

				// Enqueue
				MessageQueue.Enqueue(message);
			}	
		});

		MainThread.Start();
	}

	public static void Stop () {
		IsRunning = false;

		var worker = new Thread(() => {
			MainThread.Abort();
			Client.Close();
		});

		worker.Start();
	}

	public static void Send (byte[] bytes) {
		var worker = new Thread(() => {
			Client.Send(bytes);
		});

		worker.Start();
	}
}
