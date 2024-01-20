using DC_Assignment1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class ChatServer : ChatServerInterface
    {
        DatabaseStorage d;
        private CancellationTokenSource cancellationSource;


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the chat server");

            ServiceHost host;

            NetTcpBinding tcp = new NetTcpBinding();

            host = new ServiceHost(typeof(ChatServer));


            host.AddServiceEndpoint(typeof(ChatServerInterface), tcp, "net.tcp://0.0.0.0:8100/ChatService");

            host.Open();

            Console.WriteLine("System Online");

            Console.ReadLine();

            host.Close();
        }
        public ChatServer()
        {
            // Initialize any necessary resources here
            if (d == null)
            {
                d = new DatabaseStorage();
            }
            cancellationSource = new CancellationTokenSource();
        }

        public ChatServer(string userName, string chatRoomName)
        {
            if (d == null)
            {
                d = new DatabaseStorage();
            }
            cancellationSource = new CancellationTokenSource();


           // Task.Run(() => ReceiveMessages(userName));
          //  Task.Run(() => GetChatRoomMes(chatRoomName));


        }

        public class ChatMessage
        {
            public string UserName { get; set; }
            public string Message { get; set; }
        }

        public void AddNewUser(string newUser)
        {
            d.AddNewUser(newUser);
        }

        public void RemoveUser(string user)
        {
            d.RemoveUser(user);
        }

        public void AddMessage(string userName, string inMessage)
        {
            d.AddMessage(userName, inMessage);
        }

        public List<string> GetAllMessages()
        {
            return d.GetAllMessages();
        }

        public List<string> GetAllUsers()
        {
            int i = d.GetNumOfClients();

            List<string> resultList = new List<string>();

            for (int j = 1; j <= i; j++)
            {
                resultList.Add(d.GetUserByIndex(j));
            }

            return resultList;
        }

        public int GetNumUsers()
        {
            return d.GetNumOfClients();
        }

        public string GetUserByName(string nameToSearch)
        {
            return d.GetUserByName(nameToSearch);
        }

        public string GetUserByIndex(int index)
        {
            return d.GetUserByIndex(index);
        }

        public void AddMainUser(string user)
        {
            d.AddMainUser(user);
            d.AddNewUser(user);
        }

        public string GetMainUser()
        {
            return d.GetMainUser();
        }

        public bool SendPrivateMessage(string senderUserName, string recipientUserName, string messageText)
        {
            // Check if the sender exists in the chat server.
            if (!d.GetUserByName(senderUserName).Equals("User not found!"))
            {
                // Call the corresponding method in DatabaseStorage to send the private message.
                return d.SendPrivateMessage(senderUserName, recipientUserName, messageText);
            }
            else
            {
                Console.WriteLine("User doesn't exist");
                return false;
            }


        }


        public List<string> GetPrivateMessages(string userName)
        {
            return d.GetPrivateMessages(userName);
        }


        

        public void ShareImage(string userName, string fileName, byte[] imageBytes)
        {
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                using (Bitmap image = new Bitmap(stream))
                {
                    d.ShareImage(userName, fileName, image);
                }
            }
        }

        public void ShareTextFile(string userName, string fileName, string content)
        {
            d.ShareTextFile(userName, fileName, content);
        }

        public List<Bitmap> GetSharedImages(string userName)
        {
            return d.GetSharedImages(userName);
        }

        public List<string> GetSharedTextFiles(string userName)
        {
            return d.GetSharedTextFiles(userName);
        }

        public bool CreateChatRoom(string chatRoomName)
        {
            return d.CreateChatRoom(chatRoomName);
        }

        public void AddUserToChatRoom(string userName, string chatRoomName)
        {
            d.AddUserToChatRoom(userName, chatRoomName);
        }

        public void RemoveUserFromChatRoom(string userName, string chatRoomName)
        {
            d.RemoveUserFromChatRoom(userName, chatRoomName);
        }

        public void SendChatRoomMessage(string userName, string chatRoomName, string message)
        {
            d.SendChatRoomMessage(userName, chatRoomName, message);
        }

        public List<string> GetChatRoomMessages(string chatRoomName)
        {
            return d.GetChatRoomMessages(chatRoomName);

        }

        

        public List<string> GetAvailableChatRooms() 
        {
            return d.GetAvailableChatRooms();
        }


        private async Task ReceiveMessages(string userName)
        {
            while (true)
            {
                List<string> privateMessages = d.GetPrivateMessages(userName);
                foreach (string message in privateMessages)
                {
                    Console.WriteLine(message);
                }


                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }



        private async Task GetChatRoomMes(string chatRoomName)
        {
            while (true)
            {
                List<string> cMessages = d.GetChatRoomMessages(chatRoomName);
                foreach (string message in cMessages)
                {
                    Console.WriteLine(message);
                }


                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }


    }
}

