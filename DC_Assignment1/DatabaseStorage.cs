using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DC_Assignment1
{
    public class DatabaseStorage
    {
        List<DataStruct> dataStruct;
        List<string> messages;

        string mainUser;
        int userCount;

        private Dictionary<string, List<SharedFile>> sharedFiles = new Dictionary<string, List<SharedFile>>();

        public DatabaseStorage() 
        {
            dataStruct = new List<DataStruct>();
            messages = new List<string>();
            mainUser = "";
            userCount = 0;
        }

        public void AddMainUser(string user)
        {
            this.mainUser = user;
        }

        public string GetMainUser()
        {
            return this.mainUser;
        }

        public void AddNewUser(string newUser)
        {
            DataStruct ds = new DataStruct(newUser, userCount);
            userCount++;
           
            dataStruct.Add(ds);

            string message = newUser + " joined ";

            messages.Add(message);
        }

        public void RemoveUser(string userToRemove)
        {
            string tempResult = "";

            for (int i = 1; i <= dataStruct.Count; i++)
            {
                tempResult = GetUserByIndex(i);

                if (tempResult.Equals(userToRemove))
                {
                    dataStruct.Remove(dataStruct[i - 1]);
                }
            }

            string message = userToRemove + " left ";

            messages.Add(message);
        }


        public void AddMessage(string userName, string inMessage)
        {
            string message = userName + " sent: " + inMessage;

            messages.Add(message);
        }

        public List<string> GetAllMessages()
        {
            return messages;
        }
        public string GetUserByName(string nameToSearch)
        {
            string result = "User not found!";
            string tempResult = "";

            // Change the loop to start from 0 and end at dataStruct.Count - 1
            for (int i = 0; i < dataStruct.Count; i++)
            {
                tempResult = GetUserByIndex(i);

                if (tempResult.Equals(nameToSearch))
                {
                    result = tempResult;
                }
            }

            return result;
        }



        public int GetNumOfClients()
        {
            return dataStruct.Count;
        }
        public string GetUserByIndex(int index)
        {
            return dataStruct[index].userName;
        }

        public void ShareImage(string userName, string fileName, Bitmap image)
        {
            ShareFile(userName, fileName, BitmapToByteArray(image));
        }

        public void ShareTextFile(string userName, string fileName, string content)
        {
            ShareFile(userName, fileName, content);
        }

        public List<Bitmap> GetSharedImages(string userName)
        {
            if (sharedFiles.ContainsKey(userName))
            {
                return sharedFiles[userName]
                    .Where(file => file is ImageFile)
                    .Select(file => ByteArrayToBitmap((byte[])file.Content))
                    .ToList();
            }
            return new List<Bitmap>();
        }

        public List<string> GetSharedTextFiles(string userName)
        {
            if (sharedFiles.ContainsKey(userName))
            {
                return sharedFiles[userName]
                    .Where(file => file is TextFile)
                    .Select(file => file.Content.ToString())
                    .ToList();
            }
            return new List<string>();
        }

        private void ShareFile(string userName, string fileName, object content)
        {
            if (!sharedFiles.ContainsKey(userName))
            {
                sharedFiles[userName] = new List<SharedFile>();
            }

            if (content is byte[])
            {
                sharedFiles[userName].Add(new ImageFile(fileName, content as byte[]));
            }
            else if (content is string)
            {
                sharedFiles[userName].Add(new TextFile(fileName, content.ToString()));
            }
        }

        private byte[] BitmapToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png); // You can change the format as needed
                return stream.ToArray();
            }
        }

        private Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                return new Bitmap(stream);
            }
        }

      
        // Add a dictionary to store private messages for each user.
        private Dictionary<string, List<string>> privateMessages = new Dictionary<string, List<string>>();

        public bool SendPrivateMessage(string senderUserName, string recipientUserName, string messageText)
        {
            // Validate sender and recipient existence.
            if (!dataStruct.Any(user => user.userName == senderUserName))
            {
                // Log error and return false.
                Console.WriteLine("Sender doesn't exist");
                return false;
            }

            if (!dataStruct.Any(user => user.userName == recipientUserName))
            {
                // Log error and return false.
                Console.WriteLine("Recipient doesn't exist");
                return false;
            }

            // Create a private message.
            string privateMessage = $"{senderUserName} to {recipientUserName}: {messageText}";

            // Store the private message in the recipient's private message list.
            if (!privateMessages.ContainsKey(recipientUserName))
            {
                privateMessages[recipientUserName] = new List<string>();
            }
            privateMessages[recipientUserName].Add(privateMessage);

            // Optionally, store the sent message in the sender's message list.
            if (!privateMessages.ContainsKey(senderUserName))
            {
                privateMessages[senderUserName] = new List<string>();
            }
            privateMessages[senderUserName].Add(privateMessage);

            // Return true to indicate that the message was sent successfully.
            return true;
        }


        public List<string> GetPrivateMessages(string userName)
        {
            // Return the private messages for the user.
            if (privateMessages.ContainsKey(userName))
            {
                return privateMessages[userName];
            }
            return new List<string>(); // No private messages for the user.
        }
        public class ChatMessage
        {
            public string UserName { get; set; }
            public string Message { get; set; }
        }
        public class ChatRoom
        {
            public string Name { get; }
            public List<string> Users { get; }
            public List<ChatMessage> Messages { get; }

            public ChatRoom(string name)
            {
                Name = name;
                Users = new List<string>();
                Messages = new List<ChatMessage>();
            }

            public void AddUser(string userName)
            {
                Users.Add(userName);
            }

            public void RemoveUser(string userName)
            {
                Users.Remove(userName);
            }

            public void AddMessage(string userName, string message)
            {
                Messages.Add(new ChatMessage { UserName = userName, Message = message });
            }
        }

        private Dictionary<string, ChatRoom> chatRooms = new Dictionary<string, ChatRoom>();

        public bool CreateChatRoom(string chatRoomName)
        {
            if (!chatRooms.ContainsKey(chatRoomName))
            {
                chatRooms[chatRoomName] = new ChatRoom(chatRoomName);
                return true;
            }
            return false;
        }

        public void AddUserToChatRoom(string userName, string chatRoomName)
        {
            if (chatRooms.ContainsKey(chatRoomName))
            {
                chatRooms[chatRoomName].AddUser(userName);
            }
        }

        public void RemoveUserFromChatRoom(string userName, string chatRoomName)
        {
            if (chatRooms.ContainsKey(chatRoomName))
            {
                chatRooms[chatRoomName].RemoveUser(userName);
            }
        }

        public void SendChatRoomMessage(string userName, string chatRoomName, string message)
        {
            if (chatRooms.ContainsKey(chatRoomName))
            {
                chatRooms[chatRoomName].AddMessage(userName, message);
            }
        }


        public List<string> GetChatRoomMessages(string chatRoomName)
        {
            if (chatRooms.ContainsKey(chatRoomName))
            {
                return chatRooms[chatRoomName].Messages
                    .Select(m => $"{m.UserName}: {m.Message}")
                    .ToList();
            }
            return new List<string>(); // Return an empty list
        }


        public List<string> GetAvailableChatRooms()
        {
            return chatRooms.Keys.ToList();
        }





    }


}
