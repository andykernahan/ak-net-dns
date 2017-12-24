// Copyright 2008 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using System.Net.Sockets;

namespace AK.Net.Dns.IO
{
    /// <summary>
    /// Extension class for <see cref="System.Net.Sockets.Socket"/> instances.
    /// This class is <see langword="static"/>.
    /// </summary>
    internal static class SocketExtensions
    {
        #region Public Interface.

        public static int SendTo(this Socket socket, ArraySegment<byte> buffer, EndPoint endpoint)
        {
            return socket.SendTo(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, endpoint);
        }

        public static int Send(this Socket socket, ArraySegment<byte> buffer)
        {
            return socket.Send(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None);
        }

        public static bool TryReceiveBuffer(this Socket socket, byte[] buffer)
        {
            int read;
            var remaining = buffer.Length;

            do
            {
                if ((read = socket.Receive(buffer, buffer.Length - remaining, remaining, SocketFlags.None)) == 0)
                {
                    return false;
                }
                remaining -= read;
            } while (remaining > 0);

            return true;
        }

        #endregion
    }
}
