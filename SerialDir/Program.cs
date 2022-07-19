using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Loader;
using SerialDir.SerialDir;

namespace SerialDir {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("SerialDir V0.3.0");

            string argPrimary = "";
            string argSecondary = "";

            int mode = 0;

            if (args.Length <= 1) {
                Console.WriteLine("Usage: SerialDir {-c / -s} [COM Port / Socket Address] [Baudrate / Socket Port]");
                return;
            }

            int argNum = 0;
            foreach (string arg in args) {
                if (arg.Equals("-c")) mode = 0;
                else if (arg.Equals("-s")) mode = 1;
                else {
                    if (argNum == 0) {
                        argPrimary = arg;
                        argNum++;
                    } else if (argNum == 1) {
                        argSecondary = arg;
                        argNum++;
                    } else {
                        Console.WriteLine("Too Many Arguments!");
                        return;
                    }
                }
            }

            // Lazy mode switch, will make it better sometime
            if (mode == 0) {
                // Mode 0 = Serial Connection
                int baudRate = 0;

                try {
                    baudRate = Int32.Parse(argSecondary);
                } catch (FormatException) {
                    Console.WriteLine("Bad Baudrate Format!");
                    return;
                }

                SerialDirStateMachine stateMachine = new SerialDirStateMachine();

                SerialPort serialPort = new SerialPort(argPrimary, baudRate, Parity.None, 8, StopBits.One);
                serialPort.Handshake = Handshake.None;
                serialPort.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");

                serialPort.WriteTimeout = 1000;

                try {
                    serialPort.Open();
                } catch (Exception) {
                    Console.WriteLine("Cannot Open Serial Port!");
                    return;
                }

                Console.WriteLine("Opened Serial Port " + argPrimary + " At " + baudRate + " Baud");

                while (true) {
                    int readChar = serialPort.ReadChar();

                    byte b = Convert.ToByte(readChar);

                    List<byte> response = stateMachine.ReceiveByte(b);

                    if (response.Count > 0) {

                        serialPort.Write(response.ToArray(), 0, response.Count);
                    }
                }
            } else if (mode == 1) {
                // Mode 1 = TCP Connection
                int tcpPort = 0;

                try {
                    tcpPort = Int32.Parse(argSecondary);
                } catch (FormatException) {
                    Console.WriteLine("Bad Port Format!");
                    return;
                }

                
                SerialDirStateMachine stateMachine = new SerialDirStateMachine();

                IPAddress ipAddress = IPAddress.Parse(argPrimary);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, tcpPort);
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try {
                    socket.Connect(ipEndPoint);
                } catch (Exception) {
                    Console.WriteLine("Cannot Open Socket Connection!");
                    return;
                }
                
                Console.WriteLine("Opened TCP Connection To " + argPrimary + " On Port " + tcpPort);

                byte[] bytes = new byte[1024];
                while (true) {
                    int received = socket.Receive(bytes);

                    for (int i = 0; i < received; i++) {
                        List<byte> response = stateMachine.ReceiveByte(bytes[i]);

                        if (response.Count > 0) {
                            socket.Send(response.ToArray());
                        }
                    }
                }
            } else {
                Console.WriteLine("Unknown Mode?");
            }
        }
    }
}
