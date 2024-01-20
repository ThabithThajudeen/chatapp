
using Microsoft.Win32;
using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatServerInterface foob;
        private string mainUser;
        private int numUsersDisplaying;
       // private string selectedChatRoom;


        public MainWindow()
        {
            InitializeComponent();

            mainUser = "";
            numUsersDisplaying = 0;

            ChannelFactory<ChatServerInterface> foobFactory;

            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8100/ChatService";

            foobFactory = new ChannelFactory<ChatServerInterface>(tcp, URL);

            foob = foobFactory.CreateChannel();

            chatList.Visibility = Visibility.Hidden;
            chatFeed.Visibility = Visibility.Hidden;
            chatInput.Visibility = Visibility.Hidden;
            sendButton.Visibility = Visibility.Hidden;
            refreshButton.Visibility = Visibility.Hidden;
            privateMessageButton.Visibility = Visibility.Hidden;
            chatRoomButton.Visibility = Visibility.Hidden;

            StartUpdatingChatRoomsAndUsers();
            StartReceivingMessages();


        }




        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png;*.gif;*.bmp"; // Filter for image files
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                imagePathText.Text = "Selected Image Path: " + selectedImagePath;
                imagePathText.Visibility = Visibility.Visible;
            }
        }

        // Event handler for choosing a text file
        private void ChooseTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files|*.txt"; // Filter for text files
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedTextFilePath = openFileDialog.FileName;
                textFilePathText.Text = "Selected Text File Path: " + selectedTextFilePath;
                textFilePathText.Visibility = Visibility.Visible;
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            AddMainUser(loginInput.Text);

            foob.AddNewUser(loginInput.Text);

            var newBorder = new Border() { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black };
            var newUserDisplayBox = new TextBlock() { Name = "userDisplayBox" + numUsersDisplaying.ToString(), Text = foob.GetUserByIndex(numUsersDisplaying), Height = 27, Padding = new Thickness(5, 5, 0, 0) };

            newBorder.Child = newUserDisplayBox;
            this.chatList.Children.Add(newBorder);

            numUsersDisplaying++;

            for(int i = numUsersDisplaying; i < foob.GetNumUsers(); i++)
            {
                 var tempBorder = new Border() { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black };
                 var tempUserDisplayBox = new TextBlock() { Name = "userDisplayBox" + i.ToString(), Text = foob.GetUserByIndex(i), Height = 27, Padding = new Thickness(5,5,0,0)};
                        
                 tempBorder.Child = tempUserDisplayBox;
                 this.chatList.Children.Add(tempBorder);

                 numUsersDisplaying++;
            }
            
            loginButton.Visibility = Visibility.Hidden;
            loginInput.Visibility = Visibility.Hidden;

            chatList.Visibility = Visibility.Visible;
            
            chatFeed.Visibility = Visibility.Visible;
            chatInput.Visibility = Visibility.Visible;
            sendButton.Visibility = Visibility.Visible;
            refreshButton.Visibility = Visibility.Visible;
            privateMessageButton.Visibility = Visibility.Visible;
            chatRoomButton.Visibility = Visibility.Visible;
        }

       

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageToSend = chatInput.Text.ToString();
            string userSendingMessage = GetMainUser();

            foob.AddMessage(userSendingMessage, messageToSend);
        }

        private void user_click(object sender, RoutedEventArgs e)
        {
           
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> messages = new List<string>();

            if(foob.GetNumUsers() > numUsersDisplaying) 
            {
                for (int i = numUsersDisplaying; i < foob.GetNumUsers(); i++)
                {
                    var newBorder = new Border() { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black };
                    Button newUserDisplayButton = new Button() { Name = "userDisplayButton" + i.ToString(), Content = foob.GetUserByIndex(i), Height = 27, Padding = new Thickness(5, 5, 0, 0)};
                    newUserDisplayButton.Click += user_click;
                    newBorder.Child = newUserDisplayButton;
                    this.chatList.Children.Add(newBorder);

                    numUsersDisplaying++;
                }
            }
            

            messages = foob.GetAllMessages();

            string allMessages = "";

            for(int i = 0; i < messages.Count; i++)
            {
                allMessages += "\n" + messages[i].ToString();
            }

            chatFeed.Text = allMessages;
        }

        private void AddMainUser(string username)
        {
            mainUser = username;
        }

        private string GetMainUser()
        {
            return this.mainUser;
        }

//PRIVATE MESSAGING
        // Event Handler for privateSendButton
        private void PrivateSendButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve recipient's name and message from TextBoxes
            string recipient = privateRecipientInput.Text;
            string message = privateChatInput.Text;

            // Check if recipient and message are not empty
            if (string.IsNullOrWhiteSpace(recipient) || string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Recipient and message cannot be empty!");
                return;
            }

            // Send the private message using the ChatServerInterface
            var success = foob.SendPrivateMessage(mainUser, recipient, message);
            if (!success)
            {
                MessageBox.Show("Recipient Not Found!");

            }
           

            // Clear the input TextBoxes
            privateRecipientInput.Clear();
            privateChatInput.Clear();

            // Optionally, you might want to refresh the private message feed
            RefreshPrivateMessages();
        }

        // Method to refresh private messages for the main user
        private void RefreshPrivateMessages()
        {
            // Retrieve private messages for the main user from the server
            List<string> privateMessages = foob.GetPrivateMessages(mainUser);

            // Convert the list of messages to a format suitable for display in the ListView
            // For example, you could create a List of custom Message objects, where Message is a class you define
            // Message class might have properties like Sender, Recipient, and Text

            List<Message> displayMessages = privateMessages.Select(m => new Message(m)).ToList();

            // Set the ItemsSource of the privateChatFeed ListView
            privateChatFeed.ItemsSource = displayMessages;
        }

        // Assuming you define a Message class like this
        public class Message
        {
            public string Sender { get; set; }
            public string Text { get; set; }

            // Constructor to parse the raw message string and extract sender and text
            public Message(string rawMessage)
            {
                // Assuming the rawMessage is in a format like "Sender: Text"
                // Adjust the parsing according to the actual format of your messages
                int separatorIndex = rawMessage.IndexOf(":");
                if (separatorIndex >= 0)
                {
                    Sender = rawMessage.Substring(0, separatorIndex).Trim();
                    Text = rawMessage.Substring(separatorIndex + 1).Trim();
                }
                else
                {
                    // Handle the case where the message format is unexpected
                    Sender = "Unknown";
                    Text = rawMessage;
                }
            }
        }

        private void PrivateMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainChatGrid.Visibility == Visibility.Visible)
            {
                mainChatGrid.Visibility = Visibility.Hidden;
                privateChatGrid.Visibility = Visibility.Visible;
            }
            else
            {
                mainChatGrid.Visibility = Visibility.Visible;
                privateChatGrid.Visibility = Visibility.Hidden;
            }
        }

        private void PrivateRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Call the method to refresh private messages
            RefreshPrivateMessages();
        }

        private void BackToMainChatButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the privateChatGrid and show the mainChatGrid
            privateChatGrid.Visibility = Visibility.Hidden;
            mainChatGrid.Visibility = Visibility.Visible;
        }
//CHATROOM IMPLEMENTATION
        private void ChatRoomButton_Click(object sender, RoutedEventArgs e)
        {
            mainChatGrid.Visibility = Visibility.Hidden;
            privateChatGrid.Visibility = Visibility.Hidden;
            chatRoomGrid.Visibility = Visibility.Visible;
        }

        private void backFromChatRoomButton_Click(object sender, RoutedEventArgs e)
        {
            chatRoomGrid.Visibility = Visibility.Hidden;
            mainChatGrid.Visibility = Visibility.Visible;

            var selectedChatRoom = listBoxChatRooms.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedChatRoom))
            {
                try
                {
                    
                        foob.RemoveUserFromChatRoom(loginInput.Text, selectedChatRoom);
                        joinedChatRooms = "";
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error leaving chat room: " + ex.Message);
                }
            }
        }


        // Maintain a for the chat rooms the user has joined
        private string joinedChatRooms;

        private void buttonJoinChatRoom_Click(object sender, RoutedEventArgs e)
        {
            joinedChatRooms = listBoxChatRooms.SelectedItem as string;
            if (string.IsNullOrEmpty(joinedChatRooms))
            {
                MessageBox.Show("Please select a chat room to join.");
                return;
            }

            try
            {
                foob.AddUserToChatRoom(loginInput.Text, joinedChatRooms);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error joining chat room: " + ex.Message);
            }
        }

        private void buttonCreateChatRoom_Click(object sender, RoutedEventArgs e)
        {
            var chatRoomName = textBoxChatRoomName.Text;
            if (string.IsNullOrEmpty(chatRoomName))
            {
                MessageBox.Show("Please enter a name for the chat room.");
                return;
            }

            try
            {
                if (foob.CreateChatRoom(chatRoomName))
                {
                    MessageBox.Show("ChatRoom Created Successfully!");

                }
                else 
                {
                    MessageBox.Show("ChatRoom Not Created! Enter unique Name!");
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating chat room: " + ex.Message);
            }
        }

        private void RefreshChatRooms()
        {
            try
            {
                    var chatRooms = foob.GetAvailableChatRooms();
                    listBoxChatRooms.ItemsSource = chatRooms;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing chat rooms: " + ex.Message);
            }
        }

        private void buttonSendChatRoomMessage_Click(object sender, RoutedEventArgs e)
        {
            var message = textBoxMessage.Text;
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Please enter a message to send.");
                return;
            }

         
            if (string.IsNullOrEmpty(joinedChatRooms))
            {
                MessageBox.Show("Please Join a chat room to send a message.");
                return;
            }

            try
            {
                
                foob.SendChatRoomMessage(loginInput.Text, joinedChatRooms, message);
              //  UpdateMessageViewer(selectedChatRoom);
                textBoxMessage.Clear(); // Clear the TextBox


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending message: " + ex.Message);
            }
        }


        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedChatRoom = joinedChatRooms;
                // Refresh the list of chat rooms
                RefreshChatRooms();
                if(selectedChatRoom!= null) 
                {
                   UpdateMessageViewer(selectedChatRoom);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing data: " + ex.Message);
            }
        }

    public void UpdateMessageViewer(string chatRoomName)
        {
            // Get the updated list of messages from the service
            if (string.IsNullOrEmpty(chatRoomName)) 
            {
                chatRoomName = "";
            }
            List<string> messages = foob.GetChatRoomMessages(chatRoomName);

            // Dispatch UI update to the UI thread
            Dispatcher.Invoke(() =>
            {
                // Update the ItemsSource of the ListView
                chatRoomMessageList.ItemsSource = messages;
            });
      }


        private CancellationTokenSource cts;

        private void StartUpdatingChatRoomsAndUsers()
        {
            cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Refresh Chat Rooms
                        Dispatcher.Invoke(() => RefreshChatRooms());

                        // Similarly update available users here

                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show("Error refreshing data: " + ex.Message));
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), cts.Token); // Adjust the delay as necessary
                }
            }, cts.Token);
        }



        private void StartReceivingMessages()
        {
            cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(joinedChatRooms))
                        {
                            UpdateMessageViewer(joinedChatRooms);

                            // Similarly check for and update private messages here
                        }
                        else 
                        {
                            UpdateMessageViewer("");
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show("Error receiving messages: " + ex.Message));
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), cts.Token); // Adjust the delay as necessary
                }
            }, cts.Token);
        }







    }

}
