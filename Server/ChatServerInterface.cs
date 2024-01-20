using DC_Assignment1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [ServiceContract]
    public interface ChatServerInterface
    {
        [OperationContract]
        List<string> GetAllUsers();

        [OperationContract]
        int GetNumUsers();

        [OperationContract]
        string GetUserByName(string nameToSearch);

        [OperationContract]
        void AddNewUser(string newUser);

        [OperationContract]
        void RemoveUser(string user);

        [OperationContract]
        void AddMainUser(string user);

        [OperationContract]
        string GetMainUser();

        [OperationContract]
        string GetUserByIndex(int index);

        [OperationContract]
        void AddMessage(string userName, string inMessage);

        [OperationContract]
        List<string> GetAllMessages();

        [OperationContract]
        bool SendPrivateMessage(string senderUserName, string recipientUserName, string messageText);

        [OperationContract]
        List<string> GetPrivateMessages(string userName);

        [OperationContract]
        void ShareImage(string userName, string fileName, byte[] imageBytes);

        [OperationContract]
        void ShareTextFile(string userName, string fileName, string fileContent);

        [OperationContract]
        bool CreateChatRoom(string chatRoomName);

        [OperationContract]
        void AddUserToChatRoom(string userName, string chatRoomName);

        [OperationContract]
        void RemoveUserFromChatRoom(string userName, string chatRoomName);

        [OperationContract]
        void SendChatRoomMessage(string userName, string chatRoomName, string message);

        [OperationContract]
        List<string> GetChatRoomMessages(string chatRoomName);

        [OperationContract]
        List<string> GetAvailableChatRooms();



    }
}
