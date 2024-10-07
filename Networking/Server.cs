using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.Json;
using OnwardAPIVRML;

namespace OnwardAPIVRML.Networking {
    public class Server {
        private string API_URL = $"http://localhost:7821/";

        private readonly HttpListener listener;
        private Thread serverThread;

        public Server() {
            listener = new HttpListener();
            serverThread = null;
        }

        public void CreateAndStartServer() {
            listener.Prefixes.Add(API_URL);
            listener.Start();

            Plugin.Log.LogInfo($"Listener started. Waiting for requests on {API_URL}");

            serverThread = new Thread(() => {
                while (true) {
                    HttpListenerContext context = listener.GetContext();

                    ThreadPool.QueueUserWorkItem((state) => {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        if (request.HttpMethod == "GET") {
                            response.StatusCode = 200;
                            response.ContentType = "application/json";
                            
                            response.Headers.Add("Access-Control-Allow-Origin", "*");
                            response.Headers.Add("Access-Control-Allow-Methods", "GET");
                            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                            
                            string json = JsonSerializer.Serialize(Plugin.RootDTO);
                            byte[] buffer = Encoding.UTF8.GetBytes(json);
                            System.IO.Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();
                        }
                    });
                }
            });

            serverThread.Start();
            Plugin.Log.LogInfo("Server started");
        }

        public void CloseServer() {
            listener.Stop();
            
            if(serverThread != null) {
                serverThread.Join();
            } else {
                Plugin.Log.LogInfo("No threaded server running");
            }
            
            Plugin.Log.LogInfo("Server stopped");
        }
    }
}