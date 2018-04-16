using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace TelnetNASAtest
{
	class Program
	{
		static void Main()
		{
			JplConnect jplConnect = new JplConnect();
			
			jplConnect.ServerSocket("horizons.jpl.nasa.gov", 6775);
		}
	}

	public class JplConnect
	{
		private TcpClient client;
		private Thread readWriteThread;
		private NetworkStream networkStream;
		private DateTime today; 
		private DateTime tomorrow;
		private double[] bodyChar = new double[6];

		public void ServerSocket(string ip, int port)
		{
			try
			{
				client = new TcpClient(ip, port);
				Console.WriteLine("Connected to server.");
			}
			catch (SocketException)
			{
				Console.WriteLine("Failed to connect to server");
				return;
			}

			//Assign networkstream
			networkStream = client.GetStream();

			//start socket read/write thread
			readWriteThread = new Thread(readWrite);
			readWriteThread.Start();
			
		}

		private void readWrite()
		{
			string command, recieved;
			bool title = false;

			today = DateTime.Now;
			tomorrow = today.AddDays(1);


			//Read first thing given
			recieved = read();
			Console.WriteLine(recieved);

			//Set up connection loop
			while (true)
			{
				Console.Write("Response: ");

				if (!title)
				{
					command = "";
				}
				else
				{
					accessBody("499");
					command = Console.ReadLine();
					command = Console.ReadLine();
				}
				if (recieved == "\r\nHorizons> ")
				{
					title = true;
				}

				if (command == "exit")
					break;

				write(command);
				Thread.Sleep(500);
				recieved = read();

				Console.WriteLine(recieved);
			}

			Console.WriteLine("Disconnected from server");
			networkStream.Close();
			client.Close();
		}


		/// <summary>
		/// Accesses orbital body through horizons and extracts ephemeris into vars and returns those vars into a double[]
		/// </summary>
		/// <param name="id"></param>
		public OribtalBody accessBody(string id)
		{
			string[] _commands = new string[] { id, "e", "v", "500", "y", "eclip", today.ToString(), tomorrow.ToString(), "1d", "y", "1", "n" };
			StringBuilder sb = new StringBuilder();
			string m_stringholder;
			double[] m_bodyChar = new double[6];
			double body_x = 0;
			double body_y = 0;
			double body_z = 0;


			//Send body ID
			write(id);

			//Send Command loop
			for (int i = 1; i < _commands.Length; i++)
			{
				while (true)
				{
					if (networkStream.DataAvailable)
						break;
				}
				write(_commands[i]);
				Thread.Sleep(50);
				sb.Append(read());
			}
			sb.Append(read());
			m_stringholder = sb.ToString();
			
			//Get empheris
			//$$SOE Start of ephemeris
			int startPos = m_stringholder.LastIndexOf("$$SOE") + "$$SOE".Length + 1;
			//$$EOE End of ephemeris
			int length = m_stringholder.IndexOf("$$EOE") - startPos;
			string sub = m_stringholder.Substring(startPos, length);

			//Split into only today
			m_stringholder = sub.Substring(0, sub.IndexOf("\r\n"));

			//Fixa string ^ den innehåller skit som inte ska med vilket gör exception ner

			//Split into Vars
			string[] splitString = m_stringholder.Split(' ');
			for (int i = 0; i < 6; i++)
			{
				m_bodyChar[i] = double.Parse(splitString[i]);
			}

			

			OribtalBody oribtalBody = new OribtalBody(id, body_x, body_y, body_z);

			Console.WriteLine(sub);

			return oribtalBody;
			
		}

		/// <summary>
		/// Converts message string into bytes and then sends them to JPL
		/// </summary>
		/// <param name="message"></param>
		public void write(string message)
		{
			message += Environment.NewLine;
			byte[] messageBytes = Encoding.ASCII.GetBytes(message);
			networkStream.Write(messageBytes, 0, messageBytes.Length);
			networkStream.Flush();
		}


		/// <summary>
		/// Reads network stream, has a buffer of 1024 bytes but reads all data through networkStream.DataAvailable
		/// </summary>
		/// <returns></returns>
		public string read()
		{
			byte[] data = new byte[1024];
			StringBuilder recieved = new StringBuilder();

			int numberOfBytesRead = 0;

			// Incoming message may be larger than the buffer size, therefore reads all available data through this loop before stitching it togheter. 
			do
			{
				numberOfBytesRead = networkStream.Read(data, 0, data.Length);

				recieved.AppendFormat("{0}", Encoding.ASCII.GetString(data, 0, numberOfBytesRead));

			}
			while (networkStream.DataAvailable);


			return recieved.ToString();
		}

	}
}
